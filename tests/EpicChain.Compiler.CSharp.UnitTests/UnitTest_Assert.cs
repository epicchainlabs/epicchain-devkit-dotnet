using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.Json;
using EpicChain.Optimizer;
using EpicChain.SmartContract.Testing;
using EpicChain.SmartContract.Testing.Exceptions;
using EpicChain.VM;
using System.Collections.Generic;
using System.Linq;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Assert : DebugAndTestBase<Contract_Assert>
    {
        private readonly JObject _debugInfo;

        public UnitTest_Assert()
        {
            var contract = TestCleanup.EnsureArtifactUpToDateInternal(nameof(Contract_Assert));
            _debugInfo = contract.CreateDebugInformation();
        }

        public void AssertsInFalse(TestException exception)
        {
            // All the ASSERT opcode addresses in method testAssertFalse
            List<int> assertAddresses = DumpNef.OpCodeAddressesInMethod(Contract_Assert.Nef, _debugInfo, "testAssertFalse", OpCode.ASSERT);
            Assert.AreEqual(exception.CurrentContext?.InstructionPointer, assertAddresses[1]);  // stops at the 2nd ASSERT
            Assert.AreEqual(exception.CurrentContext?.LocalVariables?[0].GetInteger(), 1);  // v==1
            Assert.AreEqual(exception.State, VMState.FAULT);
        }

        [TestMethod]
        public void Test_AssertFalse()
        {
            var exception = Assert.ThrowsException<TestException>(() => Contract.TestAssertFalse());
            AssertEpicPulseConsumed(1021470);
            AssertsInFalse(exception);
        }

        [TestMethod]
        public void Test_AssertInFunction()
        {
            var exception = Assert.ThrowsException<TestException>(() => Contract.TestAssertInFunction());
            AssertEpicPulseConsumed(1038900);
            AssertsInFalse(exception);
            Assert.AreEqual(exception.InvocationStack?.ToArray()?[1]?.LocalVariables?[0].GetInteger(), 0);  // v==0
        }

        [TestMethod]
        public void Test_AssertInTry()
        {
            var exception = Assert.ThrowsException<TestException>(() => Contract.TestAssertInTry());
            AssertEpicPulseConsumed(1039020);
            AssertsInFalse(exception);
            Assert.AreEqual(exception.InvocationStack?.ToArray()?[1]?.LocalVariables?[0].GetInteger(), 0);  // v==0
        }

        [TestMethod]
        public void Test_AssertInCatch()
        {
            var exception = Assert.ThrowsException<TestException>(() => Contract.TestAssertInCatch());
            AssertEpicPulseConsumed(1054770);
            AssertsInFalse(exception);
            Assert.AreEqual(exception.InvocationStack?.ToArray()?[1]?.LocalVariables?[0].GetInteger(), 1);  // v==1
        }

        [TestMethod]
        public void Test_AssertInFinally()
        {
            var exception = Assert.ThrowsException<TestException>(() => Contract.TestAssertInFinally());
            AssertEpicPulseConsumed(1039230);
            AssertsInFalse(exception);
            Assert.AreEqual(exception.InvocationStack?.ToArray()?[1]?.LocalVariables?[0].GetInteger(), 1);  // v==1
        }
    }
}
