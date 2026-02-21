/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace Deathmatch;

public class Mode(
    string name,
    List<string> gunNames,
    int duration,
    bool helmet = true,
    BotLoadout? botLoadout = null
)
{
    public string Name { get; set; } = name;

    public List<string> GunNames { get; set; } = gunNames;

    public int Duration { get; set; } = duration;

    public bool Helmet { get; set; } = helmet;

    public BotLoadout? BotLoadout { get; set; } = botLoadout;

    private readonly Dictionary<ushort, Gun> _guns = gunNames
        .Select(Guns.Find)
        .OfType<Gun>()
        .ToDictionary(g => g.ItemDef);

    private readonly Gun? _defaultSecondary = gunNames
        .Select(Guns.Find)
        .OfType<Gun>()
        .FirstOrDefault(g => g.IsSecondary);

    private readonly Gun? _defaultPrimary = gunNames
        .Select(Guns.Find)
        .OfType<Gun>()
        .FirstOrDefault(g => !g.IsSecondary);

    public Gun? GetDefaultSecondary() => _defaultSecondary;

    public Gun? GetDefaultPrimary() => _defaultPrimary;

    public bool AllowsGun(Gun gun) => _guns.ContainsKey(gun.ItemDef);

    public IEnumerable<Gun> GetGuns() => _guns.Values;
}
