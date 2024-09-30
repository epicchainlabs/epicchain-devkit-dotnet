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
    /// Convet relational pattern to OpCodes.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about convert pattern.</param>
    /// <param name="pattern">The convert pattern to be converted.</param>
    /// <param name="localIndex">The index of the local variable.</param>
    /// <exception cref="CompilationException"></exception>
    /// <remarks>
    /// In a relational pattern, you can use any of the relational operators <![CDATA[<, >, <=, or >=]]>.
    /// The right-hand part of a relational pattern must be a constant expression.
    /// The constant expression can be of an integer, char, or enum type.
    /// To check if an expression result is in a certain range, match it against a <see cref="ConvertBinaryPattern(SemanticModel, BinaryPatternSyntax, byte)">conjunctive and pattern</see>.
    /// </remarks>
    /// <example>
    /// You use a relational pattern to compare an expression result with a constant,
    /// as the following example shows:
    /// <code>
    /// int a = 1;
    /// var b = a switch
    /// {
    ///     > 1 => true,
    ///     <= 1 => false
    /// };
    /// </code>
    /// <c>> 1</c> and <c><= 1</c> is RelationalPatternSyntax;
    /// </example>
    private void ConvertRelationalPattern(SemanticModel model, RelationalPatternSyntax pattern, byte localIndex)
    {
        AccessSlot(OpCode.LDLOC, localIndex);
        ConvertExpression(model, pattern.Expression);
        AddInstruction(pattern.OperatorToken.ValueText switch
        {
            "<" => OpCode.LT,
            "<=" => OpCode.LE,
            ">" => OpCode.GT,
            ">=" => OpCode.GE,
            _ => throw new CompilationException(pattern, DiagnosticId.SyntaxNotSupported, $"Unsupported pattern: {pattern}")
        });
    }
}
