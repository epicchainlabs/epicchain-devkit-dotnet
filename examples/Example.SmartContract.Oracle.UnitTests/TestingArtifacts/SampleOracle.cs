using Chain.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Chain.SmartContract.Testing;

public abstract class SampleOracle : Chain.SmartContract.Testing.SmartContract, IContractInfo
{
    #region Compiled data

    public static Chain.SmartContract.Manifest.ContractManifest Manife> EpicChain.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleOracle"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""getResponse"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""doRequest"",""parameters"":[],""returntype"":""Void"",""offset"":21,""safe"":false},{""name"":""onOracleResponse"",""parameters"":[{""name"":""requestedUrl"",""type"":""String""},{""name"":""userData"",""type"":""Any""},{""name"":""oracleResponse"",""type"":""Integer""},{""name"":""jsonString"",""type"":""String""}],""returntype"":""Void"",""offset"":276,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""code-dev"",""Description"":""A sample contract to demonstrate how to use Example.SmartContract.Oracle Service"",""Version"":""0.0.1"",""Sourcecode"":""https://github.com/epicchainlabs/epicchain-devkit-dotnet/tree/master/examples/"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Chain.SmartContract.NefFile N> EpicChain.IO.Helper.AsSlizable<EpicChain.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAANYhxcRfgqoEHKvq3HS3Yn+fEuS/gdyZXF1ZXN0BQAAD8DvOc7g5OklxsKgannhRA3Yb86sBGl0b2EBAAEPwO85zuDk6SXGwqBqeeFEDdhvzqwPanNvbkRlc2VyaWFsaXplAQABDwAA/R4BDAhSZXNwb25zZUGb9mfOQZJd6DFAVwEADDVodHRwczovL2FwaS5qc29uYmluLmlvL3YzL3FzLzY1MjBhZDNjMTJhNWQzNzY1OTg4NTQyYXACgJaYAAsMEG9uT3JhY2xlUmVzcG9uc2UMFSQucmVjb3JkLnByb3BlcnR5TmFtZWg3AABAVwIFQTlTbjwMFFiHFxF+CqgQcq+rcdLdif58S5L+mCYWDBFObyBBdXRob3JpemF0aW9uITp7EJgmLgwiT3JhY2xlIHJlc3BvbnNlIGZhaWx1cmUgd2l0aCBjb2RlIHs3AQCL2yg6fDcCAHBoEM5xaQwIUmVzcG9uc2VBm/ZnzkHmPxiEQFcAAXg0A0BXAAFAwko08yNs////QKLVcGc="));

    #endregion

    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract string? Response { [DisplayName("getResponse")] get; }

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("doRequest")]
    public abstract void DoRequest();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("onOracleResponse")]
    public abstract void OnOracleResponse(string? requestedUrl, object? userData, BigInteger? oracleResponse, string? jsonString);

    #endregion

    #region Constructor for internal use only

    protected SampleOracle(Chain.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
