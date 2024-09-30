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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using EpicChain.VM;

namespace EpicChain.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// Convet a binary pattern to OpCodes.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about binary pattern.</param>
    /// <param name="pattern">The binary pattern to be converted.</param>
    /// <param name="localIndex">The index of the local variable.</param>
    /// <see cref="ConvertAndPattern(SemanticModel, PatternSyntax, PatternSyntax, byte)"/>
    /// <see cref="ConvertOrPattern(SemanticModel, PatternSyntax, PatternSyntax, byte)"/>
    private void ConvertBinaryPattern(SemanticModel model, BinaryPatternSyntax pattern, byte localIndex)
    {
        switch (pattern.OperatorToken.ValueText)
        {
            case "and":
                ConvertAndPattern(model, pattern.Left, pattern.Right, localIndex);
                break;
            case "or":
                ConvertOrPattern(model, pattern.Left, pattern.Right, localIndex);
                break;
            default:
                throw new CompilationException(pattern, DiagnosticId.SyntaxNotSupported, $"Unsupported pattern: {pattern}");
        }
    }

    /// <summary>
    /// Convet a "and" pattern to OpCodes.
    /// </summary>
    /// <remarks>
    /// Conjunctive "and" pattern that matches an expression when both patterns match the expression.
    /// </remarks>
    /// <param name="model">The semantic model providing context and information about "and" pattern.</param>
    /// <param name="left">The left pattern to be converted.</param>
    /// <param name="right">The right pattern to be converted.</param>
    /// <param name="localIndex">The index of the local variable.</param>
    /// <example>
    /// The following example shows how you can combine relational patterns to check if a value is in a certain range:
    /// <code>
    /// public static string Classify(int measurement) => measurement switch
    /// {
    ///     < -40 => "Too low",
    ///     >= -40 and < 0 => "Low",
    ///     >= 0 and < 10 => "Acceptable",
    ///     >= 10 and < 20 => "High",
    ///     >= 20 => "Too high"
    /// };
    /// </code>
    /// </example>
    private void ConvertAndPattern(SemanticModel model, PatternSyntax left, PatternSyntax right, byte localIndex)
    {
        // Define jump targets for the right pattern and the end of the conversion process
        JumpTarget rightTarget = new();
        JumpTarget endTarget = new();

        // Convert the left pattern
        ConvertPattern(model, left, localIndex);

        // Jump to the right pattern if the left pattern matches
        Jump(OpCode.JMPIF_L, rightTarget);

        // Push 'false' onto the evaluation stack and jump to the end if the left pattern does not match
        Push(false);
        Jump(OpCode.JMP_L, endTarget);

        // Define an instruction for the right pattern and convert it
        rightTarget.Instruction = AddInstruction(OpCode.NOP);
        ConvertPattern(model, right, localIndex);

        // Define an instruction for the end of the conversion process
        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }

    /// <summary>
    /// Convet a "or" pattern to OpCodes.
    /// </summary>
    /// <remarks>
    /// Disjunctive "or" pattern that matches an expression when either pattern matches the expression.
    /// </remarks>
    /// <param name="model">The semantic model providing context and information about "or" pattern.</param>
    /// <param name="left">The left pattern to be converted.</param>
    /// <param name="right">The right pattern to be converted.</param>
    /// <param name="localIndex">The index of the local variable.</param>
    /// <example>
    /// As the following example shows:
    /// <code>
    /// public static string GetCalendarSeason(int month) => month switch
    /// {
    ///     3 or 4 or 5 => "spring",
    ///     6 or 7 or 8 => "summer",
    ///     9 or 10 or 11 => "autumn",
    ///     12 or 1 or 2 => "winter",
    ///     _ => throw new Exception($"Unexpected month: {month}."),
    /// };
    /// </code>
    /// As the preceding example shows, you can repeatedly use the pattern combinators in a pattern.
    /// </example>
    private void ConvertOrPattern(SemanticModel model, PatternSyntax left, PatternSyntax right, byte localIndex)
    {
        // Define jump targets for the right pattern and the end of the conversion process
        JumpTarget rightTarget = new();
        JumpTarget endTarget = new();

        // Convert the left pattern
        ConvertPattern(model, left, localIndex);

        // Jump to the right pattern if the left pattern does not match
        Jump(OpCode.JMPIFNOT_L, rightTarget);

        // Push 'true' onto the evaluation stack and jump to the end if the left pattern matches
        Push(true);
        Jump(OpCode.JMP_L, endTarget);

        // Define an instruction for the right pattern and convert it
        rightTarget.Instruction = AddInstruction(OpCode.NOP);
        ConvertPattern(model, right, localIndex);

        // Define an instruction for the end of the conversion process
        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }
}
