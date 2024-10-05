using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;
using System.Collections.Generic;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Char : DebugAndTestBase<Contract_Char>
    {
        public static IEnumerable<object[]> CharTestData =>
            new List<object[]>
            {
                new object[] { '0', true, false, false, false, false },
                new object[] { '9', true, false, false, false, false },
                new object[] { 'a', false, true, false, true, false },
                new object[] { 'Z', false, true, false, false, true },
                new object[] { ' ', false, false, true, false, false },
                new object[] { '\t', false, false, true, false, false },
                new object[] { '$', false, false, false, false, false },
                new object[] { '\n', false, false, true, false, false },
            };

        [TestMethod]
        [DynamicData(nameof(CharTestData))]
        public void TestCharProperties(char c, bool isDigit, bool isLetter, bool isWhiteSpace, bool isLower, bool isUpper)
        {
            Assert.AreEqual(isDigit, Contract.TestCharIsDigit(c), $"IsDigit failed for '{c}'");
            AssertEpicPulseConsumed(1047330);
            Assert.AreEqual(isLetter, Contract.TestCharIsLetter(c), $"IsLetter failed for '{c}'");
            AssertEpicPulseConsumed(1047990);
            Assert.AreEqual(isWhiteSpace, Contract.TestCharIsWhiteSpace(c), $"IsWhiteSpace failed for '{c}'");
            Assert.AreEqual(isLower, Contract.TestCharIsLower(c), $"IsLower failed for '{c}'");
            AssertEpicPulseConsumed(1047330);
            Assert.AreEqual(isUpper, Contract.TestCharIsUpper(c), $"IsUpper failed for '{c}'");
            AssertEpicPulseConsumed(1047330);
        }

        [TestMethod]
        public void TestCharGetNumericValue()
        {
            Assert.AreEqual(0, Contract.TestCharGetNumericValue('0'));
            AssertEpicPulseConsumed(1047720);
            Assert.AreEqual(9, Contract.TestCharGetNumericValue('9'));
            AssertEpicPulseConsumed(1047720);
            Assert.AreEqual(-1, Contract.TestCharGetNumericValue('a'));
            AssertEpicPulseConsumed(1047540);
            Assert.AreEqual(-1, Contract.TestCharGetNumericValue('$'));
            AssertEpicPulseConsumed(1047540);
        }

        [TestMethod]
        public void TestCharSpecialCategories()
        {
            Assert.IsTrue(Contract.TestCharIsPunctuation('.'));
            AssertEpicPulseConsumed(1047450);
            Assert.IsTrue(Contract.TestCharIsPunctuation(','));
            AssertEpicPulseConsumed(1047450);
            Assert.IsFalse(Contract.TestCharIsPunctuation('a'));
            AssertEpicPulseConsumed(1048590);

            Assert.IsTrue(Contract.TestCharIsSymbol('$'));
            AssertEpicPulseConsumed(1047450);
            Assert.IsTrue(Contract.TestCharIsSymbol('+'));
            AssertEpicPulseConsumed(1047450);
            Assert.IsFalse(Contract.TestCharIsSymbol('a'));
            AssertEpicPulseConsumed(1049010);

            Assert.IsTrue(Contract.TestCharIsControl('\n'));
            AssertEpicPulseConsumed(1047990);
            Assert.IsTrue(Contract.TestCharIsControl('\0'));
            AssertEpicPulseConsumed(1047990);
            Assert.IsFalse(Contract.TestCharIsControl('a'));
            AssertEpicPulseConsumed(1047990);
        }

        [TestMethod]
        public void TestCharSurrogates()
        {
            Assert.IsTrue(Contract.TestCharIsSurrogate('\uD800'));
            AssertEpicPulseConsumed(1047990);
            Assert.IsTrue(Contract.TestCharIsSurrogate('\uDFFF'));
            AssertEpicPulseConsumed(1047990);
            Assert.IsFalse(Contract.TestCharIsSurrogate('a'));
            AssertEpicPulseConsumed(1047990);

            Assert.IsTrue(Contract.TestCharIsHighSurrogate('\uD800'));
            AssertEpicPulseConsumed(1047330);
            Assert.IsFalse(Contract.TestCharIsHighSurrogate('\uDC00'));
            AssertEpicPulseConsumed(1047330);
            Assert.IsFalse(Contract.TestCharIsHighSurrogate('a'));
            AssertEpicPulseConsumed(1047330);

            Assert.IsTrue(Contract.TestCharIsLowSurrogate('\uDC00'));
            AssertEpicPulseConsumed(1047330);
            Assert.IsFalse(Contract.TestCharIsLowSurrogate('\uD800'));
            AssertEpicPulseConsumed(1047330);
            Assert.IsFalse(Contract.TestCharIsLowSurrogate('a'));
            AssertEpicPulseConsumed(1047330);
        }

        [TestMethod]
        public void TestCharConversions()
        {
            Assert.AreEqual('A', Contract.TestCharToUpper('a'));
            AssertEpicPulseConsumed(1047990);
            Assert.AreEqual('A', Contract.TestCharToUpper('A'));
            AssertEpicPulseConsumed(1047450);
            Assert.AreEqual('a', Contract.TestCharToLower('A'));
            AssertEpicPulseConsumed(1047990);
            Assert.AreEqual('a', Contract.TestCharToLower('a'));
            AssertEpicPulseConsumed(1047450);
        }

        [TestMethod]
        public void TestCharIsLetterOrDigit()
        {
            Assert.IsTrue(Contract.TestCharIsLetterOrDigit('a'));
            AssertEpicPulseConsumed(1048170);
            Assert.IsTrue(Contract.TestCharIsLetterOrDigit('A'));
            AssertEpicPulseConsumed(1047870);
            Assert.IsTrue(Contract.TestCharIsLetterOrDigit('0'));
            AssertEpicPulseConsumed(1047450);
            Assert.IsFalse(Contract.TestCharIsLetterOrDigit('$'));
            AssertEpicPulseConsumed(1048170);
        }

        [TestMethod]
        public void TestCharIsBetween()
        {
            Assert.IsTrue(Contract.TestCharIsBetween('a', 'a', 'z'));
            AssertEpicPulseConsumed(1047870);
            Assert.IsTrue(Contract.TestCharIsBetween('z', 'a', 'z'));
            AssertEpicPulseConsumed(1047870);

            Assert.IsFalse(Contract.TestCharIsBetween('A', 'a', 'z'));
            AssertEpicPulseConsumed(1047990);
            Assert.IsFalse(Contract.TestCharIsBetween('0', 'a', 'z'));
            AssertEpicPulseConsumed(1047990);
        }
    }
}
