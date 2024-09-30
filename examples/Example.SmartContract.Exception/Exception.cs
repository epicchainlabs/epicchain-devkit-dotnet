// Copyright (C) 2021-2024 EpicChain Labs.
//
// Exception.cs file is a crucial component of the EpicChain project and is freely distributed as open-source software.
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
using System.ComponentModel;

namespace Exception
{
    [DisplayName("SampleException")]
    [ContractAuthor("core-dev", "devs@epic-chain.org")]
    [ContractDescription("A sample contract to demonstrate how to handle exception")]
    [ContractVersion("0.0.1")]
    [ContractSourceCode("https://github.com/epicchainlabs/epicchain-devkit-dotnet/tree/master/examples/")]
    [ContractPermission(Permission.Any, Method.Any)]
    public class SampleException : SmartContract
    {
        [ByteArray("0a0b0c0d0E0F")]
        private static readonly ByteString invalidECpoint = default;

        [ByteArray("024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9")]
        private static readonly ByteString byteString2Ecpoint = default;

        [Hash160("NXV7ZhHiyM1aHXwpVsRZC6BwNFP2jghXAq")]
        private static readonly ByteString validUInt160 = default;

        [ByteArray("edcf8679104ec2911a4fe29ad7db232a493e5b990fb1da7af0c7b989948c8925")]
        private static readonly byte[] validUInt256 = default;

        public static object try01()
        {
            int v = 0;
            try
            {
                v = 2;
            }
            catch
            {
                v = 3;
            }
            finally
            {
                v++;
            }
            return v;
        }

        public static object try02()
        {
            int v = 0;
            try
            {
                v = 2;
                throw new System.Exception();
            }
            catch
            {
                v = 3;
            }
            finally
            {
                v++;
            }
            return v;
        }

        public static object try03()
        {
            int v = 0;
            try
            {
                v = 2;
                Throwcall();
            }
            catch
            {
                v = 3;
            }
            finally
            {
                v++;
            }
            return v;
        }

        public static object tryNest()
        {
            int v = 0;
            try
            {
                try
                {
                    v = 2;
                    Throwcall();
                }
                catch
                {
                    v = 3;
                    Throwcall();
                }
                finally
                {
                    Throwcall();
                    v++;
                }
            }
            catch
            {
                v++;
            }
            return v;
        }

        public static object tryFinally()
        {
            int v = 0;
            try
            {
                v = 2;
            }
            finally
            {
                v++;
            }
            return v;
        }

        public static object tryFinallyAndRethrow()
        {
            int v = 0;
            try
            {
                v = 2;
                Throwcall();
            }
            finally
            {
                v++;
            }
            return v;
        }

        public static object tryCatch()
        {
            int v = 0;
            try
            {
                v = 2;
                Throwcall();
            }
            catch
            {
                v++;
            }
            return v;
        }

        public static object tryWithTwoFinally()
        {
            int v = 0;
            try
            {
                try
                {
                    v++;
                }
                catch
                {
                    v += 2;
                }
                finally
                {
                    v += 3;
                }
            }
            catch
            {
                v += 4;
            }
            finally
            {
                v += 5;
            }
            return v;
        }

        public static object tryecpointCast()
        {
            int v = 0;
            try
            {
                v = 2;
                ECPoint pubkey = (ECPoint)invalidECpoint;
            }
            catch
            {
                v = 3;
            }
            finally
            {
                v++;
            }
            return v;
        }

        public static object tryvalidByteString2Ecpoint()
        {
            int v = 0;
            try
            {
                v = 2;
                ECPoint pubkey = (ECPoint)byteString2Ecpoint;
            }
            catch
            {
                v = 3;
            }
            finally
            {
                v++;
            }
            return v;
        }

        public static object tryinvalidByteArray2UInt160()
        {
            int v = 0;
            try
            {
                v = 2;
                UInt160 data = (UInt160)invalidECpoint;
            }
            catch
            {
                v = 3;
            }
            finally
            {
                v++;
            }
            return v;
        }

        public static object tryvalidByteArray2UInt160()
        {
            int v = 0;
            try
            {
                v = 2;
                UInt160 data = (UInt160)validUInt160;
            }
            catch
            {
                v = 3;
            }
            finally
            {
                v++;
            }
            return v;
        }

        public static object tryinvalidByteArray2UInt256()
        {
            int v = 0;
            try
            {
                v = 2;
                UInt256 data = (UInt256)invalidECpoint;
            }
            catch
            {
                v = 3;
            }
            finally
            {
                v++;
            }
            return v;
        }

        public static object tryvalidByteArray2UInt256()
        {
            int v = 0;
            try
            {
                v = 2;
                UInt256 data = (UInt256)validUInt256;
            }
            catch
            {
                v = 3;
            }
            finally
            {
                v++;
            }
            return v;
        }

        public static (int, object) tryNULL2Ecpoint_1()
        {
            int v = 0;
            ECPoint data = (ECPoint)(new byte[33]);
            try
            {
                v = 2;
                data = (ECPoint)null;
            }
            catch
            {
                v = 3;
            }
            finally
            {
                v++;
                if (data is null)
                {
                    v++;
                }
            }
            return (v, data);
        }

        public static (int, object) tryNULL2Uint160_1()
        {
            int v = 0;
            UInt160 data = (UInt160)(new byte[20]);
            try
            {
                v = 2;
                data = (UInt160)null;
            }
            catch
            {
                v = 3;
            }
            finally
            {
                v++;
                if (data is null)
                {
                    v++;
                }
            }
            return (v, data);
        }

        public static (int, object) tryNULL2Uint256_1()
        {
            int v = 0;
            UInt256 data = (UInt256)(new byte[32]);
            try
            {
                v = 2;
                data = (UInt256)null;
            }
            catch
            {
                v = 3;
            }
            finally
            {
                v++;
                if (data is null)
                {
                    v++;
                }
            }
            return (v, data);
        }

        public static (int, object) tryNULL2Bytestring_1()
        {
            int v = 0;
            ByteString data = "123";
            try
            {
                v = 2;
                data = (ByteString)null;
            }
            catch
            {
                v = 3;
            }
            finally
            {
                v++;
                if (data is null)
                {
                    v++;
                }
            }
            return (v, data);
        }

        private static object Throwcall()
        {
            throw new System.Exception();
        }

        public static object TryUncatchableException()
        {
            int v = 0;
            try
            {
                v = 2;
                throw new UncatchableException();
            }
            catch
            {
                v = 3;
            }
            finally
            {
                v++;
            }
            return v;
        }

    }
}
