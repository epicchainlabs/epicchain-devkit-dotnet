using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_GoTo : DebugAndTestBase<Contract_GoTo>
    {
        [TestMethod]
        public void Test()
        {
            Assert.AreEqual(3, Contract.Test());
            AssertGasConsumed(989700);
            Assert.AreEqual(3, Contract.TestTry());
            AssertGasConsumed(990180);
        }
    }
}
