using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_StaticClass : DebugAndTestBase<Contract_StaticClass>
    {
        [TestMethod]
        public void Test_StaticClass()
        {
            Assert.AreEqual(2, Contract.TestStaticClass());
            AssertEpicPulseConsumed(1055790);
        }
    }
}
