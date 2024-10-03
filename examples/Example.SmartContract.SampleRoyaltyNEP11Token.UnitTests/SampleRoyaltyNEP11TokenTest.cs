using EpicChain.SmartContract.Testing.TestingStandards;

namespace Example.SmartContract.SampleRoyaltyXEP11Token.UnitTests
{
    [TestClass]
    public class SampleRoyaltyXEP11TokenTest : TestBase<EpicChain.SmartContract.TestXEP11ampleRoyaltyXEP11Token>
    {
        [TestInitialize]
        public void TestSetup()
        {
            var (nef, manifest) = TestCleanup.EnsureArtifactsUpToDateInternal();
            TestBaseSetup(nef, manifest);
        }

        [TestMethod]
        public void Test()
        {

        }
    }
}
