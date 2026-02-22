/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Text.Json.Serialization;

namespace Deathmatch;

public class BotWeapon(string weapon, float probability)
{
    [JsonPropertyName("weapon")]
    public string Weapon { get; set; } = weapon;

    [JsonPropertyName("probability")]
    public float Probability { get; set; } = probability;
}
