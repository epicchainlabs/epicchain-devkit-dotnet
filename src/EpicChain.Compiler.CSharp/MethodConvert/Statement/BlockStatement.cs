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
using System.Collections.Generic;

namespace EpicChain.Compiler
{
    internal partial class MethodConvert
    {
        /// <summary>
        /// Converts a block statement to OpCodes. This method is used for parsing
        /// the syntax of block statements within the context of a semantic model. A block statement
        /// typically consists of a series of statements enclosed in braces `{}`.
        /// </summary>
        /// <param name="model">The semantic model that provides information about the block statement.</param>
        /// <param name="syntax">The syntax of the block statement to be converted.</param>
        /// <remarks>
        /// This method starts by initializing a new list of local symbols for the current block.
        /// It then iterates through each statement within the block, converting each to the appropriate
        /// set of instructions. Local symbols are tracked and removed once the block is fully converted.
        /// </remarks>
        /// <example>
        /// Here is an example of a block statement syntax:
        ///
        /// <code>
        /// {
        ///     string x = "Hello world.";
        ///     Runtime.Log(x);
        /// }
        /// </code>
        ///
        /// In this example, the block contains two statements: a variable declaration and
        /// a method call.
        /// </example>
        private void ConvertBlockStatement(SemanticModel model, BlockSyntax syntax)
        {
            _blockSymbols.Push(new List<ILocalSymbol>());
            using (InsertSequencePoint(syntax.OpenBraceToken))
                AddInstruction(OpCode.NOP);
            foreach (StatementSyntax child in syntax.Statements)
                ConvertStatement(model, child);
            using (InsertSequencePoint(syntax.CloseBraceToken))
                AddInstruction(OpCode.NOP);
            foreach (ILocalSymbol symbol in _blockSymbols.Pop())
                RemoveLocalVariable(symbol);
        }
    }
}
