using EpicChain.SmartContract.Framework;
using EpicChain.SmartContract.Framework.Attributes;

namespace EpicChain.Compiler.CSharp.TestContracts
{
    [SupportedStandards(XepStandard.Nep17)]
    public class Contract_NEP17 : Nep17Token
    {
        public override byte Decimals { [Safe] get => 8; }

        public override string Symbol { [Safe] get => "TEST"; }
    }
}
