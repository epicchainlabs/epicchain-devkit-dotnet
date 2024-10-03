using EpicChain.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace EpicChain.SmartContract.Testing;

public abstract class Contract_Partial(EpicChain.SmartContract.Testing.SmartContractInitialize initialize) : EpicChain.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static EpicChain.SmartContract.Manifest.ContractManifest Manifest => EpicChain.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Partial"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""test1"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""test2"",""parameters"":[],""returntype"":""Integer"",""offset"":2,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static EpicChain.SmartContract.NefFile Nef => EpicChain.IO.Helper.AsSerializable<EpicChain.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQRQBJAbXCJZw=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("test1")]
    public abstract BigInteger? Test1();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("test2")]
    public abstract BigInteger? Test2();

    #endregion

}
