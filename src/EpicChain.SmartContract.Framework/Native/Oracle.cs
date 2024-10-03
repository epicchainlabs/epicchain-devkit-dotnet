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

namespace EpicChain.SmartContract.Framework.Native
{
    [Contract("0xf95f1e73b6b852e0cdf1535d5371d211707a2d95")]
    public class Oracle
    {
        [ContractHash]
        public static extern UInt160 Hash { get; }
        public const uint MinimumResponseFee = 0_10000000;
        public static extern long GetPrice();
        public static extern void Request(string url, string filter, string callback, object userData, long gasForResponse);
    }
}
