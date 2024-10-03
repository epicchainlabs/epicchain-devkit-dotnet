using EpicChain.SmartContract.Framework.Attributes;
using System.ComponentModel;
using System.Numerics;
using EpicChain.SmartContract.Framework.Interfaces;

namespace EpicChain.SmartContract.Framework.TestContracts
{
    [DisplayName(nameof(Contract_SupportedStandard17Payable))]
    [ContractDescription("<Description Here>")]
    [ContractAuthor("<Your Name Or Company Here>", "<Your Public Email Here>")]
    [ContractVersion("<Version String Here>")]
    [ContractPermission(Permission.Any, Method.Any)]
    [SupportedStandards(XepStandard.Xep17Payable)]
    public class Contract_SupportedStandard17Payable : SmartContract, IXEP17Payable
    {
        public void onXEP17Payment(UInt160 from, BigInteger amount, object? data = null)
        {
        }
    }
}
