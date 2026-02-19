/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Collections.Concurrent;
using SwiftlyS2.Shared.Players;
using SwiftlyS2.Shared.SchemaDefinitions;
using SwiftlyS2.Shared.Services;

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
            var pawn = self.SteamID == 0 ? self.Pawn : self.PlayerPawn;
            if (pawn?.Health != value)
            {
                pawn?.Health = value;
                pawn?.HealthUpdated();
            }
        }

        public void SetArmor(int value)
        {
            var pawn = (self.SteamID == 0 ? self.Pawn : self.PlayerPawn)?.As<CCSPlayerPawn>();
            if (pawn?.ArmorValue != value)
            {
                pawn?.ArmorValue = value;
                pawn?.ArmorValueUpdated();
            }
        }

        public void GiveLoadout()
        {
            if (Game.CurrentMode == null)
                return;
            var loadout = self.GetState().GetLoadout(Game.CurrentMode.Value.Name);
            var primary = loadout.Primary ?? Game.DefaultGuns?.Primary;
            var secondary = loadout.Secondary ?? Game.DefaultGuns?.Secondary;
            if (secondary != null)
                self.PlayerPawn?.ItemServices?.GiveItem(secondary.DesignerName);
            if (primary != null)
                self.PlayerPawn?.ItemServices?.GiveItem(primary.DesignerName);
            self.SetArmor(100);
            if (self.PlayerPawn?.ItemServices?.HasHelmet != Game.CurrentMode.Value.Helmet)
            {
                self.PlayerPawn?.ItemServices?.HasHelmet = Game.CurrentMode.Value.Helmet;
                self.PlayerPawn?.ItemServices?.HasHelmetUpdated();
            }
        }
    }
}
