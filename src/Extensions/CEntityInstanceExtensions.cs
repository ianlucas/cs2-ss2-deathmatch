/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using SwiftlyS2.Shared.Players;
using SwiftlyS2.Shared.SchemaDefinitions;

namespace Deathmatch;

public static class CEntityInstanceExtensions
{
    extension(CEntityInstance self)
    {
        public IPlayer? GetPlayer()
        {
            return self.DesignerName == "player" ? self.As<CCSPlayerPawn>().ToPlayer() : null;
        }
    }
}
