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
    /// Converts the code for constructing arrays and initializing arrays into OpCodes.
    /// This method includes analyzing the array length, array type, array dimension and initial data.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about the array creation.</param>
    /// <param name="expression">The syntax representation of the array creation statement being converted.</param>
    /// <exception cref="CompilationException">Only one-dimensional arrays are supported, otherwise an exception is thrown.</exception>
    /// <remarks>
    /// When the array is initialized to null, this code converts it to "array length" + OpCode.NEWBUFFER (only for byte[]) or  OpCode.NEWARRAY_T.
    /// When the array is not initialized to null, this code converts the initialized constants one by one in reverse order, then adds the "array length" and OpCode.PACK
    /// </remarks>
    /// <example>
    /// Example of a array creation syntax:
    /// <c>var array = new byte[4];</c>
    /// The compilation result of the example code is: OpCode.PUSH4, OpCode.NEWBUFFER
    /// <c>var array = new int[4] { 5, 6, 7, 8};</c>
    /// The compilation result of the example code is: OpCode.PUSH8, OpCode.PUSH7, OpCode.PUSH6, OpCode.PUSH5, OpCode.PUSH4, OpCode.PACK
    /// </example>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/arrays">Arrays</seealso>
    private void ConvertArrayCreationExpression(SemanticModel model, ArrayCreationExpressionSyntax expression)
    {
        ArrayRankSpecifierSyntax specifier = expression.Type.RankSpecifiers[0];
        if (specifier.Rank != 1)
            throw new CompilationException(specifier, DiagnosticId.MultidimensionalArray, $"Unsupported array rank: {specifier}");
        IArrayTypeSymbol type = (IArrayTypeSymbol)model.GetTypeInfo(expression.Type).Type!;
        if (expression.Initializer is null)
        {
            ConvertExpression(model, specifier.Sizes[0]);
            if (type.ElementType.SpecialType == SpecialType.System_Byte)
                AddInstruction(OpCode.NEWBUFFER);
            else
                AddInstruction(new Instruction { OpCode = OpCode.NEWARRAY_T, Operand = new[] { (byte)type.ElementType.GetStackItemType() } });
        }
        else
        {
            ConvertInitializerExpression(model, type, expression.Initializer);
        }
    }
}
