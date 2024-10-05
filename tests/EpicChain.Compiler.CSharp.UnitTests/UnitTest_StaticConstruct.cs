using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_StaticConstruct : DebugAndTestBase<Contract_StaticConstruct>
    {
        [TestMethod]
        public void Test_StaticConsturct()
        {
            var var1 = Contract.TestStatic();
            AssertEpicPulseConsumed(987270);
            // static byte[] callscript = ExecutionEngine.EntryScriptHash;
            // ...
            // return callscript

            Assert.IsNotNull(var1);
            Assert.AreEqual(4, var1);
        }
    }
}
