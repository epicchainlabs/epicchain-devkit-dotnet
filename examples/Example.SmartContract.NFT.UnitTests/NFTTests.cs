using Microsoft.VisualStudio.TestTools.UnitTesting;
using ntract;
using ntract.Testing;
using ntract.Testing.TestingStandards;

namespace Example.SmartContract.NFT.UnitTests
{
    [TestClass]
    public class NFTTests : TestBase<SampleLootNFT>
    {

        [TestInitialize]
        public void TestSetup()
        {
            var (nef, manifest) = TestCleanup.EnsureArtifactsUpToDateInternal();
            TestBaseSetup(nef, manifest);
        }

        [TestMethod]
        public void TestClaim()
        {
            Contract.Claim(7772);
        }
    }
}
