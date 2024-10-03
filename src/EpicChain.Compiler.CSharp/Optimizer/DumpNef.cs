// Copyright (C) 2021-2024 EpicChain Lab's
//
// DumpNef.cs file is a crucial component of the EpicChain project and is freely distributed as open-source software.
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

using EpicChain.Json;
using EpicChain.SmartContract;
using EpicChain.SmartContract.Native;
using EpicChain.VM;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace EpicChain.Optimizer
{
    static class DumpNef
    {
        private static readonly Regex DocumentRegex = new(@"\[(\d+)\](\d+)\:(\d+)\-(\d+)\:(\d+)", RegexOptions.Compiled);
        private static readonly Regex RangeRegex = new(@"(\d+)\-(\d+)", RegexOptions.Compiled);
        private static readonly Regex SequencePointRegex = new(@"(\d+)(\[\d+\]\d+\:\d+\-\d+\:\d+)", RegexOptions.Compiled);

        static readonly Lazy<IReadOnlyDictionary<uint, string>> sysCallNames = new(
            () => ApplicationEngine.Services.ToImmutableDictionary(kvp => kvp.Value.Hash, kvp => kvp.Value.Name));

        public static byte[] ZipDebugInfo(byte[] content, string innerFilename)
        {
            using (var compressedFileStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Update, false))
                {
                    var zipEntry = zipArchive.CreateEntry(innerFilename);
                    using (var originalFileStream = new MemoryStream(content))
                    {
                        using (var zipEntryStream = zipEntry.Open())
                        {
                            originalFileStream.CopyTo(zipEntryStream);
                        }
                    }
                }
                return compressedFileStream.ToArray();
            }
        }

        public static string UnzipDebugInfo(byte[] zippedBuffer)
        {
            using var zippedStream = new MemoryStream(zippedBuffer);
            using var archive = new ZipArchive(zippedStream, ZipArchiveMode.Read, false, Encoding.UTF8);
            var entry = archive.Entries.FirstOrDefault();
            if (entry != null)
            {
                using var unzippedEntryStream = entry.Open();
                using var ms = new MemoryStream();
                unzippedEntryStream.CopyTo(ms);
                var unzippedArray = ms.ToArray();
                return Encoding.UTF8.GetString(unzippedArray);
            }
            throw new ArgumentException("No file found in zip archive");
        }

        public static string GetInstructionAddressPadding(this Script script)
        {
            var digitCount = EnumerateInstructions(script).Last().address switch
            {
                var x when x < 10 => 1,
                var x when x < 100 => 2,
                var x when x < 1000 => 3,
                var x when x < 10000 => 4,
                var x when x <= ushort.MaxValue => 5,
                _ => throw new Exception($"Max script length is {ushort.MaxValue} bytes"),
            };
            return new string('0', digitCount);
        }

        public static string WriteInstruction(int address, Instruction instruction, string padString, MethodToken[] tokens)
        {
            string result = "";
            try
            {
                result += $"{address.ToString(padString)}";
                result += $" {instruction.OpCode}";
                if (!instruction.Operand.IsEmpty)
                    result += $" {instruction.GetOperandString()}";

                var comment = instruction.GetComment(address, tokens);
                if (comment.Length > 0)
                    result += $" # {comment}";
            }
            finally { }
            return result;
        }

        /// <summary>
        /// DO NOT use this very often. It builds new instruction objects,
        /// while the optimizer compares instruction objects using ReferenceEquals
        /// </summary>
        /// <param name="script"></param>
        /// <param name="print">Console.WriteLine all instructions for debugging</param>
        /// <returns></returns>
        public static IEnumerable<(int address, Instruction instruction)> EnumerateInstructions(this Script script, bool print = false)
        {
            int address = 0;
            OpCode opcode = OpCode.PUSH0;
            Instruction instruction;
            for (; address < script.Length; address += instruction.Size)
            {
                instruction = script.GetInstruction(address);
                opcode = instruction.OpCode;
                if (print)
                    Console.WriteLine(WriteInstruction(address, instruction, "0000", Array.Empty<MethodToken>()));
                yield return (address, instruction);
            }
            if (opcode != OpCode.RET)
                yield return (address, Instruction.RET);
        }

        public static string GetOperandString(this Instruction instruction) => BitConverter.ToString(instruction.Operand.Span.ToArray());

        public static string GetComment(this Instruction instruction, int ip, MethodToken[]? tokens = null)
        {
            tokens ??= Array.Empty<MethodToken>();

            switch (instruction.OpCode)
            {
                case OpCode.PUSHINT8:
                case OpCode.PUSHINT16:
                case OpCode.PUSHINT32:
                case OpCode.PUSHINT64:
                case OpCode.PUSHINT128:
                case OpCode.PUSHINT256:
                    return $"{new BigInteger(instruction.Operand.Span)}";
                case OpCode.PUSHA:
                    return $"{checked(ip + instruction.TokenI32)}";
                case OpCode.PUSHDATA1:
                case OpCode.PUSHDATA2:
                case OpCode.PUSHDATA4:
                    {
                        var text = System.Text.Encoding.UTF8.GetString(instruction.Operand.Span)
                            .Replace("\r", "\"\\r\"").Replace("\n", "\"\\n\"");
                        if (instruction.Operand.Length == 20)
                        {
                            return $"as script hash: {new UInt160(instruction.Operand.Span)}, as text: \"{text}\"";
                        }
                        return $"as text: \"{text}\"";
                    }
                case OpCode.JMP:
                case OpCode.JMPIF:
                case OpCode.JMPIFNOT:
                case OpCode.JMPEQ:
                case OpCode.JMPNE:
                case OpCode.JMPGT:
                case OpCode.JMPGE:
                case OpCode.JMPLT:
                case OpCode.JMPLE:
                case OpCode.CALL:
                    return OffsetComment(instruction.TokenI8);
                case OpCode.JMP_L:
                case OpCode.JMPIF_L:
                case OpCode.JMPIFNOT_L:
                case OpCode.JMPEQ_L:
                case OpCode.JMPNE_L:
                case OpCode.JMPGT_L:
                case OpCode.JMPGE_L:
                case OpCode.JMPLT_L:
                case OpCode.JMPLE_L:
                case OpCode.CALL_L:
                    return OffsetComment(instruction.TokenI32);
                case OpCode.CALLT:
                    {
                        int index = instruction.TokenU16;
                        if (index >= tokens.Length)
                            return $"Unknown token {instruction.TokenU16}";
                        var token = tokens[index];
                        var contract = NativeContract.Contracts.SingleOrDefault(c => c.Hash == token.Hash);
                        var tokenName = contract is null ? $"{token.Hash}" : contract.Name;
                        return $"{tokenName}.{token.Method} token call";
                    }
                case OpCode.TRY:
                    return TryComment(instruction.TokenI8, instruction.TokenI8_1);
                case OpCode.TRY_L:
                    return TryComment(instruction.TokenI32, instruction.TokenI32_1);
                case OpCode.ENDTRY:
                    return OffsetComment(instruction.TokenI8);
                case OpCode.ENDTRY_L:
                    return OffsetComment(instruction.TokenI32);
                case OpCode.SYSCALL:
                    return sysCallNames.Value.TryGetValue(instruction.TokenU32, out var name)
                        ? $"{name} SysCall"
                        : $"Unknown SysCall {instruction.TokenU32}";
                case OpCode.INITSSLOT:
                    return $"{instruction.TokenU8} static variables";
                case OpCode.INITSLOT:
                    return $"{instruction.TokenU8} local variables, {instruction.TokenU8_1} arguments";
                case OpCode.LDSFLD:
                case OpCode.STSFLD:
                case OpCode.LDLOC:
                case OpCode.STLOC:
                case OpCode.LDARG:
                case OpCode.STARG:
                    return $"Slot index {instruction.TokenU8}";
                case OpCode.NEWARRAY_T:
                case OpCode.ISTYPE:
                case OpCode.CONVERT:
                    return $"{(VM.Types.StackItemType)instruction.TokenU8} type";
                default:
                    return string.Empty;
            }

            string OffsetComment(int offset) => $"pos: {checked(ip + offset)} (offset: {offset})";
            string TryComment(int catchOffset, int finallyOffset)
            {
                var builder = new System.Text.StringBuilder();
                builder.Append(catchOffset == 0 ? "no catch block, " : $"catch {OffsetComment(catchOffset)}, ");
                builder.Append(finallyOffset == 0 ? "no finally block" : $"finally {OffsetComment(finallyOffset)}");
                return builder.ToString();
            }
        }

        public static (int start, int end) GetMethodStartEndAddress(string name, JToken debugInfo)
        {
            name = name.Length == 0 ? string.Empty : name[0].ToString().ToUpper() + name.Substring(1);  // first letter uppercase
            int start = -1, end = -1;
            foreach (JToken? method in (JArray)debugInfo["methods"]!)
            {
                string methodName = method!["name"]!.AsString().Split(",")[1];
                if (methodName == name)
                {
                    GroupCollection rangeGroups = RangeRegex.Match(method["range"]!.AsString()).Groups;
                    (start, end) = (int.Parse(rangeGroups[1].ToString()), int.Parse(rangeGroups[2].ToString()));
                }
            }
            return (start, end);
        }

        public static List<int> OpCodeAddressesInMethod(NefFile nef, JToken DebugInfo, string method, OpCode opcode)
        {
            (int start, int end) = GetMethodStartEndAddress(method, DebugInfo);
            List<(int a, Instruction i)> instructions = EnumerateInstructions(nef.Script).ToList();
            return instructions.Where(
                ai => ai.i.OpCode == opcode &&
                ai.a >= start && ai.a <= end
                ).Select(ai => ai.a).ToList();
        }

        public static string GenerateDumpNef(NefFile nef, JToken? debugInfo)
        {
            Script script = nef.Script;
            List<(int, Instruction)> addressAndInstructionsList = script.EnumerateInstructions().ToList();
            Dictionary<int, Instruction> addressToInstruction = new();
            foreach ((int a, Instruction i) in addressAndInstructionsList)
                addressToInstruction.Add(a, i);
            Dictionary<int, string> methodStartAddrToName = new();
            Dictionary<int, string> methodEndAddrToName = new();
            Dictionary<int, List<(int docId, int startLine, int startCol, int endLine, int endCol)>> newAddrToSequencePoint = new();

            if (debugInfo != null)
            {
                foreach (JToken? method in (JArray)debugInfo["methods"]!)
                {
                    GroupCollection rangeGroups = RangeRegex.Match(method!["range"]!.AsString()).Groups;
                    (int methodStartAddr, int methodEndAddr) = (int.Parse(rangeGroups[1].ToString()), int.Parse(rangeGroups[2].ToString()));
                    methodStartAddrToName.Add(methodStartAddr, method!["id"]!.AsString());  // TODO: same format of method name as dumpnef
                    methodEndAddrToName.Add(methodEndAddr, method["id"]!.AsString());

                    foreach (JToken? sequencePoint in (JArray)method["sequence-points"]!)
                    {
                        GroupCollection sequencePointGroups = SequencePointRegex.Match(sequencePoint!.AsString()).Groups;
                        GroupCollection documentGroups = DocumentRegex.Match(sequencePointGroups[2].ToString()).Groups;
                        int addr = int.Parse(sequencePointGroups[1].Value);
                        if (!newAddrToSequencePoint.ContainsKey(addr))
                            newAddrToSequencePoint.Add(addr, new());
                        newAddrToSequencePoint[addr].Add((
                            int.Parse(documentGroups[1].ToString()),
                            int.Parse(documentGroups[2].ToString()),
                            int.Parse(documentGroups[3].ToString()),
                            int.Parse(documentGroups[4].ToString()),
                            int.Parse(documentGroups[5].ToString())
                        ));
                    }
                }
            }

            Dictionary<string, string[]> docPathToContent = new();
            string dumpnef = "";
            foreach ((int a, Instruction i) in script.EnumerateInstructions(/*print: true*/).ToList())
            {
                if (methodStartAddrToName.ContainsKey(a))
                    dumpnef += $"# Method Start {methodStartAddrToName[a]}\n";
                if (methodEndAddrToName.ContainsKey(a))
                    dumpnef += $"# Method End {methodEndAddrToName[a]}\n";
                if (newAddrToSequencePoint.ContainsKey(a))
                {
                    foreach ((int docId, int startLine, int startCol, int endLine, int endCol) in newAddrToSequencePoint[a])
                    {
                        string docPath = debugInfo!["documents"]![docId]!.AsString();
                        if (debugInfo["document-root"] != null)
                            docPath = Path.Combine(debugInfo["document-root"]!.AsString(), docPath);
                        if (!docPathToContent.ContainsKey(docPath))
                            docPathToContent.Add(docPath, File.ReadAllLines(docPath).ToArray());
                        if (startLine == endLine)
                            dumpnef += $"# Code {Path.GetFileName(docPath)} line {startLine}: \"{docPathToContent[docPath][startLine - 1][(startCol - 1)..(endCol - 1)]}\"\n";
                        else
                            for (int lineIndex = startLine; lineIndex <= endLine; lineIndex++)
                            {
                                string src;
                                if (lineIndex == startLine)
                                    src = docPathToContent[docPath][lineIndex - 1][(startCol - 1)..].Trim();
                                else if (lineIndex == endLine)
                                    src = docPathToContent[docPath][lineIndex - 1][..(endCol - 1)].Trim();
                                else
                                    src = docPathToContent[docPath][lineIndex - 1].Trim();
                                dumpnef += $"# Code {Path.GetFileName(docPath)} line {startLine}: \"{src}\"\n";
                            }
                    }
                }
                if (a < script.Length)
                    dumpnef += $"{WriteInstruction(a, script.GetInstruction(a), script.GetInstructionAddressPadding(), nef.Tokens)}\n";
            }
            return dumpnef;
        }
    }
}
