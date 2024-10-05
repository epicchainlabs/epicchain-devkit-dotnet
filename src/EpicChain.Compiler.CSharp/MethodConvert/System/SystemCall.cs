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
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq.Expressions;
using EpicChain.VM;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using Array = System.Array;
using Akka.Util.Internal;

namespace EpicChain.Compiler;

internal partial class MethodConvert
{
    private delegate void SystemCallHandler(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments);

    private static readonly Dictionary<string, SystemCallHandler> SystemCallHandlers = new();

    static MethodConvert()
    {
        RegisterSystemCallHandlers();
    }

    private static void RegisterHandler<TResult>(Expression<Func<TResult>> expression, SystemCallHandler handler)
    {
        var key = GetKeyFromExpression(expression);
        SystemCallHandlers[key] = handler;
    }

    private static void RegisterHandler<T, TResult>(Expression<Func<T, TResult>> expression, SystemCallHandler handler, string? key = null)
    {
        key = key ?? GetKeyFromExpression(expression, typeof(T));
        SystemCallHandlers[key] = handler;
    }

    private static void RegisterHandler<T1, T2, TResult>(Expression<Func<T1, T2, TResult>> expression, SystemCallHandler handler, string? key = null)
    {
        key = key ?? GetKeyFromExpression(expression, typeof(T1), typeof(T2));
        SystemCallHandlers[key] = handler;
    }

    private static void RegisterHandler<T1, T2, T3, TResult>(Expression<Func<T1, T2, T3, TResult>> expression, SystemCallHandler handler)
    {
        var key = GetKeyFromExpression(expression, typeof(T1), typeof(T2), typeof(T3));
        SystemCallHandlers[key] = handler;
    }
    private static void RegisterHandler<T1, T2, T3, T4>(Expression<Func<T1, T2, T3, T4, bool>> expression, SystemCallHandler handler)
    {
        var key = GetKeyFromExpression(expression, typeof(T1), typeof(T2), typeof(T3), typeof(T4));
        SystemCallHandlers[key] = handler;
    }

    private static void RegisterHandler<T1, T2, T3, T4, T5, TResult>(Expression<Func<T1, T2, T3, T4, T5, TResult>> expression, SystemCallHandler handler)
    {
        var key = GetKeyFromExpression(expression, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
        SystemCallHandlers[key] = handler;
    }

    private static void RegisterHandler<T>(Expression<Action<T>> expression, SystemCallHandler handler)
    {
        var key = GetKeyFromExpression(expression, typeof(T));
        SystemCallHandlers[key] = handler;
    }

    private static void RegisterHandler<T1, T2>(Expression<Action<T1, T2>> expression, SystemCallHandler handler)
    {
        var key = GetKeyFromExpression(expression, typeof(T1), typeof(T2));
        SystemCallHandlers[key] = handler;
    }

    private static void RegisterHandler<T1, T2, T3>(Expression<Action<T1, T2, T3>> expression, SystemCallHandler handler)
    {
        var key = GetKeyFromExpression(expression, typeof(T1), typeof(T2), typeof(T3));
        SystemCallHandlers[key] = handler;
    }

    private static string GetKeyFromExpression(LambdaExpression expression, params Type[] argumentTypes)
    {
        return expression.Body switch
        {
            MethodCallExpression methodCall => GetMethodCallKey(methodCall, argumentTypes),
            MemberExpression { Member: PropertyInfo property } => $"{GetShortTypeName(property.DeclaringType)}.{property.Name}.get",
            MemberExpression { Member: FieldInfo field } => $"{GetShortTypeName(field.DeclaringType)}.{field.Name}",
            UnaryExpression { NodeType: ExpressionType.Convert } unaryExpression => GetUnaryExpressionKey(unaryExpression),
            IndexExpression indexExpression => GetIndexExpressionKey(indexExpression),
            _ => throw new ArgumentException("Expression must be a method call, property, field access, or special member.", nameof(expression)),
        };
    }

    private static string GetMethodCallKey(MethodCallExpression methodCall, Type[] argumentTypes)
    {
        var method = methodCall.Method;
        // Static method
        if (methodCall.Object == null) return GetMethodKey(method, argumentTypes);

        var methodName = method.Name;
        var paramNames = argumentTypes.Select(GetShortTypeName).ToArray();
        var parameters = paramNames.Length > 0 ? string.Join(", ", paramNames[1..]) : null;

        if (method.IsSpecialName && (methodName.StartsWith("get_Char") || methodName.StartsWith("set_Char")))
        {
            var accessorType = methodName.StartsWith("get_Char") ? "get" : "set";
            return $"{GetShortTypeName(method.DeclaringType)}.this[{parameters}].{accessorType}";
        }

        if (method.IsGenericMethod)
        {
            var containingType = GetShortTypeName(method.DeclaringType);
            var genericArguments = $"<{string.Join(", ", method.GetGenericArguments().Select(GetShortTypeName))}>";
            return $"{containingType}.{methodName}{genericArguments}({parameters})";
        }

        return $"{paramNames[0]}.{methodName}({parameters})";
    }

    private static string GetUnaryExpressionKey(UnaryExpression unaryExpression)
    {
        var operandType = GetShortTypeName(unaryExpression.Operand.Type);
        var targetType = GetShortTypeName(unaryExpression.Type);
        return unaryExpression.Method.Name == "op_Implicit"
            ? $"{targetType}.implicit operator {targetType}({operandType})"
            : $"{operandType}.explicit operator {targetType}({operandType})";
    }

    private static string GetIndexExpressionKey(IndexExpression indexExpression)
    {
        var indexParams = string.Join(", ", indexExpression.Arguments.Select(arg => GetShortTypeName(arg.Type)));
        return $"{GetShortTypeName(indexExpression.Object.Type)}.this[{indexParams}].get";
    }

    private static string GetMethodKey(MethodInfo method, Type[] argumentTypes)
    {
        var containingType = GetShortTypeName(method.DeclaringType);
        var parameters = string.Join(", ", argumentTypes.Select(GetShortTypeName));

        switch (method.IsSpecialName)
        {
            case true when method.Name.StartsWith("get_Char") || method.Name.StartsWith("set_Char"):
                {
                    var accessorType = method.Name.StartsWith("get_Char") ? "get" : "set";
                    return $"{GetShortTypeName(method.DeclaringType)}.this[{parameters}].{accessorType}";
                }
            case true when method.Name.StartsWith("op_"):
                {
                    var operatorName = GetOperatorName(method.Name);
                    if (operatorName is "implicit operator" or "explicit operator")
                    {
                        var returnType = GetShortTypeName(method.ReturnType);
                        return $"{containingType}.{operatorName} {returnType}({parameters})";
                    }
                    return $"{containingType}.{operatorName}({parameters})";
                }
            default:
                {
                    var genericArguments = method.IsGenericMethod
                        ? $"<{string.Join(", ", method.GetGenericArguments().Select(GetShortTypeName))}>"
                        : "";

                    return $"{containingType}.{method.Name}{genericArguments}({parameters})";
                }
        }
    }

    private static string GetShortTypeName(Type type)
    {
        if (type.IsArray)
        {
            return GetShortTypeName(type.GetElementType()) + "[]";
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return GetShortTypeName(type.GetGenericArguments()[0]) + "?";
        }

        return type switch
        {
            _ when type == typeof(int) => "int",
            _ when type == typeof(long) => "long",
            _ when type == typeof(short) => "short",
            _ when type == typeof(byte) => "byte",
            _ when type == typeof(bool) => "bool",
            _ when type == typeof(string) => "string",
            _ when type == typeof(char) => "char",
            _ when type == typeof(void) => "void",
            _ when type == typeof(object) => "object",
            _ when type == typeof(sbyte) => "sbyte",
            _ when type == typeof(uint) => "uint",
            _ when type == typeof(ulong) => "ulong",
            _ when type == typeof(ushort) => "ushort",
            // _ when type == typeof(byte[]) => "byte[]",
            _ when type == typeof(BigInteger) => "System.Numerics.BigInteger",
            _ when type == typeof(Array) => "System.Array",
            _ when type == typeof(Math) => "System.Math",
            _ when type == typeof(Type) => "System.Type",
            _ when type == typeof(Enum) => "System.Enum",
            _ when type.IsGenericType => $"{type.Name.Split('`')[0]}<{string.Join(", ", type.GetGenericArguments().Select(GetShortTypeName))}>",
            _ => type.Name,
        };
    }

    private static string GetOperatorName(string methodName)
    {
        return methodName switch
        {
            "op_Implicit" => "implicit operator",
            "op_Explicit" => "explicit operator",
            _ => methodName.StartsWith("op_") ? methodName[3..] : methodName
        };
    }

    /// <summary>
    /// Attempts to process system constructors. Performs different processing operations based on the method symbol.
    /// </summary>
    /// <param name="model">The semantic model used to obtain detailed information about the symbol.</param>
    /// <param name="symbol">The method symbol to be processed.</param>
    /// <param name="arguments">A list of syntax nodes representing the arguments of the method.</param>
    /// <returns>True if system constructors are successfully processed; otherwise, false.</returns>
    private bool TryProcessSystemConstructors(SemanticModel model, IMethodSymbol symbol, IReadOnlyList<ArgumentSyntax> arguments)
    {
        switch (symbol.ToString())
        {
            //For the BigInteger(byte[]) constructor, prepares method arguments and changes the return type to integer.
            case "System.Numerics.BigInteger.BigInteger(byte[])":
                PrepareArgumentsForMethod(model, symbol, arguments);
                ChangeType(VM.Types.StackItemType.Integer);
                return true;
            //For other constructors, such as List<T>(), return processing failure.
            default:
                return false;
        }
    }

    /// <summary>
    /// Attempts to process system methods. Performs different processing operations based on the method symbol.
    /// </summary>
    /// <param name="model">The semantic model used to obtain detailed information about the symbol.</param>
    /// <param name="symbol">The method symbol to be processed.</param>
    /// <param name="instanceExpression">The instance expression representing the instance of method invocation, if any.</param>
    /// <param name="arguments">A list of syntax nodes representing the arguments of the method.</param>
    /// <returns>True if system methods are successfully processed; otherwise, false.</returns>
    private bool TryProcessSystemMethods(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        //If the method belongs to a delegate and the method name is "Invoke",
        //calls the PrepareArgumentsForMethod method with CallingConvention.Cdecl convention and changes the return type to integer.
        //Example: Func<int, int, int>(privateSum).Invoke(a, b);
        //see ~/tests/EpicChain.Compiler.CSharp.TestContracts/Contract_Delegate.cs
        if (symbol.ContainingType.TypeKind == TypeKind.Delegate && symbol.Name == "Invoke")
        {
            if (arguments is not null)
                PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.Cdecl);
            ConvertExpression(model, instanceExpression!);
            AddInstruction(OpCode.CALLA);
            return true;
        }

        var key = symbol.ToString()!.Replace("out ", "");
        key = (from parameter in symbol.Parameters let parameterType = parameter.Type.ToString() where !parameter.Type.IsValueType && parameterType!.EndsWith('?') select parameterType).Aggregate(key, (current, parameterType) => current.Replace(parameterType, parameterType[..^1]));
        if (key == "string.ToString()") key = "object.ToString()";
        if (key.StartsWith("System.Enum.GetName<")) key = "System.Enum.GetName<>()";
        if (key.StartsWith("System.Enum.GetName(")) key = "System.Enum.GetName()";
        if (!SystemCallHandlers.TryGetValue(key, out var handler)) return false;
        handler(this, model, symbol, instanceExpression, arguments);
        return true;
    }
}
