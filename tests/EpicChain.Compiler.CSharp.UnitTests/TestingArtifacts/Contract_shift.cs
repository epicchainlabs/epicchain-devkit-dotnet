using EpicChain.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace EpicChain.SmartContract.Testing;

public abstract class Contract_shift(EpicChain.SmartContract.Testing.SmartContractInitialize initialize) : EpicChain.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static EpicChain.SmartContract.Manifest.ContractManifest Manifest => EpicChain.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_shift"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testShift"",""parameters"":[],""returntype"":""Array"",""offset"":0,""safe"":false},{""name"":""testShiftBigInt"",""parameters"":[],""returntype"":""Array"",""offset"":64,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static EpicChain.SmartContract.NefFile Nef => EpicChain.IO.Helper.AsSerializable<EpicChain.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFxXAwAYcGgRqEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xaBGpcmppEsBAVwUAGHBoEKhxaBGocmgRqXNoEql0bGtqaRTAQMjG+94="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testShift")]
    public abstract IList<object>? TestShift();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testShiftBigInt")]
    public abstract IList<object>? TestShiftBigInt();

    #endregion

}
