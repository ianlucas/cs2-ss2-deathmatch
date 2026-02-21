/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

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

    public void SetNoPrimary(bool value)
    {
        _noPrimary = value;
    }

    public Gun? GetPrimary() => _noPrimary ? null : _primary;

    public Gun? GetSecondary() => _secondary;

    public bool HasNoPrimary() => _noPrimary;
}
