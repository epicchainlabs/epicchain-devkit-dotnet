using EpicChain.SmartContract.Framework.Attributes;

namespace EpicChain.SmartContract.Framework.UnitTests.TestClasses
{
    [ContractAuthor("core-dev")]
    [ContractEmail("dev@EpicChain.org")]
    [ContractVersion("v3.6.3")]
    [ContractDescription("This is a test contract.")]
    [ManifestExtra("ExtraKey", "ExtraValue")]
    public class Contract_ManifestAttribute : SmartContract
    {
        [NoReentrant]
        public void reentrantTest(int value)
        {
            if (value == 0) return;
            if (value == 123)
            {
                reentrantTest(0);
            }
        }
    }
}
