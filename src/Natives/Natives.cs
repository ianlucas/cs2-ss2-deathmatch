/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using SwiftlyS2.Shared.Memory;

namespace Deathmatch;

public static partial class Natives
{
    private static IUnmanagedFunction<TDelegate> GetFunctionBySignature<TDelegate>(
        string signatureName
    )
        where TDelegate : Delegate
    {
        var address = Runtime.Core.GameData.GetSignature(signatureName);
        if (address == 0)
            throw new InvalidOperationException(
                $"Failed to locate game function signature '{signatureName}'. The function may not exist in the current game version or the signature pattern may be outdated."
            );
        return Runtime.Core.Memory.GetUnmanagedFunctionByAddress<TDelegate>(address);
    }
}
