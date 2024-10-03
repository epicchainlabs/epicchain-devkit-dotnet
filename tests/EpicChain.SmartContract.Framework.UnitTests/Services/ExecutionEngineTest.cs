using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;
using EpicChain.SmartContract.Testing.Extensions;
using EpicChain.VM.Types;

namespace EpicChain.SmartContract.Framework.UnitTests.Services
{
    [TestClass]
    public class ExecutionEngineTest
         : DebugAndTestBase<Contract_ExecutionEngine>
    {
        [TestMethod]
        public void CallingScriptHashTest()
        {
            var hash = Contract.CallingScriptHash();
            Assert.AreEqual(Engine.Transaction.Script.Span.ToScriptHash().ToString(), new UInt160(hash!).ToString());
        }

        [TestMethod]
        public void EntryScriptHashTest()
        {
            var hash = Contract.EntryScriptHash();
            Assert.AreEqual(Engine.Transaction.Script.Span.ToScriptHash().ToString(), new UInt160(hash!).ToString());
        }

        [TestMethod]
        public void ExecutingScriptHashTest()
        {
            Assert.AreEqual(Contract.Hash.ToString(), new UInt160(Contract.ExecutingScriptHash()).ToString());
        }

        [TestMethod]
        public void ScriptContainerTest()
        {
            var item = Contract.ScriptContainer() as StackItem;
            var tx = item?.ConvertTo(typeof(Testing.Native.Models.Transaction)) as Testing.Native.Models.Transaction;

            Assert.AreEqual(Engine.Transaction.Hash.ToString(), tx?.Hash?.ToString());
        }

        [TestMethod]
        public void TransactionTest()
        {
            var item = Contract.Transaction() as StackItem;
            var tx = item?.ConvertTo(typeof(Testing.Native.Models.Transaction)) as Testing.Native.Models.Transaction;

            Assert.AreEqual(Engine.Transaction.Hash.ToString(), tx?.Hash?.ToString());
        }
    }
}
