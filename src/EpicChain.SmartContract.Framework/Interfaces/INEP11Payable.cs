// Copyright (C) 2021-2024 EpicChain Lab's
//
// ================================================================================================
//                                        INEP11Payable.cs
//                                   Part of the EpicChain Project
// ------------------------------------------------------------------------------------------------
// This file is a core component of the EpicChain project. It is distributed as free software under
// the MIT license, a permissive open-source license. For the full legal details on the usage,
// modification, and distribution of this software, please see the LICENSE file in the root folder
// of the project repository or visit the official MIT license page at:
// http://www.opensource.org/licenses/mit-license.php
// ------------------------------------------------------------------------------------------------
// MIT License Overview:
// - Redistribution and use in source and binary forms, with or without modification, are allowed.
// - The software can be freely modified, ensuring compliance with the terms and conditions.
// ------------------------------------------------------------------------------------------------
// A Message from xmoohad, Founder & CEO of EpicChain Labs:
// "EpicChain is driven by innovation and transparency. Every line of code, including
// INEP11Payable.cs, reflects our vision to provide robust and scalable blockchain solutions for
// the global community. We encourage developers to contribute and build on our work. Together,
// we are laying the foundations for the future of decentralized technology."
// ------------------------------------------------------------------------------------------------
//                                  Thank you for supporting EpicChain.
// ================================================================================================


#nullable enable

using System.Numerics;

namespace EpicChain.SmartContract.Framework.Interfaces;

/// <summary>
/// Interface of method that indicate a contract receives NEP-11 Payment
/// </summary>
public interface INep11Payable
{
    /// <summary>
    /// Contracts should implement the <see cref="OnNEP11Payment"/> method
    /// to receive NFT (NEP11) tokens.
    /// </summary>
    /// <param name="from">The address of the payer</param>
    /// <param name="amount">The amount of token to be transferred</param>
    /// <param name="tokenId">The token id to be transferred</param>
    /// <param name="data">Additional payment description data</param>
    /// <remarks>
    /// This interface method is defined as non-static,
    /// but if you need it to be static, you can directly
    /// remove the interface and define it as a static method.
    /// Both static and non-static methods of smart contract interface works,
    /// they differs on how you process static field.
    /// </remarks>
    public void OnNEP11Payment(UInt160 from, BigInteger amount, string tokenId, object? data = null);
}
