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
using EpicChain.Cryptography;
using EpicChain.IO;
using EpicChain.SmartContract;
using EpicChain.VM;
using scfx::EpicChain.SmartContract.Framework.Attributes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace EpicChain.Compiler;

internal partial class MethodConvert
{
    private void ConvertExtern()
    {
        _inline = true;
        AttributeData? contractAttribute = Symbol.ContainingType.GetAttributes().FirstOrDefault(p => p.AttributeClass!.Name == nameof(ContractAttribute));
        if (contractAttribute is null)
        {
            bool emitted = false;
            foreach (AttributeData attribute in Symbol.GetAttributes())
            {
                using var sequencePoint = InsertSequencePoint(attribute.ApplicationSyntaxReference);

                switch (attribute.AttributeClass!.Name)
                {
                    case nameof(OpCodeAttribute):
                        if (!emitted)
                        {
                            emitted = true;
                            _callingConvention = CallingConvention.StdCall;
                        }
                        AddInstruction(new Instruction
                        {
                            OpCode = (OpCode)attribute.ConstructorArguments[0].Value!,
                            Operand = ((string)attribute.ConstructorArguments[1].Value!).HexToBytes(true)
                        });
                        break;
                    case nameof(SyscallAttribute):
                        if (!emitted)
                        {
                            emitted = true;
                            _callingConvention = CallingConvention.Cdecl;
                        }
                        AddInstruction(new Instruction
                        {
                            OpCode = OpCode.SYSCALL,
                            Operand = Encoding.ASCII.GetBytes((string)attribute.ConstructorArguments[0].Value!).Sha256()[..4]
                        });
                        break;
                    case nameof(CallingConventionAttribute):
                        emitted = true;
                        _callingConvention = (CallingConvention)attribute.ConstructorArguments[0].Value!;
                        break;
                }
            }
            if (Symbol.ToString()?.StartsWith("System.Array.Empty") == true)
            {
                emitted = true;
                AddInstruction(OpCode.NEWARRAY0);
            }
            else if (Symbol.ToString()?.Equals("EpicChain.SmartContract.Framework.Services.Runtime.Debug(string)") == true)
            {
                _context.AddEvent(new AbiEvent(Symbol, "Debug", new SmartContract.Manifest.ContractParameterDefinition() { Name = "message", Type = ContractParameterType.String }), false);
            }
            if (!emitted) throw new CompilationException(Symbol, DiagnosticId.ExternMethod, $"Unknown method: {Symbol}");
        }
        else
        {
            using var sequencePoint = InsertSequencePoint(contractAttribute.ApplicationSyntaxReference);

            UInt160 hash = UInt160.Parse((string)contractAttribute.ConstructorArguments[0].Value!);
            if (Symbol.MethodKind == MethodKind.PropertyGet)
            {
                AttributeData? attribute = Symbol.AssociatedSymbol!.GetAttributes().FirstOrDefault(p => p.AttributeClass!.Name == nameof(ContractHashAttribute));
                if (attribute is not null)
                {
                    Push(hash.ToArray());
                    return;
                }
            }
            string method = Symbol.GetDisplayName(true);
            ushort parametersCount = (ushort)Symbol.Parameters.Length;
            bool hasReturnValue = !Symbol.ReturnsVoid || Symbol.MethodKind == MethodKind.Constructor;
            CallContractMethod(hash, method, parametersCount, hasReturnValue);
        }
    }
}
