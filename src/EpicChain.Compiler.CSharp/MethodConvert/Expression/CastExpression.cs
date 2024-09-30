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

namespace EpicChain.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// This method converts a cast expression to OpCodes.
    /// A cast expression of the form (T)E performs an explicit conversion of the result of expression E to type T.
    /// If no explicit conversion exists from the type of E to type T, a compile-time error occurs.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about cast expression.</param>
    /// <param name="expression">The syntax representation of the cast expression statement being converted.</param>
    /// <remarks>
    /// This method determines the source type and the target type of the cast expression.
    /// If the cast can be resolved to a method symbol, it calls the corresponding method.
    /// Otherwise, it generates OpCodes based on the types involved in the cast operation.
    /// </remarks>
    /// <example>
    /// This code is cast a ByteString type to an ECPoint type,
    /// where the source type is ByteString and the target type is ECPoint.
    /// <code>
    /// ByteString bytes = ByteString.Empty;
    /// ECPoint point = (ECPoint)bytes;
    /// Runtime.Log(point.ToString());
    /// </code>
    /// </example>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/type-testing-and-cast#cast-expression">Cast expression</seealso>
    private void ConvertCastExpression(SemanticModel model, CastExpressionSyntax expression)
    {
        ITypeSymbol sType = model.GetTypeInfo(expression.Expression).Type!;
        ITypeSymbol tType = model.GetTypeInfo(expression.Type).Type!;
        IMethodSymbol method = (IMethodSymbol)model.GetSymbolInfo(expression).Symbol!;
        if (method is not null)
        {
            CallMethodWithInstanceExpression(model, method, null, expression.Expression);
            return;
        }
        ConvertExpression(model, expression.Expression);
        switch ((sType.Name, tType.Name))
        {
            case ("ByteString", "ECPoint"):
                {
                    JumpTarget endTarget = new();
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ISNULL);
                    Jump(OpCode.JMPIF_L, endTarget);
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.SIZE);
                    Push(33);
                    Jump(OpCode.JMPEQ_L, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                }
                break;
            case ("ByteString", "UInt160"):
                {
                    JumpTarget endTarget = new();
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ISNULL);
                    Jump(OpCode.JMPIF_L, endTarget);
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.SIZE);
                    Push(20);
                    Jump(OpCode.JMPEQ_L, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                }
                break;
            case ("ByteString", "UInt256"):
                {
                    JumpTarget endTarget = new();
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ISNULL);
                    Jump(OpCode.JMPIF_L, endTarget);
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.SIZE);
                    Push(32);
                    Jump(OpCode.JMPEQ_L, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                }
                break;
            case ("SByte", "Byte"):
            case ("SByte", "UInt16"):
            case ("SByte", "UInt32"):
            case ("SByte", "UInt64"):
            case ("Int16", "SByte"):
            case ("Int16", "Byte"):
            case ("Int16", "UInt16"):
            case ("Int16", "UInt32"):
            case ("Int16", "UInt64"):
            case ("Int32", "SByte"):
            case ("Int32", "Int16"):
            case ("Int32", "Byte"):
            case ("Int32", "UInt16"):
            case ("Int32", "UInt32"):
            case ("Int32", "UInt64"):
            case ("Int64", "SByte"):
            case ("Int64", "Int16"):
            case ("Int64", "Int32"):
            case ("Int64", "Byte"):
            case ("Int64", "UInt16"):
            case ("Int64", "UInt32"):
            case ("Int64", "UInt64"):
            case ("Byte", "SByte"):
            case ("UInt16", "SByte"):
            case ("UInt16", "Int16"):
            case ("UInt16", "Byte"):
            case ("UInt32", "SByte"):
            case ("UInt32", "Int16"):
            case ("UInt32", "Int32"):
            case ("UInt32", "Byte"):
            case ("UInt32", "UInt16"):
            case ("UInt64", "SByte"):
            case ("UInt64", "Int16"):
            case ("UInt64", "Int32"):
            case ("UInt64", "Int64"):
            case ("UInt64", "Byte"):
            case ("UInt64", "UInt16"):
            case ("UInt64", "UInt32"):
            case ("Char", "SByte"):
            case ("Char", "Byte"):
            case ("Char", "Int16"):
            case ("Char", "UInt16"):
            case ("Char", "Int32"):
            case ("Char", "UInt32"):
            case ("Char", "Int64"):
            case ("Char", "UInt64"):
            case ("SByte", "Char"):
            case ("Byte", "Char"):
            case ("Int16", "Char"):
            case ("UInt16", "Char"):
            case ("Int32", "Char"):
            case ("UInt32", "Char"):
            case ("Int64", "Char"):
            case ("UInt64", "Char"):
                {
                    EnsureIntegerInRange(tType);
                }
                break;
        }
    }
}
