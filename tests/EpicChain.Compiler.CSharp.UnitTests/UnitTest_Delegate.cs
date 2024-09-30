using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Delegate : DebugAndTestBase<Contract_Delegate>
    {
        [TestMethod]
        public void TestFunc()
        {
            Assert.AreEqual(5, Contract.SumFunc(2, 3));
            AssertGasConsumed(1065180);
        }

        [TestMethod]
        public void TestDelegate()
        {
            Contract.TestDelegate();
            AssertGasConsumed(3435960);
        }
    }
}
