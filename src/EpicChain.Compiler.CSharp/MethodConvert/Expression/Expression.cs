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
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using EpicChain.Cryptography.ECC;
using EpicChain.IO;
using EpicChain.SmartContract.Native;
using EpicChain.VM;
using EpicChain.VM.Types;
using EpicChain.Wallets;
using System.Linq;
using System.Numerics;

namespace EpicChain.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// Converts an expression to EpicChainVM instructions.
    /// </summary>
    /// <param name="model">The semantic model of the compilation.</param>
    /// <param name="syntax">The expression syntax to convert.</param>
    /// <param name="syntaxNode">Optional parent syntax node for context.</param>
    private void ConvertExpression(SemanticModel model, ExpressionSyntax syntax, SyntaxNode? syntaxNode = null)
    {
        // Insert a sequence point for debugging purposes
        using var sequence = InsertSequencePoint(syntax);

        // Try to convert the expression as a constant first
        if (TryConvertConstant(model, syntax, syntaxNode))
            return;

        // If it's not a constant, convert it as a non-constant expression
        ConvertNonConstantExpression(model, syntax);
    }

    private bool TryConvertConstant(SemanticModel model, ExpressionSyntax syntax, SyntaxNode? syntaxNode)
    {
        Optional<object?> constant = model.GetConstantValue(syntax);
        if (!constant.HasValue)
            return false;

        var value = constant.Value;
        ITypeSymbol? typeSymbol = GetTypeSymbol(syntaxNode, model);

        if (typeSymbol != null)
        {
            value = ConvertComplexConstantTypes(typeSymbol, value, syntax);
        }

        Push(value);
        return true;
    }

    private void ConvertNonConstantExpression(SemanticModel model, ExpressionSyntax syntax)
    {
        switch (syntax)
        {
            case AnonymousObjectCreationExpressionSyntax expression:
                // Example: new { Name = "John", Age = 30 }
                ConvertAnonymousObjectCreationExpression(model, expression);
                break;
            case ArrayCreationExpressionSyntax expression:
                // Example: new int[] { 1, 2, 3 }
                ConvertArrayCreationExpression(model, expression);
                break;
            case AssignmentExpressionSyntax expression:
                // Example: x = 5
                ConvertAssignmentExpression(model, expression);
                break;
            case BaseObjectCreationExpressionSyntax expression:
                // Example: new MyClass()
                ConvertObjectCreationExpression(model, expression);
                break;
            case BinaryExpressionSyntax expression:
                // Example: a + b
                ConvertBinaryExpression(model, expression);
                break;
            case CastExpressionSyntax expression:
                // Example: (int)myDouble
                ConvertCastExpression(model, expression);
                break;
            case CheckedExpressionSyntax expression:
                // Example: checked(x + y)
                ConvertCheckedExpression(model, expression);
                break;
            case ConditionalAccessExpressionSyntax expression:
                // Example: person?.Name
                ConvertConditionalAccessExpression(model, expression);
                break;
            case ConditionalExpressionSyntax expression:
                // Example: isTrue ? "Yes" : "No"
                ConvertConditionalExpression(model, expression);
                break;
            case ElementAccessExpressionSyntax expression:
                // Example: myArray[0]
                ConvertElementAccessExpression(model, expression);
                break;
            case ElementBindingExpressionSyntax expression:
                // Example: obj?[0]
                ConvertElementBindingExpression(model, expression);
                break;
            case IdentifierNameSyntax expression:
                // Example: myVariable
                ConvertIdentifierNameExpression(model, expression);
                break;
            case ImplicitArrayCreationExpressionSyntax expression:
                // Example: new[] { 1, 2, 3 }
                ConvertImplicitArrayCreationExpression(model, expression);
                break;
            case InitializerExpressionSyntax expression:
                // Example: { 1, 2, 3 }
                ConvertInitializerExpression(model, expression);
                break;
            case InterpolatedStringExpressionSyntax expression:
                // Example: $"Hello, {name}!"
                ConvertInterpolatedStringExpression(model, expression);
                break;
            case InvocationExpressionSyntax expression:
                // Example: MyMethod()
                ConvertInvocationExpression(model, expression);
                break;
            case IsPatternExpressionSyntax expression:
                // Example: obj is string s
                ConvertIsPatternExpression(model, expression);
                break;
            case MemberAccessExpressionSyntax expression:
                // Example: myObject.Property
                ConvertMemberAccessExpression(model, expression);
                break;
            case MemberBindingExpressionSyntax expression:
                // Example: ?.Property
                ConvertMemberBindingExpression(model, expression);
                break;
            case ParenthesizedExpressionSyntax expression:
                // Example: (x + y)
                ConvertExpression(model, expression.Expression);
                break;
            case PostfixUnaryExpressionSyntax expression:
                // Example: x++
                ConvertPostfixUnaryExpression(model, expression);
                break;
            case PrefixUnaryExpressionSyntax expression:
                // Example: ++x
                ConvertPrefixUnaryExpression(model, expression);
                break;
            case SwitchExpressionSyntax expression:
                // Example: x switch { 1 => "One", 2 => "Two", _ => "Other" }
                ConvertSwitchExpression(model, expression);
                break;
            case BaseExpressionSyntax:
            case ThisExpressionSyntax:
                // Example: base.Method() or this.Property
                AddInstruction(OpCode.LDARG0);
                break;
            case ThrowExpressionSyntax expression:
                // Example: throw new Exception("Error")
                Throw(model, expression.Expression);
                break;
            case TupleExpressionSyntax expression:
                // Example: (1, "Hello")
                ConvertTupleExpression(model, expression);
                break;
            case ParenthesizedLambdaExpressionSyntax expression:
                // Example: (x, y) => x + y
                ConvertParenthesizedLambdaExpression(model, expression);
                break;
            case SimpleLambdaExpressionSyntax expression:
                // Example: x => x * x
                ConvertSimpleLambdaExpression(model, expression);
                break;
            case CollectionExpressionSyntax expression:
                // Example: [1, 2, 3]
                ConvertCollectionExpression(model, expression);
                break;
            case WithExpressionSyntax expression:
                // Example: person with { Name = "John" }
                ConvertWithExpressionSyntax(model, expression);
                break;
            case LiteralExpressionSyntax expression:
                // Example: 42 or "Hello"
                ConvertLiteralExpression(model, expression);
                break;
            case TypeOfExpressionSyntax expression:
                // Example: typeof(int)
                // Note: EpicChain currently does not support the Type type of C#. The typeof operator here
                // will only return the string name of the class/type. This support is added
                // to ensure we can process enum parse methods.
                ConvertTypeOfExpression(model, expression);
                break;
            default:
                throw new CompilationException(syntax, DiagnosticId.SyntaxNotSupported, $"Unsupported syntax: {syntax}");
        }
    }

    private void ConvertTypeOfExpression(SemanticModel model, TypeOfExpressionSyntax expression)
    {
        var typeInfo = model.GetTypeInfo(expression.Type);
        if (typeInfo.Type == null)
            throw new CompilationException(expression, DiagnosticId.InvalidType, $"Invalid type in typeof expression: {expression.Type}");

        Push(typeInfo.Type.Name);
    }

    private static ITypeSymbol? GetTypeSymbol(SyntaxNode? syntaxNode, SemanticModel model)
    {
        return syntaxNode switch
        {
            VariableDeclaratorSyntax { Parent: VariableDeclarationSyntax declaration }
                => ModelExtensions.GetTypeInfo(model, declaration.Type).Type,
            PropertyDeclarationSyntax propertyDeclaration
                => ModelExtensions.GetTypeInfo(model, propertyDeclaration.Type).Type,
            _ => null
        };
    }

    private object ConvertComplexConstantTypes(ITypeSymbol typeSymbol, object value, ExpressionSyntax syntax)
    {
        string fullName = typeSymbol.ToDisplayString();
        return fullName switch
        {
            "EpicChain.SmartContract.Framework.UInt160" => ConvertToUInt160((string)value!),
            "EpicChain.SmartContract.Framework.UInt256" => ConvertToUInt256((string)value!, syntax),
            "EpicChain.SmartContract.Framework.ECPoint" => ConvertToECPoint((string)value!),
            "EpicChain.SmartContract.Framework.ByteArray" => ((string)value!).HexToBytes(true),
            _ => value
        };
    }

    private byte[] ConvertToUInt160(string strValue)
    {
        return (UInt160.TryParse(strValue, out var hash)
            ? hash
            : strValue.ToScriptHash(_context.Options.AddressVersion)).ToArray();
    }

    private static byte[] ConvertToUInt256(string strValue, ExpressionSyntax syntax)
    {
        var value = strValue.HexToBytes(true);
        if (value.Length != 32)
            throw new CompilationException(syntax, DiagnosticId.InvalidInitialValue, "Invalid UInt256 literal");
        return value;
    }

    private static byte[] ConvertToECPoint(string strValue)
    {
        return ECPoint.Parse(strValue, ECCurve.Secp256r1).EncodePoint(true);
    }


    /// <summary>
    /// Ensures that the value of the incoming integer type is within the specified range.
    /// If the type is BigInteger, no range check is performed.
    /// </summary>
    /// <param name="type">The integer type to be checked.</param>
    private void EnsureIntegerInRange(ITypeSymbol type)
    {
        if (type.Name == "BigInteger") return;
        var (minValue, maxValue, mask) = type.Name switch
        {
            "SByte" => ((BigInteger)sbyte.MinValue, (BigInteger)sbyte.MaxValue, (BigInteger)0xff),
            "Int16" => (short.MinValue, short.MaxValue, 0xffff),
            "Char" => (ushort.MinValue, ushort.MaxValue, 0xffff),
            "Int32" => (int.MinValue, int.MaxValue, 0xffffffff),
            "Int64" => (long.MinValue, long.MaxValue, 0xffffffffffffffff),
            "Byte" => (byte.MinValue, byte.MaxValue, 0xff),
            "UInt16" => (ushort.MinValue, ushort.MaxValue, 0xffff),
            "UInt32" => (uint.MinValue, uint.MaxValue, 0xffffffff),
            "UInt64" => (ulong.MinValue, ulong.MaxValue, 0xffffffffffffffff),
            _ => throw new CompilationException(DiagnosticId.SyntaxNotSupported, $"Unsupported type: {type}")
        };
        JumpTarget checkUpperBoundTarget = new(), adjustTarget = new(), endTarget = new();
        AddInstruction(OpCode.DUP);
        Push(minValue);
        Jump(OpCode.JMPGE_L, checkUpperBoundTarget);
        if (_checkedStack.Peek())
            AddInstruction(OpCode.THROW);
        else
            Jump(OpCode.JMP_L, adjustTarget);
        checkUpperBoundTarget.Instruction = AddInstruction(OpCode.DUP);
        Push(maxValue);
        Jump(OpCode.JMPLE_L, endTarget);
        if (_checkedStack.Peek())
        {
            AddInstruction(OpCode.THROW);
        }
        else
        {
            adjustTarget.Instruction = Push(mask);
            AddInstruction(OpCode.AND);
            if (minValue < 0)
            {
                AddInstruction(OpCode.DUP);
                Push(maxValue);
                Jump(OpCode.JMPLE_L, endTarget);
                Push(mask + 1);
                AddInstruction(OpCode.SUB);
            }
        }
        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }

    /// <summary>
    /// Converts an object to a string. Different conversion methods are used based on the type of the object.
    /// </summary>
    /// <param name="model">The semantic model used to obtain type information of the expression.</param>
    /// <param name="expression">The expression to be converted to a string.</param>
    /// <remarks>
    /// For integer types and BigInteger type, call the itoa method of NativeContract.EssentialLib.Hash for conversion.
    /// For string type and specific types in EpicChain.SmartContract.Framework, directly perform expression conversion.
    /// </remarks>
    /// <exception cref="CompilationException">For unsupported types, throw a compilation exception.</exception>
    private void ConvertObjectToString(SemanticModel model, ExpressionSyntax expression)
    {
        ITypeSymbol? type = ModelExtensions.GetTypeInfo(model, expression).Type;
        switch (type?.ToString())
        {
            case "sbyte":
            case "byte":
            case "short":
            case "ushort":
            case "int":
            case "uint":
            case "long":
            case "ulong":
            case "System.Numerics.BigInteger":
                ConvertExpression(model, expression);
                CallContractMethod(NativeContract.EssentialLib.Hash, "itoa", 1, true);
                break;
            case "char":
                ConvertExpression(model, expression);
                ChangeType(StackItemType.ByteString);
                break;
            case "string":
            case "EpicChain.SmartContract.Framework.ECPoint":
            case "EpicChain.SmartContract.Framework.ByteString":
            case "EpicChain.SmartContract.Framework.UInt160":
            case "EpicChain.SmartContract.Framework.UInt256":
                ConvertExpression(model, expression);
                break;
            case "bool":
                {
                    ConvertExpression(model, expression);
                    JumpTarget falseTarget = new();
                    Jump(OpCode.JMPIFNOT_L, falseTarget);
                    Push("True");
                    JumpTarget endTarget = new();
                    Jump(OpCode.JMP_L, endTarget);
                    falseTarget.Instruction = Push("False");
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    break;
                }
            case "byte[]":
                {
                    Push("System.Byte[]");
                    break;
                }
            default:
                throw new CompilationException(expression, DiagnosticId.InvalidToStringType, $"Unsupported interpolation: {expression}");
        }
    }
}
