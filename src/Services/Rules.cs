/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using SwiftlyS2.Shared.SchemaDefinitions;

namespace Deathmatch;

public static class Rules
{
    public static int ModeStartedAt { get; set; } = 0;
    public static LinkedListNode<Mode>? CurrentMode { get; set; }
    public static LinkedList<Mode> Modes { get; set; } = new([]);

    public static void SetModes(IEnumerable<Mode> modes)
    {
        Modes = new(modes);
        ModeStartedAt = 0;
    }

    public static string GetChatPrefix(bool stripColors = false)
    {
        return stripColors
            ? ConVars.ChatPrefix.Value.StripColors()
            : ConVars.ChatPrefix.Value.ApplyColors();
    }

    public static int GetRemainingTime()
    {
        if (CurrentMode == null)
            return 0;
        var tick = Runtime.Core.Engine.GlobalVars.TickCount;
        return Math.Max(CurrentMode.Value.Duration - ((tick - ModeStartedAt) / 64), 0);
    }

    public static Mode? GetCurrentMode()
    {
        return CurrentMode?.Value;
    }

    public static Mode? GetNextMode()
    {
        return CurrentMode?.Next?.Value ?? Modes.First?.Value;
    }

    public static bool HasMultipleModes()
    {
        return Modes.Count > 1;
    }

    public static void Think()
    {
        var tick = Runtime.Core.Engine.GlobalVars.TickCount;
        if (CurrentMode != null && ((tick - ModeStartedAt) / 64) <= CurrentMode.Value.Duration)
            return;
        CurrentMode = CurrentMode?.Next ?? Modes.First;
        ModeStartedAt = tick;
        if (CurrentMode == null)
            return;
        ConVars.FreeArmor.Value = CurrentMode.Value.Helmet ? 2 : 1;
        ResetPlayers();
    }

    private static void ResetPlayers()
    {
        var hasHelmet = GetCurrentMode()?.Helmet == true;
        foreach (
            var player in Runtime.Core.PlayerManager.GetAllValidPlayers().Where(p => p.IsAlive)
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
