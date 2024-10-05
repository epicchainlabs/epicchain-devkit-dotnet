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
    /// This method converts an interpolated string expression to OpCodes.
    /// The $ character identifies a string literal as an interpolated string.
    /// An interpolated string is a string literal that might contain interpolation expressions.
    /// When an interpolated string is resolved to a result string,
    /// items with interpolation expressions are replaced by the string representations of the expression results.
    /// Interpolated string expression are a new feature introduced in C# 8.0(Released September, 2019).
    /// </summary>
    /// <param name="model">The semantic model providing context and information about interpolated string expression.</param>
    /// <param name="expression">The syntax representation of the interpolated string expression statement being converted.</param>
    /// <remarks>
    /// The method processes each interpolated string content segment and concatenates them using the CAT opcode.
    /// If the interpolated string contains no segments, it pushes an empty string onto the evaluation stack.
    /// If the interpolated string contains two or more segments, it changes the type of the resulting string to ByteString.
    /// </remarks>
    /// <example>
    /// The following interpolated string will be divided into 5 parts and concatenated via OpCode.CAT
    /// <code>
    /// var name = "Mark";
    /// var timestamp = Ledger.GetBlock(Ledger.CurrentHash).Timestamp;
    /// Runtime.Log($"Hello, {name}! Current timestamp is {timestamp}.");
    /// </code>
    /// </example>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/tokens/interpolated">String interpolation using $</seealso>
    private void ConvertInterpolatedStringExpression(SemanticModel model, InterpolatedStringExpressionSyntax expression)
    {
        if (expression.Contents.Count == 0)
        {
            Push(string.Empty);
            return;
        }
        ConvertInterpolatedStringContent(model, expression.Contents[0]);
        for (int i = 1; i < expression.Contents.Count; i++)
        {
            ConvertInterpolatedStringContent(model, expression.Contents[i]);
            AddInstruction(OpCode.CAT);
        }
        if (expression.Contents.Count >= 2)
            ChangeType(VM.Types.StackItemType.ByteString);
    }

    private void ConvertInterpolatedStringContent(SemanticModel model, InterpolatedStringContentSyntax content)
    {
        switch (content)
        {
            case InterpolatedStringTextSyntax syntax:
                Push(syntax.TextToken.ValueText);
                break;
            case InterpolationSyntax syntax:
                if (syntax.AlignmentClause is not null)
                    throw new CompilationException(syntax.AlignmentClause, DiagnosticId.AlignmentClause, $"Alignment clause is not supported: {syntax.AlignmentClause}");
                if (syntax.FormatClause is not null)
                    throw new CompilationException(syntax.FormatClause, DiagnosticId.FormatClause, $"Format clause is not supported: {syntax.FormatClause}");
                ConvertObjectToString(model, syntax.Expression);
                break;
        }
    }
}
