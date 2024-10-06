using EpicChain.SmartContract.Framework;
using EpicChain.SmartContract.Framework.Attributes;

namespace EpicChain.Compiler.CSharp.TestContracts
{
    [SupportedStandards(XepStandard.XEP11)]
    public class Contract_XEP11 : XEP11Token<TokenState>
    {
        public override string Symbol { [Safe] get => "TEST"; }
    }

    public class TokenState : XEP11TokenState
    {

    }
}
