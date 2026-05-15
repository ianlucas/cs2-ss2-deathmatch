/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using SwiftlyS2.Shared.GameEventDefinitions;
using SwiftlyS2.Shared.Misc;

namespace Deathmatch;

public partial class Deathmatch
{
    public HookResult OnPlayerTeamPre(EventPlayerTeam @event)
    {
        return HookResult.Stop;
    }

    public HookResult OnPlayerSpawn(EventPlayerSpawn @event)
    {
        var player = @event.UserIdPlayer;
        if (player != null)
            player.OnSpawn();
        return HookResult.Continue;
    }

    public static HookResult OnItemPickup(EventItemPickup @event)
    {
        var player = @event.UserIdPlayer;
        if (player != null)
            player.OnPickupItem();
        return HookResult.Continue;
    }

    public HookResult OnPlayerDeath(EventPlayerDeath @event)
    {
        var victim = @event.UserIdPlayer;
        var attacker = @event.AttackerPlayer;
        if (attacker != null && victim != null)
        {
            var weapon = attacker.PlayerPawn?.WeaponServices?.ActiveWeapon.Value;
            if (weapon != null)
                attacker.OnWeaponKill(weapon, @event.Headshot);
            if (!victim.IsFakeClient && attacker != victim)
                attacker.OnKillPlayer(victim);
        }
        return HookResult.Continue;
    }

    public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event)
    {
        var player = @event.UserIdPlayer;
        if (player != null)
            player.OnDisconnect();
        return HookResult.Continue;
    }
}
