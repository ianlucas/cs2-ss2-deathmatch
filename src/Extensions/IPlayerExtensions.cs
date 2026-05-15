/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Collections.Concurrent;
using SwiftlyS2.Shared.Players;
using SwiftlyS2.Shared.SchemaDefinitions;

namespace Deathmatch;

public static class IPlayerExtensions
{
    private static readonly ConcurrentDictionary<ulong, PlayerState> _stateMng = [];
    private static readonly ConcurrentDictionary<ulong, PlayerSessionState> _sessionStateMng = [];

    extension(IPlayer self)
    {
        public PlayerState GetState()
        {
            return _stateMng.GetOrAdd(self.SteamID, _ => new());
        }

        public PlayerSessionState GetSessionState()
        {
            return _sessionStateMng.GetOrAdd(self.SteamID, _ => new());
        }

        public void RemoveState()
        {
            _stateMng.TryRemove(self.SteamID, out var _);
        }

        public void RemoveSessionState()
        {
            _sessionStateMng.TryRemove(self.SteamID, out var _);
        }

        public void PrintHelp()
        {
            self.SendChat(Runtime.Core.Localizer["dm.help_header", Rules.GetChatPrefix()]);
            self.SendChat(Runtime.Core.Localizer["dm.help_commands"]);
            self.SendChat(Runtime.Core.Localizer["dm.help_commands_help"]);
            self.SendChat(Runtime.Core.Localizer["dm.help_commands_guns"]);
        }

        public void SetHealth(int value)
        {
            var pawn = self.IsFakeClient ? self.Pawn : self.PlayerPawn;
            if (pawn != null && pawn.Health != value)
            {
                pawn.Health = value;
                pawn.HealthUpdated();
            }
        }

        public void SetArmor(int value)
        {
            var pawn = (self.IsFakeClient ? self.Pawn : self.PlayerPawn)?.As<CCSPlayerPawn>();
            if (pawn != null && pawn.ArmorValue != value)
            {
                pawn.ArmorValue = value;
                pawn.ArmorValueUpdated();
            }
        }

        public void OnSpawn()
        {
            self.GiveLoadout();
            var session = self.GetSessionState();
            if (!session.IsInitialHelpSent)
            {
                self.ResetStats();
                self.PrintHelp();
                session.IsInitialHelpSent = true;
            }
        }

        public void RequestWeapon(Weapon weapon)
        {
            if (!self.IsAlive || Rules.GetCurrentMode()?.IsWeaponAllowed(weapon) != true)
                return;
            self.SwitchWeapon(weapon);
        }

        public void GiveWeapon(Weapon weapon)
        {
            var originalTeam = self.Controller.Team;
            if (weapon.Team != Team.None && weapon.Team != originalTeam)
                self.Controller.Team = weapon.Team;
            self.PlayerPawn?.ItemServices?.GiveItem(weapon.DesignerName);
            self.Controller.Team = originalTeam;
        }

        public void SwitchWeapon(Weapon weapon)
        {
            var mode = Rules.GetCurrentMode();
            if (mode == null)
                return;
            var loadout = self.GetState().GetLoadout();
            if (loadout.GetPrimary() == weapon)
                return;
            self.PlayerPawn?.WeaponServices?.RemoveWeaponBySlot(weapon.GearSlot);
            self.GiveWeapon(weapon);
            loadout.Set(weapon);
            self.PlayerPawn?.WeaponServices?.SelectWeaponBySlot(weapon.GearSlot);
        }

        public void GiveLoadout()
        {
            var mode = Rules.GetCurrentMode();
            if (mode == null)
                return;
            var pawn = (self.IsFakeClient ? self.PlayerPawn : self.Pawn)?.As<CCSPlayerPawn>();
            if (self.IsFakeClient)
            {
                var loadout = mode.BotLoadout;
                if (loadout == null)
                    return;
                var secondary = PickBotWeapon(loadout.Secondary);
                var primary = loadout.Primary != null ? PickBotWeapon(loadout.Primary) : null;
                if (secondary != null)
                    pawn?.ItemServices?.GiveItem(secondary.DesignerName);
                if (primary != null)
                    pawn?.ItemServices?.GiveItem(primary.DesignerName);
            }
            else
            {
                var loadout = self.GetState().GetLoadout();
                var primary =
                    loadout.GetPrimary()
                    ?? (loadout.HasNoPrimary() ? null : mode.GetDefaultPrimary());
                var secondary = loadout.GetSecondary() ?? mode.GetDefaultSecondary();
                if (secondary != null)
                    self.GiveWeapon(secondary);
                if (primary != null)
                    self.GiveWeapon(primary);
            }
        }

        public void OnPickupItem()
        {
            var inGameMoneyServices = self.Controller.InGameMoneyServices;
            if (inGameMoneyServices != null)
            {
                inGameMoneyServices.Account = 10000;
                inGameMoneyServices.AccountUpdated();
            }
        }

        public bool OnAcquireWeapon(Weapon weapon, CCSWeaponBaseVData vData)
        {
            if (
                vData.GearSlot != gear_slot_t.GEAR_SLOT_RIFLE
                && vData.GearSlot != gear_slot_t.GEAR_SLOT_PISTOL
            )
                return true;
            if (Rules.GetCurrentMode()?.IsWeaponAllowed(weapon) != true)
                return false;
            self.GetState().GetLoadout().Set(weapon);
            return true;
        }

        public void OnWeaponKill(CBasePlayerWeapon weapon, bool isHeadshot)
        {
            var vData = weapon.PlayerWeaponVData;
            weapon.Clip1 = vData.DefaultClip1 + 1;
            weapon.Clip1Updated();
            var pawn = (self.IsFakeClient ? self.Pawn : self.PlayerPawn)?.As<CCSPlayerPawn>();
            if (pawn != null && pawn.IsValid && self.IsAlive)
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

        public void OnKillPlayer(IPlayer victim)
        {
            var record = self.Controller.DamageServices?.DamageList.FirstOrDefault(r =>
                r.PlayerControllerDamager.Value?.SteamID == victim.SteamID
            );
            if (record != null)
            {
                victim.SendChat(
                    Runtime.Core.Localizer[
                        "dm.attacker_damage",
                        Rules.GetChatPrefix(),
                        (int)record.Damage,
                        $" ([lime]{record.NumHits}[white] {(record.NumHits > 1 ? Runtime.Core.Localizer["dm.attacker_damage_hits"] : Runtime.Core.Localizer["dm.attacker_damage_hit"])})",
                        self.Controller.PlayerName
                    ]
                );
                return;
            }
            victim.SendChat(
                Runtime.Core.Localizer[
                    "dm.attacker_damage_no",
                    Rules.GetChatPrefix(),
                    self.Controller.PlayerName
                ]
            );
        }

        public string GetKDR()
        {
            var matchStats = self.Controller.ActionTrackingServices?.MatchStats;
            if (matchStats == null)
                return "0.00";
            var kills = Math.Max(0, matchStats.Kills);
            var deaths = Math.Max(1, matchStats.Deaths);
            return (kills / (float)deaths).ToString("0.00");
        }

        public void ResetStats()
        {
            var matchStats = self.Controller.ActionTrackingServices?.MatchStats;
            if (matchStats == null)
                return;
            matchStats.Kills = 0;
            matchStats.KillsUpdated();
            matchStats.Deaths = 0;
            matchStats.DeathsUpdated();
        }

        public void OnDisconnect()
        {
            self.RemoveSessionState();
        }
    }

    private static Weapon? PickBotWeapon(List<BotWeapon> botWeapons)
    {
        var roll = Random.Shared.NextSingle();
        var cumulative = 0f;
        foreach (var botWeapon in botWeapons)
        {
            cumulative += botWeapon.Probability;
            if (roll < cumulative)
                return Weapons.Find(botWeapon.Weapon);
        }
        return null;
    }
}
