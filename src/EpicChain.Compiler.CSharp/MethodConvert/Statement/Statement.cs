// Copyright (C) 2021-2024 EpicChain Lab's
//
// The EpicChain.Compiler.CSharp is open-source software that is distributed under the widely recognized and permissive MIT License.
// This software is intended to provide developers with a powerful framework to create and deploy smart contracts on the EpicChain blockchain,
// and it is made freely available to all individuals and organizations. Whether you are building for personal, educational, or commercial
// purposes, you are welcome to utilize this framework with minimal restrictions, promoting the spirit of open innovation and collaborative
// development within the blockchain ecosystem.
//
// As a permissive license, the MIT License allows for broad usage rights, granting you the freedom to redistribute, modify, and adapt the
// source code or its binary versions as needed. You are permitted to incorporate the EpicChain Lab's Project into your own
// projects, whether for profit or non-profit, and may make changes to suit your specific needs. There is no requirement to make your
// modifications open-source, though doing so contributes to the overall growth of the open-source community.
//
// To comply with the terms of the MIT License, we kindly ask that you include a copy of the original copyright notice, this permission
// notice, and the license itself in all substantial portions of the software that you redistribute, whether in source code form or as
// compiled binaries. The purpose of this requirement is to ensure that future users and developers are aware of the origin of the software,
// the freedoms granted to them, and the limitations of liability that apply.
//
// The complete terms and conditions of the MIT License are documented in the LICENSE file that accompanies this project. This file can be
// found in the main directory of the source code repository. Alternatively, you may access the license text online at the following URL:
// http://www.opensource.org/licenses/mit-license.php. We encourage you to review these terms in detail to fully understand your rights
// and responsibilities when using this software.
//
// Redistribution and use of the EpicChain Lab's Project, whether in source or binary forms, with or without modification, are
// permitted as long as the following conditions are met:
//
// 1. The original copyright notice, along with this permission notice, must be retained in all copies or significant portions of the software.
// 2. The software is provided "as-is," without any express or implied warranty. This means that the authors and contributors are not
//    responsible for any issues that may arise from the use of the software, including but not limited to damages caused by defects or
//    performance issues. Users assume all responsibility for determining the suitability of the software for their specific needs.
//
// In addition to the above terms, the authors of the EpicChain Lab's Project encourage developers to explore and experiment
// with the framework's capabilities. Whether you are an individual developer, a startup, or a large organization, you are invited to
// leverage the power of blockchain technology to create decentralized applications, smart contracts, and more. We believe that by fostering
// a robust ecosystem of developers and contributors, we can help drive innovation in the blockchain space and unlock new opportunities
// for distributed ledger technology.
//
// However, please note that while the MIT License allows for modifications and redistribution, it does not imply endorsement of any
// derived works by the original authors. Therefore, if you significantly modify the EpicChain Lab's Project and redistribute it
// under your own brand or as part of a larger project, you must clearly indicate the changes you have made, and the original authors
// cannot be held liable for any issues resulting from your modifications.
//
// By choosing to use the EpicChain Lab's Project, you acknowledge that you have read and understood the terms of the MIT License.
// You agree to abide by these terms and recognize that this software is provided without warranty of any kind, express or implied, including
// but not limited to warranties of merchantability, fitness for a particular purpose, or non-infringement. Should any legal issues or
// disputes arise as a result of using this software, the authors and contributors disclaim all liability and responsibility.
//
// Finally, we encourage all users of the EpicChain Lab's Project to consider contributing back to the community. Whether through
// bug reports, feature suggestions, or code contributions, your involvement helps improve the framework for everyone. Open-source projects
// thrive when developers collaborate and share their knowledge, and we welcome your input as we continue to develop and refine the
// EpicChain ecosystem.


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
