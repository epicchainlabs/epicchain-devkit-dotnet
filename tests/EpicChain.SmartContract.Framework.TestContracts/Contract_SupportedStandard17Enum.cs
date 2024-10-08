using EpicChain.SmartContract.Framework.Attributes;
using System.ComponentModel;
using System.Numerics;
using EpicChain.SmartContract.Framework.Interfaces;

namespace EpicChain.SmartContract.Framework.UnitTests.TestClasses
{
    [DisplayName(nameof(Contract_SupportedStandard17Enum))]
    [ManifestExtra("Author", "<Your Name Or Company Here>")]
    [ContractDescription("<Description Here>")]
    [ManifestExtra("Email", "<Your Public Email Here>")]
    [ManifestExtra("Version", "<Version String Here>")]
    [ContractSourceCode("https://github.com/epicchainlabs/epicchain-devkit-dotnet/tree/master/src/EpicChain.SmartContract.Template")]
    [ContractPermission(Permission.Any, Method.Any)]
    [SupportedStandards(XepStandard.Xep17)]
    public class Contract_SupportedStandard17Enum : Xep17Token, IXEP17Payable
    {
        public override string Symbol { [Safe] get; } = "EXAMPLE";
        public override byte Decimals { [Safe] get; } = 0;

        public void onXEP17Payment(UInt160 from, BigInteger amount, object? data = null)
        {
        }
    }
}
