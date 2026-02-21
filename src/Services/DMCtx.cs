/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using SwiftlyS2.Shared.SchemaDefinitions;

namespace Deathmatch;

public static class DMCtx
{
    private static int _modeStartedAt = 0;
    private static LinkedListNode<Mode>? _currentMode;
    private static readonly Dictionary<ushort, Gun> _currentGuns = [];
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
            botLoadout: new([new("deagle", 1)], [new("mp9", 0.3f), new("mac10", 0.1f)])
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
        if (_currentMode == null)
            return "00:00";
        var tick = Swiftly.Core.Engine.GlobalVars.TickCount;
        return TimeHelper.FormatMmSs(
            Math.Max(_currentMode.Value.Duration - ((tick - _modeStartedAt) / 64), 0)
        );
    }

    public static Mode? GetCurrentMode()
    {
        return _currentMode?.Value;
    }

    public static Mode? GetNextMode()
    {
        return _currentMode?.Next?.Value ?? Modes.First?.Value;
    }

    public static void Think()
    {
        var tick = Swiftly.Core.Engine.GlobalVars.TickCount;
        if (_currentMode != null && ((tick - _modeStartedAt) / 64) <= _currentMode.Value.Duration)
            return;
        _currentMode = _currentMode?.Next ?? Modes.First;
        _modeStartedAt = tick;
        if (_currentMode == null)
            return;
        RebuildCurrentGuns(_currentMode.Value);
        var (secondary, primary) = ComputeDefaultGuns(_currentMode.Value);
        if (secondary != null)
            DefaultGuns = (secondary, primary);
        ResetAllPlayers();
        Swiftly.Core.ConVar.Find<int>("mp_free_armor")?.Value = _currentMode.Value.Helmet ? 2 : 1;
    }

    private static void RebuildCurrentGuns(Mode mode)
    {
        _currentGuns.Clear();
        foreach (var gun in Guns)
            if (mode.Guns.Contains(gun.DesignerName.Replace("weapon_", "")))
                _currentGuns.Add(gun.ItemDef, gun);
    }

    public static bool AllowsGun(Gun gun)
    {
        return _currentGuns.ContainsKey(gun.ItemDef);
    }

    public static IEnumerable<Gun> GetCurrentGuns()
    {
        return _currentGuns.Values;
    }

    private static (Gun? Secondary, Gun? Primary) ComputeDefaultGuns(Mode mode)
    {
        Gun? secondary = null;
        Gun? primary = null;
        foreach (
            var gun in mode.Guns.Select(name =>
                Guns.FirstOrDefault(g => g.DesignerName == $"weapon_{name}")
            )
        )
        {
            if (gun == null)
                continue;
            if (gun.IsSecondary)
                secondary ??= gun;
            else
                primary ??= gun;
            if (secondary != null && primary != null)
                break;
        }
        return (secondary, primary);
    }

    private static void ResetAllPlayers()
    {
        var hasHelmet = GetCurrentMode()?.Helmet == true;
        foreach (
            var player in Swiftly.Core.PlayerManager.GetAllValidPlayers().Where(p => p.IsAlive)
        )
        {
            var pawn = (player.IsFakeClient ? player.Pawn : player.PlayerPawn)?.As<CCSPlayerPawn>();
            player.SetHealth(100);
            pawn?.WeaponServices?.RemoveWeaponBySlot(gear_slot_t.GEAR_SLOT_PISTOL);
            pawn?.WeaponServices?.RemoveWeaponBySlot(gear_slot_t.GEAR_SLOT_RIFLE);
            player.GiveLoadout();
            pawn?.ItemServices?.HasHelmet = hasHelmet;
            pawn?.ItemServices?.HasHelmetUpdated();
            player.SetArmor(100);
        }
    }
}
