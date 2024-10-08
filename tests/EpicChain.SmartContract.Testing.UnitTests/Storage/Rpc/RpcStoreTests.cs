using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using EpicChain.Cryptography.ECC;
using EpicChain.SmartContract.Testing.Storage;
using EpicChain.SmartContract.Testing.Storage.Rpc;
using System.Numerics;

namespace EpicChain.SmartContract.Testing.UnitTests.Storage
{
    [TestClass]
    public class RpcStoreTests
    {
        public abstract class DummyContract : SmartContract
        {
            public abstract BigInteger? GetCandidateVote(ECPoint? point);
            protected DummyContract(SmartContractInitialize initialize) : base(initialize) { }
        }

        // TODO:
        //      This test fail because the endPoint has a different manifest than the artifacts
        //      because we include a new method in the next fork, so it call the wrong method.
        //[TestMethod]
        public void TestRpcStore()
        {
            var engine = new TestEngine(new EngineStorage(new RpcStore("http://seed2t5.EpicChain.org:20332")), false);

            // check network values

            Assert.AreEqual(1_000_000_000, engine.Native.EpicChain.TotalSupply);
            Assert.IsTrue(engine.Native.Ledger.CurrentIndex > 3_510_270);

            // check with Seek (RPC doesn't support Backward, it could be slow)

            Assert.IsTrue(engine.Native.EpicChain.EpicPulsePerBlock == 5_0000_0000);

            // check deploy

            var node = ECPoint.Parse("03009b7540e10f2562e5fd8fac9eaec25166a58b26e412348ff5a86927bfac22a2", ECCurve.Secp256r1);
            var state = engine.Native.ContractManagement.GetContract(engine.Native.EpicChain.Hash);
            var contract = engine.Deploy<DummyContract>(state!.Nef, state.Manifest, null,
                c => c.Setup(s => s.GetCandidateVote(It.IsAny<ECPoint>())).Returns(() => engine.Native.EpicChain.GetCandidateVote(node)));

            var votes = contract.GetCandidateVote(node);
            Assert.IsTrue(votes > 3_000_000, $"Votes: {votes}");
        }
    }
}
