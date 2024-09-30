// Copyright (C) 2021-2024 EpicChain Labs.
//
// ContractManifest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace EpicChain.SmartContract.Framework.Services
{
    public struct ContractManifest
    {
        public string Name;
        public ContractGroup[] Groups;
        public readonly object Reserved;
        public string[] SupportedStandards;
        public ContractAbi Abi;
        public ContractPermission[] Permissions;
        public ByteString[] Trusts;
        public string Extra;
    }
}
