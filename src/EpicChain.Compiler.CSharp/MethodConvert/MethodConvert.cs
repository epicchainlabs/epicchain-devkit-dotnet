// Copyright (C) 2021-2024 EpicChain Lab's
//
// The EpicChain.Compiler.CSharp  MIT License allows for broad usage rights, granting you the freedom to redistribute, modify, and adapt the
// source code or its binary versions as needed. You are permitted to incorporate the EpicChain Lab's Project into your own
// projects, whether for profit or non-profit, and may make changes to suit your specific needs. There is no requirement to make your
// modifications open-source, though doing so contributes to the overall growth of the open-source community.
//
// To comply with the terms of the MIT License, we kindly ask that you include a copy of the original copyright notice, this permission
// notice, and the license itself in all substantial portions of the software that you redistribute, whether in source code form or as
// compiled binaries. The purpose of this requirement is to ensure that future users and developers are aware of the origin of the software,
// the freedoms granted to them, and the limitations of liability that apply.
//
// The complete terms and conditions of the MIT License are documented in the LICENSE file that accompanies this project. This file can be
// found in the main directory of the source code repository. Alternatively, you may access the license text online at the following URL:
// http://www.opensource.org/licenses/mit-license.php. We encourage you to review these terms in detail to fully understand your rights
// and responsibilities when using this software.
//
// Redistribution and use of the EpicChain Lab's Project, whether in source or binary forms, with or without modification, are
// permitted as long as the following conditions are met:
//
// 1. The original copyright notice, along with this permission notice, must be retained in all copies or significant portions of the software.
// 2. The software is provided "as-is," without any express or implied warranty. This means that the authors and contributors are not
//    responsible for any issues that may arise from the use of the software, including but not limited to damages caused by defects or
//    performance issues. Users assume all responsibility for determining the suitability of the software for their specific needs.
//
// In addition to the above terms, the authors of the EpicChain Lab's Project encourage developers to explore and experiment
// with the framework's capabilities. Whether you are an individual developer, a startup, or a large organization, you are invited to
// leverage the power of blockchain technology to create decentralized applications, smart contracts, and more. We believe that by fostering
// a robust ecosystem of developers and contributors, we can help drive innovation in the blockchain space and unlock new opportunities
// for distributed ledger technology.
//
// However, please note that while the MIT License allows for modifications and redistribution, it does not imply endorsement of any
// derived works by the original authors. Therefore, if you significantly modify the EpicChain Lab's Project and redistribute it
// under your own brand or as part of a larger project, you must clearly indicate the changes you have made, and the original authors
// cannot be held liable for any issues resulting from your modifications.
//
// By choosing to use the EpicChain Lab's Project, you acknowledge that you have read and understood the terms of the MIT License.
// You agree to abide by these terms and recognize that this software is provided without warranty of any kind, express or implied, including
// but not limited to warranties of merchantability, fitness for a particular purpose, or non-infringement. Should any legal issues or
// disputes arise as a result of using this software, the authors and contributors disclaim all liability and responsibility.
//
// Finally, we encourage all users of the EpicChain Lab's Project to consider contributing back to the community. Whether through
// bug reports, feature suggestions, or code contributions, your involvement helps improve the framework for everyone. Open-source projects
// thrive when developers collaborate and share their knowledge, and we welcome your input as we continue to develop and refine the
// EpicChain ecosystem.


extern alias scfx;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using EpicChain.Compiler.Optimizer;
using EpicChain.Cryptography.ECC;
using EpicChain.IO;
using EpicChain.SmartContract;
using EpicChain.VM;
using EpicChain.Wallets;
using scfx::EpicChain.SmartContract.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

namespace EpicChain.Compiler
{
    internal partial class MethodConvert
    {
        #region Fields

        private readonly CompilationContext _context;
        private CallingConvention _callingConvention = CallingConvention.Cdecl;
        private bool _inline;
        private bool _internalInline;
        // _initSlot is a boolean flag that determines whether an INITSLOT instruction
        // should be added at the beginning of the method's bytecode.
        // It's used to allocate space for local variables and parameters in the EpicChain VM.
        // The _initSlot flag allows the compiler to avoid adding unnecessary INITSLOT
        // instructions for methods that don't need local variables or parameters.
        // It's typically set to false for inline methods or external method calls.
        // By using this flag, the compiler can efficiently manage stack space allocation
        // for method execution in the EpicChain VM, only allocating space when necessary.
        private bool _initSlot;
        private readonly Dictionary<IParameterSymbol, byte> _parameters = new(SymbolEqualityComparer.Default);
        private readonly List<(ILocalSymbol, byte)> _variableSymbols = new();
        private readonly Dictionary<ILocalSymbol, byte> _localVariables = new(SymbolEqualityComparer.Default);
        private readonly List<byte> _anonymousVariables = new();
        private int _localsCount;
        private readonly Stack<List<ILocalSymbol>> _blockSymbols = new();
        private readonly List<Instruction> _instructions = new();
        private readonly JumpTarget _startTarget = new();
        private readonly Dictionary<ILabelSymbol, JumpTarget> _labels = new(SymbolEqualityComparer.Default);
        private readonly Stack<JumpTarget> _continueTargets = new();
        private readonly Stack<JumpTarget> _breakTargets = new();
        private readonly JumpTarget _returnTarget = new();
        private readonly Stack<ExceptionHandling> _tryStack = new();
        private readonly Stack<byte> _exceptionStack = new();
        private readonly Stack<(SwitchLabelSyntax, JumpTarget)[]> _switchStack = new();
        private readonly Stack<bool> _checkedStack = new();

        #endregion

        #region Properties

        public IMethodSymbol Symbol { get; }
        public SyntaxNode? SyntaxNode { get; private set; }
        public IReadOnlyList<Instruction> Instructions => _instructions;
        public IReadOnlyList<(ILocalSymbol Symbol, byte SlotIndex)> Variables => _variableSymbols;
        public bool IsEmpty => _instructions.Count == 0
            || _instructions is [{ OpCode: OpCode.RET }]
            || _instructions is [{ OpCode: OpCode.INITSLOT }, { OpCode: OpCode.RET }];

        /// <summary>
        /// captured local variable/parameter symbols when converting current method
        /// </summary>
        public HashSet<ISymbol> CapturedLocalSymbols { get; } = new(SymbolEqualityComparer.Default);

        #endregion

        #region Constructors

        public MethodConvert(CompilationContext context, IMethodSymbol symbol)
        {
            this.Symbol = symbol;
            this._context = context;
            this._checkedStack.Push(context.Options.Checked);
        }
        #endregion

        #region Convert

        /// <summary>
        /// This method is responsible for converting a method into a EpicChain VM bytecode.
        /// </summary>
        /// <param name="model">The semantic model of the method</param>
        public void Convert(SemanticModel model)
        {
            // Step 1: Determine if the method is extern or empty
            // This checks if:
            // a) The method is marked as extern, e.g.:
            //    public static extern int ExternMethod(int arg);
            // b) The containing type (class) of the method has no declaring syntax references
            //    This can happen with partial classes or classes defined in other assemblies
            if (Symbol.IsExtern || Symbol.ContainingType.DeclaringSyntaxReferences.IsEmpty)
            {
                // Step 2a: Handle extern or empty methods
                // This path is taken for:
                // - Methods explicitly marked as extern (see example above)
                // - Methods in classes defined in other assemblies or partial classes, e.g.:
                //   public partial class MyClass
                //   {
                //       // This method might be defined in another file or assembly
                //       public void EmptyMethod();
                //   }
                if (Symbol.Name == "_initialize")
                {
                    // Special handling for _initialize method
                    // This method is used for contract initialization
                    ProcessStaticFields(model);
                    InsertStaticFieldInitialization();
                }
                else
                {
                    // Convert extern method
                    // This usually involves creating a stub or placeholder for the external implementation
                    ConvertExtern();
                }
            }
            else
            {
                // Step 2b: Handle regular methods
                // This path is taken for normal methods with implementations in the current compilation unit
                // These methods require full processing and conversion
                // Example of a regular method:
                // public class MyClass
                // {
                //     public int RegularMethod(int arg)
                //     {
                //         return arg * 2;
                //     }
                // }
                // Set syntax node if available
                if (!Symbol.DeclaringSyntaxReferences.IsEmpty)
                    SyntaxNode = Symbol.DeclaringSyntaxReferences[0].GetSyntax();

                // Step 3: Process method based on its kind
                // This handles special cases for constructors and static constructors
                // Examples:
                // public MyClass() { } // Constructor
                // static MyClass() { } // Static constructor
                InitializeFieldsBasedOnMethodKind(model);

                // Step 4: Validate method name
                // Ensures that method names starting with '_' are valid
                // Example of an invalid method name:
                // public void _invalidMethod() { } // This would throw an exception
                ValidateMethodName();

                // Step 5: Convert modifiers
                // process ModifierAttribute
                var modifiers = ConvertModifier(model).ToArray();

                // Step 6: Convert the main method body
                // This is where the actual method implementation is converted
                ConvertSource(model);

                // Step 7: Insert initialization instructions if needed
                // This includes initializing static fields and local variables
                // This might insert INITSLOT instruction to the beginning of the method
                // Example:
                // static int staticField = 10;
                // public void Method()
                // {
                //     int localVar = 5;
                //     // ... rest of the method
                // }
                InsertInitializationInstructions();

                // Step 8: Process modifiers (exit)
                // Handle any cleanup or additional instructions required by modifiers
                ProcessModifiersExit(model, modifiers);
            }

            // Step 9: Finalize the method
            // Add RET instruction to the end of the method
            // This ensures proper method termination and return instruction
            FinalizeMethod();

            // Step 10: Optimize the instructions if needed
            // Basic optimization to remove unnecessary NOP instructions
            if (_context.Options.Optimize.HasFlag(CompilationOptions.OptimizationType.Basic))
                BasicOptimizer.RemoveNops(_instructions);

            // Step 11: Set the start target
            // Mark the first instruction as the entry point of the method
            _startTarget.Instruction = _instructions[0];
        }

        public void ConvertForward(SemanticModel model, MethodConvert target)
        {
            INamedTypeSymbol type = Symbol.ContainingType;
            CreateObject(model, type, null);
            IMethodSymbol? constructor = type.InstanceConstructors.FirstOrDefault(p => p.Parameters.Length == 0)
                ?? throw new CompilationException(type, DiagnosticId.NoParameterlessConstructor, "The contract class requires a parameterless constructor.");
            CallInstanceMethod(model, constructor, true, Array.Empty<ArgumentSyntax>());
            _returnTarget.Instruction = Jump(OpCode.JMP_L, target._startTarget);
            _startTarget.Instruction = _instructions[0];
        }

        private void ProcessFieldInitializer(SemanticModel model, IFieldSymbol field, Action? preInitialize, Action? postInitialize)
        {
            AttributeData? initialValue = field.GetAttributes().FirstOrDefault(p => p.AttributeClass!.Name == nameof(InitialValueAttribute) || p.AttributeClass!.IsSubclassOf(nameof(InitialValueAttribute)));
            if (initialValue is null)
            {
                EqualsValueClauseSyntax? initializer = null;
                SyntaxNode syntaxNode;
                if (field.DeclaringSyntaxReferences.IsEmpty)
                {
                    // Special handling for string.Empty is required as it lacks an AssociatedSymbol.
                    // Without this check, the method would return prematurely, bypassing necessary processing.
                    if (field.ContainingType.ToString() == "string" && field.Name == "Empty")
                    {
                        preInitialize?.Invoke();
                        Push(string.Empty);
                        postInitialize?.Invoke();
                        return;
                    }

                    if (field.AssociatedSymbol is not IPropertySymbol property) return;
                    syntaxNode = property.DeclaringSyntaxReferences[0].GetSyntax();
                    if (syntaxNode is PropertyDeclarationSyntax syntax)
                    {
                        initializer = syntax.Initializer;
                    }
                }
                else
                {
                    VariableDeclaratorSyntax syntax = (VariableDeclaratorSyntax)field.DeclaringSyntaxReferences[0].GetSyntax();
                    syntaxNode = syntax;
                    initializer = syntax.Initializer;
                }
                if (initializer is null) return;
                model = model.Compilation.GetSemanticModel(syntaxNode.SyntaxTree);
                using (InsertSequencePoint(syntaxNode))
                {
                    preInitialize?.Invoke();
                    ConvertExpression(model, initializer.Value, syntaxNode);
                    postInitialize?.Invoke();
                }
            }
            else
            {
                preInitialize?.Invoke();
                string value = (string)initialValue.ConstructorArguments[0].Value!;
                var attributeName = initialValue.AttributeClass!.Name;
                ContractParameterType parameterType = attributeName switch
                {
                    nameof(InitialValueAttribute) => (ContractParameterType)initialValue.ConstructorArguments[1].Value!,
                    nameof(IntegerAttribute) => ContractParameterType.Integer,
                    nameof(Hash160Attribute) => ContractParameterType.Hash160,
                    nameof(PublicKeyAttribute) => ContractParameterType.PublicKey,
                    nameof(ByteArrayAttribute) => ContractParameterType.ByteArray,
                    nameof(StringAttribute) => ContractParameterType.String,
