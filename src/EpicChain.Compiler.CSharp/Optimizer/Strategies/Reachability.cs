// Copyright (C) 2021-2024 EpicChain Lab's
//
// Reachability.cs file is a crucial component of the EpicChain project and is freely distributed as open-source software.
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

using EpicChain.Json;
using EpicChain.SmartContract;
using EpicChain.SmartContract.Manifest;
using EpicChain.VM;
using System.Collections.Generic;
using System.Linq;
using static EpicChain.Optimizer.JumpTarget;
using static EpicChain.Optimizer.OpCodeTypes;

namespace EpicChain.Optimizer
{
    static class Reachability
    {
        [Strategy(Priority = int.MaxValue - 16)]
        public static (NefFile, ContractManifest, JObject?) RemoveUncoveredInstructions(NefFile nef, ContractManifest manifest, JObject? debugInfo = null)
        {
            InstructionCoverage oldContractCoverage = new InstructionCoverage(nef, manifest);
            Dictionary<int, BranchType> coveredMap = oldContractCoverage.coveredMap;
            List<(int, Instruction)> oldAddressAndInstructionsList = oldContractCoverage.addressAndInstructions;
            Dictionary<int, Instruction> oldAddressToInstruction = new();
            foreach ((int a, Instruction i) in oldAddressAndInstructionsList)
                oldAddressToInstruction.Add(a, i);
            //DumpNef.GenerateDumpNef(nef, debugInfo);
            //coveredMap.Where(kv => !kv.Value).Select(kv => (kv.Key, oldAddressToInstruction[kv.Key].OpCode)).ToList();
            System.Collections.Specialized.OrderedDictionary simplifiedInstructionsToAddress = new();
            int currentAddress = 0;
            foreach ((int a, Instruction i) in oldAddressAndInstructionsList)
            {
                if (coveredMap[a] != BranchType.UNCOVERED)
                {
                    simplifiedInstructionsToAddress.Add(i, currentAddress);
                    currentAddress += i.Size;
                }
            }
            // retarget all NOP targets
            foreach ((int a, Instruction i) in oldAddressAndInstructionsList)
            {
                if (i.OpCode == OpCode.NOP && oldContractCoverage.jumpTargetToSources.ContainsKey(i))
                {
                    int currentA = a + i.Size;
                    Instruction currentI = oldAddressToInstruction[currentA];
                    while (coveredMap[currentA] == BranchType.UNCOVERED && currentI.OpCode == OpCode.NOP)
                    {
                        currentA += currentI.Size;
                        currentI = oldAddressToInstruction[currentA];
                    }
                    OptimizedScriptBuilder.RetargetJump(i, currentI,
                        oldContractCoverage.jumpInstructionSourceToTargets,
                        oldContractCoverage.tryInstructionSourceToTargets,
                        oldContractCoverage.jumpTargetToSources);
                }
            }

            return AssetBuilder.BuildOptimizedAssets(nef, manifest, debugInfo,
                simplifiedInstructionsToAddress,
                oldContractCoverage.jumpInstructionSourceToTargets, oldContractCoverage.tryInstructionSourceToTargets,
                oldAddressToInstruction);
        }

        public static Dictionary<int, BranchType>
            FindCoveredInstructions(NefFile nef, ContractManifest manifest)
            => new InstructionCoverage(nef, manifest).coveredMap;

        /// <summary>
        /// Removes JMP and JMP_L that targets the next instruction after the JMP or JMP_L.
        /// If the JMP or JMP_L itself is a jump target,
        /// re-target to the instruction after the JMP or JMP_L
        /// </summary>
        /// <param name="nef"></param>
        /// <param name="manifest"></param>
        /// <param name="debugInfo"></param>
        /// <returns></returns>
        [Strategy(Priority = int.MaxValue)]
        public static (NefFile, ContractManifest, JObject?) RemoveUnnecessaryJumps(NefFile nef, ContractManifest manifest, JObject? debugInfo = null)
        {
            Script script = nef.Script;
            List<(int a, Instruction i)> oldAddressAndInstructionsList = script.EnumerateInstructions().ToList();
            Dictionary<int, Instruction> oldAddressToInstruction = new();
            foreach ((int a, Instruction i) in oldAddressAndInstructionsList)
                oldAddressToInstruction.Add(a, i);
            (Dictionary<Instruction, Instruction> jumpSourceToTargets,
                Dictionary<Instruction, (Instruction, Instruction)> trySourceToTargets,
                Dictionary<Instruction, HashSet<Instruction>> jumpTargetToSources) =
                FindAllJumpAndTrySourceToTargets(oldAddressAndInstructionsList);

            System.Collections.Specialized.OrderedDictionary simplifiedInstructionsToAddress = new();
            int currentAddress = 0;
            foreach ((int a, Instruction i) in oldAddressAndInstructionsList)
            {
                if (unconditionalJump.Contains(i.OpCode))
                {
                    int target = ComputeJumpTarget(a, i);
                    if (target - a == i.Size)
                    {
                        // Just jumping to the instruction after the jump itself
                        // This is unnecessary jump. The jump should be deleted.
                        // And, if this JMP is the target of other jump instructions,
                        // re-target to the next instruction after this JMP.
                        Instruction nextInstruction = oldAddressToInstruction[a + i.Size];
                        // handle the reference of the deleted JMP
                        jumpSourceToTargets.Remove(i);
                        jumpTargetToSources[nextInstruction].Remove(i);
                        if (jumpTargetToSources[nextInstruction].Count == 0)
                            jumpTargetToSources.Remove(nextInstruction);
                        OptimizedScriptBuilder.RetargetJump(i, nextInstruction, jumpSourceToTargets, trySourceToTargets, jumpTargetToSources);
                        continue;  // do not add this JMP into simplified instructions
                    }
                }
                simplifiedInstructionsToAddress.Add(i, currentAddress);
                currentAddress += i.Size;
            }

            return AssetBuilder.BuildOptimizedAssets(nef, manifest, debugInfo,
                simplifiedInstructionsToAddress,
                jumpSourceToTargets, trySourceToTargets,
                oldAddressToInstruction);
        }

        /// <summary>
        /// If a JMP or JMP_L jumps to a RET, replace the JMP with RET
        /// </summary>
        /// <param name="nef"></param>
        /// <param name="manifest"></param>
        /// <param name="debugInfo"></param>
        /// <returns></returns>
        [Strategy(Priority = int.MaxValue - 4)]
        public static (NefFile, ContractManifest, JObject?) ReplaceJumpWithRet(NefFile nef, ContractManifest manifest, JObject? debugInfo = null)
        {
            Script script = nef.Script;
            List<(int a, Instruction i)> oldAddressAndInstructionsList = script.EnumerateInstructions().ToList();
            Dictionary<int, Instruction> oldAddressToInstruction = new();
            foreach ((int a, Instruction i) in oldAddressAndInstructionsList)
                oldAddressToInstruction.Add(a, i);
            (Dictionary<Instruction, Instruction> jumpSourceToTargets,
                Dictionary<Instruction, (Instruction, Instruction)> trySourceToTargets,
                Dictionary<Instruction, HashSet<Instruction>> jumpTargetToSources) =
                FindAllJumpAndTrySourceToTargets(oldAddressAndInstructionsList);
            Dictionary<int, int> oldSequencePointAddressToNew = new();

            System.Collections.Specialized.OrderedDictionary simplifiedInstructionsToAddress = new();
            int currentAddress = 0;
            foreach ((int a, Instruction i) in oldAddressAndInstructionsList)
            {
                if (unconditionalJump.Contains(i.OpCode))
                {
                    int target = ComputeJumpTarget(a, i);
                    if (!oldAddressToInstruction.TryGetValue(target, out Instruction? dstRet))
                        throw new BadScriptException($"Bad {nameof(oldAddressToInstruction)}. No target found for {i} jumping from {a} to {target}");
                    if (dstRet.OpCode == OpCode.RET)
                    {
                        oldSequencePointAddressToNew[a] = currentAddress;
                        // handle the reference of the deleted JMP
                        jumpSourceToTargets.Remove(i);
                        jumpTargetToSources[dstRet].Remove(i);
                        if (jumpTargetToSources[dstRet].Count == 0)
                            jumpTargetToSources.Remove(dstRet);
                        // handle the reference of the added RET
                        Instruction newRet = new Script(new byte[] { (byte)OpCode.RET }).GetInstruction(0);
                        // above is a workaround of new Instruction(OpCode.RET)
                        OptimizedScriptBuilder.RetargetJump(i, newRet,
                            jumpSourceToTargets, trySourceToTargets, jumpTargetToSources);
                        simplifiedInstructionsToAddress.Add(newRet, currentAddress);
                        currentAddress += newRet.Size;
                        continue;
                    }
                }
                simplifiedInstructionsToAddress.Add(i, currentAddress);
                currentAddress += i.Size;
            }

            return AssetBuilder.BuildOptimizedAssets(nef, manifest, debugInfo,
                simplifiedInstructionsToAddress,
                jumpSourceToTargets, trySourceToTargets,
                oldAddressToInstruction,
                oldSequencePointAddressToNew: oldSequencePointAddressToNew);
        }
    }
}
