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


extern alias scfx;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using EpicChain.VM;
using System.Linq;

namespace EpicChain.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// This method converts an array element or indexer access ([]) expression to OpCodes.
    /// An array element or indexer access ([]) expression accesses a single element in an array or a collection.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about element access expression.</param>
    /// <param name="expression">The syntax representation of the element access expression statement being converted.</param>
    /// <exception cref="CompilationException">Only one-dimensional arrays are supported, otherwise an exception is thrown.</exception>
    /// <remarks>
    /// If the accessed element is a property, the method calls the property's getter.
    /// If the accessed element is an array or a collection, the method generates OpCodes to fetch the element.
    /// </remarks>
    /// <example>
    /// The following example demonstrates how to access array elements:
    /// <code>
    /// var array = new byte[8];
    /// var sum = 0;
    /// for (var i = 0; i< 8; i++)
    /// {
    ///     sum += array[i];
    /// }
    /// Runtime.Log(sum.ToString());
    /// </code>
    /// </example>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/member-access-operators#indexer-operator-">Indexer operator []</seealso>
    private void ConvertElementAccessExpression(SemanticModel model, ElementAccessExpressionSyntax expression)
    {
        if (expression.ArgumentList.Arguments.Count != 1)
            throw new CompilationException(expression.ArgumentList, DiagnosticId.MultidimensionalArray, $"Unsupported array rank: {expression.ArgumentList.Arguments}");
        if (model.GetSymbolInfo(expression).Symbol is IPropertySymbol property)
        {
            CallMethodWithInstanceExpression(model, property.GetMethod!, expression.Expression, expression.ArgumentList.Arguments.ToArray());
        }
        else
        {
            ITypeSymbol type = model.GetTypeInfo(expression).Type!;
            ConvertExpression(model, expression.Expression);
            ConvertIndexOrRange(model, type, expression.ArgumentList.Arguments[0].Expression);
        }
    }

    /// <summary>
    /// Further conversion of the ?[] statement in the <see cref="ConvertConditionalAccessExpression"/> method
    /// </summary>
    /// <param name="model">The semantic model providing context and information about element binding expression.</param>
    /// <param name="expression">The syntax representation of the element binding expression statement being converted.</param>
    /// <exception cref="CompilationException">Only one-dimensional arrays are supported, otherwise an exception is thrown.</exception>
    /// <example>
    /// <code>
    /// var a = Ledger.GetBlock(10000);
    /// var b = Ledger.GetBlock(10001);
    /// var array = new[] { a, b };
    /// var firstItem = array?[0];
    /// Runtime.Log(firstItem?.Timestamp.ToString());
    /// </code>
    /// </example>
    private void ConvertElementBindingExpression(SemanticModel model, ElementBindingExpressionSyntax expression)
    {
        if (expression.ArgumentList.Arguments.Count != 1)
            throw new CompilationException(expression.ArgumentList, DiagnosticId.MultidimensionalArray, $"Unsupported array rank: {expression.ArgumentList.Arguments}");
        ITypeSymbol type = model.GetTypeInfo(expression).Type!;
        ConvertIndexOrRange(model, type, expression.ArgumentList.Arguments[0].Expression);
    }

    /// <summary>
    /// This method converts an index or range expression to OpCodes.
    /// An index or range expression specifies the index or range of elements to access in an array or a collection.
    /// </summary>
    /// <param name="model">The semantic model providing contextual information for the expression.</param>
    /// <param name="type">The type symbol of the array or collection being accessed.</param>
    /// <param name="indexOrRange">The expression representing the index or range.</param>
    /// <exception cref="CompilationException">Only byte[] and string type support range access,
    /// otherwise, an exception is thrown. For examples.
    /// <code>
    /// int[] oneThroughTen = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    /// var a = oneThroughTen[..];
    /// </code>
    /// </exception>
    /// <example>
    /// The following code is an example of an index or range selector in a collective:
    /// <code>
    /// byte[] oneThroughTen = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    /// var a = oneThroughTen[..];
    /// var b = oneThroughTen[..3];
    /// var c = oneThroughTen[2..];
    /// var d = oneThroughTen[3..5];
    /// var e = oneThroughTen[^2..];
    /// var f = oneThroughTen[..^3];
    /// var g = oneThroughTen[3..^4];
    /// var h = oneThroughTen[^4..^2];
    /// var i = oneThroughTen[0];
    /// </code>
    /// result:
    /// a: 1, 2, 3, 4, 5, 6, 7, 8, 9, 10
    /// b: 1, 2, 3
    /// c: 3, 4, 5, 6, 7, 8, 9, 10
    /// d: 4, 5
    /// e: 9, 10
    /// f: 1, 2, 3, 4, 5, 6, 7
    /// g: 4, 5, 6
    /// h: 7, 8
    /// i: 1
    /// </example>
    /// <remarks>
    /// If the expression is a range, it calculates the start and end indices and extracts the relevant sub-array or sub-collection.
    /// If the expression is an index, it generates OpCodes to fetch the element at the specified index.
    /// </remarks>
    private void ConvertIndexOrRange(SemanticModel model, ITypeSymbol type, ExpressionSyntax indexOrRange)
    {
