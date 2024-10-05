// Copyright (C) 2021-2024 EpicChain Lab's
//
// The EpicChain.Compiler.CSharp is open-source software that is distributed under the widely recognized and permissive MIT License.
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


extern alias scfx;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using EpicChain.VM;
using System;
using System.Buffers.Binary;
using System.Numerics;
using scfx::EpicChain.SmartContract.Framework;
using OpCode = EpicChain.VM.OpCode;

namespace EpicChain.Compiler;

internal partial class MethodConvert
{
    #region Instructions
    private Instruction AddInstruction(Instruction instruction)
    {
        _instructions.Add(instruction);
        return instruction;
    }

    private Instruction AddInstruction(OpCode opcode)
    {
        return AddInstruction(new Instruction
        {
            OpCode = opcode
        });
    }

    private SequencePointInserter InsertSequencePoint(SyntaxNodeOrToken? syntax)
    {
        return new SequencePointInserter(_instructions, syntax);
    }

    private SequencePointInserter InsertSequencePoint(SyntaxReference? syntax)
    {
        return new SequencePointInserter(_instructions, syntax);
    }

    private SequencePointInserter InsertSequencePoint(Location? location)
    {
        return new SequencePointInserter(_instructions, location);
    }

    #endregion

    private Instruction Jump(OpCode opcode, JumpTarget target)
    {
        return AddInstruction(new Instruction
        {
            OpCode = opcode,
            Target = target
        });
    }

    private void Push(bool value)
    {
        AddInstruction(value ? OpCode.PUSHT : OpCode.PUSHF);
    }

    private Instruction Ret() => AddInstruction(OpCode.RET);

    private Instruction Push(BigInteger number)
    {
        if (number >= -1 && number <= 16) return AddInstruction(number == -1 ? OpCode.PUSHM1 : OpCode.PUSH0 + (byte)(int)number);
        Span<byte> buffer = stackalloc byte[32];
        if (!number.TryWriteBytes(buffer, out var bytesWritten, isUnsigned: false, isBigEndian: false))
            throw new ArgumentOutOfRangeException(nameof(number));
        var instruction = bytesWritten switch
        {
            1 => new Instruction
            {
                OpCode = OpCode.PUSHINT8,
                Operand = PadRight(buffer, bytesWritten, 1, number.Sign < 0).ToArray()
            },
            2 => new Instruction
            {
                OpCode = OpCode.PUSHINT16,
                Operand = PadRight(buffer, bytesWritten, 2, number.Sign < 0).ToArray()
            },
            <= 4 => new Instruction
            {
                OpCode = OpCode.PUSHINT32,
                Operand = PadRight(buffer, bytesWritten, 4, number.Sign < 0).ToArray()
            },
            <= 8 => new Instruction
            {
                OpCode = OpCode.PUSHINT64,
                Operand = PadRight(buffer, bytesWritten, 8, number.Sign < 0).ToArray()
            },
            <= 16 => new Instruction
            {
                OpCode = OpCode.PUSHINT128,
                Operand = PadRight(buffer, bytesWritten, 16, number.Sign < 0).ToArray()
            },
            <= 32 => new Instruction
            {
                OpCode = OpCode.PUSHINT256,
                Operand = PadRight(buffer, bytesWritten, 32, number.Sign < 0).ToArray()
            },
            _ => throw new ArgumentOutOfRangeException($"Number too large: {bytesWritten}")
        };
        AddInstruction(instruction);
        return instruction;
    }

    private Instruction Push(string s)
    {
        return Push(Utility.StrictUTF8.GetBytes(s));
    }

    private Instruction Push(byte[] data)
    {
        OpCode opcode;
        byte[] buffer;
        switch (data.Length)
        {
            case <= byte.MaxValue:
                opcode = OpCode.PUSHDATA1;
                buffer = new byte[sizeof(byte) + data.Length];
                buffer[0] = (byte)data.Length;
                Buffer.BlockCopy(data, 0, buffer, sizeof(byte), data.Length);
                break;
            case <= ushort.MaxValue:
                opcode = OpCode.PUSHDATA2;
                buffer = new byte[sizeof(ushort) + data.Length];
                BinaryPrimitives.WriteUInt16LittleEndian(buffer, (ushort)data.Length);
                Buffer.BlockCopy(data, 0, buffer, sizeof(ushort), data.Length);
                break;
            default:
                opcode = OpCode.PUSHDATA4;
                buffer = new byte[sizeof(uint) + data.Length];
                BinaryPrimitives.WriteUInt32LittleEndian(buffer, (uint)data.Length);
                Buffer.BlockCopy(data, 0, buffer, sizeof(uint), data.Length);
                break;
        }
        return AddInstruction(new Instruction
        {
            OpCode = opcode,
            Operand = buffer
        });
    }

    private void Push(object? obj)
    {
        switch (obj)
        {
            case bool data:
                Push(data);
                break;
            case byte[] data:
                Push(data);
                break;
            case string data:
                Push(data);
                break;
            case BigInteger data:
                Push(data);
                break;
            case char data:
                Push((ushort)data);
                break;
            case sbyte data:
                Push(data);
                break;
            case byte data:
                Push(data);
                break;
            case short data:
                Push(data);
                break;
            case ushort data:
                Push(data);
                break;
            case int data:
                Push(data);
                break;
            case uint data:
                Push(data);
                break;
            case long data:
                Push(data);
                break;
            case ulong data:
                Push(data);
                break;
            case Enum data:
                Push(BigInteger.Parse(data.ToString("d")));
                break;
            case null:
                AddInstruction(OpCode.PUSHNULL);
                break;
            case float or double or decimal:
                throw new CompilationException(DiagnosticId.FloatingPointNumber, "Floating-point numbers are not supported.");
            default:
                throw new NotSupportedException($"Unsupported constant value: {obj}");
        }
    }

    private Instruction PushDefault(ITypeSymbol type)
    {
        return AddInstruction(type.GetStackItemType() switch
        {
            VM.Types.StackItemType.Boolean or VM.Types.StackItemType.Integer => OpCode.PUSH0,
            _ => OpCode.PUSHNULL,
        });
    }

    // Helper method to reverse stack items
    private void ReverseStackItems(int count)
    {
        switch (count)
        {
            case 2:
                AddInstruction(OpCode.SWAP);
                break;
            case 3:
                AddInstruction(OpCode.REVERSE3);
                break;
            case 4:
                AddInstruction(OpCode.REVERSE4);
                break;
            default:
                Push(count);
                AddInstruction(OpCode.REVERSEN);
                break;
        }
    }

    #region LabelsAndTargets

    private JumpTarget AddLabel(ILabelSymbol symbol, bool checkTryStack)
    {
        if (!_labels.TryGetValue(symbol, out JumpTarget? target))
        {
            target = new JumpTarget();
            _labels.Add(symbol, target);
        }
        if (checkTryStack && _tryStack.TryPeek(out ExceptionHandling? result) && result.State != ExceptionHandlingState.Finally)
        {
            result.Labels.Add(symbol);
        }
        return target;
    }

    #endregion

    private void PushSwitchLabels((SwitchLabelSyntax, JumpTarget)[] labels)
    {
        _switchStack.Push(labels);
        if (_tryStack.TryPeek(out ExceptionHandling? result))
            result.SwitchCount++;
    }

    private void PopSwitchLabels()
    {
        _switchStack.Pop();
        if (_tryStack.TryPeek(out ExceptionHandling? result))
            result.SwitchCount--;
    }

    private void PushContinueTarget(JumpTarget target)
    {
        _continueTargets.Push(target);
        if (_tryStack.TryPeek(out ExceptionHandling? result))
            result.ContinueTargetCount++;
    }

    private void PopContinueTarget()
    {
        _continueTargets.Pop();
        if (_tryStack.TryPeek(out ExceptionHandling? result))
            result.ContinueTargetCount--;
    }

    private void PushBreakTarget(JumpTarget target)
    {
        _breakTargets.Push(target);
        if (_tryStack.TryPeek(out ExceptionHandling? result))
            result.BreakTargetCount++;
    }

    private void PopBreakTarget()
    {
        _breakTargets.Pop();
        if (_tryStack.TryPeek(out ExceptionHandling? result))
            result.BreakTargetCount--;
    }

    private static ReadOnlySpan<byte> PadRight(Span<byte> buffer, int dataLength, int padLength, bool negative)
    {
        byte pad = negative ? (byte)0xff : (byte)0;
        for (int x = dataLength; x < padLength; x++)
            buffer[x] = pad;
        return buffer[..padLength];
    }

    /// <summary>
    /// Convert a throw expression or throw statement to OpCodes.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about the Throw.</param>
    /// <param name="exception">The content of exception</param>
    /// <exception cref="CompilationException">Only a single parameter is supported for exceptions.</exception>
    /// <example>
    /// throw statement:
    /// <code>
    /// if (shapeAmount <= 0)
    /// {
    ///     throw new Exception("Amount of shapes must be positive.");
    /// }
    ///</code>
    /// throw expression:
    /// <code>
    /// string a = null;
    /// var b = a ?? throw new Exception();
    /// </code>
    /// <code>
    /// var first = args.Length >= 1 ? args[0] : throw new Exception();
    /// </code>
    /// </example>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/exception-handling-statements#the-throw-expression">The throw expression</seealso>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/exception-handling-statements#the-try-catch-statement">Exception-handling statements - throw</seealso>
    private void Throw(SemanticModel model, ExpressionSyntax? exception)
    {
        if (exception is not null)
        {
            var type = model.GetTypeInfo(exception).Type!;
            if (type.IsSubclassOf(nameof(UncatchableException), includeThisClass: true))
            {
                AddInstruction(OpCode.ABORT);
                return;
            }
        }
        switch (exception)
        {
            case ObjectCreationExpressionSyntax expression:
                switch (expression.ArgumentList?.Arguments.Count)
                {
                    case null:
                    case 0:
                        Push("exception");
                        break;
                    case 1:
                        ConvertExpression(model, expression.ArgumentList.Arguments[0].Expression);
                        break;
                    default:
                        throw new CompilationException(expression, DiagnosticId.MultiplyThrows, "Only a single parameter is supported for exceptions.");
                }
                break;
            case null:
                AccessSlot(OpCode.LDLOC, _exceptionStack.Peek());
                break;
            default:
                ConvertExpression(model, exception);
                break;
        }
        AddInstruction(OpCode.THROW);
    }

    private Instruction IsType(VM.Types.StackItemType type)
    {
        return AddInstruction(new Instruction
        {
            OpCode = OpCode.ISTYPE,
            Operand = [(byte)type]
        });
    }

    private Instruction ChangeType(VM.Types.StackItemType type)
    {
        return AddInstruction(new Instruction
        {
            OpCode = OpCode.CONVERT,
            Operand = [(byte)type]
        });
    }
}
