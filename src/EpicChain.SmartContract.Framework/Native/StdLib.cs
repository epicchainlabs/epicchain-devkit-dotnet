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
using System.Numerics;

namespace EpicChain.SmartContract.Framework.Native
{
    [Contract("0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0")]
    public static class StdLib
    {
        [ContractHash]
        public static extern UInt160 Hash { get; }

        public extern static ByteString Serialize(object source);

        public extern static object Deserialize(ByteString source);

        public extern static string JsonSerialize(object obj);

        public extern static object JsonDeserialize(string json);

        public static extern ByteString Base64Decode(string input);

        public static extern string Base64Encode(ByteString input);

        public static extern ByteString Base58Decode(string input);

        public static extern string Base58Encode(ByteString input);

        public static extern string Base58CheckEncode(ByteString input);

        public static extern ByteString Base58CheckDecode(string input);

        public static extern string Itoa(BigInteger value, int @base = 10);

        public static extern string Itoa(int value, int @base = 10);

        public static extern string Itoa(uint value, int @base = 10);

        public static extern string Itoa(long value, int @base = 10);

        public static extern string Itoa(ulong value, int @base = 10);

        public static extern string Itoa(short value, int @base = 10);

        public static extern string Itoa(ushort value, int @base = 10);

        public static extern string Itoa(byte value, int @base = 10);

        public static extern string Itoa(sbyte value, int @base = 10);

        public static extern BigInteger Atoi(string value, int @base = 10);

        public static extern int MemoryCompare(ByteString str1, ByteString str2);

        public static extern int MemorySearch(ByteString mem, ByteString value);

        public static extern int MemorySearch(ByteString mem, ByteString value, int start);

        public static extern int MemorySearch(ByteString mem, ByteString value, int start, bool backward);

        public static extern string[] StringSplit(string str, string separator);

        public static extern string[] StringSplit(string str, string separator, bool removeEmptyEntries);

        /// <summary>
        /// Get the string length by elements
        /// </summary>
        /// <param name="str">String value</param>
        /// <returns>Number of elements in the string</returns>
        /// <example>
        ///        string a = "A"; // return 1
        ///        string tilde = "ã"; // return 1
        ///        string duck = "🦆"; //return 1
        /// </example>
        public static extern int StrLen(string str);
    }
}
