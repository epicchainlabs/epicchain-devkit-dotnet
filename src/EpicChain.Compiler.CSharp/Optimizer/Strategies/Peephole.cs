// Copyright (C) 2021-2024 EpicChain Labs.
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

namespace EpicChain.Optimizer
{
    public static class Peephole
    {
        public static HashSet<OpCode> RemoveDupDropOpCodes = new() { OpCode.REVERSEITEMS, OpCode.CLEARITEMS, OpCode.DUP, OpCode.DROP, OpCode.ABORTMSG };

        /// <summary>
        /// DUP SOMEOP DROP
        /// delete DUP and DROP when they are meaningless, optimizing to SOMPOP only
        /// This is mainly used for simple assignments like `a=1`, which is compiled to
        /// PUSH1 DUP STLOC:$index_of_a DROP
        /// This is correct compilation, because the expression `a=1` has return value 1
        /// The return value of assignment expression is used in continuous assignments like `a=b=1`
        /// But at runtime we just need PUSH1 STLOC:$index_of_a
        /// TODO in the future: use symbolic VM to judge multiple instructions between DUP and DROP
        /// </summary>
        /// <param name="nef"></param>
        /// <param name="manifest"></param>
        /// <param name="debugInfo"></param>
        /// <returns></returns>
        [Strategy(Priority = 1 << 10)]
        public static (NefFile, ContractManifest, JObject?) RemoveDupDrop(NefFile nef, ContractManifest manifest, JObject? debugInfo = null)
        {
            ContractInBasicBlocks contractInBasicBlocks = new(nef, manifest, debugInfo);
            InstructionCoverage oldContractCoverage = contractInBasicBlocks.coverage;
            Dictionary<int, Instruction> oldAddressToInstruction = new();
            foreach ((int a, Instruction i) in oldContractCoverage.addressAndInstructions)
                oldAddressToInstruction.Add(a, i);
            (Dictionary<Instruction, Instruction> jumpSourceToTargets,
                Dictionary<Instruction, (Instruction, Instruction)> trySourceToTargets,
                Dictionary<Instruction, HashSet<Instruction>> jumpTargetToSources) =
                (oldContractCoverage.jumpInstructionSourceToTargets,
                oldContractCoverage.tryInstructionSourceToTargets,
                oldContractCoverage.jumpTargetToSources);
            System.Collections.Specialized.OrderedDictionary simplifiedInstructionsToAddress = new();
            int currentAddress = 0;
            foreach (List<Instruction> basicBlock in contractInBasicBlocks.sortedListInstructions.Select(i => i.block))
            {
                for (int index = 0; index < basicBlock.Count; index++)
                {
                    if (index + 2 < basicBlock.Count
                     && basicBlock[index].OpCode == OpCode.DUP
                     && basicBlock[index + 2].OpCode == OpCode.DROP)
                    {
                        Instruction currentDup = basicBlock[index];
                        Instruction nextInstruction = basicBlock[index + 1];
                        OpCode opAfterDup = nextInstruction.OpCode;
                        if (OpCodeTypes.storeArguments.Contains(opAfterDup)
                         || OpCodeTypes.storeStaticFields.Contains(opAfterDup)
                         || OpCodeTypes.storeLocalVariables.Contains(opAfterDup)
                         || RemoveDupDropOpCodes.Contains(opAfterDup))
                        {
                            // Include only the instruction between DUP and DROP
                            simplifiedInstructionsToAddress.Add(nextInstruction, currentAddress);
                            currentAddress += nextInstruction.Size;
                            index += 2;

                            // If the old DUP is target of jump, re-target to the next instruction
                            OptimizedScriptBuilder.RetargetJump(currentDup, nextInstruction,
                                jumpSourceToTargets, trySourceToTargets, jumpTargetToSources);
                            continue;
                        }
                    }
                    simplifiedInstructionsToAddress.Add(basicBlock[index], currentAddress);
                    currentAddress += basicBlock[index].Size;
                }
            }
            return AssetBuilder.BuildOptimizedAssets(nef, manifest, debugInfo,
                simplifiedInstructionsToAddress,
                jumpSourceToTargets, trySourceToTargets,
                oldAddressToInstruction);
        }
    }
}
