using System.ComponentModel;

namespace EpicChain.SmartContract.Testing.TestingStandards;

public interface IVerificable
{
    /// <summary>
    /// Safe property
    /// </summary>
    public bool? Verify { [DisplayName("verify")] get; }
}
