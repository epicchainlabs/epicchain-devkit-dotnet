using EpicChain.SmartContract.Framework.Attributes;
using EpicChain.SmartContract.Framework.Native;
using EpicChain.SmartContract.Framework.Services;

namespace EpicChain.SmartContract.Framework.UnitTests.TestClasses
{
    [ContractPermission(Permission.Any, Method.Any)]
    public class Contract_Create : SmartContract
    {
        public static string OldContract()
        {
            return ContractManagement.GetContract(Runtime.CallingScriptHash).Manifest.Name;
        }

        public static Contract GetContractById(int id)
        {
            return ContractManagement.GetContractById(id);
        }

        public static object GetContractHashes()
        {
            var iter = ContractManagement.GetContractHashes();
            iter.Next();
            return iter.Value.Item2;
        }

        public static void Update(byte[] nef, string manifest)
        {
            ContractManagement.Update((ByteString)nef, manifest, null);
        }

        public static void Destroy()
        {
            ContractManagement.Destroy();
        }

        public static int GetCallFlags()
        {
            return (int)Contract.GetCallFlags();
        }
    }
}
