using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;
using System.Numerics;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Inline : DebugAndTestBase<Contract_Inline>
    {
        [TestMethod]
        public void Test_Inline()
        {
            Assert.AreEqual(BigInteger.One, Contract.TestInline("inline"));
            AssertEpicPulseConsumed(1048650);
            Assert.AreEqual(new BigInteger(3), Contract.TestInline("inline_with_one_parameters"));
            AssertEpicPulseConsumed(1049970);
            Assert.AreEqual(new BigInteger(5), Contract.TestInline("inline_with_multi_parameters"));
            AssertEpicPulseConsumed(1051860);
        }

        [TestMethod]
        public void Test_NoInline()
        {
            Assert.AreEqual(BigInteger.One, Contract.TestInline("not_inline"));
            AssertEpicPulseConsumed(1067970);
            Assert.AreEqual(new BigInteger(3), Contract.TestInline("not_inline_with_one_parameters"));
            AssertEpicPulseConsumed(1071270);
            Assert.AreEqual(new BigInteger(5), Contract.TestInline("not_inline_with_multi_parameters"));
            AssertEpicPulseConsumed(1073220);
        }

        [TestMethod]
        public void Test_NestedInline()
        {
            Assert.AreEqual(new BigInteger(3), Contract.TestInline("inline_nested"));
            AssertEpicPulseConsumed(1071930);
        }
    }
}
