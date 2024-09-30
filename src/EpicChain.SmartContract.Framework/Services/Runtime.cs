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
using System;
using System.Numerics;

namespace EpicChain.SmartContract.Framework.Services
{
    public static class Runtime
    {
        public static extern TriggerType Trigger
        {
            [Syscall("System.Runtime.GetTrigger")]
            get;
        }

        public static extern string Platform
        {
            [Syscall("System.Runtime.Platform")]
            get;
        }

        [Obsolete("Use System.Runtime.Transaction instead")]
        public static extern object ScriptContainer
        {
            [Syscall("System.Runtime.GetScriptContainer")]
            get;
        }

        public static extern Transaction Transaction
        {
            [Syscall("System.Runtime.GetScriptContainer")]
            get;
        }

        public static extern UInt160 ExecutingScriptHash
        {
            [Syscall("System.Runtime.GetExecutingScriptHash")]
            get;
        }

        public static extern UInt160 CallingScriptHash
        {
            [Syscall("System.Runtime.GetCallingScriptHash")]
            get;
        }

        public static extern UInt160 EntryScriptHash
        {
            [Syscall("System.Runtime.GetEntryScriptHash")]
            get;
        }

        public static extern ulong Time
        {
            [Syscall("System.Runtime.GetTime")]
            get;
        }

        public static extern uint InvocationCounter
        {
            [Syscall("System.Runtime.GetInvocationCounter")]
            get;
        }

        public static extern long GasLeft
        {
            [Syscall("System.Runtime.GasLeft")]
            get;
        }

        public static extern byte AddressVersion
        {
            [Syscall("System.Runtime.GetAddressVersion")]
            get;
        }

        /// <summary>
        /// This method gets current invocation notifications from specific 'scriptHash'
        /// 'scriptHash' must have 20 bytes, but if it's all zero 0000...0000 it refers to all existing notifications (like a * wildcard)
        /// It will return an array of all matched notifications
        /// Each notification has two elements: a ScriptHash and the stackitem content of notification itself (called a 'State')
        /// The stackitem 'State' can be of any kind (a number, a string, an array, ...), so it's up to the developer perform the expected cast here
        /// </summary>
        [Syscall("System.Runtime.GetNotifications")]
        public static extern Notification[] GetNotifications(UInt160 hash = null);

        [Syscall("System.Runtime.CheckWitness")]
        public static extern bool CheckWitness(UInt160 hash);

        [Syscall("System.Runtime.CheckWitness")]
        public static extern bool CheckWitness(ECPoint pubkey);

        [Syscall("System.Runtime.Log")]
        public static extern void Log(string message);

        // Events not present in the ABI were disabled in HF_Basilisk
        // [Syscall("System.Runtime.Notify")]
        // public static extern void Notify(string eventName, object[] state);

        [OpCode(OpCode.PUSH1)]
        [OpCode(OpCode.PACK)]
        [OpCode(OpCode.PUSHDATA1, "054465627567")] // 0x5 - Debug
        //[OpCode(OpCode.SYSCALL, "95016f61")] // SHA256(System.Runtime.Notify)[0..4]
        [Syscall("System.Runtime.Notify")]
        public static extern void Debug(string message);

        [Syscall("System.Runtime.BurnGas")]
        public static extern void BurnGas(long gas);

        [Syscall("System.Runtime.GetRandom")]
        public static extern BigInteger GetRandom();

        [Syscall("System.Runtime.GetNetwork")]
        public static extern uint GetNetwork();

        [Syscall("System.Runtime.LoadScript")]
        public static extern object LoadScript(ByteString script, CallFlags flags, params object[] args);

        [Syscall("System.Runtime.CurrentSigners")]
        public static extern Signer[] CurrentSigners();
    }
}
