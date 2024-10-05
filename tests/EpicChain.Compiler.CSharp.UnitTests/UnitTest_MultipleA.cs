using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_MultipleA : DebugAndTestBase<Contract_MultipleA>
    {
        [TestMethod]
        public void Test()
        {
            Assert.IsTrue(Contract.Test());
            AssertEpicPulseConsumed(984060);
        }
    }
}
