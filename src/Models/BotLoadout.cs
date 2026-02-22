/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Text.Json.Serialization;

namespace Deathmatch;

public class BotLoadout(List<BotWeapon> secondary, List<BotWeapon>? primary = null)
{
    [JsonPropertyName("secondary")]
    public List<BotWeapon> Secondary { get; set; } = secondary;

    [JsonPropertyName("primary")]
    public List<BotWeapon>? Primary { get; set; } = primary;
}
