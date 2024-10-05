using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;
using System.Numerics;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_NULL : DebugAndTestBase<Contract_NULL>
    {
        [TestMethod]
        public void IsNull()
        {
            // True

            Assert.IsTrue(Contract.IsNull(null));
            AssertEpicPulseConsumed(1048140);

            // False

            Assert.IsFalse(Contract.IsNull(1));
            AssertEpicPulseConsumed(1048140);
        }

        [TestMethod]
        public void IfNull()
        {
            Assert.IsFalse(Contract.IfNull(null));
            AssertEpicPulseConsumed(1047120);
        }

        [TestMethod]
        public void NullProperty()
        {
            Assert.IsTrue(Contract.NullProperty(null));
            AssertEpicPulseConsumed(1048200);
            Assert.IsFalse(Contract.NullProperty(""));
            AssertEpicPulseConsumed(1048530);
            Assert.IsTrue(Contract.NullProperty("123"));
            AssertEpicPulseConsumed(1048530);
        }

        [TestMethod]
        public void NullPropertyGT()
        {
            Assert.IsFalse(Contract.NullPropertyGT(null));
            AssertEpicPulseConsumed(1047480);
            Assert.IsFalse(Contract.NullPropertyGT(""));
            AssertEpicPulseConsumed(1047810);
            Assert.IsTrue(Contract.NullPropertyGT("123"));
            AssertEpicPulseConsumed(1047810);
        }

        [TestMethod]
        public void NullPropertyLT()
        {
            Assert.IsFalse(Contract.NullPropertyLT(null));
            AssertEpicPulseConsumed(1047480);
            Assert.IsFalse(Contract.NullPropertyLT(""));
            AssertEpicPulseConsumed(1047810);
            Assert.IsFalse(Contract.NullPropertyLT("123"));
            AssertEpicPulseConsumed(1047810);
        }

        [TestMethod]
        public void NullPropertyGE()
        {
            Assert.IsFalse(Contract.NullPropertyGE(null));
            AssertEpicPulseConsumed(1047480);
            Assert.IsTrue(Contract.NullPropertyGE(""));
            AssertEpicPulseConsumed(1047810);
            Assert.IsTrue(Contract.NullPropertyGE("123"));
            AssertEpicPulseConsumed(1047810);
        }

        [TestMethod]
        public void NullPropertyLE()
        {
            Assert.IsFalse(Contract.NullPropertyLE(null));
            AssertEpicPulseConsumed(1047480);
            Assert.IsTrue(Contract.NullPropertyLE(""));
            AssertEpicPulseConsumed(1047810);
            Assert.IsFalse(Contract.NullPropertyLE("123"));
            AssertEpicPulseConsumed(1047810);
        }

        [TestMethod]
        public void NullCoalescing()
        {
            //  call NullCoalescing(string code)
            // return  code ?.Substring(1,2);

            // a123b->12
            {
                var data = (VM.Types.ByteString)Contract.NullCoalescing("a123b")!;
                AssertEpicPulseConsumed(1109040);
                Assert.AreEqual("12", System.Text.Encoding.ASCII.GetString(data.GetSpan()));
            }
            // null->null
            {
                Assert.IsNull(Contract.NullCoalescing(null));
                AssertEpicPulseConsumed(1047330);
            }
        }

        [TestMethod]
        public void NullCollation()
        {
            // call nullCollation(string code)
            // return code ?? "linux"

            // nes->nes
            {
                Assert.AreEqual("nes", Contract.NullCollation("nes"));
                AssertEpicPulseConsumed(1047540);
            }

            // null->linux
            {
                Assert.AreEqual("linux", Contract.NullCollation(null));
                AssertEpicPulseConsumed(1047630);
            }
        }

        [TestMethod]
        public void NullCollationAndCollation()
        {
            Assert.AreEqual(new BigInteger(123), ((VM.Types.ByteString)Contract.NullCollationAndCollation("nes")!).GetInteger());
            AssertEpicPulseConsumed(2522880);
        }

        [TestMethod]
        public void NullCollationAndCollation2()
        {
            Assert.AreEqual("111", ((VM.Types.ByteString)Contract.NullCollationAndCollation2("nes")!).GetString());
            AssertEpicPulseConsumed(3614460);
        }

        [TestMethod]
        public void EqualNull()
        {
            // True

            Assert.IsTrue(Contract.EqualNullA(null));
            AssertEpicPulseConsumed(1048020);

            // False

            Assert.IsFalse(Contract.EqualNullA(1));
            AssertEpicPulseConsumed(1048020);

            // True

            Assert.IsTrue(Contract.EqualNullB(null));
            AssertEpicPulseConsumed(1048020);

            // False

            Assert.IsFalse(Contract.EqualNullB(1));
            AssertEpicPulseConsumed(1048020);
        }

        [TestMethod]
        public void EqualNotNull()
        {
            // True

            Assert.IsFalse(Contract.EqualNotNullA(null));
            AssertEpicPulseConsumed(1048020);

            // False

            Assert.IsTrue(Contract.EqualNotNullA(1));
            AssertEpicPulseConsumed(1048020);

            // True

            Assert.IsFalse(Contract.EqualNotNullB(null));
            AssertEpicPulseConsumed(1048020);

            // False

            Assert.IsTrue(Contract.EqualNotNullB(1));
            AssertEpicPulseConsumed(1048020);
        }

        [TestMethod]
        public void NullTypeTest()
        {
            Contract.NullType(); // no error
            AssertEpicPulseConsumed(986340);
        }
    }
}
