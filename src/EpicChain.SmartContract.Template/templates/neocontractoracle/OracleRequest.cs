using EpicChain.SmartContract;
using EpicChain.SmartContract.Framework;
using EpicChain.SmartContract.Framework.Attributes;
using EpicChain.SmartContract.Framework.Native;
using EpicChain.SmartContract.Framework.Services;

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace EpicChain.SmartContract.Template
{
    [DisplayName(nameof(OracleRequest))]
    [ContractAuthor("<Your Name Or Company Here>", "<Your Public Email Here>")]
    [ContractDescription( "<Description Here>")]
    [ContractVersion("<Version String Here>")]
    [ContractSourceCode("https://github.com/epicchainlabs/epicchain-devkit-dotnet/tree/master/src/EpicChain.SmartContract.Template/templates/epicchaincontractoracle/OracleRequest.cs")]
    [ContractPermission(Permission.Any, Method.Any)]
    public class OracleRequest : EpicChain.SmartContract.Framework.SmartContract
    {
        [Safe]
        public static string GetResponse()
        {
            return Storage.Get(Storage.CurrentContext, "Response");
        }

        public static void DoRequest()
        {
            /*
                JSON DATA EXAMPLE
                {
                    "id": "6520ad3c12a5d3765988542a",
                    "record": {
                        "propertyName": "Hello World!"
                    },
                    "metadata": {
                        "name": "HelloWorld",
                        "readCountRemaining": 98,
                        "timeToExpire": 86379,
                        "createdAt": "2023-10-07T00:58:36.746Z"
                    }
                }
                See JSONPath format at https://github.com/atifaziz/JSONPath
                JSONPath = "$.record.propertyName"
                ReturnValue = ["Hello World!"]
                ReturnValueType = string array
            */
            var requestUrl = "https://api.jsonbin.io/v3/qs/6520ad3c12a5d3765988542a";
            Oracle.Request(requestUrl, "$.record.propertyName", "onOracleResponse", null, Oracle.MinimumResponseFee);
        }

        /// <summary>
        /// This method is called after the Oracle receives response from requested URL
        /// </summary>
        /// <param name="requestedUrl">Requested url</param>
        /// <param name="userData">User data provided during the request</param>
        /// <param name="oracleResponse">Oracle response code</param>
        /// <param name="jsonString">Oracle response data</param>
        /// <exception cref="InvalidOperationException">It was not called by the oracle</exception>
        /// <exception cref="Exception">It was not a success</exception>
        public static void onOracleResponse(string requestedUrl, object userData, OracleResponseCode oracleResponse, string jsonString)
        {
            if (Runtime.CallingScriptHash != Oracle.Hash)
                throw new InvalidOperationException("No Authorization!");
            if (oracleResponse != OracleResponseCode.Success)
                throw new Exception("Oracle response failure with code " + (byte)oracleResponse);

            var jsonArrayValues = (object[])EssentialLib.JsonDeserialize(jsonString);
            var jsonFirstValue = (string)jsonArrayValues[0];

            Storage.Put(Storage.CurrentContext, "Response", jsonFirstValue);
        }
    }
}
