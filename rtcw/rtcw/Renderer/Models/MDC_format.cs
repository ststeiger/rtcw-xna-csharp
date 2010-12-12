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

// MDC_format.cs (c) 2010 JV Software
//

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;


using idLib.Engine.Public;
using idLib.Math;

namespace rtcw.Renderer.Models
{
       // Ridah, mesh compression
    /*
    ==============================================================================

    MDC file format

    ==============================================================================
    */

    // version history:
    // 1 - original
    // 2 - changed tag structure so it only lists the names once

    class mdcXyzCompressed_t
    {
	    public uint ofsVec;                    // offset direction from the last base frame
    //	unsigned short	ofsVec;
    };

    class mdcTagName_t {
	    //char name[MAX_QPATH];           // tag name
        public string name;
    };

    class mdcTag_t {
	    //short[] xyz = new short[3];
	    //short[] angles = new short[3];
        public idVector3 xyz = new idVector3();
        public idVector3 angles = new idVector3();

        public mdcTag_t(ref idFile f)
        {
            for (int i = 0; i < 3; i++)
            {
                xyz[i] = f.ReadShort();
            }

            for (int i = 0; i < 3; i++)
            {
                angles[i] = f.ReadShort();
            }
        }
    };

    /*
    ** mdcSurface_t
    **
    ** CHUNK			SIZE
    ** header			sizeof( md3Surface_t )
    ** shaders			sizeof( md3Shader_t ) * numShaders
    ** triangles[0]		sizeof( md3Triangle_t ) * numTriangles
    ** st				sizeof( md3St_t ) * numVerts
    ** XyzNormals		sizeof( md3XyzNormal_t ) * numVerts * numBaseFrames
    ** XyzCompressed	sizeof( mdcXyzCompressed ) * numVerts * numCompFrames
    ** frameBaseFrames	sizeof( short ) * numFrames
    ** frameCompFrames	sizeof( short ) * numFrames (-1 if frame is a baseFrame)
    */
    class mdcSurface_t : idDrawSurface
    {
        public mdcSurface_t(ref idFile f)
        {
            ident = f.ReadInt();
            //if (ident != idModelMDC.MDC_IDENT)
            //{
            //    Engine.common.ErrorFatal("MDC Surface has a invalid ident\n");
           // }

            name = f.ReadString(Engine.MAX_QPATH);
            flags = f.ReadInt();
            numCompFrames = f.ReadInt();
            numBaseFrames = f.ReadInt();
            numShaders = f.ReadInt();
            numVerts = f.ReadInt();
            numTriangles = f.ReadInt();
            ofsTriangles = f.ReadInt();
            ofsShaders = f.ReadInt();
            ofsSt = f.ReadInt();
            ofsXyzNormals = f.ReadInt();
            ofsXyzCompressed = f.ReadInt();
            ofsFrameBaseFrames = f.ReadInt();
            ofsFrameCompFrames = f.ReadInt();
            ofsEnd = f.ReadInt();
        }

	    public int ident;                  //

	    //char name[MAX_QPATH];       // polyset name
        public string name;

	    public int flags;
	    public int numCompFrames;          // all surfaces in a model should have the same
	    public int numBaseFrames;          // ditto

	    public int numShaders;             // all surfaces in a model should have the same
	    public int numVerts;

	    public int numTriangles;
	    public int ofsTriangles;

	    public int ofsShaders;             // offset from start of md3Surface_t
	    public int ofsSt;                  // texture coords are common for all frames
	    public int ofsXyzNormals;          // numVerts * numBaseFrames
	    public int ofsXyzCompressed;       // numVerts * numCompFrames

	    public int ofsFrameBaseFrames;     // numFrames
	    public int ofsFrameCompFrames;     // numFrames

	    public int ofsEnd;                 // next surface follows

        public mdcXyzCompressed_t[] xyzCompressedIndexPool;
        public short[] baseFrames;
        public short[] compFrames;
    };

    class mdcHeader_t {
	    public int ident;
	    public int version;

	    public string name;           // model name

	    public int flags;

	    public int numFrames;
	    public int numTags;
	    public int numSurfaces;

	    public int numSkins;

	    public int ofsFrames;                  // offset for first frame, stores the bounds and localOrigin
	    public int ofsTagNames;                // numTags
	    public int ofsTags;                    // numFrames * numTags
	    public int ofsSurfaces;                // first surface, others follow

	    public int ofsEnd;                     // end of file
    };
    // done.
}
