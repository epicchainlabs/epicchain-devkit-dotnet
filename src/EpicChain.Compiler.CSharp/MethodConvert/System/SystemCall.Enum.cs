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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using EpicChain.SmartContract.Native;
using EpicChain.VM;

namespace EpicChain.Compiler;

internal partial class MethodConvert
{
    private static void HandleEnumParse(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);

        // Get the enum type from the first argument (typeof(enum))
        ITypeSymbol? enumTypeSymbol = null;
        if (arguments is { Count: > 0 })
        {
            if ((arguments[0] as ArgumentSyntax).Expression is TypeOfExpressionSyntax typeOfExpression)
            {
                var typeInfo = model.GetTypeInfo(typeOfExpression.Type);
                enumTypeSymbol = typeInfo.Type;
            }
            else
            {
                throw new CompilationException(arguments[0], DiagnosticId.InvalidArgument, "First argument must be a typeof(enum)");
            }
        }
        else
        {
            throw new CompilationException(symbol.Locations.FirstOrDefault().MetadataModule, DiagnosticId.InvalidArgument, "Enum.Parse requires at least two arguments");
        }

        if (enumTypeSymbol is not { TypeKind: TypeKind.Enum })
        {
            throw new CompilationException(arguments![0], DiagnosticId.InvalidType, "Unable to determine enum type from first argument");
        }

        var enumMembers = enumTypeSymbol.GetMembers().OfType<IFieldSymbol>()
            .Where(field => field is { HasConstantValue: true, IsImplicitlyDeclared: false }).ToArray();

        foreach (var t in enumMembers)
        {
            // Duplicate inputString
            methodConvert.Dup();                      // Stack: [type, inputString,inputString]

            // Push enum name
            methodConvert.Push(t.Name);  // Stack: [type, inputString,inputString, enumName]

            // Equal comparison
            methodConvert.Equal();                    // Stack: [type,inputString, isEqual]

            var nextCheck = new JumpTarget();
            // If not equal, discard duplicated inputString and proceed to next
            methodConvert.Jump(OpCode.JMPIFNOT, nextCheck);

            // If equal:
            // Push enum value
            methodConvert.Push(t.ConstantValue); // Stack: [type, inputString, enumValue]
            methodConvert.Reverse3();
            methodConvert.Drop(2);
            methodConvert.AddInstruction(OpCode.RET);

            nextCheck.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        }

        // No match found
        // Remove the inputString from the stack
        methodConvert.Drop();
        methodConvert.Push("No such enum value");
        methodConvert.Throw();
    }

    private static void HandleEnumParseIgnoreCase(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);

        // Get the enum type from the first argument (typeof(enum))
        ITypeSymbol? enumTypeSymbol = null;
        if (arguments is { Count: > 0 })
        {
            if ((arguments[0] as ArgumentSyntax).Expression is TypeOfExpressionSyntax typeOfExpression)
            {
                var typeInfo = model.GetTypeInfo(typeOfExpression.Type);
                enumTypeSymbol = typeInfo.Type;
            }
            else
            {
                throw new CompilationException(arguments[0], DiagnosticId.InvalidArgument, "First argument must be a typeof(enum)");
            }
        }
        else
        {
            throw new CompilationException(symbol.Locations.FirstOrDefault().MetadataModule, DiagnosticId.InvalidArgument, "Enum.Parse requires at least two arguments");
        }

        if (enumTypeSymbol is not { TypeKind: TypeKind.Enum })
        {
            throw new CompilationException(arguments![0], DiagnosticId.InvalidType, "Unable to determine enum type from first argument");
        }

        var enumMembers = enumTypeSymbol.GetMembers().OfType<IFieldSymbol>()
            .Where(field => field is { HasConstantValue: true, IsImplicitlyDeclared: false }).ToArray();

        var ignoreCase = new JumpTarget();
        var ignoreCase2 = new JumpTarget();
        methodConvert.JumpIfNot(ignoreCase);
        ConvertToUpper(methodConvert);            // Convert inputString to upper case
        ignoreCase.Instruction = methodConvert.Nop();
        foreach (var t in enumMembers)
        {
            // Duplicate inputString
            methodConvert.Dup();                      // Stack: [..., inputString, inputString]
            methodConvert.AddInstruction(OpCode.LDARG1);
            methodConvert.JumpIfNot(ignoreCase2);
            JumpTarget endCase = new JumpTarget();
            // Push enum name
            methodConvert.Push(t.Name.ToUpper());               // Stack: [..., inputString, inputString, enumName]
            methodConvert.Jump(endCase);
            ignoreCase2.Instruction = methodConvert.Nop();
            methodConvert.Push(t.Name);
            endCase.Instruction = methodConvert.Nop();

            // Equal comparison
            methodConvert.Equal();                    // Stack: [..., inputString, isEqual]

            var nextCheck = new JumpTarget();
            // If not equal, discard duplicated inputString and proceed to next
            methodConvert.Jump(OpCode.JMPIFNOT, nextCheck);

            // If equal:
            // Remove the duplicated inputString from the stack
            methodConvert.Drop(3);
            // Push enum value
            methodConvert.Push(t.ConstantValue);
            methodConvert.AddInstruction(OpCode.RET);

            nextCheck.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        }

        // No match found
        // Remove the inputString from the stack
        methodConvert.Drop(2);
        methodConvert.Push("No such enum value");
        methodConvert.Throw();
    }

    private static void HandleEnumTryParseIgnoreCase(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);

        // Get the enum type from the first argument (typeof(enum))
        ITypeSymbol? enumTypeSymbol = null;
        if (arguments is { Count: > 2 })
        {
            if ((arguments[0] as ArgumentSyntax).Expression is TypeOfExpressionSyntax typeOfExpression)
            {
                var typeInfo = model.GetTypeInfo(typeOfExpression.Type);
                enumTypeSymbol = typeInfo.Type;
            }
            else
            {
                throw new CompilationException(arguments[0], DiagnosticId.InvalidArgument, "First argument must be a typeof(enum)");
            }
        }
        else
        {
            throw new CompilationException(symbol.Locations.FirstOrDefault().MetadataModule, DiagnosticId.InvalidArgument, "Enum.TryParse requires at least three arguments");
        }

        if (enumTypeSymbol is not { TypeKind: TypeKind.Enum })
        {
            throw new CompilationException(arguments![0], DiagnosticId.InvalidType, "Unable to determine enum type from first argument");
        }

        if (!methodConvert._context.TryGetCapturedStaticField(symbol.Parameters[3], out var index))
            throw new CompilationException(symbol, DiagnosticId.SyntaxNotSupported, "Out parameter must be captured in a static field.");

        var enumMembers = enumTypeSymbol.GetMembers().OfType<IFieldSymbol>()
            .Where(field => field is { HasConstantValue: true, IsImplicitlyDeclared: false }).ToArray();
        var ignoreCase = new JumpTarget();
        var ignoreCase2 = new JumpTarget();
        methodConvert.Drop();
        methodConvert.JumpIfNot(ignoreCase);
        methodConvert.Swap();
        methodConvert.Drop();
        ConvertToUpper(methodConvert);            // Convert inputString to upper case
        ignoreCase.Instruction = methodConvert.Nop();
        foreach (var t in enumMembers)
        {
            // Duplicate inputString
            methodConvert.Dup();                      // Stack: [..., inputString, inputString]
            methodConvert.AddInstruction(OpCode.LDARG1);
            methodConvert.JumpIfNot(ignoreCase2);
            JumpTarget endCase = new JumpTarget();
            // Push enum name
            methodConvert.Push(t.Name.ToUpper());               // Stack: [..., inputString, inputString, enumName]
            methodConvert.Jump(endCase);
            ignoreCase2.Instruction = methodConvert.Nop();
            methodConvert.Push(t.Name);
            endCase.Instruction = methodConvert.Nop();

            // Equal comparison
            methodConvert.Equal();                    // Stack: [..., inputString, isEqual]

            var nextCheck = new JumpTarget();
            // If not equal, discard duplicated inputString and proceed to next
            methodConvert.Jump(OpCode.JMPIFNOT, nextCheck);

            // If equal:
            // Remove the duplicated inputString from the stack
            methodConvert.Drop(2);
            // Push enum value
            methodConvert.Push(t.ConstantValue);
            // Store the result in the out parameter
            methodConvert.AccessSlot(OpCode.STSFLD, index);
            // Push true to indicate success
            methodConvert.Push(true);
            methodConvert.AddInstruction(OpCode.RET);

            nextCheck.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        }

        // No match found
        // Remove the inputString from the stack
        methodConvert.Drop(2);
        // Push default value (0) for the out parameter
        methodConvert.Push(0);
        methodConvert.AccessSlot(OpCode.STSFLD, index);
        // Push false to indicate failure
        methodConvert.Push(false);
        methodConvert.AddInstruction(OpCode.RET);
    }

    private static void ConvertToUpper(MethodConvert methodConvert)
    {
        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        var charIsLower = new JumpTarget();
        methodConvert.Push(""); // Create an empty ByteString

        // methodConvert.AddInstruction(OpCode.LDARG0); // Load the string | arr str
        methodConvert.AddInstruction(OpCode.PUSH0); // Push the initial index (0)  arr str 0
        loopStart.Instruction = methodConvert.AddInstruction(OpCode.NOP);

        methodConvert.AddInstruction(OpCode.DUP); // Duplicate the index
        methodConvert.AddInstruction(OpCode.LDARG0); // Load the string
        methodConvert.AddInstruction(OpCode.SIZE); // Get the length of the string
        methodConvert.AddInstruction(OpCode.LT); // Check if index < length
        methodConvert.Jump(OpCode.JMPIFNOT, loopEnd); // If not, exit the loop

        methodConvert.AddInstruction(OpCode.DUP); // Duplicate the index | arr str 0 0
        methodConvert.AddInstruction(OpCode.LDARG0); // Load the string
        methodConvert.Swap();
        methodConvert.AddInstruction(OpCode.PICKITEM); // Get the character at the current index
        methodConvert.AddInstruction(OpCode.DUP); // Duplicate the character
        methodConvert.Push((ushort)'a'); // Push 'a'
        methodConvert.Push((ushort)'z' + 1); // Push 'z' + 1
        methodConvert.AddInstruction(OpCode.WITHIN); // Check if character is within 'a' to 'z'
        methodConvert.Jump(OpCode.JMPIF, charIsLower); // If true, jump to charIsLower
        methodConvert.Rot();
        methodConvert.Swap();
        methodConvert.AddInstruction(OpCode.CAT); // Append the original character to the array
        methodConvert.Swap();
        methodConvert.Inc();
        methodConvert.Jump(OpCode.JMP, loopStart); // Jump to the start of the loop

        charIsLower.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.Push((ushort)'a'); // Push 'a'
        methodConvert.AddInstruction(OpCode.SUB); // Subtract 'a' from the character
        methodConvert.Push((ushort)'A'); // Push 'A'
        methodConvert.AddInstruction(OpCode.ADD); // Add 'A' to the result
        methodConvert.Rot();
        methodConvert.Swap();
        methodConvert.AddInstruction(OpCode.CAT); // Append the upper case character to the array
        methodConvert.Swap();
        methodConvert.Inc();
        methodConvert.Jump(OpCode.JMP, loopStart); // Jump to the start of the loop

        loopEnd.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.Drop();
        methodConvert.ChangeType(VM.Types.StackItemType.ByteString); // Convert the array to a byte string
    }

    private static void HandleEnumIsDefinedByName(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);

        // Get the enum type from the first argument (typeof(enum))
        ITypeSymbol? enumTypeSymbol = null;
        if (arguments is { Count: > 0 })
        {
            if ((arguments[0] as ArgumentSyntax).Expression is TypeOfExpressionSyntax typeOfExpression)
            {
                var typeInfo = model.GetTypeInfo(typeOfExpression.Type);
                enumTypeSymbol = typeInfo.Type;
            }
            else
            {
                throw new CompilationException(arguments[0], DiagnosticId.InvalidArgument, "First argument must be a typeof(enum)");
            }
        }
        else
        {
            throw new CompilationException(symbol.Locations.FirstOrDefault().MetadataModule, DiagnosticId.InvalidArgument, "Enum.IsDefined requires at least two arguments");
        }

        if (enumTypeSymbol is not { TypeKind: TypeKind.Enum })
        {
            throw new CompilationException(arguments![0], DiagnosticId.InvalidType, "Unable to determine enum type from first argument");
        }

        var enumMembers = enumTypeSymbol.GetMembers().OfType<IFieldSymbol>()
            .Where(field => field is { HasConstantValue: true, IsImplicitlyDeclared: false }).ToArray();

        foreach (var t in enumMembers)
        {
            methodConvert.Dup();                      // Duplicate input name
            methodConvert.Push(t.Name);               // Push enum name
            methodConvert.Equal();                    // Compare strings

            var nextCheck = new JumpTarget();
            methodConvert.JumpIfNot(nextCheck);       // If not equal, check next

            methodConvert.Drop();                     // Remove the duplicated input name
            methodConvert.Push(true);                 // Push true (enum name is defined)
            methodConvert.AddInstruction(OpCode.RET); // Return true

            nextCheck.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        }

        // No match found
        methodConvert.Drop();                         // Remove the input name
        methodConvert.Push(false);                    // Push false (enum name is not defined)
        methodConvert.AddInstruction(OpCode.RET);     // Return false
    }

    private static void HandleEnumGetName(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);

        // Get the enum type from the Enum value
        var enumType = symbol.Parameters[0].Type;

        if (enumType is not INamedTypeSymbol enumTypeSymbol)
        {
            throw new CompilationException(symbol.Locations.FirstOrDefault().MetadataModule, DiagnosticId.InvalidType, "Unable to determine enum type");
        }

        var enumMembers = enumTypeSymbol.GetMembers().OfType<IFieldSymbol>()
            .Where(field => field is { HasConstantValue: true, IsImplicitlyDeclared: false }).ToArray();

        foreach (var t in enumMembers)
        {
            methodConvert.Dup();                      // Duplicate input value
            methodConvert.Push(t.ConstantValue);      // Push enum value
            methodConvert.Equal();                    // Compare values

            var nextCheck = new JumpTarget();
            methodConvert.JumpIfNot(nextCheck);       // If not equal, check next

            methodConvert.Drop();                     // Remove the duplicated input value
            methodConvert.Push(t.Name);               // Push enum name
            methodConvert.AddInstruction(OpCode.RET); // Return enum name

            nextCheck.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        }

        // No match found
        methodConvert.Drop();                         // Remove the input value
        methodConvert.AddInstruction(OpCode.PUSHNULL);                     // Push null (no matching enum name)
        methodConvert.AddInstruction(OpCode.RET);     // Return null
    }

    private static void HandleEnumGetNameWithType(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);

        // Get the enum type from the first argument (typeof(enum))
        ITypeSymbol? enumTypeSymbol = null;
        if (arguments is { Count: > 0 })
        {
            if ((arguments[0] as ArgumentSyntax).Expression is TypeOfExpressionSyntax typeOfExpression)
            {
                var typeInfo = model.GetTypeInfo(typeOfExpression.Type);
                enumTypeSymbol = typeInfo.Type;
            }
            else
            {
                throw new CompilationException(arguments[0], DiagnosticId.InvalidArgument, "First argument must be a typeof(enum)");
            }
        }
        else
        {
            throw new CompilationException(symbol.Locations.FirstOrDefault().MetadataModule, DiagnosticId.InvalidArgument, "Enum.GetName requires at least two arguments");
        }

        if (enumTypeSymbol is not { TypeKind: TypeKind.Enum })
        {
            throw new CompilationException(arguments![0], DiagnosticId.InvalidType, "Unable to determine enum type from first argument");
        }

        var enumMembers = enumTypeSymbol.GetMembers().OfType<IFieldSymbol>()
            .Where(field => field is { HasConstantValue: true, IsImplicitlyDeclared: false }).ToArray();

        foreach (var t in enumMembers)
        {
            methodConvert.Dup();                      // Duplicate input value
            methodConvert.Push(t.ConstantValue);      // Push enum value
            methodConvert.Equal();                    // Compare values

            var nextCheck = new JumpTarget();
            methodConvert.JumpIfNot(nextCheck);       // If not equal, check next

            methodConvert.Drop(2);                     // Remove the duplicated input value
            methodConvert.Push(t.Name);               // Push enum name
            methodConvert.AddInstruction(OpCode.RET); // Return enum name

            nextCheck.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        }

        // No match found
        methodConvert.Drop(2);                         // Remove the input value
        methodConvert.AddInstruction(OpCode.PUSHNULL);                     // Push null (no matching enum name)
        methodConvert.AddInstruction(OpCode.RET);     // Return null
    }

    private static void HandleEnumTryParse(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);

        if (!methodConvert._context.TryGetCapturedStaticField(symbol.Parameters[2], out var index))
            throw new CompilationException(symbol, DiagnosticId.SyntaxNotSupported, "Out parameter must be captured in a static field.");

        // Get the enum type from the first argument (typeof(enum))
        ITypeSymbol? enumTypeSymbol = null;
        if (arguments is { Count: > 0 })
        {
            if ((arguments[0] as ArgumentSyntax).Expression is TypeOfExpressionSyntax typeOfExpression)
            {
                var typeInfo = model.GetTypeInfo(typeOfExpression.Type);
                enumTypeSymbol = typeInfo.Type;
            }
            else
            {
                throw new CompilationException(arguments[0], DiagnosticId.InvalidArgument, "First argument must be a typeof(enum)");
            }
        }
        else
        {
            throw new CompilationException(symbol.Locations.FirstOrDefault().MetadataModule, DiagnosticId.InvalidArgument, "Enum.TryParse requires at least three arguments");
        }

        if (enumTypeSymbol is not { TypeKind: TypeKind.Enum })
        {
            throw new CompilationException(arguments[0], DiagnosticId.InvalidType, "Unable to determine enum type from first argument");
        }

        var enumMembers = enumTypeSymbol.GetMembers().OfType<IFieldSymbol>()
            .Where(field => field is { HasConstantValue: true, IsImplicitlyDeclared: false }).ToArray();

        JumpTarget endTarget = new();
        methodConvert.Drop();
        foreach (var t in enumMembers)
        {
            methodConvert.Dup();                      // Stack: [..., inputString, inputString]
            methodConvert.Push(t.Name);               // Stack: [..., inputString, inputString, enumName]

            // Equal comparison
            methodConvert.Equal();                    // Stack: [..., inputString, isEqual]

            JumpTarget nextCheck = new();
            methodConvert.JumpIfNot(nextCheck);

            // If equal:
            methodConvert.Drop(2);                     // Remove the inputString
            methodConvert.Push(t.ConstantValue);      // Stack: [..., enumValue]
            methodConvert.AccessSlot(OpCode.STSFLD, index); // Store enum value in out parameter
            methodConvert.Push(true);                 // Stack: [..., true]
            methodConvert.Ret();

            nextCheck.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        }

        // No match found
        methodConvert.Drop(2);                         // Remove the inputString
        methodConvert.Push(0);                        // Default enum value
        methodConvert.AccessSlot(OpCode.STSFLD, index); // Store default value in out parameter
        methodConvert.Push(false);                    // Success flag set to false

        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleEnumGetNames(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);

        // Get the enum type from the first argument (typeof(enum))
        ITypeSymbol? enumTypeSymbol = null;
        if (arguments is { Count: > 0 })
        {
            if ((arguments[0] as ArgumentSyntax).Expression is TypeOfExpressionSyntax typeOfExpression)
            {
                var typeInfo = model.GetTypeInfo(typeOfExpression.Type);
                enumTypeSymbol = typeInfo.Type;
            }
            else
            {
                throw new CompilationException(arguments[0], DiagnosticId.InvalidArgument, "First argument must be a typeof(enum)");
            }
        }
        else
        {
            throw new CompilationException(symbol.Locations.FirstOrDefault().MetadataModule, DiagnosticId.InvalidArgument, "Enum.GetNames requires one argument");
        }

        if (enumTypeSymbol is not { TypeKind: TypeKind.Enum })
        {
            throw new CompilationException(arguments![0], DiagnosticId.InvalidType, "Unable to determine enum type from first argument");
        }

        var enumMembers = enumTypeSymbol.GetMembers().OfType<IFieldSymbol>()
            .Where(field => field is { HasConstantValue: true, IsImplicitlyDeclared: false }).ToArray();

        // Create an array of names
        methodConvert.Push(enumMembers.Length);       // Stack: [..., size]
        methodConvert.AddInstruction(OpCode.NEWARRAY); // Stack: [..., array]

        for (int i = 0; i < enumMembers.Length; i++)
        {
            methodConvert.Dup();                      // Duplicate array reference
            methodConvert.Push(i);                    // Index
            methodConvert.Push(enumMembers[i].Name);  // Name
            methodConvert.AddInstruction(OpCode.SETITEM); // array[i] = name
        }

        methodConvert.AddInstruction(OpCode.RET);
    }

    private static void HandleEnumGetValues(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);

        // Get the enum type from the first argument (typeof(enum))
        ITypeSymbol? enumTypeSymbol = null;
        if (arguments is { Count: > 0 })
        {
            if ((arguments[0] as ArgumentSyntax).Expression is TypeOfExpressionSyntax typeOfExpression)
            {
                var typeInfo = model.GetTypeInfo(typeOfExpression.Type);
                enumTypeSymbol = typeInfo.Type;
            }
            else
            {
                throw new CompilationException(arguments[0], DiagnosticId.InvalidArgument, "First argument must be a typeof(enum)");
            }
        }
        else
        {
            throw new CompilationException(symbol.Locations.FirstOrDefault().MetadataModule, DiagnosticId.InvalidArgument, "Enum.GetValues requires one argument");
        }

        if (enumTypeSymbol is not { TypeKind: TypeKind.Enum })
        {
            throw new CompilationException(arguments![0], DiagnosticId.InvalidType, "Unable to determine enum type from first argument");
        }

        var enumMembers = enumTypeSymbol.GetMembers().OfType<IFieldSymbol>()
            .Where(field => field is { HasConstantValue: true, IsImplicitlyDeclared: false }).ToArray();

        // Create an array of values
        methodConvert.Push(enumMembers.Length);       // Stack: [..., size]
        methodConvert.AddInstruction(OpCode.NEWARRAY); // Stack: [..., array]

        for (int i = 0; i < enumMembers.Length; i++)
        {
            methodConvert.Dup();                      // Duplicate array reference
            methodConvert.Push(i);                    // Index
            methodConvert.Push(enumMembers[i].ConstantValue); // Value
            methodConvert.AddInstruction(OpCode.SETITEM); // array[i] = value
        }

        methodConvert.AddInstruction(OpCode.RET);
    }

    private static void HandleEnumIsDefined(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);

        // Get the enum type from the first argument (typeof(enum))
        ITypeSymbol? enumTypeSymbol = null;
        if (arguments is { Count: > 0 })
        {
            if ((arguments[0] as ArgumentSyntax).Expression is TypeOfExpressionSyntax typeOfExpression)
            {
                var typeInfo = model.GetTypeInfo(typeOfExpression.Type);
                enumTypeSymbol = typeInfo.Type;
            }
            else
            {
                throw new CompilationException(arguments[0], DiagnosticId.InvalidArgument, "First argument must be a typeof(enum)");
            }
        }
        else
        {
            throw new CompilationException(symbol.Locations.FirstOrDefault().MetadataModule, DiagnosticId.InvalidArgument, "Enum.IsDefined requires at least two arguments");
        }

        if (enumTypeSymbol is not { TypeKind: TypeKind.Enum })
        {
            throw new CompilationException(arguments![0], DiagnosticId.InvalidType, "Unable to determine enum type from first argument");
        }

        var enumMembers = enumTypeSymbol.GetMembers().OfType<IFieldSymbol>()
            .Where(field => field is { HasConstantValue: true, IsImplicitlyDeclared: false }).ToArray();

        var valueType = model.GetTypeInfo((arguments[1] as ArgumentSyntax)?.Expression).Type;

        // We need to compare the input value against the enum values
        var endLabel = new JumpTarget();
        // here add check logic to verify if valueType is string
        var isName = valueType is INamedTypeSymbol { Name: "String" };

        foreach (var t in enumMembers)
        {
            methodConvert.Dup();                      // Duplicate value to compare
            if (isName)
            {
                methodConvert.Push(t.Name);
            }
            else
            {
                methodConvert.Push(t.ConstantValue);
            }

            // Equal comparison
            methodConvert.Equal();                    // Stack: [..., isEqual]

            var nextCheck = new JumpTarget();
            methodConvert.JumpIfNot(nextCheck);

            // If equal, set result to true
            methodConvert.Drop();
            methodConvert.Drop();                     // Remove the false
            methodConvert.Push(true);                 // Set result to true
            methodConvert.Ret();
            nextCheck.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        }
        methodConvert.Drop();
        methodConvert.Drop(); // Remove the duplicated value
        methodConvert.Push(false);
    }
}
