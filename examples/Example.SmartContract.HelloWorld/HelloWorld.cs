// Copyright (C) 2021-2024 EpicChain Lab's
//
// HelloWorld.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using EpicChain.SmartContract.Framework;
using EpicChain.SmartContract.Framework.Attributes;

using System.ComponentModel;

namespace HelloWorld;

[DisplayName("SampleHelloWorld")]
[ContractDescription("A simple `hello world` contract")]
[ContractEmail("devs@epic-chain.org")]
[ContractVersion("0.0.1")]
[ContractSourceCode("https://github.com/epicchainlabs/epicchain-devkit-dotnet/tree/master/examples/")]
[ContractPermission(Permission.Any, Method.Any)]
public class HelloWorldorld : SmartContract
{
    /// <summary>
    /// Hello world from NEO!
    /// </summary>
    /// <returns>Hello world string</returns>
    [Safe]
    public static string SayHello()
    {
        return "Hello, World!";
    }
}
