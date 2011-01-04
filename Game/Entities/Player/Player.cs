﻿/*
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

// Player.cs (c) 2010 JV Software
//

using idLib;
using idLib.Game;
using idLib.Engine.Public;
using idLib.Engine.Public.Net;

using Game.Physics;

namespace Game.Entities.Player
{
    //
    // idPlayer
    //
    public class idPlayer : idEntity
    {
        idPlayerPhysics physics;
        idPhysicsPlayerState physicsState = new idPhysicsPlayerState();

        string profilename;
        bool clientSpawned = false;

        int cameraframe = 0;

        public bool isActive
        {
            get
            {
                return clientSpawned;
            }
        }

        //
        // Spawn
        //
        public override void Spawn()
        {
            //state.modelindex = Level.net.ModelIndex(model);
            //state.modelindex2 = Level.net.ModelIndex(model2);

            //state.modelSkin = Level.net.SkinIndex(aiSkin);
            //state.modelSkin2 = Level.net.SkinIndex(aihSkin);

            profilename = spawnArgs.FindKey("name");

            state.eType = entityType_t.ET_PLAYER;

            physics = new idPlayerPhysics();

            InitAnim();
        }

        //
        // EnterWorld
        //
        public override void EnterWorld()
        {
            Engine.common.Printf(profilename + " has entered the world.\n");
            clientSpawned = true;

            // Run a script for this player if there is one.
            idScriptAction playerStartScript = Level.aiscript.FindAction("player", "playerstart", true);

            if (playerStartScript != null)
            {
                Level.aiscript.Execute(this, playerStartScript);
            }
        }

        //
        // Frame
        //
        public override void Frame()
        {
            float fov = 0;
            physicsState.cmd = Engine.common.GetUserCmdForClient(state.number);
            physicsState.ps = state;

            if (Level.cameranum != -1)
            {
                if (Level.camerapath.Length > 0)
                {
                    idCameraManager.loadCamera(0, Level.camerapath);
                    Level.camerapath = "";
                    cameraframe = 0;
                }
                idCameraManager.getCameraInfo(Level.cameranum, cameraframe++, ref state.origin, ref state.angles2, ref fov);
                state.angles2.X = -state.angles2.X;
            }
            else
            {
                physics.Move(ref physicsState);
            }

            LinkEntity();
        }
    }
}
