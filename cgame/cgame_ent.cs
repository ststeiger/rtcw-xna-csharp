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
using idLib.Math;
using idLib.Game;
using idLib.Engine.Public.Net;

namespace cgame
{
    //
    // idClientEntity
    //
    static class EntitySnapshot
    {
        /*
        ======================
        CG_PositionRotatedEntityOnTag

        Modifies the entities position and axis by the given
        tag location
        ======================
        */
        private static void PositionRotatedEntityOnTag( ref idRenderEntity entity, idRenderEntity parent, string tagName ) {
	        int i;
	        idOrientation lerped;
	        idMatrix tempAxis = idMatrix.IdenityMatrix;

	        // lerp the tag
            lerped.origin = idVector3.vector_origin;
            lerped.axis = idMatrix.IdenityMatrix;
            parent.hModel.GetTag(tagName, parent.frame, parent.torsoFrame, 0, ref lerped);
	        
	        // FIXME: allow origin offsets along tag?
            entity.origin = parent.origin;
	        for ( i = 0 ; i < 3 ; i++ ) {
                entity.origin = entity.origin + lerped.origin[i] * parent.axis[i];
	        }
	        
            tempAxis.AxisMultiply(entity.axis, lerped.axis);
            entity.axis.AxisMultiply(tempAxis, parent.axis);
        }

        //
        // GeneralEntity
        //
        public static void GeneralEntity(ref entityState_t entity)
        {
            idRenderEntity ent;
            
            if(Globals.world == null)
                return;
            
            ent = Globals.world.AllocRenderEntity(ref Globals.localview.refdef);


            ent.frame = entity.frame;
            ent.oldframe = 0;
            ent.origin = entity.origin;
            ent.axis = entity.angles.ToAxis();
            if (entity.modelSkin >= 0)
            {
                ent.customSkin = Globals.skins[entity.modelindex2];
            }
            ent.hModel = Globals.models[entity.modelindex];
        }

        //
        // CreatePlayerTorso
        //
        private static void CreatePlayerTorso(ref entityState_t entity, out idRenderEntity ent)
        {
            if (Globals.world == null)
            {
                ent = null;
                return;
            }

            if (Globals.skins[entity.modelSkin] == null)
            {
                ent = null;
                return;
            }

            ent = Globals.world.AllocRenderEntity(ref Globals.localview.refdef);

            ent.frame = 0;
            ent.oldframe = 0;
            ent.origin = entity.origin;
            ent.axis = entity.angles.ToAxis();
            ent.customSkin = Globals.skins[entity.modelSkin];
            ent.hModel = Globals.models[entity.modelindex];
        }

        //
        // CreatePlayerHead
        //
        private static void CreatePlayerHead(ref entityState_t entity, idRenderEntity torso)
        {
            idRenderEntity ent;

            if (Globals.world == null)
                return;

            if (Globals.skins[entity.modelSkin2] == null)
                return;

            ent = Globals.world.AllocRenderEntity(ref Globals.localview.refdef);

            ent.frame = 0;
            ent.oldframe = 0;
            ent.axis = idMatrix.IdenityMatrix;

            PositionRotatedEntityOnTag(ref ent, torso, "tag_head");

            ent.customSkin = Globals.skins[entity.modelSkin2];
            ent.hModel = Globals.models[entity.modelindex2];
        }

        //
        // PlayerEntiy
        //
        public static void PlayerEntiy(ref entityState_t entity)
        {
            idRenderEntity torso;

            if (entity.number == Globals.localViewEntity)
            {
                Globals.localview.SetViewOrigin(entity.origin);
                Globals.localview.SetViewAngle(entity.angles2);
                Globals.viewPacketRecv = true;
                return;
            }

            CreatePlayerTorso(ref entity, out torso);
            if (torso != null)
            {
                CreatePlayerHead(ref entity, torso);
            }
        }
    }
}
