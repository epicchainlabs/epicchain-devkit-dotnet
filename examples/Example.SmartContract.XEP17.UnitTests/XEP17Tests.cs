using EpicChain.SmartContract.Testing;
using EpicChain.SmartContract.Testing.TestingStandards;

namespace Example.SmartContract.XEP17.UnitTests
{
    [TestClass]
    public class XEP17Tests : TestBase<SampleXep17Token>
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
            Assert.AreEqual(Contract.Symbol, "SampleXep17Token");
            Assert.AreEqual(Contract.Decimals, 8);
        }
    }
}
