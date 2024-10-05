// Copyright (C) 2021-2024 EpicChain Lab's
//
// The EpicChain.SmartContract.Framework  MIT License allows for broad usage rights, granting you the freedom to redistribute, modify, and adapt the
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


#pragma warning disable CS0169
#pragma warning disable IDE0051

using EpicChain.SmartContract.Framework.Attributes;
using EpicChain.SmartContract.Framework.Native;
using System.Numerics;
using System.Runtime.InteropServices;

namespace EpicChain.SmartContract.Framework.Services
{
    public class StorageMap
    {
        private readonly StorageContext context;
        private readonly byte[] prefix;

        public extern ByteString this[ByteString key]
        {
            [CallingConvention(CallingConvention.Cdecl)]
            [OpCode(OpCode.UNPACK)]
            [OpCode(OpCode.DROP)]
            [OpCode(OpCode.REVERSE3)]
            [OpCode(OpCode.CAT)]
            [OpCode(OpCode.SWAP)]
            [Syscall("System.Storage.Get")]
            get;
            [CallingConvention(CallingConvention.Cdecl)]
            [OpCode(OpCode.UNPACK)]
            [OpCode(OpCode.DROP)]
            [OpCode(OpCode.REVERSE3)]
            [OpCode(OpCode.CAT)]
            [OpCode(OpCode.SWAP)]
            [Syscall("System.Storage.Put")]
            set;
        }

        public extern ByteString this[byte[] key]
        {
            [CallingConvention(CallingConvention.Cdecl)]
            [OpCode(OpCode.UNPACK)]
            [OpCode(OpCode.DROP)]
            [OpCode(OpCode.REVERSE3)]
            [OpCode(OpCode.CAT)]
            [OpCode(OpCode.SWAP)]
            [Syscall("System.Storage.Get")]
            get;
            [CallingConvention(CallingConvention.Cdecl)]
            [OpCode(OpCode.UNPACK)]
            [OpCode(OpCode.DROP)]
            [OpCode(OpCode.REVERSE3)]
            [OpCode(OpCode.CAT)]
            [OpCode(OpCode.SWAP)]
            [Syscall("System.Storage.Put")]
            set;
        }

        [Syscall("System.Storage.GetContext")]
        [OpCode(OpCode.PUSH2)]
        [OpCode(OpCode.PACK)]
        public extern StorageMap(byte[] prefix);

        [Syscall("System.Storage.GetContext")]
        [OpCode(OpCode.PUSH2)]
        [OpCode(OpCode.PACK)]
        public extern StorageMap(ByteString prefix);

        [OpCode(OpCode.PUSH1)]
        [OpCode(OpCode.NEWBUFFER)]
        [OpCode(OpCode.TUCK)]
        [OpCode(OpCode.PUSH0)]
        [OpCode(OpCode.ROT)]
        [OpCode(OpCode.SETITEM)]
        [Syscall("System.Storage.GetContext")]
        [OpCode(OpCode.PUSH2)]
        [OpCode(OpCode.PACK)]
        public extern StorageMap(byte prefix);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.PUSH2)]
        [OpCode(OpCode.PACK)]
        public extern StorageMap(StorageContext context, byte[] prefix);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.PUSH2)]
        [OpCode(OpCode.PACK)]
        public extern StorageMap(StorageContext context, ByteString prefix);

        [OpCode(OpCode.PUSH1)]
        [OpCode(OpCode.NEWBUFFER)]
        [OpCode(OpCode.TUCK)]
        [OpCode(OpCode.PUSH0)]
        [OpCode(OpCode.ROT)]
        [OpCode(OpCode.SETITEM)]
        [OpCode(OpCode.SWAP)]
        [OpCode(OpCode.PUSH2)]
        [OpCode(OpCode.PACK)]
        public extern StorageMap(StorageContext context, byte prefix);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        public extern ByteString Get(ByteString key);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        public extern UInt160 GetUInt160(ByteString key);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        public extern UInt256 GetUInt256(ByteString key);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        public extern ECPoint GetECPoint(ByteString key);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        [OpCode(OpCode.CONVERT, StackItemType.Buffer)]
        public extern byte[] GetByteArray(ByteString key);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        public extern ByteString GetString(ByteString key);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        [OpCode(OpCode.CONVERT, StackItemType.Integer)]
        public extern BigInteger GetInteger(ByteString key);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        [OpCode(OpCode.NOT)]
        [OpCode(OpCode.NOT)]
        public extern bool GetBoolean(ByteString key);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.ISNULL)]
        [OpCode(OpCode.JMPIFNOT, "0x06")]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.PUSH0)]
        [OpCode(OpCode.JMP, "0x04")]
        [OpCode(OpCode.CONVERT, StackItemType.Integer)]
        public extern BigInteger GetIntegerOrZero(ByteString key);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        public extern ByteString Get(byte[] key);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        public extern UInt160 GetUInt160(byte[] key);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        public extern UInt256 GetUInt256(byte[] key);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        public extern ECPoint GetECPoint(byte[] key);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        [OpCode(OpCode.CONVERT, StackItemType.Buffer)]
        public extern byte[] GetByteArray(byte[] key);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        public extern ByteString GetString(byte[] key);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        [OpCode(OpCode.CONVERT, StackItemType.Integer)]
        public extern BigInteger GetInteger(byte[] key);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        [OpCode(OpCode.NOT)]
        [OpCode(OpCode.NOT)]
        public extern bool GetBoolean(byte[] key);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.ISNULL)]
        [OpCode(OpCode.JMPIFNOT, "0x06")]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.PUSH0)]
        [OpCode(OpCode.JMP, "0x04")]
        [OpCode(OpCode.CONVERT, StackItemType.Integer)]
        public extern BigInteger GetIntegerOrZero(byte[] key);

        public object GetObject(ByteString key)
        {
            ByteString value = Get(key);
            if (value is null) return null;
            return EssentialLib.Deserialize(value);
        }

        public object GetObject(byte[] key)
        {
            ByteString value = Get(key);
            if (value is null) return null;
            return EssentialLib.Deserialize(value);
        }

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [Syscall("System.Storage.Find")]
        public extern Iterator Find(FindOptions options = FindOptions.None);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Find")]
        public extern Iterator Find(ByteString prefix, FindOptions options = FindOptions.None);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Find")]
        public extern Iterator Find(byte[] prefix, FindOptions options = FindOptions.None);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Put")]
        public extern void Put(ByteString key, ByteString value);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Put")]
        public extern void Put(byte[] key, ByteString value);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Put")]
        public extern void Put(ByteString key, BigInteger value);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Put")]
        public extern void Put(byte[] key, BigInteger value);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Put")]
        public extern void Put(ByteString key, bool value);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Put")]
        public extern void Put(byte[] key, bool value);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
