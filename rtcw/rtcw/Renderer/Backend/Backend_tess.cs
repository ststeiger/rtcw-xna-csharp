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

// Render_tess.cs (c) 2010 JV Software
//

using System;

using idLib.Engine.Public;
using idLib.Math;

namespace rtcw.Renderer
{
    public class stageVars_t
    {
	    public idVector4[] colors = new idVector4[idMaterialBase.SHADER_MAX_VERTEXES];
	    public idVector2[,] texcoords = new idVector2[idMaterialBase.NUM_TEXTURE_BUNDLES,idMaterialBase.SHADER_MAX_VERTEXES];
    };

    public class shaderCommands_t
    {
        public delegate void currentStageIteratorFunc_t();
	    public short[] indexes = new short[idMaterialBase.SHADER_MAX_INDEXES];
        public idDrawVertex[] drawVerts = new idDrawVertex[idMaterialBase.SHADER_MAX_VERTEXES];
	    public int[] vertexDlightBits = new int[idMaterialBase.SHADER_MAX_VERTEXES];

	    public stageVars_t svars;

	    public idVector4[] constantColor255 = new idVector4[idMaterialBase.SHADER_MAX_VERTEXES];

	    public idMaterialBase shader;
	    public float shaderTime;
	    public int fogNum;

	    public int dlightBits;         // or together of all vertexDlightBits

	    public int numIndexes = 0;
	    public int numVertexes = 0;

	    // info extracted from current shader
	    public int numPasses;
        public currentStageIteratorFunc_t currentStageIteratorFunc;
	    public shaderStage_t[] xstages;

        //
        // UploadVertex
        //
        public void UploadVertex(float x, float y, float z, float s, float t)
        {
            drawVerts[numVertexes].xyz.X = x;
            drawVerts[numVertexes].xyz.Y = y;
            drawVerts[numVertexes].xyz.Z = z;

            drawVerts[numVertexes].st.X = s;
            drawVerts[numVertexes].st.Y = t;

            drawVerts[numVertexes].lightmapST.X = 0;
            drawVerts[numVertexes].lightmapST.Y = 0;

            drawVerts[numVertexes].tangent.X = 0;
            drawVerts[numVertexes].tangent.Y = 0;
            drawVerts[numVertexes].tangent.Z = 0;

            drawVerts[numVertexes].normal.X = 0;
            drawVerts[numVertexes].normal.Y = 0;
            drawVerts[numVertexes].normal.Z = 0;

            drawVerts[numVertexes].binormal.X = 0;
            drawVerts[numVertexes].binormal.Y = 0;
            drawVerts[numVertexes].binormal.Z = 0;

            numVertexes++;
        }

        //
        // Uploadindex
        //
        public void UploadIndex(short s)
        {
            indexes[numIndexes++] = s;
        }
    };
}
