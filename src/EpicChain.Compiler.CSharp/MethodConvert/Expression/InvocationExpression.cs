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
using EpicChain.SmartContract;
using EpicChain.VM;
using System;
using System.Linq;

namespace EpicChain.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// Converts Invocation, include method invocation, event invocation and delegate invocation to OpCodes.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about invocation expression.</param>
    /// <param name="expression">The syntax representation of the invocation expression statement being converted.</param>
    private void ConvertInvocationExpression(SemanticModel model, InvocationExpressionSyntax expression)
    {
        ArgumentSyntax[] arguments = expression.ArgumentList.Arguments.ToArray();
        ISymbol symbol = model.GetSymbolInfo(expression.Expression).Symbol!;
        switch (symbol)
        {
            case IEventSymbol @event:
                ConvertEventInvocationExpression(model, @event, arguments);
                break;
            case IMethodSymbol method:
                ConvertMethodInvocationExpression(model, method, expression.Expression, arguments);
                break;
            default:
                ConvertDelegateInvocationExpression(model, expression.Expression, arguments);
                break;
        }
    }

    /// <summary>
    /// Convert the event invocation expression to OpCodes.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about event invocation expression.</param>
    /// <param name="symbol">Symbol of the event</param>
    /// <param name="arguments">Arguments of the event</param>
    /// <example><see href="https://github.com/epicchainlabs/epicchain-devkit-dotnet/blob/master/examples/Example.SmartContract.Event/Event.cs"/></example>
    private void ConvertEventInvocationExpression(SemanticModel model, IEventSymbol symbol, ArgumentSyntax[] arguments)
    {
        AddInstruction(OpCode.NEWARRAY0);
        foreach (ArgumentSyntax argument in arguments)
        {
            AddInstruction(OpCode.DUP);
            ConvertExpression(model, argument.Expression);
            AddInstruction(OpCode.APPEND);
        }
        Push(symbol.GetDisplayName());
        CallInteropMethod(ApplicationEngine.System_Runtime_Notify);
    }

    /// <summary>
    /// Convert the method invocation expression to OpCodes.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about method invocation expression.</param>
    /// <param name="symbol">Symbol of the method</param>
    /// <param name="expression">The syntax representation of the method invocation expression statement being converted.</param>
    /// <param name="arguments">Arguments of the method</param>
    /// <example>
    /// <c>Runtime.Log("hello World!");</c>
    /// </example>
    private void ConvertMethodInvocationExpression(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax expression, ArgumentSyntax[] arguments)
    {
        switch (expression)
        {
            case IdentifierNameSyntax:
                CallMethodWithInstanceExpression(model, symbol, null, arguments);
                break;
            case MemberAccessExpressionSyntax syntax:
                CallMethodWithInstanceExpression(model, symbol, symbol.IsStatic ? null : syntax.Expression, arguments);
                break;
            case MemberBindingExpressionSyntax:
                CallInstanceMethod(model, symbol, true, arguments);
                break;
            default:
                throw new CompilationException(expression, DiagnosticId.SyntaxNotSupported, $"Unsupported expression: {expression}");
        }
    }

    /// <summary>
    /// Convert the delegate invocation expression to OpCodes.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about delegate invocation expression.</param>
    /// <param name="expression">The syntax representation of the delegate invocation expression statement being converted.</param>
    /// <param name="arguments">Arguments of the delegate</param>
    /// <example>
    /// <code>
    /// public delegate int MyDelegate(int x, int y);
    ///
    /// static int CalculateSum(int x, int y)
    /// {
    ///     return x + y;
    /// }
    ///
    /// public void MyMethod()
    /// {
    ///     MyDelegate myDelegate = CalculateSum;
    ///     int result = myDelegate(5, 6);
    ///     Runtime.Log($"Sum: {result}");
    /// }
    /// </code>
    /// <c>myDelegate(5, 6)</c> This line will be converted by the following method.
    /// The  IdentifierNameSyntax is "myDelegate" the "type" is "MyDelegate".
    /// </example>
    private void ConvertDelegateInvocationExpression(SemanticModel model, ExpressionSyntax expression, ArgumentSyntax[] arguments)
    {
        INamedTypeSymbol type = (INamedTypeSymbol)model.GetTypeInfo(expression).Type!;
        PrepareArgumentsForMethod(model, type.DelegateInvokeMethod!, arguments);
        ConvertExpression(model, expression);
        AddInstruction(OpCode.CALLA);
    }
}
