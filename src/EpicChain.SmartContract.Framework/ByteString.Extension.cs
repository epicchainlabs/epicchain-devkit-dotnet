// Copyright (C) 2021-2024 EpicChain Labs.
//
// ByteString.Extension.cs file is a crucial component of the EpicChain project and is freely distributed as open-source software.
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


using EpicChain.SmartContract.Framework.Native;
namespace EpicChain.SmartContract.Framework;

public static class ByteStringExtension
{
    /// <summary>
    ///  Denotes whether provided character is a number.
    /// </summary>
    /// <param name="byteString">Input to check</param>
    /// <returns>True if is number</returns>
    public static bool IsNumber(this ByteString byteString)
    {
        foreach (var value in byteString)
        {
            if (value is < 48 or > 57)
                return false;
        }
        return true;
    }

    /// <summary>
    ///  Denotes whether provided character is a lowercase letter.
    /// </summary>
    /// <param name="byteString">Input to check</param>
    /// <returns>True if is Alpha character</returns>
    public static bool IsLowerAlphabet(this ByteString byteString)
    {
        foreach (var value in byteString)
        {
            if (value is < 97 or > 122)
                return false;
        }
        return true;
    }

    /// <summary>
    ///  Denotes whether provided character is a lowercase letter.
    /// </summary>
    /// <param name="byteString">Input to check</param>
    /// <returns>True if is Alpha character</returns>
    public static bool IsUpperAlphabet(this ByteString byteString)
    {
        foreach (var value in byteString)
        {
            if (value is < 65 or > 90)
                return false;
        }
        return true;
    }

    /// <summary>
    ///  Denotes whether provided character is a lowercase letter.
    /// </summary>
    /// <param name="byteString">Input to check</param>
    /// <returns>True if is Alpha character</returns>
    public static bool IsAlphabet(this ByteString byteString)
    {
        foreach (var value in byteString)
        {
            if (!((value >= 65 && value <= 90) || (value >= 97 && value <= 122)))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Returns the index of the first occurrence of a given value in an array.
    /// </summary>
    /// <param name="byteString">Array where to search.</param>
    /// <param name="byteToFind">Array to search.</param>
    /// <returns>Index where it is located or -1</returns>
    public static int IndexOf(this ByteString byteString, ByteString byteToFind)
    {
        return StdLib.MemorySearch(byteString, byteToFind);
    }

    /// <summary>
    /// Determines whether the beginning of this string instance matches the specified string when compared using the specified culture.
    /// </summary>
    /// <param name="byteString">Array where to search.</param>
    /// <param name="byteToFind">Array to search.</param>
    /// <returns>True if start with</returns>
    public static bool StartWith(this ByteString byteString, ByteString byteToFind)
    {
        return StdLib.MemorySearch(byteString, byteToFind) == 0;
    }

    /// <summary>
    /// Determines whether the end of this string instance matches a specified string.
    /// </summary>
    /// <param name="byteString">Array where to search.</param>
    /// <param name="byteToFind">Array to search.</param>
    /// <returns>True if ends with</returns>
    public static bool EndsWith(this ByteString byteString, ByteString byteToFind)
    {
        return StdLib.MemorySearch(byteString, byteToFind) + byteToFind.Length == byteString.Length;
    }

    /// <summary>
    /// Checks if the <see cref="ByteString"/> contains the given <see cref="ByteString"/>.
    /// </summary>
    /// <param name="byteString"><see cref="ByteString"/> to search.</param>
    /// <param name="byteToFind"><see cref="ByteString"/> to be searched.</param>
    /// <returns></returns>
    public static bool Contains(this ByteString byteString, ByteString byteToFind)
    {
        return StdLib.MemorySearch(byteString, byteToFind) != -1;
    }
}
