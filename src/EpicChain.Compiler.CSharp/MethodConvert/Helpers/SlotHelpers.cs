// Copyright (C) 2021-2024 EpicChain Lab's
//
// The EpicChain.Compiler.CSharp is open-source software that is distributed under the widely recognized and permissive MIT License.
// This software is intended to provide developers with a powerful framework to create and deploy smart contracts on the EpicChain blockchain,
// and it is made freely available to all individuals and organizations. Whether you are building for personal, educational, or commercial
// purposes, you are welcome to utilize this framework with minimal restrictions, promoting the spirit of open innovation and collaborative
// development within the blockchain ecosystem.
//
// As a permissive license, the MIT License allows for broad usage rights, granting you the freedom to redistribute, modify, and adapt the
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

using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OpCode = EpicChain.VM.OpCode;

namespace EpicChain.Compiler;

internal partial class MethodConvert
{
    #region Variables

    /// <summary>
    /// Adds a local variable to the method's scope.
    /// </summary>
    /// <remarks>
    /// ILocalSymbols defined by the 'out' parameter will not be processed here
    /// but are considered as static fields.
    /// </remarks>
    /// <param name="symbol">The ILocalSymbol representing the local variable to be added.</param>
    /// <returns>The index of the newly added local variable.</returns>
    private byte AddLocalVariable(ILocalSymbol symbol)
    {
        var index = (byte)(_localVariables.Count + _anonymousVariables.Count);
        _variableSymbols.Add((symbol, index));
        _localVariables.Add(symbol, index);
        if (_localsCount < index + 1)
            _localsCount = index + 1;
        _blockSymbols.Peek().Add(symbol);
        return index;
    }

    private byte AddAnonymousVariable()
    {
        var index = (byte)(_localVariables.Count + _anonymousVariables.Count);
        _anonymousVariables.Add(index);
        if (_localsCount < index + 1)
            _localsCount = index + 1;
        return index;
    }

    private void RemoveAnonymousVariable(byte index)
    {
        if (_context.Options.Optimize.HasFlag(CompilationOptions.OptimizationType.Basic))
            _anonymousVariables.Remove(index);
    }

    private void RemoveLocalVariable(ILocalSymbol symbol)
    {
        if (_context.Options.Optimize.HasFlag(CompilationOptions.OptimizationType.Basic))
            _localVariables.Remove(symbol);
    }

    #endregion

    #region Helper

    /// <summary>
    /// Loads the value of a parameter onto the evaluation stack.
    /// </summary>
    /// <param name="parameter">The parameter symbol to load.</param>
    /// <param name="method">Optional method symbol, used for determining if it's a built-in type method.</param>
    /// <returns>An instruction representing the load operation.</returns>
    private Instruction LdArgSlot(IParameterSymbol parameter)
    {
        if (_context.TryGetCapturedStaticField(parameter, out var staticFieldIndex))
        {
            //using created static fields
            return AccessSlot(OpCode.LDSFLD, staticFieldIndex);
        }

        if ((Symbol.MethodKind == MethodKind.AnonymousFunction && !_parameters.ContainsKey(parameter)) ||
            (Symbol.MethodKind == MethodKind.Ordinary && parameter.RefKind == RefKind.Out))
        {
            //create static fields from captured parameter
            var staticIndex = _context.GetOrAddCapturedStaticField(parameter);
            CapturedLocalSymbols.Add(parameter);
            return AccessSlot(OpCode.LDSFLD, staticIndex);
        }
        // local parameter in current method
        var index = _parameters[parameter];
        return AccessSlot(OpCode.LDARG, index);
    }

    /// <summary>
    /// Stores the value from the evaluation stack to a parameter.
    /// </summary>
    /// <param name="parameter">The parameter symbol to store the value to.</param>
    /// <param name="method">Optional method symbol, used for determining if it's a built-in type method.</param>
    /// <returns>An instruction representing the store operation.</returns>
    private Instruction StArgSlot(IParameterSymbol parameter)
    {
        if (_context.TryGetCapturedStaticField(parameter, out var staticFieldIndex))
        {
            //using created static fields
            return AccessSlot(OpCode.STSFLD, staticFieldIndex);
        }

        if ((Symbol.MethodKind == MethodKind.AnonymousFunction && !_parameters.ContainsKey(parameter)) ||
            (Symbol.MethodKind == MethodKind.Ordinary && parameter.RefKind == RefKind.Out))
        {
            //create static fields from captured parameter
            var staticIndex = _context.GetOrAddCapturedStaticField(parameter);
            CapturedLocalSymbols.Add(parameter);
            return AccessSlot(OpCode.STSFLD, staticIndex);
        }
        // local parameter in current method
        var index = _parameters[parameter];
        return AccessSlot(OpCode.STARG, index);
    }

    /// <summary>
    /// Loads the value of a local variable onto the evaluation stack.
    /// </summary>
    /// <param name="local">The local variable symbol to load.</param>
    /// <param name="method">Optional method symbol, used for determining if it's a built-in type method.</param>
    /// <returns>An instruction representing the load operation.</returns>
    private Instruction LdLocSlot(ILocalSymbol local)
    {
        if (_context.TryGetCapturedStaticField(local, out var staticFieldIndex))
        {
            //using created static fields
            return AccessSlot(OpCode.LDSFLD, staticFieldIndex);
        }

        if ((Symbol.MethodKind == MethodKind.AnonymousFunction && !_localVariables.ContainsKey(local)) ||
            (Symbol.MethodKind == MethodKind.Ordinary && local.RefKind == RefKind.Out))
        {
            //create static fields from captured local
            var staticIndex = _context.GetOrAddCapturedStaticField(local);
            CapturedLocalSymbols.Add(local);
            return AccessSlot(OpCode.LDSFLD, staticIndex);
        }
        // local variables in current method
        var index = _localVariables[local];
        return AccessSlot(OpCode.LDLOC, index);
    }

    /// <summary>
    /// Stores the value from the evaluation stack into a local variable.
    /// </summary>
    /// <param name="local">The local variable symbol to store the value into.</param>
    /// <param name="method">Optional method symbol, used for determining if it's a built-in type method.</param>
    /// <returns>An instruction representing the store operation.</returns>
    private Instruction StLocSlot(ILocalSymbol local)
    {
        if (_context.TryGetCapturedStaticField(local, out var staticFieldIndex))
        {
            //using created static fields
            return AccessSlot(OpCode.STSFLD, staticFieldIndex);
        }

        if ((Symbol.MethodKind == MethodKind.AnonymousFunction && !_localVariables.ContainsKey(local)) ||
            (Symbol.MethodKind == MethodKind.Ordinary && local.RefKind == RefKind.Out))
        {
            //create static fields from captured local
            var staticIndex = _context.GetOrAddCapturedStaticField(local);
            CapturedLocalSymbols.Add(local);
            return AccessSlot(OpCode.STSFLD, staticIndex);
        }
        var index = _localVariables[local];
        return AccessSlot(OpCode.STLOC, index);
    }

    private Instruction AccessSlot(OpCode opcode, byte index)
    {
        return index >= 7
            ? AddInstruction(new Instruction { OpCode = opcode, Operand = new[] { index } })
            : AddInstruction(opcode - 7 + index);
    }

    /// <summary>
    /// Prepares arguments for a method call, handling various parameter types and calling conventions.
    /// </summary>
    /// <param name="model">The semantic model used for analysis.</param>
    /// <param name="symbol">The method symbol being called.</param>
    /// <param name="arguments">The list of argument syntax nodes.</param>
    /// <param name="callingConvention">The calling convention to use (default is Cdecl).</param>
    /// <param name="isSysCall">Indicates whether the method is a system call.</param>
    /// <remarks>
    /// This method processes named arguments, determines parameter order based on calling convention,
    /// and handles regular, params, and out arguments. It also takes into account system calls for
    /// special processing of out parameters.
    /// </remarks>
    private void PrepareArgumentsForMethod(SemanticModel model, IMethodSymbol symbol, IReadOnlyList<SyntaxNode> arguments, CallingConvention callingConvention = CallingConvention.Cdecl)
    {
        // 1. Process named arguments
        var namedArguments = ProcessNamedArguments(model, arguments);

        // 2. Determine parameter order based on calling convention
        var parameters = DetermineParameterOrder(symbol, callingConvention);

        // 3. Process each parameter
        foreach (var parameter in parameters)
        {
            // a. Named Arguments
            // Example: MethodCall(paramName: value)
            if (TryProcessNamedArgument(model, namedArguments, parameter))
                continue;

            if (parameter.IsParams)
            {
                // b. Params Arguments
                // Example: MethodCall(1, 2, 3, 4, 5)
                // Where method signature is: void MethodCall(params int[] numbers)
                ProcessParamsArgument(model, arguments, parameter);
            }
            else if (parameter.RefKind == RefKind.Out)
            {
                // c. Out Arguments
                // Example: MethodCall(Out value)
                // Where method signature is: void MethodCall(Out int value)
                ProcessOutArgument(model, symbol, arguments, parameter);
            }
            else
            {
                // d. Regular Arguments
                // Example: MethodCall(42, "Hello")
                // Where method signature is: void MethodCall(int num, string message)
                ProcessRegularArgument(model, arguments, parameter);
            }
        }
    }

    // Helper methods
    private static Dictionary<IParameterSymbol, ExpressionSyntax> ProcessNamedArguments(SemanticModel model, IReadOnlyList<SyntaxNode> arguments)
    {
        // NameColon is not null means the argument is a named argument
        // so we need to get the parameter symbol and the expression
        // and put them into a dictionary
        return arguments.OfType<ArgumentSyntax>()
             .Where(p => p.NameColon is not null)
             .Select(p => (Symbol: (IParameterSymbol)ModelExtensions.GetSymbolInfo(model, p.NameColon!.Name).Symbol!, p.Expression))
             .ToDictionary(p =>
                 p.Symbol,
                 p => p.Expression, (IEqualityComparer<IParameterSymbol>)SymbolEqualityComparer.Default);
    }

    private static IEnumerable<IParameterSymbol> DetermineParameterOrder(IMethodSymbol symbol, CallingConvention callingConvention)
    {
        return callingConvention == CallingConvention.Cdecl ? symbol.Parameters.Reverse() : symbol.Parameters;
    }

    private bool TryProcessNamedArgument(SemanticModel model, Dictionary<IParameterSymbol, ExpressionSyntax> namedArguments, IParameterSymbol parameter)
    {
        if (!namedArguments.TryGetValue(parameter, out var expression)) return false;
        ConvertExpression(model, expression);
        return true;
    }

    private void ProcessParamsArgument(SemanticModel model, IReadOnlyList<SyntaxNode> arguments, IParameterSymbol parameter)
    {
        if (arguments.Count > parameter.Ordinal)
        {
            if (TryProcessSingleParamsArgument(model, arguments, parameter))
                return;

            ProcessMultipleParamsArguments(model, arguments, parameter);
        }
        else
        {
            AddInstruction(OpCode.NEWARRAY0);
        }
    }

    private bool TryProcessSingleParamsArgument(SemanticModel model, IReadOnlyList<SyntaxNode> arguments, IParameterSymbol parameter)
    {
        if (arguments.Count != parameter.Ordinal + 1) return false;
        var expression = ExtractExpression(arguments[parameter.Ordinal]);
        var conversion = model.ClassifyConversion(expression, parameter.Type);
        if (!conversion.Exists) return false;
        ConvertExpression(model, expression);
        return true;
    }

    private void ProcessMultipleParamsArguments(SemanticModel model, IReadOnlyList<SyntaxNode> arguments, IParameterSymbol parameter)
    {
        for (int i = arguments.Count - 1; i >= parameter.Ordinal; i--)
        {
            var expression = ExtractExpression(arguments[i]);
            ConvertExpression(model, expression);
        }
        Push(arguments.Count - parameter.Ordinal);
        AddInstruction(OpCode.PACK);
    }

    private void ProcessOutArgument(SemanticModel model, IMethodSymbol methodSymbol, IReadOnlyList<SyntaxNode> arguments, IParameterSymbol parameter)
    {
        try
        {
            LdArgSlot(parameter);
        }
        catch
        {
            // check if the argument is a discard
            var argument = arguments[parameter.Ordinal] as ArgumentSyntax;
            if (argument.Expression is not IdentifierNameSyntax { Identifier.ValueText: "_" })
                throw new CompilationException(arguments[parameter.Ordinal], DiagnosticId.SyntaxNotSupported,
                    $"In method {Symbol.Name}, unsupported out argument: {arguments[parameter.Ordinal]}");
            LdArgSlot(parameter);
        }
    }

    private void ProcessRegularArgument(SemanticModel model, IReadOnlyList<SyntaxNode> arguments, IParameterSymbol parameter)
    {
        if (arguments.Count > parameter.Ordinal)
        {
            var argument = arguments[parameter.Ordinal];
            switch (argument)
            {
                case ArgumentSyntax { NameColon: null } arg:
                    ConvertExpression(model, arg.Expression);
                    return;
                case ExpressionSyntax ex:
                    ConvertExpression(model, ex);
                    return;
                default:
                    throw new CompilationException(argument, DiagnosticId.SyntaxNotSupported, $"Unsupported argument: {argument}");
            }
        }
        Push(parameter.ExplicitDefaultValue);
    }

    private static ExpressionSyntax ExtractExpression(SyntaxNode node)
    {
        return node switch
        {
            ArgumentSyntax argument => argument.Expression,
            ExpressionSyntax exp => exp,
            _ => throw new CompilationException(node, DiagnosticId.SyntaxNotSupported, $"Unsupported argument: {node}"),
        };
    }
    #endregion
}
