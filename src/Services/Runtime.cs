/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Runtime.CompilerServices;
using SwiftlyS2.Shared;

namespace Deathmatch;

public static class Runtime
{
    [SwiftlyInject]
    public static ISwiftlyCore Core { get; set; } = null!;

    public static void Initialize() =>
        RuntimeHelpers.RunClassConstructor(typeof(Runtime).TypeHandle);
}
