/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Text.Json.Serialization;

namespace Deathmatch;

public class Mode(
    string name,
    List<string> weaponNames,
    int duration,
    bool helmet = true,
    BotLoadout? botLoadout = null
)
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = name;

    [JsonPropertyName("weapons")]
    public List<string> WeaponNames { get; set; } = weaponNames;

    [JsonPropertyName("duration")]
    public int Duration { get; set; } = duration;

    [JsonPropertyName("helmet")]
    public bool Helmet { get; set; } = helmet;

    [JsonPropertyName("bots")]
    public BotLoadout? BotLoadout { get; set; } = botLoadout;

    private readonly Dictionary<ushort, Weapon> _weapons = weaponNames
        .Select(Weapons.Find)
        .OfType<Weapon>()
        .ToDictionary(g => g.ItemDef);

    private readonly Weapon? _defaultSecondary = weaponNames
        .Select(Weapons.Find)
        .OfType<Weapon>()
        .FirstOrDefault(g => g.IsSecondary);

    private readonly Weapon? _defaultPrimary = weaponNames
        .Select(Weapons.Find)
        .OfType<Weapon>()
        .FirstOrDefault(g => !g.IsSecondary);

    public bool HasPrimary => _defaultPrimary != null;

    public Weapon? GetDefaultSecondary() => _defaultSecondary;

    public Weapon? GetDefaultPrimary() => _defaultPrimary;

    public bool IsWeaponAllowed(Weapon weapon) => _weapons.ContainsKey(weapon.ItemDef);

    public IEnumerable<Weapon> GetWeapons() => _weapons.Values;
}
