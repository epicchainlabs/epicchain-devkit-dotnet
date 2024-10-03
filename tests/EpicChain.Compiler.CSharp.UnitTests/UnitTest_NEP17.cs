using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;
using System.Numerics;
using EpicChain.SmartContract.Testing.TestingStandards;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_NEP17 : XEP17Tests<Contract_NEP17>
    {
        #region Expected values in base tests

        public override BigInteger ExpectedTotalSupply => 0;
        public override string ExpectedSymbol => "TEST";
        public override byte ExpectedDecimals => 8;

        #endregion

        /// <summary>
        /// Initialize Test
        /// </summary>
        public UnitTest_NEP17() : base(Contract_NEP17.Nef, Contract_NEP17.Manifest)
        {
            _ = TestCleanup.TestInitialize(typeof(Contract_NEP17));
        }

        [TestMethod]
        public override void TestTransfer()
        {
            // Contract has no mint
        }
    }
}
