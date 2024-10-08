using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;

namespace EpicChain.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class SupportedStandardsTest
    {
        public SupportedStandardsTest()
        {
            // Ensure also Contract_ExtraAttribute
            TestCleanup.TestInitialize(typeof(Contract_SupportedStandards));
            TestCleanup.TestInitialize(typeof(Contract_SupportedStandard11Enum));
            TestCleanup.TestInitialize(typeof(Contract_SupportedStandard11Payable));
            TestCleanup.TestInitialize(typeof(Contract_SupportedStandard17Enum));
            TestCleanup.TestInitialize(typeof(Contract_SupportedStandard17Payable));
        }

        [TestMethod]
        public void TestAttribute()
        {
            CollectionAssert.AreEqual(Contract_SupportedStandards.Manifest.SupportedStandards, new string[] { "XEP-10", "XEP-5" });
        }

        [TestMethod]
        public void TestStandardXEP11AttributeEnum()
        {
            CollectionAssert.AreEqual(Contract_SupportedStandard11Enum.Manifest.SupportedStandards, new string[] { "XEP-11" });
        }

        [TestMethod]
        public void TestStandardXEP17AttributeEnum()
        {
            CollectionAssert.AreEqual(Contract_SupportedStandard17Enum.Manifest.SupportedStandards, new string[] { "XEP-17" });
        }

        [TestMethod]
        public void TestStandardXEP11PayableAttribute()
        {
            CollectionAssert.AreEqual(Contract_SupportedStandard11Payable.Manifest.SupportedStandards, new string[] { "XEP-26" });
        }

        [TestMethod]
        public void TestStandardXep17PayableAttribute()
        {
            CollectionAssert.AreEqual(Contract_SupportedStandard17Payable.Manifest.SupportedStandards, new string[] { "XEP-27" });
        }
    }
}
