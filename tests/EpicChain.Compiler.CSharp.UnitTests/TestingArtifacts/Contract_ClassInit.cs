using EpicChain.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace EpicChain.SmartContract.Testing;

public abstract class Contract_ClassInit(EpicChain.SmartContract.Testing.SmartContractInitialize initialize) : EpicChain.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static EpicChain.SmartContract.Manifest.ContractManifest Manifest => EpicChain.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_ClassInit"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testInitInt"",""parameters"":[],""returntype"":""Array"",""offset"":0,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static EpicChain.SmartContract.NefFile Nef => EpicChain.IO.Helper.AsSerializable<EpicChain.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA/FShDPShDPSjQDQFcAAUA8l/ge"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testInitInt")]
    public abstract IList<object>? TestInitInt();

    #endregion

}
