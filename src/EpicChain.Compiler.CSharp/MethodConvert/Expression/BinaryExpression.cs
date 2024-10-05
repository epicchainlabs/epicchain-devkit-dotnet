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
