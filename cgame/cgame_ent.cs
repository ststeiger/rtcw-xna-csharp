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

// cgame_ent.cs (c) 2010 JV Software
//

using idLib;
using idLib.Engine.Public;
using idLib.Game.Client;
using idLib.Game;
using idLib.Engine.Public.Net;

namespace cgame
{
    //
    // idClientEntity
    //
    static class EntitySnapshot
    {
        //
        // GeneralEntity
        //
        public static void GeneralEntity(ref entityState_t entity)
        {
            idRenderEntity ent = Globals.world.AllocRenderEntity(ref Globals.localview.refdef);

            ent.frame = 0;
            ent.oldframe = 0;
            ent.origin = entity.origin;
            ent.axis = entity.angles.ToAxis();
            ent.hModel = Globals.models[entity.modelindex];
        }

        //
        // PlayerEntiy
        //
        public static void PlayerEntiy(ref entityState_t entity)
        {
            if (entity.eType == entityType_t.ET_PLAYER)
            {
                if (entity.number == Globals.localViewEntity)
                {
                    Globals.localview.SetViewOrigin(entity.origin);
                    Globals.localview.SetViewAngle(entity.angles2);
                    Globals.viewPacketRecv = true;
                }
            }
        }
    }
}
