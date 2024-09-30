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
            assignmentTarget.Instruction = AddInstruction(OpCode.NOP);
            ConvertExpression(model, right);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.REVERSE4);
            AddInstruction(OpCode.REVERSE3);
            AddInstruction(OpCode.SETITEM);
        }
        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }

    private void ConvertPropertyMemberAccessCoalesceAssignment(SemanticModel model, MemberAccessExpressionSyntax left, ExpressionSyntax right, IPropertySymbol property)
    {
        JumpTarget endTarget = new();
        if (property.IsStatic)
        {
            CallMethodWithConvention(model, property.GetMethod!);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.ISNULL);
            Jump(OpCode.JMPIFNOT_L, endTarget);
            AddInstruction(OpCode.DROP);
            ConvertExpression(model, right);
            AddInstruction(OpCode.DUP);
            CallMethodWithConvention(model, property.SetMethod!);
        }
        else
        {
            JumpTarget assignmentTarget = new();
            ConvertExpression(model, left.Expression);
            AddInstruction(OpCode.DUP);
            CallMethodWithConvention(model, property.GetMethod!);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.ISNULL);
            Jump(OpCode.JMPIF_L, assignmentTarget);
            AddInstruction(OpCode.NIP);
            Jump(OpCode.JMP_L, endTarget);
            assignmentTarget.Instruction = AddInstruction(OpCode.DROP);
            ConvertExpression(model, right);
            AddInstruction(OpCode.TUCK);
            CallMethodWithConvention(model, property.SetMethod!, CallingConvention.StdCall);
        }
        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }
}
