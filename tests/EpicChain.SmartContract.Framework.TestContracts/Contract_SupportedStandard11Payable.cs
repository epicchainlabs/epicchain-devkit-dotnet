using EpicChain.SmartContract.Framework.Attributes;
using System.ComponentModel;
using System.Numerics;
using EpicChain.SmartContract.Framework.Interfaces;

namespace EpicChain.SmartContract.Framework.TestContracts
{
    [DisplayName(nameof(Contract_SupportedStandard11Payable))]
    [ContractDescription("<Description Here>")]
    [ContractAuthor("<Your Name Or Company Here>", "<Your Public Email Here>")]
    [ContractVersion("<Version String Here>")]
    [ContractPermission(Permission.Any, Method.Any)]
    [SupportedStandards(NepStandard.Nep11Payable)]
    public class Contract_SupportedStandard11Payable : SmartContract, INep11Payable
    {
        public void OnNEP11Payment(UInt160 from, BigInteger amount, string tokenId, object? data = null)
        {
        }
    }
}
