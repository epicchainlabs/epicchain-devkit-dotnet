using EpicChain.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace EpicChain.SmartContract.Testing;

public abstract class Contract_Optimize(EpicChain.SmartContract.Testing.SmartContractInitialize initialize) : EpicChain.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static EpicChain.SmartContract.Manifest.ContractManifest Manifest => EpicChain.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Optimize"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""unitTest_001"",""parameters"":[],""returntype"":""ByteArray"",""offset"":0,""safe"":false},{""name"":""testVoid"",""parameters"":[],""returntype"":""Void"",""offset"":14,""safe"":false},{""name"":""testArgs1"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""ByteArray"",""offset"":27,""safe"":false},{""name"":""testArgs2"",""parameters"":[{""name"":""a"",""type"":""ByteArray""}],""returntype"":""Any"",""offset"":48,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static EpicChain.SmartContract.NefFile Nef => EpicChain.IO.Helper.AsSerializable<EpicChain.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADVXAQAMBAECAwTbMHBoQFcBAAwEAQIDBNswcEBXAQEMBAECAwPbMHB4SmgTUdBFaEBXAAF4QH4bEdg="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testArgs1")]
    public abstract byte[]? TestArgs1(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testArgs2")]
    public abstract object? TestArgs2(byte[]? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testVoid")]
    public abstract void TestVoid();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_001")]
    public abstract byte[]? UnitTest_001();

    #endregion

}
