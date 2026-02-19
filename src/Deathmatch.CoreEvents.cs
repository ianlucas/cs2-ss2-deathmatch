/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using SwiftlyS2.Shared.Events;
using SwiftlyS2.Shared.Misc;
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
    }

    public static void OnCommandExecute(IOnCommandExecuteHookEvent @event)
    {
        var commandName = @event.Command.Arg(0);
        if (@event.HookMode == HookMode.Pre)
            switch (commandName)
            {
                case "autobuy":
                    @event.Result = HookResult.Stop;
                    break;
            }
    }

    public void OnCanAcquire(IOnItemServicesCanAcquireHookEvent @event)
    {
        var player = @event.ItemServices.GetPlayer();
        var vData = @event.WeaponVData;
        if (player == null || player.IsFakeClient || vData == null)
            return;
        if (vData.GearSlot == gear_slot_t.GEAR_SLOT_KNIFE)
            return;
        var gun = Game.Guns.FirstOrDefault(g =>
            g.ItemDef == @event.EconItemView.ItemDefinitionIndex
        );
        if (gun == null || !HandlePlayerGunAcquire(player, gun, vData))
            @event.SetAcquireResult(AcquireResult.NotAllowedByMode);
        else
            @event.SetAcquireResult(AcquireResult.Allowed);
    }
}
