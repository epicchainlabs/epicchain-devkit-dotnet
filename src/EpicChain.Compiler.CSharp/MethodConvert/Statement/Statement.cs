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
    internal partial class MethodConvert
    {
        private void ConvertStatement(SemanticModel model, StatementSyntax statement)
        {
            switch (statement)
            {
                // Converts a block statement, which is a series of statements enclosed in braces.
                // Example: { int x = 1; Console.WriteLine(x); }
                case BlockSyntax syntax:
                    ConvertBlockStatement(model, syntax);
                    break;
                // Converts a break statement, typically used within loops or switch cases.
                // Example: break;
                case BreakStatementSyntax syntax:
                    ConvertBreakStatement(syntax);
                    break;
                // Converts a checked statement, used for arithmetic operations with overflow checking.
                // Example: checked { int x = int.MaxValue; }
                case CheckedStatementSyntax syntax:
                    ConvertCheckedStatement(model, syntax);
                    break;
                // Converts a continue statement, used to skip the current iteration of a loop.
                // Example: continue;
                case ContinueStatementSyntax syntax:
                    ConvertContinueStatement(syntax);
                    break;
                // Converts a do-while loop statement.
                // Example: do { /* actions */ } while (condition);
                case DoStatementSyntax syntax:
                    ConvertDoStatement(model, syntax);
                    break;
                // Converts an empty statement, which is typically just a standalone semicolon.
                // Example: ;
                case EmptyStatementSyntax syntax:
                    ConvertEmptyStatement(syntax);
                    break;
                // Converts an expression statement, which is a statement consisting of a single expression.
                // Example: Console.WriteLine("Hello");
                case ExpressionStatementSyntax syntax:
                    ConvertExpressionStatement(model, syntax);
                    break;
                // Converts a foreach loop statement.
                // Example: foreach (var item in collection) { /* actions */ }
                case ForEachStatementSyntax syntax:
                    ConvertForEachStatement(model, syntax);
                    break;
                // Converts a foreach loop statement with variable declarations.
                // Example: foreach (var (key, value) in dictionary) { /* actions */ }
                case ForEachVariableStatementSyntax syntax:
                    ConvertForEachVariableStatement(model, syntax);
                    break;
                // Converts a for loop statement.
                // Example: for (int i = 0; i < 10; i++) { /* actions */ }
                case ForStatementSyntax syntax:
                    ConvertForStatement(model, syntax);
                    break;
                // Converts a goto statement, used for jumping to a labeled statement.
                // Example: goto myLabel;
                case GotoStatementSyntax syntax:
                    ConvertGotoStatement(model, syntax);
                    break;
                // Converts an if statement, including any else or else if branches.
                // Example: if (condition) { /* actions */ } else { /* actions */ }
                case IfStatementSyntax syntax:
                    ConvertIfStatement(model, syntax);
                    break;
                // Converts a labeled statement, used as a target for goto statements.
                // Example: myLabel: /* actions */
                case LabeledStatementSyntax syntax:
                    ConvertLabeledStatement(model, syntax);
                    break;
                // Converts a local variable declaration statement.
                // Example: int x = 5;
                case LocalDeclarationStatementSyntax syntax:
                    ConvertLocalDeclarationStatement(model, syntax);
                    break;
                // Currently, local function statements are not supported in this context.
                case LocalFunctionStatementSyntax:
                    break;
                // Converts a return statement, used to exit a method and optionally return a value.
                // Example: return x + y;
                case ReturnStatementSyntax syntax:
                    ConvertReturnStatement(model, syntax);
                    break;
                // Converts a switch statement, including its cases and default case.
                // Example: switch (variable) { case 1: /* actions */ break; default: /* actions */
                case SwitchStatementSyntax syntax:
                    ConvertSwitchStatement(model, syntax);
                    break;
                // Converts a throw statement, used for exception handling.
                // Example: throw new Exception("Error");
                case ThrowStatementSyntax syntax:
                    ConvertThrowStatement(model, syntax);
                    break;
                // Converts a try-catch-finally statement, used for exception handling.
                // Example: try { /* actions */ } catch (Exception e) { /* actions */ } finally { /* actions */ }
                case TryStatementSyntax syntax:
                    ConvertTryStatement(model, syntax);
                    break;
                // Converts a while loop statement.
                // Example: while (condition) { /* actions */ }
                case WhileStatementSyntax syntax:
                    ConvertWhileStatement(model, syntax);
                    break;
                default:
                    throw new CompilationException(statement, DiagnosticId.SyntaxNotSupported, $"Unsupported syntax: {statement}");
            }
        }
    }
}
