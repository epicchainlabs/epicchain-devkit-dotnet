// Copyright (C) 2021-2024 EpicChain Lab's
//
// The EpicChain.SmartContract.Framework is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;

namespace EpicChain.SmartContract.Framework.Attributes
{
    /// <summary>
    /// Specify the URL of the contract source code.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ContractSourceCodeAttribute : ManifestExtraAttribute
    {
        /// <summary>
        /// Specify the URL of the contract source code.
        /// </summary>
        /// <param name="url">The url of the contract source code</param>
        public ContractSourceCodeAttribute(string url) : base(AttributeType[nameof(ContractSourceCodeAttribute)], url)
        {
        }
    }
}
