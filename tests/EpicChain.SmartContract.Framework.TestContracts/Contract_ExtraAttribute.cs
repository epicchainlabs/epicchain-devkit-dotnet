using EpicChain.SmartContract.Framework.Attributes;

namespace EpicChain.SmartContract.Framework.UnitTests.TestClasses
{
    [ManifestExtra("Author", "Neo")]
    [ManifestExtra("E-mail", "devs@epic-chain.org")]
    public class Contract_ExtraAttribute : SmartContract
    {
        public static object Main2(string method, object[] args)
        {
            return true;
        }
    }
}
