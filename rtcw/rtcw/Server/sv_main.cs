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
using idLib.Engine.Public.Net;
using idLib.Game.Server;
using idLib.Game;
using idLib;
using rtcw.Framework;

namespace rtcw.Server
{
    //
    // idSharedEntity
    //
    class idSharedEntity
    {
        public entityShared_t shared = new entityShared_t();
        public entityState_t state = new entityState_t();
    }

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

        public static idCVar sv_running;
        public static idCVar sv_reloading;  //----(SA)	added
    }

    static class Globals
    {
        public static bool killBots = false;
        public static bool cheats = false;
        public static idServerManager sv;
        public static idSysModule gvm;
        public static idGamePublic game;
        public static idWorld world;

        public static int numSnapshotEntities = 0;
        public const int MAX_CONFIGSTRINGS = 2048;
        public static string[] configstrings = new string[MAX_CONFIGSTRINGS];

        public static entityState_t[] snapshotEntities;
        public static int nextSnapshotEntities = 0;
        public static int snapFlagServerBit = 0;

        public static idServerClient[] clients = new idServerClient[idGamePublic.MAX_CLIENTS];

        public static idSharedEntity[] gentities;
        public static int[] linkedEntities = new int[idGamePublic.MAX_GENTITIES];
        public static int numLinkedEntities = 0;
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
            Globals.sv = this;

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
            CVars.sv_running = Engine.cvarManager.Cvar_Get("sv_running", "0", idCVar.CVAR_ROM);

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
        // InitGameVM
        //
        private void InitGameVM(string mapname)
        {
            // Load the vm.
            Globals.gvm = Engine.Sys.LoadDLL("game");

            // Allocate the game interface from the vm.
            Globals.game = Globals.gvm.AllocClass<idGamePublic>("Game.idGameLocal");

            // Init game.
            Globals.game.Init(mapname, 0, 0, 0);
        }

        //
        // RegisterEntity
        //
        public void RegisterEntity(int entityNum, out idLib.Game.entityShared_t shared, out idLib.Game.entityState_t state)
        {
            if (Globals.gentities[entityNum] != null)
            {
                Engine.common.ErrorFatal("Entity %d already registered...\n", entityNum);
            }

            Globals.gentities[entityNum] = new idSharedEntity();

            shared = Globals.gentities[entityNum].shared;
            state = Globals.gentities[entityNum].state;
        }

        //
        // ShutdownGameProgs
        //
        private void ShutdownGameProgs()
        {
            if (Globals.gvm == null)
            {
                return;
            }

           // Globals.clients = null;

            Globals.game.Shutdown(false);
            Globals.snapshotEntities = null;

            Engine.common.ForceGCCollect();
        }

        //
        // ClearServer
        //
        private void ClearServer() {
	        int i;

	        for ( i = 0 ; i < Globals.MAX_CONFIGSTRINGS ; i++ ) {
                Globals.configstrings[i] = "";
	        }
        }

        
        /*
        ===============
        SV_Startup

        Called when a host starts a map when it wasn't running
        one before.  Successive map or map_restart commands will
        NOT cause this to be called, unless the game is exited to
        the menu system first.
        ===============
        */
        private void Startup()
        {
            if ( CVars.sv_running.GetValueInteger() != 0 ) {
		        Engine.common.ErrorFatal( "SV_Startup: svs.initialized" );
	        }
	        //SV_BoundMaxClients( 1 );

            Globals.clients = new idServerClient[CVars.sv_maxclients.GetValueInteger()];

            //	SV_InitReliableCommands( svs.clients );	// RF

            // jv - we won't support dedicated servers.
	       // if ( com_dedicated->integer ) {
		   //     Globals.numSnapshotEntities = sv_maxclients->integer * PACKET_BACKUP * 64;
	       // } else {
		        // we don't need nearly as many when playing locally
                Globals.numSnapshotEntities = CVars.sv_maxclients.GetValueInteger() * 4 * 64;
	       // }
           // jv end

            CVars.sv_running.SetValueInt(1);
        }

        /*
        ====================
        SV_SetExpectedHunkUsage

          Sets com_expectedhunkusage, so the client knows how to draw the percentage bar
        ====================
        */
        private void SetExpectedHunkUsage(string mapname)
        {
            int handle;
            string token;
            int len;

            idFile hunkFile = Engine.fileSystem.OpenFileRead("hunkusage.dat", true); 
            if (hunkFile != null)
            { // the file exists, so read it in, strip out the current entry for this map, and save it out, so we can append the new value
                idParser parser = new idParser(hunkFile);

                // now parse the file, filtering out the current map
                while (parser.ReachedEndOfBuffer == false)
                {
                    token = parser.NextToken;

                    if (token == null || token.Length <= 0)
                        break;

                    if (token == mapname)
                    {
                        // found a match
                        token = parser.NextToken;
                        if (token != null || token.Length > 0)
                        {
                            // this is the usage
                            idCVar usage = Engine.cvarManager.Cvar_Set("com_expectedhunkusage", token, true);
                            usage.SetValueInt(int.Parse(token) + Engine.fileSystem.FS_LoadStack());
                            parser.Dispose();
                            Engine.fileSystem.CloseFile(ref hunkFile);
                            return;
                        }
                    }
                }

                parser.Dispose();
                Engine.fileSystem.CloseFile(ref hunkFile);
            }

            // just set it to a negative number,so the cgame knows not to draw the percent bar
            Engine.cvarManager.Cvar_Set("com_expectedhunkusage", "-1", true);
        }

        //
        // SpawnServer
        //
        public void SpawnServer(string mapname, bool killBots)
        {
            // Init the live server.
            Engine.net.CreateServer(CVars.sv_maxclients.GetValueInteger());

            // Shutdown the game if its already running.
            ShutdownGameProgs();

            Engine.common.Printf("------ Server Initialization ------\n");
            Engine.common.Printf("Server: %s\n", mapname);

            CVars.sv_mapname.SetValue(mapname, true);

            // if not running a dedicated server CL_MapLoading will connect the client to the server
            // also print some status stuff 

            // jv - this also shutsdown and clears cgame.
            ((idCommonLocal)Engine.common).BeginClientMapLoading();

            ClearServer();

            if (CVars.sv_running.GetValueInteger() != 0)
            {
                Startup();
            }
            else
            {
                /*
                // check for maxclients change
                if (sv_maxclients->modified)
                {
                    SV_ChangeMaxClients();
                }
                */
            }

            // allocate the snapshot entities on the hunk
            Globals.snapshotEntities = new entityState_t[Globals.numSnapshotEntities];
            Globals.nextSnapshotEntities = 0;

            // toggle the server bit so clients can detect that a
            // server has changed
            Globals.snapFlagServerBit ^= idSnapshotType.SNAPFLAG_SERVERCOUNT;

            // set nextmap to the same map, but it may be overriden
            // by the game startup or another console command
            Engine.cvarManager.Cvar_Set("nextmap", "map_restart 0", true);
            //	Cvar_Set( "nextmap", va("map %s", server) );

            SetExpectedHunkUsage("maps/" + mapname + ".bsp");

            Globals.gentities = new idSharedEntity[idGamePublic.MAX_GENTITIES];

            InitGameVM(mapname);
        }

        

        //
        // AddOperatorCommands
        //
        public void AddOperatorCommands()
        {
            Engine.cmdSystem.Cmd_AddCommand("spmap", idServerConsoleCommands.Command_Map);
#if !ID_DEMO_BUILD
            Engine.cmdSystem.Cmd_AddCommand("map", idServerConsoleCommands.Command_Map);
            Engine.cmdSystem.Cmd_AddCommand("devmap", idServerConsoleCommands.Command_Map);
            Engine.cmdSystem.Cmd_AddCommand("spdevmap", idServerConsoleCommands.Command_Map);
#endif
        }

        //
        // GetChallenge
        //
        private void GetChallenge(idNetAdress from)
        {
            // If this is a loopback client, connect immediatly.
            if (from.GetType() == idNetAddressType.NA_LOOPBACK)
            {
                idMsgWriter msg = new idMsgWriter(idNetwork.netcmd_serverinfo.Length + 4 + CVars.sv_mapname.GetValue().Length + 4);

                msg.WriteString(idNetwork.netcmd_serverinfo);
                msg.WriteString(CVars.sv_mapname.GetValue());

                Engine.net.SendReliablePacketToAddress(idNetSource.NS_CLIENT, from, ref msg);

                msg.Dispose();
                return;
            }
        }

        //
        // LinkEntity
        //
        public void LinkEntity(int entityNum, entityState_t state, entityShared_t shared)
        {
            Globals.gentities[entityNum].state = state;
            Globals.gentities[entityNum].shared = shared;
            Globals.linkedEntities[Globals.numLinkedEntities++] = entityNum;
        }

        //
        // GetUsercmdForClient
        //
        public idUsercmd GetUsercmdForClient(int clientNum)
        {
            return Globals.clients[clientNum].cmd;
        }

        //
        // PacketEvent
        //
        public void PacketEvent(idNetAdress from, ref idMsgReader buf)
        {
            string cmd;

            cmd = buf.ReadString();

            if (cmd == idNetwork.netcmd_getchallenge)
            {
                GetChallenge(from);
            }
            else if (cmd == idNetwork.netcmd_getconfigmsg)
            {
                idMsgWriter msg;
                string configstr;

                // Have the client connect first so the config string is properly updated.
                Globals.game.ClientConnect(0, buf.ReadString(), true, false);

                configstr = Globals.game.GetConfigString();
                msg = new idMsgWriter(configstr.Length + 4 + configstr.Length + 4);
                msg.WriteString(idNetwork.netcmd_sendconfigmsg);
                msg.WriteString(configstr);

                Globals.clients[0] = new idServerClient();

                Engine.net.SendReliablePacketToAddress(idNetSource.NS_CLIENT, Engine.net.GetLoopBackAddress(), ref msg);
            }
            else if (cmd == idNetwork.netcmd_enterworldmsg)
            {
                Globals.clients[0].clientIsReady = true;
                Globals.game.ClientBegin(0);

                // The client should have the level loaded by now so just get a copy for ourselves.
                if (Globals.world == null)
                {
                    Globals.world = Engine.RenderSystem.LoadWorld(CVars.sv_mapname.GetValue());
                }
            }
            else if (cmd == idNetwork.netcmd_usercmd)
            {
                int clientNum = buf.ReadInt();

                Globals.clients[clientNum].cmd.ReadPacket(ref buf);
            }
            else
            {
                Engine.common.Warning("SV_PacketEvent: Unknown packet command recieved from " + from.GetAddress() + " " + cmd + "\n");
            }
        }

        //
        // Frame
        //
        public void Frame(int frameTime)
        {
            if (Globals.game == null)
            {
                return;
            }
            // Reset the linked entities.
            Globals.numLinkedEntities = 0;

            // Run the game frame.
            Globals.game.Frame(frameTime);

            if (Globals.numLinkedEntities <= 0)
            {
                return;
            }

            if (Globals.clients[0] != null && Globals.clients[0].clientIsReady == false)
            {
                return;
            }

            // Loop through the linked entities and only send down the ones that are in each players view.
#if false
            idMsgWriter msg = new idMsgWriter(Globals.numLinkedEntities * entityState_t.NET_SIZE + idNetwork.netcmd_snapshot.Length + 4 + 4);
            msg.WriteString(idNetwork.netcmd_snapshot);
            msg.WriteInt(Globals.numLinkedEntities);
#else
            if (idCommonLocal.cl.cls.state == connstate_t.CA_LOADING)
            {
                idCommonLocal.cl.cls.state = connstate_t.CA_ACTIVE;
            }

            Globals.clients[0].cmd = Engine.usercmd.GetCurrentCommand();
#endif
            for (int i = 0; i < Globals.numLinkedEntities; i++)
            {
                entityShared_t shared = Globals.gentities[Globals.linkedEntities[i]].shared;
                entityState_t ent = Globals.gentities[Globals.linkedEntities[i]].state;

                if (shared.bounds != null)
                {
                    if (!Globals.world.isBoundsInPVS(Globals.gentities[0].state.origin, ent.origin, shared.bounds) && i > 0)
                    {
                        continue;
                    }
                }
                else
                {
                    if (!Globals.world.isPointInPVS(Globals.gentities[0].state.origin, ent.origin) && i > 0)
                    {
                        continue;
                    }
                }

#if false
                ent.WritePacket(ref msg);
#else
                idCommonLocal.cl.cls.cgame.NetworkRecieveSnapshot( ref ent );
#endif
            }
#if false
            Engine.net.SendReliablePacketToAddress(idNetSource.NS_CLIENT, Engine.net.GetLoopBackAddress(), ref msg);

            msg.Dispose();
#endif
        }
    }
}
