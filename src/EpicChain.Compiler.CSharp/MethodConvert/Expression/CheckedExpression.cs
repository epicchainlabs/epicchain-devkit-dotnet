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
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EpicChain.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// The checked and unchecked statements specify the overflow-checking context for integral-type arithmetic operations and conversions.
    /// When integer arithmetic overflow occurs, the overflow-checking context defines what happens.
    /// In a checked context, a System.OverflowException is thrown;
    /// if overflow happens in a constant expression, a compile-time error occurs.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about checked and unchecked statement.</param>
    /// <param name="expression">The syntax representation of the checked and unchecked statement being converted.</param>
    /// <example>
    /// Use the checked keyword to qualify the result of the temp*2 calculation and use a try catch to handle the overflow if it occurs.
    /// <code>
    /// try
    /// {
    ///     int temp = int.MaxValue;
    ///     int a = checked(temp * 2);
    /// }
    /// catch (OverflowException)
    /// {
    ///     Runtime.Log("Overflow");
    /// }
    /// </code>
    /// </example>
    /// <remarks>
    /// This code is not called when the checked keyword modifies a block of statements, for example.
    /// <code>
    /// checked
    /// {
    ///     int a = temp * 2;
    /// }
    /// </code>
    /// For a checked statement, see <see cref="ConvertCheckedStatement(SemanticModel, CheckedStatementSyntax)"/>
    /// </remarks>
    /// <seealso href="https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/operators/arithmetic-operators#integer-arithmetic-overflow">Integer arithmetic overflow</seealso>
    private void ConvertCheckedExpression(SemanticModel model, CheckedExpressionSyntax expression)
    {
        _checkedStack.Push(expression.Keyword.IsKind(SyntaxKind.CheckedKeyword));
        ConvertExpression(model, expression.Expression);
        _checkedStack.Pop();
    }
}
