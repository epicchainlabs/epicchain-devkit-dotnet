using Chain.SmartContract.Testing.TestingStandards;

namespace Example.SmartContract.SampleRoyaltyNEP11Token.UnitTests
{
    [TestClass]
    public class SampleRoyaltyNEP11TokenTest : TestBase<Chain.SmartContract.Testing.SampleRoyaltyNEP11Token>
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
