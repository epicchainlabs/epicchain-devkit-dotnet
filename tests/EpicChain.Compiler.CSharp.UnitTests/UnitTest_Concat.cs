using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Concat : DebugAndTestBase<Contract_Concat>
    {
        [TestMethod]
        public void TestStringAdd1()
        {
            Assert.AreEqual("ahello", Contract.TestStringAdd1("a"));
            AssertEpicPulseConsumed(1354680);
        }

        [TestMethod]
        public void TestStringAdd2()
        {
            Assert.AreEqual("abhello", Contract.TestStringAdd2("a", "b"));
            AssertEpicPulseConsumed(1662180);
        }

        [TestMethod]
        public void TestStringAdd3()
        {
            Assert.AreEqual("abchello", Contract.TestStringAdd3("a", "b", "c"));
            AssertEpicPulseConsumed(1969680);
        }

        [TestMethod]
        public void TestStringAdd4()
        {
            Assert.AreEqual("abcdhello", Contract.TestStringAdd4("a", "b", "c", "d"));
            AssertEpicPulseConsumed(2277180);
        }
    }
}
