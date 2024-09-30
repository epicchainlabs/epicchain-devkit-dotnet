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
using System.Linq;
using System.Runtime.InteropServices;

namespace EpicChain.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// Converts the code for simple assignment expression into OpCodes.
    /// The assignment operator = assigns the value of its right-hand operand to a variable,
    /// a property, or an indexer element given by its left-hand operand.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about simple assignment expression.</param>
    /// <param name="expression">The syntax representation of the simple assignment expression statement being converted.</param>
    /// <exception cref="CompilationException">Thrown when the syntax is not supported.</exception>
    /// <remarks>
    /// The result of an assignment expression is the value assigned to the left-hand operand.
    /// The type of the right-hand operand must be the same as the type of the left-hand operand or implicitly convertible to it.
    /// </remarks>
    /// <example>
    /// The assignment operator = is right-associative, that is, an expression of the form
    /// <c>a = b = c</c> is evaluated as <c>a = (b = c)</c>
    /// </example>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/assignment-operator">Assignment operators</seealso>
    private void ConvertSimpleAssignmentExpression(SemanticModel model, AssignmentExpressionSyntax expression)
    {
        ConvertExpression(model, expression.Right);
        AddInstruction(OpCode.DUP);
        switch (expression.Left)
        {
            case DeclarationExpressionSyntax left:
                ConvertDeclarationAssignment(model, left);
                break;
            case ElementAccessExpressionSyntax left:
                ConvertElementAccessAssignment(model, left);
                break;
            case IdentifierNameSyntax left:
                ConvertIdentifierNameAssignment(model, left);
                break;
            case MemberAccessExpressionSyntax left:
                ConvertMemberAccessAssignment(model, left);
                break;
            case TupleExpressionSyntax left:
                ConvertTupleAssignment(model, left);
                break;
            default:
                throw new CompilationException(expression.Left, DiagnosticId.SyntaxNotSupported,
                    $"Unsupported assignment: {expression.Left}");
        }
    }

    private void ConvertDeclarationAssignment(SemanticModel model, DeclarationExpressionSyntax left)
    {
        ITypeSymbol type = model.GetTypeInfo(left).Type!;
        if (!type.IsValueType)
            throw new CompilationException(left, DiagnosticId.SyntaxNotSupported, $"Unsupported assignment type: {type}");
        AddInstruction(OpCode.UNPACK);
        AddInstruction(OpCode.DROP);
        foreach (VariableDesignationSyntax variable in ((ParenthesizedVariableDesignationSyntax)left.Designation).Variables)
        {
            switch (variable)
            {
                case SingleVariableDesignationSyntax singleVariableDesignation:
                    ILocalSymbol local = (ILocalSymbol)model.GetDeclaredSymbol(singleVariableDesignation)!;
                    byte index = AddLocalVariable(local);
                    AccessSlot(OpCode.STLOC, index);
                    break;
                case DiscardDesignationSyntax:
                    AddInstruction(OpCode.DROP);
                    break;
                default:
                    throw new CompilationException(variable, DiagnosticId.SyntaxNotSupported, $"Unsupported designation: {variable}");
            }
        }
    }

    private void ConvertElementAccessAssignment(SemanticModel model, ElementAccessExpressionSyntax left)
    {
        if (left.ArgumentList.Arguments.Count != 1)
            throw new CompilationException(left.ArgumentList, DiagnosticId.MultidimensionalArray, $"Unsupported array rank: {left.ArgumentList.Arguments}");
        if (model.GetSymbolInfo(left).Symbol is IPropertySymbol property)
        {
            ConvertExpression(model, left.ArgumentList.Arguments[0].Expression);
            ConvertExpression(model, left.Expression);
            CallMethodWithConvention(model, property.SetMethod!, CallingConvention.Cdecl);
        }
        else
        {
            ConvertExpression(model, left.Expression);
            ConvertExpression(model, left.ArgumentList.Arguments[0].Expression);
            AddInstruction(OpCode.ROT);
            AddInstruction(OpCode.SETITEM);
        }
    }

    private void ConvertIdentifierNameAssignment(SemanticModel model, IdentifierNameSyntax left)
    {
        ISymbol symbol = model.GetSymbolInfo(left).Symbol!;
        switch (symbol)
        {
            case IDiscardSymbol:
                AddInstruction(OpCode.DROP);
                break;
            case IFieldSymbol field:
                if (field.IsStatic)
                {
                    byte index = _context.AddStaticField(field);
                    AccessSlot(OpCode.STSFLD, index);
                }
                else
                {
                    int index = Array.IndexOf(field.ContainingType.GetFields(), field);
                    AddInstruction(OpCode.LDARG0);
                    Push(index);
                    AddInstruction(OpCode.ROT);
                    AddInstruction(OpCode.SETITEM);
                }
                break;
            case ILocalSymbol local:
                StLocSlot(local);
                break;
            case IParameterSymbol parameter:
                StArgSlot(parameter);
                break;
            case IPropertySymbol property:
                // Check if the property is within a constructor and is readonly
                // C# document here https://learn.microsoft.com/en-us/dotnet/csharp/properties
                // example of this syntax:
                // public class Person
                // {
                //     public Person(string firstName) => FirstName = firstName;
                //     // Readonly property
                //     public string FirstName { get; }
                // }
                if (property.SetMethod == null)
                {
                    IFieldSymbol[] fields = property.ContainingType.GetAllMembers().OfType<IFieldSymbol>().ToArray();
                    fields = fields.Where(p => !p.IsStatic).ToArray();
                    int backingFieldIndex = Array.FindIndex(fields, p => SymbolEqualityComparer.Default.Equals(p.AssociatedSymbol, property));
                    AccessSlot(OpCode.LDARG, 0);
                    Push(backingFieldIndex);
                    AddInstruction(OpCode.ROT);
                    AddInstruction(OpCode.SETITEM);
                }
                else if (property.SetMethod != null)
                {
                    if (!property.IsStatic) AddInstruction(OpCode.LDARG0);
                    CallMethodWithConvention(model, property.SetMethod, CallingConvention.Cdecl);
                }
                else
                {
                    throw new CompilationException(left, DiagnosticId.SyntaxNotSupported, $"Property is readonly and not within a constructor: {property.Name}");
                }
                break;
            default:
                throw new CompilationException(left, DiagnosticId.SyntaxNotSupported, $"Unsupported symbol: {symbol}");
        }
    }

    private void ConvertMemberAccessAssignment(SemanticModel model, MemberAccessExpressionSyntax left)
    {
        ISymbol symbol = model.GetSymbolInfo(left.Name).Symbol!;
        switch (symbol)
        {
            case IFieldSymbol field:
                if (field.IsStatic)
                {
                    byte index = _context.AddStaticField(field);
                    AccessSlot(OpCode.STSFLD, index);
                }
                else
                {
                    int index = Array.IndexOf(field.ContainingType.GetFields(), field);
                    ConvertExpression(model, left.Expression);
                    Push(index);
                    AddInstruction(OpCode.ROT);
                    AddInstruction(OpCode.SETITEM);
                }
                break;
            case IPropertySymbol property:
                if (!property.IsStatic) ConvertExpression(model, left.Expression);
                CallMethodWithConvention(model, property.SetMethod!, CallingConvention.Cdecl);
                break;
            default:
                throw new CompilationException(left, DiagnosticId.SyntaxNotSupported, $"Unsupported symbol: {symbol}");
        }
    }

    private void ConvertTupleAssignment(SemanticModel model, TupleExpressionSyntax left)
    {
        AddInstruction(OpCode.UNPACK);
        AddInstruction(OpCode.DROP);
        foreach (ArgumentSyntax argument in left.Arguments)
        {
            switch (argument.Expression)
            {
                case DeclarationExpressionSyntax declaration:
                    switch (declaration.Designation)
                    {
                        case SingleVariableDesignationSyntax singleVariableDesignation:
                            ILocalSymbol local = (ILocalSymbol)model.GetDeclaredSymbol(singleVariableDesignation)!;
                            byte index = AddLocalVariable(local);
                            AccessSlot(OpCode.STLOC, index);
                            break;
                        case DiscardDesignationSyntax:
                            AddInstruction(OpCode.DROP);
                            break;
                        default:
                            throw new CompilationException(argument, DiagnosticId.SyntaxNotSupported, $"Unsupported designation: {argument}");
                    }
                    break;
                case IdentifierNameSyntax identifier:
                    ConvertIdentifierNameAssignment(model, identifier);
                    break;
                case MemberAccessExpressionSyntax memberAccess:
                    ConvertMemberAccessAssignment(model, memberAccess);
                    break;
                default:
                    throw new CompilationException(argument, DiagnosticId.SyntaxNotSupported, $"Unsupported assignment: {argument}");
            }
        }
    }
}
