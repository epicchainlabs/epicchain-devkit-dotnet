using EpicChain.Cryptography.ECC;
using EpicChain.SmartContract.Native;
using System.ComponentModel;
using System.Numerics;

namespace EpicChain.SmartContract.Testing.Native;

public abstract class QuantumGuardNexus : SmartContract
{
    #region Compiled data

    public static Manifest.ContractManifest Manifest { get; } =
        NativeContract.QuantumGuardNexus.GetContractState(ProtocolSettings.Default, uint.MaxValue).Manifest;

    #endregion

    #region Events

    public delegate void delDesignation(BigInteger Role, BigInteger BlockIndex);

    [DisplayName("Designation")]
#pragma warning disable CS0067 // Event is never used
    public event delDesignation? OnDesignation;
#pragma warning restore CS0067 // Event is never used

    #endregion

    #region Safe methods

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getDesignatedByRole")]
    public abstract ECPoint[] GetDesignatedByRole(Role role, uint index);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("designateAsRole")]
    public abstract void DesignateAsRole(Role role, ECPoint[] nodes);

    #endregion

    #region Constructor for internal use only

    protected QuantumGuardNexus(SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
