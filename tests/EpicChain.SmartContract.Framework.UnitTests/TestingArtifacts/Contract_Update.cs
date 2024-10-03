using EpicChain.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace EpicChain.SmartContract.Testing;

public abstract class Contract_Update(EpicChain.SmartContract.Testing.SmartContractInitialize initialize) : EpicChain.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static EpicChain.SmartContract.Manifest.ContractManifest Manifest => EpicChain.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Update"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""imUpdated"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static EpicChain.SmartContract.NefFile Nef => EpicChain.IO.Helper.AsSerializable<EpicChain.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAYMA1lFU0DmhhMr"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("imUpdated")]
    public abstract string? ImUpdated();

    #endregion

}
