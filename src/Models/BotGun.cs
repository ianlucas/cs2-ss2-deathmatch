/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace Deathmatch;

public class BotGun(string gun, float probability)
{
    public string Gun { get; set; } = gun;

    public float Probability { get; set; } = probability;
}
