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
using System.Numerics;

namespace EpicChain.SmartContract.Framework.Native
{
    [Contract("0x6dc3bff7b2e6061f3cad5744edf307c14823328e")]
    public class NEO
    {
        [ContractHash]
        public static extern UInt160 Hash { get; }
        public static extern string Symbol { get; }
        public static extern byte Decimals { get; }
        public static extern BigInteger TotalSupply();
        public static extern BigInteger BalanceOf(UInt160 account);
        public static extern bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object data = null);

        public static extern BigInteger GetEpicPulsePerBlock();
        public static extern long GetRegisterPrice();
        public static extern BigInteger UnclaimedEpicPulse(UInt160 account, uint end);

        public static extern bool RegisterCandidate(ECPoint pubkey);
        public static extern bool UnRegisterCandidate(ECPoint pubkey);
        public static extern bool Vote(UInt160 account, ECPoint voteTo);
        public static bool Unvote(UInt160 account) => Vote(account, null);
        public static extern (ECPoint, BigInteger)[] GetCandidates();
        public static extern Iterator<(ECPoint, BigInteger)> GetAllCandidates();
        public static extern BigInteger GetCandidateVote(ECPoint pubkey);
        public static extern ECPoint[] GetCommittee();
        public static extern UInt160 GetCommitteeAddress();
        public static extern ECPoint[] GetNextBlockValidators();
        public static extern NeoAccountState GetAccountState(UInt160 account);
    }
}
