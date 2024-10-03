using EpicChain.SmartContract.Framework.Interfaces;
using EpicChain.SmartContract.Framework.Native;
using EpicChain.SmartContract.Framework.Services;

namespace EpicChain.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_IOracle : SmartContract, IOracle
    {
        public void OnOracleResponse(string url, object userData, OracleResponseCode code, string result)
        {
            if (Runtime.CallingScriptHash != Oracle.Hash)
                throw new System.Exception("Unauthorized!");

            Runtime.Log("Oracle call!");
        }
    }
}
