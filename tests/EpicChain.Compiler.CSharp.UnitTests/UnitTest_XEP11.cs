using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_XEP11 : DebugAndTestBase<Contract_XEP11>
    {
        [TestMethod]
        public void UnitTest_Symbol()
        {
            Assert.AreEqual("TEST", Contract.Symbol);
        }

        [TestMethod]
        public void UnitTest_Decimals()
        {
            Assert.AreEqual(0, Contract.Decimals);
        }
    }
}
