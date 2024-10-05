// Copyright (C) 2021-2024 EpicChain Lab's
//
// The EpicChain.Compiler.CSharp  MIT License allows for broad usage rights, granting you the freedom to redistribute, modify, and adapt the
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


extern alias scfx;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using EpicChain.VM;
using System;
using System.Runtime.InteropServices;

namespace EpicChain.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// Converts the code for null-coalescing assignment expression into OpCodes.
    /// The null-coalescing assignment operator ??= assigns the value of its right-hand operand to its left-hand operand only if the left-hand operand evaluates to null.
    /// The ??= operator doesn't evaluate its right-hand operand if the left-hand operand evaluates to non-null.
    /// Null-coalescing assignment expressions are a new feature introduced in C# 8.0(Released September, 2019).
    /// </summary>
    /// <param name="model">The semantic model providing context and information about coalesce assignment expression.</param>
    /// <param name="expression">The syntax representation of the coalesce assignment expression statement being converted.</param>
    /// <exception cref="CompilationException">Thrown when the syntax is not supported.</exception>
    /// <example>
    /// <code>
    /// public class Cat
    /// {
    ///     public string Name { get; set; }
    /// }
    /// </code>
    /// <code>
    /// Cat nullableCat = null;
    /// Cat nonNullableCat = new() { Name = "Mimi" };
    /// nullableCat ??= nonNullableCat;
    /// Runtime.Log("Nullable cat: " + nullableCat.Name);
    /// </code>
    /// <c>nullableCat ??= nonNullableCat;</c> this line is evaluated as
    /// <c>nullableCat = nullableCat ?? nonNullableCat;</c> is evaluated as <c>if (nullableCat == null) nullableCat = nonNullableCat;</c>
    /// </example>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-coalescing-operator">?? and ??= operators - the null-coalescing operators</seealso>
    private void ConvertCoalesceAssignmentExpression(SemanticModel model, AssignmentExpressionSyntax expression)
    {
        switch (expression.Left)
        {
            case ElementAccessExpressionSyntax left:
                ConvertElementAccessCoalesceAssignment(model, left, expression.Right);
                break;
            case IdentifierNameSyntax left:
                ConvertIdentifierNameCoalesceAssignment(model, left, expression.Right);
                break;
            case MemberAccessExpressionSyntax left:
                ConvertMemberAccessCoalesceAssignment(model, left, expression.Right);
                break;
            default:
                throw new CompilationException(expression, DiagnosticId.SyntaxNotSupported, $"Unsupported coalesce assignment: {expression}");
        }
    }

    private void ConvertElementAccessCoalesceAssignment(SemanticModel model, ElementAccessExpressionSyntax left, ExpressionSyntax right)
    {
        if (left.ArgumentList.Arguments.Count != 1)
            throw new CompilationException(left.ArgumentList, DiagnosticId.MultidimensionalArray, $"Unsupported array rank: {left.ArgumentList.Arguments}");
        JumpTarget assignmentTarget = new();
        JumpTarget endTarget = new();
        if (model.GetSymbolInfo(left).Symbol is IPropertySymbol property)
        {
            ConvertExpression(model, left.Expression);
            ConvertExpression(model, left.ArgumentList.Arguments[0].Expression);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.OVER);
            CallMethodWithConvention(model, property.GetMethod!, CallingConvention.StdCall);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.ISNULL);
            Jump(OpCode.JMPIF_L, assignmentTarget);
            AddInstruction(OpCode.NIP);
            AddInstruction(OpCode.NIP);
            Jump(OpCode.JMP_L, endTarget);
            assignmentTarget.Instruction = AddInstruction(OpCode.DROP);
            ConvertExpression(model, right);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.REVERSE4);
            CallMethodWithConvention(model, property.SetMethod!, CallingConvention.Cdecl);
        }
        else
        {
            ConvertExpression(model, left.Expression);
            ConvertExpression(model, left.ArgumentList.Arguments[0].Expression);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.PICKITEM);
            AddInstruction(OpCode.ISNULL);
            Jump(OpCode.JMPIF_L, assignmentTarget);
            AddInstruction(OpCode.PICKITEM);
            Jump(OpCode.JMP_L, endTarget);
            assignmentTarget.Instruction = AddInstruction(OpCode.NOP);
            ConvertExpression(model, right);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.REVERSE4);
            AddInstruction(OpCode.REVERSE3);
            AddInstruction(OpCode.SETITEM);
        }
        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }

    private void ConvertIdentifierNameCoalesceAssignment(SemanticModel model, IdentifierNameSyntax left, ExpressionSyntax right)
    {
        ISymbol symbol = model.GetSymbolInfo(left).Symbol!;
        switch (symbol)
        {
            case IFieldSymbol field:
                ConvertFieldIdentifierNameCoalesceAssignment(model, field, right);
                break;
            case ILocalSymbol local:
                ConvertLocalIdentifierNameCoalesceAssignment(model, local, right);
                break;
            case IParameterSymbol parameter:
                ConvertParameterIdentifierNameCoalesceAssignment(model, parameter, right);
                break;
            case IPropertySymbol property:
                ConvertPropertyIdentifierNameCoalesceAssignment(model, property, right);
                break;
            default:
                throw new CompilationException(left, DiagnosticId.SyntaxNotSupported, $"Unsupported symbol: {symbol}");
        }
    }

    private void ConvertMemberAccessCoalesceAssignment(SemanticModel model, MemberAccessExpressionSyntax left, ExpressionSyntax right)
    {
        ISymbol symbol = model.GetSymbolInfo(left).Symbol!;
        switch (symbol)
        {
            case IFieldSymbol field:
                ConvertFieldMemberAccessCoalesceAssignment(model, left, right, field);
                break;
            case IPropertySymbol property:
                ConvertPropertyMemberAccessCoalesceAssignment(model, left, right, property);
                break;
            default:
                throw new CompilationException(left, DiagnosticId.SyntaxNotSupported, $"Unsupported symbol: {symbol}");
        }
    }

    private void ConvertFieldIdentifierNameCoalesceAssignment(SemanticModel model, IFieldSymbol left, ExpressionSyntax right)
    {
        JumpTarget assignmentTarget = new();
        JumpTarget endTarget = new();
        if (left.IsStatic)
        {
            byte index = _context.AddStaticField(left);
            AccessSlot(OpCode.LDSFLD, index);
            AddInstruction(OpCode.ISNULL);
            Jump(OpCode.JMPIF_L, assignmentTarget);
            AccessSlot(OpCode.LDSFLD, index);
            Jump(OpCode.JMP_L, endTarget);
            assignmentTarget.Instruction = AddInstruction(OpCode.NOP);
            ConvertExpression(model, right);
            AddInstruction(OpCode.DUP);
            AccessSlot(OpCode.STSFLD, index);
        }
        else
        {
            int index = Array.IndexOf(left.ContainingType.GetFields(), left);
            AddInstruction(OpCode.LDARG0);
            Push(index);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.PICKITEM);
            AddInstruction(OpCode.ISNULL);
            Jump(OpCode.JMPIF_L, assignmentTarget);
            AddInstruction(OpCode.PICKITEM);
            Jump(OpCode.JMP_L, endTarget);
            assignmentTarget.Instruction = AddInstruction(OpCode.NOP);
            ConvertExpression(model, right);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.REVERSE4);
            AddInstruction(OpCode.REVERSE3);
            AddInstruction(OpCode.SETITEM);
        }
        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }

    private void ConvertLocalIdentifierNameCoalesceAssignment(SemanticModel model, ILocalSymbol left, ExpressionSyntax right)
    {
        JumpTarget assignmentTarget = new();
        JumpTarget endTarget = new();
        LdLocSlot(left);
        AddInstruction(OpCode.ISNULL);
        Jump(OpCode.JMPIF_L, assignmentTarget);
        LdLocSlot(left);
        Jump(OpCode.JMP_L, endTarget);
        assignmentTarget.Instruction = AddInstruction(OpCode.NOP);
        ConvertExpression(model, right);
        AddInstruction(OpCode.DUP);
        StLocSlot(left);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }

    private void ConvertParameterIdentifierNameCoalesceAssignment(SemanticModel model, IParameterSymbol left, ExpressionSyntax right)
    {
        JumpTarget assignmentTarget = new();
        JumpTarget endTarget = new();
        LdArgSlot(left);
        AddInstruction(OpCode.ISNULL);
        Jump(OpCode.JMPIF_L, assignmentTarget);
        LdArgSlot(left);
        Jump(OpCode.JMP_L, endTarget);
        assignmentTarget.Instruction = AddInstruction(OpCode.NOP);
        ConvertExpression(model, right);
        AddInstruction(OpCode.DUP);
        StArgSlot(left);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }

    private void ConvertPropertyIdentifierNameCoalesceAssignment(SemanticModel model, IPropertySymbol left, ExpressionSyntax right)
    {
        JumpTarget endTarget = new();
        if (left.IsStatic)
        {
            CallMethodWithConvention(model, left.GetMethod!);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.ISNULL);
            Jump(OpCode.JMPIFNOT_L, endTarget);
            AddInstruction(OpCode.DROP);
            ConvertExpression(model, right);
            AddInstruction(OpCode.DUP);
            CallMethodWithConvention(model, left.SetMethod!);
        }
        else
        {
            AddInstruction(OpCode.LDARG0);
            CallMethodWithConvention(model, left.GetMethod!);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.ISNULL);
            Jump(OpCode.JMPIFNOT_L, endTarget);
            AddInstruction(OpCode.DROP);
            AddInstruction(OpCode.LDARG0);
            ConvertExpression(model, right);
            AddInstruction(OpCode.TUCK);
            CallMethodWithConvention(model, left.SetMethod!, CallingConvention.StdCall);
        }
        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }

    private void ConvertFieldMemberAccessCoalesceAssignment(SemanticModel model, MemberAccessExpressionSyntax left, ExpressionSyntax right, IFieldSymbol field)
    {
        JumpTarget assignmentTarget = new();
        JumpTarget endTarget = new();
        if (field.IsStatic)
        {
            byte index = _context.AddStaticField(field);
            AccessSlot(OpCode.LDSFLD, index);
            AddInstruction(OpCode.ISNULL);
            Jump(OpCode.JMPIF_L, assignmentTarget);
            AccessSlot(OpCode.LDSFLD, index);
            Jump(OpCode.JMP_L, endTarget);
            assignmentTarget.Instruction = AddInstruction(OpCode.NOP);
            ConvertExpression(model, right);
            AddInstruction(OpCode.DUP);
            AccessSlot(OpCode.STSFLD, index);
        }
        else
        {
            int index = Array.IndexOf(field.ContainingType.GetFields(), field);
            ConvertExpression(model, left.Expression);
            Push(index);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.PICKITEM);
            AddInstruction(OpCode.ISNULL);
            Jump(OpCode.JMPIF_L, assignmentTarget);
            AddInstruction(OpCode.PICKITEM);
            Jump(OpCode.JMP_L, endTarget);
