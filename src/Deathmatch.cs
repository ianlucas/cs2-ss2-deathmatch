/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using SwiftlyS2.Shared;
using SwiftlyS2.Shared.GameEventDefinitions;
using SwiftlyS2.Shared.Plugins;
using SwiftlyS2.Shared.ProtobufDefinitions;
using SwiftlyS2.Shared.Sounds;

namespace Deathmatch;

[PluginMetadata(
    Id = "Deathmatch",
    Version = "1.0.0",
    Name = "Deathmatch",
    Author = "Ian Lucas",
    Description = "Free-for-all deathmatch with configurable weapons."
)]
public partial class Deathmatch(ISwiftlyCore core) : BasePlugin(core)
{
    public bool PendingInternalPush = true;
    public readonly SoundEvent CountdownBeepSound = new("Alert.WarmupTimeoutBeep");

    public override void Load(bool hotReload)
    {
        Swiftly.Initialize();
        ConVars.Initialize();
        Core.GameData.ApplyPatch("RandomSpawnPatch");
        Core.GameData.ApplyPatch("DeathmatchScorePatch");
        Core.GameData.ApplyPatch("RespawnSoundPatch");
        Core.Event.OnConVarValueChanged += OnConVarValueChanged;
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
        foreach (var weapon in Weapons.All)
        foreach (var name in weapon.Aliases)
            Core.Command.RegisterCommand(
                name,
                (context) =>
                {
                    var player = context.Sender;
                    if (player != null)
                        HandlePlayerWeaponRequest(player, weapon);
                }
            );
        HandleModesFileChanged();
    }

    public override void Unload() { }
}
