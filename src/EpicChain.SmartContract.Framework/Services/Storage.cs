// Copyright (C) 2021-2024 EpicChain Lab's
//
// The EpicChain.SmartContract.Framework is open-source software that is distributed under the widely recognized and permissive MIT License.
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


using EpicChain.SmartContract.Framework.Attributes;
using System.Numerics;

namespace EpicChain.SmartContract.Framework.Services
{
    public static class Storage
    {
        /// <summary>
        /// Returns current StorageContext
        /// </summary>
        public static extern StorageContext CurrentContext
        {
            [Syscall("System.Storage.GetContext")]
            get;
        }

        /// <summary>
        /// Returns current read only StorageContext
        /// </summary>
        public static extern StorageContext CurrentReadOnlyContext
        {
            [Syscall("System.Storage.GetReadOnlyContext")]
            get;
        }

        /// <summary>
        /// Returns the value corresponding to the given key for Storage context (faster: generates opcode directly)
        /// </summary>
        [Syscall("System.Storage.Get")]
        public static extern ByteString Get(StorageContext context, ByteString key);

        /// <summary>
        /// Returns the value corresponding to the given key for Storage context (faster: generates opcode directly)
        /// </summary>
        [Syscall("System.Storage.Get")]
        public static extern ByteString Get(StorageContext context, byte[] key);

        /// <summary>
        /// Writes the key/value pair for the given Storage context (faster: generates opcode directly)
        /// </summary>
        [Syscall("System.Storage.Put")]
        public static extern void Put(StorageContext context, ByteString key, ByteString value);

        /// <summary>
        /// Writes the key/value pair for the given Storage context (faster: generates opcode directly)
        /// </summary>
        [Syscall("System.Storage.Put")]
        public static extern void Put(StorageContext context, byte[] key, ByteString value);

        /// <summary>
        /// Writes the key/value pair for the given Storage context (faster: generates opcode directly)
        /// </summary>
        [Syscall("System.Storage.Put")]
        public static extern void Put(StorageContext context, byte[] key, byte[] value);

        /// <summary>
        /// Writes the key/value pair for the given Storage context (faster: generates opcode directly)
        /// </summary>
        [Syscall("System.Storage.Put")]
        public static extern void Put(StorageContext context, ByteString key, BigInteger value);

        /// <summary>
        /// Writes the key/value pair for the given Storage context (faster: generates opcode directly)
        /// </summary>
        [Syscall("System.Storage.Put")]
        public static extern void Put(StorageContext context, byte[] key, BigInteger value);

        /// <summary>
        /// Deletes the entry from the given Storage context (faster: generates opcode directly)
        /// </summary>
        [Syscall("System.Storage.Delete")]
        public static extern void Delete(StorageContext context, ByteString key);

        /// <summary>
        /// Deletes the entry from the given Storage context (faster: generates opcode directly)
        /// </summary>
        [Syscall("System.Storage.Delete")]
        public static extern void Delete(StorageContext context, byte[] key);

        /// <summary>
        /// Returns a byte[] to byte[] iterator for a byte[] prefix on a given Storage context (faster: generates opcode directly)
        /// </summary>
        [Syscall("System.Storage.Find")]
        public static extern Iterator Find(StorageContext context, ByteString prefix, FindOptions options = FindOptions.None);

        /// <summary>
        /// Returns a byte[] to byte[] iterator for a byte[] prefix on a given Storage context (faster: generates opcode directly)
        /// </summary>
        [Syscall("System.Storage.Find")]
        public static extern Iterator Find(StorageContext context, byte[] prefix, FindOptions options = FindOptions.None);

        #region Interface with default Context
        public static ByteString Get(ByteString key) => Get(CurrentReadOnlyContext, key);
        public static ByteString Get(byte[] key) => Get(CurrentReadOnlyContext, key);
        public static void Put(ByteString key, ByteString value) => Put(CurrentContext, key, value);
        public static void Put(byte[] key, ByteString value) => Put(CurrentContext, key, value);
        public static void Put(byte[] key, byte[] value) => Put(CurrentContext, key, value);
        public static void Put(ByteString key, BigInteger value) => Put(CurrentContext, key, value);
        public static void Put(byte[] key, BigInteger value) => Put(CurrentContext, key, value);
        public static void Delete(ByteString key) => Delete(CurrentContext, key);
        public static void Delete(byte[] key) => Delete(CurrentContext, key);
        public static Iterator Find(ByteString prefix, FindOptions options = FindOptions.None) => Find(CurrentReadOnlyContext, prefix, options);
        public static Iterator Find(byte[] prefix, FindOptions options = FindOptions.None) => Find(CurrentReadOnlyContext, prefix, options);
        #endregion
    }
}
