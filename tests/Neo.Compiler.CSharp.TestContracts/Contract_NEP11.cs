using Chain.SmartContract.Framework;
using Chain.SmartContract.Framework.Attributes;

namespace Chain.Compiler.CSharp.TestContracts
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
