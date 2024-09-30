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

namespace EpicChain.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// Convert pattern to OpCodes.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="pattern"></param>
    /// <param name="localIndex"></param>
    /// <exception cref="CompilationException"></exception>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/patterns#logical-patterns">
    /// Pattern matching - the is and switch expressions, and operators and, or and not in patterns
    /// </seealso>
    private void ConvertPattern(SemanticModel model, PatternSyntax pattern, byte localIndex)
    {
        switch (pattern)
        {
            //Convet "and" / "or" pattern  to OpCodes.
            //Example: return value is > 1 and < 100;
            //Example: return value is >= 80 or <= 20;
            case BinaryPatternSyntax binaryPattern:
                ConvertBinaryPattern(model, binaryPattern, localIndex);
                break;
            //Convet constant pattern to OpCodes.
            //Example: return value is > 1;
            //Example: return value is null;
            case ConstantPatternSyntax constantPattern:
                ConvertConstantPattern(model, constantPattern, localIndex);
                break;
            //Convet declaration pattern to OpCodes.
            //Example: if (greeting is string message)
            case DeclarationPatternSyntax declarationPattern:
                ConvertDeclarationPattern(model, declarationPattern, localIndex);
                break;
            //Convet discard pattern (_) to OpCodes.
            //Example: if (greeting2 is string _)
            case DiscardPatternSyntax:
                Push(true);
                break;
            //Convet relational pattern to OpCodes.
            //Example: return value is > 1;
            case RelationalPatternSyntax relationalPattern:
                ConvertRelationalPattern(model, relationalPattern, localIndex);
                break;
            //Convert type pattern to OpCodes.
            //Example:
            //switch (o1)
            //{
            //    case byte[]: break;
            //    case string: break;
            //}
            case TypePatternSyntax typePattern:
                ConvertTypePattern(model, typePattern, localIndex);
                break;
            //Convet "not" pattern  to OpCodes.
            //Example: return value is not null;
            case UnaryPatternSyntax unaryPattern when unaryPattern.OperatorToken.ValueText == "not":
                ConvertNotPattern(model, unaryPattern, localIndex);
                break;
            //Convet parenthesized to OpCodes.
            //Example: return value is (> 1 and < 100);
            case ParenthesizedPatternSyntax parenthesizedPattern:
                ConvertParenthesizedPatternSyntax(model, parenthesizedPattern, localIndex);
                break;
            case RecursivePatternSyntax recursivePattern:
                ConvertRecursivePattern(model, recursivePattern, localIndex);
                break;
            default:
                //Example:
                //object greeting = "Hello, World!";
                //if (greeting3 is var message) { }
                //Example:
                //public static void M(object o1, object o2)
                //{
                //  var t = (o1, o2);
                //  if (t is (int, string)) { }
                //}
                throw new CompilationException(pattern, DiagnosticId.SyntaxNotSupported, $"Unsupported pattern: {pattern}");
        }
    }
}
