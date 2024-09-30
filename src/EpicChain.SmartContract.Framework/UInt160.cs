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
using EpicChain.SmartContract.Framework.Native;
using EpicChain.SmartContract.Framework.Services;

namespace EpicChain.SmartContract.Framework
{
    public abstract class UInt160 : ByteString
    {
        public static extern UInt160 Zero { [OpCode(OpCode.PUSHDATA1, "140000000000000000000000000000000000000000")] get; }

        public extern bool IsZero
        {
            [OpCode(OpCode.PUSH0)]
            [OpCode(OpCode.NUMEQUAL)]
            get;
        }

        public extern bool IsValid
        {
            [OpCode(OpCode.DUP)]
            [OpCode(OpCode.ISTYPE, "0x28")] //ByteString
            [OpCode(OpCode.SWAP)]
            [OpCode(OpCode.SIZE)]
            [OpCode(OpCode.PUSHINT8, "14")] // 0x14 == 20 bytes expected array size
            [OpCode(OpCode.NUMEQUAL)]
            [OpCode(OpCode.BOOLAND)]
            get;
        }

        public bool IsValidAndNotZero => IsValid && !IsZero;

        [OpCode(OpCode.CONVERT, StackItemType.ByteString)]
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.ISNULL)]
        [OpCode(OpCode.JMPIF, "09")]
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.SIZE)]
        [OpCode(OpCode.PUSHINT8, "14")] // 0x14 == 20 bytes expected array size
        [OpCode(OpCode.JMPEQ, "03")]
        [OpCode(OpCode.THROW)]
        public static extern explicit operator UInt160(byte[] value);

        [OpCode(OpCode.CONVERT, StackItemType.Buffer)]
        public static extern explicit operator byte[](UInt160 value);

        /// <summary>
        /// Converts the specified script hash to an address, using the current blockchain AddressVersion value.
        /// </summary>
        /// <returns>The converted address.</returns>
        public string ToAddress()
        {
            return ToAddress(Runtime.AddressVersion);
        }

        /// <summary>
        /// Converts the specified script hash to an address.
        /// </summary>
        /// <param name="version">The address version.</param>
        /// <returns>The converted address.</returns>
        public string ToAddress(byte version)
        {
            byte[] data = { version };
            data = Helper.Concat(data, this);
            return StdLib.Base58CheckEncode((ByteString)data);
        }

        /// <summary>
        /// Implicitly converts a hexadecimal string to a UInt160 object.
        /// This can be a 20 bytes hex string or a neo address.
        /// <example>
        /// 20 bytes hex string: "01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4" (no prefix)
        ///             Address: "NZNosnRn6FpRjwGKx8VdXv5Sn7BvzrjZVb"
        /// </example>
        /// <remarks>
        /// This is a compile time conversion, only work with constant string.
        /// If you want to convert a runtime string, convert it to byte[] first.
        /// </remarks>
        /// </summary>
#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it
        public static extern implicit operator UInt160(string value);
#pragma warning restore CS0626 // Method, operator, or accessor is marked external and has no attributes on it
    }
}
