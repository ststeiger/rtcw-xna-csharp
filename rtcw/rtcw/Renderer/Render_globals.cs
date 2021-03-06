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

// Render_globals.cs (c) 2010 JV Software
//

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

using idLib.Engine.Public;
using idLib.Math;


namespace rtcw.Renderer
{
    
    // NOTE: also mirror changes to max2skl.c
    public enum surfaceType_t
    {
	    SF_BAD,
	    SF_SKIP,                // ignore
	    SF_FACE,
	    SF_GRID,
	    SF_TRIANGLES,
	    SF_POLY,
	    SF_MD3,
	    SF_MDC,
	    SF_MDS,
	    SF_FLARE,
	    SF_ENTITY,              // beams, rails, lightning, etc that can be determined by entity
	    SF_DISPLAY_LIST,

	    SF_NUM_SURFACE_TYPES,
	    SF_MAX = 0xffffff         // ensures that sizeof( surfaceType_t ) == sizeof( int )
    };

    public enum cullType_t {
	    CT_FRONT_SIDED,
	    CT_BACK_SIDED,
	    CT_TWO_SIDED
    };

    public enum fogPass_t
    {
	    FP_NONE,        // surface is translucent and will just be adjusted properly
	    FP_EQUAL,       // surface is opaque but possibly alpha tested
	    FP_LE           // surface is trnaslucent, but still needs a fog pass (fog surface)
    };

    public class skyParms_t
    {
	    public float cloudHeight;
	    public idImage[] outerbox = new idImage[6];
        public idImage[] innerbox= new idImage[6];
    };

    public class fogParms_t
   {
	    public idVector3 color = new idVector3();
	    public float depthForOpaque;
    };

    //
    // idDrawSurface
    //
    public abstract class idDrawSurface
    {
        public float sort;                      // bit combination for fast compares
	    public surfaceType_t       type;       // any of surface*_t
        public idMaterial[]        materials;
        public int startVertex;
        public int numVertexes;

        public int startIndex;
        public int numIndexes;

        public int visCount;

        public int fogIndex;
    }

    //
    // idDrawSurfaceFace
    //
    public class idDrawSurfaceFace : idDrawSurface
    {
        public idPlane plane = new idPlane();
    }

    //
    // idDrawSurfaceTri
    //
    public class idDrawSurfaceTri : idDrawSurface
    {
        
    }

    //
    // idDrawSurfaceFlare
    //
    public class idDrawSurfaceFlare : idDrawSurface {
	    public idVector3 origin;
        public idVector3 normal;
	    public idVector3 color;
    };

    /*
    ** performanceCounters_t
    */
    struct frontEndCounters_t
    {
	    public int c_sphere_cull_patch_in, c_sphere_cull_patch_clip, c_sphere_cull_patch_out;
        public int c_box_cull_patch_in, c_box_cull_patch_clip, c_box_cull_patch_out;
        public int c_sphere_cull_md3_in, c_sphere_cull_md3_clip, c_sphere_cull_md3_out;
        public int c_box_cull_md3_in, c_box_cull_md3_clip, c_box_cull_md3_out;

        public int c_leafs;
        public int c_dlightSurfaces;
        public int c_dlightSurfacesCulled;
    };


    //
    // orientationr_t
    //
    public struct orientationr_t
    {
	    public idVector3 origin;              // in world coordinates
	    public idMatrix axis;             // orientation in world
	    public idVector3  viewOrigin;          // viewParms->or.origin in local coordinates
	    public idMatrix modelMatrix;
    };

    //
    // idRenderEntityLocal
    //
    public class idRenderEntityLocal : idRenderEntity
    {
        public float axisLength;           // compensate for non-normalized axis

	    public bool needDlights;       // true for bmodels that touch a dlight
	    public bool lightingCalculated;
	    public idVector3 lightDir;            // normalized direction towards light
	    public idVector3 ambientLight;        // color normalized to 0-255
	    public int ambientLightInt;            // 32 bit rgba packed
	    public idVector3 directedLight;
	    public float brightness;
    }

    //
    // idWorldFog_t
    //
    public class idWorldFog_t {
	    public int originalBrushNumber;
	    public idVector3[] bounds = new idVector3[2];

	    public uint colorInt;                  // in packed byte format
	    public float tcScale;                      // texture coordinate vector scales
	    public fogParms_t parms;

	    // for clipping distance in fog when outside
	    public bool hasSurface;
        public idVector4 surface;
    };

    //
    // idRenderNode
    //
    public class idRenderNode {
        public const int CONTENTS_NODE   =    -1;
	    // common with leaf and node
	    public int contents;               // -1 for nodes, to differentiate from leafs
	    public int visframe;               // node needs to be traversed if current
	    public idVector3 mins, maxs;          // for bounding box culling
	    public idRenderNode parent;

	    // node specific
	    public idPlane    plane;
        public idRenderNode[] children = new idRenderNode[2];
        public int[] childrenhandles = new int[2];

	    // leaf specific
	    public int cluster;
        public int area;

        public int firstmarksurface;
        public int nummarksurfaces;

        // Used by the collision model manager.
        public int firstBrushSurface;
        public int numBrushSurfaces;
    };

    //
    // corona_t
    //
    public struct corona_t
    {
	    public idVector3 origin;
	    public idVector3 color;               // range from 0.0 to 1.0, should be color normalized
	    public idVector3 transformed;         // origin in local coordinate system
	    public float scale;                // uses r_flaresize as the baseline (1.0)
	    public int id;
	    public int flags;                  // '1' is 'visible'
					    // still send the corona request, even if not visible, for proper fading
    };

    public struct dlight_t
    {
	    public idVector3 origin;
	    public idVector3  color;               // range from 0.0 to 1.0, should be color normalized
	    float radius;

	    public idVector3  transformed;         // origin in local coordinate system

	    // Ridah
	    public int overdraw;
	    // done.

	    public idMaterial dlshader;  //----(SA) adding a shader to dlights, so, if desired, we can change the blend or texture of a dlight

	    public bool forced;        //----(SA)	use this dlight when r_dynamiclight is either 1 or 2 (rather than just 1) for "important" gameplay lights (alarm lights, etc)
	    //done

    };

    //
    // polyVert_t 
    //
    public struct polyVert_t
    {
	    public idVector3 xyz;
	    public idVector2 st;
	    public byte modulate_r;
        public byte modulate_g;
        public byte modulate_b;
        public byte modulate_a;
    };


    //
    // srfPoly_t
    //
    public struct srfPoly_t
    {
	    surfaceType_t surfaceType;
	    public idMaterial hShader;
	    public int fogIndex;
	    public int numVerts;
	    public polyVert_t[] verts;
    };

    //
    // idRefdefLocal
    //
    public class idRefdefLocal : idRefdef {

        public bool areamaskModified;      // qtrue if areamask changed since last scene

	    public float floatTime;                // tr.refdef.time / 1000.0

        public int refnum;
        public float zFar;

	    public int num_entities = 0;
        public idRenderEntityLocal[] entities = new idRenderEntityLocal[idRenderGlobals.MAX_RENDER_ENTITIES];
        /*
	    public int num_dlights;
	    public List<dlight_t> dlights;

	    public int num_coronas;
	    public List<corona_t>coronas;

	    public int numPolys;
	    public List<srfPoly_t> polys;

	    public int numDrawSurfs;
        public List<idDrawSurface> drawSurfs;
        */
        public idRenderEntityLocal GetNextRenderEntity()
        {
            if (entities[num_entities] == null)
            {
                entities[num_entities] = new idRenderEntityLocal();
            }

            return entities[num_entities++];
        }
    };

    //
    // viewParms_t
    //
    public class viewParms_t {
	    public orientationr_t  or;
	    public orientationr_t world;
	    public idVector3 pvsOrigin;               // may be different than or.origin for portals
	    public bool isPortal;              // true if this view is through a portal
	    public bool isMirror;              // the portal is a mirror, invert the face culling
	    public int frameSceneNum;              // copied from tr.frameSceneNum
	    public int frameCount;                 // copied from tr.frameCount
	    public idPlane portalPlane;           // clip anything behind this if mirroring
	    public int viewportX, viewportY, viewportWidth, viewportHeight;
	    public float fovX, fovY;
        public idMatrix projectionMatrix;
	    public idPlane[] frustum = new idPlane[4];
	    public idVector3[] visBounds = new idVector3[2];
	    public float zFar;

	    public int dirty;

	    public idFog glFog;                  // fog parameters	//----(SA)	added
    };

    //
    // idDrawVertexSkin
    //
    public struct idDrawVertexSkin
    {
        public idVector3 offset1;
        public idVector3 offset2;
        public idVector3 offset3;
        public idVector3 offset4;
        public idVector2 st;
        public idVector4 blendIndices;
        public idVector4 weights;
    };

    /*
    ** idRenderGlobals
    **
    ** Most renderer globals are defined here.
    ** backend functions should never modify any of these fields,
    ** but may read fields that aren't dynamically modified
    ** by the frontend.
    */
    class idRenderGlobals
    {
        public const int MAX_DRAWSURFS = 0x10000;
        public const int DRAWSURF_MASK      =     ( MAX_DRAWSURFS - 1 );
        public const int MAX_DRAWIMAGES     =     2048;
        public const int MAX_LIGHTMAPS      =     256;
        public const int MAX_SKINS          =     1024;
        public const int MAX_SHADERS        =     2048;
        public const int MAX_MOD_KNOWN      =     2048;
        public const int FOG_TABLE_SIZE     =     256;
        public const int FUNCTABLE_SIZE     =     1024;
        public const int FUNCTABLE_SIZE2    =     10;
        public const int FUNCTABLE_MASK     =     ( FUNCTABLE_SIZE - 1 );
        public const int MAX_POLYS          =     4096;
        public const int MAX_POLYVERTS = 8192;
        public const int MAX_FONTS = 6;
        public const int MAX_RENDER_COMMANDS = 0x40000;
        public const int SMP_FRAMES = 2;
        public const int MAX_RENDER_ENTITIES = 2000;

        public readonly static VertexDeclaration idDrawVertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(20, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(28, VertexElementFormat.Vector3, VertexElementUsage.Tangent, 0),
            new VertexElement(40, VertexElementFormat.Vector3, VertexElementUsage.Binormal, 0),
            new VertexElement(52, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(64, VertexElementFormat.Vector4, VertexElementUsage.Color, 0)
        );

        public readonly static VertexDeclaration idDrawSkinnedVertexDeclaration = new VertexDeclaration
        (
            // Skinned weight offset
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Position, 1),
            new VertexElement(24, VertexElementFormat.Vector3, VertexElementUsage.Position, 2),
            new VertexElement(36, VertexElementFormat.Vector3, VertexElementUsage.Position, 3),

            // ST Coordinate.
            new VertexElement(48, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),

            // Blend weights/Indicies
            new VertexElement(56, VertexElementFormat.Vector4, VertexElementUsage.BlendIndices, 0),
            new VertexElement(72, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0)
        );

        public int registeredFontCount = 0;
        public idFont[] registeredFont = new idFont[MAX_FONTS];

        public bool registered = false;                    // cleared at shutdown, set at beginRegistration

	    public int visCount;                           // incremented every time a new vis cluster is entered
	    public int frameCount;                         // incremented every frame
	    public int sceneCount;                         // incremented every scene
	    public int viewCount;                          // incremented every view (twice a scene if portaled)
											    // and every R_MarkFragments call

	    public int smpFrame;                           // toggles from 0 to 1 every endFrame

	    public int frameSceneNum;                      // zeroed at RE_BeginFrame

	    public bool worldMapLoaded;
	    public idWorld world;

	    //const byte              *externalVisData;   // from RE_SetWorldVisData, shared with CM_Load

	    public idImage           defaultImage;
	    public idImage[]         scratchImage = new idImage[32];
	    public idImage           fogImage;
	    public idImage           dlightImage;   // inverse-square highlight for projective adding
	    public idImage           flareImage;
	    public idImage           whiteImage;            // full of 0xff
	    public idImage           identityLightImage;    // full of tr.identityLightByte

	    public idMaterial        defaultShader;
	    public idMaterial        shadowShader;
    //	public idMaterial        projectionShadowShader;
	    public idMaterial        dlightShader;      //----(SA) added

	    public idMaterial        flareShader;
	    public idMaterial        spotFlareShader;
	    public string            sunShaderName;
	    public idMaterial        sunShader;
	    public idMaterial[]      sunflareShader = new idMaterial[6];  //----(SA) for the camera lens flare effect for sun

	    public int numLightmaps;
	    public idImage[]        lightmaps = new idImage[MAX_LIGHTMAPS];

	    public idRenderEntityLocal currentEntity;
	    public idRenderEntityLocal worldEntity;                  // point currentEntity at this when rendering world
	    public int currentEntityNum;
	    public int shiftedEntityNum;                       // currentEntityNum << QSORT_ENTITYNUM_SHIFT
	    public idModel          currentModel;

        public viewParms_t viewParms;

	    public float identityLight = 1;                        // 1.0 / ( 1 << overbrightBits )
	    public int identityLightByte;                      // identityLight * 255
	    public int overbrightBits;                         // r_overbrightBits->integer, but set to 0 if no hw gamma

        public orientationr_t or;                 // for current entity

        public idRefdefLocal refdef = new idRefdefLocal();
        public List<idWorldLocal> worlds = new List<idWorldLocal>();

	    public int viewCluster;

	    public idVector3 sunLight;                            // from the sky shader for this level
	    public idVector3 sunDirection;

    //----(SA)	added
	    public float lightGridMulAmbient;          // lightgrid multipliers specified in sky shader
	    public float lightGridMulDirected;         //
    //----(SA)	end

    //	qboolean				levelGLFog;

	    public frontEndCounters_t pc;
	    public int frontEndMsec;                           // not in pc due to clearing issue

	    //
	    // put large tables at the end, so most elements will be
	    // within the +/32K indexed range on risc processors
	    //
	    public idModel[] models = new idModel[MAX_MOD_KNOWN];
	    public int numModels;

	    public int numImages;
	    public idImage[] images = new idImage[MAX_DRAWIMAGES];
	    // Ridah
	    public int numCacheImages;

	    // shader indexes from other modules will be looked up in tr.shaders[]
	    // shader indexes from drawsurfs will be looked up in sortedShaders[]
	    // lower indexed sortedShaders must be rendered first (opaque surfaces before translucent)
	    public int numShaders;
	    public idMaterial[] shaders = new idMaterial[MAX_SHADERS];
	    public idMaterial[] sortedShaders = new idMaterial[MAX_SHADERS];

	    public int numSkins;
	    public idSkin[] skins = new idSkin[MAX_SKINS];

	    public float[] sinTable = new float[FUNCTABLE_SIZE];
	    public float[] squareTable = new float[FUNCTABLE_SIZE];
	    public float[] triangleTable = new float[FUNCTABLE_SIZE];
	    public float[] sawToothTable = new float[FUNCTABLE_SIZE];
	    public float[] inverseSawToothTable = new float[FUNCTABLE_SIZE];
	    public float[] fogTable = new float[FUNCTABLE_SIZE];

	    // RF, temp var used while parsing shader only
	    public int allowCompress;
    }
}
