using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Initializer : DebugAndTestBase<Contract_Initializer>
    {
        [TestMethod]
        public void Initializer_Test()
        {
            Assert.AreEqual(3, Contract.Sum());
            AssertEpicPulseConsumed(1596420);
            Assert.AreEqual(12, Contract.Sum1(5, 7));
            AssertEpicPulseConsumed(2149290);
            Assert.AreEqual(12, Contract.Sum2(5, 7));
            AssertEpicPulseConsumed(2149650);
        }
    }
}
