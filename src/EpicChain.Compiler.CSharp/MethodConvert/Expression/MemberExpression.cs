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
        }
    }

    /// <summary>
    /// Further conversion of the ?. statement in the <see cref="ConvertConditionalAccessExpression"/> method
    /// </summary>
    /// <param name="model">The semantic model providing context and information about member binding expression.</param>
    /// <param name="expression">The syntax representation of the member binding expression statement being converted.</param>
    /// <exception cref="CompilationException">Only attributes and fields are supported, otherwise an exception is thrown.</exception>
    /// <example>
    /// <code>
    /// public class Person
    /// {
    ///     public string Name;
    ///     public int Age { get; set; }
    /// }
    /// </code>
    /// <code>
    /// Person person = null;
    /// Runtime.Log(person?.Name);
    /// Runtime.Log(person?.Age.ToString());
    /// </code>
    /// <c>person?.Name</c> code executes the <c>case IFieldSymbol field</c> branch;
    /// <c>person?.Age</c> code executes the <c>case IPropertySymbol property</c> branch.
    /// </example>
    private void ConvertMemberBindingExpression(SemanticModel model, MemberBindingExpressionSyntax expression)
    {
        ISymbol symbol = model.GetSymbolInfo(expression).Symbol!;
        switch (symbol)
        {
            case IFieldSymbol field:
                int index = Array.IndexOf(field.ContainingType.GetFields(), field);
                Push(index);
                AddInstruction(OpCode.PICKITEM);
                break;
            case IPropertySymbol property:
                CallMethodWithConvention(model, property.GetMethod!);
                break;
            default:
                throw new CompilationException(expression, DiagnosticId.SyntaxNotSupported, $"Unsupported symbol: {symbol}");
        }
    }
}
