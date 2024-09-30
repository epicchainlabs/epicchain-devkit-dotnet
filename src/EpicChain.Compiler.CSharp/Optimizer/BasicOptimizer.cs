// Copyright (C) 2021-2024 EpicChain Labs.
//
// The EpicChain.Compiler.CSharp is open-source software made available under the MIT License.
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


using EpicChain.VM;
using System.Collections.Generic;

namespace EpicChain.Compiler.Optimizer
{
    static class BasicOptimizer
    {
        public static void RemoveNops(List<Instruction> instructions)
        {
            for (int i = 0; i < instructions.Count;)
            {
                Instruction instruction = instructions[i];
                if (instruction.OpCode == OpCode.NOP)
                {
                    instructions.RemoveAt(i);
                    foreach (Instruction other in instructions)
                    {
                        if (other.Target?.Instruction == instruction)
                            other.Target.Instruction = instructions[i];
                        if (other.Target2?.Instruction == instruction)
                            other.Target2.Instruction = instructions[i];
                    }
                }
                else
                {
                    i++;
                }
            }
        }

        public static void CompressJumps(IReadOnlyList<Instruction> instructions)
        {
            bool compressed;
            do
            {
                compressed = false;
                foreach (Instruction instruction in instructions)
                {
                    if (instruction.Target is null) continue;
                    if (instruction.OpCode >= OpCode.JMP && instruction.OpCode <= OpCode.CALL_L)
                    {
                        if ((instruction.OpCode - OpCode.JMP) % 2 == 0) continue;
                    }
                    else
                    {
                        if (instruction.OpCode != OpCode.TRY_L && instruction.OpCode != OpCode.ENDTRY_L) continue;
                    }
                    if (instruction.OpCode == OpCode.TRY_L)
                    {
                        int offset1 = instruction.Target.Instruction?.Offset - instruction.Offset ?? 0;
                        int offset2 = instruction.Target2!.Instruction?.Offset - instruction.Offset ?? 0;
                        if (offset1 >= sbyte.MinValue && offset1 <= sbyte.MaxValue && offset2 >= sbyte.MinValue && offset2 <= sbyte.MaxValue)
                        {
                            compressed = true;
                            instruction.OpCode--;
                        }
                    }
                    else
                    {
                        int offset = instruction.Target.Instruction!.Offset - instruction.Offset;
                        if (offset >= sbyte.MinValue && offset <= sbyte.MaxValue)
                        {
                            compressed = true;
                            instruction.OpCode--;
                        }
                    }
                }
                if (compressed) instructions.RebuildOffsets();
            } while (compressed);
        }
    }
}
