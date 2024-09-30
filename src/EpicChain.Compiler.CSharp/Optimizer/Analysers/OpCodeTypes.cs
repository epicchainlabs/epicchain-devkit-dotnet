// Copyright (C) 2021-2024 EpicChain Labs.
//
// OpCodeTypes.cs file is a crucial component of the EpicChain project and is freely distributed as open-source software.
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


using EpicChain.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using static EpicChain.VM.OpCode;

namespace EpicChain.Optimizer
{
    static class OpCodeTypes
    {
        public static readonly HashSet<OpCode> push = new();
        public static readonly HashSet<OpCode> allowedBasicBlockEnds;

        static OpCodeTypes()
        {
            foreach (OpCode op in pushInt)
                push.Add(op);
            foreach (OpCode op in pushBool)
                push.Add(op);
            push.Add(PUSHA);
            push.Add(PUSHNULL);
            foreach (OpCode op in pushData)
                push.Add(op);
            foreach (OpCode op in pushConstInt)
                push.Add(op);
            foreach (OpCode op in pushStackOps)
                push.Add(op);
            foreach (OpCode op in pushNewCompoundType)
                push.Add(op);
            allowedBasicBlockEnds = ((OpCode[])Enum.GetValues(typeof(OpCode)))
                    .Where(i => JumpTarget.SingleJumpInOperand(i) && i != PUSHA || JumpTarget.DoubleJumpInOperand(i)).ToHashSet()
                    .Union(new HashSet<OpCode>() { RET, ABORT, ABORTMSG, THROW, ENDFINALLY
            }).ToHashSet();
        }

        public static readonly HashSet<OpCode> pushInt = new()
        {
            PUSHINT8,
            PUSHINT16,
            PUSHINT32,
            PUSHINT64,
            PUSHINT128,
            PUSHINT256,
        };

        public static readonly HashSet<OpCode> pushBool = new()
        {
            PUSHT, PUSHF,
        };

        public static readonly HashSet<OpCode> pushData = new()
        {
            PUSHDATA1,
            PUSHDATA2,
            PUSHDATA4,
        };

        public static readonly HashSet<OpCode> pushConstInt = new()
        {
            PUSHM1,
            PUSH0,
            PUSH1,
            PUSH2,
            PUSH3,
            PUSH4,
            PUSH5,
            PUSH6,
            PUSH7,
            PUSH8,
            PUSH9,
            PUSH10,
            PUSH11,
            PUSH12,
            PUSH13,
            PUSH14,
            PUSH15,
            PUSH16,
        };

        public static readonly HashSet<OpCode> pushStackOps = new()
        {
            DEPTH,
            DUP,
            OVER,
        };

        public static readonly HashSet<OpCode> pushNewCompoundType = new()
        {
            NEWARRAY0,
            NEWSTRUCT0,
            NEWMAP,
        };

        // BE AWARE that PUSHA is also related to addresses
        public static readonly HashSet<OpCode> tryThrowFinally = new()
        {
            TRY,
            TRY_L,
            THROW,
            ENDTRY,
            ENDTRY_L,
            ENDFINALLY,
        };

        public static readonly HashSet<OpCode> unconditionalJump = new()
        {
            JMP,
            JMP_L,
        };

        public static readonly HashSet<OpCode> callWithJump = new()
        {
            CALL,
            CALL_L,
            CALLA,
        };

        public static readonly HashSet<OpCode> conditionalJump = new()
        {
            JMPIF,
            JMPIFNOT,
            JMPEQ,
            JMPNE,
            JMPGT,
            JMPGE,
            JMPLT,
            JMPLE,
        };

        public static readonly HashSet<OpCode> conditionalJump_L = new()
        {
            JMPIF_L,
            JMPIFNOT_L,
            JMPEQ_L,
            JMPNE_L,
            JMPGT_L,
            JMPGE_L,
            JMPLT_L,
            JMPLE_L,
        };

        public static readonly HashSet<OpCode> loadStaticFields = new()
        {
            LDSFLD,
            LDSFLD0,
            LDSFLD1,
            LDSFLD2,
            LDSFLD3,
            LDSFLD4,
            LDSFLD5,
            LDSFLD6,
        };
        public static readonly HashSet<OpCode> storeStaticFields = new()
        {
            STSFLD,
            STSFLD0,
            STSFLD1,
            STSFLD2,
            STSFLD3,
            STSFLD4,
            STSFLD5,
            STSFLD6,
        };
        public static readonly HashSet<OpCode> loadLocalVariables = new()
        {
            LDLOC,
            LDLOC0,
            LDLOC1,
            LDLOC2,
            LDLOC3,
            LDLOC4,
            LDLOC5,
            LDLOC6,
        };
        public static readonly HashSet<OpCode> storeLocalVariables = new()
        {
            STLOC,
            STLOC0,
            STLOC1,
            STLOC2,
            STLOC3,
            STLOC4,
            STLOC5,
            STLOC6,
        };
        public static readonly HashSet<OpCode> loadArguments = new()
        {
            LDARG,
            LDARG0,
            LDARG1,
            LDARG2,
            LDARG3,
            LDARG4,
            LDARG5,
            LDARG6,
        };
        public static readonly HashSet<OpCode> storeArguments = new()
        {
            STARG,
            STARG0,
            STARG1,
            STARG2,
            STARG3,
            STARG4,
            STARG5,
            STARG6,
        };
    }
}
