// Copyright (C) 2021-2024 EpicChain Labs.
//
// ContractEmailAttribute.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;

namespace EpicChain.SmartContract.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ContractEmailAttribute : ManifestExtraAttribute
    {
        public ContractEmailAttribute(string value) : base(AttributeType[nameof(ContractEmailAttribute)], value)
        {
        }
    }
}
