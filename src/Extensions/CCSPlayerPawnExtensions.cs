/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using SwiftlyS2.Shared.SchemaDefinitions;

namespace Deathmatch;

public static class CCSPlayerPawnExtensions
{
    extension(CCSPlayerPawn self)
    {
        public void SetAlpha(int alpha)
        {
            if (self.Render.A == alpha)
                return;
            self.Render.A = (byte)alpha;
            self.RenderUpdated();
            self.RenderMode =
                alpha < 255 ? RenderMode_t.kRenderTransAlpha : RenderMode_t.kRenderNormal;
            self.RenderModeUpdated();
        }
    }
}
