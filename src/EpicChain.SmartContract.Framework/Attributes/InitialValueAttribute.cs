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


using System;

namespace EpicChain.SmartContract.Framework.Attributes
{
    /// <summary>
    /// Specifies an initial value for a static field within a smart contract,
    /// enabling the field to be initialized at compile time.
    /// </summary>
    ///
    /// <remarks>
    /// In smart contracts, it's necessary to initialize static variables.
    /// However, initializing some contract-specific types directly may introduce
    /// complex type conversions, leading to additional overhead during contract
    /// execution as the conversion operation is called each time.
    /// By using the <see cref="InitialValueAttribute"/>, variables can be assigned
    /// an initial value, allowing for compile-time initialization and avoiding runtime
    /// conversion overhead.
    ///
    /// <para>Examples:</para>
    /// <code>
    /// // Example of initializing a UInt160 field with a Hash160 address
    /// [InitialValue("NXV7ZhHiyM1aHXwpVsRZC6BwNFP2jghXAq")]
    /// private static readonly UInt160 validUInt160 = default;
    ///
    /// // Example of initializing a byte array field with a hex string representing a UInt256 value
    /// [InitialValue("edcf8679104ec2911a4fe29ad7db232a493e5b990fb1da7af0c7b989948c8925")]
    /// private static readonly byte[] validUInt256 = default;
    /// </code>
    ///
    /// Currently supported types are:
    ///     <see cref="ContractParameterType.String"/>
    ///     <see cref="ContractParameterType.ByteArray"/>
    ///     <see cref="ContractParameterType.Hash160"/>
    ///     <see cref="ContractParameterType.PublicKey"/>
    ///     <see cref="ContractParameterType.Integer"/>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field)]
    public class InitialValueAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InitialValueAttribute"/> class
        /// with the specified initial value and contract parameter type.
        /// </summary>
        /// <param name="value">The initial value to assign to the field, represented
        /// as a string.
        /// </param>
        /// <param name="type">The <see cref="ContractParameterType"/> indicating the
        /// type of the field being initialized.
        /// </param>
        public InitialValueAttribute(string value, ContractParameterType type)
        {
        }

        public InitialValueAttribute(string value)
        {
        }
    }
}
