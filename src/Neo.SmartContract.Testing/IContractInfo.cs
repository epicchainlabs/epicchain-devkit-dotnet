using Chain.SmartContract.Manifest;

namespace Chain.SmartContract.Testing;

public interface IContractInfo
{
    public static abstract NefFile Nef { get; }
    public static abstract ContractManifest Manifest { get; }
}
