using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;
using EpicChain.SmartContract.Testing.Exceptions;
using System.Numerics;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_IntegerParse : DebugAndTestBase<Contract_IntegerParse>
    {
        [TestMethod]
        public void SByteParse_Test()
        {
            Assert.AreEqual(new BigInteger(sbyte.MaxValue), Contract.TestSbyteparse("127"));
            AssertEpicPulseConsumed(2032650);
            Assert.AreEqual(new BigInteger(sbyte.MinValue), Contract.TestSbyteparse("-128"));
            AssertEpicPulseConsumed(2032650);

            //test backspace trip
            Assert.ThrowsException<TestException>(() => Contract.TestSbyteparse("20 "));
            AssertEpicPulseConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestSbyteparse(" 20 "));
            AssertEpicPulseConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestSbyteparse("128"));
            AssertEpicPulseConsumed(2048010);
            Assert.ThrowsException<TestException>(() => Contract.TestSbyteparse("-129"));
            AssertEpicPulseConsumed(2048010);
            Assert.ThrowsException<TestException>(() => Contract.TestSbyteparse(""));
            AssertEpicPulseConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestSbyteparse("abc"));
            AssertEpicPulseConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestSbyteparse("@"));
            AssertEpicPulseConsumed(2032230);
        }

        [TestMethod]
        public void ByteParse_Test()
        {
            Assert.AreEqual(new BigInteger(byte.MinValue), Contract.TestByteparse("0"));
            AssertEpicPulseConsumed(2032650);
            Assert.AreEqual(new BigInteger(byte.MaxValue), Contract.TestByteparse("255"));
            AssertEpicPulseConsumed(2032650);

            //test backspace trip
            Assert.ThrowsException<TestException>(() => Contract.TestByteparse("20 "));
            AssertEpicPulseConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestByteparse(" 20 "));
            AssertEpicPulseConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestByteparse("-1"));
            AssertEpicPulseConsumed(2048010);
            Assert.ThrowsException<TestException>(() => Contract.TestByteparse("256"));
            AssertEpicPulseConsumed(2048010);
            Assert.ThrowsException<TestException>(() => Contract.TestByteparse(""));
            AssertEpicPulseConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestByteparse("abc"));
            AssertEpicPulseConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestByteparse("@"));
            AssertEpicPulseConsumed(2032230);
        }

        [TestMethod]
        public void UShortParse_Test()
        {
            Assert.AreEqual(new BigInteger(ushort.MinValue), Contract.TestUshortparse("0"));
            AssertEpicPulseConsumed(2032650);
            Assert.AreEqual(new BigInteger(ushort.MaxValue), Contract.TestUshortparse("65535"));
            AssertEpicPulseConsumed(2032650);

            //test backspace trip
            Assert.ThrowsException<TestException>(() => Contract.TestUshortparse("20 "));
            AssertEpicPulseConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestUshortparse(" 20 "));
            AssertEpicPulseConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestUshortparse("-1"));
            AssertEpicPulseConsumed(2048010);
            Assert.ThrowsException<TestException>(() => Contract.TestUshortparse("65536"));
            AssertEpicPulseConsumed(2048010);
            Assert.ThrowsException<TestException>(() => Contract.TestUshortparse(""));
            AssertEpicPulseConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestUshortparse("abc"));
            AssertEpicPulseConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestUshortparse("@"));
            AssertEpicPulseConsumed(2032230);
        }

        [TestMethod]
        public void ShortParse_Test()
        {
            Assert.AreEqual(new BigInteger(short.MinValue), Contract.TestShortparse("-32768"));
            AssertEpicPulseConsumed(2032650);
            Assert.AreEqual(new BigInteger(short.MaxValue), Contract.TestShortparse("32767"));
            AssertEpicPulseConsumed(2032650);

            //test backspace trip
            Assert.ThrowsException<TestException>(() => Contract.TestShortparse("20 "));
            AssertEpicPulseConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestShortparse(" 20 "));
            AssertEpicPulseConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestShortparse("-32769"));
            AssertEpicPulseConsumed(2048010);
            Assert.ThrowsException<TestException>(() => Contract.TestShortparse("32768"));
            AssertEpicPulseConsumed(2048010);
            Assert.ThrowsException<TestException>(() => Contract.TestShortparse(""));
            AssertEpicPulseConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestShortparse("abc"));
            AssertEpicPulseConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestShortparse("@"));
            AssertEpicPulseConsumed(2032230);
        }

        [TestMethod]
        public void ULongParse_Test()
        {
            Assert.AreEqual(new BigInteger(ulong.MinValue), Contract.TestUlongparse("0"));
            AssertEpicPulseConsumed(2032740);
            Assert.AreEqual(new BigInteger(ulong.MaxValue), Contract.TestUlongparse("18446744073709551615"));
            AssertEpicPulseConsumed(2032740);

            //test backspace trip
            Assert.ThrowsException<TestException>(() => Contract.TestUlongparse("20 "));
            AssertEpicPulseConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestUlongparse(" 20 "));
            AssertEpicPulseConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestUlongparse("-1"));
            AssertEpicPulseConsumed(2048100);
            Assert.ThrowsException<TestException>(() => Contract.TestUlongparse("18446744073709551616"));
            AssertEpicPulseConsumed(2048100);
            Assert.ThrowsException<TestException>(() => Contract.TestUlongparse(""));
            AssertEpicPulseConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestUlongparse("abc"));
            AssertEpicPulseConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestUlongparse("@"));
            AssertEpicPulseConsumed(2032230);
        }

        [TestMethod]
        public void LongParse_Test()
        {
            Assert.AreEqual(new BigInteger(long.MinValue), Contract.TestLongparse("-9223372036854775808"));
            AssertEpicPulseConsumed(2032740);
            Assert.AreEqual(new BigInteger(long.MaxValue), Contract.TestLongparse("9223372036854775807"));
            AssertEpicPulseConsumed(2032740);

            //test backspace trip
            Assert.ThrowsException<TestException>(() => Contract.TestLongparse("20 "));
            AssertEpicPulseConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestLongparse(" 20 "));
            AssertEpicPulseConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestLongparse("-9223372036854775809"));
            AssertEpicPulseConsumed(2048100);
            Assert.ThrowsException<TestException>(() => Contract.TestLongparse("9223372036854775808"));
            AssertEpicPulseConsumed(2048100);
            Assert.ThrowsException<TestException>(() => Contract.TestLongparse(""));
            AssertEpicPulseConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestLongparse("abc"));
            AssertEpicPulseConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestLongparse("@"));
            AssertEpicPulseConsumed(2032230);
        }

        [TestMethod]
        public void UIntParse_Test()
        {
            Assert.AreEqual(new BigInteger(uint.MinValue), Contract.TestUintparse("0"));
            AssertEpicPulseConsumed(2032650);
            Assert.AreEqual(new BigInteger(uint.MaxValue), Contract.TestUintparse("4294967295"));
            AssertEpicPulseConsumed(2032650);

            //test backspace trip
            Assert.ThrowsException<TestException>(() => Contract.TestUintparse("20 "));
            AssertEpicPulseConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestUintparse(" 20 "));
            AssertEpicPulseConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestUintparse("-1"));
            AssertEpicPulseConsumed(2048010);
            Assert.ThrowsException<TestException>(() => Contract.TestUintparse("4294967296"));
            AssertEpicPulseConsumed(2048010);
            Assert.ThrowsException<TestException>(() => Contract.TestUintparse(""));
            AssertEpicPulseConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestUintparse("abc"));
            AssertEpicPulseConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestUintparse("@"));
            AssertEpicPulseConsumed(2032230);
        }

        [TestMethod]
        public void IntParse_Test()
        {
            Assert.AreEqual(new BigInteger(int.MinValue), Contract.TestIntparse("-2147483648"));
            AssertEpicPulseConsumed(2032650);
            Assert.AreEqual(new BigInteger(int.MaxValue), Contract.TestIntparse("2147483647"));
            AssertEpicPulseConsumed(2032650);

            //test backspace trip
            Assert.ThrowsException<TestException>(() => Contract.TestIntparse("20 "));
            AssertEpicPulseConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestIntparse(" 20 "));
            AssertEpicPulseConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestIntparse("-2147483649"));
            AssertEpicPulseConsumed(2048010);
            Assert.ThrowsException<TestException>(() => Contract.TestIntparse("2147483648"));
            AssertEpicPulseConsumed(2048010);
            Assert.ThrowsException<TestException>(() => Contract.TestIntparse(""));
            AssertEpicPulseConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestIntparse("abc"));
            AssertEpicPulseConsumed(2032230);
            Assert.ThrowsException<TestException>(() => Contract.TestIntparse("@"));
            AssertEpicPulseConsumed(2032230);
        }
    }
}
