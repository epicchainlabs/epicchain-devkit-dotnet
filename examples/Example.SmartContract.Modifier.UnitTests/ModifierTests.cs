using ntract.Testing;
using ntract.Testing.TestingStandards;

namespace Example.SmartContract.Modifier.UnitTests
{
    [TestClass]
    public class ModifierTests : TestBase<SampleModifier>
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
