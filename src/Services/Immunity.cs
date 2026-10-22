/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using SwiftlyS2.Shared.Players;

namespace Deathmatch;

public static class Immunity
{
    private const int ReminderIntervalTicks = 64 * 300;

    // Pawn addresses checked by the bot vision hook, which runs too often to resolve players.
    private static readonly HashSet<nint> _pawns = [];
    private static readonly Dictionary<int, nint> _pawnByPlayerId = [];

    public static bool IsImmunePawn(nint address) => _pawns.Contains(address);

    public static void Think()
    {
        if (
            ConVars.IsImmunityEnabled.Value
            && Runtime.Core.Engine.GlobalVars.TickCount % ReminderIntervalTicks == 0
        )
            Runtime.Core.PlayerManager.SendChat(
                Runtime.Core.Localizer["dm.immunity_reminder", Rules.GetChatPrefix()]
            );
    }

    public static void Update(IPlayer player)
    {
        if (!ConVars.IsImmunityEnabled.Value || player.IsFakeClient)
            return;
        Remove(player);
        var pawn = player.PlayerPawn;
        if (pawn == null || !pawn.IsValid)
            return;
        var isImmune = player.IsImmune();
        if (isImmune)
        {
            _pawns.Add(pawn.Address);
            _pawnByPlayerId[player.PlayerID] = pawn.Address;
        }
        pawn.SetAlpha(isImmune ? ConVars.ImmunityAlpha.Value : 255);
    }

    public static void UpdateAll()
    {
        foreach (var player in Runtime.Core.PlayerManager.GetAllValidPlayers())
            Update(player);
    }

    public static void Remove(IPlayer player)
    {
        if (_pawnByPlayerId.Remove(player.PlayerID, out var address))
            _pawns.Remove(address);
    }

    public static void Clear()
    {
        _pawns.Clear();
        _pawnByPlayerId.Clear();
    }

    public static void Reset()
    {
        Clear();
        foreach (var player in Runtime.Core.PlayerManager.GetAllValidPlayers())
        {
            var pawn = player.PlayerPawn;
            if (!player.IsFakeClient && pawn != null && pawn.IsValid)
                pawn.SetAlpha(255);
        }
    }
}
