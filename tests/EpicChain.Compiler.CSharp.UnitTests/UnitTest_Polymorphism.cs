using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Polymorphism : DebugAndTestBase<Contract_Polymorphism>
    {
        [TestMethod]
        public void Test()
        {
            Assert.AreEqual(14, Contract.Sum(5, 9));
            AssertEpicPulseConsumed(1514550);
            Assert.AreEqual(40, Contract.Mul(5, 8));
            AssertEpicPulseConsumed(1531890);
            Assert.AreEqual("test", Contract.Test());
            AssertEpicPulseConsumed(1487760);
            Assert.AreEqual("base.test", Contract.Test2());
            AssertEpicPulseConsumed(1812540);
        }
    }
}
