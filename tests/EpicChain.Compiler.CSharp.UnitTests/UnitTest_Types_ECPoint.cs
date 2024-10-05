using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.Extensions;
using EpicChain.SmartContract.Testing;
using EpicChain.SmartContract.Testing.Exceptions;
using EpicChain.SmartContract.Testing.Interpreters;
using EpicChain.SmartContract.Testing.InvalidTypes;

namespace EpicChain.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Types_ECPoint : DebugAndTestBase<Contract_Types_ECPoint>
    {
        [TestMethod]
        public void ECPoint_test()
        {
            Assert.IsFalse(Contract.IsValid(InvalidECPoint.InvalidLength));
            AssertEpicPulseConsumed(1048830);
            Assert.IsFalse(Contract.IsValid(InvalidECPoint.InvalidType));
            AssertEpicPulseConsumed(1048620);
            Assert.ThrowsException<TestException>(() => Contract.IsValid(InvalidECPoint.Null));
            AssertEpicPulseConsumed(1048110);
            Assert.IsTrue(Contract.IsValid(Cryptography.ECC.ECPoint.Parse("024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9", Cryptography.ECC.ECCurve.Secp256r1)));
            AssertEpicPulseConsumed(1048830);

            Engine.StringInterpreter = new HexStringInterpreter();

            Assert.AreEqual("024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9", Contract.Ecpoint2String());
            AssertEpicPulseConsumed(984870);
            Assert.AreEqual("024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9", Contract.EcpointReturn()?.ToString());
            AssertEpicPulseConsumed(984870);
            Assert.AreEqual("024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9", (Contract.Ecpoint2ByteArray() as VM.Types.Buffer)!.GetSpan().ToHexString());
            AssertEpicPulseConsumed(1230630);
        }
    }
}
