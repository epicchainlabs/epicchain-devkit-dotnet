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
    /// This method converts an identifier name expression to OpCodes.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about identifier name expression.</param>
    /// <param name="expression">The syntax representation of the identifier name expression statement being converted.</param>
    /// <exception cref="CompilationException">Unsupported symbols will result in a compilation exception.</exception>
    /// <example>
    /// Processing of the identifier "param" goes to the "IParameterSymbol parameter" branch of the code;
    /// processing of the identifier "temp" goes to the "ILocalSymbol local" branch of the code.
    /// Unused identifier "param2" will not be processed.
    /// <code>
    /// public static void MyMethod(int param, int param2)
    /// {
    ///     int temp = 0;
    ///     byte[] temp2 = new byte[1];
    ///     Runtime.Log(temp.ToString());
    ///     Runtime.Log(temp2[0].ToString());
    ///     Runtime.Log(param.ToString());
    /// }
    /// </code>
    ///  The <c>symbol.Name</c> of the above code is as follows: "temp", "temp2", "param";
    /// </example>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/identifier-names">C# identifier naming rules and conventions</seealso>
    private void ConvertIdentifierNameExpression(SemanticModel model, IdentifierNameSyntax expression)
    {
        ISymbol symbol = model.GetSymbolInfo(expression).Symbol!;
        switch (symbol)
        {
            case IFieldSymbol field:
                if (field.IsConst)
                {
                    Push(field.ConstantValue);
                }
                else if (field.IsStatic)
                {
                    byte index = _context.AddStaticField(field);
                    AccessSlot(OpCode.LDSFLD, index);
                }
                else
                {
                    int index = Array.IndexOf(field.ContainingType.GetFields(), field);
                    AddInstruction(OpCode.LDARG0);
                    Push(index);
                    AddInstruction(OpCode.PICKITEM);
                }
                break;
            case ILocalSymbol local:
                if (local.IsConst)
                    Push(local.ConstantValue);
                else
                    LdLocSlot(local);
                break;
            case IMethodSymbol method:
                if (!method.IsStatic)
                    throw new CompilationException(expression, DiagnosticId.NonStaticDelegate, $"Unsupported delegate: {method}");
                InvokeMethod(model, method);
                break;
            case IParameterSymbol parameter:
                if (!_internalInline) LdArgSlot(parameter);
                break;
            case IPropertySymbol property:
                if (!property.IsStatic)
                    AddInstruction(OpCode.LDARG0);
                CallMethodWithConvention(model, property.GetMethod!);
                break;
            case ITypeSymbol type:
                IsType(type.GetPatternType());
                Push(true);
                break;
            default:
                throw new CompilationException(expression, DiagnosticId.SyntaxNotSupported, $"Unsupported symbol: {symbol}");
        }
    }
}
