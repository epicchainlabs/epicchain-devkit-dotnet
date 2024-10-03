extern alias scfx;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using scfxStackItemType = scfx.EpicChain.SmartContract.Framework.StackItemType;

namespace EpicChain.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class StackItemTypeTest
    {
        [TestMethod]
        public void TestValues()
        {
            Assert.AreEqual(((byte)VM.Types.StackItemType.Buffer).ToString("x2"), scfxStackItemType.Buffer[2..]);
            Assert.AreEqual(((byte)VM.Types.StackItemType.Integer).ToString("x2"), scfxStackItemType.Integer[2..]);
        }
    }
}
