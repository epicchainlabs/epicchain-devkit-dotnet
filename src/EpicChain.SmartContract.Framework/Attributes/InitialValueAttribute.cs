// Copyright (C) 2021-2024 EpicChain Labs.
//
// The EpicChain.SmartContract.Framework is open-source software made available under the MIT License.
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
