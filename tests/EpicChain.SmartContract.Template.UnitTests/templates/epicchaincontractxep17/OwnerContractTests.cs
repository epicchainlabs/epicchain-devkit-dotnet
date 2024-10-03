using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;
using EpicChain.SmartContract.Testing.Exceptions;
using EpicChain.SmartContract.Testing.TestingStandards;

namespace EpicChain.SmartContract.Template.UnitTests.templates.epicchaincontractxep17
{
    /// <summary>
    /// You need to build the solution to resolve Xep17Contract class.
    /// </summary>
    [TestClass]
    public class OwnerContractTests : OwnableTests<Xep17ContractTemplate>
    {
        /// <summary>
        /// Initialize Test
        /// </summary>
        public OwnerContractTests() : base(Xep17ContractTemplate.Nef, Xep17ContractTemplate.Manifest) { }

        [TestMethod]
        public override void TestSetGetOwner()
        {
            base.TestSetGetOwner();

            // Test throw if was stored an invalid owner
            // Technically not possible, but raise 100% coverage

            Contract.Storage.Put(new byte[] { 0xff }, 123);
            Assert.ThrowsException<TestException>(() => Contract.Owner);
        }
    }
}
