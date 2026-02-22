/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace Deathmatch;

public class BotLoadout(List<BotWeapon> secondary, List<BotWeapon>? primary = null)
{
    public List<BotWeapon> Secondary { get; set; } = secondary;

    public List<BotWeapon>? Primary { get; set; } = primary;
}
