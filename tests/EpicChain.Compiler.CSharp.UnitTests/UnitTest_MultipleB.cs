using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_MultipleB : DebugAndTestBase<Contract_MultipleB>
    {
        [TestMethod]
        public void Test()
        {
            Assert.IsFalse(Contract.Test());
            AssertGasConsumed(984060);
        }
    }
}
