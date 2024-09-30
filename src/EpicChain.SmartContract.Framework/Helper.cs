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


using EpicChain.SmartContract.Framework.Attributes;
using System;
using System.Numerics;

namespace EpicChain.SmartContract.Framework
{
    public static class Helper
    {
        /// <summary>
        /// Converts byte to byte[] considering the byte as a BigInteger (0x00 at the end)
        /// </summary>
        [OpCode(OpCode.PUSH1)]
        [OpCode(OpCode.LEFT)]
        public extern static byte[] ToByteArray(this byte source);

        /// <summary>
        /// Converts sbyte to byte[].
        /// </summary>
        [OpCode(OpCode.CONVERT, StackItemType.Buffer)]
        public extern static byte[] ToByteArray(this sbyte source);

        /// <summary>
        /// Converts string to byte[]. Examples: "hello" -> [0x68656c6c6f]; "" -> []; "Neo" -> [0x4e656f]
        /// </summary>
        [OpCode(OpCode.CONVERT, StackItemType.Buffer)]
        public extern static byte[] ToByteArray(this string source);

        /// <summary>
        /// Converts byte[] to string. Examples: [0x68656c6c6f] -> "hello"; [] -> ""; [0x4e656f] -> "Neo"
        /// </summary>
        [OpCode(OpCode.CONVERT, StackItemType.ByteString)]
        public extern static string ToByteString(this byte[] source);

        /// <summary>
        /// Returns true iff a <= x && x < b. Examples: x=5 a=5 b=15 is true; x=15 a=5 b=15 is false
        /// </summary>
        [OpCode(OpCode.WITHIN)]
        public extern static bool Within(this BigInteger x, BigInteger a, BigInteger b);

        /// <summary>
        /// Returns true iff a <= x && x < b. Examples: x=5 a=5 b=15 is true; x=15 a=5 b=15 is false
        /// </summary>
        [OpCode(OpCode.WITHIN)]
        public extern static bool Within(this int x, BigInteger a, BigInteger b);

        /// <summary>
        /// Converts and ensures parameter source is sbyte (range 0x00 to 0xff); faults otherwise.
        /// Examples: 255 -> fault; -128 -> [0x80]; 0 -> [0x00]; 10 -> [0x0a]; 127 -> [0x7f]; 128 -> fault
        /// ScriptAttribute: DUP SIZE PUSH1 NUMEQUAL ASSERT
        /// </summary>
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.SIZE)]
        [OpCode(OpCode.PUSH1)]
        [OpCode(OpCode.NUMEQUAL)]
        [OpCode(OpCode.ASSERT)]
        public extern static sbyte AsSbyte(this BigInteger source);
        //{
        //    Assert(source.AsByteArray().Length == 1);
        //    return (sbyte) source;
        //}

        /// <summary>
        /// Converts and ensures parameter source is sbyte (range 0x00 to 0xff); faults otherwise.
        /// Examples: 255 -> fault; -128 -> [0x80]; 0 -> [0x00]; 10 -> [0x0a]; 127 -> [0x7f]; 128 -> fault
        /// ScriptAttribute: DUP SIZE PUSH1 NUMEQUAL ASSERT
        /// </summary>
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.SIZE)]
        [OpCode(OpCode.PUSH1)]
        [OpCode(OpCode.NUMEQUAL)]
        [OpCode(OpCode.ASSERT)]
        public extern static sbyte AsSbyte(this int source);
        //{
        //    Assert(((BigInteger)source).AsByteArray().Length == 1);
        //    return (sbyte) source;
        //}

        /// <summary>
        /// Converts and ensures parameter source is byte (range 0x00 to 0xff); faults otherwise.
        /// Examples: 255 -> fault; -128 -> [0x80]; 0 -> [0x00]; 10 -> [0x0a]; 127 -> [0x7f]; 128 -> fault
        /// ScriptAttribute: DUP SIZE PUSH1 NUMEQUAL ASSERT
        /// </summary>
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.SIZE)]
        [OpCode(OpCode.PUSH1)]
        [OpCode(OpCode.NUMEQUAL)]
        [OpCode(OpCode.ASSERT)]
        public extern static byte AsByte(this BigInteger source);
        //{
        //    Assert(source.AsByteArray().Length == 1);
        //    return (byte) source;
        //}

        /// <summary>
        /// Converts and ensures parameter source is byte (range 0x00 to 0xff); faults otherwise.
        /// Examples: 255 -> fault; -128 -> [0x80]; 0 -> [0x00]; 10 -> [0x0a]; 127 -> [0x7f]; 128 -> fault
        /// ScriptAttribute: DUP SIZE PUSH1 NUMEQUAL ASSERT
        /// </summary>
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.SIZE)]
        [OpCode(OpCode.PUSH1)]
        [OpCode(OpCode.NUMEQUAL)]
        [OpCode(OpCode.ASSERT)]
        public extern static byte AsByte(this int source);
        //{
        //    Assert(((BigInteger)source).AsByteArray().Length == 1);
        //    return (byte) source;
        //}

        /// <summary>
        /// Converts parameter to sbyte from (big)integer range -128-255; faults if out-of-range.
        /// Examples: 256 -> fault; -1 -> -1 [0xff]; 255 -> -1 [0xff]; 0 -> 0 [0x00]; 10 -> 10 [0x0a]; 127 -> 127 [0x7f]; 128 -> -128 [0x80]
        /// </summary>
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.PUSHINT8, "7f")]
        [OpCode(OpCode.JMPLE, "06")]
        [OpCode(OpCode.PUSHINT16, "0001")]
        [OpCode(OpCode.SUB)]
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.PUSHINT8, "80")]
        [OpCode(OpCode.PUSHINT16, "8000")]
        [OpCode(OpCode.WITHIN)]
        [OpCode(OpCode.ASSERT)]
        public static extern sbyte ToSbyte(this BigInteger source);

        /// <summary>
        /// Converts parameter to sbyte from (big)integer range -128-255; faults if out-of-range.
        /// Examples: 256 -> fault; -1 -> -1 [0xff]; 255 -> -1 [0xff]; 0 -> 0 [0x00]; 10 -> 10 [0x0a]; 127 -> 127 [0x7f]; 128 -> -128 [0x80]
        /// </summary>
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.PUSHINT8, "7f")]
        [OpCode(OpCode.JMPLE, "06")]
        [OpCode(OpCode.PUSHINT16, "0001")]
        [OpCode(OpCode.SUB)]
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.PUSHINT8, "80")]
        [OpCode(OpCode.PUSHINT16, "8000")]
        [OpCode(OpCode.WITHIN)]
        [OpCode(OpCode.ASSERT)]
        public static extern sbyte ToSbyte(this int source);

        /// <summary>
        /// Converts parameter to byte from (big)integer range 0-255; faults if out-of-range.
        /// Examples: 256 -> fault; -1 -> fault; 255 -> -1 [0xff]; 0 -> 0 [0x00]; 10 -> 10 [0x0a]; 127 -> 127 [0x7f]; 128 -> -128 [0x80]
        /// </summary>
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.PUSH0)]
        [OpCode(OpCode.PUSHINT16, "0001")]
        [OpCode(OpCode.WITHIN)]
        [OpCode(OpCode.ASSERT)]
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.PUSHINT8, "7f")]
        [OpCode(OpCode.JMPLE, "06")]
        [OpCode(OpCode.PUSHINT16, "0001")]
        [OpCode(OpCode.SUB)]
        public static extern byte ToByte(this BigInteger source);

        /// <summary>
        /// Converts parameter to byte from (big)integer range 0-255; faults if out-of-range.
        /// Examples: 256 -> fault; -1 -> fault; 255 -> -1 [0xff]; 0 -> 0 [0x00]; 10 -> 10 [0x0a]; 127 -> 127 [0x7f]; 128 -> -128 [0x80]
        /// </summary>
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.PUSH0)]
        [OpCode(OpCode.PUSHINT16, "0001")]
        [OpCode(OpCode.WITHIN)]
        [OpCode(OpCode.ASSERT)]
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.PUSHINT8, "7f")]
        [OpCode(OpCode.JMPLE, "06")]
        [OpCode(OpCode.PUSHINT16, "0001")]
        [OpCode(OpCode.SUB)]
        public static extern byte ToByte(this int source);

        [OpCode(OpCode.CAT)]
        public extern static byte[] Concat(this byte[] first, byte[] second);

        [OpCode(OpCode.CAT)]
        public extern static byte[] Concat(this byte[] first, ByteString second);

        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.CONVERT, StackItemType.ByteString)]
        public extern static ByteString Concat(this ByteString first, ByteString second);

        [OpCode(OpCode.SUBSTR)]
        public extern static byte[] Range(this byte[] source, int index, int count);

        /// <summary>
        /// Returns byte[] with first 'count' elements from 'source'. Faults if count < 0
        /// </summary>
        [OpCode(OpCode.LEFT)]
        public extern static byte[] Take(this byte[] source, int count);

        /// <summary>
        /// Returns byte[] with last 'count' elements from 'source'. Faults if count < 0
        /// </summary>
        [OpCode(OpCode.RIGHT)]
        public extern static byte[] Last(this byte[] source, int count);

        /// <summary>
        /// Returns a reversed copy of parameter 'source'.
        /// Example: [0a,0b,0c,0d,0e] -> [0e,0d,0c,0b,0a]
        /// </summary>
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.REVERSEITEMS)]
        public extern static byte[] Reverse(this Array source);

        /// <summary>
        /// Returns the square root of number x
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        [OpCode(OpCode.SQRT)]
        public extern static BigInteger Sqrt(this BigInteger x);

        [OpCode(OpCode.MODMUL)]
        public extern static BigInteger ModMultiply(this BigInteger x, BigInteger y, BigInteger modulus);

        public static BigInteger ModInverse(this BigInteger value, BigInteger modulus)
        {
            return BigInteger.ModPow(value, -1, modulus);
        }
    }
}
