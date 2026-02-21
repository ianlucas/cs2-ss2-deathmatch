/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using SwiftlyS2.Shared.Commands;
using SwiftlyS2.Shared.SchemaDefinitions;

namespace Deathmatch;

public partial class Deathmatch
{
    [Command("guns")]
    public void OnGunsCommand(ICommandContext context)
    {
        context.Sender?.SendChat(
            $"Weapons: {string.Join("[white], ", (DMCtx.GetCurrentMode()?.GetGuns() ?? []).Select(g => $"[lime]!{g.Aliases[0]}"))}"
        );
    }

    [Command("noprimary")]
    public void OnNoprimaryCommand(ICommandContext context)
    {
        var player = context.Sender;
        if (player != null)
        {
            player.PlayerPawn?.WeaponServices?.RemoveWeaponBySlot(gear_slot_t.GEAR_SLOT_RIFLE);
            player.GetState().GetLoadout().SetNoPrimary(true);
        }
    }

    [Command("help")]
    public void OnHelpCommand(ICommandContext context)
    {
        var player = context.Sender;
        if (player != null)
            HandlePlayerPrintHelp(player);
    }
}
