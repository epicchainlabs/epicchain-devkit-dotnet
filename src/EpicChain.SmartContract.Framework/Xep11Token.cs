// Copyright (C) 2021-2024 EpicChain Lab's
//
// The EpicChain.SmartContract.Framework is open-source software that is distributed under the widely recognized and permissive MIT License.
// This software is intended to provide developers with a powerful framework to create and deploy smart contracts on the EpicChain blockchain,
// and it is made freely available to all individuals and organizations. Whether you are building for personal, educational, or commercial
// purposes, you are welcome to utilize this framework with minimal restrictions, promoting the spirit of open innovation and collaborative
// development within the blockchain ecosystem.
//
// As a permissive license, the MIT License allows for broad usage rights, granting you the freedom to redistribute, modify, and adapt the
// source code or its binary versions as needed. You are permitted to incorporate the EpicChain Lab's Project into your own
// projects, whether for profit or non-profit, and may make changes to suit your specific needs. There is no requirement to make your
// modifications open-source, though doing so contributes to the overall growth of the open-source community.
//
// To comply with the terms of the MIT License, we kindly ask that you include a copy of the original copyright notice, this permission
// notice, and the license itself in all substantial portions of the software that you redistribute, whether in source code form or as
// compiled binaries. The purpose of this requirement is to ensure that future users and developers are aware of the origin of the software,
// the freedoms granted to them, and the limitations of liability that apply.
//
// The complete terms and conditions of the MIT License are documented in the LICENSE file that accompanies this project. This file can be
// found in the main directory of the source code repository. Alternatively, you may access the license text online at the following URL:
// http://www.opensource.org/licenses/mit-license.php. We encourage you to review these terms in detail to fully understand your rights
// and responsibilities when using this software.
//
// Redistribution and use of the EpicChain Lab's Project, whether in source or binary forms, with or without modification, are
// permitted as long as the following conditions are met:
//
// 1. The original copyright notice, along with this permission notice, must be retained in all copies or significant portions of the software.
// 2. The software is provided "as-is," without any express or implied warranty. This means that the authors and contributors are not
//    responsible for any issues that may arise from the use of the software, including but not limited to damages caused by defects or
//    performance issues. Users assume all responsibility for determining the suitability of the software for their specific needs.
//
// In addition to the above terms, the authors of the EpicChain Lab's Project encourage developers to explore and experiment
// with the framework's capabilities. Whether you are an individual developer, a startup, or a large organization, you are invited to
// leverage the power of blockchain technology to create decentralized applications, smart contracts, and more. We believe that by fostering
// a robust ecosystem of developers and contributors, we can help drive innovation in the blockchain space and unlock new opportunities
// for distributed ledger technology.
//
// However, please note that while the MIT License allows for modifications and redistribution, it does not imply endorsement of any
// derived works by the original authors. Therefore, if you significantly modify the EpicChain Lab's Project and redistribute it
// under your own brand or as part of a larger project, you must clearly indicate the changes you have made, and the original authors
// cannot be held liable for any issues resulting from your modifications.
//
// By choosing to use the EpicChain Lab's Project, you acknowledge that you have read and understood the terms of the MIT License.
// You agree to abide by these terms and recognize that this software is provided without warranty of any kind, express or implied, including
// but not limited to warranties of merchantability, fitness for a particular purpose, or non-infringement. Should any legal issues or
// disputes arise as a result of using this software, the authors and contributors disclaim all liability and responsibility.
//
// Finally, we encourage all users of the EpicChain Lab's Project to consider contributing back to the community. Whether through
// bug reports, feature suggestions, or code contributions, your involvement helps improve the framework for everyone. Open-source projects
// thrive when developers collaborate and share their knowledge, and we welcome your input as we continue to develop and refine the
// EpicChain ecosystem.


using EpicChain.SmartContract.Framework.Attributes;
using EpicChain.SmartContract.Framework.Native;
using EpicChain.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace EpicChain.SmartContract.Framework
{
    [SupportedStandards(XepStandard.XEP11)]
    [ContractPermission(Permission.Any, Method.OnXEP11Payment)]
    public abstract class XEP11Token<TokenState> : TokenContract
        where TokenState : XEP11TokenState
    {
        public delegate void OnTransferDelegate(UInt160 from, UInt160 to, BigInteger amount, ByteString tokenId);

        [DisplayName("Transfer")]
        public static event OnTransferDelegate OnTransfer;

        protected const byte Prefix_TokenId = 0x02;
        protected const byte Prefix_Token = 0x03;
        protected const byte Prefix_AccountToken = 0x04;

        public sealed override byte Decimals
        {
            [Safe]
            get => 0;
        }

        [Safe]
        public static UInt160 OwnerOf(ByteString tokenId)
        {
            if (tokenId.Length > 64) throw new Exception("The argument \"tokenId\" should be 64 or less bytes long.");
            var tokenMap = new StorageMap(Prefix_Token);
            var tokenKey = tokenMap[tokenId] ?? throw new Exception("The token with given \"tokenId\" does not exist.");
            TokenState token = (TokenState)EssentialLib.Deserialize(tokenKey);
            return token.Owner;
        }

        [Safe]
        public virtual Map<string, object> Properties(ByteString tokenId)
        {
            var tokenMap = new StorageMap(Prefix_Token);
            TokenState token = (TokenState)EssentialLib.Deserialize(tokenMap[tokenId]);
            return new Map<string, object>()
            {
                ["name"] = token.Name
            };
        }

        [Safe]
        public static Iterator Tokens()
        {
            var tokenMap = new StorageMap(Prefix_Token);
            return tokenMap.Find(FindOptions.KeysOnly | FindOptions.RemovePrefix);
        }

        [Safe]
        public static Iterator TokensOf(UInt160 owner)
        {
            if (owner is null || !owner.IsValid)
                throw new Exception("The argument \"owner\" is invalid");
            var accountMap = new StorageMap(Prefix_AccountToken);
            return accountMap.Find(owner, FindOptions.KeysOnly | FindOptions.RemovePrefix);
        }

        public static bool Transfer(UInt160 to, ByteString tokenId, object data)
        {
            if (to is null || !to.IsValid)
                throw new Exception("The argument \"to\" is invalid.");
            var tokenMap = new StorageMap(Prefix_Token);
            TokenState token = (TokenState)EssentialLib.Deserialize(tokenMap[tokenId]);
            UInt160 from = token.Owner;
            if (!Runtime.CheckWitness(from)) return false;
            if (from != to)
            {
                token.Owner = to;
                tokenMap[tokenId] = EssentialLib.Serialize(token);
                UpdateBalance(from, tokenId, -1);
                UpdateBalance(to, tokenId, +1);
            }
            PostTransfer(from, to, tokenId, data);
            return true;
        }

        protected static ByteString NewTokenId()
        {
            return NewTokenId(Runtime.ExecutingScriptHash);
        }

        protected static ByteString NewTokenId(ByteString salt)
        {
            StorageContext context = Storage.CurrentContext;
            byte[] key = new byte[] { Prefix_TokenId };
            ByteString id = Storage.Get(context, key);
            Storage.Put(context, key, (BigInteger)id + 1);
            if (id is not null) salt += id;
            return CryptoHive.Sha256(salt);
        }

        protected static void Mint(ByteString tokenId, TokenState token)
        {
            StorageMap tokenMap = new(Storage.CurrentContext, Prefix_Token);
            tokenMap[tokenId] = EssentialLib.Serialize(token);
            UpdateBalance(token.Owner, tokenId, +1);
            TotalSupply++;
            PostTransfer(null, token.Owner, tokenId, null);
        }

        protected static void Burn(ByteString tokenId)
        {
            StorageMap tokenMap = new(Storage.CurrentContext, Prefix_Token);
            TokenState token = (TokenState)EssentialLib.Deserialize(tokenMap[tokenId]);
            tokenMap.Delete(tokenId);
            UpdateBalance(token.Owner, tokenId, -1);
            TotalSupply--;
            PostTransfer(token.Owner, null, tokenId, null);
        }

        protected static void UpdateBalance(UInt160 owner, ByteString tokenId, int increment)
        {
            UpdateBalance(owner, increment);
            StorageMap accountMap = new(Storage.CurrentContext, Prefix_AccountToken);
            ByteString key = owner + tokenId;
            if (increment > 0)
                accountMap.Put(key, 0);
            else
                accountMap.Delete(key);
        }

        protected static void PostTransfer(UInt160 from, UInt160 to, ByteString tokenId, object data)
        {
            OnTransfer(from, to, 1, tokenId);
            if (to is not null && ContractManagement.GetContract(to) is not null)
                Contract.Call(to, "OnXEP11Payment", CallFlags.All, from, 1, tokenId, data);
        }
    }
}
