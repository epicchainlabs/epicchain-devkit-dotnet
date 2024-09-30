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
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace EpicChain.SmartContract.Framework
{
    public abstract class ByteString : IEnumerable<byte>
    {
        public static extern ByteString Empty { [OpCode(OpCode.PUSHDATA1, "00")] get; }

        public extern byte this[int index]
        {
            [OpCode(OpCode.PICKITEM)]
            get;
        }

        public extern int Length
        {
            [OpCode(OpCode.SIZE)]
            get;
        }

        IEnumerator<byte> IEnumerable<byte>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        [OpCode(OpCode.NOP)]
        public static extern implicit operator string(ByteString str);

        [OpCode(OpCode.NOP)]
        public static extern implicit operator ByteString(string str);

        [OpCode(OpCode.CONVERT, StackItemType.Buffer)]
        public static extern explicit operator byte[](ByteString str);

        [OpCode(OpCode.CONVERT, StackItemType.ByteString)]
        public static extern explicit operator ByteString(byte[] buffer);

        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.ISNULL)]
        [OpCode(OpCode.JMPIFNOT, "0x04")]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.PUSH0)]
        [OpCode(OpCode.CONVERT, StackItemType.Integer)]
        public static extern explicit operator BigInteger(ByteString text);

        [OpCode(OpCode.CONVERT, StackItemType.ByteString)]
        public static extern explicit operator ByteString(BigInteger integer);
    }
}
