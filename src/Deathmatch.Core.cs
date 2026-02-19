/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using SwiftlyS2.Shared.Misc;
using SwiftlyS2.Shared.Players;
using SwiftlyS2.Shared.ProtobufDefinitions;
using SwiftlyS2.Shared.SchemaDefinitions;

namespace Deathmatch;

public partial class Deathmatch
{
    public void HandleOnTick()
    {
        Game.TryStartNextMode();
        var current = Game.CurrentMode?.Value.Name ?? "N/A";
        var remaining = Game.GetRemainingTime();
        var next = (Game.CurrentMode?.Next ?? Game.Modes.First)?.Value.Name ?? "N/A";
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
        if (!player.IsAlive)
            return;
        var mode = Game.CurrentMode;
        if (mode == null)
            return;
        if (!Game.CurrentGuns.ContainsKey(gun.ItemDef))
            return;
        var isSecondary = gun.Type == "Pistol";
        player.PlayerPawn?.WeaponServices?.RemoveWeaponBySlot(
            isSecondary ? gear_slot_t.GEAR_SLOT_PISTOL : gear_slot_t.GEAR_SLOT_RIFLE
        );
        player.PlayerPawn?.ItemServices?.GiveItem(gun.DesignerName);
        var playerState = player.GetState();
        var loadout = playerState.GetLoadout(mode.Value.Name);
        if (isSecondary)
            loadout?.Secondary = gun;
        else
            loadout?.Primary = gun;
    }

    public void HandlePlayerSpawnGuns(IPlayer player)
    {
        player.GiveLoadout();
    }

    public bool HandlePlayerGunAcquire(
        IPlayer player,
        AcquireMethod method,
        Gun gun,
        CCSWeaponBaseVData vData
    )
    {
        if (
            vData.GearSlot != gear_slot_t.GEAR_SLOT_RIFLE
            && vData.GearSlot != gear_slot_t.GEAR_SLOT_PISTOL
        )
            return true;
        if (Game.CurrentMode == null || !Game.CurrentGuns.ContainsKey(gun.ItemDef))
            return false;
        var playerState = player.GetState();
        var loadout = playerState.GetLoadout(Game.CurrentMode.Value.Name);
        if (vData.GearSlot == gear_slot_t.GEAR_SLOT_RIFLE)
        {
            if (loadout.Primary != null && loadout.Primary != gun)
                loadout.Primary = gun;
        }
        else if (vData.GearSlot == gear_slot_t.GEAR_SLOT_PISTOL)
        {
            if (loadout.Secondary != null && loadout.Secondary != gun)
                loadout.Secondary = gun;
        }
        return true;
    }
}
