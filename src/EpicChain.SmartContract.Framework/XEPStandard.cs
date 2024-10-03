// Copyright (C) 2021-2024 EpicChain Lab's
//
// XepStandard.cs file is a crucial component of the EpicChain project and is freely distributed as open-source software.
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

namespace EpicChain.SmartContract.Framework
{
    public enum XepStandard
    {
        // The XEP-11 standard is used for non-fungible tokens (NFTs).
        // Defined at https://github.com/neo-project/proposals/blob/master/XEP-11.mediawiki
        XEP11,
        // The XEP-17 standard is used for fungible tokens.
        // Defined at https://github.com/neo-project/proposals/blob/master/XEP-17.mediawiki
        Xep17,
        // Smart contract transfer callback for non-fungible tokens (NFTs).
        // This is an extension standard of XEP-11.
        // Defined at https://github.com/neo-project/proposals/pull/169/files#diff-2b5f7c12a23f7dbe4cb46bbf4be6936882f8e0f0b3a4db9d8c58eb294b02e6ed
        Xep26,
        // This is the nick name of XEP-25.
        XEP11Payable,
        // Smart contract transfer callback for fungible tokens.
        // This is an extension standard of XEP-17.
        // Defined at https://github.com/neo-project/proposals/pull/169/files#diff-70768f307c9aa84f8c94e790495a76d47fffeca2331444592ebba6f13b1e6460
        Xep27,
        // This is the nick name of XEP-26.
        Xep17Payable,
        // This XEP defines a global standard to get royalty payment information for Non-Fungible Tokens (NFTs)
        // in order to enable support for royalty payments across all NFT marketplaces in the NEO Smart Economy.
        // This XEP requires XEP-11.
        // Defined at https://github.com/neo-project/proposals/blob/master/XEP-24.mediawiki
        Xep24
    }

    public static class XepStandardExtensions
    {
        public static string ToStandard(this XepStandard standard)
        {
            return standard switch
            {
                XepStandard.XEP11 => "XEP-11",
                XepStandard.Xep17 => "XEP-17",
                XepStandard.Xep24 => "XEP-24",
                XepStandard.XEP11Payable or XepStandard.Xep26 => "XEP-26",
                XepStandard.Xep17Payable or XepStandard.Xep27 => "XEP-27",
                _ => standard.ToString()
            };
        }
    }
}
