/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using SwiftlyS2.Shared.Players;
using SwiftlyS2.Shared.ProtobufDefinitions;
using SwiftlyS2.Shared.SchemaDefinitions;

namespace Deathmatch;

public partial class Deathmatch
{
    public void HandleOnTick()
    {
        DMCtx.Think();
        var current = DMCtx.CurrentMode?.Value.Name ?? "N/A";
        var remaining = DMCtx.GetRemainingTime();
        var next = (DMCtx.CurrentMode?.Next ?? DMCtx.Modes.First)?.Value.Name ?? "N/A";
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

    public static void HandlePlayerGunRequest(IPlayer player, Gun gun)
    {
        if (
            !player.IsAlive
            || DMCtx.CurrentMode == null
            || !DMCtx.CurrentGuns.ContainsKey(gun.ItemDef)
        )
            return;
        player.SwitchGun(gun);
    }

    public static void HandlePlayerSpawn(IPlayer player)
    {
        player.GiveLoadout();
    }

    public static bool HandlePlayerGunAcquire(IPlayer player, Gun gun, CCSWeaponBaseVData vData)
    {
        if (
            vData.GearSlot != gear_slot_t.GEAR_SLOT_RIFLE
            && vData.GearSlot != gear_slot_t.GEAR_SLOT_PISTOL
        )
            return true;
        if (DMCtx.CurrentMode == null || !DMCtx.CurrentGuns.ContainsKey(gun.ItemDef))
            return false;
        player.GetState().GetLoadout(DMCtx.CurrentMode.Value.Name).UpdateSlot(gun.GearSlot, gun);
        return true;
    }
}
