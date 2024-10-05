using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.Cryptography.ECC;
using EpicChain.Network.P2P.Payloads;
using EpicChain.SmartContract.Testing;
using EpicChain.SmartContract.Testing.Extensions;
using EpicChain.VM.Types;
using System.Linq;

namespace EpicChain.SmartContract.Framework.UnitTests.Services
{
    [TestClass]
    public class NativeTest : DebugAndTestBase<Contract_Native>
    {
        [TestMethod]
        public void Test_EpicChain()
        {
            Assert.AreEqual(0, Contract.EpicChain_Decimals());
            Assert.AreEqual(5_0000_0000, Contract.EpicChain_GetEpicPulsePerBlock());
            Assert.IsNull(Contract.EpicChain_GetAccountState(Bob.Account));
            Assert.AreEqual(1_000_000_000, Contract.EpicChain_BalanceOf(Engine.ValidatorsAddress));
            Assert.AreEqual(0, Contract.EpicChain_UnclaimedEpicPulse(Bob.Account, 0));
            Assert.IsFalse(Contract.EpicChain_Transfer(Bob.Account, Bob.Account, 0));

            // Before RegisterCandidate
            Assert.AreEqual(0, Contract.EpicChain_GetCandidates()?.Count);
            // RegisterCandidate
            Engine.Fee = 1005_0000_0000;
            var pubKey = ECPoint.Parse("03b209fd4f53a7170ea4444e0cb0a6bb6a53c2bd016926989cf85f9b0fba17a70c", ECCurve.Secp256r1);
            Engine.SetTransactionSigners(WitnessScope.Global, pubKey);
            Assert.IsTrue(Contract.EpicChain_RegisterCandidate(pubKey));
            // After RegisterCandidate
            Assert.AreEqual(1, Contract.EpicChain_GetCandidates()?.Count);
            Assert.AreEqual(pubKey, ((Testing.Native.Models.Candidate)Contract.EpicChain_GetCandidates()?
                .Cast<StackItem>().First().ConvertTo(typeof(Testing.Native.Models.Candidate))!).PublicKey);
        }

        [TestMethod]
        public void Test_EPICPULSE()
        {
            Assert.AreEqual(8, Contract.EpicPulse_Decimals());
        }

        [TestMethod]
        public void Test_Policy()
        {
            Assert.AreEqual(1000L, Contract.Policy_GetFeePerByte());
            Assert.IsFalse(Contract.Policy_IsBlocked(Alice.Account));
        }
    }
}
