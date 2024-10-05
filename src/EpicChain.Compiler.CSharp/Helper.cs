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
using EpicChain.SmartContract;
using EpicChain.SmartContract.Manifest;
using EpicChain.VM;
using EpicChain.VM.Types;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace EpicChain.Compiler
{
    static class Helper
    {
        public static byte[] HexToBytes(this string hex, bool removePrefix)
        {
            ReadOnlySpan<char> s = hex;
            if (removePrefix && hex.StartsWith("0x"))
                s = s[2..];
            return Convert.FromHexString(s);
        }

        public static bool IsSubclassOf(this ITypeSymbol type, string baseTypeName, bool includeThisClass = false)
        {
            ITypeSymbol? baseType = includeThisClass ? type : type.BaseType;
            while (baseType is not null)
            {
                if (baseType.Name == baseTypeName)
                    return true;
                baseType = baseType.BaseType;
            }
            return false;
        }

        public static bool IsVirtualMethod(this IMethodSymbol method)
        {
            return method.IsAbstract || method.IsVirtual || method.IsOverride;
        }

        public static bool IsInternalCoreMethod(this IMethodSymbol method)
        {
            return method.Name switch
            {
                "_initialize" => true,
                "_deploy" => true,
                _ => false,
            };
        }

        public static ContractParameterType GetContractParameterType(this ITypeSymbol type)
        {
            if (type is INamedTypeSymbol { NullableAnnotation: NullableAnnotation.Annotated } namedType)
            {
                // use the original type for nullable types, depend on the script to deal with null for value types
                type = namedType.TypeArguments.Length > 0 ? namedType.TypeArguments[0] : namedType.OriginalDefinition;
            }

            switch (type.ToString())
            {
                case "void": return ContractParameterType.Void;
                case "bool": return ContractParameterType.Boolean;
                case "char": return ContractParameterType.Integer;
                case "sbyte": return ContractParameterType.Integer;
                case "byte": return ContractParameterType.Integer;
                case "short": return ContractParameterType.Integer;
                case "ushort": return ContractParameterType.Integer;
                case "int": return ContractParameterType.Integer;
                case "uint": return ContractParameterType.Integer;
                case "long": return ContractParameterType.Integer;
                case "ulong": return ContractParameterType.Integer;
                case "string": return ContractParameterType.String;
                case "byte[]": return ContractParameterType.ByteArray;
                case "object": return ContractParameterType.Any;
                case "EpicChain.Cryptography.ECC.ECPoint": // Old EpicChain.SmartContract.Framework
                case "EpicChain.SmartContract.Framework.ECPoint": return ContractParameterType.PublicKey;
                case "EpicChain.SmartContract.Framework.ByteString": return ContractParameterType.ByteArray;
                case "EpicChain.UInt160": // Old EpicChain.SmartContract.Framework
                case "EpicChain.SmartContract.Framework.UInt160": return ContractParameterType.Hash160;
                case "EpicChain.UInt256": // Old EpicChain.SmartContract.Framework
                case "EpicChain.SmartContract.Framework.UInt256": return ContractParameterType.Hash256;
                case "System.Numerics.BigInteger": return ContractParameterType.Integer;
            }
            if (type.Name == "Map") return ContractParameterType.Map;
            if (type.Name == "List") return ContractParameterType.Array;
            if (type.TypeKind == TypeKind.Enum) return ContractParameterType.Integer;
            if (type is IArrayTypeSymbol) return ContractParameterType.Array;
            if (type.AllInterfaces.Any(p => p.Name == nameof(scfx::EpicChain.SmartContract.Framework.IApiInterface)))
                return ContractParameterType.InteropInterface;
            if (type.IsValueType) return ContractParameterType.Array;
            return ContractParameterType.Any;
        }

        public static StackItemType GetStackItemType(this ITypeSymbol type)
        {
            return type.SpecialType switch
            {
                SpecialType.System_Boolean => StackItemType.Boolean,
                SpecialType.System_Char => StackItemType.Integer,
                SpecialType.System_SByte => StackItemType.Integer,
                SpecialType.System_Byte => StackItemType.Integer,
                SpecialType.System_Int16 => StackItemType.Integer,
                SpecialType.System_UInt16 => StackItemType.Integer,
                SpecialType.System_Int32 => StackItemType.Integer,
                SpecialType.System_UInt32 => StackItemType.Integer,
                SpecialType.System_Int64 => StackItemType.Integer,
                SpecialType.System_UInt64 => StackItemType.Integer,
                SpecialType.System_String => StackItemType.ByteString,
                _ => type.Name switch
                {
                    "byte[]" => StackItemType.Buffer,
                    nameof(BigInteger) => StackItemType.Integer,
                    nameof(ByteString) => StackItemType.ByteString,
                    _ => StackItemType.Any
                }
            };
        }

        public static StackItemType GetPatternType(this ITypeSymbol type)
        {
            return type.ToString() switch
            {
                "bool" => StackItemType.Boolean,
                "byte[]" => StackItemType.Buffer,
                "string" => StackItemType.ByteString,
                "EpicChain.SmartContract.Framework.ByteString" => StackItemType.ByteString,
                "System.Numerics.BigInteger" => StackItemType.Integer,
                _ => throw new CompilationException(type, DiagnosticId.SyntaxNotSupported, $"Unsupported pattern type: {type}")
            };
        }

        public static AttributeData? GetAttributeFromSelfOrParent(this IMethodSymbol symbol, string name)
        {
            ISymbol? i = symbol;
            do
            {
                (AttributeData? attribute, i) = i switch
                {
                    IMethodSymbol s => (s.GetAttributesWithInherited().FirstOrDefault(p => p.AttributeClass?.Name == name), s.MethodKind == MethodKind.PropertyGet || s.MethodKind == MethodKind.PropertySet ? s.AssociatedSymbol : s.ContainingSymbol),
                    IPropertySymbol s => (s.GetAttributesWithInherited().FirstOrDefault(p => p.AttributeClass?.Name == name), s.ContainingSymbol),
                    INamedTypeSymbol s => (s.GetAttributesWithInherited().FirstOrDefault(p => p.AttributeClass?.Name == name), s.ContainingSymbol),
                    _ => (i.GetAttributes().FirstOrDefault(p => p.AttributeClass?.Name == name), i.ContainingSymbol),
                };
                if (attribute != null) return attribute;
            } while (i != null);
            return null;
        }

        public static IEnumerable<AttributeData> GetAttributesWithInherited(this INamedTypeSymbol symbol)
        {
            foreach (var attribute in symbol.GetAttributes())
            {
                yield return attribute;
            }

            var baseType = symbol.BaseType;
            while (baseType != null)
            {
                foreach (var attribute in baseType.GetAttributes())
                {
                    if (IsInherited(attribute))
                    {
                        yield return attribute;
                    }
                }

                baseType = baseType.BaseType;
            }
        }

        public static IEnumerable<AttributeData> GetAttributesWithInherited(this IMethodSymbol symbol)
        {
            foreach (var attribute in symbol.GetAttributes())
            {
                yield return attribute;
            }

            var overriddenMethod = symbol.OverriddenMethod;
            while (overriddenMethod != null)
            {
                foreach (var attribute in overriddenMethod.GetAttributes())
                {
                    if (IsInherited(attribute))
                    {
                        yield return attribute;
                    }
                }
                overriddenMethod = overriddenMethod.OverriddenMethod;
            }
        }

        public static IEnumerable<AttributeData> GetAttributesWithInherited(this IPropertySymbol symbol)
        {
            foreach (var attribute in symbol.GetAttributes())
            {
                yield return attribute;
            }

            var overriddenProperty = symbol.OverriddenProperty;
            while (overriddenProperty != null)
            {
                foreach (var attribute in overriddenProperty.GetAttributes())
                {
                    if (IsInherited(attribute))
                    {
                        yield return attribute;
                    }
                }
                overriddenProperty = overriddenProperty.OverriddenProperty;
            }
        }

        private static bool IsInherited(this AttributeData attribute)
        {
            if (attribute.AttributeClass == null)
            {
                return false;
            }

            foreach (var attributeAttribute in attribute.AttributeClass.GetAttributes())
            {
                var @class = attributeAttribute.AttributeClass;
                if (@class != null && @class.Name == nameof(AttributeUsageAttribute) &&
                    @class.ContainingNamespace?.Name == "System")
                {
                    foreach (var kvp in attributeAttribute.NamedArguments)
                    {
                        if (kvp.Key == nameof(AttributeUsageAttribute.Inherited))
                        {
                            return (bool)kvp.Value.Value!;
                        }
                    }

                    // Default value of Inherited is true
                    return true;
                }
            }

            return false;
        }

        public static ISymbol[] GetAllMembers(this ITypeSymbol type)
        {
            return GetAllMembersInternal(type).ToArray();
        }

        private static IEnumerable<ISymbol> GetAllMembersInternal(ITypeSymbol type)
        {
            if (type.SpecialType == SpecialType.System_Object) yield break;
            if (type.Name == nameof(Attribute)) yield break;
            List<ISymbol> myMembers = type.GetMembers().ToList();
            if (type.IsReferenceType)
                foreach (ISymbol member in GetAllMembersInternal(type.BaseType!))
                {
                    if (member is IMethodSymbol method && (method.MethodKind == MethodKind.Constructor || method.MethodKind == MethodKind.StaticConstructor))
                    {
                        continue;
                    }
                    else if (member.IsAbstract || member.IsVirtual || member.IsOverride)
                    {
                        int index = myMembers.FindIndex(p => p is IMethodSymbol method && SymbolEqualityComparer.Default.Equals(method.OverriddenMethod, member));
                        if (index >= 0)
                        {
                            yield return myMembers[index];
                            myMembers.RemoveAt(index);
                        }
                        else
                        {
                            yield return member;
                        }
                    }
                    else
                    {
                        yield return member;
                    }
                }
            foreach (ISymbol member in myMembers)
            {
                yield return member;
            }
        }

        public static IFieldSymbol[] GetFields(this ITypeSymbol type)
        {
            return type.GetAllMembers().OfType<IFieldSymbol>().Where(p => !p.IsStatic).ToArray();
        }

        public static string GetDisplayName(this ISymbol symbol, bool lowercase = false)
        {
            AttributeData? attribute = symbol.GetAttributes().FirstOrDefault(p => p.AttributeClass!.Name == nameof(DisplayNameAttribute));
            if (attribute is not null) return (string)attribute.ConstructorArguments[0].Value!;
            if (symbol is IMethodSymbol method)
            {
                switch (method.MethodKind)
                {
                    case MethodKind.Constructor:
                        symbol = method.ContainingType;
                        break;
                    case MethodKind.PropertyGet:
                        ISymbol property = method.AssociatedSymbol!;
                        attribute = property.GetAttributes().FirstOrDefault(p => p.AttributeClass!.Name == nameof(DisplayNameAttribute));
                        if (attribute is not null) return (string)attribute.ConstructorArguments[0].Value!;
                        symbol = property;
                        break;
                    case MethodKind.PropertySet:
                        return "set" + symbol.Name[4..];
                    case MethodKind.StaticConstructor:
                        return "_initialize";
                }
            }
            if (lowercase)
                return symbol.Name[..1].ToLowerInvariant() + symbol.Name[1..];
            else
                return symbol.Name;
        }

        public static ContractParameterDefinition ToAbiParameter(this IParameterSymbol symbol)
        {
            return new ContractParameterDefinition
            {
                Name = symbol.Name,
                Type = symbol.Type.GetContractParameterType()
            };
        }

        public static void RebuildOffsets(this IReadOnlyList<Instruction> instructions)
        {
            int offset = 0;
            foreach (Instruction instruction in instructions)
            {
                instruction.Offset = offset;
                offset += instruction.Size;
            }
        }

        public static void RebuildOperands(this IReadOnlyList<Instruction> instructions)
        {
            foreach (Instruction instruction in instructions)
            {
                if (instruction.Target is null) continue;
                bool isLong;
                if (instruction.OpCode >= OpCode.JMP && instruction.OpCode <= OpCode.CALL_L)
                    isLong = (instruction.OpCode - OpCode.JMP) % 2 != 0;
                else
                    isLong = instruction.OpCode == OpCode.PUSHA || instruction.OpCode == OpCode.CALLA || instruction.OpCode == OpCode.TRY_L || instruction.OpCode == OpCode.ENDTRY_L;
                if (instruction.OpCode == OpCode.TRY || instruction.OpCode == OpCode.TRY_L)
                {
                    int offset1 = (instruction.Target.Instruction?.Offset - instruction.Offset) ?? 0;
                    int offset2 = (instruction.Target2!.Instruction?.Offset - instruction.Offset) ?? 0;
                    if (isLong)
                    {
                        instruction.Operand = new byte[sizeof(int) + sizeof(int)];
                        BinaryPrimitives.WriteInt32LittleEndian(instruction.Operand, offset1);
                        BinaryPrimitives.WriteInt32LittleEndian(instruction.Operand.AsSpan(sizeof(int)), offset2);
                    }
                    else
                    {
                        instruction.Operand = new byte[sizeof(sbyte) + sizeof(sbyte)];
                        sbyte sbyte1 = checked((sbyte)offset1);
                        sbyte sbyte2 = checked((sbyte)offset2);
                        instruction.Operand[0] = unchecked((byte)sbyte1);
                        instruction.Operand[1] = unchecked((byte)sbyte2);
                    }
                }
                else
                {
                    int offset = instruction.Target.Instruction!.Offset - instruction.Offset;
                    if (isLong)
                    {
                        instruction.Operand = BitConverter.GetBytes(offset);
                    }
                    else
                    {
                        sbyte sbyte1 = checked((sbyte)offset);
                        instruction.Operand = new[] { unchecked((byte)sbyte1) };
                    }
                }
            }
        }
    }
}
