using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;
using EpicChain.SmartContract.Testing.Exceptions;
using EpicChain.VM.Types;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Enum : DebugAndTestBase<Contract_Enum>
    {
        [TestMethod]
        public void TestEnumParse()
        {
            Assert.AreEqual(new Integer(1), Contract.TestEnumParse("Value1"));
            AssertEpicPulseConsumed(1049490);
            Assert.AreEqual(new Integer(2), Contract.TestEnumParse("Value2"));
            AssertEpicPulseConsumed(1050810);
            Assert.AreEqual(new Integer(3), Contract.TestEnumParse("Value3"));
            AssertEpicPulseConsumed(1052130);
            Assert.ThrowsException<TestException>(() => Contract.TestEnumParse("InvalidValue"));
            AssertEpicPulseConsumed(1067580);
        }

        [TestMethod]
        public void TestEnumParseIgnoreCase()
        {
            Assert.AreEqual(new Integer(1), Contract.TestEnumParseIgnoreCase("value1", true));
            AssertEpicPulseConsumed(1688250);
            Assert.AreEqual(new Integer(2), Contract.TestEnumParseIgnoreCase("VALUE2", true));
            AssertEpicPulseConsumed(1686990);
            Assert.AreEqual(new Integer(3), Contract.TestEnumParseIgnoreCase("VaLuE3", true));
            AssertEpicPulseConsumed(1689570);
            Assert.ThrowsException<TestException>(() => Contract.TestEnumParseIgnoreCase("value1", false));
            AssertEpicPulseConsumed(1065270);
            Assert.ThrowsException<TestException>(() => Contract.TestEnumParseIgnoreCase("InvalidValue", true));
            AssertEpicPulseConsumed(2098560);
        }

        [TestMethod]
        public void TestEnumTryParse()
        {
            Assert.IsTrue(Contract.TestEnumTryParse("Value1"));
            AssertEpicPulseConsumed(1049730);
            Assert.IsTrue(Contract.TestEnumTryParse("Value2"));
            AssertEpicPulseConsumed(1051050);
            Assert.IsTrue(Contract.TestEnumTryParse("Value3"));
            AssertEpicPulseConsumed(1052370);
            Assert.IsFalse(Contract.TestEnumTryParse("InvalidValue"));
            AssertEpicPulseConsumed(1052370);
        }

        [TestMethod]
        public void TestEnumTryParseIgnoreCase()
        {
            Assert.IsTrue(Contract.TestEnumTryParseIgnoreCase("value1", true));
            AssertEpicPulseConsumed(1688610);
            Assert.IsTrue(Contract.TestEnumTryParseIgnoreCase("VALUE2", true));
            AssertEpicPulseConsumed(1687350);
            Assert.IsTrue(Contract.TestEnumTryParseIgnoreCase("VaLuE3", true));
            AssertEpicPulseConsumed(1689930);
            Assert.IsFalse(Contract.TestEnumTryParseIgnoreCase("value1", false));
            AssertEpicPulseConsumed(1050000);
            Assert.IsFalse(Contract.TestEnumTryParseIgnoreCase("InvalidValue", true));
            AssertEpicPulseConsumed(2083410);
        }

        [TestMethod]
        public void TestEnumIsDefined()
        {
            Assert.IsTrue(Contract.TestEnumIsDefined(1));
            AssertEpicPulseConsumed(1049010);
            Assert.IsTrue(Contract.TestEnumIsDefined(2));
            AssertEpicPulseConsumed(1050120);
            Assert.IsTrue(Contract.TestEnumIsDefined(3));
            AssertEpicPulseConsumed(1051230);
            Assert.IsFalse(Contract.TestEnumIsDefined(0));
            AssertEpicPulseConsumed(1051230);
            Assert.IsFalse(Contract.TestEnumIsDefined(4));
            AssertEpicPulseConsumed(1051230);
        }

        [TestMethod]
        public void TestEnumIsDefinedByName()
        {
            Assert.IsTrue(Contract.TestEnumIsDefinedByName("Value1"));
            AssertEpicPulseConsumed(1049430);
            Assert.IsTrue(Contract.TestEnumIsDefinedByName("Value2"));
            AssertEpicPulseConsumed(1050750);
            Assert.IsTrue(Contract.TestEnumIsDefinedByName("Value3"));
            AssertEpicPulseConsumed(1052070);
            Assert.IsFalse(Contract.TestEnumIsDefinedByName("value1"));
            AssertEpicPulseConsumed(1052070);
            Assert.IsFalse(Contract.TestEnumIsDefinedByName("InvalidValue"));
            AssertEpicPulseConsumed(1052070);
        }

        [TestMethod]
        public void TestEnumGetName()
        {
            Assert.AreEqual("Value1", Contract.TestEnumGetName(1));
            AssertEpicPulseConsumed(1048920);
            Assert.AreEqual("Value2", Contract.TestEnumGetName(2));
            AssertEpicPulseConsumed(1050030);
            Assert.AreEqual("Value3", Contract.TestEnumGetName(3));
            AssertEpicPulseConsumed(1051140);
            Assert.IsNull(Contract.TestEnumGetName(0));
            AssertEpicPulseConsumed(1050930);
            Assert.IsNull(Contract.TestEnumGetName(4));
            AssertEpicPulseConsumed(1050930);
        }

        [TestMethod]
        public void TestEnumGetNameWithType()
        {
            Assert.AreEqual("Value1", Contract.TestEnumGetNameWithType(1));
            AssertEpicPulseConsumed(1049220);
            Assert.AreEqual("Value2", Contract.TestEnumGetNameWithType(2));
            AssertEpicPulseConsumed(1050330);
            Assert.AreEqual("Value3", Contract.TestEnumGetNameWithType(3));
            AssertEpicPulseConsumed(1051440);
            Assert.IsNull(Contract.TestEnumGetNameWithType(0));
            AssertEpicPulseConsumed(1051230);
            Assert.IsNull(Contract.TestEnumGetNameWithType(4));
            AssertEpicPulseConsumed(1051230);
        }
    }
}
