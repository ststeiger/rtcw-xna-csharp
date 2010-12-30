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

// World.cs (c) 2010 JV Software
//

using System;
using System.Collections.Generic;
using idLib.Engine.Public;
using rtcw.Renderer.Backend;
using rtcw.Renderer.Models;
using rtcw.Renderer.Map;

namespace rtcw.Renderer
{
    //
    // idWorldLocal
    //
    class idWorldLocal : idWorld
    {
        idMap map = null;

        //
        // idWorldLocal
        //
        public idWorldLocal()
        {

        }

        //
        // idWorldLocal
        //
        public idWorldLocal(string mappath)
        {
            map = new idMap();
            map.LoadMap(mappath);
        }

        //
        // LoadBrushModel
        //
        public override idModel LoadBrushModel(string name)
        {
            for (int i = 0; i < map.bmodels.Length; i++)
            {
                if (map.bmodels[i].GetName() == name)
                {
                    return map.bmodels[i];
                }
            }

            Engine.common.ErrorFatal("R_LoadBrushModel: Failed to find brush model " + name + "\n");

            return null;
        }

        //
        // AllocRefdef
        //
        public override idRefdef AllocRefdef()
        {
            return Globals.backEnd.AllocRefDef();
        }

        //
        // AllocRenderEntity
        //
        public override idRenderEntity AllocRenderEntity( ref idRefdef refdef )
        {
            return ((idRefdefLocal)refdef).GetNextRenderEntity();
        }

        //
        // SetWorldMatrix
        //
        private void SetWorldMatrix( idRenderEntityLocal entity )
        {
            idRenderCommand cmd = Globals.backEnd.GetCommandBuffer();

            cmd.type = renderCommandType.RC_SET_ENTITYMATRIX;
            cmd.entity = entity;
        }

        //
        // RenderScene
        //
        public override void RenderScene(idRefdef refdef)
        {
            idRenderCommand cmd = Globals.backEnd.GetCommandBuffer();
            
            cmd.type = renderCommandType.RC_SET_REFDEF;
            cmd.refdef = (idRefdefLocal)refdef;

            // Render the bsp if its present.
            if (map != null)
            {
                map.RenderMap(refdef.vieworg);
            }

            // Render the entities.
            for (int i = 0; i < cmd.refdef.num_entities; i++)
            {
                idRenderEntityLocal entity = cmd.refdef.entities[i];

                SetWorldMatrix(entity);

                switch (entity.reType)
                {
                    case refEntityType_t.RT_MODEL:
                        ((idModelLocal)entity.hModel).TessModel(ref entity);
                        break;
                }
            }
        }
    }
}
