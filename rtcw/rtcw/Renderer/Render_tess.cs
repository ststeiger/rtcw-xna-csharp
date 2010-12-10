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

// Render_tess.cs (c) 2010 JV Software
//

using System;

using idLib.Engine.Public;
using idLib.Math;

namespace rtcw.Renderer
{
    class stageVars_t
    {
	    public idVector4[] colors = new idVector4[idMaterialBase.SHADER_MAX_VERTEXES];
	    public idVector2[,] texcoords = new idVector2[idMaterialBase.NUM_TEXTURE_BUNDLES,idMaterialBase.SHADER_MAX_VERTEXES];
    };

    class shaderCommands_t
    {
        public delegate void currentStageIteratorFunc_t();
	    public short[] indexes = new short[idMaterialBase.SHADER_MAX_INDEXES];
	    public idVector4[] xyz = new idVector4[idMaterialBase.SHADER_MAX_VERTEXES];
	    public idVector4[] normal = new idVector4[idMaterialBase.SHADER_MAX_VERTEXES];
	    public idVector2[,] texCoords = new idVector2[idMaterialBase.SHADER_MAX_VERTEXES,2];
	    public idVector4[] vertexColors = new idVector4[idMaterialBase.SHADER_MAX_VERTEXES];
	    public int[] vertexDlightBits = new int[idMaterialBase.SHADER_MAX_VERTEXES];

	    public stageVars_t svars;

	    public idVector4[] constantColor255 = new idVector4[idMaterialBase.SHADER_MAX_VERTEXES];

	    public idMaterial shader;
	    public float shaderTime;
	    public int fogNum;

	    public int dlightBits;         // or together of all vertexDlightBits

	    public int numIndexes;
	    public int numVertexes;

	    // info extracted from current shader
	    public int numPasses;
        public currentStageIteratorFunc_t currentStageIteratorFunc;
	    public shaderStage_t[] xstages;
    };
}
