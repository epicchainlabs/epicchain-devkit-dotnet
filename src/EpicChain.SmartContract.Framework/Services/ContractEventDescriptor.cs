// Copyright (C) 2021-2024 EpicChain Lab's
//
// ContractEventDescriptor.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace EpicChain.SmartContract.Framework.Services
{
    public struct ContractEventDescriptor
    {
        public string Name;
        public ContractParameterDefinition[] Parameters;
    }
}
