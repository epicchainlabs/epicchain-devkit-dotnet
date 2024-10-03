using System.Numerics;
using EpicChain.SmartContract.Framework.Attributes;
using EpicChain.SmartContract.Framework.Interfaces;

namespace EpicChain.SmartContract.Framework.UnitTests.TestClasses
{
    [SupportedStandards(XepStandard.XEP11)]
    public class Contract_SupportedStandard11Enum : XEP11Token<XEP11TokenState>, IXEP11Payable
    {
        public static bool TestStandard()
        {
            return true;
        }

        public override string Symbol { [Safe] get; } = "EXAMPLE";

        public void OnXEP11Payment(UInt160 from, BigInteger amount, string tokenId, object? data = null)
        {
        }
    }
}
