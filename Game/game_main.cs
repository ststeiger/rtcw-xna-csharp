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

// game_main.cs (c) 2010 JV Software
//

using idLib;
using idLib.Math;
using idLib.Engine.Public;
using idLib.Engine.Public.Net;
using idLib.Game.Server;

using Game.AAS;
using Game.Entities.Player;

namespace Game
{
    //
    // idGameLocal
    //
    public class idGameLocal : idGamePublic
    {
        //
        // idGameLocal
        //
        public idGameLocal(int version)
        {
            Engine.common.Printf("Game module loaded...\n");

            // Register the cvars.
            Cvars.RegisterCvars();
        }

        //
        // LoadAASForWorld
        //
        private void LoadAASForWorld(string mapname)
        {
            // Load in the AAS file.
            Level.aas.LoadAASForWorld(mapname);
        }

        //
        // Init
        //
        public override void Init(string mapname, int levelTime, int randomSeed, int restart)
        {
            Engine.common.Printf("------- Game Initialization -------\n");
            Engine.common.Printf("gamename: %s\n", Level.GAMEVERSION);

            // Reset the game network manager.
            Level.net.Reset();

            // Init the AAS manager.
            Level.aas = new idAAS();

            // Load in the AAS data for the world.
            LoadAASForWorld(mapname);

            Level.mapname = mapname;

            // Allocate the spawn, and spawn the 
            Level.spawner = new idGameSpawner();
            Level.spawner.SpawnEntitiesFromBsp(mapname);
        }

        //
        // ClientBegin
        //
        public override void ClientBegin(int clientNum)
        {
            if (Level.world == null)
            {
                Level.world = Engine.RenderSystem.LoadWorld(Level.mapname);
            }
            Level.entities[clientNum].EnterWorld();
        }

        //
        // ClientConnect
        //
        public override void ClientConnect(int clientNum, string playername, bool firstTime, bool isBot)
        {
            idDict dict = new idDict();
            idVector3 spawnpoint;

            dict.AddKey("classname", "player");
            dict.AddKey("clientnum", clientNum);

            // Set the body model/skin
            dict.AddKey("skin", "models/players/bj2/head_default.skin");
            dict.AddKey("model", "models/players/bj2/body.mds");
            
            // Set the head model/skin.
            dict.AddKey("head", "models/players/bj2/head_default.skin");
            dict.AddKey("model2", "models/players/bj2/head.mdc");

            dict.AddKey("name", playername);

            // Try to find a valid spawn point for the client.
            if (Level.spawner.FindSpawnPoint(out spawnpoint) == false)
            {
                Engine.common.ErrorFatal("Failed to find spawn point or an info_player_start for client...\n");
                return;
            }

            // Set the spawn point for the entity.
            dict.AddKey("origin", spawnpoint.ToString());

            Level.spawner.SpawnEntity(dict);
        }

        //
        // GetConfigString
        //
        public override string GetConfigString()
        {
            // jv - this is a hack but getconfigstring should only be called when a new client,
            // is connecting so assume numclients - 1 is the handle to the local client.
            return Level.net.ConfigStr + " localClient " + (Level.num_clients - 1) + " ";
        }

        //
        // Shutdown
        //
        public override void Shutdown(bool restart)
        {
            
        }

        //
        // Frame
        //
        public override void Frame(int frameTime)
        {
            // Don't run any frames unless we have one client thats in the world.
            if (Level.entities[0] == null || ((idPlayer)Level.entities[0]).isActive == false)
            {
                return;
            }

            Level.time += frameTime;

            // Run any scripts that are running.
            if (Level.script != null)
            {
                Level.script.Think();
            }

            if (Level.aiscript != null)
            {
                Level.aiscript.Think();
            }

            // Run the clients.
            for (int i = 0; i < Level.num_clients; i++)
            {
                Level.entities[i].Frame();
            }

            for (int i = idGamePublic.MAX_CLIENTS; i < idGamePublic.MAX_CLIENTS + Level.num_entities; i++)
            {
                Level.entities[i].Frame();
            }
        }
    }
}
