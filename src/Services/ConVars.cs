/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using SwiftlyS2.Shared.Convars;

namespace Deathmatch;

public static class ConVars
{
    public static readonly IConVar<string> ChatPrefix = Swiftly.Core.ConVar.CreateOrFind(
        "dm_chat_prefix",
        "Prefix displayed before chat messages.",
        "[{red}Deathmatch{default}]"
    );

    public static IConVar<int> InfinityAmmo =>
        Swiftly.Core.ConVar.Find<int>("sv_infinite_ammo") ?? throw new InvalidOperationException(
            "Failed to find sv_infinite_ammo ConVar!"
        );

    public static IConVar<int> FreeArmor =>
        Swiftly.Core.ConVar.Find<int>("mp_free_armor") ?? throw new InvalidOperationException(
            "Failed to find mp_free_armor ConVar!"
        );

    public static void Initialize()
    {
        _ = ChatPrefix;
    }
}
