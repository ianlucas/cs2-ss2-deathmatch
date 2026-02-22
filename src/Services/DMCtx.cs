/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using Microsoft.Extensions.Logging;
using SwiftlyS2.Shared.SchemaDefinitions;

namespace Deathmatch;

public static class DMCtx
{
    private static int _modeStartedAt = 0;
    private static LinkedListNode<Mode>? _currentMode;

    private static LinkedList<Mode> _modes = new([]);

    public static void SetModes(IEnumerable<Mode> modes)
    {
        _modes = new(modes);
        _modeStartedAt = 0;
    }

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
        return _currentMode?.Next?.Value ?? _modes.First?.Value;
    }

    public static int GetModeCount()
    {
        return _modes.Count;
    }

    public static void Think()
    {
        var tick = Swiftly.Core.Engine.GlobalVars.TickCount;
        if (_currentMode != null && ((tick - _modeStartedAt) / 64) <= _currentMode.Value.Duration)
            return;
        _currentMode = _currentMode?.Next ?? _modes.First;
        _modeStartedAt = tick;
        if (_currentMode == null)
            return;
        ConVars.FreeArmor.Value = _currentMode.Value.Helmet ? 2 : 1;
        ResetPlayers();
    }

    private static void ResetPlayers()
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
