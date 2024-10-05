using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;

namespace EpicChain.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class ManifestExtraTest
    {
        public ManifestExtraTest()
        {
            // Ensure also Contract_ExtraAttribute

            TestCleanup.TestInitialize(typeof(Contract_ExtraAttribute));
        }

        [TestMethod]
        public void TestExtraAttribute()
        {
            var extra = Contract_ExtraAttribute.Manifest.Extra;

            Assert.AreEqual("EpicChain", extra["xmoohad"]?.GetString());
            Assert.AreEqual("devs@epic-chain.org", extra["E-mail"]?.GetString());
        }
    }
}
