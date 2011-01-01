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

// Entity.cs (c) 2010 JV Software
//

using idLib;
using idLib.Math;
using idLib.Game;
using idLib.Engine.Public;

namespace Game
{
    //
    // idEntity
    //
    public abstract class idEntity
    {
        public entityState_t state;   // communicated by server to clients
        public entityShared_t shared; // shared by both the server system and game
        
        public idDict spawnArgs;
        public string classname;
        public string model;
        public string model2;

        public int spawnFlags;
        public float speed;
        public float closespeed;

        public string target;
        public string targetname;
        public string targetdeath;

        public string message;
        public string popup;
        public string book;

        public string team;

        public float wait;
        public float random;

        public int count;
        public int health;
        public int light;
        public int damage;
        public int dmg;

        public float duration;
        public idVector3 rotate;

        public string aiAttirbutes;
        public string aiName;
        public int aiTeam;

        public string aiSkin;
        public string aihSkin;

        public idVector3 dl_color;
        public string dl_stylestring;
        public string dl_shader;

        public int key;
        public float harc;
        public float varc;
        public float delay;
        public int radius;
        public int start_size;
        public int end_size;

        public string spawnitem;
        public string track;
        public string scriptName;

        public idModel hModel;

        //
        // SplitModelSkinString
        //
        private void SplitModelSkinString(string input, ref string model, ref string skin)
        {
            if (input.Contains("/") == false)
            {
                model = input;
                skin = input;
                return;
            }

            model = "";
            skin = "";

            for (int i = 0, pos = 0; i < input.Length; i++, pos++)
            {
                if (input[i] == '/')
                {
                    pos = 0;
                    continue;
                }

                if (i > pos)
                {
                    skin += input[i];
                }
                else
                {
                    model += input[i];
                }
            }
        }

        //
        // ParseFieldsFromSpawnArgs
        //
        public void ParseFieldsFromSpawnArgs()
        {
            // jv - i was thinking as I was almost done with this, this is the wrong way to do this,
            // ill fix it later if it poses a problem.

            classname = spawnArgs.FindKey( "classname" );
            state.origin = spawnArgs.FindKeyidVector3( "origin" );
            model = spawnArgs.FindKey( "model" );
            model2 = spawnArgs.FindKey( "model2" );
            spawnFlags = spawnArgs.FindKeyInt( "spawnflags" );
            speed = spawnArgs.FindKeyFloat( "speed" );
            closespeed = spawnArgs.FindKeyFloat("closespeed" );

            target = spawnArgs.FindKey( "target" );
            targetname = spawnArgs.FindKey("targetname");
            targetdeath = spawnArgs.FindKey("targetdeath");
            message = spawnArgs.FindKey("message");
            popup = spawnArgs.FindKey("popup");
            book = spawnArgs.FindKey("book");
	
	        team = spawnArgs.FindKey("team" );
            wait = spawnArgs.FindKeyFloat("wait");
            random = spawnArgs.FindKeyFloat("random");
            team = spawnArgs.FindKey("team");
            wait = spawnArgs.FindKeyFloat("wait");
            random = spawnArgs.FindKeyFloat("random");
            count = spawnArgs.FindKeyInt("count");
            health = spawnArgs.FindKeyInt("health");
            light = spawnArgs.FindKeyInt("light");
            dmg = spawnArgs.FindKeyInt("count");
            state.angles = spawnArgs.FindKeyidVector3("angles");
            state.angles.Y = spawnArgs.FindKeyFloat("angle");

            duration = spawnArgs.FindKeyFloat("duration");
            rotate = spawnArgs.FindKeyidVector3("rotate");
            if (state.angles.Y == 0)
            {
                state.angles.Y = spawnArgs.FindKeyFloat("degrees");
            }
            speed = spawnArgs.FindKeyFloat("time");

            aiAttirbutes = spawnArgs.FindKey("aiattributes");
            aiName = spawnArgs.FindKey("ainame");

            aiTeam = spawnArgs.FindKeyInt("aiteam");

            aiSkin = spawnArgs.FindKey("skin");
            aihSkin = spawnArgs.FindKey("head");

            // jv - probably not the best way to do this.
            if (aiSkin != null && aiSkin.Length > 0 && (model == null || model.Length <= 0))
            {
                SplitModelSkinString(aiSkin, ref model, ref aiSkin);

                if(model == aiSkin)
                    aiSkin = "default";

                aihSkin = "models/players/" + model + "/head_" + aiSkin + ".skin";
                aiSkin = "models/players/" + model + "/body_" + aiSkin + ".skin";

                if (model2 == null || model2.Length <= 0)
                {
                    model2 = "models/players/" + model + "/head.mds";
                }

                model = "models/players/" + model + "/body.mds";
            }

            dl_color = spawnArgs.FindKeyidVector3( "_color" );	    
            if(dl_color.X == 0 && dl_color.Y == 0 && dl_color.Z == 0)
            {
                dl_color = spawnArgs.FindKeyidVector3( "color" );	    
            }
            dl_stylestring = spawnArgs.FindKey("stylestring");
            dl_shader = spawnArgs.FindKey("shader");

            key = spawnArgs.FindKeyInt("key");
            if(state.frame == 0)
            {
                state.frame = spawnArgs.FindKeyInt("stand");
            }

            harc = spawnArgs.FindKeyFloat("harc");
            varc = spawnArgs.FindKeyFloat("varc");

            delay = spawnArgs.FindKeyFloat("delay");
            if(radius == 0)
            {
                radius = spawnArgs.FindKeyInt("radius");
            }

            start_size = spawnArgs.FindKeyInt("start_size");
            end_size = spawnArgs.FindKeyInt("end_size");

            if(count == 0)
            {
                count = spawnArgs.FindKeyInt("shard");
            }

            spawnitem = spawnArgs.FindKey("spawnitem");
            track = spawnArgs.FindKey("track");
            scriptName = spawnArgs.FindKey("scriptName");

            state.number = spawnArgs.FindKeyInt("entitynum");
            state.modelindex2 = -3;
        }

        //
        // EnterWorld
        //
        public virtual void EnterWorld()
        {

        }

        //
        // LinkEntity
        //
        public void LinkEntity()
        {
            Engine.common.LinkEntity(state.number);
        }

        // Spawn
        public abstract void Spawn();

        // Frame
        public abstract void Frame();
    }
}
