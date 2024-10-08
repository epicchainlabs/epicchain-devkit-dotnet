using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.IO;
using EpicChain.SmartContract.Testing;
using EpicChain.SmartContract.Testing.Exceptions;
using EpicChain.SmartContract.Testing.InvalidTypes;
using EpicChain.SmartContract.Testing.TestingStandards;

namespace EpicChain.SmartContract.Template.UnitTests.templates.epicchaincontractowner
{
    /// <summary>
    /// You need to build the solution to resolve Ownable class.
    /// </summary>
    [TestClass]
    public class OwnableContractTests : OwnableTests<OwnableTemplate>
    {
        /// <summary>
        /// Initialize Test
        /// </summary>
        public OwnableContractTests() : base(OwnableTemplate.Nef, OwnableTemplate.Manifest) { }

        [TestMethod]
        public override void TestSetGetOwner()
        {
            base.TestSetGetOwner();

            // Test throw if was stored an invalid owner
            // Technically not possible, but raise 100% coverage

            Contract.Storage.Put(new byte[] { 0xff }, 123);
            Assert.ThrowsException<TestException>(() => Contract.Owner);
        }

        [TestMethod]
        public void TestMyMethod()
        {
            Assert.AreEqual("World", Contract.MyMethod());
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

            TestVerify();
        }

        [TestMethod]
        public void TestDeployWithOwner()
        {
            // Alice is the deployer

            Engine.SetTransactionSigners(Bob);

            // Try with invalid owners

            Assert.ThrowsException<TestException>(() => Engine.Deploy<OwnableTemplate>(NefFile, Manifest, UInt160.Zero));
            Assert.ThrowsException<TestException>(() => Engine.Deploy<OwnableTemplate>(NefFile, Manifest, InvalidUInt160.InvalidLength));
            Assert.ThrowsException<TestException>(() => Engine.Deploy<OwnableTemplate>(NefFile, Manifest, InvalidUInt160.InvalidType));

            // Test SetOwner notification

            UInt160? previousOwnerRaised = null;
            UInt160? newOwnerRaised = null;

            var expectedHash = Engine.GetDeployHash(NefFile, Manifest);
            var check = Engine.FromHash<OwnableTemplate>(expectedHash, false);
            check.OnSetOwner += (previous, newOwner) =>
            {
                previousOwnerRaised = previous;
                newOwnerRaised = newOwner;
            };

            // Deploy with random owner, we can use the same storage
            // because the contract hash contains the Sender, and now it's random

            var rand = TestEngine.GetNewSigner().Account;
            var xep17 = Engine.Deploy<OwnableTemplate>(NefFile, Manifest, rand);
            Assert.AreEqual(check.Hash, xep17.Hash);

            Coverage?.Join(xep17.GetCoverage());

            Assert.AreEqual(rand, xep17.Owner);
            Assert.IsNull(previousOwnerRaised);
            Assert.AreEqual(newOwnerRaised, xep17.Owner);
            Assert.AreEqual(newOwnerRaised, rand);
        }

        [TestMethod]
        public void TestDestroy()
        {
            // Try without being owner

            Engine.SetTransactionSigners(Bob);
            Assert.ThrowsException<TestException>(Contract.Destroy);

            // Try with the owner

            var checkpoint = Engine.Checkpoint();

            Engine.SetTransactionSigners(Alice);
            Contract.Destroy();
            Assert.IsNull(Engine.Native.ContractManagement.GetContract(Contract));

            Engine.Restore(checkpoint);
        }
    }
}
