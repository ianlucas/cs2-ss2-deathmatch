/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace Deathmatch;

public static class Guns
{
    public static readonly IReadOnlyList<Gun> All =
    [
        // Pistols
        new("weapon_deagle", "Pistol", ["deagle", "deag", "desert_eagle"], 1),
        new("weapon_elite", "Pistol", ["elite", "elites", "dualies", "dual_berettas"], 2),
        new("weapon_fiveseven", "Pistol", ["fiveseven", "five_seven"], 3),
        new("weapon_glock", "Pistol", ["glock", "glock18"], 4),
        new("weapon_tec9", "Pistol", ["tec", "tec9"], 30),
        new("weapon_hkp2000", "Pistol", ["p2000", "hkp2000"], 32),
        new("weapon_p250", "Pistol", ["p250"], 36),
        new("weapon_usp_silencer", "Pistol", ["usp", "usps", "usp_silencer"], 61),
        new("weapon_cz75a", "Pistol", ["cz", "cz75", "cz75a"], 63),
        new("weapon_revolver", "Pistol", ["revolver", "r8"], 64),
        // Mid-Tier (SMGs)
        new("weapon_mac10", "MidTier", ["mac", "mac10"], 17),
        new("weapon_mp5sd", "MidTier", ["mp5", "mp5sd"], 23),
        new("weapon_mp7", "MidTier", ["mp7"], 33),
        new("weapon_mp9", "MidTier", ["mp9"], 34),
        new("weapon_ump45", "MidTier", ["ump", "ump45"], 24),
        new("weapon_p90", "MidTier", ["p90"], 19),
        new("weapon_bizon", "MidTier", ["bizon", "ppbizon"], 26),
        // Shotguns
        new("weapon_mag7", "Shotgun", ["mag7", "mag"], 27),
        new("weapon_nova", "Shotgun", ["nova"], 35),
        new("weapon_sawedoff", "Shotgun", ["sawedoff", "sawed_off"], 29),
        new("weapon_xm1014", "Shotgun", ["xm", "xm1014"], 25),
        // LMGs
        new("weapon_m249", "LMG", ["m249"], 14),
        new("weapon_negev", "LMG", ["negev"], 28),
        // Rifles
        new("weapon_ak47", "Rifle", ["ak", "ak47"], 7),
        new("weapon_m4a1", "Rifle", ["m4", "m4a4", "m4a1"], 16),
        new("weapon_m4a1_silencer", "Rifle", ["m4a1s", "m4a1_silencer", "m4a1_s"], 60),
        new("weapon_aug", "Rifle", ["aug"], 8),
        new("weapon_sg556", "Rifle", ["sg", "sg553", "sg556"], 39),
        new("weapon_famas", "Rifle", ["famas"], 10),
        new("weapon_galilar", "Rifle", ["galil", "galilar"], 13),
        new("weapon_awp", "Rifle", ["awp"], 9),
        new("weapon_ssg08", "Rifle", ["scout", "ssg", "ssg08"], 40),
        new("weapon_scar20", "Rifle", ["scar", "scar20"], 38),
        new("weapon_g3sg1", "Rifle", ["g3", "g3sg1"], 11),
    ];

    private static readonly IReadOnlyDictionary<ushort, Gun> _byItemDef = All.ToDictionary(g =>
        g.ItemDef
    );

    private static readonly IReadOnlyDictionary<string, Gun> _byDesignerName = All.ToDictionary(g =>
        g.DesignerName
    );

    public static Gun? GetByItemDef(ushort itemDef) => _byItemDef.GetValueOrDefault(itemDef);

    public static string NormalizeName(string name) =>
        name.StartsWith("weapon_") ? name : $"weapon_{name}";

    public static Gun? Find(string name) => _byDesignerName.GetValueOrDefault(NormalizeName(name));
}
