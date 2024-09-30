using Chain.SmartContract.Framework;
using Chain.SmartContract.Framework.Attributes;

namespace Chain.Compiler.CSharp.TestContracts;

[ContractPermission(Permission.Any, "c")]
[ContractPermission("0x01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4", "a", "b")]
[ContractPermission("0x01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4")]
[ContractPermission("0x01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4", Permission.Any)]
[ContractPermission("0x01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4", "*")]
[ContractPermission("0x01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4", "a")]
[ContractPermission("*", "a")]
[ContractPermission(Permission.Any, Method.Any)]
[ContractPermission("*", "b")]
[ContractTrust("0x0a0b00ff00ff00ff00ff00ff00ff00ff00ff00a4")]
public class Contract_ABIAttributes2 : SmartContract.Framework.SmartContract
{
    public static int test() => 0;
}
