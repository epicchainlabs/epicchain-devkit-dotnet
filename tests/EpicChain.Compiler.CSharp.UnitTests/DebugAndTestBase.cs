using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.Json;
using EpicChain.Optimizer;
using EpicChain.SmartContract;
using EpicChain.SmartContract.Manifest;
using EpicChain.SmartContract.Testing;
using EpicChain.SmartContract.Testing.TestingStandards;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EpicChain.Compiler.CSharp.UnitTests;

public class DebugAndTestBase<T> : TestBase<T>
    where T : SmartContract.Testing.SmartContract, IContractInfo
{
    internal bool TestEpicPulseConsume { set; get; } = true;

    static DebugAndTestBase()
    {
        var context = TestCleanup.TestInitialize(typeof(T));
        TestSingleContractBasicBlockStartEnd(context!);
    }

    public static void TestSingleContractBasicBlockStartEnd(CompilationContext result)
    {
        TestSingleContractBasicBlockStartEnd(result.CreateExecutable(), result.CreateManifest(), result.CreateDebugInformation());
    }

    public static void TestSingleContractBasicBlockStartEnd(NefFile nef, ContractManifest manifest, JObject? debugInfo)
    {
        // Make sure the contract is optimized with RemoveUncoveredInstructions
        // Basic block analysis does not consider jump targets that are not covered
        (nef, manifest, debugInfo) = Reachability.RemoveUncoveredInstructions(nef, manifest, debugInfo);
        var basicBlocks = new ContractInBasicBlocks(nef, manifest, debugInfo);

        List<VM.Instruction> instructions = basicBlocks.coverage.addressAndInstructions.Select(kv => kv.i).ToList();
        Dictionary<VM.Instruction, HashSet<VM.Instruction>> jumpTargets = basicBlocks.coverage.jumpTargetToSources;

        Dictionary<VM.Instruction, VM.Instruction> nextAddrTable = new();
        VM.Instruction? prev = null;
        foreach (VM.Instruction i in instructions)
        {
            if (prev != null)
                nextAddrTable[prev] = i;
            prev = i;
        }

        foreach (BasicBlock basicBlock in basicBlocks.sortedBasicBlocks)
        {
            // Basic block ends with allowed OpCodes only, or the next instruction is a jump target
            Assert.IsTrue(OpCodeTypes.allowedBasicBlockEnds.Contains(basicBlock.instructions.Last().OpCode) || jumpTargets.ContainsKey(nextAddrTable[basicBlock.instructions.Last()]));
            // Instructions except the first are not jump targets
            foreach (VM.Instruction i in basicBlock.instructions.Skip(1))
                Assert.IsFalse(jumpTargets.ContainsKey(i));
            // Other instructions in the basic block are not those in allowedBasicBlockEnds
            foreach (VM.Instruction i in basicBlock.instructions.Take(basicBlock.instructions.Count - 1))
                Assert.IsFalse(OpCodeTypes.allowedBasicBlockEnds.Contains(i.OpCode));
        }

        // Each jump target starts a new basic block
        foreach (VM.Instruction target in jumpTargets.Keys)
            Assert.IsTrue(basicBlocks.basicBlocksByStartInstruction.ContainsKey(target));

        // Each instruction is included in only 1 basic block
        HashSet<VM.Instruction> includedInstructions = new();
        foreach (BasicBlock basicBlock in basicBlocks.sortedBasicBlocks)
            foreach (VM.Instruction instruction in basicBlock.instructions)
            {
                Assert.IsFalse(includedInstructions.Contains(instruction));
                includedInstructions.Add(instruction);
            }
    }

    protected void AssertEpicPulseConsumed(long epicpulseConsumed)
    {
        if (TestEpicPulseConsume)
            Assert.AreEqual(epicpulseConsumed, Engine.FeeConsumed.Value);
    }
}
