// Copyright (C) 2021-2024 EpicChain Lab's
//
// The EpicChain.SmartContract.Framework is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace EpicChain.SmartContract.Framework.Services
{
    public class Block
    {
        public readonly UInt256 Hash;
        public readonly uint Version;
        public readonly UInt256 PrevHash;
        public readonly UInt256 MerkleRoot;
        public readonly ulong Timestamp;
        public readonly ulong Nonce;
        public readonly uint Index;
        public readonly byte PrimaryIndex;
        public readonly UInt160 NextConsensus;
        public readonly int TransactionsCount;
    }
}
