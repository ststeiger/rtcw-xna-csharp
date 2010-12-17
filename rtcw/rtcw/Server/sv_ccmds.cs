/*
===========================================================================

Return to Castle Wolfenstein XNA Managed C# Port
Copyright (c) 2010 JV Software
Copyright (C) 1999-2010 id Software LLC, a ZeniMax Media company. 

This file is part of the Return to Castle Wolfenstein XNA Managed C# Port GPL Source Code.  

RTCW C# Source Code is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

RTCW C# Source Code is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with RTCW C# Source Code.  If not, see <www.gnu.org/licenses/>.

In addition, the RTCW SP Source Code is also subject to certain additional terms. 
You should have received a copy of these additional terms immediately following the terms 
and conditions of the GNU General Public License which accompanied the RTCW C# Source Code.  
If not, please request a copy in writing from id Software at the address below.

If you have questions concerning this license or the applicable additional terms, you may contact in writing 
id Software LLC, c/o ZeniMax Media Inc., Suite 120, Rockville, Maryland 20850 USA.

===========================================================================
*/

// sv_ccmds.cs (c) 2010 JV Software
//

using System;
using idLib.Engine.Public;
using idLib.Game.Server;
using rtcw.Framework;

namespace rtcw.Server
{
    //
    // idServerConsoleCommands
    //
    static class idServerConsoleCommands
    {
        //
        // Command_Map
        //
        public static void Command_Map()
        {
            string map, cmd;

            map = Engine.cmdSystem.Cmd_Argv(1);
            if (map == null || map.Length <= 0)
            {
                return;
            }

            // make sure the level exists before trying to change, so that
            // a typo at the server console won't end the game
            if (Engine.fileSystem.FileExists("maps/" + map + ".bsp") == false)
            {
                Engine.common.Warning("Can't find map " + map + "\n");
            }

            Engine.cvarManager.Cvar_Set("r_mapFogColor", "0", true);       //----(SA)	added
            Engine.cvarManager.Cvar_Set("r_waterFogColor", "0", true);     //----(SA)	added
            Engine.cvarManager.Cvar_Set("r_savegameFogColor", "0", true);      //----(SA)	added

            // force latched values to get set
            Engine.cvarManager.Cvar_Get("g_gametype", "0", idCVar.CVAR_SERVERINFO | idCVar.CVAR_USERINFO | idCVar.CVAR_LATCH);

            // Rafael gameskill
            Engine.cvarManager.Cvar_Get("g_gameskill", "1", idCVar.CVAR_SERVERINFO | idCVar.CVAR_LATCH);
            // done

            Engine.cvarManager.Cvar_Set("g_episode", "0", true); //----(SA) added

            cmd = Engine.cmdSystem.Cmd_Argv(0);
            if (cmd.Contains("sp") == true)
            {
                Engine.cvarManager.Cvar_Set("g_gametype", "" + idGameType.GT_SINGLE_PLAYER, true);
                Engine.cvarManager.Cvar_Set("g_doWarmup", "0", true);
                // may not set sv_maxclients directly, always set latched
                Engine.cvarManager.Cvar_Set("sv_maxclients", "32", true); // Ridah, modified this
                Globals.killBots = true;
                if (cmd.Contains("devmap"))
                {
                    Globals.cheats = true;
                }
                else
                {
                    Globals.cheats = false;
                }
            }
            else
            {
                if (cmd.Contains("devmap"))
                {
                    Globals.cheats = true;
                    Globals.killBots = true;
                }
                else
                {
                    Globals.cheats = true;
                    Globals.killBots = true;
                }
                if (CVars.sv_gametype.GetValueInteger() == (int)idGameType.GT_SINGLE_PLAYER)
                {
                    Engine.cvarManager.Cvar_Set("g_gametype", "" + idGameType.GT_FFA, true);
                }
            }

            // start up the map
            Globals.sv.SpawnServer(map, Globals.killBots);

            // set the cheat value
            // if the level was started with "map <levelname>", then
            // cheats will not be allowed.  If started with "devmap <levelname>"
            // then cheats will be allowed
            if (Globals.cheats)
            {
                Engine.cvarManager.Cvar_Set("sv_cheats", "1", true);
            }
            else
            {
                Engine.cvarManager.Cvar_Set("sv_cheats", "0", true);
            }
        }
    }
}
