using ntract.Testing;
using ntract.Testing.TestingStandards;

namespace Example.SmartContract.ZKP.UnitTests
{
    [TestClass]
    public class ZKPTests : TestBase<SampleZKP>
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
