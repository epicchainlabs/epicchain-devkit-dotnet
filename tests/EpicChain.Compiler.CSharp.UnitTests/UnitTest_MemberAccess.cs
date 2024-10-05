using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;
using System.Collections.Generic;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_MemberAccess : DebugAndTestBase<Contract_MemberAccess>
    {
        [TestMethod]
        public void Test_Main()
        {
            var logs = new Queue<string>();
            Contract.OnRuntimeLog += (sender, log) => logs.Enqueue(log);
            Contract.TestMain();
            AssertEpicPulseConsumed(6370920);

            // Check logs
            Assert.AreEqual(4, logs.Count);
            Assert.AreEqual("0", logs.Dequeue());
            Assert.AreEqual("msg", logs.Dequeue());
            Assert.AreEqual("hello", logs.Dequeue());
            Assert.AreEqual("", logs.Dequeue());
        }
    }
}
