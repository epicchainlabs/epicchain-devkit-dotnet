using EpicChain.SmartContract.Manifest;

namespace EpicChain.SmartContract.Testing;

public interface IContractInfo
{
    public static abstract NefFile Nef { get; }
    public static abstract ContractManifest Manifest { get; }
}
