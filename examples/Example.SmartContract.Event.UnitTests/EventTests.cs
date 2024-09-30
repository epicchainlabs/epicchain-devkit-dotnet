using Chain.SmartContract.Testing;
using Chain.SmartContract.Testing.TestingStandards;

namespace Example.SmartContract.Event.UnitTests
{
    [TestClass]
    public class EventTests : TestBase<SampleEvent>
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
            Assert.IsFalse(Contract.Main());
        }
    }
}
