using EpicChain.SmartContract.Framework.Services;
using System.ComponentModel;

namespace EpicChain.Compiler.CSharp.TestContracts
{
    public class Contract_OnDeployment1 : SmartContract.Framework.SmartContract
    {
        [DisplayName("_deploy")]
        public static void MyDeployMethod(object data, bool update)
        {
            Runtime.Log("Deployed");
        }
    }
}
