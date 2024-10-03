using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace ntract.Testing;

public abstract class SampleHelloWorld : ntract.Testing.SmartContract, IContractInfo
{
    #region Compiled data

    public static ntract.Manifest.ContractManifest ManifemartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleHelloWorld"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""sayHello"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true}],""events"":[]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Description"":""A simple \u0060hello world\u0060 contract"",""E-mail"":""dev@neo.org"",""Version"":""0.0.1"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static ntract.NefFile Nef => Neo.IO.Helper.AsSerialimartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABAMDUhlbGxvLCBXb3JsZCFATVW3pg=="));

    #endregion

    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract string? SayHello { [DisplayName("sayHello")] get; }

    #endregion

    #region Constructor for internal use only

    protected SampleHelloWorld(ntract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
