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
        var current = DMCtx.GetCurrentMode()?.Name ?? "N/A";
        var remaining = DMCtx.GetRemainingTime();
        var next = DMCtx.GetNextMode()?.Name ?? "N/A";
        foreach (var player in Core.PlayerManager.GetAllValidPlayers())
        {
            if (!player.IsFakeClient && player.IsAlive)
            {
                player.SendAlert($"Session - {player.GetKDR()} K/D\nPRO - 2.50 K/D");
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
        if (!player.IsAlive || DMCtx.GetCurrentMode()?.AllowsGun(gun) != true)
            return;
        player.SwitchGun(gun);
    }

    public static void HandlePlayerSpawn(IPlayer player)
    {
        player.GiveLoadout();
    }

    public static void HandlePlayerWeaponKill(
        IPlayer player,
        CBasePlayerWeapon weapon,
        bool isHeadshot
    )
    {
        var vData = weapon.PlayerWeaponVData;
        weapon.Clip1 = vData.DefaultClip1 + 1;
        weapon.Clip1Updated();
        var pawn = (player.IsFakeClient ? player.Pawn : player.PlayerPawn)?.As<CCSPlayerPawn>();
        if (pawn != null && pawn.IsValid && player.IsAlive)
        {
            var amount = isHeadshot ? 25 : 10;
            pawn.Health = Math.Min(Math.Max(pawn.Health + amount, 0), 100);
            pawn.HealthUpdated();
            pawn.ArmorValue = Math.Min(Math.Max(pawn.ArmorValue + amount, 0), 100);
            pawn.ArmorValueUpdated();
        }
    }

    public static bool HandlePlayerGunAcquire(IPlayer player, Gun gun, CCSWeaponBaseVData vData)
    {
        if (
            vData.GearSlot != gear_slot_t.GEAR_SLOT_RIFLE
            && vData.GearSlot != gear_slot_t.GEAR_SLOT_PISTOL
        )
            return true;
        if (DMCtx.GetCurrentMode()?.AllowsGun(gun) != true)
            return false;
        player.GetState().GetLoadout().UpdateSlot(gun.GearSlot, gun);
        return true;
    }
}
