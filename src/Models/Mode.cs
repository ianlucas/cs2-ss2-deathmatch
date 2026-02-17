/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace Deathmatch;

public class Mode(
    string name,
    List<string> guns,
    int duration,
    int freeArmor = 2,
    BotLoadout? botLoadout = null
)
{
    public string Name { get; set; } = name;

    public List<string> Guns { get; set; } = guns;

    public int Duration { get; set; } = duration;

    public int FreeArmor { get; set; } = freeArmor;

    public BotLoadout? BotLoadout { get; set; } = botLoadout;
}
