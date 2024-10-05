using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;
using System.Numerics;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Invoke : DebugAndTestBase<Contract_InvokeCsNef>
    {
        [TestMethod]
        public void Test_Return_Integer()
        {
            Assert.AreEqual(new BigInteger(42), Contract.ReturnInteger());
            AssertEpicPulseConsumed(984060);
        }

        [TestMethod]
        public void Test_Return_String()
        {
            Assert.AreEqual("hello world", Contract.ReturnString());
            AssertEpicPulseConsumed(984270);
        }

        [TestMethod]
        public void Test_Main()
        {
            Assert.AreEqual(new BigInteger(22), Contract.TestMain());
            AssertEpicPulseConsumed(984060);
        }
    }
}
