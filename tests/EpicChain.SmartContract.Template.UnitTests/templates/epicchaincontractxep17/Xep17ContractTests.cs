using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.IO;
using EpicChain.SmartContract.Testing;
using EpicChain.SmartContract.Testing.Exceptions;
using EpicChain.SmartContract.Testing.InvalidTypes;
using System.Numerics;
using EpicChain.SmartContract.Testing.TestingStandards;

namespace EpicChain.SmartContract.Template.UnitTests.templates.epicchaincontractxep17
{
    /// <summary>
    /// You need to build the solution to resolve Xep17Contract class.
    /// </summary>
    [TestClass]
    public class Xep17ContractTests : XEP17Tests<Xep17ContractTemplate>
    {
        #region Expected values in base tests

        public override BigInteger ExpectedTotalSupply => 0;
        public override string ExpectedSymbol => "EXAMPLE";
        public override byte ExpectedDecimals => 8;

        #endregion

        /// <summary>
        /// Initialize Test
        /// </summary>
        public Xep17ContractTests() : base(Xep17ContractTemplate.Nef, Xep17ContractTemplate.Manifest) { }

        [TestMethod]
        public void TestMyMethod()
        {
            Assert.AreEqual("World", Contract.MyMethod());
        }

        [TestMethod]
        public override void TestTransfer()
        {
            Engine.SetTransactionSigners(Alice);

            // Test mint

            Assert.AreEqual(0, Contract.TotalSupply);

            // Alice is the owner

            Engine.SetTransactionSigners(Alice);

            Contract.Mint(Alice.Account, 10);

            Assert.AreEqual(10, Contract.BalanceOf(Alice.Account));
            Assert.AreEqual(10, Contract.TotalSupply);
            AssertTransferEvent(null, Alice.Account, 10);

            // Transfer is done between alice balance to bob

            base.TestTransfer();

            // Test Burn

            Engine.SetTransactionSigners(Alice);

            Contract.Burn(Alice.Account, Contract.BalanceOf(Alice.Account));
            Contract.Burn(Bob.Account, Contract.BalanceOf(Bob.Account));

            Assert.AreEqual(0, Contract.TotalSupply);
        }

        [TestMethod]
        public void TestMintAndBurn()
        {
            // Alice is the owner

            Engine.SetTransactionSigners(Alice);

            // Test mint -1

            Assert.ThrowsException<TestException>(() => Contract.Mint(Alice.Account, -1));

            // Test mint 0

            Contract.Mint(Alice.Account, 0);

            Assert.AreEqual(0, Contract.BalanceOf(Alice.Account));
            Assert.AreEqual(0, Contract.TotalSupply);
            AssertNoTransferEvent();

            // test mint

            Contract.Mint(Alice.Account, 10);

            Assert.AreEqual(10, Contract.BalanceOf(Alice.Account));
            Assert.AreEqual(10, Contract.TotalSupply);
            AssertTransferEvent(null, Alice.Account, 10);

            // Test burn -1

            Assert.ThrowsException<TestException>(() => Contract.Burn(Alice.Account, -1));

            // Test burn 0

            Contract.Burn(Alice.Account, 0);

            Assert.AreEqual(10, Contract.BalanceOf(Alice.Account));
            Assert.AreEqual(10, Contract.TotalSupply);
            AssertNoTransferEvent();

            // Test burn

            Contract.Burn(Alice.Account, 10);

            Assert.AreEqual(0, Contract.BalanceOf(Alice.Account));
            Assert.AreEqual(0, Contract.TotalSupply);
            AssertTransferEvent(Alice.Account, null, 10);

            // Can't burn more than the BalanceOf

            Assert.ThrowsException<TestException>(() => Contract.Burn(Alice.Account, 1));
            Assert.ThrowsException<TestException>(() => Contract.Burn(Bob.Account, 1));

            // Now check with Bob

            Engine.SetTransactionSigners(Bob);
            Assert.ThrowsException<TestException>(() => Contract.Mint(Alice.Account, 10));
            Assert.ThrowsException<TestException>(() => Contract.Burn(Alice.Account, 10));

            // Clean

            Assert.AreEqual(0, Contract.TotalSupply);
        }

        [TestMethod]
        public void TestUpdate()
        {
            // Alice is the deployer

            Engine.SetTransactionSigners(Bob);

            Assert.ThrowsException<TestException>(() => Contract.Update(NefFile.ToArray(), Manifest.ToJson().ToString()));

            Engine.SetTransactionSigners(Alice);

            // Test Update with the same script

            Contract.Update(NefFile.ToArray(), Manifest.ToJson().ToString());

            // Ensure that it works with the same script

            TestTotalSupply();
        }

        [TestMethod]
        public void TestDeployWithOwner()
        {
            // Alice is the deployer

            Engine.SetTransactionSigners(Bob);

            // Try with invalid owners

            Assert.ThrowsException<TestException>(() => Engine.Deploy<Xep17ContractTemplate>(NefFile, Manifest, UInt160.Zero));
            Assert.ThrowsException<TestException>(() => Engine.Deploy<Xep17ContractTemplate>(NefFile, Manifest, InvalidUInt160.InvalidLength));
            Assert.ThrowsException<TestException>(() => Engine.Deploy<Xep17ContractTemplate>(NefFile, Manifest, InvalidUInt160.InvalidType));

            // Test SetOwner notification

            UInt160? previousOwnerRaised = null;
            UInt160? newOwnerRaised = null;

            var expectedHash = Engine.GetDeployHash(NefFile, Manifest);
            var check = Engine.FromHash<Xep17ContractTemplate>(expectedHash, false);
            check.OnSetOwner += (previous, newOwner) =>
            {
                previousOwnerRaised = previous;
                newOwnerRaised = newOwner;
            };

            // Deploy with random owner, we can use the same storage
            // because the contract hash contains the Sender, and now it's random

            var rand = TestEngine.GetNewSigner().Account;
            var xep17 = Engine.Deploy<Xep17ContractTemplate>(NefFile, Manifest, rand);
            Assert.AreEqual(check.Hash, xep17.Hash);

            Coverage?.Join(xep17.GetCoverage());

            Assert.AreEqual(rand, xep17.Owner);
            Assert.IsNull(previousOwnerRaised);
            Assert.AreEqual(newOwnerRaised, xep17.Owner);
            Assert.AreEqual(newOwnerRaised, rand);
        }
    }
}
