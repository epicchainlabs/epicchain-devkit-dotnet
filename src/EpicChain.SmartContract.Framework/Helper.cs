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
        /// Converts string to byte[]. Examples: "hello" -> [0x68656c6c6f]; "" -> []; "EpicChain" -> [0x4e656f]
        /// </summary>
        [OpCode(OpCode.CONVERT, StackItemType.Buffer)]
        public extern static byte[] ToByteArray(this string source);

        /// <summary>
        /// Converts byte[] to string. Examples: [0x68656c6c6f] -> "hello"; [] -> ""; [0x4e656f] -> "EpicChain"
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
