// Copyright (C) 2021-2024 EpicChain Labs.
//
// The EpicChain.SmartContract.Framework is open-source software made available under the MIT License.
// This permissive license allows anyone to freely use, modify, and distribute both the source code
// and binary forms of the software, either with or without modifications, provided that the conditions
// specified in the license are met. For more information about the MIT License, you can refer to the
// LICENSE file located in the main directory of the project, or visit the following link:
// http://www.opensource.org/licenses/mit-license.php.
//
// The key permissions granted by the MIT License include the following:
// 1. The right to use the software for any purpose, including commercial applications.
// 2. The right to modify the source code to suit individual needs.
// 3. The right to distribute copies of the original or modified versions of the software.
//
// Redistribution and use in both source and binary forms are permitted, provided that the following
// conditions are met:
//
// 1. The original copyright notice and permission notice must be included in all copies or substantial
//    portions of the software, whether the distribution is of the unmodified source code or modified
//    versions.
// 2. The software is provided "as is," without any warranty of any kind, express or implied, including
//    but not limited to the warranties of merchantability, fitness for a particular purpose, or
//    non-infringement. In no event shall the authors or copyright holders be liable for any claim,
//    damages, or other liabilities, whether in an action of contract, tort, or otherwise, arising from,
//    out of, or in connection with the software or the use or other dealings in the software.
//
// The MIT License is widely used for open-source projects because of its flexibility, encouraging both
// individual and corporate use of the licensed software without restrictive obligations. If you wish to
// learn more about this license or its implications for the project, please consult the official page
// provided above.


#pragma warning disable CS0626

using EpicChain.SmartContract.Framework.Attributes;
using EpicChain.SmartContract.Framework.Services;
using System.Numerics;

namespace EpicChain.SmartContract.Framework.Native
{
    [Contract("0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5")]
    public class NEO
    {
        [ContractHash]
        public static extern UInt160 Hash { get; }
        public static extern string Symbol { get; }
        public static extern byte Decimals { get; }
        public static extern BigInteger TotalSupply();
        public static extern BigInteger BalanceOf(UInt160 account);
        public static extern bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object data = null);

        public static extern BigInteger GetGasPerBlock();
        public static extern long GetRegisterPrice();
        public static extern BigInteger UnclaimedGas(UInt160 account, uint end);

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
