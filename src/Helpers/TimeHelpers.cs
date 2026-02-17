/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace Deathmatch;

public static class TimeHelper
{
    public static string FormatMmSs(long seconds)
    {
        return $"{seconds / 60:D2}:{seconds % 60:D2}";
    }
}
