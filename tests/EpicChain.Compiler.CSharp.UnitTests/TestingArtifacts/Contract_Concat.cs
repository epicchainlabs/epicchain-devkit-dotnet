using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace EpicChain.SmartContract.Testing;

public abstract class Contract_Concat(EpicChain.SmartContract.Testing.SmartContractInitialize initialize) : EpicChain.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static EpicChain.SmartContract.Manifest.ContractManifest Manifest => EpicChain.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Concat"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testStringAdd1"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""String"",""offset"":0,""safe"":false},{""name"":""testStringAdd2"",""parameters"":[{""name"":""a"",""type"":""String""},{""name"":""b"",""type"":""String""}],""returntype"":""String"",""offset"":15,""safe"":false},{""name"":""testStringAdd3"",""parameters"":[{""name"":""a"",""type"":""String""},{""name"":""b"",""type"":""String""},{""name"":""c"",""type"":""String""}],""returntype"":""String"",""offset"":34,""safe"":false},{""name"":""testStringAdd4"",""parameters"":[{""name"":""a"",""type"":""String""},{""name"":""b"",""type"":""String""},{""name"":""c"",""type"":""String""},{""name"":""d"",""type"":""String""}],""returntype"":""String"",""offset"":57,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static EpicChain.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<EpicChain.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFRXAAF4DAVoZWxsb4vbKEBXAAJ4eYvbKAwFaGVsbG+L2yhAVwADeHmL2yh6i9soDAVoZWxsb4vbKEBXAAR4eYvbKHqL2yh7i9soDAVoZWxsb4vbKEDmSoTR"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testStringAdd1")]
    public abstract string? TestStringAdd1(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testStringAdd2")]
    public abstract string? TestStringAdd2(string? a, string? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testStringAdd3")]
    public abstract string? TestStringAdd3(string? a, string? b, string? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testStringAdd4")]
    public abstract string? TestStringAdd4(string? a, string? b, string? c, string? d);

    #endregion

}
