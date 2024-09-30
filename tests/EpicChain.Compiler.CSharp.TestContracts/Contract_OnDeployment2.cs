using EpicChain.SmartContract.Framework.Services;

namespace EpicChain.Compiler.CSharp.TestContracts
{
    public class Contract_OnDeployment2 : SmartContract.Framework.SmartContract
    {
        public static void _deploy(object data, bool update)
        {
            Runtime.Log("Deployed");
        }
    }
}
