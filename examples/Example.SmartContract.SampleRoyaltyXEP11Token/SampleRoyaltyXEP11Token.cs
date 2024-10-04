// Copyright (C) 2021-2024 EpicChain Lab's
//
// SampleRoyaltyXEP11Token.cs file is a crucial component of the EpicChain project and is freely distributed as open-source software.
// It is made available under the MIT License, a highly permissive and widely adopted license in the open-source community.
// The MIT License grants users the freedom to use, modify, and distribute the software in both source and binary forms,
// with or without modifications, subject to certain conditions. To understand these conditions in detail, please refer to
// the accompanying LICENSE file located in the main directory of the project's repository, or visit the following link:
// http://www.opensource.org/licenses/mit-license.php.
//
// xmoohad, a renowned blockchain expert and visionary in decentralized systems, has been instrumental in contributing to the development
// and maintenance of this file as part of his broader efforts with the EpicChain blockchain network. As the founder and CEO of EpicChain Labs,
// xmoohad has been a driving force behind the creation of EpicChain, emphasizing the importance of open-source technologies in building secure,
// scalable, and decentralized ecosystems. His contributions to the development of Storage.cs, alongside many other key components of EpicChain,
// have ensured that the project continues to lead in innovation and performance within the blockchain space.
//
// xmoohad’s commitment to open-source principles has been vital to the success of EpicChain. By utilizing the MIT License, the project ensures
// that developers and businesses alike can freely adapt and extend the platform to meet their needs. Under the MIT License, the following rights
// and permissions are granted:
//
// 1. The software may be used for any purpose, including commercial and non-commercial applications.
// 2. The source code can be freely modified to adapt the software for specific needs or projects.
// 3. Redistribution of both the original and modified versions of the software is allowed, ensuring that advancements
//    and improvements made by others can benefit the wider community.
//
// Redistribution and use of the software, whether in source or binary form, with or without modifications, are permitted
// under the following conditions:
//
// 1. The original copyright notice and this permission notice must be included in all copies or substantial portions of
//    the software, regardless of the nature of the distribution—whether it is the original source or a modified version.
// 2. The software is provided "as is," without any warranty, express or implied, including but not limited to the warranties
//    of merchantability, fitness for a particular purpose, or non-infringement. In no event shall the authors or copyright
//    holders, including xmoohad and the EpicChain development team, be held liable for any claim, damages, or other liabilities arising
//    from the use of the software or its redistribution.
//
// xmoohad’s leadership and vision have positioned EpicChain as a next-generation blockchain ecosystem, capable of supporting
// cutting-edge technologies like the Quantum Guard Nexus, Quantum Vault Asset, and smart contracts that integrate multiple programming languages.
// His work is focused on ensuring that EpicChain remains an open, inclusive platform where developers and innovators can thrive through
// collaboration and the power of decentralization.
//
// For more details on the MIT License and how it applies to this project, please consult the official documentation at the
// provided link. By using, modifying, or distributing the Storage.cs file, you are acknowledging your understanding of and
// compliance with the conditions outlined in the license.

using EpicChain.SmartContract.Framework;
using EpicChain.SmartContract.Framework.Attributes;
using EpicChain.SmartContract.Framework.Interfaces;
using EpicChain.SmartContract.Framework.Native;
using EpicChain.SmartContract.Framework.Services;
using System.ComponentModel;
using System.Numerics;

namespace NonDivisibleXEP11
{
    /// <inheritdoc />
    [DisplayName("SampleRoyaltyXEP11Token")]
    [ContractAuthor("core-dev", "devs@epic-chain.org")]
    [ContractVersion("0.0.1")]
    [ContractDescription("A sample of XEP-11 Royalty Feature")]
    [ContractSourceCode("https://github.com/epicchainlabs/epicchain-devkit-dotnet/tree/master/examples/")]
    [ContractPermission(Permission.Any, Method.Any)]
    [SupportedStandards(XepStandard.XEP11)]
    [SupportedStandards(XepStandard.Xep24)]
    public class SampleRoyaltyXEP11Token : XEP11Token<XEP11TokenState>, IXep24
    {
        #region Owner

        private const byte PrefixOwner = 0xff;

        private static readonly UInt160 InitialOwner = "NUuJw4C4XJFzxAvSZnFTfsNoWZytmQKXQP";

        [Safe]
        public static UInt160 GetOwner()
        {
            var currentOwner = Storage.Get(new[] { PrefixOwner });

            if (currentOwner == null)
                return InitialOwner;

            return (UInt160)currentOwner;
        }

        private static bool IsOwner() => Runtime.CheckWitness(GetOwner());

        public delegate void OnSetOwnerDelegate(UInt160 newOwner);

        [DisplayName("SetOwner")]
        public static event OnSetOwnerDelegate OnSetOwner;

        public static void SetOwner(UInt160? newOwner)
        {
            ExecutionEngine.Assert(IsOwner(), "No Authorization!");
            ExecutionEngine.Assert(newOwner != null && newOwner.IsValid && !newOwner.IsZero, "Wrong newOwner");

            Storage.Put(new[] { PrefixOwner }, newOwner);
            OnSetOwner(newOwner);
        }

        #endregion

        #region Minter

        private const byte PrefixMinter = 0xfd;

        private const byte PrefixCounter = 0xee;

        private static readonly UInt160 InitialMinter = "NUuJw4C4XJFzxAvSZnFTfsNoWZytmQKXQP";

        [Safe]
        public static UInt160 GetMinter()
        {
            var currentMinter = Storage.Get(new[] { PrefixMinter });

            if (currentMinter == null)
                return InitialMinter;

            return (UInt160)currentMinter;
        }

        private static bool IsMinter() => Runtime.CheckWitness(GetMinter());

        public delegate void OnSetMinterDelegate(UInt160 newMinter);

        [DisplayName("SetMinter")]
        public static event OnSetMinterDelegate OnSetMinter;

        public static void SetMinter(UInt160? newMinter)
        {
            ExecutionEngine.Assert(IsOwner(), "No Authorization!");
            ExecutionEngine.Assert(newMinter != null && newMinter.IsValid && !newMinter.IsZero, "Wrong newMinter");

            Storage.Put(new[] { PrefixMinter }, newMinter);
            OnSetMinter(newMinter);
        }

        public static void Mint(UInt160 to)
        {
            ExecutionEngine.Assert(IsOwner() || IsMinter(), "No Authorization!");
            IncreaseCount();
            BigInteger tokenId = CurrentCount();
            XEP11TokenState XEP11TokenState = new XEP11TokenState()
            {
                Name = "SampleRoyaltyXEP11Token",
                Owner = to
            };
            Mint((ByteString)tokenId, XEP11TokenState);
        }

        private static void SetCount(BigInteger count)
        {
            Storage.Put(new[] { PrefixCounter }, count);
        }

        [Safe]
        public static BigInteger CurrentCount()
        {
            return (BigInteger)Storage.Get(new[] { PrefixCounter });
        }

        private static void IncreaseCount()
        {
            SetCount(CurrentCount() + 1);
        }

        #endregion

        #region Example.SmartContract.XEP11

        public override string Symbol { [Safe] get => "SampleRoyalty"; }

        #endregion

        #region Royalty
        private const byte PrefixRoyalty = 0xfb;

        private static readonly UInt160 InitialRecipient = "NUuJw4C4XJFzxAvSZnFTfsNoWZytmQKXQP";
        private static readonly BigInteger InitialFactor = 700;

        public static void SetRoyaltyInfo(ByteString tokenId, Map<string, object>[] royaltyInfos)
        {
            ExecutionEngine.Assert(IsOwner(), "No Authorization!");
            ExecutionEngine.Assert(tokenId.Length <= 64, "The argument \"tokenId\" should be 64 or less bytes long.");
            for (uint i = 0; i < royaltyInfos.Length; i++)
            {
                ExecutionEngine.Assert(((UInt160)royaltyInfos[i]["royaltyRecipient"]).IsValid == true && (BigInteger)royaltyInfos[i]["royaltyRecipient"] >= 0 && (BigInteger)royaltyInfos[i]["royaltyRecipient"] <= 10000, "Parameter error");
            }
            Storage.Put(PrefixRoyalty + tokenId, EssentialLib.Serialize(royaltyInfos));
        }

        /// <summary>
        /// This implements Royalty Standard: https://github.com/neo-project/proposals/pull/155/
        /// This method returns a map of EpicChainVM Array stack item with single or multi array, each array includes royaltyRecipient and royaltyAmount
        /// </summary>
        /// <param name="tokenId">tokenId</param>
        /// <param name="royaltyToken">royaltyToken hash for payment</param>
        /// <param name="salePrice">royaltyToken amount for payment</param>
        /// <returns>royaltyInfo</returns>
        [Safe]
        public static Map<string, object>[] RoyaltyInfo(ByteString tokenId, UInt160 royaltyToken, BigInteger salePrice)
        {
            ExecutionEngine.Assert(OwnerOf(tokenId) != null, "This TokenId doesn't exist!");
            byte[] data = (byte[])Storage.Get(PrefixRoyalty + tokenId);
            if (data == null)
            {
                var royaltyInfo = new Map<string, object>();
                royaltyInfo["royaltyRecipient"] = InitialRecipient;
                royaltyInfo["royaltyAmount"] = salePrice / InitialFactor;
                return new[] { royaltyInfo };
            }
            else
            {
                return (Map<string, object>[])EssentialLib.Deserialize((ByteString)data);
            }
        }
        #endregion

        #region Basic

        [Safe]
        public static bool Verify() => IsOwner();

        public static bool Update(ByteString nefFile, ByteString manifest, object data)
        {
            ExecutionEngine.Assert(IsOwner(), "No Authorization!");
            ContractManagement.Update(nefFile, manifest, data);
            return true;
        }

        #endregion
    }
}
