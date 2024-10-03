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
using EpicChain.SmartContract.Framework.Services;

namespace EpicChain.SmartContract.Framework.Native
{
    [Contract("0x8fd7b7687ff40a5ddd6ea466a8787df2633ed3df")]
    public class Ledger
    {
        [ContractHash]
        public static extern UInt160 Hash { get; }
        public static extern UInt256 CurrentHash { get; }
        public static extern uint CurrentIndex { get; }
        public static extern Block GetBlock(uint index);
        public static extern Block GetBlock(UInt256 hash);
        public static extern Transaction GetTransaction(UInt256 hash);
        public static extern Transaction GetTransactionFromBlock(UInt256 blockHash, int txIndex);
        public static extern Transaction GetTransactionFromBlock(uint blockHeight, int txIndex);
        public static extern int GetTransactionHeight(UInt256 hash);
        public static extern Signer[] GetTransactionSigners(UInt256 hash);
        public static extern VMState GetTransactionVMState(UInt256 hash);
    }
}
