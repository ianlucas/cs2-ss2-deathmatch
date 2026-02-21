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

        public void SwitchGun(Gun gun)
        {
            var mode = DMCtx.GetCurrentMode();
            if (mode == null)
                return;
            var originalTeam = self.Controller.Team;
            self.PlayerPawn?.WeaponServices?.RemoveWeaponBySlot(gun.GearSlot);
            if (gun.Team != Team.None && gun.Team != originalTeam)
                self.Controller.Team = gun.Team;
            self.PlayerPawn?.ItemServices?.GiveItem(gun.DesignerName);
            self.Controller.Team = originalTeam;
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
                var botLoadout = mode.BotLoadout;
                if (botLoadout == null)
                    return;
                var secondary = PickBotGun(botLoadout.Secondary);
                var primary = botLoadout.Primary != null ? PickBotGun(botLoadout.Primary) : null;
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
                var originalTeam = self.Controller.Team;
                if (secondary != null)
                {
                    if (secondary.Team != Team.None && secondary.Team != originalTeam)
                        self.Controller.Team = secondary.Team;
                    pawn?.ItemServices?.GiveItem(secondary.DesignerName);
                    self.Controller.Team = originalTeam;
                }
                if (primary != null)
                {
                    if (primary.Team != Team.None && primary.Team != originalTeam)
                        self.Controller.Team = primary.Team;
                    pawn?.ItemServices?.GiveItem(primary.DesignerName);
                    self.Controller.Team = originalTeam;
                }
            }
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
