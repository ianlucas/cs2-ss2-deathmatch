/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Collections.Concurrent;

namespace Deathmatch;

public class PlayerState
{
    private readonly ConcurrentDictionary<string, PlayerLoadout> _loadout = new();

    public PlayerLoadout GetLoadout()
    {
        return _loadout.GetOrAdd(DMCtx.GetCurrentMode()?.Name ?? "Default", _ => new());
    }
}
