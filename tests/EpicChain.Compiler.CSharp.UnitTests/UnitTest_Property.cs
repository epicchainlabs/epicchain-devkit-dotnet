using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Property : DebugAndTestBase<Contract_Property>
    {
        [TestMethod]
        public void TestABIOffsetWithoutOptimizer()
        {
            var property = Contract_Property.Manifest.Abi.Methods[0];
            Assert.AreEqual("symbol", property.Name);
        }
    }
}
