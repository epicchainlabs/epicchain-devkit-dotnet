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
