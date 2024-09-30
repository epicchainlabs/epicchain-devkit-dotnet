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

namespace EpicChain.Compiler
{
    internal partial class MethodConvert
    {
        /// <summary>
        /// Converts a 'do-while' loop statement into a corresponding set of instructions.
        /// This method handles the parsing and translation of the 'do-while' loop construct,
        /// creating the necessary control flow for the loop's execution.
        /// </summary>
        /// <param name="model">The semantic model providing context and information about the 'do-while' statement.</param>
        /// <param name="syntax">The syntax representation of the 'do-while' statement being converted.</param>
        /// <remarks>
        /// The method sets up jump targets for the start, continue, and break points of the loop.
        /// It then converts the loop's statement body and condition. The loop continues if the condition
        /// is true, jumping back to the start. If the condition is false, the loop exits, and the control
        /// flow moves to the break target.
        /// </remarks>
        /// <example>
        /// Example of a 'do-while' loop syntax:
        /// <code>
        /// do
        /// {
        ///     // Loop body
        /// }
        /// while (condition);
        /// </code>
        /// This example shows a 'do-while' loop where the loop body is executed at least once
        /// before evaluating the condition.
        /// </example>
        private void ConvertDoStatement(SemanticModel model, DoStatementSyntax syntax)
        {
            JumpTarget startTarget = new();
            JumpTarget continueTarget = new();
            JumpTarget breakTarget = new();
            PushContinueTarget(continueTarget);
            PushBreakTarget(breakTarget);
            startTarget.Instruction = AddInstruction(OpCode.NOP);
            ConvertStatement(model, syntax.Statement);
            continueTarget.Instruction = AddInstruction(OpCode.NOP);
            ConvertExpression(model, syntax.Condition);
            Jump(OpCode.JMPIF_L, startTarget);
            breakTarget.Instruction = AddInstruction(OpCode.NOP);
            PopContinueTarget();
            PopBreakTarget();
        }
    }
}
