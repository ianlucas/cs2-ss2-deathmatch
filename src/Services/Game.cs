/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using SwiftlyS2.Shared.SchemaDefinitions;

namespace Deathmatch;

public static class Game
{
    public static int ModeStartedAt { get; set; } = 0;
    public static LinkedListNode<Mode>? CurrentMode { get; set; }
    public static Dictionary<ushort, Gun> CurrentGuns { get; set; } = [];
    public static (Gun Secondary, Gun? Primary)? DefaultGuns { get; set; }
    public static readonly int SpawnDelay = 32;
    public static readonly List<Gun> Guns =
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

    public static readonly LinkedList<Mode> Modes = new([
        new(
            name: "Pistols",
            guns:
            [
                "usp_silencer",
                "deagle",
                "elite",
                "fiveseven",
                "glock",
                "tec9",
                "hkp2000",
                "p250",
                "cz75a",
                "revolver",
            ],
            helmet: false,
            duration: 100,
            botLoadout: new([new("usp_silencer", 0.8f), new("glock", 0.1f), new("deagle", 0.1f)])
        ),
        new(
            name: "Mid-Tier",
            guns:
            [
                "deagle",
                "mp9",
                "usp_silencer",
                "elite",
                "fiveseven",
                "glock",
                "tec9",
                "hkp2000",
                "p250",
                "cz75a",
                "revolver",
                "mac10",
                "mp5sd",
                "mp7",
                "ump45",
            ],
            duration: 100,
            botLoadout: new([new("deagle", 1)], [new("mp9", 0.1f)])
        ),
        new(
            name: "Rifles",
            guns:
            [
                "deagle",
                "ak47",
                "usp_silencer",
                "elite",
                "fiveseven",
                "glock",
                "tec9",
                "hkp2000",
                "p250",
                "cz75a",
                "revolver",
                "m4a1",
                "m4a1_silencer",
                "aug",
                "sg556",
                "famas",
                "galilar",
                "awp",
                "ssg08",
            ],
            duration: 100,
            botLoadout: new(
                [new("deagle", 1)],
                [new("ak47", 0.7f), new("m4a1", 0.1f), new("m4a1_silencer", 0.1f), new("awp", 0.1f)]
            )
        ),
    ]);

    public static string GetChatPrefix(bool stripColors = false)
    {
        return stripColors
            ? ConVars.ChatPrefix.Value.StripColors()
            : ConVars.ChatPrefix.Value.ApplyColors();
    }

    public static string GetRemainingTime()
    {
        if (CurrentMode == null)
            return "00:00";
        var tick = Swiftly.Core.Engine.GlobalVars.TickCount;
        return TimeHelper.FormatMmSs(
            Math.Max(CurrentMode.Value.Duration - ((tick - ModeStartedAt) / 64), 0)
        );
    }

    public static void TryStartNextMode()
    {
        var tick = Swiftly.Core.Engine.GlobalVars.TickCount;
        if (CurrentMode == null || ((tick - ModeStartedAt) / 64) > CurrentMode.Value.Duration)
        {
            CurrentMode = CurrentMode?.Next ?? Modes.First;
            ModeStartedAt = tick;
            if (CurrentMode == null)
                return;
            CurrentGuns.Clear();
            foreach (var gun in Guns)
                if (CurrentMode.Value.Guns.Contains(gun.DesignerName.Replace("weapon_", "")))
                    CurrentGuns.Add(gun.ItemDef, gun);
            Gun? secondary = null;
            Gun? primary = null;
            foreach (
                var gun in CurrentMode.Value.Guns.Select(designerName =>
                    Guns.FirstOrDefault(g => g.DesignerName == $"weapon_{designerName}")
                )
            )
            {
                if (gun == null)
                    continue;
                if (gun.Type == "Pistol")
                    secondary ??= gun;
                else
                    primary ??= gun;
                if (secondary != null && primary != null)
                    break;
            }
            if (secondary != null)
                DefaultGuns = (secondary, primary);
            foreach (
                var player in Swiftly.Core.PlayerManager.GetAllValidPlayers().Where(p => p.IsAlive)
            )
            {
                player.SetHealth(100);
                player.PlayerPawn?.WeaponServices?.RemoveWeaponBySlot(gear_slot_t.GEAR_SLOT_PISTOL);
                player.PlayerPawn?.WeaponServices?.RemoveWeaponBySlot(gear_slot_t.GEAR_SLOT_RIFLE);
                player.GiveLoadout();
            }
        }
    }
}
