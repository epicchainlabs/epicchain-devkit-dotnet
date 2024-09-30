// Copyright (C) 2015-2024 The Neo Project.
//
// The Chain.SmartContract.Framework is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Chain.SmartContract.Framework.Native
{
    public class Signer
    {
        public UInt160 Account;
        public WitnessScope Scopes;
        public UInt160[] AllowedContracts;
        public ECPoint[] AllowedGroups;
        public WitnessRule[] Rules;
    }
}
