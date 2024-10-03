using EpicChain.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace EpicChain.SmartContract.Testing;

public abstract class SampleContractCall : EpicChain.SmartContract.Testing.SmartContract, IContractInfo
{
    #region Compiled data

    public static EpicChain.SmartContract.Manifest.ContractManifest Manifest => EpicChain.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleContractCall"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""onNEP17Payment"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Integer""}],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":73,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Author"":""core-dev"",""Version"":""0.0.1"",""Description"":""A sample contract to demonstrate how to call a contract"",""Sourcecode"":""https://github.com/epicchainlabs/epicchain-devkit-dotnet/tree/master/examples/"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static EpicChain.SmartContract.NefFile Nef => EpicChain.IO.Helper.AsSerializable<EpicChain.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAGNXAwN6AHuzqiYEIj5B2/6odHBBOVNuPHFoEcAfDAliYWxhbmNlT2ZpQWJ9W1JyamloE8AfDAtkdW1teU1ldGhvZFhBYn1bUkVAVgEMFF6QEGiCvFczbXZ7FdXtLpwFPqgTYEBwobBz"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("onNEP17Payment")]
    public abstract void OnNEP17Payment(UInt160? from, BigInteger? amount, BigInteger? data);

    #endregion

    #region Constructor for internal use only

    protected SampleContractCall(EpicChain.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
