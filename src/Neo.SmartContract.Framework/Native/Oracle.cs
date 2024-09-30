// Copyright (C) 2015-2024 The Neo Project.
//
// The Chain.SmartContract.Framework is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

#pragma warning disable CS0626

using Chain.SmartContract.Framework.Attributes;

namespace Chain.SmartContract.Framework.Native
{
    [Contract("0xfe924b7cfe89ddd271abaf7210a80a7e11178758")]
    public class Oracle
    {
        [ContractHash]
        public static extern UInt160 Hash { get; }
        public const uint MinimumResponseFee = 0_10000000;
        public static extern long GetPrice();
        public static extern void Request(string url, string filter, string callback, object userData, long gasForResponse);
    }
}
