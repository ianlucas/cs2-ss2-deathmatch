/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using SwiftlyS2.Shared.SchemaDefinitions;

namespace Deathmatch;

public class PlayerLoadout
{
    private bool _noPrimary = false;

    private Gun? _primary;
    private Gun? _secondary;

    public void Set(Gun gun)
    {
        if (gun.IsSecondary)
            _secondary = gun;
        else
        {
            _primary = gun;
            _noPrimary = false;
        }
    }

    public void UpdateSlot(gear_slot_t slot, Gun gun)
    {
        if (slot == gear_slot_t.GEAR_SLOT_RIFLE && _primary != null && _primary != gun)
            _primary = gun;
        else if (slot == gear_slot_t.GEAR_SLOT_PISTOL && _secondary != null && _secondary != gun)
            _secondary = gun;
    }

    public void SetNoPrimary(bool value)
    {
        _noPrimary = value;
    }

    public Gun? GetPrimary() => _primary;

    public Gun? GetSecondary() => _secondary;

    public bool HasNoPrimary() => _noPrimary;
}
