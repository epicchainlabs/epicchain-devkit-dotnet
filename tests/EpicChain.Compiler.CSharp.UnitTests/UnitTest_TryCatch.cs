using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;
using EpicChain.SmartContract.Testing.Exceptions;
using System.Numerics;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_TryCatch : DebugAndTestBase<Contract_TryCatch>
    {
        [TestMethod]
        public void Test_Try01_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.Try01(false, false, false));
            AssertEpicPulseConsumed(1049550);
            Assert.AreEqual(new BigInteger(3), Contract.Try01(false, false, true));
            AssertEpicPulseConsumed(1050210);
            Assert.AreEqual(new BigInteger(3), Contract.Try01(true, true, false));
            AssertEpicPulseConsumed(1065420);
            Assert.AreEqual(new BigInteger(4), Contract.Try01(true, true, true));
            AssertEpicPulseConsumed(1066080);
        }

        [TestMethod]
        public void Test_Try02_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.Try02(false, false, false));
            AssertEpicPulseConsumed(1067010);
            Assert.AreEqual(new BigInteger(3), Contract.Try02(false, false, true));
            AssertEpicPulseConsumed(1067670);
            Assert.AreEqual(new BigInteger(3), Contract.Try02(true, true, false));
            AssertEpicPulseConsumed(1082880);
            Assert.AreEqual(new BigInteger(4), Contract.Try02(true, true, true));
            AssertEpicPulseConsumed(1083540);
        }

        [TestMethod]
        public void Test_Try03_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.Try03(false, false, false));
            AssertEpicPulseConsumed(1049550);
            Assert.AreEqual(new BigInteger(3), Contract.Try03(false, false, true));
            AssertEpicPulseConsumed(1050210);
            Assert.AreEqual(new BigInteger(3), Contract.Try03(true, true, false));
            AssertEpicPulseConsumed(1080780);
            Assert.AreEqual(new BigInteger(4), Contract.Try03(true, true, true));
            AssertEpicPulseConsumed(1081440);
        }

        [TestMethod]
        public void Test_TryNest_AllPaths()
        {
            Assert.AreEqual(new BigInteger(3), Contract.TryNest(false, false, false, false));
            AssertEpicPulseConsumed(1050480);
            Assert.AreEqual(new BigInteger(4), Contract.TryNest(true, false, false, false));
            AssertEpicPulseConsumed(1081710);
            Assert.AreEqual(new BigInteger(3), Contract.TryNest(false, true, false, false));
            AssertEpicPulseConsumed(1050480);
            Assert.AreEqual(new BigInteger(3), Contract.TryNest(false, false, true, true));
            AssertEpicPulseConsumed(1081500);
            Assert.AreEqual(new BigInteger(4), Contract.TryNest(true, true, true, true));
            AssertEpicPulseConsumed(1143570);
        }

        [TestMethod]
        public void Test_ThrowInCatch_AllPaths()
        {
            Assert.AreEqual(new BigInteger(4), Contract.ThrowInCatch(false, false, true));
            AssertEpicPulseConsumed(1049730);
            Assert.AreEqual(new BigInteger(4), Contract.ThrowInCatch(true, false, true));
            AssertEpicPulseConsumed(1065600);
            Assert.ThrowsException<TestException>(() => Contract.ThrowInCatch(true, true, true));
            AssertEpicPulseConsumed(1080930);
        }

        [TestMethod]
        public void Test_TryFinally_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryFinally(false, false));
            AssertEpicPulseConsumed(1049520);
            Assert.AreEqual(new BigInteger(3), Contract.TryFinally(false, true));
            AssertEpicPulseConsumed(1050180);
            Assert.ThrowsException<TestException>(() => Contract.TryFinally(true, true));
            AssertEpicPulseConsumed(1065600);
        }

        [TestMethod]
        public void Test_TryFinallyAndRethrow_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryFinallyAndRethrow(false, false));
            AssertEpicPulseConsumed(1049520);
            Assert.AreEqual(new BigInteger(3), Contract.TryFinallyAndRethrow(false, true));
            AssertEpicPulseConsumed(1050180);
            Assert.ThrowsException<TestException>(() => Contract.TryFinallyAndRethrow(true, true));
            AssertEpicPulseConsumed(1080960);
        }

        [TestMethod]
        public void Test_TryCatch_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryCatch(false, false));
            AssertEpicPulseConsumed(1049280);
            Assert.AreEqual(new BigInteger(3), Contract.TryCatch(true, true));
            AssertEpicPulseConsumed(1081080);
        }

        [TestMethod]
        public void Test_TryWithTwoFinally_AllPaths()
        {
            Assert.AreEqual(new BigInteger(1), Contract.TryWithTwoFinally(false, false, false, false, false, false));
            AssertEpicPulseConsumed(1050810);
            Assert.AreEqual(new BigInteger(4), Contract.TryWithTwoFinally(false, false, false, false, true, false));
            AssertEpicPulseConsumed(1051500);
            Assert.AreEqual(new BigInteger(6), Contract.TryWithTwoFinally(false, false, false, false, false, true));
            AssertEpicPulseConsumed(1051500);
            Assert.AreEqual(new BigInteger(3), Contract.TryWithTwoFinally(true, false, true, false, false, false));
            AssertEpicPulseConsumed(1067280);
            Assert.AreEqual(new BigInteger(10), Contract.TryWithTwoFinally(false, true, false, true, false, true));
            AssertEpicPulseConsumed(1067970);
            Assert.AreEqual(new BigInteger(15), Contract.TryWithTwoFinally(true, true, true, true, true, true));
            AssertEpicPulseConsumed(1085130);
        }

        [TestMethod]
        public void Test_TryECPointCast_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryecpointCast(false, false, false));
            AssertEpicPulseConsumed(1050120);
            Assert.AreEqual(new BigInteger(3), Contract.TryecpointCast(false, false, true));
            AssertEpicPulseConsumed(1050780);
            Assert.AreEqual(new BigInteger(3), Contract.TryecpointCast(true, true, false));
            AssertEpicPulseConsumed(1065750);
            Assert.AreEqual(new BigInteger(4), Contract.TryecpointCast(true, true, true));
            AssertEpicPulseConsumed(1066410);
        }

        [TestMethod]
        public void Test_TryValidByteString2Ecpoint_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteString2Ecpoint(false, false));
            AssertEpicPulseConsumed(1049970);
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteString2Ecpoint(false, true));
            AssertEpicPulseConsumed(1050630);
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteString2Ecpoint(true, false));
            AssertEpicPulseConsumed(1049970);
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteString2Ecpoint(true, true));
            AssertEpicPulseConsumed(1050630);
        }

        [TestMethod]
        public void Test_TryInvalidByteArray2UInt160_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryinvalidByteArray2UInt160(false, false, false));
            AssertEpicPulseConsumed(1050120);
            Assert.AreEqual(new BigInteger(3), Contract.TryinvalidByteArray2UInt160(false, false, true));
            AssertEpicPulseConsumed(1050780);
            Assert.AreEqual(new BigInteger(3), Contract.TryinvalidByteArray2UInt160(true, true, false));
            AssertEpicPulseConsumed(1065750);
            Assert.AreEqual(new BigInteger(4), Contract.TryinvalidByteArray2UInt160(true, true, true));
            AssertEpicPulseConsumed(1066410);
        }

        [TestMethod]
        public void Test_TryValidByteArray2UInt160_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteArray2UInt160(false, false));
            AssertEpicPulseConsumed(1049970);
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteArray2UInt160(false, true));
            AssertEpicPulseConsumed(1050630);
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteArray2UInt160(true, false));
            AssertEpicPulseConsumed(1049970);
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteArray2UInt160(true, true));
            AssertEpicPulseConsumed(1050630);
        }

        [TestMethod]
        public void Test_TryInvalidByteArray2UInt256_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryinvalidByteArray2UInt256(false, false, false));
            AssertEpicPulseConsumed(1049670);
            Assert.AreEqual(new BigInteger(3), Contract.TryinvalidByteArray2UInt256(false, false, true));
            AssertEpicPulseConsumed(1050330);
            Assert.AreEqual(new BigInteger(3), Contract.TryinvalidByteArray2UInt256(true, true, false));
            AssertEpicPulseConsumed(1065690);
            Assert.AreEqual(new BigInteger(4), Contract.TryinvalidByteArray2UInt256(true, true, true));
            AssertEpicPulseConsumed(1066350);
        }

        [TestMethod]
        public void Test_TryValidByteArray2UInt256_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteArray2UInt256(false, false));
            AssertEpicPulseConsumed(1049520);
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteArray2UInt256(false, true));
            AssertEpicPulseConsumed(1050180);
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteArray2UInt256(true, false));
            AssertEpicPulseConsumed(1049520);
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteArray2UInt256(true, true));
            AssertEpicPulseConsumed(1050180);
        }

        [TestMethod]
        public void Test_TryNULL2Ecpoint_1_AllPaths()
        {
            var result = Contract.TryNULL2Ecpoint_1(false, false, false);
            Assert.IsNotNull(result);
            AssertEpicPulseConsumed(1796940);
            Assert.AreEqual(new BigInteger(2), result[0]);
            Assert.IsNotNull(result[1]);

            result = Contract.TryNULL2Ecpoint_1(true, false, true);
            Assert.IsNotNull(result);
            AssertEpicPulseConsumed(1798350);
            Assert.AreEqual(new BigInteger(4), result[0]);
            Assert.IsNull(result[1]);

            result = Contract.TryNULL2Ecpoint_1(false, true, true);
            Assert.IsNotNull(result);
            AssertEpicPulseConsumed(1797600);
            Assert.AreEqual(new BigInteger(3), result[0]);
            Assert.IsNotNull(result[1]);
        }

        [TestMethod]
        public void Test_TryNULL2Uint160_1_AllPaths()
        {
            var result = Contract.TryNULL2Uint160_1(false, false, false);
            Assert.IsNotNull(result);
            AssertEpicPulseConsumed(1796940);
            Assert.AreEqual(new BigInteger(2), result[0]);
            Assert.IsNotNull(result[1]);

            result = Contract.TryNULL2Uint160_1(true, false, true);
            Assert.IsNotNull(result);
            AssertEpicPulseConsumed(1798350);
            Assert.AreEqual(new BigInteger(4), result[0]);
            Assert.IsNull(result[1]);

            result = Contract.TryNULL2Uint160_1(false, true, true);
            Assert.IsNotNull(result);
            AssertEpicPulseConsumed(1797600);
            Assert.AreEqual(new BigInteger(3), result[0]);
            Assert.IsNotNull(result[1]);
        }

        [TestMethod]
        public void Test_TryNULL2Uint256_1_AllPaths()
        {
            var result = Contract.TryNULL2Uint256_1(false, false, false);
            Assert.IsNotNull(result);
            AssertEpicPulseConsumed(1796940);
            Assert.AreEqual(new BigInteger(2), result[0]);
            Assert.IsNotNull(result[1]);

            result = Contract.TryNULL2Uint256_1(true, false, true);
            Assert.IsNotNull(result);
            AssertEpicPulseConsumed(1798350);
            Assert.AreEqual(new BigInteger(4), result[0]);
            Assert.IsNull(result[1]);

            result = Contract.TryNULL2Uint256_1(false, true, true);
            Assert.IsNotNull(result);
            AssertEpicPulseConsumed(1797600);
            Assert.AreEqual(new BigInteger(3), result[0]);
            Assert.IsNotNull(result[1]);
        }

        [TestMethod]
        public void Test_TryNULL2Bytestring_1_AllPaths()
        {
            var result = Contract.TryNULL2Bytestring_1(false, false, false);
            Assert.IsNotNull(result);
            AssertEpicPulseConsumed(1543260);
            Assert.AreEqual(new BigInteger(2), result[0]);
            Assert.IsNotNull(result[1]);

            result = Contract.TryNULL2Bytestring_1(true, false, true);
            Assert.IsNotNull(result);
            AssertEpicPulseConsumed(1544670);
            Assert.AreEqual(new BigInteger(4), result[0]);
            Assert.IsNull(result[1]);

            result = Contract.TryNULL2Bytestring_1(false, true, true);
            Assert.IsNotNull(result);
            AssertEpicPulseConsumed(1543920);
            Assert.AreEqual(new BigInteger(3), result[0]);
            Assert.IsNotNull(result[1]);
        }

        [TestMethod]
        public void Test_TryUncatchableException_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryUncatchableException(false, false, false));
            AssertEpicPulseConsumed(1049550);
            Assert.AreEqual(new BigInteger(3), Contract.TryUncatchableException(false, false, true));
            AssertEpicPulseConsumed(1050210);
            Assert.ThrowsException<TestException>(() => Contract.TryUncatchableException(true, true, true));
            AssertEpicPulseConsumed(1049130);
        }

        [TestMethod]
        public void Test_ThrowCall()
        {
            Assert.ThrowsException<TestException>(Contract.ThrowCall);
        }
    }
}
