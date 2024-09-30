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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using EpicChain.VM;

namespace EpicChain.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// Attempts to process system operators. Performs different processing operations based on the method symbol.
    /// </summary>
    /// <param name="model">The semantic model used to obtain detailed information about the symbol.</param>
    /// <param name="symbol">The method symbol to be processed.</param>
    /// <param name="arguments">An array of expression parameters.</param>
    /// <returns>True if system operators are successfully processed; otherwise, false.</returns>
    private bool TryProcessSystemOperators(SemanticModel model, IMethodSymbol symbol, params ExpressionSyntax[] arguments)
    {
        switch (symbol.ToString())
        {
            //Handles cases of equality operator (==), comparing whether two objects or strings are equal.
            case "object.operator ==(object, object)":
            case "string.operator ==(string, string)":
                ConvertExpression(model, arguments[0]);
                ConvertExpression(model, arguments[1]);
                AddInstruction(OpCode.EQUAL);
                return true;
            //Handles cases of inequality operator (!=), comparing whether two objects are not equal.
            case "object.operator !=(object, object)":
                ConvertExpression(model, arguments[0]);
                ConvertExpression(model, arguments[1]);
                AddInstruction(OpCode.NOTEQUAL);
                return true;
            //Handles cases of string concatenation operator (+), concatenating two strings into one.
            case "string.operator +(string, string)":
                ConvertExpression(model, arguments[0]);
                ConvertExpression(model, arguments[1]);
                AddInstruction(OpCode.CAT);
                ChangeType(VM.Types.StackItemType.ByteString);
                return true;
            //Handles cases of string concatenation operator (+), concatenating a string with an object.
            //Unsupported interpolation: object
            case "string.operator +(string, object)":
                ConvertExpression(model, arguments[0]);
                ConvertObjectToString(model, arguments[1]);
                AddInstruction(OpCode.CAT);
                ChangeType(VM.Types.StackItemType.ByteString);
                return true;
            //Handles cases of string concatenation operator (+), concatenating an object with a string.
            //Unsupported interpolation: object
            case "string.operator +(object, string)":
                ConvertObjectToString(model, arguments[0]);
                ConvertExpression(model, arguments[1]);
                AddInstruction(OpCode.CAT);
                ChangeType(VM.Types.StackItemType.ByteString);
                return true;
            default:
                return false;
        }
    }
}
