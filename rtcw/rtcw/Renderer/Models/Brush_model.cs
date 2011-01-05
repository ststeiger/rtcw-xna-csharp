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

// Brush_Model.cs (c) 2010 JV Software
//

using Microsoft.Xna.Framework.Graphics;
using idLib.Math;
using rtcw.Renderer.Map;
using idLib.Engine.Public;

namespace rtcw.Renderer.Models
{
    //
    // idModelBrush
    //
    public class idModelBrush : idModelLocal
    {
        idVector3 mins = new idVector3();
        idVector3 maxs = new idVector3();
        int firstSurface;
        int numSurfaces;
        string modelname;
        idMap parent;

        //
        // idModelBrush
        //
        public idModelBrush(string name, idMap parent)
        {
            modelname = name;
            this.parent = parent;
        }

        //
        // SetModelBounds
        //
        public void SetModelBounds(idVector3 mins, idVector3 maxs)
        {
            this.mins.X = mins.X;
            this.mins.Y = mins.Y;
            this.mins.Z = mins.Z;

            this.maxs.X = maxs.X;
            this.maxs.Y = maxs.Y;
            this.maxs.Z = maxs.Z;
        }

        //
        // SetSurfaceParams
        //
        public void SetSurfaceParams(int firstSurface, int numSurfaces)
        {
            this.firstSurface = firstSurface;
            this.numSurfaces = numSurfaces;
        }

        //
        // GetModelBounds
        //
        public override void GetModelBounds(out idLib.Math.idVector3 mins, out idLib.Math.idVector3 maxs)
        {
            mins = this.mins;
            maxs = this.maxs;
        }

        //
        // GetName
        //
        public override string GetName()
        {
            return modelname;
        }

        //
        // GetNumFrames
        //
        public override int GetNumFrames()
        {
            return -1;
        }

        //
        // TestModel
        //
        public override void TessModel(ref idRenderEntityLocal entity)
        {
            Globals.SetVertexIndexBuffers(parent.vertexBuffer, parent.indexBuffer);
            for (int i = firstSurface; i < firstSurface + numSurfaces; i++)
            {
                parent.drawSurfs[i].visCount = Globals.visCount;
                Globals.SortSurface<idDrawSurface>(0, ref parent.drawSurfs[i]);
            }
        }

        //
        // BuildVertexIndexBuffer
        //
        public override void BuildVertexIndexBuffer()
        {
            
        }
    }
}
