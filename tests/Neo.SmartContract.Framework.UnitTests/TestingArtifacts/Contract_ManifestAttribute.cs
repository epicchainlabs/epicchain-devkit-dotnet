using EpicChain.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace EpicChain.SmartContract.Testing;

public abstract class Contract_ManifestAttribute(EpicChain.SmartContract.Testing.SmartContractInitialize initialize) : EpicChain.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static EpicChain.SmartContract.Manifest.ContractManifest Manifest => EpicChain.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_ManifestAttribute"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""reentrantTest"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":207,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":191,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Author"":""core-dev"",""E-mail"":""dev@EpicChain.org"",""Version"":""v3.6.3"",""Description"":""This is a test contract."",""ExtraKey"":""ExtraValue"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static EpicChain.SmartContract.NefFile Nef => EpicChain.IO.Helper.AsSerializable<EpicChain.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAANlXAAJY2CYeCwsSwEpZzwwLbm9SZWVudHJhbnQB/wASTTQeYFg0QXkQlyYEIgx5AHuXJgYQeDTLWDVoAAAAQFcAA3g0H0Gb9mfOeRGIThBR0FASwEp4EFHQRXpKeBFR0EVAVwABQFcBAXgRzngQzsFFU4tQQZJd6DFwaAuXDA9BbHJlYWR5IGVudGVyZWThEXgRzngQzsFFU4tQQeY/GIRAVwABeBHOeBDOwUVTi1BBL1jF7UBXAAF4NANAVwABQFYCCt////8Kn////xLAYUDCSjTjIy3///9AZXJBLQ=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("reentrantTest")]
    public abstract void ReentrantTest(BigInteger? value);

    #endregion

}
