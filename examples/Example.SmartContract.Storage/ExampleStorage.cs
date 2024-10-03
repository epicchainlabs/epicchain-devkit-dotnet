// Copyright (C) 2021-2024 EpicChain Lab's
//
// Storage.cs file is a crucial component of the EpicChain project and is freely distributed as open-source software.
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
using EpicChain.SmartContract.Framework.Services;
using System.ComponentModel;

namespace ExampleStorage
{
    [DisplayName("SampleStorage")]
    [ContractAuthor("code-dev", "devs@epic-chain.org")]
    [ContractDescription("A sample contract to demonstrate how to use storage")]
    [ContractVersion("0.0.1")]
    [ContractSourceCode("https://github.com/epicchainlabs/epicchain-devkit-dotnet/tree/master/examples/")]
    [ContractPermission(Permission.Any, Method.Any)]
    public class SampleStorage : SmartContract
    {
        #region Byte

        public static bool TestPutByte(byte[] key, byte[] value)
        {
            var storage = new StorageMap(0x11);
            storage.Put((ByteString)key, (ByteString)value);
            return true;
        }

        public static void TestDeleteByte(byte[] key)
        {
            var storage = new StorageMap(0x11);
            storage.Delete((ByteString)key);
        }

        public static byte[] TestGetByte(byte[] key)
        {
            var storage = new StorageMap(0x11);
            var value = storage.Get((ByteString)key);
            return (byte[])value;
        }

        public static byte[] TestOver16Bytes()
        {
            var value = new byte[] { 0x3b, 0x00, 0x32, 0x03, 0x23, 0x23, 0x23, 0x23, 0x02, 0x23, 0x23, 0x02, 0x23, 0x23, 0x02, 0x23, 0x23, 0x02, 0x23, 0x23, 0x02, 0x23, 0x23, 0x02 };
            StorageMap storageMap = new StorageMap("test_map");
            storageMap.Put((ByteString)new byte[] { 0x01 }, (ByteString)value);
            return (byte[])storageMap.Get((ByteString)new byte[] { 0x01 });
        }

        #endregion

        #region String

        public static bool TestPutString(byte[] key, byte[] value)
        {
            var prefix = "aa";
            var storage = new StorageMap(prefix);
            storage.Put((ByteString)key, (ByteString)value);
            return true;
        }

        public static void TestDeleteString(byte[] key)
        {
            var prefix = "aa";
            var storage = new StorageMap(prefix);
            storage.Delete((ByteString)key);
        }

        public static byte[] TestGetString(byte[] key)
        {
            var prefix = "aa";
            var storage = new StorageMap(prefix);
            var value = storage.Get((ByteString)key);
            return (byte[])value;
        }

        #endregion

        #region ByteArray

        public static bool TestPutByteArray(byte[] key, byte[] value)
        {
            var prefix = new byte[] { 0x00, 0xFF };
            var storage = new StorageMap(prefix);
            storage.Put((ByteString)key, (ByteString)value);
            return true;
        }

        public static void TestDeleteByteArray(byte[] key)
        {
            var prefix = new byte[] { 0x00, 0xFF };
            var storage = new StorageMap(prefix);
            storage.Delete((ByteString)key);
        }

        public static byte[] TestGetByteArray(byte[] key)
        {
            var prefix = new byte[] { 0x00, 0xFF };
            var storage = new StorageMap(prefix);
            var value = storage.Get((ByteString)key);
            return (byte[])value;
        }

        public static bool TestNewGetMethods()
        {
            var prefix = new byte[] { 0x00, 0xFF };
            var storage = new StorageMap(prefix);

            var boolValue = true;
            var intValue = 123;
            var stringValue = "hello world";
            var uint160Value = (UInt160)new byte[] {
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09,
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09
            };
            var uint256Value = (UInt256)new byte[] {
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09,
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09,
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09,
                0x00, 0x01
            };
            var ecPointValue = (ECPoint)new byte[] {
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09,
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09,
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09,
                0x00, 0x01, 0x02
            };

            storage.Put("bool", boolValue);
            storage.Put("int", intValue);
            storage.Put("string", stringValue);
            storage.Put("uint160", uint160Value);
            storage.Put("uint256", uint256Value);
            storage.Put("ecpoint", ecPointValue);

            var boolValue2 = storage.GetBoolean("bool");
            var intValue2 = storage.GetInteger("int");
            var stringValue2 = storage.GetString("string");
            var uint160Value2 = storage.GetUInt160("uint160");
            var uint256Value2 = storage.GetUInt256("uint256");
            var ecPointValue2 = storage.GetECPoint("ecpoint");

            return boolValue == boolValue2
                && intValue == intValue2
                && stringValue == stringValue2
                && uint160Value == uint160Value2
                && uint256Value == uint256Value2
                && ecPointValue == ecPointValue2;
        }

        public static byte[] TestNewGetByteArray()
        {
            var prefix = new byte[] { 0x00, 0xFF };
            var storage = new StorageMap(prefix);
            var byteArray = new byte[] { 0x00, 0x01 };
            storage.Put("byteArray", byteArray);
            var byteArray2 = storage.GetByteArray("byteArray");
            return byteArray2;
        }

        #endregion

        public static bool TestPutReadOnly(byte[] key, byte[] value)
        {
            var prefix = new byte[] { 0x00, 0xFF };
            var storage = new StorageMap(prefix);
            storage.Put((ByteString)key, (ByteString)value);
            return true;
        }

        #region Serialize

        class Value
        {
            public int Val;
        }

        public static int SerializeTest(byte[] key, int value)
        {
            var prefix = new byte[] { 0x01, 0xAA };
            var storage = new StorageMap(prefix);
            var val = new Value { Val = value };
            storage.PutObject(key, val);
            val = (Value)storage.GetObject(key);
            return val.Val;
        }

        #endregion

        #region Find

        public static byte[] TestFind()
        {
            Storage.Put((ByteString)"key1", (ByteString)new byte[] { 0x01 });
            Storage.Put((ByteString)"key2", (ByteString)new byte[] { 0x02 });
            var iterator = (Iterator<byte[]>)Storage.Find("key", FindOptions.ValuesOnly);
            iterator.Next();
            return iterator.Value;
        }

        #endregion

        #region IndexProperty

        public static bool TestIndexPut(byte[] key, byte[] value)
        {
            var prefix = "ii";
            var storage = new StorageMap(prefix);
            storage[(ByteString)key] = (ByteString)value;
            return true;
        }

        public static byte[] TestIndexGet(byte[] key)
        {
            var prefix = "ii";
            var storage = new StorageMap(prefix);
            return (byte[])storage[(ByteString)key];
        }

        #endregion
    }
}
