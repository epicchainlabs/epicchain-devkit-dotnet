using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.Optimizer;
using EpicChain.SmartContract.Testing;

namespace EpicChain.Compiler.CSharp.UnitTests.Optimizer
{
    [TestClass]
    public class HasCallATests
    {
        [TestMethod]
        public void Test_HasCallA()
        {
            Assert.IsTrue(EntryPoint.HasCallA(Contract_Lambda.Nef));
            Assert.IsTrue(EntryPoint.HasCallA(Contract_Linq.Nef));
            Assert.IsTrue(EntryPoint.HasCallA(Contract_Delegate.Nef));
            Assert.IsFalse(EntryPoint.HasCallA(Contract_Polymorphism.Nef));
            Assert.IsFalse(EntryPoint.HasCallA(Contract_TryCatch.Nef));
        }
    }
}
