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
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using EpicChain.SmartContract.Native;
using EpicChain.VM;

namespace EpicChain.Compiler;

internal partial class MethodConvert
{
    private static void HandleCharParse(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push(char.MinValue);
        methodConvert.Push(char.MaxValue + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    // Handler for equality methods (Equals)
    private static void HandleEquals(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.NUMEQUAL);
    }

    // Handler for Array.Length and string.Length properties
    private static void HandleLength(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.SIZE);
    }

    private static void HandleCharIsDigit(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Push((ushort)'0');
        methodConvert.Push((ushort)'9' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
    }

    private static void HandleCharIsLetter(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)'A');
        methodConvert.Push((ushort)'Z' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.Push((ushort)'a');
        methodConvert.Push((ushort)'z' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.AddInstruction(OpCode.BOOLOR);
    }

    private static void HandleCharIsWhiteSpace(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)'\t');
        methodConvert.Push((ushort)'\r' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.Push((ushort)'\n');
        methodConvert.Push((ushort)' ' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.AddInstruction(OpCode.BOOLOR);
    }

    private static void HandleCharIsLower(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Push((ushort)'a');
        methodConvert.Push((ushort)'z' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
    }

    private static void HandleCharIsUpper(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Push((ushort)'A');
        methodConvert.Push((ushort)'Z' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
    }

    private static void HandleCharIsPunctuation(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        var endTarget = new JumpTarget();
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)'!');
        methodConvert.Push((ushort)'/' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)':');
        methodConvert.Push((ushort)'@' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)'[');
        methodConvert.Push((ushort)'`' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.Push((ushort)'{');
        methodConvert.Push((ushort)'~' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleCharIsSymbol(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        var endTarget = new JumpTarget();
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)'$');
        methodConvert.Push((ushort)'+' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)'<');
        methodConvert.Push((ushort)'=' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)'>');
        methodConvert.Push((ushort)'@' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)'[');
        methodConvert.Push((ushort)'`' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.Push((ushort)'{');
        methodConvert.Push((ushort)'~' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleCharIsControl(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)'\0');
        methodConvert.Push((ushort)'\x1F' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.Push((ushort)'\x7F');
        methodConvert.Push((ushort)'\x9F' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.AddInstruction(OpCode.BOOLOR);
    }

    private static void HandleCharIsSurrogate(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)0xD800);
        methodConvert.Push((ushort)0xDBFF + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.Push((ushort)0xDC00);
        methodConvert.Push((ushort)0xDFFF + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.AddInstruction(OpCode.BOOLOR);
    }

    private static void HandleCharIsHighSurrogate(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Push((ushort)0xD800);
        methodConvert.Push((ushort)0xDBFF + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
    }

    private static void HandleCharIsLowSurrogate(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Push((ushort)0xDC00);
        methodConvert.Push((ushort)0xDFFF + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
    }

    private static void HandleCharIsLetterOrDigit(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)'0');
        methodConvert.Push((ushort)'9' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)'A');
        methodConvert.Push((ushort)'Z' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.Push((ushort)'a');
        methodConvert.Push((ushort)'z' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleCharIsBetween(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget validTarget = new();
        JumpTarget endTarget = new();
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ROT);
        methodConvert.AddInstruction(OpCode.GE);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Jump(OpCode.JMPIFNOT, validTarget);
        methodConvert.AddInstruction(OpCode.REVERSE3);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Jump(OpCode.JMP, endTarget);
        validTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.AddInstruction(OpCode.LT);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleCharGetNumericValue(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        JumpTarget validTarget = new();
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)'0');
        methodConvert.Push((ushort)'9' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, validTarget);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.AddInstruction(OpCode.PUSHM1);
        methodConvert.Jump(OpCode.JMP, endTarget);
        validTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.Push((ushort)'0');
        methodConvert.AddInstruction(OpCode.SUB);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleCharToLower(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)'A');
        methodConvert.Push((ushort)'Z' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.Push((ushort)'A');
        methodConvert.AddInstruction(OpCode.SUB);
        methodConvert.Push((ushort)'a');
        methodConvert.AddInstruction(OpCode.ADD);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleCharToUpper(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)'a');
        methodConvert.Push((ushort)'z' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.Push((ushort)'a');
        methodConvert.AddInstruction(OpCode.SUB);
        methodConvert.Push((ushort)'A');
        methodConvert.AddInstruction(OpCode.ADD);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleCharToLowerInvariant(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)'a');
        methodConvert.Push((ushort)'z' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
    }

    private static void HandleCharToUpperInvariant(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)'a');
        methodConvert.Push((ushort)'z' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
    }

    private static void HandleCharIsAscii(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Push(128);
        methodConvert.AddInstruction(OpCode.LT);
    }

    private static void HandleCharIsAsciiDigit(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Push((ushort)'0');
        methodConvert.Push((ushort)'9' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
    }

    private static void HandleCharIsAsciiLetter(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Push((ushort)'A');
        methodConvert.Push((ushort)'Z' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.Push((ushort)'a');
        methodConvert.Push((ushort)'z' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }
}
