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
using EpicChain.VM;
using System.Linq;

namespace EpicChain.Compiler
{
    internal partial class MethodConvert
    {
        /// <summary>
        /// Converts a 'switch' statement into a set of conditional jump instructions and targets.
        /// This method handles the translation of 'switch' statements, including various forms of
        /// case labels (like pattern matching cases and default cases) into executable instructions.
        /// </summary>
        /// <param name="model">The semantic model providing context and information about the switch statement.</param>
        /// <param name="syntax">The syntax representation of the switch statement being converted.</param>
        /// <remarks>
        /// The method first evaluates the switch expression and then iterates over the different cases,
        /// generating conditional jumps based on the case labels. It supports pattern matching,
        /// value comparison, and default cases. The method ensures the correct control flow between
        /// different cases and handles the 'break' statement logic for exiting the switch.
        /// </remarks>
        /// <example>
        /// Example of a switch statement syntax:
        /// <code>
        /// switch (expression)
        /// {
        ///     case 1:
        ///         // Code for case 1
        ///         break;
        ///     case 2 when condition:
        ///         // Code for case 2 with condition
        ///         break;
        ///     default:
        ///         // Default case code
        ///         break;
        /// }
        /// </code>
        /// In this example, the switch statement includes different case scenarios, including a
        /// conditional case and a default case.
        /// </example>
        private void ConvertSwitchStatement(SemanticModel model, SwitchStatementSyntax syntax)
        {
            var sections = syntax.Sections.Select(p => (p.Labels, p.Statements, Target: new JumpTarget())).ToArray();
            var labels = sections.SelectMany(p => p.Labels, (p, l) => (l, p.Target)).ToArray();
            PushSwitchLabels(labels);
            JumpTarget breakTarget = new();
            byte anonymousIndex = AddAnonymousVariable();
            PushBreakTarget(breakTarget);
            using (InsertSequencePoint(syntax.Expression))
            {
                ConvertExpression(model, syntax.Expression);
                AccessSlot(OpCode.STLOC, anonymousIndex);
            }
            foreach (var (label, target) in labels)
            {
                switch (label)
                {
                    case CasePatternSwitchLabelSyntax casePatternSwitchLabel:
                        using (InsertSequencePoint(casePatternSwitchLabel))
                        {
                            JumpTarget endTarget = new();
                            ConvertPattern(model, casePatternSwitchLabel.Pattern, anonymousIndex);
                            Jump(OpCode.JMPIFNOT_L, endTarget);
                            if (casePatternSwitchLabel.WhenClause is not null)
                            {
                                ConvertExpression(model, casePatternSwitchLabel.WhenClause.Condition);
                                Jump(OpCode.JMPIFNOT_L, endTarget);
                            }
                            Jump(OpCode.JMP_L, target);
                            endTarget.Instruction = AddInstruction(OpCode.NOP);
                        }
                        break;
                    case CaseSwitchLabelSyntax caseSwitchLabel:
                        using (InsertSequencePoint(caseSwitchLabel))
                        {
                            AccessSlot(OpCode.LDLOC, anonymousIndex);
                            ConvertExpression(model, caseSwitchLabel.Value);
                            AddInstruction(OpCode.EQUAL);
                            Jump(OpCode.JMPIF_L, target);
                        }
                        break;
                    case DefaultSwitchLabelSyntax defaultSwitchLabel:
                        using (InsertSequencePoint(defaultSwitchLabel))
                        {
                            Jump(OpCode.JMP_L, target);
                        }
                        break;
                    default:
                        throw new CompilationException(label, DiagnosticId.SyntaxNotSupported, $"Unsupported syntax: {label}");
                }
            }
            RemoveAnonymousVariable(anonymousIndex);
            Jump(OpCode.JMP_L, breakTarget);
            foreach (var (_, statements, target) in sections)
            {
                target.Instruction = AddInstruction(OpCode.NOP);
                foreach (StatementSyntax statement in statements)
                    ConvertStatement(model, statement);
            }
            breakTarget.Instruction = AddInstruction(OpCode.NOP);
            PopSwitchLabels();
            PopBreakTarget();
        }
    }
}
