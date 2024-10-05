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
using System;

namespace EpicChain.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// This method converts a member access expression to OpCodes.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about member access expression.</param>
    /// <param name="expression">The syntax representation of the member access expression statement being converted.</param>
    /// <exception cref="CompilationException">Unsupported symbols will result in a compilation exception, such as non-static methods.</exception>
    /// <remarks>
    /// The method determines the symbol associated with the member access expression from the semantic model.
    /// It then generates OpCodes based on the type of symbol.
    /// Supported symbols include fields, methods, and properties.
    /// For fields, it handles constant fields, static fields, and instance fields.
    /// For methods, it handles static methods.
    /// For properties, it handles accessing static properties and instance properties.
    /// </remarks>
    /// <example>
    /// This is a member access example. The following code branches to "case IPropertySymbol property".
    /// <code>
    /// Runtime.Log(Ledger.CurrentHash.ToString());
    /// </code>
    /// </example>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/member-access-operators#member-access-expression-">Member access expression</seealso>
    private void ConvertMemberAccessExpression(SemanticModel model, MemberAccessExpressionSyntax expression)
    {
        ISymbol symbol = model.GetSymbolInfo(expression).Symbol!;
        switch (symbol)
        {
            case IFieldSymbol field:
                if (field.IsConst)
                {
                    // This branch is not covered, is there any c# code that matches the conditions?
                    // Const member field access is handled via ConvertMethodInvocationExpression
                    Push(field.ConstantValue);
                }
                else if (field.IsStatic)
                {
                    // Have to process the string.Empty specially since it has no AssociatedSymbol
                    // thus will return directly without this if check.
                    if (field.ContainingType.ToString() == "string" && field.Name == "Empty")
                    {
                        Push(string.Empty);
                        return;
                    }

                    byte index = _context.AddStaticField(field);
                    AccessSlot(OpCode.LDSFLD, index);
                }
                else
                {
                    int index = Array.IndexOf(field.ContainingType.GetFields(), field);
                    ConvertExpression(model, expression.Expression);
                    Push(index);
                    AddInstruction(OpCode.PICKITEM);
                }
                break;
            case IMethodSymbol method:
                //This branch is not covered, is there any c# code that matches the conditions?
                if (!method.IsStatic)
                    throw new CompilationException(expression, DiagnosticId.NonStaticDelegate, $"Unsupported delegate: {method}");
                InvokeMethod(model, method);
                break;
            case IPropertySymbol property:
                ExpressionSyntax? instanceExpression = property.IsStatic ? null : expression.Expression;
                CallMethodWithInstanceExpression(model, property.GetMethod!, instanceExpression);
                break;
            default:
                throw new CompilationException(expression, DiagnosticId.SyntaxNotSupported, $"Unsupported symbol: {symbol}");
