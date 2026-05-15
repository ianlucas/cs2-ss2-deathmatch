/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Runtime.CompilerServices;
using SwiftlyS2.Shared.Convars;

namespace Deathmatch;

public static class ConVars
{
    public static readonly IConVar<string> ChatPrefix = Runtime.Core.ConVar.CreateOrFind(
        "dm_chat_prefix",
        "Prefix displayed before chat messages.",
        "[{red}Deathmatch{default}]"
    );

    public static readonly IConVar<string> ModesFile = Runtime.Core.ConVar.CreateOrFind(
        "dm_modes_file",
        "Path to the modes configuration file.",
        "addons/swiftlys2/plugins/Deathmatch/resources/configs/default.json"
    );

    public static readonly IConVar<string> ProRatio = Runtime.Core.ConVar.CreateOrFind(
        "dm_pro_ratio",
        "Target K/D ratio that pro players typically achieve in deathmatch.",
        "2.50"
    );

    public static readonly IConVar<int> ReplenishHealth = Runtime.Core.ConVar.CreateOrFind(
        "dm_replenish_health",
        "Amount of health replenished on kill.",
        10
    );

    public static readonly IConVar<int> ReplenishHealthHeadshot = Runtime.Core.ConVar.CreateOrFind(
        "dm_replenish_health_headshot",
        "Amount of health replenished on headshot kill.",
        25
    );

    public static readonly IConVar<int> ReplenishArmor = Runtime.Core.ConVar.CreateOrFind(
        "dm_replenish_armor",
        "Amount of armor replenished on kill.",
        5
    );

    public static readonly IConVar<int> ReplenishArmorHeadshot = Runtime.Core.ConVar.CreateOrFind(
        "dm_replenish_armor_headshot",
        "Amount of armor replenished on headshot kill.",
        20
    );

    public static IConVar<int> InfinityAmmo =>
        Runtime.Core.ConVar.Find<int>("sv_infinite_ammo") ?? throw new InvalidOperationException(
            "Failed to find sv_infinite_ammo ConVar!"
        );

    public static IConVar<int> FreeArmor =>
        Runtime.Core.ConVar.Find<int>("mp_free_armor") ?? throw new InvalidOperationException(
            "Failed to find mp_free_armor ConVar!"
        );

    public static void Initialize() =>
        RuntimeHelpers.RunClassConstructor(typeof(ConVars).TypeHandle);
}
