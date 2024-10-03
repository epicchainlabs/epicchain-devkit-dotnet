// Copyright (C) 2021-2024 EpicChain Lab's
//
// Contract1.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using EpicChain.SmartContract.Framework;
using EpicChain.SmartContract.Framework.Attributes;
using EpicChain.SmartContract.Framework.Native;
using System.ComponentModel;
using System.Numerics;
using EpicChain.SmartContract.Framework.Services;
using UInt160 = EpicChain.SmartContract.Framework.UInt160;

namespace Transfer;

/// <summary>
/// This is a sample contract that can be used as a template for creating new contracts.
/// </summary>
[DisplayName("SampleTransferContract")]
[ContractAuthor("code-dev", "devs@epic-chain.org")]
[ContractDescription("A sample contract to demonstrate how to transfer NEO and GAS")]
[ContractVersion("1.0.0.0")]
[ContractSourceCode("https://github.com/epicchainlabs/epicchain-devkit-dotnet/tree/master/examples/")]
[ContractPermission(Permission.Any, Method.Any)]
public class TransferContract : SmartContract
{
    private static readonly UInt160 Owner = "NUuJw4C4XJFzxAvSZnFTfsNoWZytmQKXQP";

    /// <summary>
    /// Transfer method that demonstrate how to transfer NEO and GAS
    /// </summary>
    /// <param name="to">Target address to send Neo and GAS</param>
    /// <param name="amount">Amount of tokens to be sent</param>
    public static void Transfer(UInt160 to, BigInteger amount)
    {
        ExecutionEngine.Assert(Runtime.CheckWitness(Owner));
        ExecutionEngine.Assert(NEO.Transfer(Runtime.ExecutingScriptHash, to, amount));
        ExecutionEngine.Assert(GAS.Transfer(Runtime.ExecutingScriptHash, to, GAS.BalanceOf(Runtime.ExecutingScriptHash), true));
    }
}
