using EpicChain.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace EpicChain.SmartContract.Testing;

public abstract class Contract_Event(EpicChain.SmartContract.Testing.SmartContractInitialize initialize) : EpicChain.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static EpicChain.SmartContract.Manifest.ContractManifest Manifest => EpicChain.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Event"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""test"",""parameters"":[],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":42,""safe"":false}],""events"":[{""name"":""transfer"",""parameters"":[{""name"":""arg1"",""type"":""ByteArray""},{""name"":""arg2"",""type"":""ByteArray""},{""name"":""arg3"",""type"":""Integer""}]}]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static EpicChain.SmartContract.NefFile Nef => EpicChain.IO.Helper.AsSerializable<EpicChain.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAC0XYAhhwkoMAwECA9swz0oMAwQFBtswz0pYzwwIdHJhbnNmZXJBlQFvYUBWAkDaSVoX"));

    #endregion

    #region Events

    public delegate void deltransfer(byte[]? arg1, byte[]? arg2, BigInteger? arg3);

    [DisplayName("transfer")]
    public event deltransfer? OnTransfer;

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("test")]
    public abstract void Test();

    #endregion

}
