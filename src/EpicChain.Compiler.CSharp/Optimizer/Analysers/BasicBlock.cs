// Copyright (C) 2021-2024 EpicChain Lab's
//
// BasicBlock.cs file is a crucial component of the EpicChain project and is freely distributed as open-source software.
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
    /// <summary>
    /// A basic block is a group of assembly instructions that are surely executed together.
    /// The start of a basic block can be the target of a jump, or an entry point of execution.
    /// The end of a basic block can be a jumping instruction, an ENDFINALLY, a RET, etc.
    /// Instructions in the same basic block can be replaced with more effcient ones.
    /// </summary>
    public class BasicBlock
    {
        public readonly int startAddr;
        public List<Instruction> instructions { get; set; }  // instructions in this basic block
        public BasicBlock? prevBlock = null;  // the previous basic block (with subseqent address)
        public BasicBlock? nextBlock = null;  // the following basic block (with subseqent address)
        public HashSet<BasicBlock> jumpTargetBlocks = new();  // jump target of the last instruction of this basic block
        public HashSet<BasicBlock> jumpSourceBlocks = new();
        public BranchType branchType = BranchType.UNCOVERED;

        public BasicBlock(int startAddr, List<Instruction> instructions)
        {
            this.startAddr = startAddr;
            this.instructions = instructions;
        }

        /// <summary>
        /// Sort the instruction dictionary by address
        /// </summary>
        /// <param name="instructions">address -> <see cref="Instruction"/></param>
        public BasicBlock(Dictionary<int, Instruction> instructions)
        {
            IEnumerable<(int addr, Instruction i)> addrToInstructions = from kv in instructions orderby kv.Key ascending select (kv.Key, kv.Value);
            this.startAddr = addrToInstructions.First().addr;
            this.instructions = addrToInstructions.Select(kv => kv.i).ToList();
        }

        //public void SetNextBasicBlock(BasicBlock block) => this.nextBlock = block;
        //public void SetJumpTargetBlock1(BasicBlock block) => this.jumpTargetBlock1 = block;
        //public void SetJumpTargetBlock2(BasicBlock block) => this.jumpTargetBlock2 = block;
    }

    /// <summary>
    /// Should contain all basic blocks in a contract
    /// </summary>
    public class ContractInBasicBlocks
    {
        public static Dictionary<int, Dictionary<int, Instruction>> BasicBlocksInDict(NefFile nef, ContractManifest manifest)
            => new InstructionCoverage(nef, manifest).basicBlocksInDict;

        public Dictionary<Instruction, BasicBlock> basicBlocksByStartInstruction;
        public Dictionary<int, BasicBlock> basicBlocksByStartAddr;
        public InstructionCoverage coverage;
        public IEnumerable<(int startAddr, List<Instruction> block)> sortedListInstructions;
        public List<BasicBlock> sortedBasicBlocks;
        public ContractManifest manifest;
        public JToken? debugInfo;
        public ContractInBasicBlocks(NefFile nef, ContractManifest manifest, JToken? debugInfo = null)
        {
            this.manifest = manifest;
            this.debugInfo = debugInfo;
            coverage = new InstructionCoverage(nef, manifest);
            sortedListInstructions =
                (from kv in coverage.basicBlocksInDict
                 orderby kv.Key ascending
                 select (kv.Key,
                     // kv.Value sorted by address
                     (from singleBlockKv in kv.Value orderby singleBlockKv.Key ascending select singleBlockKv.Value).ToList()
                 ));
            sortedBasicBlocks = new();
            basicBlocksByStartInstruction = new();
            basicBlocksByStartAddr = new();
            // build all blocks without handling jumps or continuations between blocks
            foreach ((int startAddr, List<Instruction> block) in sortedListInstructions)
            {
                BasicBlock thisBlock = new(startAddr, block);
                int firstNotNopAddr = startAddr;
                foreach (Instruction i in block)
                    if (i.OpCode == OpCode.NOP)
                        firstNotNopAddr += i.Size;
                thisBlock.branchType = coverage.coveredMap[firstNotNopAddr];
                sortedBasicBlocks.Add(thisBlock);
                basicBlocksByStartInstruction.Add(block.First(), thisBlock);
                basicBlocksByStartAddr.Add(startAddr, thisBlock);
            }
            // handle jumps and continuations between blocks
            foreach ((int startAddr, List<Instruction> block) in sortedListInstructions)
            {
                if (coverage.basicBlockContinuation.TryGetValue(startAddr, out int continuationTarget))
                {
                    basicBlocksByStartAddr[startAddr].nextBlock = basicBlocksByStartAddr[continuationTarget];
                    basicBlocksByStartAddr[continuationTarget].prevBlock = basicBlocksByStartAddr[startAddr];
                }
                if (coverage.basicBlockJump.TryGetValue(startAddr, out HashSet<int>? jumpTargets))
                    foreach (int target in jumpTargets)
                    {
                        basicBlocksByStartAddr[startAddr].jumpTargetBlocks.Add(basicBlocksByStartAddr[target]);
                        basicBlocksByStartAddr[target].jumpSourceBlocks.Add(basicBlocksByStartAddr[startAddr]);
                    }
            }
        }

        public IEnumerable<Instruction> GetScriptInstructions()
        {
            // WARNING: OpCode.NOP at the start of a basic block may not be included
            // and the jumping operands may be wrong
            // Refer to InstructionCoverage coverage for jump targets
            foreach ((_, List<Instruction> basicBlock) in sortedListInstructions)
                foreach (Instruction instruction in basicBlock)
                    yield return instruction;
        }
    }
}
