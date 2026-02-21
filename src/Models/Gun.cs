/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using SwiftlyS2.Shared.Players;
using SwiftlyS2.Shared.SchemaDefinitions;

namespace Deathmatch;

public class Gun(
    string designerName,
    string type,
    List<string> aliases,
    ushort itemDef,
    Team team = Team.None
)
{
    public string DesignerName { get; set; } = designerName;
    public string Type { get; set; } = type;
    public List<string> Aliases { get; set; } = aliases;
    public ushort ItemDef { get; set; } = itemDef;
    public Team Team { get; set; } = team;
    public bool IsPrimary => !IsSecondary;
    public bool IsSecondary => Type == "Pistol";
    public gear_slot_t GearSlot =>
        IsSecondary ? gear_slot_t.GEAR_SLOT_PISTOL : gear_slot_t.GEAR_SLOT_RIFLE;
}
