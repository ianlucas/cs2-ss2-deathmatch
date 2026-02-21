/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using SwiftlyS2.Shared.GameEventDefinitions;
using SwiftlyS2.Shared.Misc;

namespace Deathmatch;

public partial class Deathmatch
{
    public HookResult OnPlayerSpawn(EventPlayerSpawn @event)
    {
        var player = @event.UserIdPlayer;
        if (player != null)
            HandlePlayerSpawn(player);
        return HookResult.Continue;
    }

    public HookResult OnPlayerDeath(EventPlayerDeath @event)
    {
        var attacker = @event.AttackerPlayer;
        if (attacker != null)
        {
            var weapon = attacker.PlayerPawn?.WeaponServices?.ActiveWeapon.Value;
            if (weapon != null)
                HandlePlayerWeaponKill(attacker, weapon, @event.Headshot);
        }
        return HookResult.Continue;
    }
}
