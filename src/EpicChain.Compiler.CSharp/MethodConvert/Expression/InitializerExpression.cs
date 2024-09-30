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
using System.Linq;

namespace EpicChain.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// Converts initialization of array fields into OpCodes.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about initialization of array fields expression.</param>
    /// <param name="expression">The syntax representation of the initialization of array fields expression statement being converted.</param>
    /// <example>
    /// The following 4 static fields will each be converted in this method.
    /// <code>
    /// static string[] A = { "BTC", "NEO", "GAS" };
    /// static int[] B = { 1, 2 };
    /// static byte[] C = { 1, 2 };
    /// static UInt160 D = UInt160.Zero;
    ///
    /// public static void MyMethod()
    /// {
    ///     Runtime.Log(A[0]);
    ///     Runtime.Log(B[0]);
    ///     Runtime.Log(C[0]);
    ///     Runtime.Log(D.ToAddress());
    /// }
    /// </code>
    /// </example>
    private void ConvertInitializerExpression(SemanticModel model, InitializerExpressionSyntax expression)
    {
        IArrayTypeSymbol type = (IArrayTypeSymbol)model.GetTypeInfo(expression).ConvertedType!;
        ConvertInitializerExpression(model, type, expression);
    }

    private void ConvertInitializerExpression(SemanticModel model, IArrayTypeSymbol type, InitializerExpressionSyntax expression)
    {
        if (type.ElementType.SpecialType == SpecialType.System_Byte)
        {
            Optional<object?>[] values = expression.Expressions.Select(p => model.GetConstantValue(p)).ToArray();
            if (values.Any(p => !p.HasValue))
            {
                Push(values.Length);
                AddInstruction(OpCode.NEWBUFFER);
                for (int i = 0; i < expression.Expressions.Count; i++)
                {
                    AddInstruction(OpCode.DUP);
                    Push(i);
                    ConvertExpression(model, expression.Expressions[i]);
                    AddInstruction(OpCode.SETITEM);
                }
            }
            else
            {
                byte[] data = values.Select(p => (byte)System.Convert.ChangeType(p.Value, typeof(byte))!).ToArray();
                Push(data);
                ChangeType(VM.Types.StackItemType.Buffer);
            }
        }
        else
        {
            for (int i = expression.Expressions.Count - 1; i >= 0; i--)
                ConvertExpression(model, expression.Expressions[i]);
            Push(expression.Expressions.Count);
            AddInstruction(OpCode.PACK);
        }
    }
}
