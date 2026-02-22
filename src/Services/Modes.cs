/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Deathmatch;

public static class Modes
{
    private static readonly string _configsDir = "csgo/addons/swiftlycs2/configs";

    public static IEnumerable<Mode>? Load()
    {
        try
        {
            var filename = ConVars.ModesFile.Value;
            var candidates = new[]
            {
                Path.Combine(Swiftly.Core.GameDirectory, _configsDir, filename),
                Path.Combine(Swiftly.Core.GameDirectory, "csgo", filename),
            };
            var path =
                candidates.FirstOrDefault(File.Exists)
                ?? (Path.IsPathRooted(filename) && File.Exists(filename) ? filename : null);
            if (path == null)
                return null;
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<Mode>>(json);
        }
        catch (Exception ex)
        {
            Swiftly.Core.Logger.LogError(
                "Error when processing \"{File}\": {Error}",
                ConVars.ModesFile.Value,
                ex.Message
            );
            return null;
        }
    }
}
