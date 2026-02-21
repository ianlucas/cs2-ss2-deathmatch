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
    private static readonly ConcurrentDictionary<ulong, PlayerState> _playerStateManager = [];

    extension(IPlayer self)
    {
        public PlayerState GetState()
        {
            return _playerStateManager.GetOrAdd(self.SteamID, _ => new());
        }

        public void RemoveState()
        {
            _playerStateManager.TryRemove(self.SteamID, out var _);
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

        public void GiveGun(Gun gun)
        {
            var originalTeam = self.Controller.Team;
            if (gun.Team != Team.None && gun.Team != originalTeam)
                self.Controller.Team = gun.Team;
            self.PlayerPawn?.ItemServices?.GiveItem(gun.DesignerName);
            self.Controller.Team = originalTeam;
        }

        public void SwitchGun(Gun gun)
        {
            var mode = DMCtx.GetCurrentMode();
            if (mode == null)
                return;
            self.PlayerPawn?.WeaponServices?.RemoveWeaponBySlot(gun.GearSlot);
            self.GiveGun(gun);
            self.GetState().GetLoadout().Set(gun);
            self.PlayerPawn?.WeaponServices?.SelectWeaponBySlot(gun.GearSlot);
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
                var secondary = PickBotGun(loadout.Secondary);
                var primary = loadout.Primary != null ? PickBotGun(loadout.Primary) : null;
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
                    self.GiveGun(secondary);
                if (primary != null)
                    self.GiveGun(primary);
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
    }

    private static Gun? PickBotGun(List<BotGun> botGuns)
    {
        var roll = Random.Shared.NextSingle();
        var cumulative = 0f;
        foreach (var botGun in botGuns)
        {
            cumulative += botGun.Probability;
            if (roll < cumulative)
                return Guns.Find(botGun.Gun);
        }
        return null;
    }
}
