using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;
using EpicChain.SmartContract.Testing.Exceptions;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Throw : DebugAndTestBase<Contract_Throw>
    {
        [TestMethod]
        public void Test_Throw()
        {
            var exception = Assert.ThrowsException<TestException>(() => Contract.TestMain([]));
            AssertEpicPulseConsumed(1063530);
            Assert.IsTrue(exception.Message.Contains("Please supply at least one argument."));
        }

        [TestMethod]
        public void Test_NotThrow()
        {
            Contract.TestMain(["test"]);
            AssertEpicPulseConsumed(1111290);
        }
    }
}
