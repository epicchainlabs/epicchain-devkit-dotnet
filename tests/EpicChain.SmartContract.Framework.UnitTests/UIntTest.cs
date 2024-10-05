using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;

namespace EpicChain.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class UIntTest : DebugAndTestBase<Contract_UInt>
    {
        [TestMethod]
        public void TestStringAdd()
        {
            Assert.IsTrue(Contract.IsZeroUInt256(UInt256.Zero));
            AssertEpicPulseConsumed(1047510);
            Assert.IsFalse(Contract.IsValidAndNotZeroUInt256(UInt256.Zero));
            AssertEpicPulseConsumed(1065900);
            Assert.IsTrue(Contract.IsZeroUInt160(UInt160.Zero));
            AssertEpicPulseConsumed(1047510);
            Assert.IsFalse(Contract.IsValidAndNotZeroUInt160(UInt160.Zero));
            AssertEpicPulseConsumed(1065900);
            Assert.IsFalse(Contract.IsZeroUInt256(UInt256.Parse("0xa400ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff01")));
            AssertEpicPulseConsumed(1047510);
            Assert.IsTrue(Contract.IsValidAndNotZeroUInt256(UInt256.Parse("0xa400ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff01")));
            AssertEpicPulseConsumed(1065900);
            Assert.IsFalse(Contract.IsZeroUInt160(UInt160.Parse("01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4")));
            AssertEpicPulseConsumed(1047510);
            Assert.IsTrue(Contract.IsValidAndNotZeroUInt160(UInt160.Parse("01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4")));
            AssertEpicPulseConsumed(1065900);
            Assert.AreEqual("Nas9CRigvY94DyqA59HiBZNrgWHRsgrUgt", Contract.ToAddress(UInt160.Parse("01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4")));
            AssertEpicPulseConsumed(4592370);
        }
    }
}
