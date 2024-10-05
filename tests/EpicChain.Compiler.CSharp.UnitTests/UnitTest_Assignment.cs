using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;
using EpicChain.SmartContract.Testing.Exceptions;
using System;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Assignment : DebugAndTestBase<Contract_Assignment>
    {
        [TestMethod]
        public void Test_Assignment()
        {
            Contract.TestAssignment();
            AssertEpicPulseConsumed(989490);
        }

        [TestMethod]
        public void Test_CoalesceAssignment()
        {
            Contract.TestCoalesceAssignment();
            AssertEpicPulseConsumed(988950);
        }
    }
}
