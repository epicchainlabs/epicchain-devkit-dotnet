using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.IO;
using EpicChain.SmartContract.Testing;
using EpicChain.SmartContract.Testing.Exceptions;
using EpicChain.SmartContract.Testing.InvalidTypes;
using EpicChain.Wallets;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_UIntTypes : DebugAndTestBase<Contract_UIntTypes>
    {
        [TestMethod]
        public void UInt160_ValidateAddress()
        {
            var address = "NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB".ToScriptHash(ProtocolSettings.Default.AddressVersion);

            // True

            Assert.IsTrue(Contract.ValidateAddress(address));
            AssertEpicPulseConsumed(1049340);

            // False

            Assert.IsFalse(Contract.ValidateAddress(InvalidUInt160.InvalidType));
            AssertEpicPulseConsumed(1048710);
            Assert.ThrowsException<TestException>(() => Contract.ValidateAddress(InvalidUInt160.Null));
            AssertEpicPulseConsumed(1048110);
            Assert.IsFalse(Contract.ValidateAddress(InvalidUInt160.InvalidType));
            AssertEpicPulseConsumed(1048710);
            Assert.IsFalse(Contract.ValidateAddress(InvalidUInt160.InvalidLength));
            AssertEpicPulseConsumed(1048920);
        }

        [TestMethod]
        public void UInt160_equals_test()
        {
            var owner = "NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB".ToScriptHash(ProtocolSettings.Default.AddressVersion);
            var notOwner = "NYjzhdekseMYWvYpSoAeypqMiwMuEUDhKB".ToScriptHash(ProtocolSettings.Default.AddressVersion);

            Assert.IsTrue(Contract.CheckOwner(owner));
            AssertEpicPulseConsumed(1049040);
            Assert.IsFalse(Contract.CheckOwner(notOwner));
            AssertEpicPulseConsumed(1049040);
        }

        [TestMethod]
        public void UInt160_equals_zero_test()
        {
            var zero = UInt160.Zero;
            var notZero = "NYjzhdekseMYWvYpSoAeypqMiwMuEUDhKB".ToScriptHash(ProtocolSettings.Default.AddressVersion);

            Assert.IsTrue(Contract.CheckZeroStatic(zero));
            AssertEpicPulseConsumed(1049220);
            Assert.IsFalse(Contract.CheckZeroStatic(notZero));
            AssertEpicPulseConsumed(1049220);
        }

        [TestMethod]
        public void UInt160_byte_array_construct()
        {
            var notZero = "NYjzhdekseMYWvYpSoAeypqMiwMuEUDhKB".ToScriptHash(ProtocolSettings.Default.AddressVersion);

            Assert.AreEqual(notZero, Contract.ConstructUInt160(notZero.ToArray()));
            AssertEpicPulseConsumed(1294230);
        }
    }
}
