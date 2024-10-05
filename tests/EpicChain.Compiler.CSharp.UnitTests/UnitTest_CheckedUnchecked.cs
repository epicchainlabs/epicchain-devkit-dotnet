using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;
using EpicChain.SmartContract.Testing.Exceptions;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_CheckedUnchecked : DebugAndTestBase<Contract_CheckedUnchecked>
    {
        [TestMethod]
        public void TestAddChecked()
        {
            Assert.ThrowsException<TestException>(() => Contract.AddChecked(int.MaxValue, 1));
            AssertEpicPulseConsumed(1063020);
        }

        [TestMethod]
        public void TestAddUnchecked()
        {
            Assert.AreEqual(int.MinValue, Contract.AddUnchecked(int.MaxValue, 1));
            AssertEpicPulseConsumed(1048350);
        }

        [TestMethod]
        public void TestCastChecked()
        {
            Assert.ThrowsException<TestException>(() => Contract.CastChecked(-1));
            AssertEpicPulseConsumed(1062540);

            Assert.ThrowsException<TestException>(() => Contract.CastChecked(int.MinValue));
            AssertEpicPulseConsumed(1062540);

            Assert.AreEqual(2147483647, Contract.CastChecked(int.MaxValue));
            AssertEpicPulseConsumed(1047330);

            Assert.AreEqual(0, Contract.CastChecked(ulong.MinValue));
            AssertEpicPulseConsumed(1047330);

            Assert.ThrowsException<TestException>(() => Contract.CastChecked(ulong.MaxValue));
            AssertEpicPulseConsumed(1062780);

            Assert.ThrowsException<TestException>(() => Contract.CastChecked(long.MinValue));
            AssertEpicPulseConsumed(1062540);

            Assert.ThrowsException<TestException>(() => Contract.CastChecked(long.MaxValue));
            AssertEpicPulseConsumed(1062690);
        }

        [TestMethod]
        public void TestCastUnchecked()
        {
            Assert.AreEqual(uint.MaxValue, Contract.CastUnchecked(-1));
            AssertEpicPulseConsumed(1047510);


            Assert.AreEqual(2147483648, Contract.CastUnchecked(int.MinValue));
            AssertEpicPulseConsumed(1047510);

            Assert.AreEqual(2147483647, Contract.CastUnchecked(int.MaxValue));
            AssertEpicPulseConsumed(1047330);

            Assert.AreEqual(0, Contract.CastUnchecked(ulong.MinValue));
            AssertEpicPulseConsumed(1047330);

            Assert.AreEqual(4294967295, Contract.CastUnchecked(ulong.MaxValue));
            AssertEpicPulseConsumed(1047690);

            Assert.AreEqual(0, Contract.CastUnchecked(long.MinValue));
            AssertEpicPulseConsumed(1047510);

            Assert.AreEqual(4294967295, Contract.CastUnchecked(long.MaxValue));
            AssertEpicPulseConsumed(1047600);
        }
    }
}
