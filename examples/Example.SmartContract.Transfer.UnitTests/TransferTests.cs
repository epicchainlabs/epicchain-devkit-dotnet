using Chain.SmartContract.Testing;
using Chain.SmartContract.Testing.TestingStandards;

namespace Example.SmartContract.Transfer.UnitTests
{
    [TestClass]
    public class TransferTests : TestBase<SampleTransferContract>
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
