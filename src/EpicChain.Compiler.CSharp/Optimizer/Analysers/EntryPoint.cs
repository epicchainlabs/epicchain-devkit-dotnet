// Copyright (C) 2021-2024 EpicChain Labs.
//
// EntryPoint.cs file is a crucial component of the EpicChain project and is freely distributed as open-source software.
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
using EpicChain.SmartContract.Manifest;
using EpicChain.VM;
using System.Collections.Generic;
using System.Linq;

namespace EpicChain.Optimizer
{
    public enum EntryType
    {
        PublicMethod,
        Initialize,
        Deploy,
        PUSHA,
    }

    public static class EntryPoint
    {
        /// <summary>
        /// Gets a dictionary of method entry points based on the contract manifest and debug information.
        /// </summary>
        /// <param name="manifest">The contract manifest.</param>
        /// <param name="debugInfo">The debug information.</param>
        /// <returns>A dictionary containing method entry points. (addr -> EntryType, hasCallA)</returns>
        public static Dictionary<int, EntryType> EntryPointsByMethod(ContractManifest manifest)
        {
            Dictionary<int, EntryType> result = new();
            foreach (ContractMethodDescriptor method in manifest.Abi.Methods)
            {
                if (method.Name == "_initialize")
                {
                    result.Add(method.Offset, EntryType.Initialize);
                    continue;
                }
                if (method.Name == "_deploy")
                {
                    result.Add(method.Offset, EntryType.Deploy);
                    continue;
                }
                result.Add(method.Offset, EntryType.PublicMethod);
            }
            return result;
        }

        /// <summary>
        /// Gets a dictionary of entry points based on the CALLA instruction.
        /// </summary>
        /// <param name="nef">The NEF file.</param>
        /// <returns>A dictionary containing entry points.</returns>
        public static Dictionary<int, EntryType> EntryPointsByCallA(NefFile nef)
        {
            Dictionary<int, EntryType> result = new();
            Script script = nef.Script;
            List<(int, Instruction)> instructions = script.EnumerateInstructions().ToList();
            bool hasCallA = HasCallA(instructions);
            if (hasCallA)
                foreach ((int addr, Instruction instruction) in instructions)
                    if (instruction.OpCode == OpCode.PUSHA)
                    {
                        int target = JumpTarget.ComputeJumpTarget(addr, instruction);
                        if (target != addr && target >= 0)
                            result[target] = EntryType.PUSHA;
                    }
            return result;
        }

        /// <summary>
        /// Checks if the list of instructions contains the CALLA instruction.
        /// </summary>
        /// <param name="instructions">The list of instructions.</param>
        /// <returns>True if the CALLA instruction exists; otherwise, false.</returns>
        public static bool HasCallA(List<(int, Instruction)> instructions)
        {
            bool hasCallA = false;
            foreach ((_, Instruction instruction) in instructions)
                if (instruction.OpCode == OpCode.CALLA)
                {
                    hasCallA = true;
                    break;
                }
            return hasCallA;
        }

        /// <summary>
        /// Checks if the NEF file contains the CALLA instruction.
        /// </summary>
        /// <param name="nef">The NEF file.</param>
        /// <returns>True if the NEF file contains the CALLA instruction; otherwise, false.</returns>
        public static bool HasCallA(NefFile nef)
        {
            Script script = nef.Script;
            return HasCallA(script.EnumerateInstructions().ToList());
        }

        /// <summary>
        /// Gets a dictionary of all entry points, including those calculated based on the CALLA instruction and methods.
        /// </summary>
        /// <param name="nef">The NEF file.</param>
        /// <param name="manifest">The contract manifest.</param>
        /// <param name="debugInfo">The debug information.</param>
        /// <returns>A dictionary containing all entry points.</returns>
        public static Dictionary<int, EntryType> AllEntryPoints(NefFile nef, ContractManifest manifest)
            => EntryPointsByCallA(nef).Concat(EntryPointsByMethod(manifest)).ToDictionary(kv => kv.Key, kv => kv.Value);
    }
}
