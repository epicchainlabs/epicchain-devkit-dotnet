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
using System.Numerics;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using EpicChain.SmartContract.Native;
using EpicChain.VM;

namespace EpicChain.Compiler;

partial class MethodConvert
{
    private static void HandleByteTryParseWithOut(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleNumericTryParseWithOut(methodConvert, model, symbol, arguments, byte.MinValue, byte.MaxValue);
    }

    private static void HandleSByteTryParseWithOut(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleNumericTryParseWithOut(methodConvert, model, symbol, arguments, sbyte.MinValue, sbyte.MaxValue);
    }

    private static void HandleShortTryParseWithOut(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleNumericTryParseWithOut(methodConvert, model, symbol, arguments, short.MinValue, short.MaxValue);
    }

    private static void HandleUShortTryParseWithOut(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleNumericTryParseWithOut(methodConvert, model, symbol, arguments, ushort.MinValue, ushort.MaxValue);
    }

    private static void HandleIntTryParseWithOut(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleNumericTryParseWithOut(methodConvert, model, symbol, arguments, int.MinValue, int.MaxValue);
    }

    private static void HandleUIntTryParseWithOut(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleNumericTryParseWithOut(methodConvert, model, symbol, arguments, uint.MinValue, uint.MaxValue);
    }

    private static void HandleLongTryParseWithOut(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleNumericTryParseWithOut(methodConvert, model, symbol, arguments, long.MinValue, long.MaxValue);
    }

    private static void HandleULongTryParseWithOut(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleNumericTryParseWithOut(methodConvert, model, symbol, arguments, ulong.MinValue, ulong.MaxValue);
    }

    private static void HandleNumericTryParseWithOut(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, IReadOnlyList<SyntaxNode>? arguments, BigInteger minValue, BigInteger maxValue)
    {
        if (arguments is null) return;
        methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        if (!methodConvert._context.TryGetCapturedStaticField(symbol.Parameters[1], out var index)) throw new CompilationException(symbol, DiagnosticId.SyntaxNotSupported, "Out parameter must be captured in a static field.");

        // Drop the out parameter since it's not needed
        // We use the static field to store the result
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.AddInstruction(OpCode.DROP);

        // Convert string to integer
        methodConvert.CallContractMethod(NativeContract.EssentialLib.Hash, "atoi", 1, true);

        // Check if the parsing was successful (not null)
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);

        JumpTarget failTarget = new();
        methodConvert.Jump(OpCode.JMPIF_L, failTarget);

        // If successful, check if the parsed value is within the valid range
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push(minValue);
        methodConvert.Push(maxValue + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIFNOT_L, failTarget);

        // If within range, store the value and push true
        methodConvert.AccessSlot(OpCode.STSFLD, index);
        methodConvert.Push(true);
        JumpTarget endTarget = new();
        methodConvert.Jump(OpCode.JMP_L, endTarget);

        // Fail target: push false
        failTarget.Instruction = methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push(false);

        // End target
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleBigIntegerTryParseWithOut(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is null) return;
        methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        if (!methodConvert._context.TryGetCapturedStaticField(symbol.Parameters[1], out var index)) throw new CompilationException(symbol, DiagnosticId.SyntaxNotSupported, "Out parameter must be captured in a static field.");


        JumpTarget endTarget = new();

        // Convert string to BigInteger
        methodConvert.CallContractMethod(NativeContract.EssentialLib.Hash, "atoi", 1, true);

        // Check if the parsing was successful
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.Jump(OpCode.JMPIF_L, endTarget);

        // If successful, store the value and push true
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AccessSlot(OpCode.STSFLD, index);
        methodConvert.Push(true);
        methodConvert.Jump(OpCode.JMP_L, endTarget);

        // End target: clean up stack and push false if parsing failed
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push(false);
    }

    private static void HandleBoolTryParseWithOut(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is null) return;
        methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        if (!methodConvert._context.TryGetCapturedStaticField(symbol.Parameters[1], out var index)) throw new CompilationException(symbol, DiagnosticId.SyntaxNotSupported, "Out parameter must be captured in a static field.");

        JumpTarget trueTarget = new();
        JumpTarget falseTarget = new();
        JumpTarget endTarget = new();

        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.AddInstruction(OpCode.DROP);

        // Check for true values
        methodConvert.AddInstruction(OpCode.DUP); // x x
        methodConvert.Push("true"); // x x "true"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, trueTarget);

        methodConvert.AddInstruction(OpCode.DUP); // x x
        methodConvert.Push("TRUE"); // x x "TRUE"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, trueTarget);

        methodConvert.AddInstruction(OpCode.DUP); // x x
        methodConvert.Push("True"); // x x "True"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, trueTarget);

        methodConvert.AddInstruction(OpCode.DUP); // x x
        methodConvert.Push("t"); // x x "t"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, trueTarget);

        methodConvert.AddInstruction(OpCode.DUP); // x x
        methodConvert.Push("T"); // x x "T"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, trueTarget);

        methodConvert.AddInstruction(OpCode.DUP); // x x
        methodConvert.Push("1"); // x x "1"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, trueTarget); // x

        methodConvert.AddInstruction(OpCode.DUP); // x x
        methodConvert.Push("yes"); // x x "yes"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, trueTarget); // x

        methodConvert.AddInstruction(OpCode.DUP); // x x
        methodConvert.Push("YES"); // x x "YES"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, trueTarget); // x

        methodConvert.AddInstruction(OpCode.DUP); // x x
        methodConvert.Push("y"); // x x "y"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, trueTarget); // x

        methodConvert.AddInstruction(OpCode.DUP); // x x
        methodConvert.Push("Y"); // x x "Y"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, trueTarget); // x

        // Check for false values
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push("false"); // x x "false"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, falseTarget); // x

        methodConvert.AddInstruction(OpCode.DUP); // x x
        methodConvert.Push("FALSE"); // x x "FALSE"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, falseTarget); // x

        methodConvert.AddInstruction(OpCode.DUP); // x x
        methodConvert.Push("False"); // x x "False"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, falseTarget); // x

        methodConvert.AddInstruction(OpCode.DUP); // x x
        methodConvert.Push("f"); // x x "f"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, falseTarget); // x

        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push("F"); // x x "F"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, falseTarget); // x

        methodConvert.AddInstruction(OpCode.DUP); // x x
        methodConvert.Push("0"); // x x "0"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, falseTarget); // x

        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push("no"); // x x "no"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, falseTarget); // x

        methodConvert.AddInstruction(OpCode.DUP); // x x
        methodConvert.Push("NO"); // x x "NO"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, falseTarget); // x

        methodConvert.AddInstruction(OpCode.DUP); // x x
        methodConvert.Push("n"); // x x "n"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, falseTarget); // x

        methodConvert.AddInstruction(OpCode.DUP); // x x
        methodConvert.Push("N"); // x x "N"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, falseTarget); // x

        // If parsing failed, clean up stack and push false
        methodConvert.AddInstruction(OpCode.DROP); //
        methodConvert.Push(false); // false
        methodConvert.AccessSlot(OpCode.STSFLD, index); // false
        methodConvert.Push(false); // false
        methodConvert.Jump(OpCode.JMP_L, endTarget); // false

        // True case
        trueTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP); // x
        methodConvert.AddInstruction(OpCode.DROP); //
        methodConvert.Push(true); //  true
        methodConvert.AccessSlot(OpCode.STSFLD, index); // true
        methodConvert.Push(true); // true
        methodConvert.Jump(OpCode.JMP_L, endTarget); // true

        // False case
        falseTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP); // x
        methodConvert.AddInstruction(OpCode.DROP); //
        methodConvert.Push(false); // false
        methodConvert.AccessSlot(OpCode.STSFLD, index); // false
        methodConvert.Push(true); // true

        // End target
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }
}
