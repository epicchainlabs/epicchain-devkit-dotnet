using EpicChain.SmartContract.Framework;
using EpicChain.SmartContract.Framework.Attributes;

namespace EpicChain.Compiler.CSharp.TestContracts;

[ContractPermission("0x01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4", "a", "b", "c")]
[ContractPermission("0x01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4", "*")]
public class Contract_ABIAttributes3 : SmartContract.Framework.SmartContract
{
    public static int test() => 0;
}
