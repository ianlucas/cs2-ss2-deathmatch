/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using SwiftlyS2.Shared.Misc;
using SwiftlyS2.Shared.Players;
using SwiftlyS2.Shared.ProtobufDefinitions;

namespace Deathmatch;

public partial class Deathmatch
{
    public void HandleOnTick()
    {
        Game.TryStartNextMode();
        var current = Game.CurrentMode?.Value.Name ?? "N/A";
        var remaining = Game.GetRemainingTime();
        var next = Game.CurrentMode?.Next?.Value.Name ?? "N/A";
        foreach (var player in Core.PlayerManager.GetAllValidPlayers())
        {
            if (!player.IsFakeClient && player.IsAlive)
            {
                player.SendAlert($"Session - 2.00 K/D\nPRO - 2.50 K/D");
                if (Core.Engine.GlobalVars.TickCount % 64 == 0)
                    Core.NetMessage.Send<CCSUsrMsg_HintText>(msg =>
                    {
                        msg.Message = $"Current: {current}\nRemaining: {remaining}\nNext: {next}";
                        msg.SendToPlayer(player.PlayerID);
                    });
            }
        }
    }

    public void HandlePlayerGunRequest(IPlayer player, Gun gun)
    {
        player.PlayerPawn?.ItemServices?.GiveItem(gun.DesignerName);
    }

    public bool HandlePlayerGunAcquire(IPlayer player, Gun gun)
    {
        if (!Game.CurrentGuns.ContainsKey(gun.ItemDef))
            return false;
        return true;
    }
}
