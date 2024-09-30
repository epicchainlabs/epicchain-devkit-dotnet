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
