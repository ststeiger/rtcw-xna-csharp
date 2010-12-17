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

// sv_main.cs (c) 2010 JV Software
//

using System;
using idLib.Engine.Public;
using rtcw.Framework;

namespace rtcw.Server
{
    //
    // CVars
    //
    static class CVars
    {
        public const int MAX_MASTER_SERVERS = 5;
        public static idCVar sv_fps;                // time rate for running non-clients
        public static idCVar sv_timeout;            // seconds without any message
        public static idCVar sv_zombietime;         // seconds to sink messages after disconnect
        public static idCVar sv_rconPassword;       // password for remote server commands
        public static idCVar sv_privatePassword;    // password for the privateClient slots
        public static idCVar sv_allowDownload;
        public static idCVar sv_maxclients;
        public static idCVar sv_privateClients;     // number of clients reserved for password
        public static idCVar sv_hostname;
        public static idCVar[] sv_master = new idCVar[MAX_MASTER_SERVERS];     // master server ip address
        public static idCVar sv_reconnectlimit;     // minimum seconds between connect messages
        public static idCVar sv_showloss;           // report when usercmds are lost
        public static idCVar sv_padPackets;         // add nop bytes to messages
        public static idCVar sv_killserver;         // menu system can set to 1 to shut server down
        public static idCVar sv_mapname;
        public static idCVar sv_mapChecksum;
        public static idCVar sv_serverid;
        public static idCVar sv_maxRate;
        public static idCVar sv_minPing;
        public static idCVar sv_maxPing;
        public static idCVar sv_gametype;
        public static idCVar sv_pure;
        public static idCVar sv_floodProtect;
        public static idCVar sv_allowAnonymous;

        // Rafael gameskill
        public static idCVar sv_gameskill;
        // done

        public static idCVar sv_reloading;  //----(SA)	added
    }
    //
    // idServerManager
    //
    public class idServerManager
    {
        //
        // Init
        //
        public void Init()
        {
            // serverinfo vars
            Engine.cvarManager.Cvar_Get("dmflags", "0", idCVar.CVAR_SERVERINFO);
            Engine.cvarManager.Cvar_Get("fraglimit", "20", idCVar.CVAR_SERVERINFO);
            Engine.cvarManager.Cvar_Get("timelimit", "0", idCVar.CVAR_SERVERINFO);
            CVars.sv_gametype = Engine.cvarManager.Cvar_Get("g_gametype", "0", idCVar.CVAR_SERVERINFO | idCVar.CVAR_LATCH);

            // Rafael gameskill
            CVars.sv_gameskill = Engine.cvarManager.Cvar_Get("g_gameskill", "1", idCVar.CVAR_SERVERINFO | idCVar.CVAR_LATCH);
            // done

            Engine.cvarManager.Cvar_Get("sv_keywords", "", idCVar.CVAR_SERVERINFO);
            Engine.cvarManager.Cvar_Get("protocol", "" + Engine.PROTOCOL_VERSION, idCVar.CVAR_SERVERINFO | idCVar.CVAR_ROM);
            CVars.sv_mapname = Engine.cvarManager.Cvar_Get("mapname", "nomap", idCVar.CVAR_SERVERINFO | idCVar.CVAR_ROM);
            CVars.sv_privateClients = Engine.cvarManager.Cvar_Get("sv_privateClients", "0", idCVar.CVAR_SERVERINFO);
            CVars.sv_hostname = Engine.cvarManager.Cvar_Get("sv_hostname", "noname", idCVar.CVAR_SERVERINFO | idCVar.CVAR_ARCHIVE);
            CVars.sv_maxclients = Engine.cvarManager.Cvar_Get("sv_maxclients", "8", idCVar.CVAR_SERVERINFO | idCVar.CVAR_LATCH);
            CVars.sv_maxRate = Engine.cvarManager.Cvar_Get("sv_maxRate", "0", idCVar.CVAR_ARCHIVE | idCVar.CVAR_SERVERINFO);
            CVars.sv_minPing = Engine.cvarManager.Cvar_Get("sv_minPing", "0", idCVar.CVAR_ARCHIVE | idCVar.CVAR_SERVERINFO);
            CVars.sv_maxPing = Engine.cvarManager.Cvar_Get("sv_maxPing", "0", idCVar.CVAR_ARCHIVE | idCVar.CVAR_SERVERINFO);
            CVars.sv_floodProtect = Engine.cvarManager.Cvar_Get("sv_floodProtect", "1", idCVar.CVAR_ARCHIVE | idCVar.CVAR_SERVERINFO);
            CVars.sv_allowAnonymous = Engine.cvarManager.Cvar_Get("sv_allowAnonymous", "0", idCVar.CVAR_SERVERINFO);

            // systeminfo
            Engine.cvarManager.Cvar_Get("sv_cheats", "0", idCVar.CVAR_SYSTEMINFO | idCVar.CVAR_ROM);
            CVars.sv_serverid = Engine.cvarManager.Cvar_Get("sv_serverid", "0", idCVar.CVAR_SYSTEMINFO | idCVar.CVAR_ROM);
            //----(SA) VERY VERY TEMPORARY!!!!!!!!!!!
            //----(SA) this is so Activision can test milestones with
            //----(SA) the default config.  remember to change this back when shipping!!!
            CVars.sv_pure = Engine.cvarManager.Cvar_Get("sv_pure", "0", idCVar.CVAR_SYSTEMINFO);
            //	sv_pure = Engine.cvarManager.Cvar_Get ("sv_pure", "1", CVAR_SYSTEMINFO );
            Engine.cvarManager.Cvar_Get("sv_paks", "", idCVar.CVAR_SYSTEMINFO | idCVar.CVAR_ROM);
            Engine.cvarManager.Cvar_Get("sv_pakNames", "", idCVar.CVAR_SYSTEMINFO | idCVar.CVAR_ROM);
            Engine.cvarManager.Cvar_Get("sv_referencedPaks", "", idCVar.CVAR_SYSTEMINFO | idCVar.CVAR_ROM);
            Engine.cvarManager.Cvar_Get("sv_referencedPakNames", "", idCVar.CVAR_SYSTEMINFO | idCVar.CVAR_ROM);

            // server vars
            CVars.sv_rconPassword = Engine.cvarManager.Cvar_Get("rconPassword", "", idCVar.CVAR_TEMP);
            CVars.sv_privatePassword = Engine.cvarManager.Cvar_Get("sv_privatePassword", "", idCVar.CVAR_TEMP);
            CVars.sv_fps = Engine.cvarManager.Cvar_Get("sv_fps", "20", idCVar.CVAR_TEMP);
            CVars.sv_timeout = Engine.cvarManager.Cvar_Get("sv_timeout", "120", idCVar.CVAR_TEMP);
            CVars.sv_zombietime = Engine.cvarManager.Cvar_Get("sv_zombietime", "2", idCVar.CVAR_TEMP);
            Engine.cvarManager.Cvar_Get("nextmap", "", idCVar.CVAR_TEMP);

            CVars.sv_allowDownload = Engine.cvarManager.Cvar_Get("sv_allowDownload", "1", 0);
            //----(SA)	heh, whoops.  we've been talking to id masters since we got a connection...
            //	sv_master[0] = Engine.cvarManager.Cvar_Get ("sv_master1", "master3.idsoftware.com", 0 );
            CVars.sv_master[0] = Engine.cvarManager.Cvar_Get("sv_master1", Engine.MASTER_SERVER_ADDR, 0);
            CVars.sv_master[1] = Engine.cvarManager.Cvar_Get("sv_master2", "", idCVar.CVAR_ARCHIVE);
            CVars.sv_master[2] = Engine.cvarManager.Cvar_Get("sv_master3", "", idCVar.CVAR_ARCHIVE);
            CVars.sv_master[3] = Engine.cvarManager.Cvar_Get("sv_master4", "", idCVar.CVAR_ARCHIVE);
            CVars.sv_master[4] = Engine.cvarManager.Cvar_Get("sv_master5", "", idCVar.CVAR_ARCHIVE);
            CVars.sv_reconnectlimit = Engine.cvarManager.Cvar_Get("sv_reconnectlimit", "3", 0);
            CVars.sv_showloss = Engine.cvarManager.Cvar_Get("sv_showloss", "0", 0);
            CVars.sv_padPackets = Engine.cvarManager.Cvar_Get("sv_padPackets", "0", 0);
            CVars.sv_killserver = Engine.cvarManager.Cvar_Get("sv_killserver", "0", 0);
            CVars.sv_mapChecksum = Engine.cvarManager.Cvar_Get("sv_mapChecksum", "", idCVar.CVAR_ROM);

            CVars.sv_reloading = Engine.cvarManager.Cvar_Get("g_reloading", "0", idCVar.CVAR_ROM);   //----(SA)	added

            AddOperatorCommands();
        }

        //
        // SpawnServer
        //
        public void SpawnServer()
        {
            ((idCommonLocal)Engine.common).BeginClientMapLoading();
        }

        //
        // Command_Map
        //
        public void Command_Map()
        {
            SpawnServer();
        }

        //
        // AddOperatorCommands
        //
        public void AddOperatorCommands()
        {
            Engine.cmdSystem.Cmd_AddCommand("spmap", Command_Map);
#if !ID_DEMO_BUILD
            Engine.cmdSystem.Cmd_AddCommand("map", Command_Map);
            Engine.cmdSystem.Cmd_AddCommand("devmap", Command_Map);
            Engine.cmdSystem.Cmd_AddCommand("spdevmap", Command_Map);
#endif
        }

        public void Frame()
        {

        }
    }
}
