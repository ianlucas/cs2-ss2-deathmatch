/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using SwiftlyS2.Shared.Misc;
using SwiftlyS2.Shared.ProtobufDefinitions;

namespace Deathmatch;

public partial class Deathmatch
{
    public HookResult OnMsgPlaceDecal(CMsgPlaceDecalEvent msg)
    {
        msg.Recipients.RemoveAllPlayers();
        return HookResult.Stop;
    }
}
