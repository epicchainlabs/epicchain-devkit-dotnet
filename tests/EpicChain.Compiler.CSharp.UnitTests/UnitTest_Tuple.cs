using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;
using EpicChain.VM.Types;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Tuple : DebugAndTestBase<Contract_Tuple>
    {
        [TestMethod]
        public void Test_Assign()
        {
            var tuple = Contract.T1()! as Struct;
            AssertEpicPulseConsumed(4789620);
            Assert.AreEqual(5, tuple!.Count);
            Assert.AreEqual(1, tuple[2].GetInteger());
            Assert.AreEqual(4, tuple[3].GetInteger());
            Assert.AreEqual(2, ((Struct)tuple[4])[1].GetInteger());
        }
    }
}
