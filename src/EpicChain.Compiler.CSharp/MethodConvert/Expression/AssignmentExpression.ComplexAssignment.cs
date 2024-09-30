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
    /// Converts the code for complex assignment (or compound assignment) expression into OpCodes.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about complex assignment expression.</param>
    /// <param name="expression">The syntax representation of the complex assignment expression statement being converted.</param>
    /// <exception cref="CompilationException">Thrown when the syntax is not supported.</exception>
    /// <remarks>
    /// For a binary operator op, a compound assignment expression of the form "x op= y" is equivalent to "x = x op y" except that x is only evaluated once.
    /// </remarks>
    /// <example>
    /// The following example demonstrates the usage of compound assignment with arithmetic operators:
    /// The corresponding code branch is "ConvertComplexAssignmentExpression"
    /// <code>
    /// int a = 5;
    /// a += 9;
    /// Runtime.Log(a.ToString());
    /// a -= 4;
    /// Runtime.Log(a.ToString());
    /// a *= 2;
    /// Runtime.Log(a.ToString());
    /// a /= 4;
    /// Runtime.Log(a.ToString());
    /// a %= 3;
    /// Runtime.Log(a.ToString());
    /// </code>
    /// output: 14, 10, 20, 5, 2
    /// </example>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/assignment-operator#compound-assignment">Compound assignment</seealso>
    private void ConvertComplexAssignmentExpression(SemanticModel model, AssignmentExpressionSyntax expression)
    {
        ITypeSymbol type = model.GetTypeInfo(expression).Type!;
        switch (expression.Left)
        {
            case ElementAccessExpressionSyntax left:
                ConvertElementAccessComplexAssignment(model, type, expression.OperatorToken, left, expression.Right);
                break;
            case IdentifierNameSyntax left:
                ConvertIdentifierNameComplexAssignment(model, type, expression.OperatorToken, left, expression.Right);
                break;
            case MemberAccessExpressionSyntax left:
                ConvertMemberAccessComplexAssignment(model, type, expression.OperatorToken, left, expression.Right);
                break;
            default:
                throw new CompilationException(expression.Left, DiagnosticId.SyntaxNotSupported, $"Unsupported assignment expression: {expression}");
        }
    }

    private void ConvertElementAccessComplexAssignment(SemanticModel model, ITypeSymbol type, SyntaxToken operatorToken, ElementAccessExpressionSyntax left, ExpressionSyntax right)
    {
        if (left.ArgumentList.Arguments.Count != 1)
            throw new CompilationException(left.ArgumentList, DiagnosticId.MultidimensionalArray, $"Unsupported array rank: {left.ArgumentList.Arguments}");
        if (model.GetSymbolInfo(left).Symbol is IPropertySymbol property)
        {
            ConvertExpression(model, left.Expression);
            ConvertExpression(model, left.ArgumentList.Arguments[0].Expression);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.OVER);
            CallMethodWithConvention(model, property.GetMethod!, CallingConvention.StdCall);
            ConvertExpression(model, right);
            EmitComplexAssignmentOperator(type, operatorToken);
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
            ConvertExpression(model, right);
            EmitComplexAssignmentOperator(type, operatorToken);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.REVERSE4);
            AddInstruction(OpCode.REVERSE3);
            AddInstruction(OpCode.SETITEM);
        }
    }

    private void ConvertIdentifierNameComplexAssignment(SemanticModel model, ITypeSymbol type, SyntaxToken operatorToken, IdentifierNameSyntax left, ExpressionSyntax right)
    {
        ISymbol symbol = model.GetSymbolInfo(left).Symbol!;
        switch (symbol)
        {
            case IFieldSymbol field:
                ConvertFieldIdentifierNameComplexAssignment(model, type, operatorToken, field, right);
                break;
            case ILocalSymbol local:
                ConvertLocalIdentifierNameComplexAssignment(model, type, operatorToken, local, right);
                break;
            case IParameterSymbol parameter:
                ConvertParameterIdentifierNameComplexAssignment(model, type, operatorToken, parameter, right);
                break;
            case IPropertySymbol property:
                ConvertPropertyIdentifierNameComplexAssignment(model, type, operatorToken, property, right);
                break;
            default:
                throw new CompilationException(left, DiagnosticId.SyntaxNotSupported, $"Unsupported symbol: {symbol}");
        }
    }

    private void ConvertMemberAccessComplexAssignment(SemanticModel model, ITypeSymbol type, SyntaxToken operatorToken, MemberAccessExpressionSyntax left, ExpressionSyntax right)
    {
        ISymbol symbol = model.GetSymbolInfo(left).Symbol!;
        switch (symbol)
        {
            case IFieldSymbol field:
                ConvertFieldMemberAccessComplexAssignment(model, type, operatorToken, left, right, field);
                break;
            case IPropertySymbol property:
                ConvertPropertyMemberAccessComplexAssignment(model, type, operatorToken, left, right, property);
                break;
            default:
                throw new CompilationException(left, DiagnosticId.SyntaxNotSupported, $"Unsupported symbol: {symbol}");
        }
    }

    private void ConvertFieldIdentifierNameComplexAssignment(SemanticModel model, ITypeSymbol type, SyntaxToken operatorToken, IFieldSymbol left, ExpressionSyntax right)
    {
        if (left.IsStatic)
        {
            byte index = _context.AddStaticField(left);
            AccessSlot(OpCode.LDSFLD, index);
            ConvertExpression(model, right);
            EmitComplexAssignmentOperator(type, operatorToken);
            AddInstruction(OpCode.DUP);
            AccessSlot(OpCode.STSFLD, index);
        }
        else
        {
            int index = Array.IndexOf(left.ContainingType.GetFields(), left);
            AddInstruction(OpCode.LDARG0);
            AddInstruction(OpCode.DUP);
            Push(index);
            AddInstruction(OpCode.PICKITEM);
            ConvertExpression(model, right);
            EmitComplexAssignmentOperator(type, operatorToken);
            AddInstruction(OpCode.TUCK);
            Push(index);
            AddInstruction(OpCode.SWAP);
            AddInstruction(OpCode.SETITEM);
        }
    }

    private void ConvertLocalIdentifierNameComplexAssignment(SemanticModel model, ITypeSymbol type, SyntaxToken operatorToken, ILocalSymbol left, ExpressionSyntax right)
    {
        LdLocSlot(left);
        ConvertExpression(model, right);
        EmitComplexAssignmentOperator(type, operatorToken);
        AddInstruction(OpCode.DUP);
        StLocSlot(left);
    }

    private void ConvertParameterIdentifierNameComplexAssignment(SemanticModel model, ITypeSymbol type, SyntaxToken operatorToken, IParameterSymbol left, ExpressionSyntax right)
    {
        LdArgSlot(left);
        ConvertExpression(model, right);
        EmitComplexAssignmentOperator(type, operatorToken);
        AddInstruction(OpCode.DUP);
        StArgSlot(left);
    }

    private void ConvertPropertyIdentifierNameComplexAssignment(SemanticModel model, ITypeSymbol type, SyntaxToken operatorToken, IPropertySymbol left, ExpressionSyntax right)
    {
        if (left.IsStatic)
        {
            CallMethodWithConvention(model, left.GetMethod!);
            ConvertExpression(model, right);
            EmitComplexAssignmentOperator(type, operatorToken);
            AddInstruction(OpCode.DUP);
            CallMethodWithConvention(model, left.SetMethod!);
        }
        else
        {
            AddInstruction(OpCode.LDARG0);
            AddInstruction(OpCode.DUP);
            CallMethodWithConvention(model, left.GetMethod!);
            ConvertExpression(model, right);
            EmitComplexAssignmentOperator(type, operatorToken);
            AddInstruction(OpCode.TUCK);
            CallMethodWithConvention(model, left.SetMethod!, CallingConvention.StdCall);
        }
    }

    private void ConvertFieldMemberAccessComplexAssignment(SemanticModel model, ITypeSymbol type, SyntaxToken operatorToken, MemberAccessExpressionSyntax left, ExpressionSyntax right, IFieldSymbol field)
    {
        if (field.IsStatic)
        {
            byte index = _context.AddStaticField(field);
            AccessSlot(OpCode.LDSFLD, index);
            ConvertExpression(model, right);
            EmitComplexAssignmentOperator(type, operatorToken);
            AddInstruction(OpCode.DUP);
            AccessSlot(OpCode.STSFLD, index);
        }
        else
        {
            int index = Array.IndexOf(field.ContainingType.GetFields(), field);
            ConvertExpression(model, left.Expression);
            AddInstruction(OpCode.DUP);
            Push(index);
            AddInstruction(OpCode.PICKITEM);
            ConvertExpression(model, right);
            EmitComplexAssignmentOperator(type, operatorToken);
            AddInstruction(OpCode.TUCK);
            Push(index);
            AddInstruction(OpCode.SWAP);
            AddInstruction(OpCode.SETITEM);
        }
    }

    private void ConvertPropertyMemberAccessComplexAssignment(SemanticModel model, ITypeSymbol type, SyntaxToken operatorToken, MemberAccessExpressionSyntax left, ExpressionSyntax right, IPropertySymbol property)
    {
        if (property.IsStatic)
        {
            CallMethodWithConvention(model, property.GetMethod!);
            ConvertExpression(model, right);
            EmitComplexAssignmentOperator(type, operatorToken);
            AddInstruction(OpCode.DUP);
            CallMethodWithConvention(model, property.SetMethod!);
        }
        else
        {
            ConvertExpression(model, left.Expression);
            AddInstruction(OpCode.DUP);
            CallMethodWithConvention(model, property.GetMethod!);
            ConvertExpression(model, right);
            EmitComplexAssignmentOperator(type, operatorToken);
            AddInstruction(OpCode.TUCK);
            CallMethodWithConvention(model, property.SetMethod!, CallingConvention.StdCall);
        }
    }

    private void EmitComplexAssignmentOperator(ITypeSymbol type, SyntaxToken operatorToken)
    {
        var itemType = type.GetStackItemType();
        bool isBoolean = itemType == VM.Types.StackItemType.Boolean;
        bool isString = itemType == VM.Types.StackItemType.ByteString;

        var (opcode, checkResult) = operatorToken.ValueText switch
        {
            "+=" => isString ? (OpCode.CAT, false) : (OpCode.ADD, true),
            "-=" => (OpCode.SUB, true),
            "*=" => (OpCode.MUL, true),
            "/=" => (OpCode.DIV, true),
            "%=" => (OpCode.MOD, true),
            "&=" => isBoolean ? (OpCode.BOOLAND, false) : (OpCode.AND, true),
            "^=" when !isBoolean => (OpCode.XOR, true),
            "|=" => isBoolean ? (OpCode.BOOLOR, false) : (OpCode.OR, true),
            "<<=" => (OpCode.SHL, true),
            ">>=" => (OpCode.SHR, true),
            _ => throw new CompilationException(operatorToken, DiagnosticId.SyntaxNotSupported, $"Unsupported operator: {operatorToken}")
        };
        AddInstruction(opcode);
        if (isString) ChangeType(VM.Types.StackItemType.ByteString);
        if (checkResult) EnsureIntegerInRange(type);
    }
}
