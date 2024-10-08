using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.SmartContract.Testing;
using EpicChain.SmartContract.Testing.Exceptions;

namespace EpicChain.SmartContract.Framework.UnitTests.Services
{
    [TestClass]
    public class OracleTest : DebugAndTestBase<Contract_IOracle>
    {
        [TestMethod]
        public void Test_OracleResponse()
        {
            Assert.ThrowsException<TestException>(() => Contract.OnOracleResponse("http://127.0.0.1", "test", 0x14, "{}"));

            Engine.OnGetCallingScriptHash = (current, expected) => Engine.Native.Oracle.Hash;
            Contract.OnOracleResponse("http://127.0.0.1", "test", 0x14, "{}");
            AssertLogs("Oracle call!");
        }
    }
}
