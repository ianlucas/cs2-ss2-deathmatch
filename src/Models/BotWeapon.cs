/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace Deathmatch;

public class BotWeapon(string weapon, float probability)
{
    public string Weapon { get; set; } = weapon;

    public float Probability { get; set; } = probability;
}
