using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;

namespace EpicChain.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class RegexTest : DebugAndTestBase<Contract_Regex>
    {
        [TestMethod]
        public void TestStartWith()
        {
            Assert.IsTrue(Contract.TestStartWith());
            AssertEpicPulseConsumed(1987890);
        }

        [TestMethod]
        public void TestIndexOf()
        {
            Assert.AreEqual(4, Contract.TestIndexOf());
            AssertEpicPulseConsumed(1986900);
        }

        [TestMethod]
        public void TestEndWith()
        {
            Assert.IsTrue(Contract.TestEndWith());
            AssertEpicPulseConsumed(1988760);
        }

        [TestMethod]
        public void TestContains()
        {
            Assert.IsTrue(Contract.TestContains());
            AssertEpicPulseConsumed(1987890);
        }

        [TestMethod]
        public void TestNumberOnly()
        {
            Assert.IsTrue(Contract.TestNumberOnly());
            AssertEpicPulseConsumed(1036470);
        }

        [TestMethod]
        public void TestAlphabetOnly()
        {
            Assert.IsTrue(Contract.TestAlphabetOnly());
            AssertEpicPulseConsumed(1204290);
            Assert.IsTrue(Contract.TestLowerAlphabetOnly());
            AssertEpicPulseConsumed(1111470);
            Assert.IsTrue(Contract.TestUpperAlphabetOnly());
            AssertEpicPulseConsumed(1095090);
        }
    }
}
