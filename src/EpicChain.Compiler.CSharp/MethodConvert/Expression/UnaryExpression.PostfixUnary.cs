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

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using EpicChain.VM;
using System;
using System.Runtime.InteropServices;

namespace EpicChain.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// Converts the postfix operator into OpCodes.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about the postfix operator.</param>
    /// <param name="expression">The syntax representation of the postfix operator being converted.</param>
    /// <example>
    /// The result of x++ is the value of x before the operation, as the following example shows:
    /// <code>
    /// int i = 3;
    /// Runtime.Log(i.ToString());
    /// Runtime.Log(i++.ToString());
    /// Runtime.Log(i.ToString());
    /// </code>
    /// output: 3、3、4
    /// </example>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/arithmetic-operators#postfix-increment-operator">Postfix increment operator</seealso>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-forgiving">! (null-forgiving) operator</seealso>
    private void ConvertPostfixUnaryExpression(SemanticModel model, PostfixUnaryExpressionSyntax expression)
    {
        switch (expression.OperatorToken.ValueText)
        {
            case "++":
            case "--":
                ConvertPostIncrementOrDecrementExpression(model, expression);
                break;
            case "!":
                ConvertExpression(model, expression.Operand);
                break;
            default:
                throw new CompilationException(expression.OperatorToken, DiagnosticId.SyntaxNotSupported, $"Unsupported operator: {expression.OperatorToken}");
        }
    }

    private void ConvertPostIncrementOrDecrementExpression(SemanticModel model, PostfixUnaryExpressionSyntax expression)
    {
        switch (expression.Operand)
        {
            case ElementAccessExpressionSyntax operand:
                ConvertElementAccessPostIncrementOrDecrementExpression(model, expression.OperatorToken, operand);
                break;
            case IdentifierNameSyntax operand:
                ConvertIdentifierNamePostIncrementOrDecrementExpression(model, expression.OperatorToken, operand);
                break;
            case MemberAccessExpressionSyntax operand:
                ConvertMemberAccessPostIncrementOrDecrementExpression(model, expression.OperatorToken, operand);
                break;
            default:
                throw new CompilationException(expression, DiagnosticId.SyntaxNotSupported, $"Unsupported postfix unary expression: {expression}");
        }
    }

    private void ConvertElementAccessPostIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, ElementAccessExpressionSyntax operand)
    {
        if (operand.ArgumentList.Arguments.Count != 1)
            throw new CompilationException(operand.ArgumentList, DiagnosticId.MultidimensionalArray, $"Unsupported array rank: {operand.ArgumentList.Arguments}");
        if (model.GetSymbolInfo(operand).Symbol is IPropertySymbol property)
        {
            ConvertExpression(model, operand.Expression);
            ConvertExpression(model, operand.ArgumentList.Arguments[0].Expression);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.OVER);
            CallMethodWithConvention(model, property.GetMethod!, CallingConvention.StdCall);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.REVERSE4);
            AddInstruction(OpCode.REVERSE3);
            EmitIncrementOrDecrement(operatorToken, property.Type);
            CallMethodWithConvention(model, property.SetMethod!, CallingConvention.StdCall);
        }
        else
        {
            ConvertExpression(model, operand.Expression);
            ConvertExpression(model, operand.ArgumentList.Arguments[0].Expression);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.PICKITEM);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.REVERSE4);
            AddInstruction(OpCode.REVERSE3);
            EmitIncrementOrDecrement(operatorToken, model.GetTypeInfo(operand).Type);
            AddInstruction(OpCode.SETITEM);
        }
    }

    private void ConvertIdentifierNamePostIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, IdentifierNameSyntax operand)
    {
        ISymbol symbol = model.GetSymbolInfo(operand).Symbol!;
        switch (symbol)
        {
            case IFieldSymbol field:
                ConvertFieldIdentifierNamePostIncrementOrDecrementExpression(operatorToken, field);
                break;
            case ILocalSymbol local:
                ConvertLocalIdentifierNamePostIncrementOrDecrementExpression(operatorToken, local);
                break;
            case IParameterSymbol parameter:
                ConvertParameterIdentifierNamePostIncrementOrDecrementExpression(operatorToken, parameter);
                break;
            case IPropertySymbol property:
                ConvertPropertyIdentifierNamePostIncrementOrDecrementExpression(model, operatorToken, property);
                break;
            default:
                throw new CompilationException(operand, DiagnosticId.SyntaxNotSupported, $"Unsupported symbol: {symbol}");
        }
    }

    private void ConvertFieldIdentifierNamePostIncrementOrDecrementExpression(SyntaxToken operatorToken, IFieldSymbol symbol)
    {
        if (symbol.IsStatic)
        {
            byte index = _context.AddStaticField(symbol);
            AccessSlot(OpCode.LDSFLD, index);
            AddInstruction(OpCode.DUP);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            AccessSlot(OpCode.STSFLD, index);
        }
        else
        {
            int index = Array.IndexOf(symbol.ContainingType.GetFields(), symbol);
            AddInstruction(OpCode.LDARG0);
            AddInstruction(OpCode.DUP);
            Push(index);
            AddInstruction(OpCode.PICKITEM);
            AddInstruction(OpCode.TUCK);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            Push(index);
            AddInstruction(OpCode.SWAP);
            AddInstruction(OpCode.SETITEM);
        }
    }

    private void ConvertLocalIdentifierNamePostIncrementOrDecrementExpression(SyntaxToken operatorToken, ILocalSymbol symbol)
    {
        LdLocSlot(symbol);
        AddInstruction(OpCode.DUP);
        EmitIncrementOrDecrement(operatorToken, symbol.Type);
        StLocSlot(symbol);
    }

    private void ConvertParameterIdentifierNamePostIncrementOrDecrementExpression(SyntaxToken operatorToken, IParameterSymbol symbol)
    {
        LdArgSlot(symbol);
        AddInstruction(OpCode.DUP);
        EmitIncrementOrDecrement(operatorToken, symbol.Type);
        StArgSlot(symbol);
    }

    private void ConvertPropertyIdentifierNamePostIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, IPropertySymbol symbol)
    {
        if (symbol.IsStatic)
        {
            CallMethodWithConvention(model, symbol.GetMethod!);
            AddInstruction(OpCode.DUP);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            CallMethodWithConvention(model, symbol.SetMethod!);
        }
        else
        {
            AddInstruction(OpCode.LDARG0);
            AddInstruction(OpCode.DUP);
            CallMethodWithConvention(model, symbol.GetMethod!);
            AddInstruction(OpCode.TUCK);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            CallMethodWithConvention(model, symbol.SetMethod!, CallingConvention.StdCall);
        }
    }

    private void ConvertMemberAccessPostIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, MemberAccessExpressionSyntax operand)
    {
        ISymbol symbol = model.GetSymbolInfo(operand).Symbol!;
        switch (symbol)
        {
            case IFieldSymbol field:
                ConvertFieldMemberAccessPostIncrementOrDecrementExpression(model, operatorToken, operand, field);
                break;
            case IPropertySymbol property:
                ConvertPropertyMemberAccessPostIncrementOrDecrementExpression(model, operatorToken, operand, property);
                break;
            default:
                throw new CompilationException(operand, DiagnosticId.SyntaxNotSupported, $"Unsupported symbol: {symbol}");
        }
    }

    private void ConvertFieldMemberAccessPostIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, MemberAccessExpressionSyntax operand, IFieldSymbol symbol)
    {
        if (symbol.IsStatic)
        {
            byte index = _context.AddStaticField(symbol);
            AccessSlot(OpCode.LDSFLD, index);
            AddInstruction(OpCode.DUP);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            AccessSlot(OpCode.STSFLD, index);
        }
        else
        {
            int index = Array.IndexOf(symbol.ContainingType.GetFields(), symbol);
            ConvertExpression(model, operand.Expression);
            AddInstruction(OpCode.DUP);
            Push(index);
            AddInstruction(OpCode.PICKITEM);
            AddInstruction(OpCode.TUCK);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            Push(index);
            AddInstruction(OpCode.SWAP);
            AddInstruction(OpCode.SETITEM);
        }
    }

    private void ConvertPropertyMemberAccessPostIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, MemberAccessExpressionSyntax operand, IPropertySymbol symbol)
    {
        if (symbol.IsStatic)
        {
            CallMethodWithConvention(model, symbol.GetMethod!);
            AddInstruction(OpCode.DUP);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            CallMethodWithConvention(model, symbol.SetMethod!);
        }
        else
        {
            ConvertExpression(model, operand.Expression);
            AddInstruction(OpCode.DUP);
            CallMethodWithConvention(model, symbol.GetMethod!);
            AddInstruction(OpCode.TUCK);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            CallMethodWithConvention(model, symbol.SetMethod!, CallingConvention.StdCall);
        }
    }
}
