using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace ntract.Testing;

public abstract class SampleEvent : ntract.Testing.SmartContract, IContractInfo
{
    #region Compiled data

    public static ntract.Manifest.ContractManifest ManifemartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleEvent"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""main"",""parameters"":[],""returntype"":""Boolean"",""offset"":0,""safe"":false}],""events"":[{""name"":""new_event_name"",""parameters"":[{""name"":""arg1"",""type"":""ByteArray""},{""name"":""arg2"",""type"":""String""},{""name"":""arg3"",""type"":""Integer""}]},{""name"":""event2"",""parameters"":[{""name"":""arg1"",""type"":""ByteArray""},{""name"":""arg2"",""type"":""Integer""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""code-dev"",""Description"":""A sample contract that demonstrates how to use Events"",""Version"":""0.0.1"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static ntract.NefFile Nef => Neo.IO.Helper.AsSerialimartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAERXAQAMAwECA9swcMJKaM9KDAJvac9KGs8MDm5ld19ldmVudF9uYW1lQZUBb2HCSmjPSgAyzwwGZXZlbnQyQZUBb2EJQAYoij8="));

    #endregion

    #region Events

    public delegate void delevent2(byte[]? arg1, BigInteger? arg2);

    [DisplayName("event2")]
    public event delevent2? OnEvent2;

    public delegate void delnew_event_name(byte[]? arg1, string? arg2, BigInteger? arg3);

    [DisplayName("new_event_name")]
    public event delnew_event_name? OnNew_event_name;

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("main")]
    public abstract bool? Main();

    #endregion

    #region Constructor for internal use only

    protected SampleEvent(ntract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
