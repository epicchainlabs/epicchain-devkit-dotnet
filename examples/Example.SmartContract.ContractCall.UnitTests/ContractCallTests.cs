using Chain.SmartContract.Testing;
using Chain.SmartContract.Testing.TestingStandards;

namespace Example.SmartContract.ContractCall.UnitTests
{
    [TestClass]
    public class ContractCallTests : TestBase<SampleContractCall>
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
