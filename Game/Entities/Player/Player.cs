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
        int bodymodel = 0;
        int headmodel = 0;

        int bodyskin = 0;
        int headskin = 0;

        idPlayerPhysics physics;
        idPhysicsPlayerState physicsState = new idPhysicsPlayerState();

        string profilename;

        //
        // Spawn
        //
        public override void Spawn()
        {
            bodymodel = Level.net.ModelIndex(model);
            headmodel = Level.net.ModelIndex(model2);

            bodyskin = Level.net.SkinIndex(aiSkin);
            headskin = Level.net.SkinIndex(aihSkin);

            profilename = spawnArgs.FindKey("name");

            state.eType = entityType_t.ET_PLAYER;

            physics = new idPlayerPhysics();
        }

        //
        // EnterWorld
        //
        public override void EnterWorld()
        {
            Engine.common.Printf(profilename + " has entered the world.\n");
        }

        //
        // Frame
        //
        public override void Frame()
        {
            physicsState.cmd = Engine.common.GetUserCmdForClient(state.number);
            physicsState.ps = state;

            physics.Move(ref physicsState);

            LinkEntity();
        }
    }
}
