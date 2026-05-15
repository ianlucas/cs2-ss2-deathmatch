/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using SwiftlyS2.Shared.Events;
using SwiftlyS2.Shared.Misc;
using SwiftlyS2.Shared.ProtobufDefinitions;
using SwiftlyS2.Shared.SchemaDefinitions;

namespace Deathmatch;

public partial class Deathmatch
{
    public void OnConVarValueChanged(IOnConVarValueChanged @event)
    {
        switch (@event.ConVarName)
        {
            case "dm_modes_file":
                HandleModesFileChanged();
                return;
        }
    }

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

        Rules.Think();
        if (Core.Engine.GlobalVars.TickCount % 64 != 0)
            return;
        var hudNa = Core.Localizer["dm.hud_na"];
        var current = Rules.GetCurrentMode();
        var name = current?.Name ?? hudNa;
        var remaining = Rules.GetRemainingTime();
        var remainingMmSs = TimeHelper.FormatMmSs(remaining);
        var next = Rules.GetNextMode()?.Name ?? hudNa;
        var hudSession = Core.Localizer["dm.hud_session"];
        var hudPro = Core.Localizer["dm.hud_pro"];
        var hudProRatio = ConVars.ProRatio.Value;
        var hudCurrent = Core.Localizer["dm.hud_current"];
        var hudRemaining = Core.Localizer["dm.hud_remaining"];
        var hudNext = Core.Localizer["dm.hud_next"];
        var hudMessage = $"{hudCurrent} {name}\n{hudRemaining} {remainingMmSs}\n{hudNext} {next}";
        var showHudMessage = Rules.HasMultipleModes();
        if (remaining <= 3)
        {
            CountdownBeepSound.Recipients.AddAllPlayers();
            CountdownBeepSound.Emit();
            CountdownBeepSound.Recipients.RemoveAllPlayers();
        }
        foreach (var player in Core.PlayerManager.GetAllValidPlayers())
            if (!player.IsFakeClient)
            {
                player.SendAlert(
                    $"{hudSession} - {player.GetKDR()} K/D\n{hudPro} - {hudProRatio} K/D"
                );
                if (showHudMessage)
                    Core.NetMessage.Send<CCSUsrMsg_HintText>(msg =>
                    {
                        msg.Message = hudMessage;
                        msg.SendToPlayer(player.PlayerID);
                    });
            }
    }

    public void OnConfigsExecuted()
    {
        Config.ExecDeathmatch();
        ConVars.InfinityAmmo.Value = 2;
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
        var weapon = Weapons.GetByItemDef(@event.EconItemView.ItemDefinitionIndex);
        if (weapon == null || !player.OnAcquireWeapon(weapon, vData))
            @event.SetAcquireResult(AcquireResult.NotAllowedByMode);
    }
}
