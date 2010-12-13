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

// Materials.cs (c) 2010 JV Software
//

using System;
using Microsoft.Xna.Framework.Graphics;
using idLib.Engine.Public;
using idLib.Math;

namespace rtcw.Renderer
{
    public enum shaderSort_t
    {
	    SS_BAD,
	    SS_PORTAL,          // mirrors, portals, viewscreens
	    SS_ENVIRONMENT,     // sky box
	    SS_OPAQUE,          // opaque

	    SS_DECAL,           // scorch marks, etc.
	    SS_SEE_THROUGH,     // ladders, grates, grills that may have small blended edges
						    // in addition to alpha test
	    SS_BANNER,

	    SS_FOG,

	    SS_UNDERWATER,      // for items that should be drawn in front of the water plane

	    SS_BLEND0,          // regular transparency and filters
	    SS_BLEND1,          // generally only used for additive type effects
	    SS_BLEND2,
	    SS_BLEND3,

	    SS_BLEND6,
	    SS_STENCIL_SHADOW,
	    SS_ALMOST_NEAREST,  // gun smoke puffs

	    SS_NEAREST          // blood blobs
    };

    public enum genFunc_t
    {
	    GF_NONE,

	    GF_SIN,
	    GF_SQUARE,
	    GF_TRIANGLE,
	    GF_SAWTOOTH,
	    GF_INVERSE_SAWTOOTH,

	    GF_NOISE

    };


     public enum deform_t {
	    DEFORM_NONE,
	    DEFORM_WAVE,
	    DEFORM_NORMALS,
	    DEFORM_BULGE,
	    DEFORM_MOVE,
	    DEFORM_PROJECTION_SHADOW,
	    DEFORM_AUTOSPRITE,
	    DEFORM_AUTOSPRITE2,
	    DEFORM_TEXT0,
	    DEFORM_TEXT1,
	    DEFORM_TEXT2,
	    DEFORM_TEXT3,
	    DEFORM_TEXT4,
	    DEFORM_TEXT5,
	    DEFORM_TEXT6,
	    DEFORM_TEXT7
    };

     public enum alphaGen_t
     {
	    AGEN_IDENTITY,
	    AGEN_SKIP,
	    AGEN_ENTITY,
	    AGEN_ONE_MINUS_ENTITY,
	    AGEN_NORMALZFADE,   // Ridah
	    AGEN_VERTEX,
	    AGEN_ONE_MINUS_VERTEX,
	    AGEN_LIGHTING_SPECULAR,
	    AGEN_WAVEFORM,
	    AGEN_PORTAL,
	    AGEN_CONST
    };

    public enum colorGen_t
    {
	    CGEN_BAD,
	    CGEN_IDENTITY_LIGHTING, // tr.identityLight
	    CGEN_IDENTITY,          // always (1,1,1,1)
	    CGEN_ENTITY,            // grabbed from entity's modulate field
	    CGEN_ONE_MINUS_ENTITY,  // grabbed from 1 - entity.modulate
	    CGEN_EXACT_VERTEX,      // tess.vertexColors
	    CGEN_VERTEX,            // tess.vertexColors * tr.identityLight
	    CGEN_ONE_MINUS_VERTEX,
	    CGEN_WAVEFORM,          // programmatically generated
	    CGEN_LIGHTING_DIFFUSE,
	    CGEN_FOG,               // standard fog
	    CGEN_CONST              // fixed color
    };

    public enum texCoordGen_t
    {
	    TCGEN_BAD,
	    TCGEN_IDENTITY,         // clear to 0,0
	    TCGEN_LIGHTMAP,
	    TCGEN_TEXTURE,
	    TCGEN_ENVIRONMENT_MAPPED,
	    TCGEN_FIRERISEENV_MAPPED,
	    TCGEN_FOG,
	    TCGEN_VECTOR            // S and T from world coordinates
    };

    public enum acff_t
    {
	    ACFF_NONE,
	    ACFF_MODULATE_RGB,
	    ACFF_MODULATE_RGBA,
	    ACFF_MODULATE_ALPHA
    };

    public class waveForm_t
    {
	    public genFunc_t func;

	    public float base_c;
	    public float amplitude;
	    public float phase;
	    public float frequency;
    };

    public enum texMod_t
    {
	    TMOD_NONE,
	    TMOD_TRANSFORM,
	    TMOD_TURBULENT,
	    TMOD_SCROLL,
	    TMOD_SCALE,
	    TMOD_STRETCH,
	    TMOD_ROTATE,
	    TMOD_ENTITY_TRANSLATE,
	    TMOD_SWAP
    };


    public class deformStage_t
    {
	    public deform_t deformation;               // vertex coordinate modification type

	    public idVector3 moveVector = new idVector3();
	    public waveForm_t deformationWave;
	    public float deformationSpread;

	    public float bulgeWidth;
	    public float bulgeHeight;
	    public float bulgeSpeed;
    };


     public class texModInfo_t {
	    public texMod_t type;

	    // used for TMOD_TURBULENT and TMOD_STRETCH
	    public waveForm_t wave = new waveForm_t();

	    // used for TMOD_TRANSFORM
	    public idVector2[] matrix = new idVector2[2];                 // s' = s * m[0][0] + t * m[1][0] + trans[0]
	    public idVector2 translate = new idVector2();                 // t' = s * m[0][1] + t * m[0][1] + trans[1]

	    // used for TMOD_SCALE
	    public idVector2 scale = new idVector2();                     // s *= scale[0]
										    // t *= scale[1]

	    // used for TMOD_SCROLL
	    public idVector2 scroll = new idVector2();                    // s' = s + scroll[0] * time
										    // t' = t + scroll[1] * time

	    // + = clockwise
	    // - = counterclockwise
	    public float rotateSpeed;
    };

     public class textureBundle_t
     {
	    public idImage[] image = new idImage[idMaterialBase.MAX_IMAGE_ANIMATIONS];
	    public int numImageAnimations;
	    public float imageAnimationSpeed;

	    public texCoordGen_t tcGen;
	    public idVector3[] tcGenVectors = new idVector3[2];

	    public int numTexMods;
        public texModInfo_t[] texMods = new texModInfo_t[idMaterialBase.TR_MAX_TEXMODS];

	    public int videoMapHandle;
	    public bool isLightmap;
	    public bool vertexLightmap;
	    public bool isVideoMap;
    };

    public class shaderStage_t
    {
	    public bool active;

        public textureBundle_t[] bundle = new textureBundle_t[idMaterialBase.NUM_TEXTURE_BUNDLES];

	    public waveForm_t rgbWave;
	    public colorGen_t rgbGen;

	    public waveForm_t alphaWave;
	    public alphaGen_t alphaGen;

	    public idVector3 constantColor = new idVector3();                      // for CGEN_CONST and AGEN_CONST
        public float constantColorAlpha;
	    public int stateBits;                         // GLS_xxxx mask

        public bool useBlending;
        public BlendState blendState;

	    public acff_t adjustColorsForFog;

	    // Ridah
	    public float[] zFadeBounds = new float[2];

	    public bool isDetail;
        public bool isFogged;              // used only for shaders that have fog disabled, so we can enable it for individual stages
    };

    //
    // idMaterialLocal
    //
    public class idMaterialBase
    {
        public const int SHADER_MAX_VERTEXES = 4000;
        public const int SHADER_MAX_INDEXES = ( 6 * SHADER_MAX_VERTEXES );
        public const int NUM_TEXTURE_BUNDLES = 2;
        public const int MAX_SHADER_STAGES = 8;
        public const int TR_MAX_TEXMODS = 4;
        public const int MAX_SHADER_DEFORMS =  3;
        // RF increased this for onfire animation
        //#define	MAX_IMAGE_ANIMATIONS	8
        public const int MAX_IMAGE_ANIMATIONS =   16;
        public const int MAX_SHADER_STATES = 2048;
        public const int MAX_STATES_PER_SHADER = 32;

        public string name;              // game path, including extension
	    public int lightmapIndex;                  // for a shader to match, both name and lightmapIndex must match

	    public int index;                          // this shader == tr.shaders[index]
	    public int sortedIndex;                    // this shader == tr.sortedShaders[sortedIndex]

	    public float sort;                         // lower numbered shaders draw before higher numbered

	    public bool defaultShader;             // we want to return index 0 if the shader failed to
										    // load for some reason, but R_FindShader should
										    // still keep a name allocated for it, so if
										    // something calls RE_RegisterShader again with
										    // the same name, we don't try looking for it again

	    public bool explicitlyDefined;         // found in a .shader file

	    public int surfaceFlags;                   // if explicitlyDefined, this will have SURF_* flags
	    public int contentFlags;

	    public bool entityMergable;            // merge across entites optimizable (smoke, blood)

	    public bool isSky;
        public skyParms_t sky = new skyParms_t();
	    public fogParms_t fogParms;

	    public float portalRange;                  // distance to fog out at

	    public BlendState multitextureEnv;                // 0, GL_MODULATE, GL_ADD (FIXME: put in stage)

        public cullType_t cullType;                // CT_FRONT_SIDED, CT_BACK_SIDED, or CT_TWO_SIDED
	    public bool polygonOffset;             // set for decals and other items that must be offset
	    public bool noMipMaps;                 // for console fonts, 2D elements, etc.
	    public bool noPicMip;                  // for images that must always be full resolution
	    public bool characterMip;              // use r_picmip2 rather than r_picmip

	    public fogPass_t fogPass;                  // draw a blended pass, possibly with depth test equals

	    public bool needsNormal;               // not all shaders will need all data to be gathered
	    public bool needsST1;
	    public bool needsST2;
	    public bool needsColor;

	    // Ridah
	    public bool noFog;

	    public int numDeforms = 0;
	    public deformStage_t[] deforms = new deformStage_t[MAX_SHADER_DEFORMS];

	    public int numUnfoggedPasses;
	    public shaderStage_t[] stages = new shaderStage_t[MAX_SHADER_STAGES];

        public shaderCommands_t.currentStageIteratorFunc_t optimalStageIteratorFunc;

	    public float clampTime;                                    // time this shader is clamped to
	    public float timeOffset;                                   // current time offset for this shader

	    public int numStates;                                      // if non-zero this is a state shader
	    idMaterialLocal currentShader;                     // current state if this is a state shader
	    idMaterialLocal parentShader;                      // current state if this is a state shader
	    public int currentState;                                   // current state index for cycle purposes
	    public long expireTime;                                    // time in milliseconds this expires

	    idMaterialLocal remappedShader;                    // current shader this one is remapped too

	    public int[] shaderStates = new int[MAX_STATES_PER_SHADER];            // index to valid shader states
    }
}
