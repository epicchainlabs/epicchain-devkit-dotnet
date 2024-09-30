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


using Microsoft.CodeAnalysis.CSharp.Syntax;
using EpicChain.VM;

namespace EpicChain.Compiler
{
    internal partial class MethodConvert
    {
        /// <summary>
        /// Converts a break statement into the corresponding jump instruction. This method handles
        /// the parsing and translation of a break statement within loop or switch constructs, converting
        /// it to an appropriate jump instruction in the neo vm language.
        /// </summary>
        /// <param name="syntax">The syntax of the break statement to be converted.</param>
        /// <remarks>
        /// This method determines the target of the break statement based on the current context. If the
        /// break statement is within a try block with no specific break target, it generates an `ENDTRY_L`
        /// jump instruction to exit the try block. Otherwise, it generates a `JMP_L` instruction to jump
        /// to the standard break target. This ensures that break statements behave correctly in both
        /// normal loops/switches and those within try-catch blocks.
        /// </remarks>
        /// <example>
        /// An example of a break statement syntax in a loop:
        ///
        /// <code>
        /// for (int i = 0; i < 10; i++)
        /// {
        ///     if (i == 5)
        ///         break;
        /// }
        /// </code>
        ///
        /// In this example, the break statement exits the loop when `i` equals 5.
        /// </example>
        private void ConvertBreakStatement(BreakStatementSyntax syntax)
        {
            using (InsertSequencePoint(syntax))
                if (_tryStack.TryPeek(out ExceptionHandling? result) && result.BreakTargetCount == 0)
                    Jump(OpCode.ENDTRY_L, _breakTargets.Peek());
                else
                    Jump(OpCode.JMP_L, _breakTargets.Peek());
        }
    }
}
