/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using Microsoft.Extensions.Logging;
using SwiftlyS2.Shared.GameEventDefinitions;
using SwiftlyS2.Shared.Players;
using SwiftlyS2.Shared.ProtobufDefinitions;
using SwiftlyS2.Shared.SchemaDefinitions;

namespace Deathmatch;

public partial class Deathmatch
{
    public void HandleOnTick()
    {
        DMCtx.Think();
        var hudNa = Core.Localizer["dm.hud_na"];
        var current = DMCtx.GetCurrentMode()?.Name ?? hudNa;
        var remaining = DMCtx.GetRemainingTime();
        var next = DMCtx.GetNextMode()?.Name ?? hudNa;
        var hudSession = Core.Localizer["dm.hud_session"];
        var hudPro = Core.Localizer["dm.hud_pro"];
        var hudCurrent = Core.Localizer["dm.hud_current"];
        var hudRemaining = Core.Localizer["dm.hud_remaining"];
        var hudNext = Core.Localizer["dm.hud_next"];
        var hudMessage = $"{hudCurrent} {current}\n{hudRemaining} {remaining}\n{hudNext} {next}";
        foreach (var player in Core.PlayerManager.GetAllValidPlayers())
        {
            if (!player.IsFakeClient && player.IsAlive)
            {
                if (Core.Engine.GlobalVars.TickCount % 64 == 0)
                {
                    player.SendAlert($"{hudSession} - {player.GetKDR()} K/D\n{hudPro} - 2.50 K/D");
                    Core.NetMessage.Send<CCSUsrMsg_HintText>(msg =>
                    {
                        msg.Message = hudMessage;
                        msg.SendToPlayer(player.PlayerID);
                    });
                }
            }
        }
    }

    public static void HandlePlayerGunRequest(IPlayer player, Gun gun)
    {
        if (!player.IsAlive || DMCtx.GetCurrentMode()?.AllowsGun(gun) != true)
            return;
        player.SwitchGun(gun);
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
            var amount = isHeadshot ? 25 : 10;
            pawn.Health = Math.Min(Math.Max(pawn.Health + amount, 0), 100);
            pawn.HealthUpdated();
            pawn.ArmorValue = Math.Min(Math.Max(pawn.ArmorValue + amount, 0), 100);
            pawn.ArmorValueUpdated();
        }
    }

    public void HandlePlayerDeath(IPlayer attacker, IPlayer victim)
    {
        // TODO This is crashing right now.
        // var record = attacker.Controller.DamageServices?.DamageList.FirstOrDefault(r =>
        //     r.PlayerControllerDamager.Value?.SteamID == victim.SteamID
        // );
        // var damage = record != null ? $"[yellow]{(int)record.Damage}" : "[red]no";
        // var hits = record != null ? $" ([lime]{record.NumHits}[white] hits)" : "";
        // victim.SendChat(
        //     $"You did {damage}[white]{hits} damage to [lime]{attacker.Controller.PlayerName}[white]."
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

    public static bool HandlePlayerGunAcquire(IPlayer player, Gun gun, CCSWeaponBaseVData vData)
    {
        if (
            vData.GearSlot != gear_slot_t.GEAR_SLOT_RIFLE
            && vData.GearSlot != gear_slot_t.GEAR_SLOT_PISTOL
        )
            return true;
        if (DMCtx.GetCurrentMode()?.AllowsGun(gun) != true)
            return false;
        player.GetState().GetLoadout().Set(gun);
        return true;
    }

    public static void HandlePlayerDisconnect(IPlayer player)
    {
        player.RemoveSessionState();
    }
}
