using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;
using EpicChain.VM.Types;
using System.Numerics;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Property_Method : DebugAndTestBase<Contract_PropertyMethod>
    {
        [TestMethod]
        public void TestPropertyMethod()
        {
            var arr = Contract.TestProperty()!;
            AssertEpicPulseConsumed(2053500);

            Assert.AreEqual(2, arr.Count);
            Assert.AreEqual((arr[0] as StackItem)!.GetString(), "EpicChain");
            Assert.AreEqual(arr[1], new BigInteger(10));
        }

        [TestMethod]
        public void TestPropertyMethod2()
        {
            Contract.TestProperty2();
            AssertEpicPulseConsumed(1557360);
            // No errors
        }
    }
}
