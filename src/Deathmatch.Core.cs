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
    public void HandleModesFileChanged()
    {
        var modes = Modes.Load();
        if (modes != null)
            DMCtx.SetModes(modes);
    }

    public void HandleOnTick()
    {
        DMCtx.Think();
        if (Core.Engine.GlobalVars.TickCount % 64 != 0)
            return;
        var hudNa = Core.Localizer["dm.hud_na"];
        var current = DMCtx.GetCurrentMode();
        var name = current?.Name ?? hudNa;
        var remaining = DMCtx.GetRemainingTime();
        var remainingMmSs = TimeHelper.FormatMmSs(remaining);
        var next = DMCtx.GetNextMode()?.Name ?? hudNa;
        var hudSession = Core.Localizer["dm.hud_session"];
        var hudPro = Core.Localizer["dm.hud_pro"];
        var hudProRatio = ConVars.ProRatio.Value;
        var hudCurrent = Core.Localizer["dm.hud_current"];
        var hudRemaining = Core.Localizer["dm.hud_remaining"];
        var hudNext = Core.Localizer["dm.hud_next"];
        var hudMessage = $"{hudCurrent} {name}\n{hudRemaining} {remainingMmSs}\n{hudNext} {next}";
        var showHudMessage = DMCtx.HasMultipleModes();
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

    public static void HandlePlayerWeaponRequest(IPlayer player, Weapon weapon)
    {
        if (!player.IsAlive || DMCtx.GetCurrentMode()?.IsWeaponAllowed(weapon) != true)
            return;
        player.SwitchWeapon(weapon);
    }

    public void HandlePlayerPrintHelp(IPlayer player)
    {
        player.SendChat(Core.Localizer["dm.help_header", DMCtx.GetChatPrefix()]);
        player.SendChat(Core.Localizer["dm.help_commands"]);
        player.SendChat(Core.Localizer["dm.help_commands_help"]);
        player.SendChat(Core.Localizer["dm.help_commands_guns"]);
    }

    public void HandlePlayerSpawn(IPlayer player)
    {
        player.GiveLoadout();
        var session = player.GetSessionState();
        if (!session.IsInitialHelpSent)
        {
            player.ResetStats();
            HandlePlayerPrintHelp(player);
            session.IsInitialHelpSent = true;
        }
    }

    public static void HandlePlayerItemPickup(IPlayer player)
    {
        var inGameMoneyServices = player.Controller.InGameMoneyServices;
        if (inGameMoneyServices != null)
        {
            inGameMoneyServices.Account = 10000;
            inGameMoneyServices.AccountUpdated();
        }
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
            var amountHp = (
                isHeadshot ? ConVars.ReplenishHealthHeadshot : ConVars.ReplenishHealth
            ).Value;
            pawn.Health = Math.Min(Math.Max(pawn.Health + amountHp, 0), 100);
            pawn.HealthUpdated();
            var amountAp = (
                isHeadshot ? ConVars.ReplenishArmorHeadshot : ConVars.ReplenishArmor
            ).Value;
            pawn.ArmorValue = Math.Min(Math.Max(pawn.ArmorValue + amountAp, 0), 100);
            pawn.ArmorValueUpdated();
        }
    }

    public void HandlePlayerDeath(IPlayer attacker, IPlayer victim)
    {
        // TODO This is crashing right now.
        // var record = attacker.Controller.DamageServices?.DamageList.FirstOrDefault(r =>
        //     r.PlayerControllerDamager.Value?.SteamID == victim.SteamID
        // );
        var damageServices = attacker.Controller.DamageServices;
        if (damageServices == null)
            return;
        var record = new DamageListIterator(damageServices.Address).FirstOrDefault(r =>
            r.PlayerControllerDamager.Value?.SteamID == victim.SteamID
        );
        if (record != null)
        {
            victim.SendChat(
                Core.Localizer[
                    "dm.attacker_damage",
                    DMCtx.GetChatPrefix(),
                    (int)record.Damage,
                    $" ([lime]{record.NumHits}[white] {(record.NumHits > 1 ? Core.Localizer["dm.attacker_damage_hits"] : Core.Localizer["dm.attacker_damage_hit"])})",
                    attacker.Controller.PlayerName
                ]
            );
            return;
        }
        victim.SendChat(
            Core.Localizer[
                "dm.attacker_damage_no",
                DMCtx.GetChatPrefix(),
                attacker.Controller.PlayerName
            ]
        );
    }

    public static bool HandlePlayerWeaponAcquire(
        IPlayer player,
        Weapon weapon,
        CCSWeaponBaseVData vData
    )
    {
        if (
            vData.GearSlot != gear_slot_t.GEAR_SLOT_RIFLE
            && vData.GearSlot != gear_slot_t.GEAR_SLOT_PISTOL
        )
            return true;
        if (DMCtx.GetCurrentMode()?.IsWeaponAllowed(weapon) != true)
            return false;
        player.GetState().GetLoadout().Set(weapon);
        return true;
    }

    public static void HandlePlayerDisconnect(IPlayer player)
    {
        player.RemoveSessionState();
    }
}
