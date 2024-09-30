using System.Numerics;
using EpicChain.SmartContract.Framework.Attributes;
using EpicChain.SmartContract.Framework.Interfaces;

namespace EpicChain.SmartContract.Framework.UnitTests.TestClasses
{
    [SupportedStandards(NepStandard.Nep11)]
    public class Contract_SupportedStandard11Enum : Nep11Token<Nep11TokenState>, INep11Payable
    {
        public static bool TestStandard()
        {
            return true;
        }

        public override string Symbol { [Safe] get; } = "EXAMPLE";

        public void OnNEP11Payment(UInt160 from, BigInteger amount, string tokenId, object? data = null)
        {
        }
    }
}
