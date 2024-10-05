using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Partial : DebugAndTestBase<Contract_Partial>
    {
        [TestMethod]
        public void Test_Partial()
        {
            Assert.AreEqual(1, Contract.Test1());
            AssertEpicPulseConsumed(984060);
            Assert.AreEqual(2, Contract.Test2());
            AssertEpicPulseConsumed(984060);
        }
    }
}
