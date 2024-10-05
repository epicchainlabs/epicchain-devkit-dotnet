using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;

namespace EpicChain.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class StringTest : DebugAndTestBase<Contract_String>
    {
        [TestMethod]
        public void TestStringAdd()
        {
            // ab => 3
            Assert.AreEqual(3, Contract.TestStringAdd("a", "b"));
            AssertEpicPulseConsumed(1357590);

            // hello => 4
            Assert.AreEqual(4, Contract.TestStringAdd("he", "llo"));
            AssertEpicPulseConsumed(1356420);

            // world => 5
            Assert.AreEqual(5, Contract.TestStringAdd("wo", "rld"));
            AssertEpicPulseConsumed(1357680);
        }

        [TestMethod]
        public void TestStringAddInt()
        {
            Assert.AreEqual("EpicChain", Contract.TestStringAddInt("EpicChain", 3));
            AssertEpicPulseConsumed(2460480);
        }
    }
}
