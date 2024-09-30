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
            return StdLib.Deserialize(value);
        }

        public object GetObject(byte[] key)
        {
            ByteString value = Get(key);
            if (value is null) return null;
            return StdLib.Deserialize(value);
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
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Put")]
        public extern void Put(ByteString key, byte[] value);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Put")]
        public extern void Put(byte[] key, byte[] value);

        public void PutObject(ByteString key, object value)
        {
            Put(key, StdLib.Serialize(value));
        }

        public void PutObject(byte[] key, object value)
        {
            Put(key, StdLib.Serialize(value));
        }

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Delete")]
        public extern void Delete(ByteString key);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Delete")]
        public extern void Delete(byte[] key);
    }
}
