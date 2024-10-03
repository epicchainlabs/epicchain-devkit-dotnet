using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;
using System.Numerics;
using EpicChain.SmartContract.Testing.TestingStandards;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_XEP17 : XEP17Tests<Contract_XEP17>
    {
        #region Expected values in base tests

        public override BigInteger ExpectedTotalSupply => 0;
        public override string ExpectedSymbol => "TEST";
        public override byte ExpectedDecimals => 8;

        #endregion

        /// <summary>
        /// Initialize Test
        /// </summary>
        public UnitTest_XEP17() : base(Contract_XEP17.Nef, Contract_XEP17.Manifest)
        {
            _ = TestCleanup.TestInitialize(typeof(Contract_XEP17));
        }

        [TestMethod]
        public override void TestTransfer()
        {
            // Contract has no mint
        }
    }
}
