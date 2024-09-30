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

namespace EpicChain.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// The conditional logical OR operator ||, also known as the "short-circuiting" logical OR operator, computes the logical OR of its operands.
    /// The result of x || y is true if either x or y evaluates to true.
    /// Otherwise, the result is false. If x evaluates to true, y isn't evaluated.
    ///
    /// The conditional logical AND operator &&, also known as the "short-circuiting" logical AND operator, computes the logical AND of its operands.
    /// The result of x && y is true if both x and y evaluate to true.
    /// Otherwise, the result is false. If x evaluates to false, y isn't evaluated.
    ///
    /// The is operator checks if the run-time type of an expression result is compatible with a given type. The is operator also tests an expression result against a pattern.
    ///
    /// The as operator explicitly converts the result of an expression to a given reference or nullable value type. If the conversion isn't possible, the as operator returns null. Unlike a cast expression, the as operator never throws an exception.
    ///
    /// The null-coalescing operator ?? returns the value of its left-hand operand if it isn't null;
    /// otherwise, it evaluates the right-hand operand and returns its result.
    /// The ?? operator doesn't evaluate its right-hand operand if the left-hand operand evaluates to non-null.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about binary expression.</param>
    /// <param name="expression">The syntax representation of the binary expression statement being converted.</param>
    /// <exception cref="CompilationException">If an unsupported operator is encountered</exception>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/boolean-logical-operators">Boolean logical operators - AND, OR</seealso>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/type-testing-and-cast">Type-testing operators and cast expressions - is, as</seealso>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-coalescing-operator">?? operators - the null-coalescing operators</seealso>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/bitwise-and-shift-operators">Bitwise and shift operators</seealso>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/arithmetic-operators">Arithmetic operators</seealso>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/boolean-logical-operators">Boolean logical operators - AND, OR, NOT, XOR</seealso>
    private void ConvertBinaryExpression(SemanticModel model, BinaryExpressionSyntax expression)
    {
        switch (expression.OperatorToken.ValueText)
        {
            case "||":
                ConvertLogicalOrExpression(model, expression.Left, expression.Right);
                return;
            case "&&":
                ConvertLogicalAndExpression(model, expression.Left, expression.Right);
                return;
            case "is":
                ConvertIsExpression(model, expression.Left, expression.Right);
                return;
            case "as":
                ConvertAsExpression(model, expression.Left, expression.Right);
                return;
            case "??":
                ConvertCoalesceExpression(model, expression.Left, expression.Right);
                return;
        }
        IMethodSymbol? symbol = (IMethodSymbol?)model.GetSymbolInfo(expression).Symbol;
        if (symbol is not null && TryProcessSystemOperators(model, symbol, expression.Left, expression.Right))
            return;
        ConvertExpression(model, expression.Left);
        ConvertExpression(model, expression.Right);
        var (opcode, checkResult) = expression.OperatorToken.ValueText switch
        {
            "+" => (OpCode.ADD, true),
            "-" => (OpCode.SUB, true),
            "*" => (OpCode.MUL, true),
            "/" => (OpCode.DIV, false),
            "%" => (OpCode.MOD, false),
            "<<" => (OpCode.SHL, true),
            ">>" => (OpCode.SHR, false),
            "|" => (OpCode.OR, false),
            "&" => (OpCode.AND, false),
            "^" => (OpCode.XOR, false),
            "==" => (OpCode.EQUAL, false),
            "!=" => (OpCode.NOTEQUAL, false),
            "<" => (OpCode.LT, false),
            "<=" => (OpCode.LE, false),
            ">" => (OpCode.GT, false),
            ">=" => (OpCode.GE, false),
            _ => throw new CompilationException(expression.OperatorToken, DiagnosticId.SyntaxNotSupported, $"Unsupported operator: {expression.OperatorToken}")
        };
        AddInstruction(opcode);
        if (checkResult)
        {
            ITypeSymbol type = model.GetTypeInfo(expression).Type!;
            EnsureIntegerInRange(type);
        }
    }

    private void ConvertLogicalOrExpression(SemanticModel model, ExpressionSyntax left, ExpressionSyntax right)
    {
        JumpTarget rightTarget = new();
        JumpTarget endTarget = new();
        ConvertExpression(model, left);
        Jump(OpCode.JMPIFNOT_L, rightTarget);
        Push(true);
        Jump(OpCode.JMP_L, endTarget);
        rightTarget.Instruction = AddInstruction(OpCode.NOP);
        ConvertExpression(model, right);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }

    private void ConvertLogicalAndExpression(SemanticModel model, ExpressionSyntax left, ExpressionSyntax right)
    {
        JumpTarget rightTarget = new();
        JumpTarget endTarget = new();
        ConvertExpression(model, left);
        Jump(OpCode.JMPIF_L, rightTarget);
        Push(false);
        Jump(OpCode.JMP_L, endTarget);
        rightTarget.Instruction = AddInstruction(OpCode.NOP);
        ConvertExpression(model, right);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }

    private void ConvertIsExpression(SemanticModel model, ExpressionSyntax left, ExpressionSyntax right)
    {
        ITypeSymbol type = model.GetTypeInfo(right).Type!;
        ConvertExpression(model, left);
        IsType(type.GetPatternType());
    }

    private void ConvertAsExpression(SemanticModel model, ExpressionSyntax left, ExpressionSyntax right)
    {
        JumpTarget endTarget = new();
        ITypeSymbol type = model.GetTypeInfo(right).Type!;
        ConvertExpression(model, left);
        AddInstruction(OpCode.DUP);
        IsType(type.GetPatternType());
        Jump(OpCode.JMPIF_L, endTarget);
        AddInstruction(OpCode.DROP);
        Push((object?)null);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }

    private void ConvertCoalesceExpression(SemanticModel model, ExpressionSyntax left, ExpressionSyntax right)
    {
        JumpTarget endTarget = new();
        ConvertExpression(model, left);
        AddInstruction(OpCode.DUP);
        AddInstruction(OpCode.ISNULL);
        Jump(OpCode.JMPIFNOT_L, endTarget);
        AddInstruction(OpCode.DROP);
        ConvertExpression(model, right);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }
}
