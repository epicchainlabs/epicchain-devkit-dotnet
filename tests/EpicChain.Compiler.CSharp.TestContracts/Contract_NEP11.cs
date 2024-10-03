using EpicChain.SmartContract.Framework;
using EpicChain.SmartContract.Framework.Attributes;

namespace EpicChain.Compiler.CSharp.TestContracts
{
    [SupportedStandards(NepStandard.Nep11)]
    public class Contract_NEP11 : Nep11Token<TokenState>
    {
        public override string Symbol { [Safe] get => "TEST"; }
    }

    public class TokenState : Nep11TokenState
    {

    }
}
