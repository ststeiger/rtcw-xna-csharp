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

// MD3_Model.cs (c) 2010 JV Software
//

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;


using idLib.Engine.Public;
using idLib.Math;

namespace rtcw.Renderer.Models
{
    /*
    ========================================================================

    .MD3 triangle model file format

    ========================================================================
    */

    class md3Frame_t {
	    public idVector3 mins = new idVector3();
        public idVector3 maxss = new idVector3();
	    public idVector3 localOrigin = new idVector3();

	    public float radius;
	    //char name[16];
        public string name;
    };

    class md3Tag_t {
	    //char name[MAX_QPATH];           // tag name
        public string name;
        public idVector3 origin;
	    public idMatrix axis;
    };

    /*
    ** md3Surface_t
    **
    ** CHUNK			SIZE
    ** header			sizeof( md3Surface_t )
    ** shaders			sizeof( md3Shader_t ) * numShaders
    ** triangles[0]		sizeof( md3Triangle_t ) * numTriangles
    ** st				sizeof( md3St_t ) * numVerts
    ** XyzNormals		sizeof( md3XyzNormal_t ) * numVerts * numFrames
    */
    class md3Surface_t {
	    public int ident;                  //

	    //char name[MAX_QPATH];       // polyset name
        public string name;

	    public int flags;
	    public int numFrames;              // all surfaces in a model should have the same

	    public int numShaders;             // all surfaces in a model should have the same
	    public int numVerts;

	    public int numTriangles;
	    public int ofsTriangles;

	    public int ofsShaders;             // offset from start of md3Surface_t
	    public int ofsSt;                  // texture coords are common for all frames
	    public int ofsXyzNormals;          // numVerts * numFrames

	    public int ofsEnd;                 // next surface follows
    };

    class md3Shader_t {
	    //char name[MAX_QPATH];
        public string name;
	    public int shaderIndex;                // for in-game use
    };

    class md3Triangle_t {
	    public int[] indexes = new int[3];
    };

    class md3St_t {
	    public idVector2 st = new idVector2();
    };

    class md3XyzNormal_t {
	    public short[] xyz = new short[3];
	    public short normal;
    };

    class md3Header_t {
	    public int ident;
	    public int version;

	    //char name[MAX_QPATH];           // model name
        public string name;

	    public int flags;

	    public int numFrames;
	    public int numTags;
	    public int numSurfaces;

	    public int numSkins;

	    public int ofsFrames;                  // offset for first frame
	    public int ofsTags;                    // numFrames * numTags
	    public int ofsSurfaces;                // first surface, others follow

	    public int ofsEnd;                     // end of file
    };

}
