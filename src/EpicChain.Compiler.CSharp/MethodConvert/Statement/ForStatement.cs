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
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using EpicChain.VM;
using System.Linq;

namespace EpicChain.Compiler
{
    internal partial class MethodConvert
    {
        /// <summary>
        /// Converts a 'for' loop statement into OpCodes. This method handles the parsing
        /// and translation of the 'for' loop construct, including its initialization, condition, and
        /// increment expressions, as well as the loop body.
        /// </summary>
        /// <param name="model">The semantic model providing context and information about the 'for' loop statement.</param>
        /// <param name="syntax">The syntax representation of the 'for' loop statement being converted.</param>
        /// <remarks>
        /// The method starts by handling variable declarations and initializations. It then sets up
        /// jump targets for managing the loop's control flow: start, continue, condition, and break.
        /// The loop's body, condition, and increment expressions are then converted into appropriate
        /// instructions. The method also ensures any non-void expressions that leave a value on the stack
        /// are appropriately dropped.
        /// </remarks>
        /// <example>
        /// Example of a 'for' loop syntax:
        /// <code>
        /// for (int i = 0; i < 10; i++)
        /// {
        ///     // Loop body
        /// }
        /// </code>
        /// This example shows a 'for' loop where 'i' is initialized to 0, the loop continues as long
        /// as 'i' is less than 10, and 'i' is incremented by 1 after each iteration.
        /// </example>
        private void ConvertForStatement(SemanticModel model, ForStatementSyntax syntax)
        {
            var variables = (syntax.Declaration?.Variables ?? Enumerable.Empty<VariableDeclaratorSyntax>())
                .Select(p => (p, (ILocalSymbol)model.GetDeclaredSymbol(p)!))
                .ToArray();
            foreach (ExpressionSyntax expression in syntax.Initializers)
                using (InsertSequencePoint(expression))
                {
                    ITypeSymbol type = model.GetTypeInfo(expression).Type!;
                    ConvertExpression(model, expression);
                    if (type.SpecialType != SpecialType.System_Void)
                        AddInstruction(OpCode.DROP);
                }
            JumpTarget startTarget = new();
            JumpTarget continueTarget = new();
            JumpTarget conditionTarget = new();
            JumpTarget breakTarget = new();
            PushContinueTarget(continueTarget);
            PushBreakTarget(breakTarget);
            foreach (var (variable, symbol) in variables)
            {
                byte variableIndex = AddLocalVariable(symbol);
                if (variable.Initializer is not null)
                    using (InsertSequencePoint(variable))
                    {
                        ConvertExpression(model, variable.Initializer.Value);
                        AccessSlot(OpCode.STLOC, variableIndex);
                    }
            }
            Jump(OpCode.JMP_L, conditionTarget);
            startTarget.Instruction = AddInstruction(OpCode.NOP);
            ConvertStatement(model, syntax.Statement);
            continueTarget.Instruction = AddInstruction(OpCode.NOP);
            foreach (ExpressionSyntax expression in syntax.Incrementors)
                using (InsertSequencePoint(expression))
                {
                    ITypeSymbol type = model.GetTypeInfo(expression).Type!;
                    ConvertExpression(model, expression);
                    if (type.SpecialType != SpecialType.System_Void)
                        AddInstruction(OpCode.DROP);
                }
            conditionTarget.Instruction = AddInstruction(OpCode.NOP);
            if (syntax.Condition is null)
            {
                Jump(OpCode.JMP_L, startTarget);
            }
            else
            {
                ConvertExpression(model, syntax.Condition);
                Jump(OpCode.JMPIF_L, startTarget);
            }
            breakTarget.Instruction = AddInstruction(OpCode.NOP);
            foreach (var (_, symbol) in variables)
                RemoveLocalVariable(symbol);
            PopContinueTarget();
            PopBreakTarget();
        }
    }
}
