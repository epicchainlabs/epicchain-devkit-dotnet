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
