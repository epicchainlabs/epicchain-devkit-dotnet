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
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace EpicChain.Compiler;

internal partial class MethodConvert
{
    private void ConvertObjectCreationExpression(SemanticModel model, BaseObjectCreationExpressionSyntax expression)
    {
        ITypeSymbol type = model.GetTypeInfo(expression).Type!;
        if (type.TypeKind == TypeKind.Delegate)
        {
            ConvertDelegateCreationExpression(model, expression);
            return;
        }
        IMethodSymbol constructor = (IMethodSymbol)model.GetSymbolInfo(expression).Symbol!;
        IReadOnlyList<ArgumentSyntax> arguments = expression.ArgumentList?.Arguments ?? (IReadOnlyList<ArgumentSyntax>)Array.Empty<ArgumentSyntax>();
        if (TryProcessSystemConstructors(model, constructor, arguments))
            return;
        bool needCreateObject = !type.DeclaringSyntaxReferences.IsEmpty && !constructor.IsExtern;
        if (needCreateObject)
        {
            CreateObject(model, type, null);
        }
        CallInstanceMethod(model, constructor, needCreateObject, arguments);
        if (expression.Initializer is not null)
        {
            ConvertObjectCreationExpressionInitializer(model, expression.Initializer);
        }
    }

    private void ConvertObjectCreationExpressionInitializer(SemanticModel model, InitializerExpressionSyntax initializer)
    {
        foreach (ExpressionSyntax e in initializer.Expressions)
        {
            if (e is not AssignmentExpressionSyntax ae)
                throw new CompilationException(initializer, DiagnosticId.SyntaxNotSupported, $"Unsupported initializer: {initializer}");
            ISymbol symbol = model.GetSymbolInfo(ae.Left).Symbol!;
            switch (symbol)
            {
                case IFieldSymbol field:
                    AddInstruction(OpCode.DUP);
                    int index = Array.IndexOf(field.ContainingType.GetFields(), field);
                    Push(index);
                    ConvertExpression(model, ae.Right);
                    AddInstruction(OpCode.SETITEM);
                    break;
                case IPropertySymbol property:
                    ConvertExpression(model, ae.Right);
                    AddInstruction(OpCode.OVER);
                    CallMethodWithConvention(model, property.SetMethod!, CallingConvention.Cdecl);
                    break;
                default:
                    throw new CompilationException(ae.Left, DiagnosticId.SyntaxNotSupported, $"Unsupported symbol: {symbol}");
            }
        }
    }

    private void ConvertDelegateCreationExpression(SemanticModel model, BaseObjectCreationExpressionSyntax expression)
    {
        if (expression.ArgumentList!.Arguments.Count != 1)
            throw new CompilationException(expression, DiagnosticId.SyntaxNotSupported, $"Unsupported delegate: {expression}");
        IMethodSymbol symbol = (IMethodSymbol)model.GetSymbolInfo(expression.ArgumentList.Arguments[0].Expression).Symbol!;
        if (!symbol.IsStatic)
            throw new CompilationException(expression, DiagnosticId.NonStaticDelegate, $"Unsupported delegate: {symbol}");
        InvokeMethod(model, symbol);
    }
}
