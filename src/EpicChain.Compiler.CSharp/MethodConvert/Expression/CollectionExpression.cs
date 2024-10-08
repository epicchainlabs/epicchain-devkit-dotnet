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


extern alias scfx;
using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using EpicChain.VM;

namespace EpicChain.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// Converts a collection expression to the appropriate EpicChainVM instructions.
    /// Determines if the collection is a byte array or a generic collection and calls the appropriate conversion method.
    /// </summary>
    /// <param name="model">The semantic model of the code being analyzed.</param>
    /// <param name="expression">The collection expression syntax to convert.</param>
    private void ConvertCollectionExpression(SemanticModel model, CollectionExpressionSyntax expression)
    {
        var typeSymbol = model.GetTypeInfo(expression).ConvertedType;

        // Byte array is considered Buffer type in EpicChainVM
        if (IsArrayOfBytes(typeSymbol))
        {
            ConvertByteArrayExpression(model, expression);
        }
        else
        {
            ConvertGenericCollectionExpression(model, expression);
        }
    }

    /// <summary>
    /// Determines if the given type symbol represents an array of bytes.
    /// </summary>
    /// <param name="typeSymbol">The type symbol to check.</param>
    /// <returns>True if the type is an array of bytes, false otherwise.</returns>
    private static bool IsArrayOfBytes(ITypeSymbol? typeSymbol)
    {
        return typeSymbol is IArrayTypeSymbol { ElementType.SpecialType: SpecialType.System_Byte };
    }

    /// <summary>
    /// Converts a byte array expression to EpicChainVM instructions.
    /// Determines if the array contains constant values or not and calls the appropriate conversion method.
    /// </summary>
    /// <param name="model">The semantic model of the code being analyzed.</param>
    /// <param name="expression">The collection expression syntax representing a byte array.</param>
    private void ConvertByteArrayExpression(SemanticModel model, CollectionExpressionSyntax expression)
    {
        var values = GetConstantValues(model, expression);

        if (values.Any(p => !p.HasValue))
        {
            ConvertNonConstantByteArray(model, expression, values.Length);
        }
        else
        {
            ConvertConstantByteArray(values);
        }
    }

    /// <summary>
    /// Extracts constant values from a collection expression, if possible.
    /// </summary>
    /// <param name="model">The semantic model of the code being analyzed.</param>
    /// <param name="expression">The collection expression syntax to extract values from.</param>
    /// <returns>An array of optional objects representing the constant values, or default if not constant.</returns>
    private static Optional<object?>[] GetConstantValues(SemanticModel model, CollectionExpressionSyntax expression)
    {
        return expression.Elements
            .Select(p => p is ExpressionElementSyntax exprElement ? model.GetConstantValue(exprElement.Expression) : default)
            .ToArray();
    }

    /// <summary>
    /// Converts a non-constant byte array to EpicChainVM instructions.
    /// Creates a new buffer and sets each element individually.
    /// </summary>
    /// <param name="model">The semantic model of the code being analyzed.</param>
    /// <param name="expression">The collection expression syntax representing a non-constant byte array.</param>
    /// <param name="length">The length of the array.</param>
    private void ConvertNonConstantByteArray(SemanticModel model, CollectionExpressionSyntax expression, int length)
    {
        Push(length);
        AddInstruction(OpCode.NEWBUFFER);
        for (var i = 0; i < expression.Elements.Count; i++)
        {
            AddInstruction(OpCode.DUP);
            Push(i);
            ConvertElement(model, expression.Elements[i]);
            AddInstruction(OpCode.SETITEM);
        }
    }

    /// <summary>
    /// Converts a single element of a collection expression to EpicChainVM instructions.
    /// </summary>
    /// <param name="model">The semantic model of the code being analyzed.</param>
    /// <param name="element">The collection element syntax to convert.</param>
    private void ConvertElement(SemanticModel model, CollectionElementSyntax element)
    {
        if (element is ExpressionElementSyntax exprElement)
        {
            ConvertExpression(model, exprElement.Expression);
        }
        else
        {
            throw new NotSupportedException($"Unsupported collection element type: {element.GetType()}");
        }
    }

    /// <summary>
    /// Converts a constant byte array to EpicChainVM instructions.
    /// Creates a buffer directly from the constant values.
    /// </summary>
    /// <param name="values">An array of optional objects representing the constant byte values.</param>
    private void ConvertConstantByteArray(Optional<object?>[] values)
    {
        var data = values.Select(p => (byte)System.Convert.ChangeType(p.Value, typeof(byte))!).ToArray();
        Push(data);
        ChangeType(VM.Types.StackItemType.Buffer);
    }

    /// <summary>
    /// Converts a generic collection expression to EpicChainVM instructions.
    /// Creates an array by pushing elements onto the stack and then packing them.
    /// </summary>
    /// <param name="model">The semantic model of the code being analyzed.</param>
    /// <param name="expression">The collection expression syntax to convert.</param>
    private void ConvertGenericCollectionExpression(SemanticModel model, CollectionExpressionSyntax expression)
    {
        for (var i = expression.Elements.Count - 1; i >= 0; i--)
        {
            ConvertElement(model, expression.Elements[i]);
        }
        Push(expression.Elements.Count);
        AddInstruction(OpCode.PACK);
    }
}
