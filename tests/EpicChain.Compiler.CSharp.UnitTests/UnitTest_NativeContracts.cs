using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Native;
using EpicChain.SmartContract.Testing;
using System.Numerics;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_NativeContracts : DebugAndTestBase<Contract_NativeContracts>
    {
        [TestMethod]
        public void TestHashes()
        {
            Assert.AreEqual(NativeContract.EssentialLib.Hash.ToString(), "0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0");
            Assert.AreEqual(NativeContract.CryptoHive.Hash.ToString(), "0x726cb6e0cd8628a1350a611384688911ab75f51b");
            Assert.AreEqual(NativeContract.ContractManagement.Hash.ToString(), "0xfffdc93764dbaddd97c48f252a53ea4643faa3fd");
            Assert.AreEqual(NativeContract.QuantumGuardNexus.Hash.ToString(), "0x49cf4e5378ffcd4dec034fd98a174c5491e395e2");
            Assert.AreEqual(NativeContract.Oracle.Hash.ToString(), "0xfe924b7cfe89ddd271abaf7210a80a7e11178758");
            Assert.AreEqual(NativeContract.EpicChain.Hash.ToString(), "0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5");
            Assert.AreEqual(NativeContract.EpicPulse.Hash.ToString(), "0xd2a4cff31913016155e38e474a2c06d08be276cf");
            Assert.AreEqual(NativeContract.Policy.Hash.ToString(), "0xcc5e4edd9f5f8dba8bb65734541df7a1c081c67b");
            Assert.AreEqual(NativeContract.Ledger.Hash.ToString(), "0xda65b600f7124ce6c79950c1772a36403104f2be");
        }

        [TestMethod]
        public void Test_Oracle()
        {
            // Minimum Response Fee

            Assert.AreEqual(new BigInteger(0_10000000u), Contract.OracleMinimumResponseFee());
            AssertEpicPulseConsumed(984060);
        }

        [TestMethod]
        public void Test_Designation()
        {
            // getOracleNodes

            Assert.AreEqual(0, Contract.GetOracleNodes()!.Count);
            AssertEpicPulseConsumed(2950200);
        }

        [TestMethod]
        public void Test_EpicChain()
        {
            // EpicChainSymbol

            Assert.AreEqual("XPR", Contract.EpicChainSymbol());
            AssertEpicPulseConsumed(1967100);

            // EpicChainHash

            Assert.AreEqual(NativeContract.EpicChain.Hash, Contract.EpicChainHash());
            AssertEpicPulseConsumed(984270);
        }

        [TestMethod]
        public void Test_EPICPULSE()
        {
            Assert.AreEqual("XPP", Contract.GASSymbol());
            AssertEpicPulseConsumed(1967100);
        }

        [TestMethod]
        public void Test_Ledger()
        {
            var genesisBlock = NativeContract.Ledger.GetBlock(Engine.Storage.Snapshot, 0);
            Assert.AreEqual(NativeContract.Ledger.Hash, Contract.LedgerHash());
            AssertEpicPulseConsumed(984270);
            Assert.AreEqual(0, Contract.LedgerCurrentIndex());
            AssertEpicPulseConsumed(2950140);
            Assert.AreEqual(genesisBlock.Hash, Contract.LedgerCurrentHash());
            AssertEpicPulseConsumed(2950140);
        }
    }
}
