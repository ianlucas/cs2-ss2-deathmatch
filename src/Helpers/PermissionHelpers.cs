/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace Deathmatch;

public static class PermissionHelpers
{
    public static List<string> Split(string value)
    {
        return [.. value.Split(",").Select(s => s.Trim())];
    }
}
