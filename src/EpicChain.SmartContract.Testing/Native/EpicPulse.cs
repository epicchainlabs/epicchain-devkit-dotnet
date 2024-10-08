using EpicChain.SmartContract.Native;
using System.ComponentModel;
using System.Numerics;

namespace EpicChain.SmartContract.Testing.Native;

public abstract class EpicPulse : SmartContract, TestingStandards.IXep17Standard
{
    #region Compiled data

    public static Manifest.ContractManifest Manifest { get; } =
        NativeContract.EpicPulse.GetContractState(ProtocolSettings.Default, uint.MaxValue).Manifest;

    #endregion

    #region Events
#pragma warning disable CS0067 // Event is never used
    [DisplayName("Transfer")]
    public event TestingStandards.IXep17Standard.delTransfer? OnTransfer;
#pragma warning restore CS0067 // Event is never used
    #endregion

    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? Decimals { [DisplayName("decimals")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract string? Symbol { [DisplayName("symbol")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? TotalSupply { [DisplayName("totalSupply")] get; }

    #endregion

    #region Safe methods

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("balanceOf")]
    public abstract BigInteger? BalanceOf(UInt160? account);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? from, UInt160? to, BigInteger? amount, object? data = null);

    #endregion

    #region Constructor for internal use only

    protected EpicPulse(SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
