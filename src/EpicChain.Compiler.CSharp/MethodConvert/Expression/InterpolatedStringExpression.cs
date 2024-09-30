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
