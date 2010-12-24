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

// game_spawner.cs (c) 2010 JV Software
//

using System;
using idLib;
using idLib.Math;
using idLib.Engine.Public;
using idLib.Game.Server;

using Game.Entities.Info;
using Game.Entities.Player;
using Game.Entities.Func;
using Game.Entities.Misc;
using Game.Entities.Props;
using Game.AI;

namespace Game
{
    //
    // idGameSpawnDefs
    //
    struct idGameSpawnDefs
    {
        public delegate idEntity func_t();

        public string funcname;
        public func_t func;

        public idGameSpawnDefs( string funcname, func_t func )
        {
            this.funcname = funcname;
            this.func = func;
        }
    };

    //
    // idGameSpawner
    //
    class idGameSpawner
    {
        private idGameSpawnDefs[] spawnDefs = new idGameSpawnDefs[] {
            new idGameSpawnDefs( "info_player_start", () => new idEntityPlayerStart() ),
            new idGameSpawnDefs( "player", () => new idPlayer() ),

            new idGameSpawnDefs( "func_door_rotating", () => new idEntityFuncDoor() ),

            new idGameSpawnDefs( "misc_gamemodel", () => new idEntityGameModel() ),
            new idGameSpawnDefs( "props_decoration", () => new idEntityPropDecoration() ),

            new idGameSpawnDefs( "ai_civilian", () => new idEntityAI() ),

            // Ignored entities.
            new idGameSpawnDefs( "misc_model", () => null ), // misc models are ignored in rtcw.
            new idGameSpawnDefs( "light", () => null ), // lights are ignored in rtcw
            new idGameSpawnDefs( "lightJunior", () => null ) // lights are ignored in rtcw
        };
        //
        // RegisterEntity
        //
        private idEntity RegisterEntity(idEntity entity, idDict dict)
        {
            int i, force;
            int basepos = idGamePublic.MAX_CLIENTS;
            int endpos = idGamePublic.MAX_GENTITIES;

            if (dict.FindKey("classname") == "player")
            {
                basepos = 0;
                endpos = idGamePublic.MAX_CLIENTS;

                Level.num_clients++;
            }
            else
            {
                Level.num_entities++;
            }

	        i = 0;      // shut up warning
	        for ( force = 0 ; force < 2 ; force++ ) {
		        // if we go through all entities and can't find one to free,
		        // override the normal minimum times before use
                for (i = basepos; i < endpos; i++)
                {
                    idEntity e = Level.entities[i];
			        if ( e != null ) {
				        continue;
			        }

			        // the first couple seconds of server time can involve a lot of
			        // freeing and allocating, so relax the replacement policy
			        //if ( !force && e->freetime > level.startTime + 2000 && level.time - e->freetime < 1000 ) {
				    //    continue;
			        //}

                    dict.AddKey("entitynum", i);
                    CallEntitySpawn(ref entity, dict);
			        Level.entities[i] = entity;

                    

                    return Level.entities[i];
		        }
		        if ( i != idGamePublic.MAX_GENTITIES ) {
			        break;
		        }
	        }

            Engine.common.ErrorFatal("No open entity slots...\n");
            return null;
        }

        //
        // FindSpawnPoint
        //
        public bool FindSpawnPoint( out idVector3 org )
        {
            for (int i = idGamePublic.MAX_CLIENTS; i < idGamePublic.MAX_GENTITIES; i++)
            {
                if (Level.entities[i].classname == "info_player_start")
                {
                    org = Level.entities[i].state.origin;
                    return true;
                }
            }

            org = idVector3.vector_origin;
            return false;
        }

        //
        // CallEntitySpawn
        //
        private void CallEntitySpawn(ref idEntity entity, idDict dict)
        {
            // Register the entity with the server code.
            Engine.common.ServerRegisterEntity(dict.FindKeyInt("entitynum"), out entity.shared, out entity.state);

            entity.spawnArgs = dict;
            entity.ParseFieldsFromSpawnArgs();

            entity.Spawn();
        }

        //
        // AllocEntityByClassname
        //
        private idEntity AllocEntityByClassname(string classname)
        {
            for (int i = 0; i < spawnDefs.Length; i++)
            {
                if (spawnDefs[i].funcname == classname)
                {
                    return spawnDefs[i].func();
                }
            }
            return null;
        }

        //
        // SpawnEntity
        //
        public idEntity SpawnEntity(idDict dict)
        {
            string classname;
            idEntity entity;

            // Find the entities classname.
            classname = dict.FindKey("classname");
            if (classname == null || classname.Length <= 0)
            {
                Engine.common.ErrorFatal("SpawnEntity: Null classname\n");
            }

            entity = AllocEntityByClassname(classname);
            if (entity == null)
            {
                Engine.common.Warning("SpawnEntity: Failed to spawn entity " + classname + "\n");
                return null;
            }

            return RegisterEntity(entity, dict);
        }

        //
        // BspStringToEntityDict
        //
        private void BspStringToEntityDict(ref idParser parser, ref idDict dict)
        {
            while (true)
            {
                string token = parser.NextToken;

                if (token == null || token.Length <= 0)
                {
                    Engine.common.ErrorFatal("BspStringToEntityDict: Unexpected EOF inside of entity string\n");
                    return;
                }

                if (token == "}")
                {
                    break;
                }

                string val = parser.NextToken;
                dict.AddKey(token, val);
            }
        }

        //
        // SpawnEntitiesFromBsp
        //
        public void SpawnEntitiesFromBsp(string mappath)
        {
            idParser parser;
            string bspEntityString;

            bspEntityString = Engine.RenderSystem.LoadWorldEntityString(mappath);
            if (bspEntityString == null || bspEntityString.Length <= 0)
            {
                Engine.common.ErrorFatal("Bsp Entity String null.\n");
            }

            Engine.common.Printf("Spawning Entities...\n");

            parser = new idParser(bspEntityString);

            while (parser.ReachedEndOfBuffer == false)
            {
                string token = parser.NextToken;

                if( token == null || token.Length <= 0)
                    break;

                if (token == "{")
                {
                    idDict dict = new idDict();
                    BspStringToEntityDict(ref parser, ref dict);
                    SpawnEntity(dict);
                }
                else
                {
                    Engine.common.ErrorFatal("SpawnEntitiesFromBsp: Unknown or unepxected token " + token + "\n");
                }
            }
        }
    }
}
