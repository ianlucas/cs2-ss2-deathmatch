/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using SwiftlyS2.Shared.Commands;

namespace Deathmatch;

public partial class Deathmatch
{
    [Command("guns")]
    public void OnGunsCommand(ICommandContext context)
    {
        context.Sender?.SendChat(
            $"Weapons: {string.Join("[white], ", Game.CurrentGuns.Values.Select(g => $"[lime]!{g.Aliases[0]}"))}"
        );
    }
}
