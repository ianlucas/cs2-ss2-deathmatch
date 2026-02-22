/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using SwiftlyS2.Shared.Players;
using SwiftlyS2.Shared.SchemaDefinitions;

namespace Deathmatch;

public static class CCSPlayer_ItemServicesExtensions
{
    public static IPlayer? GetPlayer(this CCSPlayer_ItemServices self)
    {
        var pawn = self.Pawn;
        return pawn != null && pawn.IsValid ? pawn.ToPlayer() : null;
    }
}
