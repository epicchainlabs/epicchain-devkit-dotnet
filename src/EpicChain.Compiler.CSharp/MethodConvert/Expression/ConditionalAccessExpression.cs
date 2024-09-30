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
    /// This method converts a null-conditional access expression to OpCodes.
    /// </summary>
    /// /// <param name="model">The semantic model providing context and information about null-conditional access expression.</param>
    /// <param name="expression">The syntax representation of the null-conditional access expression statement being converted.</param>
    /// <remarks>
    /// The method evaluates the expression and checks if it is null.
    /// If the expression is not null, it converts the 'WhenNotNull' part of the expression.
    /// If the resulting type of the expression is 'System.Void', it handles the case differently by dropping the result.
    /// A null-conditional operator applies a member access (?.) or element access (?[]) operation to its operand only if that operand evaluates to non-null;
    /// otherwise, it returns null.
    /// It will jump to <see cref="ConvertMemberBindingExpression"/> and <see cref="ConvertElementBindingExpression"/> to handle the case where the variable or array is not null.
    /// </remarks>
    /// <example>
    /// If Block is not null, get the block's timestamp; otherwise, it returns null.
    /// <code>
    /// var block = Ledger.GetBlock(10000);
    /// var timestamp = block?.Timestamp;
    /// Runtime.Log(timestamp.ToString());
    /// </code>
    /// If array is not null, get the array's element; otherwise, it returns null.
    /// <code>
    /// var a = Ledger.GetBlock(10000);
    /// var b = Ledger.GetBlock(10001);
    /// var array = new[] { a, b };
    /// var firstItem = array?[0];
    /// Runtime.Log(firstItem?.Timestamp.ToString());
    /// </code>
    /// </example>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/member-access-operators#null-conditional-operators--and-">Null-conditional operators ?. and ?[]</seealso>
    private void ConvertConditionalAccessExpression(SemanticModel model, ConditionalAccessExpressionSyntax expression)
    {
        ITypeSymbol type = model.GetTypeInfo(expression).Type!;
        JumpTarget nullTarget = new();
        ConvertExpression(model, expression.Expression);
        AddInstruction(OpCode.DUP);
        AddInstruction(OpCode.ISNULL);
        Jump(OpCode.JMPIF_L, nullTarget);
        ConvertExpression(model, expression.WhenNotNull);
        if (type.SpecialType == SpecialType.System_Void)
        {
            JumpTarget endTarget = new();
            Jump(OpCode.JMP_L, endTarget);
            nullTarget.Instruction = AddInstruction(OpCode.DROP);
            endTarget.Instruction = AddInstruction(OpCode.NOP);
        }
        else
        {
            nullTarget.Instruction = AddInstruction(OpCode.NOP);
        }
    }
}
