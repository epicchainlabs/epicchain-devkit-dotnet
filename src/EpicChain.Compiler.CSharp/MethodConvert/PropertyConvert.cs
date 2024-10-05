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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using EpicChain.IO;
using EpicChain.SmartContract;
using EpicChain.SmartContract.Native;
using EpicChain.VM;
using scfx::EpicChain.SmartContract.Framework.Attributes;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace EpicChain.Compiler;

internal partial class MethodConvert
{
    private void ConvertNoBody(AccessorDeclarationSyntax syntax)
    {
        _callingConvention = CallingConvention.Cdecl;
        IPropertySymbol property = (IPropertySymbol)Symbol.AssociatedSymbol!;
        AttributeData? attribute = property.GetAttributes().FirstOrDefault(p => p.AttributeClass!.Name == nameof(StoredAttribute));
        using (InsertSequencePoint(syntax))
        {
            _inline = attribute is null;
            ConvertFieldBackedProperty(property);
            if (attribute is not null)
                ConvertStorageBackedProperty(property, attribute);
        }
    }

    private void ConvertFieldBackedProperty(IPropertySymbol property)
    {
        IFieldSymbol[] fields = property.ContainingType.GetAllMembers().OfType<IFieldSymbol>().ToArray();
        if (Symbol.IsStatic)
        {
            IFieldSymbol backingField = Array.Find(fields, p => SymbolEqualityComparer.Default.Equals(p.AssociatedSymbol, property))!;
            byte backingFieldIndex = _context.AddStaticField(backingField);
            switch (Symbol.MethodKind)
            {
                case MethodKind.PropertyGet:
                    AccessSlot(OpCode.LDSFLD, backingFieldIndex);
                    break;
                case MethodKind.PropertySet:
                    if (!_inline) AccessSlot(OpCode.LDARG, 0);
                    AccessSlot(OpCode.STSFLD, backingFieldIndex);
                    break;
                default:
                    throw new CompilationException(Symbol, DiagnosticId.SyntaxNotSupported, $"Unsupported accessor: {Symbol}");
            }
        }
        else
        {
            fields = fields.Where(p => !p.IsStatic).ToArray();
            int backingFieldIndex = Array.FindIndex(fields, p => SymbolEqualityComparer.Default.Equals(p.AssociatedSymbol, property));
            switch (Symbol.MethodKind)
            {
                case MethodKind.PropertyGet:
                    if (!_inline) AccessSlot(OpCode.LDARG, 0);
                    Push(backingFieldIndex);
                    AddInstruction(OpCode.PICKITEM);
                    break;
                case MethodKind.PropertySet:
                    if (_inline)
                    {
                        Push(backingFieldIndex);
                        AddInstruction(OpCode.ROT);
                    }
                    else
                    {
                        AccessSlot(OpCode.LDARG, 0);
                        Push(backingFieldIndex);
                        AccessSlot(OpCode.LDARG, 1);
                    }
                    AddInstruction(OpCode.SETITEM);
                    break;
                default:
                    throw new CompilationException(Symbol, DiagnosticId.SyntaxNotSupported, $"Unsupported accessor: {Symbol}");
            }
        }
    }

    private byte[] GetStorageBackedKey(IPropertySymbol property, AttributeData attribute)
    {
        byte[] key;

        if (attribute.ConstructorArguments.Length == 0)
        {
            key = Utility.StrictUTF8.GetBytes(property.Name);
        }
        else
        {
            if (attribute.ConstructorArguments[0].Value is byte b)
            {
                key = [b];
            }
            else if (attribute.ConstructorArguments[0].Value is string s)
            {
                key = Utility.StrictUTF8.GetBytes(s);
            }
            else
            {
                throw new CompilationException(Symbol, DiagnosticId.SyntaxNotSupported, $"Unknown StorageBacked constructor: {Symbol}");
            }
        }
        return key;
    }

    private void ConvertStorageBackedProperty(IPropertySymbol property, AttributeData attribute)
    {
        IFieldSymbol[] fields = property.ContainingType.GetAllMembers().OfType<IFieldSymbol>().ToArray();
        byte[] key = GetStorageBackedKey(property, attribute);
        if (Symbol.MethodKind == MethodKind.PropertyGet)
        {
            JumpTarget endTarget = new();
            if (Symbol.IsStatic)
            {
                // AddInstruction(OpCode.DUP);
                AddInstruction(OpCode.ISNULL);
                // Ensure that no object was sent
                Jump(OpCode.JMPIFNOT_L, endTarget);
            }
            else
            {
                // Check class
                Jump(OpCode.JMPIF_L, endTarget);
            }
            Push(key);
            CallInteropMethod(ApplicationEngine.System_Storage_GetReadOnlyContext);
            CallInteropMethod(ApplicationEngine.System_Storage_Get);
            switch (property.Type.Name)
            {
                case "byte":
                case "sbyte":
                case "Byte":
                case "SByte":

                case "short":
                case "ushort":
                case "Int16":
                case "UInt16":

                case "int":
                case "uint":
                case "Int32":
                case "UInt32":

                case "long":
                case "ulong":
                case "Int64":
                case "UInt64":
                case "BigInteger":
                    // Replace NULL with 0
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ISNULL);
                    JumpTarget ifFalse = new();
                    Jump(OpCode.JMPIFNOT_L, ifFalse);
                    {
                        AddInstruction(OpCode.DROP);
                        AddInstruction(OpCode.PUSH0);
                    }
                    ifFalse.Instruction = AddInstruction(OpCode.NOP);
                    break;
                case "String":
                case "ByteString":
                case "UInt160":
                case "UInt256":
                case "ECPoint":
                    break;
                default:
                    CallContractMethod(NativeContract.EssentialLib.Hash, "deserialize", 1, true);
                    break;
            }
            AddInstruction(OpCode.DUP);
            if (Symbol.IsStatic)
            {
                IFieldSymbol backingField = Array.Find(fields, p => SymbolEqualityComparer.Default.Equals(p.AssociatedSymbol, property))!;
                byte backingFieldIndex = _context.AddStaticField(backingField);
                AccessSlot(OpCode.STSFLD, backingFieldIndex);
            }
            else
            {
                fields = fields.Where(p => !p.IsStatic).ToArray();
                int backingFieldIndex = Array.FindIndex(fields, p => SymbolEqualityComparer.Default.Equals(p.AssociatedSymbol, property));
                AccessSlot(OpCode.LDARG, 0);
                Push(backingFieldIndex);
                AddInstruction(OpCode.ROT);
                AddInstruction(OpCode.SETITEM);
            }
            endTarget.Instruction = AddInstruction(OpCode.NOP);
        }
        else
