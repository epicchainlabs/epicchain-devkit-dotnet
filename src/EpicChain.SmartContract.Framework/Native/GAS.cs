// Copyright (C) 2021-2024 EpicChain Lab's
//
// The EpicChain.SmartContract.Framework is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

#pragma warning disable CS0626

using EpicChain.SmartContract.Framework.Attributes;
using System.Numerics;

namespace EpicChain.SmartContract.Framework.Native
{
    [Contract("0xbc8459660544656355b4f60861c22f544341e828")]
    public class GAS
    {
        [ContractHash]
        public static extern UInt160 Hash { get; }
        public static extern string Symbol { get; }
        public static extern byte Decimals { get; }
        public static extern BigInteger TotalSupply();
        public static extern BigInteger BalanceOf(UInt160 account);
        public static extern bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object data = null);
    }
}
