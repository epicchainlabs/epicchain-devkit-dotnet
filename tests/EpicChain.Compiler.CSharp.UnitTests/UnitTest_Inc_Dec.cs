using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;
using EpicChain.SmartContract.Testing.Exceptions;
using System.Numerics;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Inc_Dec : DebugAndTestBase<Contract_Inc_Dec>
    {
        [TestMethod]
        public void Test_Property_Inc_Checked()
        {
            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Property_Inc_Checked());
            AssertEpicPulseConsumed(1000440);
        }

        [TestMethod]
        public void Test_Property_Inc_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(uint.MaxValue + 2)), Contract.UnitTest_Property_Inc_UnChecked());
            AssertEpicPulseConsumed(986130);
        }

        [TestMethod]
        public void Test_Property_Dec_Checked()
        {
            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Property_Dec_Checked());
            AssertEpicPulseConsumed(1000290);
        }

        [TestMethod]
        public void Test_Property_Dec_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(uint.MinValue - 2)), Contract.UnitTest_Property_Dec_UnChecked());
            AssertEpicPulseConsumed(986040);
        }

        [TestMethod]
        public void Test_Local_Inc_Checked()
        {
            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Local_Inc_Checked());
            AssertEpicPulseConsumed(1002360);
        }

        [TestMethod]
        public void Test_Local_Inc_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(uint.MaxValue + 2)), Contract.UnitTest_Local_Inc_UnChecked());
            AssertEpicPulseConsumed(988050);
        }

        [TestMethod]
        public void Test_Local_Dec_Checked()
        {
            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Local_Dec_Checked());
            AssertEpicPulseConsumed(1002210);
        }

        [TestMethod]
        public void Test_Local_Dec_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(uint.MinValue - 2)), Contract.UnitTest_Local_Dec_UnChecked());
            AssertEpicPulseConsumed(987960);
        }

        [TestMethod]
        public void Test_Param_Inc_Checked()
        {
            Contract.UnitTest_Param_Inc_Checked(0);
            AssertEpicPulseConsumed(1048710);
        }

        [TestMethod]
        public void Test_Param_Inc_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(uint.MinValue + 2)), Contract.UnitTest_Param_Inc_UnChecked(0));
            AssertEpicPulseConsumed(1048710);
        }

        [TestMethod]
        public void Test_Param_Dec_Checked()
        {
            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Param_Dec_Checked(0));
            AssertEpicPulseConsumed(1063140);

            Contract.UnitTest_Param_Dec_Checked(uint.MaxValue);
            AssertEpicPulseConsumed(1048710);

            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Param_Dec_Checked(uint.MinValue));
            AssertEpicPulseConsumed(1063140);

            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Param_Dec_Checked(-1));
            AssertEpicPulseConsumed(1063140);

            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Param_Dec_Checked(1));
            AssertEpicPulseConsumed(1063740);
        }

        [TestMethod]
        public void Test_Param_Dec_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(uint.MinValue - 2)), Contract.UnitTest_Param_Dec_UnChecked(0));
            AssertEpicPulseConsumed(1048890);

            Contract.UnitTest_Param_Dec_UnChecked(uint.MaxValue);
            AssertEpicPulseConsumed(1048710);

            Contract.UnitTest_Param_Dec_UnChecked(uint.MinValue);
            AssertEpicPulseConsumed(1048890);

            Contract.UnitTest_Param_Dec_UnChecked(-1);
            AssertEpicPulseConsumed(1048890);

            Contract.UnitTest_Param_Dec_UnChecked(1);
            AssertEpicPulseConsumed(1048890);
        }

        // Test Methods for int type
        [TestMethod]
        public void Test_IntProperty_Inc_Checked()
        {
            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Property_Inc_Checked_Int());
            AssertEpicPulseConsumed(1000440);
        }

        [TestMethod]
        public void Test_IntProperty_Inc_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(int.MaxValue + 2)), Contract.UnitTest_Property_Inc_UnChecked_Int());
            AssertEpicPulseConsumed(986550);
        }

        [TestMethod]
        public void Test_IntProperty_Dec_Checked()
        {
            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Property_Dec_Checked_Int());
            AssertEpicPulseConsumed(1000290);
        }

        [TestMethod]
        public void Test_IntProperty_Dec_UnChecked()
        {
            Assert.AreEqual(new BigInteger(unchecked(int.MinValue - 2)), Contract.UnitTest_Property_Dec_UnChecked_Int());
            AssertEpicPulseConsumed(986190);
        }

        // Local Variable Tests for int
        [TestMethod]
        public void Test_Local_Inc_Checked_Int()
        {
            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Local_Inc_Checked_Int());
            AssertEpicPulseConsumed(1002360);
        }

        [TestMethod]
        public void Test_Local_Inc_UnChecked_Int()
        {
            Assert.AreEqual(new BigInteger(unchecked(int.MaxValue + 2)), Contract.UnitTest_Local_Inc_UnChecked_Int());
            AssertEpicPulseConsumed(988470);
        }

        [TestMethod]
        public void Test_Local_Dec_Checked_Int()
        {
            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Local_Dec_Checked_Int());
            AssertEpicPulseConsumed(1002210);
        }

        [TestMethod]
        public void Test_Local_Dec_UnChecked_Int()
        {
            Assert.AreEqual(new BigInteger(unchecked(int.MinValue - 2)), Contract.UnitTest_Local_Dec_UnChecked_Int());
            AssertEpicPulseConsumed(988110);
        }

        // Parameter Tests for int
        [TestMethod]
        public void Test_Param_Inc_Checked_Int()
        {
            Assert.AreEqual(new BigInteger(checked(0 + 2)), Contract.UnitTest_Param_Inc_Checked_Int(0));
            AssertEpicPulseConsumed(1048710);

            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Param_Inc_Checked_Int(int.MaxValue));
            AssertEpicPulseConsumed(1063290);

            Assert.AreEqual(new BigInteger(checked(int.MinValue + 2)), Contract.UnitTest_Param_Inc_Checked_Int(int.MinValue));
            AssertEpicPulseConsumed(1048710);

            Assert.AreEqual(new BigInteger(checked(-1 + 2)), Contract.UnitTest_Param_Inc_Checked_Int(-1));
            AssertEpicPulseConsumed(1048710);

            Assert.AreEqual(new BigInteger(checked(1 + 2)), Contract.UnitTest_Param_Inc_Checked_Int(1));
            AssertEpicPulseConsumed(1048710);
        }

        [TestMethod]
        public void Test_Param_Inc_UnChecked_Int()
        {
            Assert.AreEqual(new BigInteger(unchecked(0 + 2)), Contract.UnitTest_Param_Inc_UnChecked_Int(0));
            AssertEpicPulseConsumed(1048710);

            Assert.AreEqual(new BigInteger(unchecked(int.MaxValue + 2)), Contract.UnitTest_Param_Inc_UnChecked_Int(int.MaxValue));
            AssertEpicPulseConsumed(1049400);

            Assert.AreEqual(new BigInteger(unchecked(int.MinValue + 2)), Contract.UnitTest_Param_Inc_UnChecked_Int(int.MinValue));
            AssertEpicPulseConsumed(1048710);

            Assert.AreEqual(new BigInteger(unchecked(2 + 2)), Contract.UnitTest_Param_Inc_UnChecked_Int(2));
            AssertEpicPulseConsumed(1048710);

            Assert.AreEqual(new BigInteger(unchecked(-2 + 2)), Contract.UnitTest_Param_Inc_UnChecked_Int(-2));
            AssertEpicPulseConsumed(1048710);
        }

        [TestMethod]
        public void Test_Param_Dec_Checked_Int()
        {
            Contract.UnitTest_Param_Dec_Checked_Int(0);
            AssertEpicPulseConsumed(1048710);

            Contract.UnitTest_Param_Dec_Checked_Int(int.MaxValue);
            AssertEpicPulseConsumed(1048710);

            Assert.ThrowsException<TestException>(() => Contract.UnitTest_Param_Dec_Checked_Int(int.MinValue));
            AssertEpicPulseConsumed(1063140);

            Contract.UnitTest_Param_Dec_Checked_Int(1);
            AssertEpicPulseConsumed(1048710);

            Contract.UnitTest_Param_Dec_Checked_Int(-1);
            AssertEpicPulseConsumed(1048710);
        }

        [TestMethod]
        public void Test_Param_Dec_UnChecked_Int()
        {
            Assert.AreEqual(new BigInteger(unchecked(0 - 2)), Contract.UnitTest_Param_Dec_UnChecked_Int(0));
            AssertEpicPulseConsumed(1048710);

            Assert.AreEqual(new BigInteger(unchecked(int.MaxValue - 2)), Contract.UnitTest_Param_Dec_UnChecked_Int(int.MaxValue));
            AssertEpicPulseConsumed(1048710);

            Assert.AreEqual(new BigInteger(unchecked(int.MinValue - 2)), Contract.UnitTest_Param_Dec_UnChecked_Int(int.MinValue));
            AssertEpicPulseConsumed(1049040);

            Assert.AreEqual(new BigInteger(unchecked(2 - 2)), Contract.UnitTest_Param_Dec_UnChecked_Int(2));
            AssertEpicPulseConsumed(1048710);

            Assert.AreEqual(new BigInteger(unchecked(-2 - 2)), Contract.UnitTest_Param_Dec_UnChecked_Int(-2));
            AssertEpicPulseConsumed(1048710);
        }

        [TestMethod]
        public void Test_Not_DeadLoop()
        {
            Contract.UnitTest_Not_DeadLoop(); // No error
            AssertEpicPulseConsumed(993450);
        }
    }
}
