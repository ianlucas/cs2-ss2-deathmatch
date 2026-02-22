/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using SwiftlyS2.Shared;
using SwiftlyS2.Shared.GameEventDefinitions;
using SwiftlyS2.Shared.Plugins;
using SwiftlyS2.Shared.ProtobufDefinitions;

namespace Deathmatch;

[PluginMetadata(
    Id = "Deathmatch",
    Version = "1.0.0",
    Name = "Deathmatch",
    Author = "Ian Lucas",
    Description = "Deathmatch gamemode."
)]
public partial class Deathmatch(ISwiftlyCore core) : BasePlugin(core)
{
    public bool PendingInternalPush = true;

    public override void Load(bool hotReload)
    {
        Swiftly.Initialize();
        ConVars.Initialize();
        Core.GameData.ApplyPatch("RandomSpawnPatch");
        Core.GameData.ApplyPatch("DeathmatchScorePatch");
        Core.GameData.ApplyPatch("RespawnSoundPatch");
        Core.Event.OnMapLoad += OnMapLoad;
        Core.Event.OnTick += OnTick;
        Core.Event.OnItemServicesCanAcquireHook += OnCanAcquire;
        Core.GameEvent.HookPre<EventPlayerTeam>(OnPlayerTeamPre);
        Core.GameEvent.HookPost<EventPlayerSpawn>(OnPlayerSpawn);
        Core.GameEvent.HookPost<EventItemPickup>(OnItemPickup);
        Core.GameEvent.HookPost<EventPlayerDeath>(OnPlayerDeath);
        Core.GameEvent.HookPost<EventPlayerDisconnect>(OnPlayerDisconnect);
        Core.Command.HookClientCommand(OnClientCommand);
        Core.NetMessage.HookServerMessage<CMsgPlaceDecalEvent>(OnMsgPlaceDecal);
        foreach (var gun in Guns.All)
        foreach (var name in gun.Aliases)
            Core.Command.RegisterCommand(
                name,
                (context) =>
                {
                    var player = context.Sender;
                    if (player != null)
                        HandlePlayerGunRequest(player, gun);
                }
            );
    }

    public override void Unload() { }
}
