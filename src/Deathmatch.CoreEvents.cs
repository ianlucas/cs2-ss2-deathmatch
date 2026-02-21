/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using Microsoft.Extensions.Logging;
using SwiftlyS2.Shared.Commands;
using SwiftlyS2.Shared.Events;
using SwiftlyS2.Shared.Misc;
using SwiftlyS2.Shared.Players;
using SwiftlyS2.Shared.SchemaDefinitions;

namespace Deathmatch;

public partial class Deathmatch
{
    public void OnMapLoad(IOnMapLoadEvent @event)
    {
        PendingInternalPush = true;
    }

    public void OnTick()
    {
        if (PendingInternalPush)
        {
            PendingInternalPush = false;
            OnConfigsExecuted();
        }

        HandleOnTick();
    }

    public void OnConfigsExecuted()
    {
        Config.ExecDeathmatch();
        Core.ConVar.Find<int>("sv_infinite_ammo")?.Value = 2;
    }

    public HookResult OnClientCommand(int playerid, string commandLine)
    {
        var player = Core.PlayerManager.GetPlayer(playerid);
        if (player != null && commandLine.StartsWith("buyrandom"))
            return HookResult.Stop;
        return HookResult.Continue;
    }

    public void OnCanAcquire(IOnItemServicesCanAcquireHookEvent @event)
    {
        var player = @event.ItemServices.GetPlayer();
        var vData = @event.WeaponVData;
        if (player == null || player.IsFakeClient || vData == null)
            return;
        if (vData.GearSlot == gear_slot_t.GEAR_SLOT_KNIFE)
            return;
        var gun = Guns.GetByItemDef(@event.EconItemView.ItemDefinitionIndex);
        if (gun == null || !HandlePlayerGunAcquire(player, gun, vData))
            @event.SetAcquireResult(AcquireResult.NotAllowedByMode);
    }
}
