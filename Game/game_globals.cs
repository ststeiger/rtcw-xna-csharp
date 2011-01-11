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

// game_globals.cs (c) 2010 JV Software
//

using idLib.Game.Server;
using idLib.Engine.Public;

using Game.AAS;

namespace Game
{
    //
    // CVars
    //
    static class Cvars
    {
        public static idCVar g_skipLevelScript;
        public static idCVar g_gravity;

        //
        // RegisterCvars
        //
        public static void RegisterCvars()
        {
            g_skipLevelScript = Engine.cvarManager.Cvar_Get("g_skipLevelScript", "0", idCVar.CVAR_ROM);
            g_gravity = Engine.cvarManager.Cvar_Get("g_gravity", "800", idCVar.CVAR_ROM);
        }
    }

    //
    // Level
    //
    static class Level
    {
        public const string GAME = "RTCW";
        public const string GAMEVERSION = GAME + " " + Engine.CPUSTRING + "\n";

        public static idGameSpawner spawner;
        public static idEntity[] entities = new idEntity[idGamePublic.MAX_GENTITIES];
        public static int num_entities = 0;
        public static int num_clients = 0;

        public static idScript script;
        public static idScript aiscript;

        public static string mapname;

        public static int time;
        public static int cameranum = -1;
        public static string camerapath = "";

        public static idGameNetwork net = new idGameNetwork();
        public static idWorld world;
        public static idAAS aas;

        //
        // TriggerEntity
        //
        public static void TriggerEntity(idEntity other, string targetname)
        {
            if (targetname == null || targetname.Length <= 0)
                return;

            foreach (idEntity target in entities)
            {
                if (target == null)
                    continue;

                if (target.targetname == targetname)
                {
                    target.Use(other);
                    return;
                }
            }

       //     Engine.common.Warning("G_TriggerEntity: Failed to trigger entity " + targetname + "\n");
        }
    }
}
