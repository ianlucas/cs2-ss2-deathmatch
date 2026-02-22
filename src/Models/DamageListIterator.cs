/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Collections;
using System.Runtime.InteropServices;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.SchemaDefinitions;

namespace Deathmatch;

public class DamageListIterator(IntPtr damageServicesAddress) : IEnumerable<CDamageRecord>
{
    private readonly int RECORD_STRIDE = Helper.GetSchemaSize<CDamageRecord>();

    // Offset of m_DamageList within its parent struct
    // (72 = offset of m_Size, 80 = offset of m_pMemory)
    private const int OFFSET_COUNT = 72;
    private const int OFFSET_MEMBASE = 80;

    private readonly IntPtr _parentAddress = damageServicesAddress;

    private int Count => Marshal.ReadInt32(_parentAddress + OFFSET_COUNT);

    private IntPtr MemBase => Marshal.ReadIntPtr(_parentAddress + OFFSET_MEMBASE);

    public IEnumerator<CDamageRecord> GetEnumerator()
    {
        var count = Count;
        var memBase = MemBase;

        if (count <= 0 || memBase == IntPtr.Zero)
            yield break;

        for (int i = 0; i < count; i++)
        {
            var recordAddress = memBase + i * RECORD_STRIDE;
            yield return Swiftly.Core.Memory.ToSchemaClass<CDamageRecord>(recordAddress);
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
