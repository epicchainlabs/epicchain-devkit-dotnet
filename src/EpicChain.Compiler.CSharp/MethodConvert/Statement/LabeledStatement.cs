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

namespace EpicChain.Compiler
{
    internal partial class MethodConvert
    {
        /// <summary>
        /// Converts a labeled statement into a jump target and its associated instructions.
        /// This method is used for handling labeled statements, creating a target for 'goto' statements
        /// to jump to and processing the subsequent statement after the label.
        /// </summary>
        /// <param name="model">The semantic model providing context and information about the labeled statement.</param>
        /// <param name="syntax">The syntax representation of the labeled statement being converted.</param>
        /// <remarks>
        /// The method first declares a new jump target for the label. If the label is referenced in
        /// any pending 'goto' statements within a try block, it updates those 'goto' instructions
        /// to jump to the newly created label. Then, it converts the statement following the label.
        /// This is essential in implementing the label and goto mechanism found in many programming languages.
        /// </remarks>
        /// <example>
        /// Example of a labeled statement syntax:
        /// <code>
        /// myLabel:
        ///     // Code to execute after jumping to this label
        ///
        /// // Elsewhere in the code
        /// goto myLabel;
        /// </code>
        /// This example shows a labeled statement 'myLabel' and a 'goto' statement that jumps to it.
        /// </example>
        private void ConvertLabeledStatement(SemanticModel model, LabeledStatementSyntax syntax)
        {
            ILabelSymbol symbol = model.GetDeclaredSymbol(syntax)!;
            JumpTarget target = AddLabel(symbol, true);
            if (_tryStack.TryPeek(out ExceptionHandling? result))
                foreach (Instruction instruction in result.PendingGotoStatments)
                    if (instruction.Target == target)
                        instruction.OpCode = OpCode.JMP_L;
            target.Instruction = AddInstruction(OpCode.NOP);
            ConvertStatement(model, syntax.Statement);
        }
    }
}
