// Copyright (C) 2021-2024 EpicChain Labs.
//
// InstructionCoverage.cs file is a crucial component of the EpicChain project and is freely distributed as open-source software.
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static EpicChain.Optimizer.JumpTarget;
using static EpicChain.Optimizer.OpCodeTypes;

namespace EpicChain.Optimizer
{
    public enum TryStackType
    {
        ENTRY,
        TRY,
        CATCH,
        FINALLY,
    }

    [DebuggerDisplay("{catchAddr}, {finallyAddr}, {tryStackType}, {continueAfterFinally}")]
    public struct TryStack
    {
        int catchAddr;
        int finallyAddr;
        TryStackType tryStackType;
        bool continueAfterFinally;

        public TryStack(int catchAddr, int finallyAddr, TryStackType tryStackType, bool continueAfterFinally)
        {
            this.catchAddr = catchAddr;
            this.finallyAddr = finallyAddr;
            this.tryStackType = tryStackType;
            this.continueAfterFinally = continueAfterFinally;
        }

        internal void Deconstruct(out int catchAddr, out int finallyAddr,
            out TryStackType tryStackType, out bool continueAfterFinally)
        {
            catchAddr = this.catchAddr;
            finallyAddr = this.finallyAddr;
            tryStackType = this.tryStackType;
            continueAfterFinally = this.continueAfterFinally;
        }
    }

    public enum BranchType
    {
        OK = 1,     // One of the branches may return without exception
        THROW = 2,  // All branches surely have exceptions, but can be catched
        ABORT = 3,  // All branches abort, and cannot be catched
        UNCOVERED = 4,
    }

    public class InstructionCoverage
    {
        Script script;
        // Starting from the address, whether the call will surely throw or surely abort, or may be OK
        public Dictionary<int, BranchType> coveredMap { get; protected set; }

        // key: starting address of basic block
        // value: addr -> instruction of all instructions in this basic block
        public Dictionary<int, Dictionary<int, Instruction>> basicBlocksInDict { get; protected set; }

        // key: starting address of basic block
        // value: starting address of the next basic block,
        //   which is reached by increased instruction pointer in normal execution
        public Dictionary<int, int> basicBlockContinuation { get; protected set; } = new();

        // key: starting address of basic block
        // value: starting address of basic blocks that is jumped to, from this basic block
        public Dictionary<int, HashSet<int>> basicBlockJump { get; protected set; } = new();
        public List<(int a, Instruction i)> addressAndInstructions { get; init; }
        public Dictionary<Instruction, Instruction> jumpInstructionSourceToTargets { get; init; }
        public Dictionary<Instruction, (Instruction, Instruction)> tryInstructionSourceToTargets { get; init; }
        /// <summary>
        /// key: target of all kinds of Instruction that has 1 or 2 jump targets
        /// value: sources of that jump target
        /// </summary>
        public Dictionary<Instruction, HashSet<Instruction>> jumpTargetToSources { get; init; }
        public Dictionary<int, EntryType> pushaTargets { get; init; }

        public InstructionCoverage(NefFile nef, ContractManifest manifest)
        {
            this.script = nef.Script;
            coveredMap = new();
            basicBlocksInDict = new();
            addressAndInstructions = script.EnumerateInstructions().ToList();
            (jumpInstructionSourceToTargets, tryInstructionSourceToTargets, jumpTargetToSources) =
                FindAllJumpAndTrySourceToTargets(addressAndInstructions);
            pushaTargets = EntryPoint.EntryPointsByCallA(nef);
            foreach ((int addr, Instruction _) in addressAndInstructions)
                coveredMap.Add(addr, BranchType.UNCOVERED);

            // It is unsafe to go parallel, because the coveredMap value is not true/false
            //Parallel.ForEach(manifest.Abi.Methods, method =>
            //    CoverInstruction(method.Offset, script, coveredMap)
            //);
            foreach ((int addr, _) in EntryPoint.EntryPointsByMethod(manifest))
                CoverInstruction(addr);
        }

        public static Stack<TryStack> CopyStack(Stack<TryStack> stack) => new(stack.Reverse());

        public BranchType HandleThrow(int entranceAddr, int throwFromAddr, Stack<TryStack> stack)
        {
            stack = CopyStack(stack);
            TryStackType stackType;
            int catchAddr; int finallyAddr;
            do
                (catchAddr, finallyAddr, stackType, _) = stack.Pop();
            while (stackType != TryStackType.TRY && stackType != TryStackType.CATCH && stack.Count > 0);
            if (stackType == TryStackType.TRY)  // goto CATCH or FINALLY
            {
                // try with catch: cancel throw and execute catch
                if (catchAddr != -1)
                {
                    int addr = catchAddr;
                    stack.Push(new TryStack(-1, finallyAddr, TryStackType.CATCH, true));
                    return CoverInstruction(addr, stack: stack, jumpFromBasicBlockEntranceAddr: entranceAddr);
                }
                // try without catch: execute finally but do not visit codes after finally
                else if (finallyAddr != -1)
                {
                    stack.Push(new(-1, -1, TryStackType.FINALLY, false));
                    if (CoverInstruction(finallyAddr, stack, jumpFromBasicBlockEntranceAddr: entranceAddr) == BranchType.ABORT)
                        // ABORT in finally
                        return BranchType.ABORT;
                    return BranchType.THROW;
                }
                throw new BadScriptException("Try without catch or finally");
            }
            // not throwed in try
            // throwed in catch with finally: execute finally,
            // and do not continue after ENDFINALLY
            if (stackType == TryStackType.CATCH)
            {
                if (finallyAddr != -1)
                {
                    stack.Push(new(-1, -1, TryStackType.FINALLY, false));
                    if (CoverInstruction(finallyAddr, stack, jumpFromBasicBlockEntranceAddr: entranceAddr) == BranchType.ABORT)
                        // ABORT in finally
                        return BranchType.ABORT;
                }
            }
            return BranchType.THROW;
        }

        public BranchType HandleAbort(int entranceAddr, int abortFromAddr, Stack<TryStack> stack)
        {
            // See if we are in a try or catch. There may still be runtime exceptions
            (int catchAddr, int finallyAddr, TryStackType stackType, _) = stack.Peek();
            if (stackType == TryStackType.TRY && catchAddr != -1 ||
                stackType == TryStackType.CATCH && finallyAddr != -1)
            {
                // Visit catchAddr because there may still be exceptions at runtime
                if (HandleThrow(entranceAddr, abortFromAddr, stack) == BranchType.OK)
                    return BranchType.OK;  // No need to set coveredMap[entranceAddr] because it's OK when covered
            }
            return BranchType.ABORT;
        }

        /// <summary>
        /// Cover a basic block, and recursively cover all branches
        /// </summary>
        /// <param name="addr">Starting address of script. Should start at a basic block</param>
        /// <param name="script"></param>
        /// <param name="coveredMap"></param>
        /// <returns>Whether it is possible to return without exception</returns>
        /// <exception cref="BadScriptException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public BranchType CoverInstruction(int addr, Stack<TryStack>? stack = null,
            int? continueFromBasicBlockEntranceAddr = null, int? jumpFromBasicBlockEntranceAddr = null)
        {
            if (continueFromBasicBlockEntranceAddr != null)
                basicBlockContinuation[(int)continueFromBasicBlockEntranceAddr] = addr;
            if (jumpFromBasicBlockEntranceAddr != null)
            {
                HashSet<int>? jumpTargets;
                if (!basicBlockJump.TryGetValue((int)jumpFromBasicBlockEntranceAddr, out jumpTargets))
                {
                    jumpTargets = new();
                    basicBlockJump[(int)jumpFromBasicBlockEntranceAddr] = jumpTargets;
                }
                jumpTargets.Add(addr);
            }
            int entranceAddr = addr;

            // Skip all OpCode.NOP at the beginning of a basic block
            int firstNotNopAddr = addr;
            Instruction entranceInstruction = script.GetInstruction(firstNotNopAddr);
            while (entranceInstruction.OpCode == OpCode.NOP)
            {
                firstNotNopAddr += entranceInstruction.Size;
                entranceInstruction = script.GetInstruction(addr);
            }

            if (stack == null)
            {
                stack = new();
                stack.Push(new(-1, -1, TryStackType.ENTRY, false));
            }
            else
                stack = CopyStack(stack);

            (int catchAddr, int finallyAddr, TryStackType stackType, bool continueAfterFinally) = stack.Peek();

            while (true)
            {
                // For the analysis of basic blocks,
                // we launched new recursion when exception is catched.
                // Here we have the exception not catched
                if (!coveredMap.TryGetValue(addr, out BranchType value))
                    throw new BadScriptException($"wrong address {addr}");
                if (value != BranchType.UNCOVERED)
                {
                    if (stackType != TryStackType.FINALLY)
                        // We have visited the code. Skip it.
                        return coveredMap[firstNotNopAddr] = value;
                    // if we are in finally, we may visit the codes after ENDFINALLY
                    // when previous codes did not throw
                    if (value != BranchType.OK)  // the codes in finally or the codes after ENDFINALLY will THROW or ABORT
                        return coveredMap[firstNotNopAddr] = value;
                    stack.Pop();  // end current finally
                    // No THROW or ABORT in try, catch or finally
                    // visit codes after ENDFINALLY
                    if (continueAfterFinally)
                    {
                        int endPointer = finallyAddr;
                        return coveredMap[firstNotNopAddr] = CoverInstruction(endPointer, stack, jumpFromBasicBlockEntranceAddr: firstNotNopAddr);
                    }
                    // FINALLY is OK, but throwed in previous TRY (without catch) or CATCH
                    return BranchType.THROW;  // Do not set coveredMap[entranceAddr] = BranchType.THROW;
                }
                Instruction instruction = script.GetInstruction(addr);
                if (jumpTargetToSources.ContainsKey(instruction) && addr != entranceAddr)
                    // on target of jump, start a new recursion to split basic blocks
                    return coveredMap[firstNotNopAddr] = CoverInstruction(addr, stack, continueFromBasicBlockEntranceAddr: addr);
                if (instruction.OpCode != OpCode.NOP)
                {
                    coveredMap[addr] = BranchType.OK;
                    // Add a basic block starting from entranceAddr
                    if (!basicBlocksInDict.TryGetValue(entranceAddr, out Dictionary<int, Instruction>? instructions))
                    {
                        instructions = new Dictionary<int, Instruction>();
                        basicBlocksInDict.Add(entranceAddr, instructions);
                    }
                    // Add this instruction to the basic block starting from entranceAddr
                    instructions.Add(addr, instruction);
                }

                // ABORT may also THROW in execution; this has been handled in HandleAbort
                if (instruction.OpCode == OpCode.ABORT || instruction.OpCode == OpCode.ABORTMSG)
                    return coveredMap[firstNotNopAddr] = HandleAbort(firstNotNopAddr, addr, stack);
                if (callWithJump.Contains(instruction.OpCode))
                {
                    BranchType returnedType;
                    if (instruction.OpCode == OpCode.CALLA)
                    {
                        returnedType = BranchType.ABORT;
                        foreach (int callaTarget in pushaTargets.Keys)
                        {
                            BranchType singleCallaResult = CoverInstruction(callaTarget, stack, jumpFromBasicBlockEntranceAddr: firstNotNopAddr);
                            if (singleCallaResult < returnedType)
                                returnedType = singleCallaResult;
                            // TODO: if a PUSHA cannot be covered, do not add it as a CALLA target
                        }
                    }
                    else
                    {
                        int callTarget = ComputeJumpTarget(addr, instruction);
                        returnedType = CoverInstruction(callTarget, stack, jumpFromBasicBlockEntranceAddr: firstNotNopAddr);
                    }
                    if (returnedType == BranchType.OK)
                        return coveredMap[firstNotNopAddr] = CoverInstruction(addr + instruction.Size, stack, continueFromBasicBlockEntranceAddr: firstNotNopAddr);
                    if (returnedType == BranchType.ABORT)
                        return coveredMap[firstNotNopAddr] = HandleAbort(firstNotNopAddr, addr, stack);
                    if (returnedType == BranchType.THROW)
                        return coveredMap[firstNotNopAddr] = HandleThrow(firstNotNopAddr, addr, stack);
                }
                if (instruction.OpCode == OpCode.RET)
                {
                    // See if we are in a try. There may still be runtime exceptions
                    // Do not judge with current stack.Peek(),
                    // because the try can hide deep in the stack.
                    // Just throw!
                    HandleThrow(firstNotNopAddr, addr, stack);
                    return BranchType.OK;  // No need to set coveredMap[entranceAddr] because it's OK when covered
                }
                if (tryThrowFinally.Contains(instruction.OpCode))
                {
                    if (instruction.OpCode == OpCode.TRY || instruction.OpCode == OpCode.TRY_L)
                    {
                        (int catchTarget, int finallyTarget) = ComputeTryTarget(addr, instruction);
                        stack.Push(new(catchTarget, finallyTarget, TryStackType.TRY, true));
                        return coveredMap[firstNotNopAddr] = CoverInstruction(addr + instruction.Size, stack, continueFromBasicBlockEntranceAddr: firstNotNopAddr);
                    }
                    if (instruction.OpCode == OpCode.THROW)
                        return coveredMap[firstNotNopAddr] = HandleThrow(firstNotNopAddr, addr, stack);
                    if (instruction.OpCode == OpCode.ENDTRY || instruction.OpCode == OpCode.ENDTRY_L)
                    {
                        if (stackType != TryStackType.TRY && stackType != TryStackType.CATCH)
                            throw new BadScriptException("No try stack on ENDTRY");

                        // Terminate the try/catch context, but
                        // visit catchAddr for current try, or finallyAddr for current catch
                        // because there may still be exceptions at runtime
                        HandleThrow(firstNotNopAddr, addr, stack);

                        stack.Pop();  // pop the ending TRY or CATCH
                        int endPointer = ComputeJumpTarget(addr, instruction);
                        if (finallyAddr != -1)
                        {
                            stack.Push(new(-1, endPointer, TryStackType.FINALLY, true));
                            addr = finallyAddr;
                        }
                        else
                            addr = endPointer;
                        return coveredMap[firstNotNopAddr] = CoverInstruction(addr, stack, jumpFromBasicBlockEntranceAddr: firstNotNopAddr);
                    }
                    if (instruction.OpCode == OpCode.ENDFINALLY)
                    {
                        int endPointer = finallyAddr;
                        if (stackType != TryStackType.FINALLY)
                            throw new BadScriptException("No finally stack on ENDFINALLY");
                        stack.Pop();  // pop the ending FINALLY
                        if (continueAfterFinally)
                            return coveredMap[firstNotNopAddr] = CoverInstruction(endPointer, stack, jumpFromBasicBlockEntranceAddr: firstNotNopAddr);
                        // For this basic block in finally, the branch type is OK
                        // The throw is caused by previous codes
                        return BranchType.OK;  // No need to set coveredMap[entranceAddr] because it's OK when covered
                    }
                }
                if (unconditionalJump.Contains(instruction.OpCode))
                    //addr = ComputeJumpTarget(addr, instruction);
                    //continue;
                    // For the analysis of basic blocks, we launch a new recursion
                    return coveredMap[firstNotNopAddr] = CoverInstruction(ComputeJumpTarget(addr, instruction), stack, jumpFromBasicBlockEntranceAddr: firstNotNopAddr);
                if (conditionalJump.Contains(instruction.OpCode) || conditionalJump_L.Contains(instruction.OpCode))
                {
                    BranchType noJump = CoverInstruction(addr + instruction.Size, stack, continueFromBasicBlockEntranceAddr: firstNotNopAddr);
                    BranchType jump = CoverInstruction(ComputeJumpTarget(addr, instruction), stack, jumpFromBasicBlockEntranceAddr: firstNotNopAddr);
                    if (noJump == BranchType.OK || jump == BranchType.OK)
                    {
                        // See if we are in a try. There may still be runtime exceptions
                        HandleThrow(firstNotNopAddr, addr, stack);
                        return BranchType.OK;  // No need to set coveredMap[entranceAddr] because it's OK when covered
                    }
                    if (noJump == BranchType.ABORT && jump == BranchType.ABORT)
                        return coveredMap[firstNotNopAddr] = HandleAbort(firstNotNopAddr, addr, stack);
                    if (noJump == BranchType.THROW || jump == BranchType.THROW)  // THROW, ABORT => THROW
                        return coveredMap[firstNotNopAddr] = HandleThrow(firstNotNopAddr, addr, stack);
                    throw new Exception($"Unknown {nameof(BranchType)} {noJump} {jump}");
                }

                addr += instruction.Size;
            }
        }
    }
}
