/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace Deathmatch;

public partial class Deathmatch
{
    // Both legacy bot vision and the behavior tree's decorator_picker_visible go through this.
    public Natives.CCSBot_IsVisibleDelegate OnBotIsVisible(
        Func<Natives.CCSBot_IsVisibleDelegate> next
    )
    {
        return (thisPtr, player, testFOV, visParts) =>
            Immunity.IsImmunePawn(player) ? (byte)0 : next()(thisPtr, player, testFOV, visParts);
    }
}
