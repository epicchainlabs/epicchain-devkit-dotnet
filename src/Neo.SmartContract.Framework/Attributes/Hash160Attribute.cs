// Copyright (C) 2015-2024 The Neo Project.
//
// The Chain.SmartContract.Framework is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;

namespace Chain.SmartContract.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class Hash160Attribute(string value) : InitialValueAttribute(value, ContractParameterType.Hash160)
    {
    }
}
