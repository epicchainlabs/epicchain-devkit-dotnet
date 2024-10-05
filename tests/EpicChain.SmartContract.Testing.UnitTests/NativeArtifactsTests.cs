using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using EpicChain.Cryptography.ECC;
using EpicChain.SmartContract.Testing.Exceptions;
using System.Linq;
using System.Reflection;

namespace EpicChain.SmartContract.Testing.UnitTests
{
    [TestClass]
    public class NativeArtifactsTests
    {
        [TestMethod]
        public void TestInitialize()
        {
            // Create the engine without initialize the native contracts

            var engine = new TestEngine(false);

            Assert.AreEqual(0, engine.Storage.Store.Seek(System.Array.Empty<byte>(), Persistence.SeekDirection.Forward).Count());

            // Initialize native contracts

            engine.Native.Initialize(false);

            // Check symbols

            using var fee = engine.CreateEpicPulseWatcher();
            Assert.AreEqual("EpicChain", engine.Native.EpicChain.Symbol);
            Assert.AreEqual(984060L, fee.Value);

            using var epicpulse = engine.CreateEpicPulseWatcher();
            {
                Assert.AreEqual("EpicPulse", engine.Native.EpicPulse.Symbol);
                Assert.AreEqual(984060L, epicpulse);
            }

            // Ensure that the main address contains the totalSupply

            Assert.AreEqual(1_000_000_000, engine.Native.EpicChain.TotalSupply);
            Assert.AreEqual(engine.Native.EpicChain.TotalSupply, engine.Native.EpicChain.BalanceOf(engine.ValidatorsAddress));

            // Check coverage

            Assert.AreEqual(1M, engine.Native.EpicChain.GetCoverage(o => o.Symbol)!.CoveredLinesPercentage);
            Assert.AreEqual(1M, engine.Native.EpicChain.GetCoverage(o => o.TotalSupply)!.CoveredLinesPercentage);
            Assert.AreEqual(1M, engine.Native.EpicChain.GetCoverage(o => o.BalanceOf(It.IsAny<UInt160>()))!.CoveredLinesPercentage);
        }

        [TestMethod]
        public void TestCandidate()
        {
            var engine = new TestEngine(true) { Fee = 1001_0000_0000 };

            // Check initial value

            Assert.AreEqual(0, engine.Native.EpicChain.Candidates?.Length);

            // Register

            var candidate = ECPoint.Parse("03b209fd4f53a7170ea4444e0cb0a6bb6a53c2bd016926989cf85f9b0fba17a70c", ECCurve.Secp256r1);
            engine.SetTransactionSigners(Contract.CreateSignatureContract(candidate).ScriptHash);
            Assert.IsTrue(engine.Native.EpicChain.RegisterCandidate(candidate));

            // Check

            Assert.AreEqual(1, engine.Native.EpicChain.Candidates?.Length);
            Assert.AreEqual(candidate.ToString(), engine.Native.EpicChain.Candidates![0].PublicKey!.ToString());
        }

        [TestMethod]
        public void TestTransfer()
        {
            // Create and initialize TestEngine

            var engine = new TestEngine(true);

            // Fake signature of BFTAddress

            engine.SetTransactionSigners(Network.P2P.Payloads.WitnessScope.Global, engine.ValidatorsAddress);

            // Define address to transfer funds

            UInt160 addressTo = UInt160.Parse("0x1230000000000000000000000000000000000000");
            Assert.AreEqual(0, engine.Native.EpicChain.BalanceOf(addressTo));

            // Attach to Transfer event

            var raisedEvent = false;
            engine.Native.EpicChain.OnTransfer += (from, to, amount) =>
                {
                    Assert.AreEqual(engine.Transaction.Sender, from);
                    Assert.AreEqual(addressTo, to);
                    Assert.AreEqual(123, amount);

                    // If the event is raised, the variable will be changed
                    raisedEvent = true;
                };

            // Transfer funds

            Assert.IsTrue(engine.Native.EpicChain.Transfer(engine.Transaction.Sender, addressTo, 123, null));

            // Ensure that we have balance and the event was raised

            Assert.IsTrue(raisedEvent);
            Assert.AreEqual(123, engine.Native.EpicChain.BalanceOf(addressTo));
        }

        [TestMethod]
        public void TestSignature()
        {
            // Create and initialize TestEngine

            var engine = new TestEngine(true);

            // Check initial value of getRegisterPrice

            Assert.AreEqual(100000000000, engine.Native.EpicChain.RegisterPrice);

            // Fake Committee Signature

            engine.SetTransactionSigners(new Network.P2P.Payloads.Signer()
            {
                Account = engine.CommitteeAddress,
                Scopes = Network.P2P.Payloads.WitnessScope.Global
            });

            // Change RegisterPrice to 123

            engine.Native.EpicChain.RegisterPrice = 123;

            Assert.AreEqual(123, engine.Native.EpicChain.RegisterPrice);

            // Now test it without this signature

            engine.SetTransactionSigners(TestEngine.GetNewSigner());

            var exception = Assert.ThrowsException<TestException>(() => engine.Native.EpicChain.RegisterPrice = 123);
            Assert.IsInstanceOfType<TargetInvocationException>(exception.InnerException);
        }
    }
}
