// Copyright (C) 2021-2024 EpicChain Lab's
//
// ZKP.cs file is a crucial component of the EpicChain project and is freely distributed as open-source software.
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

using EpicChain.SmartContract.Framework;
using EpicChain.SmartContract.Framework.Attributes;
using EpicChain.SmartContract.Framework.Native;
using System;
using System.ComponentModel;

namespace ZKP
{
    [DisplayName("SampleZKP")]
    [ContractAuthor("code-dev")]
    [ContractVersion("0.0.1")]
    [ContractDescription("A sample contract to demonstrate how to use Example.SmartContract.ZKPil")]
    [ContractSourceCode("https://github.com/epicchainlabs/epicchain-devkit-dotnet/tree/master/examples/")]
    [ContractPermission(Permission.Any, Method.Any)]
    public class ExampleZKP : SmartContract
    {
        public static readonly byte[] alphaPoint =
        {
        0, 106, 167, 155, 164, 170, 67, 158, 237, 78, 91, 7, 243, 191, 186, 221, 27, 97, 6, 190, 193, 204, 85, 206, 83, 56, 3, 209, 132, 249, 221, 94, 124, 20, 245, 113, 143, 70, 245, 159, 104, 213, 37, 151, 209, 125, 160, 143, 22, 78, 252, 97, 225, 215, 133, 240, 84, 149, 231, 142, 83,
        116, 156, 136, 120, 45, 242, 155, 199, 169, 244, 228, 221, 245, 160, 28, 247, 39, 83, 6, 172, 25, 126, 223, 231, 40, 94, 154, 69, 103, 123, 105, 242, 163, 156, 220
    };

        public static readonly byte[] betaPoint =
        {
        3, 91, 30, 20, 61, 202, 142, 33, 164, 33, 215, 106, 219, 39, 136, 96, 112, 254, 117, 55, 156, 44, 55, 125, 240, 63, 166, 206, 157, 17, 201, 11, 33, 172, 226, 58, 254, 202, 46, 128, 2, 179, 227, 37, 230, 127, 121, 118, 6, 59, 84, 145, 104, 196, 68, 37, 209, 54, 86, 148, 155, 251,
        36, 110, 127, 190, 205, 52, 100, 136, 226, 196, 249, 172, 122, 215, 230, 42, 92, 175, 190, 120, 19, 80, 56, 148, 236, 157, 108, 74, 45, 29, 157, 243, 96, 94, 17, 16, 242, 165, 56, 132, 223, 128, 0, 35, 238, 8, 138, 176, 102, 236, 242, 177, 252, 151, 152, 94, 230, 130, 111, 185,
        250, 195, 15, 125, 31, 128, 3, 102, 181, 56, 19, 195, 121, 224, 82, 228, 3, 41, 31, 122, 220, 133, 18, 212, 95, 194, 201, 185, 75, 111, 233, 98, 80, 47, 9, 191, 178, 119, 49, 238, 30, 235, 217, 40, 8, 199, 253, 123, 8, 85, 78, 100, 32, 111, 185, 57, 197, 240, 76, 6, 252, 16, 114,
        82, 90, 163, 240, 146, 4, 2
    };

        public static readonly byte[] gamma_inversePoint =
        {
        23, 69, 47, 108, 115, 173, 254, 203, 89, 67, 183, 224, 176, 26, 127, 132, 89, 162, 99, 241, 66, 228, 177, 17, 57, 85, 3, 13, 148, 88, 162, 54, 220, 189, 33, 172, 38, 192, 116, 236, 13, 115, 219, 201, 51, 166, 253, 240, 12, 32, 77, 82, 161, 189, 240, 198, 148, 184, 17, 92, 162,
        145, 166, 55, 252, 245, 194, 95, 71, 208, 215, 23, 19, 95, 138, 147, 149, 26, 35, 108, 141, 25, 139, 103, 59, 48, 189, 88, 204, 100, 255, 116, 194, 229, 157, 5, 19, 16, 31, 158, 222, 45, 151, 86, 218, 157, 17, 252, 29, 131, 121, 107, 168, 46, 145, 176, 122, 146, 93, 180, 181, 98,
        229, 4, 29, 34, 137, 222, 93, 124, 90, 211, 99, 51, 90, 96, 191, 203, 14, 34, 14, 5, 77, 209, 0, 62, 138, 150, 103, 94, 252, 200, 186, 64, 197, 27, 173, 229, 189, 193, 196, 75, 40, 95, 107, 36, 50, 90, 146, 59, 215, 202, 184, 77, 87, 20, 53, 241, 208, 72, 158, 45, 22, 81, 53,
        220, 40, 222, 26, 69, 230, 253
    };

        public static readonly byte[] deltaPoint =
        {
        1, 78, 83, 175, 159, 103, 127, 217, 80, 213, 0, 194, 108, 30, 210, 241, 138, 209, 0, 164, 117, 32, 68, 102, 121, 36, 40, 65, 89, 205, 198, 1, 14, 144, 196, 236, 176, 214, 119, 139, 225, 118, 215, 185, 36, 216, 183, 27, 22, 126, 193, 21, 173, 212, 250, 104, 25, 69, 107, 40, 199,
        160, 228, 239, 112, 102, 144, 85, 58, 109, 122, 73, 221, 170, 145, 188, 60, 9, 228, 178, 36, 227, 175, 140, 40, 181, 158, 175, 91, 189, 92, 169, 90, 90, 30, 153, 4, 225, 187, 53, 206, 114, 60, 109, 51, 184, 2, 100, 39, 95, 43, 33, 82, 141, 161, 200, 136, 146, 33, 18, 202, 141,
        43, 222, 64, 81, 151, 58, 141, 146, 8, 214, 159, 110, 167, 173, 253, 57, 190, 62, 94, 88, 245, 59, 20, 121, 233, 209, 122, 42, 13, 184, 114, 0, 19, 32, 120, 143, 108, 118, 107, 241, 218, 182, 69, 135, 117, 42, 231, 191, 199, 88, 88, 145, 134, 24, 133, 211, 53, 72, 23, 214, 105,
        97, 134, 254, 116, 89, 166, 119, 221, 223
    };

        public static readonly byte[][] ic = {
        new byte[]
        {
            14, 152, 253, 159, 101, 142, 227, 5, 166, 71, 152, 207, 32, 152, 56, 172, 191, 43, 184, 28, 148, 40, 224, 42, 135, 137, 181, 215, 96, 34, 200, 127, 77, 151, 165, 11, 130, 57, 91, 83, 71, 38, 253, 159, 103, 191, 139, 120, 20, 9, 91, 120, 106, 16, 209, 88, 87, 206, 209, 233,
            129, 87, 15, 139, 92, 164, 84, 150, 51, 92, 220, 188, 115, 217, 131, 193, 213, 23, 225, 128, 244, 135, 95, 128, 181, 127, 159, 195, 219, 176, 152, 16, 186, 80, 5, 143
        },
        new byte[]
        {
            17, 158, 199, 19, 137, 211, 161, 248, 118, 149, 250, 145, 46, 221, 160, 86, 40, 165, 110, 198, 160, 203, 188, 84, 210, 83, 159, 176, 113, 111, 10, 235, 192, 243, 242, 110, 188, 210, 98, 199, 74, 66, 118, 251, 3, 188, 58, 84, 23, 55, 88, 168, 37, 240, 121, 248, 22, 139, 165,
            151, 47, 163, 72, 114, 80, 152, 28, 160, 76, 58, 252, 162, 110, 107, 202, 173, 57, 219, 79, 119, 196, 249, 84, 215, 233, 11, 59, 50, 204, 23, 149, 64, 88, 135, 163, 190
        }
        };

        /// <summary>
        /// Verify '&' circuit.To verify x1&x2 = 0 or 1
        /// AB=alpha*beta+sum(pub_input[i]*(beta*u_i(x)+alpha*v_i(x)+w_i(x))/gamma)*gamma+C*delta
        /// </summary>
        /// <param name="a">Point A</param>
        /// <param name="b">Point B</param>
        /// <param name="c">Point C</param>
        /// <param name="public_input">Public paramters</param>
        /// <returns>result</returns>
        public static bool Veify(byte[] a, byte[] b, byte[] c, byte[][] public_input)
        {
            //Equation left1: A*B
            byte[] lt = (byte[])CryptoLib.Bls12381Pairing(a, b);
            //Equation right1: alpha*beta
            byte[] rt1 = (byte[])CryptoLib.Bls12381Pairing(alphaPoint, betaPoint);
            //Equation right2: sum(pub_input[i]*(beta*u_i(x)+alpha*v_i(x)+w_i(x))/gamma)*gamma
            int inputlen = public_input.Length;
            int iclen = ic.Length;
            if (iclen != inputlen + 1) throw new Exception("error: inputlen or iclen");
            byte[] acc = ic[0];
            for (int i = 0; i < inputlen; i++)
            {
                byte[] temp = (byte[])CryptoLib.Bls12381Mul(ic[i + 1], public_input[i]/*32-bytes LE field element.*/, false);
                acc = (byte[])CryptoLib.Bls12381Add(acc, temp);
            }
            byte[] rt2 = (byte[])CryptoLib.Bls12381Pairing(acc, gamma_inversePoint);
            //Equation right3: C*delta
            byte[] rt3 = (byte[])CryptoLib.Bls12381Pairing(c, deltaPoint);
            //Check equal
            byte[] t1 = (byte[])CryptoLib.Bls12381Add(rt1, rt2);
            byte[] t2 = (byte[])CryptoLib.Bls12381Add(t1, rt3);
            return lt == t2;
        }
    }
}
