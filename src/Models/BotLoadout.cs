/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace Deathmatch;

public class BotLoadout(List<BotGun> secondary, List<BotGun>? primary = null)
{
    public List<BotGun> Secondary { get; set; } = secondary;

    public List<BotGun>? Primary { get; set; } = primary;
}
