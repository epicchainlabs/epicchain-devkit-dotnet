using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_StaticVarInit : DebugAndTestBase<Contract_StaticVarInit>
    {
        [TestMethod]
        public void Test_StaticVarInit()
        {
            var var1 = Contract.StaticInit();
            AssertEpicPulseConsumed(1000470);
            Assert.AreEqual(var1, Contract.Hash);

            var var2 = Contract.DirectGet();
            AssertEpicPulseConsumed(985530);
            Assert.AreEqual(var1, var2);
        }
    }
}
