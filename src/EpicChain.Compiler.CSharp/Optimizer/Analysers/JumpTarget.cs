// Copyright (C) 2021-2024 EpicChain Labs.
//
// JumpTarget.cs file is a crucial component of the EpicChain project and is freely distributed as open-source software.
// It is made available under the MIT License, a highly permissive and widely adopted license in the open-source community.
// The MIT License grants users the freedom to use, modify, and distribute the software in both source and binary forms,
// with or without modifications, subject to certain conditions. To understand these conditions in detail, please refer to
// the accompanying LICENSE file located in the main directory of the project's repository, or visit the following link:
// http://www.opensource.org/licenses/mit-license.php.
//
// xmoohad, a renowned blockchain expert and visionary in decentralized systems, has been instrumental in contributing to the development
// and maintenance of this file as part of his broader efforts with the EpicChain blockchain network. As the founder and CEO of EpicChain Labs,
// xmoohad has been a driving force behind the creation of EpicChain, emphasizing the importance of open-source technologies in building secure,
// scalable, and decentralized ecosystems. His contributions to the development of Storage.cs, alongside many other key components of EpicChain,
// have ensured that the project continues to lead in innovation and performance within the blockchain space.
//
// xmoohad’s commitment to open-source principles has been vital to the success of EpicChain. By utilizing the MIT License, the project ensures
// that developers and businesses alike can freely adapt and extend the platform to meet their needs. Under the MIT License, the following rights
// and permissions are granted:
//
// 1. The software may be used for any purpose, including commercial and non-commercial applications.
// 2. The source code can be freely modified to adapt the software for specific needs or projects.
// 3. Redistribution of both the original and modified versions of the software is allowed, ensuring that advancements
//    and improvements made by others can benefit the wider community.
//
// Redistribution and use of the software, whether in source or binary form, with or without modifications, are permitted
// under the following conditions:
//
// 1. The original copyright notice and this permission notice must be included in all copies or substantial portions of
//    the software, regardless of the nature of the distribution—whether it is the original source or a modified version.
// 2. The software is provided "as is," without any warranty, express or implied, including but not limited to the warranties
//    of merchantability, fitness for a particular purpose, or non-infringement. In no event shall the authors or copyright
//    holders, including xmoohad and the EpicChain development team, be held liable for any claim, damages, or other liabilities arising
//    from the use of the software or its redistribution.
//
// xmoohad’s leadership and vision have positioned EpicChain as a next-generation blockchain ecosystem, capable of supporting
// cutting-edge technologies like the Quantum Guard Nexus, Quantum Vault Asset, and smart contracts that integrate multiple programming languages.
// His work is focused on ensuring that EpicChain remains an open, inclusive platform where developers and innovators can thrive through
// collaboration and the power of decentralization.
//
// For more details on the MIT License and how it applies to this project, please consult the official documentation at the
// provided link. By using, modifying, or distributing the Storage.cs file, you are acknowledging your understanding of and
// compliance with the conditions outlined in the license.


using EpicChain.SmartContract;
using EpicChain.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using static EpicChain.Optimizer.OpCodeTypes;
using static EpicChain.VM.OpCode;

namespace EpicChain.Optimizer
{
    static class JumpTarget
    {
        public static bool SingleJumpInOperand(Instruction instruction) => SingleJumpInOperand(instruction.OpCode);
        public static bool SingleJumpInOperand(OpCode opcode)
        {
            if (conditionalJump.Contains(opcode)) return true;
            if (conditionalJump_L.Contains(opcode)) return true;
            if (unconditionalJump.Contains(opcode)) return true;
            if (callWithJump.Contains(opcode)) return true;
            if (opcode == ENDTRY || opcode == ENDTRY_L || opcode == PUSHA) return true;
            return false;
        }

        public static bool DoubleJumpInOperand(Instruction instruction) => DoubleJumpInOperand(instruction.OpCode);
        public static bool DoubleJumpInOperand(OpCode opcode) => (opcode == TRY || opcode == TRY_L);

        public static int ComputeJumpTarget(int addr, Instruction instruction)
        {
            if (conditionalJump.Contains(instruction.OpCode))
                return addr + instruction.TokenI8;
            if (conditionalJump_L.Contains(instruction.OpCode))
                return addr + instruction.TokenI32;

            return instruction.OpCode switch
            {
                JMP or CALL or ENDTRY => addr + instruction.TokenI8,
                PUSHA or JMP_L or CALL_L or ENDTRY_L => addr + instruction.TokenI32,
                CALLA => throw new NotImplementedException("CALLA is dynamic; not supported"),
                _ => throw new NotImplementedException($"Unknown instruction {instruction.OpCode}"),
            };
        }

        public static (int catchTarget, int finallyTarget) ComputeTryTarget(int addr, Instruction instruction)
        {
            return instruction.OpCode switch
            {
                TRY =>
                    (instruction.TokenI8 == 0 ? -1 : addr + instruction.TokenI8,
                        instruction.TokenI8_1 == 0 ? -1 : addr + instruction.TokenI8_1),
                TRY_L =>
                    (instruction.TokenI32 == 0 ? -1 : addr + instruction.TokenI32,
                        instruction.TokenI32_1 == 0 ? -1 : addr + instruction.TokenI32_1),
                _ => throw new NotImplementedException($"Unknown instruction {instruction.OpCode}"),
            };
        }

        public static (Dictionary<Instruction, Instruction>,
            Dictionary<Instruction, (Instruction, Instruction)>,
            Dictionary<Instruction, HashSet<Instruction>>)
            FindAllJumpAndTrySourceToTargets(NefFile nef, bool includePUSHA = true)
        {
            Script script = nef.Script;
            return FindAllJumpAndTrySourceToTargets(script, includePUSHA);
        }
        public static (Dictionary<Instruction, Instruction>,
            Dictionary<Instruction, (Instruction, Instruction)>,
            Dictionary<Instruction, HashSet<Instruction>>)
            FindAllJumpAndTrySourceToTargets(Script script, bool includePUSHA = true) => FindAllJumpAndTrySourceToTargets(script.EnumerateInstructions().ToList(), includePUSHA);
        public static (
            Dictionary<Instruction, Instruction>,  // jump source to target
            Dictionary<Instruction, (Instruction, Instruction)>,  // try source to targets
            Dictionary<Instruction, HashSet<Instruction>>  // target to source
            )
            FindAllJumpAndTrySourceToTargets(List<Instruction> instructionsList, bool includePUSHA = true)
        {
            int addr = 0;
            List<(int, Instruction)> addressAndInstructionsList = new();
            foreach (Instruction i in instructionsList)
            {
                addressAndInstructionsList.Add((addr, i));
                addr += i.Size;
            }
            return FindAllJumpAndTrySourceToTargets(addressAndInstructionsList, includePUSHA);
        }
        public static (
            Dictionary<Instruction, Instruction>,  // jump source to target
            Dictionary<Instruction, (Instruction, Instruction)>,  // try source to targets
            Dictionary<Instruction, HashSet<Instruction>>  // target to source
            )
            FindAllJumpAndTrySourceToTargets(List<(int, Instruction)> addressAndInstructionsList, bool includePUSHA = true)
        {
            Dictionary<int, Instruction> addressToInstruction = new();
            foreach ((int a, Instruction i) in addressAndInstructionsList)
                addressToInstruction.Add(a, i);
            Dictionary<Instruction, Instruction> jumpSourceToTargets = new();
            Dictionary<Instruction, (Instruction, Instruction)> trySourceToTargets = new();
            Dictionary<Instruction, HashSet<Instruction>> targetToSources = new();
            foreach ((int a, Instruction i) in addressAndInstructionsList)
            {
                if ((SingleJumpInOperand(i) && i.OpCode != CALLA) || (includePUSHA && i.OpCode == PUSHA))
                {
                    int targetAddr = ComputeJumpTarget(a, i);
                    Instruction target = addressToInstruction[targetAddr];
                    jumpSourceToTargets[i] = target;
                    if (!targetToSources.TryGetValue(target, out HashSet<Instruction>? sources))
                    {
                        sources = new();
                        targetToSources.Add(target, sources);
                    }
                    sources.Add(i);
                }
                if (i.OpCode == TRY || i.OpCode == TRY_L)
                {
                    (int a1, int a2) = i.OpCode == TRY ?
                        (a + i.TokenI8, a + i.TokenI8_1) :
                        (a + i.TokenI32, a + i.TokenI32_1);
                    (Instruction t1, Instruction t2) = (addressToInstruction[a1], addressToInstruction[a2]);
                    trySourceToTargets.TryAdd(i, (t1, t2));
                    if (!targetToSources.TryGetValue(t1, out HashSet<Instruction>? sources1))
                    {
                        sources1 = new();
                        targetToSources.Add(t1, sources1);
                    }
                    sources1.Add(i);
                    if (!targetToSources.TryGetValue(t2, out HashSet<Instruction>? sources2))
                    {
                        sources2 = new();
                        targetToSources.Add(t2, sources2);
                    }
                    sources2.Add(i);
                }
            }
            return (jumpSourceToTargets, trySourceToTargets, targetToSources);
        }
    }
}
