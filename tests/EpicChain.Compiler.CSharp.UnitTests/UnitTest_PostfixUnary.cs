using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_PostfixUnary : DebugAndTestBase<Contract_PostfixUnary>
    {
        [TestMethod]
        public void Test()
        {
            Assert.AreEqual("John", Contract.Test());
        }
    }
}
