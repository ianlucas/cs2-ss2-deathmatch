/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace Deathmatch;

public static class Config
{
    public static void ExecDeathmatch() =>
        Swiftly.Core.Engine.ExecuteCommand([
            "sv_hibernate_when_empty 0",
            "bot_join_after_player 0",
            "mp_warmuptime 0",
            "mp_warmup_online_enabled 0",
            "mp_roundtime 60",
            "mp_respawn_immunitytime -1",
            "mp_dm_healthshot_killcount 0",
            "mp_dm_time_between_bonus_min 3630",
            "mp_dm_time_between_bonus_max 3630",
            "mp_buytime 3630",
            "mp_buy_anywhere 1",
            "mp_buy_during_immunity 0",
            "bot_difficulty 4",
            $"mp_bot_ai_bt \"addons/swiftlys2/plugins/Deathmatch/resources/ai/bt_default.kv3\"",
        ]);
}
