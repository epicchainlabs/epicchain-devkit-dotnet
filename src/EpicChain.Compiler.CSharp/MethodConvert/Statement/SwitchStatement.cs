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
