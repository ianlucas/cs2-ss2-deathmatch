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

    public static readonly LinkedList<Mode> Modes = new([
        new(
            name: "Pistols",
            weaponNames:
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
            weaponNames:
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
            weaponNames:
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
        ConVars.FreeArmor.Value = _currentMode.Value.Helmet ? 2 : 1;
        ResetAllPlayers();
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
