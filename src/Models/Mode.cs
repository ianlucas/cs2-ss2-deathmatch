/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace Deathmatch;

public class Mode(
    string name,
    List<string> weaponNames,
    int duration,
    bool helmet = true,
    BotLoadout? botLoadout = null
)
{
    public string Name { get; set; } = name;

    public List<string> WeaponNames { get; set; } = weaponNames;

    public int Duration { get; set; } = duration;

    public bool Helmet { get; set; } = helmet;

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
