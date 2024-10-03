using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Attribute : DebugAndTestBase<Contract_AttributeChanged>
    {
        [TestMethod]
        public void AttributeTest()
        {
            Assert.AreEqual(Contract_AttributeChanged.Manifest.Name, "Contract_AttributeChanged");
            Assert.IsTrue(Contract.Test());
            AssertGasConsumed(984060);
        }
    }
}
