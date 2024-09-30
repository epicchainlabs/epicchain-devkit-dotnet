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
    /// Convert literal expression to NeoVM instructions
    /// </summary>
    /// <param name="model">The semantic model of the method</param>
    /// <param name="expression">The literal expression to convert</param>
    /// <example>
    /// <code>
    /// public void ExampleMethod()
    /// {
    ///     int intLiteral = 42;
    ///     string stringLiteral = "Hello, Neo!";
    ///     bool boolLiteral = true;
    ///     object nullLiteral = null;
    ///     int defaultLiteral = default;
    /// }
    /// </code>
    /// </example>
    private void ConvertLiteralExpression(SemanticModel model, LiteralExpressionSyntax expression)
    {
        if (expression.IsKind(SyntaxKind.DefaultLiteralExpression))
        {
            ConvertDefaultLiteralExpression(model, expression);
        }
        else if (expression.IsKind(SyntaxKind.NullLiteralExpression))
        {
            AddInstruction(OpCode.PUSHNULL);
        }
        else
        {
            throw new CompilationException(expression, DiagnosticId.SyntaxNotSupported, $"Unsupported syntax: {expression}");
        }
    }

    /// <summary>
    /// Convert default literal expression to NeoVM instructions
    /// </summary>
    /// <param name="model">The semantic model of the method</param>
    /// <param name="expression">The literal expression to convert</param>
    /// <example>
    /// <code>
    /// public void ExampleMethod()
    /// {
    ///     int defaultLiteral = default;
    /// }
    /// </code>
    /// </example>
    private void ConvertDefaultLiteralExpression(SemanticModel model, LiteralExpressionSyntax expression)
    {
        var type = model.GetTypeInfo(expression).Type;
        if (type == null)
        {
            throw new CompilationException(expression, DiagnosticId.SyntaxNotSupported, "Cannot determine type for default expression");
        }

        switch (type.SpecialType)
        {
            case SpecialType.System_Boolean:
                {
                    AddInstruction(OpCode.PUSHF);
                    break;
                }
            case SpecialType.System_Byte:
            case SpecialType.System_SByte:
            case SpecialType.System_Int16:
            case SpecialType.System_UInt16:
            case SpecialType.System_Int32:
            case SpecialType.System_UInt32:
            case SpecialType.System_Int64:
            case SpecialType.System_UInt64:
            case SpecialType.System_Decimal:
            case SpecialType.System_Single:
            case SpecialType.System_Double:
            case SpecialType.System_Char:
                AddInstruction(OpCode.PUSH0);
                break;
            case SpecialType.System_String:
            case SpecialType.System_Object:
                AddInstruction(OpCode.PUSHNULL);
                break;
            default:
                if (type.ToString() == "System.Numerics.BigInteger")
                {
                    // BigInteger's default value is 0
                    AddInstruction(OpCode.PUSH0);
                }
                else if (type.IsReferenceType)
                {
                    AddInstruction(OpCode.PUSHNULL);
                }
                else if (type.IsValueType)
                {
                    // For structs and other value types, we need to create a default instance
                    AddInstruction(OpCode.NEWSTRUCT0);
                }
                else
                {
                    throw new CompilationException(expression, DiagnosticId.SyntaxNotSupported, $"Unsupported type for default expression: {type}");
                }
                break;
        }
    }
}
