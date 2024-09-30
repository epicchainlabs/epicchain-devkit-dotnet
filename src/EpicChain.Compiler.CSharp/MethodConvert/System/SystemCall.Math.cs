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


using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using EpicChain.VM;

namespace EpicChain.Compiler;

internal partial class MethodConvert
{
    // Handler for Math.Abs methods
    private static void HandleMathAbs(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.ABS);
    }

    // Handler for Math.Sign methods
    private static void HandleMathSign(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.SIGN);
    }

    // Handler for Math.Max methods
    private static void HandleMathMax(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.MAX);
    }

    // Handler for Math.Min methods
    private static void HandleMathMin(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.MIN);
    }

    private static void HandleMathByteDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleMathBigIntegerDivRem(methodConvert, model, symbol, instanceExpression, arguments);
    }

    private static void HandleMathSByteDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleMathBigIntegerDivRem(methodConvert, model, symbol, instanceExpression, arguments);
    }

    private static void HandleMathShortDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleMathBigIntegerDivRem(methodConvert, model, symbol, instanceExpression, arguments);
    }

    private static void HandleMathUShortDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleMathBigIntegerDivRem(methodConvert, model, symbol, instanceExpression, arguments);
    }

    private static void HandleMathIntDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleMathBigIntegerDivRem(methodConvert, model, symbol, instanceExpression, arguments);
    }

    private static void HandleMathUIntDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleMathBigIntegerDivRem(methodConvert, model, symbol, instanceExpression, arguments);
    }

    private static void HandleMathLongDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleMathBigIntegerDivRem(methodConvert, model, symbol, instanceExpression, arguments);
    }

    private static void HandleMathULongDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleMathBigIntegerDivRem(methodConvert, model, symbol, instanceExpression, arguments);
    }

    private static void HandleMathClamp(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);

        var exceptionTarget = new JumpTarget();
        // Evaluation stack: value=5 min=0 max=10 <- top
        methodConvert.AddInstruction(OpCode.OVER);  // 5 0 10 0
        methodConvert.AddInstruction(OpCode.OVER);  // 5 0 10 0 10 <- top
        methodConvert.Jump(OpCode.JMPLE, exceptionTarget);  // 5 0 10  // if 0 <= 10, continue execution
        //methodConvert.Push("min>max");
        methodConvert.AddInstruction(OpCode.THROW);
        exceptionTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.REVERSE3);  // 10 0 5
        // MAX&MIN costs 1<<3 each; 16 Datoshi more expensive at runtime
        methodConvert.AddInstruction(OpCode.MAX);  // 10 5
        methodConvert.AddInstruction(OpCode.MIN);  // 5
        //methodConvert.AddInstruction(OpCode.RET);
        // Alternatively, a slightly cheaper way at runtime; 10 to 16 Datoshi
        //methodConvert.AddInstruction(OpCode.OVER);  // 10 0 5 0
        //methodConvert.AddInstruction(OpCode.OVER);  // 10 0 5 0 5
        //methodConvert.Jump(OpCode.JMPGE, minTarget);  // 10 0 5; should return 0 if JMPed
        //methodConvert.AddInstruction(OpCode.NIP);  // 10 5
        //methodConvert.AddInstruction(OpCode.OVER);  // 10 5 10
        //methodConvert.AddInstruction(OpCode.OVER);  // 10 5 10 5
        //methodConvert.Jump(OpCode.JMPLE, maxTarget);  // 10 5; should return 10 if JMPed
        //methodConvert.AddInstruction(OpCode.NIP);  // 5; should return 5
        //methodConvert.AddInstruction(OpCode.RET);
        //minTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);  // 10 0 5; should return 0
        //methodConvert.AddInstruction(OpCode.DROP);  // 10 0; should return 0
        //methodConvert.AddInstruction(OpCode.NIP);  // 0; should return 0
        //methodConvert.AddInstruction(OpCode.RET);
        //maxTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);  // 10 5; should return 10
        //methodConvert.AddInstruction(OpCode.DROP);  // 10; should return 10
        //methodConvert.AddInstruction(OpCode.RET);
    }

    private static void HandleMathBigMul(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        methodConvert.AddInstruction(OpCode.MUL);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push(long.MinValue);
        methodConvert.Push(new BigInteger(long.MaxValue) + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    // RegisterHandler((double x) => Math.Ceiling(x), HandleMathCeiling);
    // RegisterHandler((double x) => Math.Floor(x), HandleMathFloor);
    // RegisterHandler((double x) => Math.Round(x), HandleMathRound);
    // RegisterHandler((double x) => Math.Truncate(x), HandleMathTruncate);
    // RegisterHandler((double x, double y) => Math.Pow(x, y), HandleMathPow);
    // RegisterHandler((double x) => Math.Sqrt(x), HandleMathSqrt);
    // RegisterHandler((double x) => Math.Log(x), HandleMathLog);
    // RegisterHandler((double x, double y) => Math.Log(x, y), HandleMathLogBase);
}
