/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using SwiftlyS2.Shared.SchemaDefinitions;

namespace Deathmatch;

public class PlayerLoadout
{
    public Gun? Primary;
    public Gun? Secondary;

    public void Set(Gun gun)
    {
        if (gun.IsSecondary)
            Secondary = gun;
        else
            Primary = gun;
    }

    public void UpdateSlot(gear_slot_t slot, Gun gun)
    {
        if (slot == gear_slot_t.GEAR_SLOT_RIFLE && Primary != null && Primary != gun)
            Primary = gun;
        else if (slot == gear_slot_t.GEAR_SLOT_PISTOL && Secondary != null && Secondary != gun)
            Secondary = gun;
    }
}
