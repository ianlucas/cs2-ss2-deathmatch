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
            var mode = DMCtx.GetCurrentMode();
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
            var mode = DMCtx.GetCurrentMode();
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
