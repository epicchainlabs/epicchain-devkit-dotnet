using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.Json;
using EpicChain.Network.P2P.Payloads;
using EpicChain.SmartContract.Testing;
using EpicChain.SmartContract.Testing.Exceptions;
using System.Text;
using EpicChain.SmartContract.Testing.TestingStandards;

namespace EpicChain.SmartContract.Template.UnitTests.templates.epicchaincontractoracle
{
    /// <summary>
    /// You need to build the solution to resolve OracleRequest class.
    /// </summary>
    [TestClass]
    public class OracleRequestTests : TestBase<OracleRequestTemplate>
    {
        [TestMethod]
        public void TestGetResponse()
        {
            Assert.IsNull(Contract.Response);
        }

        [TestMethod]
        public void TestDoRequestAndFinish()
        {
            // Check without being oracle

            Assert.ThrowsException<TestException>(() => Contract.OnOracleResponse(null, null, null, null));

            // Check empty

            Assert.IsNull(Contract.Response);

            // Create request

            string response = "";
            ulong? requestId = null;
            JToken data = JToken.Parse(@"
{
    ""id"": ""6520ad3c12a5d3765988542a"",
    ""record"": {
        ""propertyName"": ""Hello World!""
    }
}")!;

            Engine.Native.Oracle.OnOracleRequest += (id, requestContract, url, filter) =>
            {
                requestId = id;
                response = data.JsonPath(filter!).ToString(false);
            };

            Contract.DoRequest();
            Assert.IsNotNull(requestId);

            // Execute error

            Engine.Transaction.Attributes = new[]{
                new OracleResponse()
                {
                     Code = OracleResponseCode.Error,
                     Id = requestId.Value,
                     Result = Encoding.UTF8.GetBytes(response),
                }
            };
            Assert.ThrowsException<TestException>(Engine.Native.Oracle.Finish);

            // Execute finish

            Engine.Transaction.Attributes = new[]{
                new OracleResponse()
                {
                     Code = OracleResponseCode.Success,
                     Id = requestId.Value,
                     Result = Encoding.UTF8.GetBytes(response),
                }
            };

            Engine.Native.Oracle.Finish();

            // Check result

            Assert.AreEqual("Hello World!", Contract.Response);
        }
    }
}
