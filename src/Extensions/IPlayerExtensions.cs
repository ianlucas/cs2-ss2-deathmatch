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
            self.PlayerPawn?.WeaponServices?.RemoveWeaponBySlot(gun.GearSlot);
            self.PlayerPawn?.ItemServices?.GiveItem(gun.DesignerName);
            self.GetState().GetLoadout(mode.Name).Set(gun);
        }

        public void GiveLoadout()
        {
            var mode = DMCtx.GetCurrentMode();
            if (mode == null)
                return;
            if (self.IsFakeClient)
            {
                var botLoadout = mode.BotLoadout;
                if (botLoadout == null)
                    return;
                var secondary = PickBotGun(botLoadout.Secondary);
                var primary = botLoadout.Primary != null ? PickBotGun(botLoadout.Primary) : null;
                var pawn = self.Pawn?.As<CCSPlayerPawn>();
                if (secondary != null)
                    pawn?.ItemServices?.GiveItem(secondary.DesignerName);
                if (primary != null)
                    pawn?.ItemServices?.GiveItem(primary.DesignerName);
                return;
            }
            else
            {
                var loadout = self.GetState().GetLoadout(mode.Name);
                var primary = loadout.Primary ?? DMCtx.DefaultGuns?.Primary;
                var secondary = loadout.Secondary ?? DMCtx.DefaultGuns?.Secondary;
                if (secondary != null)
                    self.PlayerPawn?.ItemServices?.GiveItem(secondary.DesignerName);
                if (primary != null)
                    self.PlayerPawn?.ItemServices?.GiveItem(primary.DesignerName);
            }
            self.SetArmor(100);
            if (self.PlayerPawn?.ItemServices?.HasHelmet != mode.Helmet)
            {
                self.PlayerPawn?.ItemServices?.HasHelmet = mode.Helmet;
                self.PlayerPawn?.ItemServices?.HasHelmetUpdated();
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
                return DMCtx.Guns.FirstOrDefault(g => g.DesignerName == $"weapon_{botGun.Gun}");
        }
        return null;
    }
}
