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
        methodConvert.CallContractMethod(NativeContract.EssentialLib.Hash, "atoi", 1, true);
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
