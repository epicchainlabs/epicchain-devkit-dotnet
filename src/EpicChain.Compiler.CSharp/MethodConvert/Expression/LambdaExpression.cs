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


using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using EpicChain.VM;

namespace EpicChain.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// Convert a simple lambda expression to a method call
    /// </summary>
    /// <param name="model">The semantic model of the method</param>
    /// <param name="expression">The lambda expression to convert</param>
    /// <example>
    /// <code>
    /// public void MyMethod()
    /// {
    ///     var lambda = x => x + 1;
    ///     lambda(1);
    /// }
    /// </code>
    /// </example>
    private void ConvertSimpleLambdaExpression(SemanticModel model, SimpleLambdaExpressionSyntax expression)
    {
        var symbol = (IMethodSymbol)model.GetSymbolInfo(expression).Symbol!;
        var mc = _context.ConvertMethod(model, symbol);
        ConvertLocalToStaticFields(mc);
        InvokeMethod(mc);
    }

    /// <summary>
    /// Convert a parenthesized lambda expression to a method call
    /// </summary>
    /// <param name="model">The semantic model of the method</param>
    /// <param name="expression">The lambda expression to convert</param>
    /// <example>
    /// <code>
    /// public void MyMethod()
    /// {
    ///     var lambda = (int x, int y) => x + y;
    ///     var result = lambda(1, 2);
    ///     Console.WriteLine(result);
    /// }
    /// </code>
    /// </example>
    private void ConvertParenthesizedLambdaExpression(SemanticModel model, ParenthesizedLambdaExpressionSyntax expression)
    {
        var symbol = (IMethodSymbol)model.GetSymbolInfo(expression).Symbol!;
        var mc = _context.ConvertMethod(model, symbol);
        ConvertLocalToStaticFields(mc);
        InvokeMethod(mc);
    }

    /// <summary>
    /// Convert captured local variables/parameters to static fields
    /// Assign values of captured local variables/parameters to related static fields
    /// </summary>
    /// <param name="mc">The method convert context</param>
    private void ConvertLocalToStaticFields(MethodConvert mc)
    {
        if (mc.CapturedLocalSymbols.Count <= 0) return;
        foreach (var local in mc.CapturedLocalSymbols)
        {
            // copy captured local variable/parameter value to related static fields
            var staticFieldIndex = _context.GetOrAddCapturedStaticField(local);
            switch (local)
            {
                case ILocalSymbol localSymbol:
                    var localIndex = _localVariables[localSymbol];
                    AccessSlot(OpCode.LDLOC, localIndex);
                    break;
                case IParameterSymbol parameterSymbol:
                    var paraIndex = _parameters[parameterSymbol];
                    AccessSlot(OpCode.LDARG, paraIndex);
                    break;
            }
            AccessSlot(OpCode.STSFLD, staticFieldIndex);
        }
    }
}
