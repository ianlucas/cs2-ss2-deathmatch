/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace Deathmatch;

public class Gun(string designerName, string type, List<string> aliases, ushort itemDef)
{
    public string DesignerName { get; set; } = designerName;
    public string Type { get; set; } = type;
    public List<string> Aliases { get; set; } = aliases;
    public ushort ItemDef { get; set; } = itemDef;
}
