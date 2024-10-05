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

using Akka.Util.Internal;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using EpicChain.IO;
using EpicChain.SmartContract;
using EpicChain.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.CSharp;

namespace EpicChain.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// Creates an instruction to call an interop method using the given descriptor.
    /// </summary>
    /// <param name="descriptor">The interop descriptor representing the method to call.</param>
    /// <returns>The instruction to perform the interop call.</returns>
    private Instruction CallInteropMethod(InteropDescriptor descriptor)
        => AddInstruction(new Instruction
        {
            OpCode = OpCode.SYSCALL,
            Operand = BitConverter.GetBytes(descriptor)
        });

    /// <summary>
    /// Creates an instruction to call a method in another smart contract identified by its hash.
    /// </summary>
    /// <param name="hash">The hash of the contract containing the method.</param>
    /// <param name="method">The name of the method to call.</param>
    /// <param name="parametersCount">The number of parameters the method takes.</param>
    /// <param name="hasReturnValue">Whether the method returns a value.</param>
    /// <param name="callFlags">The call flags to use for the method call.</param>
    /// <returns>The instruction to perform the contract method call.</returns>
    private Instruction CallContractMethod(UInt160 hash, string method, ushort parametersCount, bool hasReturnValue, CallFlags callFlags = CallFlags.All)
    {
        ushort token = _context.AddMethodToken(hash, method, parametersCount, hasReturnValue, callFlags);
        return AddInstruction(new Instruction
        {
            OpCode = OpCode.CALLT,
            Operand = BitConverter.GetBytes(token)
        });
    }

    /// <summary>
    /// Creates instructions to call an instance method, handling the instance on the stack and preparing the arguments.
    /// </summary>
    /// <param name="model">The semantic model of the code.</param>
    /// <param name="symbol">The method symbol representing the method to call.</param>
    /// <param name="instanceOnStack">Whether the instance is on the stack.</param>
    /// <param name="arguments">The list of arguments for the method call.</param>
    private void CallInstanceMethod(SemanticModel model, IMethodSymbol symbol, bool instanceOnStack, IReadOnlyList<ArgumentSyntax> arguments)
    {
        ProcessOutParameters(model, symbol, arguments);

        if (TryProcessSpecialMethods(model, symbol, null, arguments))
            return;

        var (convert, methodCallingConvention) = GetMethodConvertAndCallingConvention(model, symbol);

        HandleConstructorDuplication(instanceOnStack, methodCallingConvention, symbol);

        PrepareArgumentsForMethod(model, symbol, arguments, methodCallingConvention);

        HandleInstanceOnStack(symbol, instanceOnStack, methodCallingConvention);

        EmitMethodCall(convert, symbol);
    }

    /// <summary>
    /// Creates instructions to call a method with an optional instance expression, handling both instance and static methods.
    /// </summary>
    /// <param name="model">The semantic model of the code.</param>
    /// <param name="symbol">The method symbol representing the method to call.</param>
    /// <param name="instanceExpression">The optional instance expression for the method call.</param>
    /// <param name="arguments">The list of arguments for the method call.</param>
    private void CallMethodWithInstanceExpression(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, params SyntaxNode[] arguments)
    {
        ProcessOutParameters(model, symbol, arguments);

        if (TryProcessSpecialMethods(model, symbol, instanceExpression, arguments))
            return;

        var (convert, methodCallingConvention) = GetMethodConvertAndCallingConvention(model, symbol, instanceExpression);

        HandleInstanceExpression(model, symbol, instanceExpression, methodCallingConvention, beforeArguments: true);

        PrepareArgumentsForMethod(model, symbol, arguments, methodCallingConvention);

        HandleInstanceExpression(model, symbol, instanceExpression, methodCallingConvention, beforeArguments: false);

        EmitMethodCall(convert, symbol);
    }

    /// <summary>
    /// Creates instructions to call a method with a specified calling convention.
    /// </summary>
    /// <param name="model">The semantic model of the code.</param>
    /// <param name="symbol">The method symbol representing the method to call.</param>
    /// <param name="callingConvention">The calling convention to use for the method call.</param>
    private void CallMethodWithConvention(SemanticModel model, IMethodSymbol symbol, CallingConvention callingConvention = CallingConvention.Cdecl)
    {
        if (TryProcessSystemMethods(model, symbol, null, null) || TryProcessInlineMethods(model, symbol, null))
            return;

        var (convert, methodCallingConvention) = GetMethodConvertAndCallingConvention(model, symbol);

        int pc = symbol.Parameters.Length + (symbol.IsStatic ? 0 : 1);
        if (pc > 1 && methodCallingConvention != callingConvention)
            ReverseStackItems(pc);

        if (convert is null)
            CallVirtual(symbol);
        else
            EmitCall(convert);

        var parameters = symbol.Parameters;
        parameters.Where(p => _context.OutStaticFieldsSync.ContainsKey(p)).ForEach(p =>
        {
            foreach (var sync in _context.OutStaticFieldsSync[p])
            {
                LdArgSlot(p);
                switch (sync)
                {
                    case IParameterSymbol param:
                        StArgSlot(param);
                        break;
                    case ILocalSymbol local:
                        StLocSlot(local);
                        break;
                    default:
                        throw new CompilationException(sync, DiagnosticId.SyntaxNotSupported, $"Unsupported syntax: {sync}");
                }
            }
        });
    }

    private bool TryProcessSpecialMethods(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode> arguments)
    {
        return TryProcessSystemMethods(model, symbol, instanceExpression, arguments) ||
               TryProcessInlineMethods(model, symbol, arguments);
    }

    private void ProcessOutParameters(SemanticModel model, IMethodSymbol symbol, IEnumerable<SyntaxNode> arguments)
    {
        var parameters = DetermineParameterOrder(symbol, CallingConvention.Cdecl);
        foreach (var parameter in parameters.Where(p => p.RefKind == RefKind.Out))
        {
            if (arguments.ElementAtOrDefault(parameter.Ordinal) is not ArgumentSyntax argument) continue;

            ProcessOutArgument(model, symbol, parameter, argument);
        }
    }

    private void ProcessOutArgument(SemanticModel model, IMethodSymbol methodSymbol, IParameterSymbol parameter, ArgumentSyntax argument)
    {
        switch (argument.Expression)
        {
            case DeclarationExpressionSyntax { Designation: SingleVariableDesignationSyntax designation }:
                ProcessOutDeclaration(model, methodSymbol, parameter, designation);
                break;
            case IdentifierNameSyntax identifierName:
                ProcessOutIdentifier(model, parameter, identifierName);
                break;
            default:
                throw new CompilationException(argument, DiagnosticId.SyntaxNotSupported, $"Unsupported syntax: {argument}");
        }
    }

    private void ProcessOutDeclaration(SemanticModel model, IMethodSymbol methodSymbol, IParameterSymbol parameter, SingleVariableDesignationSyntax designation)
    {
        var local = (ILocalSymbol)model.GetDeclaredSymbol(designation)!;
        ProcessOutSymbol(parameter, local);
        PushDefault(local.Type);
        StLocSlot(local); // initialize the local variable with default value
    }

    private void ProcessOutIdentifier(SemanticModel model, IParameterSymbol parameter, IdentifierNameSyntax identifierName)
    {
        var symbol = model.GetSymbolInfo(identifierName).Symbol!;
        switch (symbol)
        {
            case ILocalSymbol local:
                LdLocSlot(local);
                ProcessOutSymbol(parameter, local);
                StLocSlot(local);
                break;
            case IParameterSymbol param:
                LdArgSlot(param);
                ProcessOutSymbol(parameter, param);
                StArgSlot(param);
                break;
            case IDiscardSymbol:
                PushDefault(parameter.Type);
                _context.GetOrAddCapturedStaticField(parameter);
                StArgSlot(parameter);
                break;
            default:
                throw new CompilationException(identifierName, DiagnosticId.SyntaxNotSupported, $"Unsupported syntax: {identifierName}");
        }
    }

    private void ProcessOutSymbol(IParameterSymbol parameter, ISymbol symbol)
    {
        bool parameterCaptured = _context.TryGetCapturedStaticField(parameter, out var parameterIndex);
        bool symbolCaptured = _context.TryGetCapturedStaticField(symbol, out var symbolIndex);

        if (parameterCaptured && !symbolCaptured)
        {
            _context.AssociateCapturedStaticField(symbol, parameterIndex);
        }
        else if (!parameterCaptured && symbolCaptured)
        {
            _context.AssociateCapturedStaticField(parameter, symbolIndex);
        }
        else if (parameterCaptured && symbolCaptured && parameterIndex != symbolIndex)
        {
            // both values are already captured in different indirectly connected methods,
            // but they are different, thus need to sync value from symbol to parameter
            if (!_context.OutStaticFieldsSync.TryGetValue(parameter, out var syncList))
            {
                syncList = new List<ISymbol>();
                _context.OutStaticFieldsSync[parameter] = syncList;
            }
            syncList.Add(symbol);
        }
        else if (!parameterCaptured && !symbolCaptured)
        {
            var index = _context.GetOrAddCapturedStaticField(symbol);
            _context.AssociateCapturedStaticField(parameter, index);
        }
    }

    private void HandleConstructorDuplication(bool instanceOnStack, CallingConvention methodCallingConvention, IMethodSymbol symbol)
    {
        if (instanceOnStack && methodCallingConvention != CallingConvention.Cdecl && symbol.MethodKind == MethodKind.Constructor)
            AddInstruction(OpCode.DUP);
    }

    private void HandleInstanceExpression(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
                                        CallingConvention methodCallingConvention, bool beforeArguments)
    {
        if (symbol.IsStatic) return;

        bool shouldConvert = beforeArguments ? (methodCallingConvention != CallingConvention.Cdecl)
                                             : (methodCallingConvention == CallingConvention.Cdecl);

        if (shouldConvert)
            ConvertInstanceExpression(model, instanceExpression);
    }

    private void EmitMethodCall(MethodConvert? convert, IMethodSymbol symbol)
    {
        if (convert is null)
            CallVirtual(symbol);
        else
            EmitCall(convert);

        var parameters = symbol.Parameters;
        parameters.Where(p => _context.OutStaticFieldsSync.ContainsKey(p)).ForEach(p =>
        {
            foreach (var sync in _context.OutStaticFieldsSync[p])
            {
                LdArgSlot(p);
                switch (sync)
                {
                    case IParameterSymbol param:
                        StArgSlot(param);
                        break;
                    case ILocalSymbol local:
                        StLocSlot(local);
                        break;
                    default:
                        throw new CompilationException(sync, DiagnosticId.SyntaxNotSupported, $"Unsupported syntax: {sync}");
                }
            }
        });
    }

    // Helper method to get MethodConvert and CallingConvention
    private (MethodConvert? convert, CallingConvention methodCallingConvention) GetMethodConvertAndCallingConvention(SemanticModel model, IMethodSymbol symbol)
    {
        if (symbol.IsVirtualMethod())
            return (null, CallingConvention.Cdecl);

        var convert = _context.ConvertMethod(model, symbol);
        return (convert, convert._callingConvention);
    }

    // Helper method to handle instance on stack
    private void HandleInstanceOnStack(IMethodSymbol symbol, bool instanceOnStack, CallingConvention methodCallingConvention)
    {
        if (!instanceOnStack || methodCallingConvention != CallingConvention.Cdecl)
            return;

        bool isConstructor = symbol.MethodKind == MethodKind.Constructor;
        switch (symbol.Parameters.Length)
        {
            case 0:
                if (isConstructor) AddInstruction(OpCode.DUP);
                break;
            case 1:
                AddInstruction(isConstructor ? OpCode.OVER : OpCode.SWAP);
                break;
            default:
                Push(symbol.Parameters.Length);
                AddInstruction(isConstructor ? OpCode.PICK : OpCode.ROLL);
                break;
        }
    }

    // Helper method to get MethodConvert and CallingConvention for instance expression
    private (MethodConvert? convert, CallingConvention methodCallingConvention) GetMethodConvertAndCallingConvention(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression)
    {
        if (symbol.IsVirtualMethod() && instanceExpression is not BaseExpressionSyntax)
            return (null, CallingConvention.Cdecl);

        var convert = symbol.ReducedFrom is null
            ? _context.ConvertMethod(model, symbol)
            : _context.ConvertMethod(model, symbol.ReducedFrom);
        return (convert, convert._callingConvention);
    }

    // Helper method to convert instance expression
    private void ConvertInstanceExpression(SemanticModel model, ExpressionSyntax? instanceExpression)
    {
        if (instanceExpression is null)
            AddInstruction(OpCode.LDARG0);
        else
            ConvertExpression(model, instanceExpression);
    }

    private void EmitCall(MethodConvert target)
    {
