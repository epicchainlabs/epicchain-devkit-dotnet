using Chain.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Chain.SmartContract.Testing;

public abstract class SampleHelloWorld : Chain.SmartContract.Testing.SmartContract, IContractInfo
{
    #region Compiled data

    public static Chain.SmartContract.Manifest.ContractManifest Manife> EpicChain.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleHelloWorld"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""sayHello"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true}],""events"":[]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Description"":""A simple \u0060hello world\u0060 contract"",""E-m":""dev@EpicChain.org"",""Version"":""0.0.1"",""Sourcecode"":""https://github.com/epicchainlabs/epicchain-devkit-dotnet/tree/master/examples/"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Chain.SmartContract.NefFile N> EpicChain.IO.Helper.AsSlizable<EpicChain.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABAMDUhlbGxvLCBXb3JsZCFATVW3pg=="));

    #endregion

    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract string? SayHello { [DisplayName("sayHello")] get; }

    #endregion

    #region Constructor for internal use only

    protected SampleHelloWorld(Chain.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
