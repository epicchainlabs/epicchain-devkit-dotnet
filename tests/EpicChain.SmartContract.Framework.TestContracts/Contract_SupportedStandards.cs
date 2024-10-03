using EpicChain.SmartContract.Framework.Attributes;
using EpicChain.SmartContract.Framework.Services;

namespace EpicChain.SmartContract.Framework.UnitTests.TestClasses
{
    // Both XEP-10 and XEP-5 are obsolete, but this is just a test contract
    [SupportedStandards("XEP-10", "XEP-5")]
    public class Contract_SupportedStandards : SmartContract
    {
        public static bool TestStandard()
        {
            return true;
        }
    }
}
