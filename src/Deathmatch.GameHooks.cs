/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using SwiftlyS2.Shared.GameHooks;
using SwiftlyS2.Shared.Misc;
using SwiftlyS2.Shared.SchemaDefinitions;

namespace Deathmatch;

public partial class Deathmatch
{
    public void OnCanAcquire(ref CanAcquireItemPostContext context)
    {
        var player = context.Params.Player;
        var vData = context.Params.WeaponVData;
        if (player == null || player.IsFakeClient || vData == null)
            return;
        if (vData.GearSlot == gear_slot_t.GEAR_SLOT_KNIFE)
            return;
        var weapon = Weapons.GetByItemDef(context.Params.EconItemView.ItemDefinitionIndex);
        if (weapon == null || !player.OnAcquireWeapon(weapon, vData))
            context.Return = AcquireResult.NotAllowedByMode;
    }
}
