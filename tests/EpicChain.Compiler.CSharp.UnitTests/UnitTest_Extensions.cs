using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Extensions : DebugAndTestBase<Contract_Extensions>
    {
        [TestMethod]
        public void TestSum()
        {
            Assert.AreEqual(5, Contract.TestSum(3, 2));
            AssertEpicPulseConsumed(1065060);
        }
    }
}
