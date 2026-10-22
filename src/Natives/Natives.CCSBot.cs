/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using SwiftlyS2.Shared.Memory;

namespace Deathmatch;

public static partial class Natives
{
    public delegate byte CCSBot_IsVisibleDelegate(
        nint thisPtr,
        nint player,
        byte testFOV,
        nint visParts
    );

    public static readonly IUnmanagedFunction<CCSBot_IsVisibleDelegate> CCSBot_IsVisible =
        GetFunctionBySignature<CCSBot_IsVisibleDelegate>("CCSBot::IsVisible");
}
