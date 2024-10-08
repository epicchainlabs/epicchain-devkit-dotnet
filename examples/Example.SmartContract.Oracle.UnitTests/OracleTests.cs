using EpicChain.SmartContract.Testing;
using EpicChain.SmartContract.Testing.TestingStandards;

namespace Example.SmartContract.Oracle.UnitTests
{
    [TestClass]
    public class OracleTests : TestBase<SampleOracle>
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
