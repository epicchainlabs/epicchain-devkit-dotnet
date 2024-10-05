using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;
using System.Text;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_ConcatByteStringAddAssign : DebugAndTestBase<Contract_ConcatByteStringAddAssign>
    {
        [TestMethod]
        public void Test_ByteStringAdd()
        {
            Assert.AreEqual("abc", Encoding.ASCII.GetString(Contract.ByteStringAddAssign(Encoding.ASCII.GetBytes("a"), Encoding.ASCII.GetBytes("b"), "c")!));
            AssertEpicPulseConsumed(1970160);
        }
    }
}
