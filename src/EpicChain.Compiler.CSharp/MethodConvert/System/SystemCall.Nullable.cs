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


using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using EpicChain.SmartContract.Native;
using EpicChain.VM;

namespace EpicChain.Compiler;

internal partial class MethodConvert
{
    private static void HandleNullableByteHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.AddInstruction(OpCode.NOT);
    }

    private static void HandleNullableByteValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableByteGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push(0);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableSByteHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.AddInstruction(OpCode.NOT);
    }

    private static void HandleNullableSByteValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableSByteGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push(0);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableShortHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.AddInstruction(OpCode.NOT);
    }

    private static void HandleNullableShortValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableShortGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push(0);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableUShortHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.AddInstruction(OpCode.NOT);
    }

    private static void HandleNullableUShortValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableUShortGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push(0);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableUIntHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.AddInstruction(OpCode.NOT);
    }

    private static void HandleNullableUIntValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableUIntGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push(0);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableULongHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.AddInstruction(OpCode.NOT);
    }

    private static void HandleNullableULongValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableULongGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push(0);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableBoolHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.AddInstruction(OpCode.NOT);
    }

    private static void HandleNullableBoolValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableBoolGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push(0);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableCharHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.AddInstruction(OpCode.NOT);
    }

    private static void HandleNullableCharValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableCharGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push(0);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableBigIntegerHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.AddInstruction(OpCode.NOT);
    }

    private static void HandleNullableBigIntegerValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableBigIntegerGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push(0);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableIntHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.AddInstruction(OpCode.NOT);
    }

    private static void HandleNullableIntValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableIntGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push(0);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableLongHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.AddInstruction(OpCode.NOT);
    }

    private static void HandleNullableLongValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableLongGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push(0);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableToString(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {

        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        JumpTarget endTarget2 = new();
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.CallContractMethod(NativeContract.EssentialLib.Hash, "itoa", 1, true);
        methodConvert.Jump(OpCode.JMP_L, endTarget2);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push("");
        endTarget2.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableBoolToString(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        JumpTarget trueTarget = new(), nullTarget = new(), endTarget = new();
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.Jump(OpCode.JMPIF_L, nullTarget);
        methodConvert.Jump(OpCode.JMPIF_L, trueTarget);
        methodConvert.Push("False");
        methodConvert.Jump(OpCode.JMP_L, endTarget);
        trueTarget.Instruction = methodConvert.Push("True");
        methodConvert.Jump(OpCode.JMP_L, endTarget);
        nullTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push("");
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableEquals(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        JumpTarget nullTarget1 = new(), nullTarget2 = new(), endTarget = new();

        methodConvert.AddInstruction(OpCode.DUP);// x y
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.Jump(OpCode.JMPIF_L, nullTarget1);

        // y is not null
        methodConvert.AddInstruction(OpCode.SWAP);// y x
        methodConvert.AddInstruction(OpCode.DUP); // y x x
        methodConvert.AddInstruction(OpCode.ISNULL); // y x
        methodConvert.Jump(OpCode.JMPIF_L, nullTarget2);

        // y and x both are not null
        methodConvert.AddInstruction(OpCode.EQUAL);
        methodConvert.Jump(OpCode.JMP_L, endTarget);

        // y is null, then return true if x is null, false otherwise
        nullTarget1.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.DROP);// x
        methodConvert.AddInstruction(OpCode.ISNULL); // true is null, otherwise false
        methodConvert.Jump(OpCode.JMP_L, endTarget);

        nullTarget2.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.DROP); // drop x
        methodConvert.AddInstruction(OpCode.DROP); // drop y
        methodConvert.Push(false); // y not null but x is null

        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableBigIntegerEquals(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        JumpTarget nullTarget1 = new(), nullTarget2 = new(), endTarget = new();

        methodConvert.AddInstruction(OpCode.DUP);// x y
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.Jump(OpCode.JMPIF_L, nullTarget1);

        // y is not null
        methodConvert.AddInstruction(OpCode.SWAP);// y x
        methodConvert.AddInstruction(OpCode.DUP); // y x x
        methodConvert.AddInstruction(OpCode.ISNULL); // y x
        methodConvert.Jump(OpCode.JMPIF_L, nullTarget2);

        // y and x both are not null
        methodConvert.AddInstruction(OpCode.NUMEQUAL);
        methodConvert.Jump(OpCode.JMP_L, endTarget);

        // y is null, then return true if x is null, false otherwise
        nullTarget1.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.DROP);// x
        methodConvert.AddInstruction(OpCode.ISNULL); // true is null, otherwise false
        methodConvert.Jump(OpCode.JMP_L, endTarget);

        nullTarget2.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.DROP); // drop x
        methodConvert.AddInstruction(OpCode.DROP); // drop y
        methodConvert.Push(false); // y not null but x is null

        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableBoolEquals(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        JumpTarget nullTarget1 = new(), nullTarget2 = new(), endTarget = new();

        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.Jump(OpCode.JMPIF_L, nullTarget1);

        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.Jump(OpCode.JMPIF_L, nullTarget2);

        methodConvert.AddInstruction(OpCode.EQUAL);
        methodConvert.Jump(OpCode.JMP_L, endTarget);

        nullTarget1.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.Jump(OpCode.JMP_L, endTarget);

        nullTarget2.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push(false);

        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableBigIntegerEqualsWithNonNullable(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
