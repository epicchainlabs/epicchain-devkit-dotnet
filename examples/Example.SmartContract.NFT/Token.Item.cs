// Copyright (C) 2021-2024 EpicChain Lab's
//
// Token.Item.cs file is a crucial component of the EpicChain project and is freely distributed as open-source software.
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

namespace NFT
{
    public partial class Loot
    {
        private static readonly string[] _weapons = {
            "Warhammer",
            "Quarterstaff",
            "Maul",
            "Mace",
            "Club",
            "Katana",
            "Falchion",
            "Scimitar",
            "Long Sword",
            "Short Sword",
            "Ghost Wand",
            "Grave Wand",
            "Bone Wand",
            "Wand",
            "Grimoire",
            "Chronicle",
            "Tome",
            "Book"
        };

        private static readonly string[] _chestArmor = {
            "Divine Robe",
            "Silk Robe",
            "Linen Robe",
            "Robe",
            "Shirt",
            "Demon Husk",
            "Dragonskin Armor",
            "Studded Leather Armor",
            "Hard Leather Armor",
            "Leather Armor",
            "Holy Chestplate",
            "Ornate Chestplate",
            "Plate Mail",
            "Chain Mail",
            "Ring Mail"
        };

        private static readonly string[] _headArmor = {
            "Ancient Helm",
            "Ornate Helm",
            "Great Helm",
            "Full Helm",
            "Helm",
            "Demon Crown",
            "Dragon's Crown",
            "War Cap",
            "Leather Cap",
            "Cap",
            "Crown",
            "Divine Hood",
            "Silk Hood",
            "Linen Hood",
            "Hood"
        };

        private static readonly string[] _waistArmor = {
            "Ornate Belt",
            "War Belt",
            "Plated Belt",
            "Mesh Belt",
            "Heavy Belt",
            "Demonhide Belt",
            "Dragonskin Belt",
            "Studded Leather Belt",
            "Hard Leather Belt",
            "Leather Belt",
            "Brightsilk Sash",
            "Silk Sash",
            "Wool Sash",
            "Linen Sash",
            "Sash"
        };

        private static readonly string[] _footArmor = {
            "Holy Greaves",
            "Ornate Greaves",
            "Greaves",
            "Chain Boots",
            "Heavy Boots",
            "Demonhide Boots",
            "Dragonskin Boots",
            "Studded Leather Boots",
            "Hard Leather Boots",
            "Leather Boots",
            "Divine Slippers",
            "Silk Slippers",
            "Wool Shoes",
            "Linen Shoes",
            "Shoes"
        };

        private static readonly string[] _handArmor = {
            "Holy Gauntlets",
            "Ornate Gauntlets",
            "Gauntlets",
            "Chain Gloves",
            "Heavy Gloves",
            "Demon's Hands",
            "Dragonskin Gloves",
            "Studded Leather Gloves",
            "Hard Leather Gloves",
            "Leather Gloves",
            "Divine Gloves",
            "Silk Gloves",
            "Wool Gloves",
            "Linen Gloves",
            "Gloves"
        };

        private static readonly string[] _necklaces = {
            "Necklace",
            "Amulet",
            "Pendant"
        };

        private static readonly string[] _rings = {
            "Gold Ring",
            "Silver Ring",
            "Bronze Ring",
            "Platinum Ring",
            "Titanium Ring"
        };

        private static readonly string[] _suffixes = {
            "of Power",
            "of Giants",
            "of Titans",
            "of Skill",
            "of Perfection",
            "of Brilliance",
            "of Enlightenment",
            "of Protection",
            "of Anger",
            "of Rage",
            "of Fury",
            "of Vitriol",
            "of the Fox",
            "of Detection",
            "of Reflection",
            "of the Twins"
        };

        private static readonly string[] _namePrefixes = {
            "Agony", "Apocalypse", "Armageddon", "Beast", "Behemoth", "Blight", "Blood", "Bramble",
            "Brimstone", "Brood", "Carrion", "Cataclysm", "Chimeric", "Corpse", "Corruption", "Damnation",
            "Death", "Demon", "Dire", "Dragon", "Dread", "Doom", "Dusk", "Eagle", "Empyrean", "Fate", "Foe",
            "Gale", "Ghoul", "Gloom", "Glyph", "Golem", "Grim", "Hate", "Havoc", "Honour", "Horror", "Hypnotic",
            "Kraken", "Loath", "Maelstrom", "Mind", "Miracle", "Morbid", "Oblivion", "Onslaught", "Pain",
            "Pandemonium", "Phoenix", "Plague", "Rage", "Rapture", "Rune", "Skull", "Sol", "Soul", "Sorrow",
            "Spirit", "Storm", "Tempest", "Torment", "Vengeance", "Victory", "Viper", "Vortex", "Woe", "Wrath",
            "Light's", "Shimmering"
        };

        private static readonly string[] _nameSuffixes = {
            "Bane",
            "Root",
            "Bite",
            "Song",
            "Roar",
            "Grasp",
            "Instrument",
            "Glow",
            "Bender",
            "Shadow",
            "Whisper",
            "Shout",
            "Growl",
            "Tear",
            "Peak",
            "Form",
            "Sun",
            "Moon"
        };
    }
}
