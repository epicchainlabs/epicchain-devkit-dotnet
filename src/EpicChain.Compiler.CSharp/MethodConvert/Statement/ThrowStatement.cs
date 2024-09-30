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

namespace EpicChain.Compiler
{
    /// <summary>
    /// Converts a 'throw' statement into the corresponding throw instruction.
    /// This method handles the translation of a throw statement, typically used for exception handling,
    /// into an intermediate language instruction.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about the throw statement.</param>
    /// <param name="syntax">The syntax representation of the throw statement being converted.</param>
    /// <remarks>
    /// The method takes a throw statement and converts the expression being thrown into
    /// an appropriate throw instruction in the target language or intermediate representation.
    /// This is essential for implementing exception handling in the converted code.
    /// </remarks>
    /// <example>
    /// Example of a throw statement syntax:
    /// <code>
    /// throw new Exception("Error message");
    /// </code>
    /// This example demonstrates a throw statement that creates and throws a new exception
    /// with a specified error message.
    /// </example>
    internal partial class MethodConvert
    {
        private void ConvertThrowStatement(SemanticModel model, ThrowStatementSyntax syntax)
        {
            using (InsertSequencePoint(syntax))
                Throw(model, syntax.Expression);
        }
    }
}
