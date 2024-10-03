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
    [Contract("0x49cf4e5378ffcd4dec034fd98a174c5491e395e2")]
    public class RoleManagement
    {
        [ContractHash]
        public static extern UInt160 Hash { get; }
        public static extern ECPoint[] GetDesignatedByRole(Role role, uint index);
    }
}
