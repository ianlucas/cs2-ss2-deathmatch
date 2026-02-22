/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace Deathmatch;

public class PlayerLoadout
{
    private bool _noprimary = false;

    private Weapon? _primary;
    private Weapon? _secondary;

    public void Set(Weapon weapon)
    {
        if (weapon.IsSecondary)
            _secondary = weapon;
        else
        {
            _primary = weapon;
            _noprimary = false;
        }
    }

    public void SetNoprimary(bool value)
    {
        _noprimary = value;
    }

    public Weapon? GetPrimary() => _noprimary ? null : _primary;

    public Weapon? GetSecondary() => _secondary;

    public bool HasNoPrimary() => _noprimary;
}
