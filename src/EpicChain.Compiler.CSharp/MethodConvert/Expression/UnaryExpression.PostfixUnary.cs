// Copyright (C) 2021-2024 EpicChain Labs.
//
// The EpicChain.Compiler.CSharp is open-source software made available under the MIT License.
// This permissive license allows anyone to freely use, modify, and distribute both the source code
// and binary forms of the software, either with or without modifications, provided that the conditions
// specified in the license are met. For more information about the MIT License, you can refer to the
// LICENSE file located in the main directory of the project, or visit the following link:
// http://www.opensource.org/licenses/mit-license.php.
//
// The key permissions granted by the MIT License include the following:
// 1. The right to use the software for any purpose, including commercial applications.
// 2. The right to modify the source code to suit individual needs.
// 3. The right to distribute copies of the original or modified versions of the software.
//
// Redistribution and use in both source and binary forms are permitted, provided that the following
// conditions are met:
//
// 1. The original copyright notice and permission notice must be included in all copies or substantial
//    portions of the software, whether the distribution is of the unmodified source code or modified
//    versions.
// 2. The software is provided "as is," without any warranty of any kind, express or implied, including
//    but not limited to the warranties of merchantability, fitness for a particular purpose, or
//    non-infringement. In no event shall the authors or copyright holders be liable for any claim,
//    damages, or other liabilities, whether in an action of contract, tort, or otherwise, arising from,
//    out of, or in connection with the software or the use or other dealings in the software.
//
// The MIT License is widely used for open-source projects because of its flexibility, encouraging both
// individual and corporate use of the licensed software without restrictive obligations. If you wish to
// learn more about this license or its implications for the project, please consult the official page
// provided above.


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
