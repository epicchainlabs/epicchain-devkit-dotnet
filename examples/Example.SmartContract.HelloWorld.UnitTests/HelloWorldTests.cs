using EpicChain.SmartContract.Testing;
using EpicChain.SmartContract.Testing.TestingStandards;

namespace Example.SmartContract.HelloWorld.UnitTests
{
    [TestClass]
    public class HelloWorldTests : TestBase<SampleHelloWorld>
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
            Assert.AreEqual(Contract.SayHello, "Hello, World!");
        }
    }
}
