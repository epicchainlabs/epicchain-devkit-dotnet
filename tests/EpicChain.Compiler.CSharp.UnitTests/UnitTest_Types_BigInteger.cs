using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;
using System.Numerics;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Types_BigInteger : DebugAndTestBase<Contract_Types_BigInteger>
    {
        [TestMethod]
        public void BigInteger_Test()
        {
            // Init

            Assert.AreEqual(BigInteger.Parse("100000000000000000000000000"), Contract.Attribute());
            AssertEpicPulseConsumed(984750);

            // static vars

            Assert.AreEqual(BigInteger.Zero, Contract.Zero());
            AssertEpicPulseConsumed(984720);
            Assert.AreEqual(BigInteger.One, Contract.One());
            AssertEpicPulseConsumed(984720);
            Assert.AreEqual(BigInteger.MinusOne, Contract.MinusOne());
            AssertEpicPulseConsumed(984720);

            // Parse

            Assert.AreEqual(456, Contract.Parse("456"));
            AssertEpicPulseConsumed(2032890);
            Assert.AreEqual(65, Contract.ConvertFromChar());
            AssertEpicPulseConsumed(984720);
        }
    }
}
