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
        var mode = DMCtx.GetCurrentMode();
        context.Sender?.SendChat(
            Core.Localizer[
                "dm.guns",
                DMCtx.GetChatPrefix(),
                string.Join(
                    "[white], ",
                    (mode?.GetGuns() ?? [])
                        .Select(g => $"[lime]!{g.Aliases[0]}")
                        .Concat(mode?.HasPrimary == true ? ["[lime]!noprimary"] : [])
                )
            ]
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
