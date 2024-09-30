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
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);

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
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);

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
