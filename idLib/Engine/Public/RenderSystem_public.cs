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

// RenderSystem_public.cs (c) 2010 JV Software
//

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using idLib.Math;

namespace idLib.Engine.Public
{
    //
    // idContentMask
    //
    public static class idContentMask
    {
        // content masks
        public const int MASK_ALL           = ( -1 );
        public const int MASK_SOLID         = (surfaceFlags.CONTENTS_SOLID);
        public const int MASK_PLAYERSOLID   = (surfaceFlags.CONTENTS_SOLID | surfaceFlags.CONTENTS_PLAYERCLIP | surfaceFlags.CONTENTS_BODY);
        public const int MASK_DEADSOLID     = (surfaceFlags.CONTENTS_SOLID | surfaceFlags.CONTENTS_PLAYERCLIP);
        public const int MASK_WATER         = (surfaceFlags.CONTENTS_WATER | surfaceFlags.CONTENTS_LAVA | surfaceFlags.CONTENTS_SLIME);
        //public const int	MASK_OPAQUE	   =	  (CONTENTS_SOLID|CONTENTS_SLIME|CONTENTS_LAVA)
        public const int MASK_OPAQUE        = (surfaceFlags.CONTENTS_SOLID | surfaceFlags.CONTENTS_LAVA);      //----(SA)	modified since slime is no longer deadly
        public const int MASK_SHOT          = (surfaceFlags.CONTENTS_SOLID | surfaceFlags.CONTENTS_BODY | surfaceFlags.CONTENTS_CORPSE | surfaceFlags.CONTENTS_CLIPSHOT);
        public const int MASK_MISSILESHOT   = (MASK_SHOT | surfaceFlags.CONTENTS_MISSILECLIP);
        public const int MASK_AISIGHT       = (surfaceFlags.CONTENTS_SOLID | surfaceFlags.CONTENTS_AI_NOSIGHT);
    }

    //                                                                  //
    // WARNING:: synch FOG_SERVER in sv_ccmds.c if you change anything	//
    //                                                                  //
    public enum fogType
    {
	    FOG_NONE,       //	0

	    FOG_SKY,        //	1	fog values to apply to the sky when using density fog for the world (non-distance clipping fog) (only used if(glfogsettings[FOG_MAP].registered) or if(glfogsettings[FOG_MAP].registered))
	    FOG_PORTALVIEW, //	2	used by the portal sky scene
	    FOG_HUD,        //	3	used by the 3D hud scene

	    //		The result of these for a given frame is copied to the scene.glFog when the scene is rendered

	    // the following are fogs applied to the main world scene
	    FOG_MAP,        //	4	use fog parameter specified using the "fogvars" in the sky shader
	    FOG_WATER,      //	5	used when underwater
	    FOG_SERVER,     //	6	the server has set my fog (probably a target_fog) (keep synch in sv_ccmds.c !!!)
	    FOG_CURRENT,    //	7	stores the current values when a transition starts
	    FOG_LAST,       //	8	stores the current values when a transition starts
	    FOG_TARGET,     //	9	the values it's transitioning to.

	    FOG_CMD_SWITCHFOG,  // 10	transition to the fog specified in the second parameter of R_SetFog(...) (keep synch in sv_ccmds.c !!!)

	    NUM_FOGS
    };
    public static class surfaceFlags
    {
        // contents flags are seperate bits
        // a given brush can contribute multiple content bits

        // these definitions also need to be in q_shared.h!

        public const int CONTENTS_SOLID = 1;      // an eye is never valid in a solid

        public const int CONTENTS_LIGHTGRID  =    4;   //----(SA)	added

        public const int CONTENTS_LAVA       =    8;
        public const int CONTENTS_SLIME      =   16;
        public const int CONTENTS_WATER      =    32;
        public const int CONTENTS_FOG        =    64;


        //----(SA) added
        public const int CONTENTS_MISSILECLIP =   128; // 0x80
        public const int CONTENTS_ITEM        =   256; // 0x100
        //----(SA) end

        // RF, AI sight/nosight & bullet/nobullet
        public const int CONTENTS_AI_NOSIGHT  =   0x1000;  // AI cannot see through this
        public const int CONTENTS_CLIPSHOT    =   0x2000;  // bullets hit this
        // RF, end

        public const int CONTENTS_MOVER       =   0x4000;
        public const int CONTENTS_AREAPORTAL  =   0x8000;

        public const int CONTENTS_PLAYERCLIP  =   0x10000;
        public const int CONTENTS_MONSTERCLIP =   0x20000;

        //bot specific contents types
        public const int CONTENTS_TELEPORTER  =   0x40000;
        public const int CONTENTS_JUMPPAD     =   0x80000;
        public const int CONTENTS_CLUSTERPORTAL =  0x100000;
        public const int CONTENTS_DONOTENTER   =  0x200000;
        public const int CONTENTS_DONOTENTER_LARGE =      0x400000;

        public const int CONTENTS_ORIGIN    =     0x1000000;   // removed before bsping an entity

        public const int CONTENTS_BODY      =     0x2000000;   // should never be on a brush, only in game
        public const int CONTENTS_CORPSE    =     0x4000000;
        public const int CONTENTS_DETAIL    =     0x8000000 ;  // brushes not used for the bsp

        public const int CONTENTS_STRUCTURAL =    0x10000000;  // brushes used for the bsp
        public const int CONTENTS_TRANSLUCENT =   0x20000000;  // don't consume surface fragments inside
        public const int CONTENTS_TRIGGER     =   0x40000000;
        public const uint CONTENTS_NODROP      =   0x80000000;  // don't leave bodies or items (death fog, lava)zz

        public const int SURF_NODAMAGE       =    0x1;     // never give falling damage
        public const int SURF_SLICK          =    0x2;     // effects game physics
        public const int SURF_SKY            =    0x4 ;    // lighting from environment map
        public const int SURF_LADDER         =    0x8;
        public const int SURF_NOIMPACT       =    0x10;    // don't make missile explosions
        public const int SURF_NOMARKS        =    0x20 ;   // don't leave missile marks
        //#define	SURF_FLESH			0x40	// make flesh sounds and effects
        public const int SURF_CERAMIC        =    0x40;    // out of surf's, so replacing unused 'SURF_FLESH'
        public const int SURF_NODRAW         =    0x80;    // don't generate a drawsurface at all
        public const int SURF_HINT           =    0x100;   // make a primary bsp splitter
        public const int SURF_SKIP           =    0x200;   // completely ignore, allowing non-closed brushes
        public const int SURF_NOLIGHTMAP     =    0x400;   // surface doesn't need a lightmap
        public const int SURF_POINTLIGHT     =    0x800 ;  // generate lighting info at vertexes
        // JOSEPH 9-16-99
        public const int SURF_METAL          =    0x1000;  // clanking footsteps
        // END JOSEPH
        public const int SURF_NOSTEPS        =    0x2000;  // no footstep sounds
        public const int SURF_NONSOLID       =    0x4000;  // don't collide against curves with this set
        public const int SURF_LIGHTFILTER    =    0x8000;  // act as a light filter during q3map -light
        public const int SURF_ALPHASHADOW    =    0x10000; // do per-pixel light shadow casting in q3map
        public const int SURF_NODLIGHT       =    0x20000; // don't dlight even if solid (solid lava, skies)
        // JOSEPH 9-16-99
        // Ridah, 11-01-99 (Q3 merge)
        public const int SURF_WOOD           =    0x40000;
        public const int SURF_GRASS          =    0x80000;
        public const int SURF_GRAVEL         =    0x100000;
        // END JOSEPH

        // (SA)
        //#define SURF_SMGROUP			0x200000
        public const int SURF_GLASS          =    0x200000;    // out of surf's, so replacing unused 'SURF_SMGROUP'
        public const int SURF_SNOW           =    0x400000;
        public const int SURF_ROOF           =    0x800000;

        //#define	SURF_RUBBLE				0x1000000	// stole 'rubble' for
        public const int SURF_RUBBLE          =   0x1000000;
        public const int SURF_CARPET          =   0x2000000;

        public const int SURF_MONSTERSLICK    =   0x4000000;   // slick surf that only affects ai's
        // #define SURF_DUST				0x8000000 // leave a dust trail when walking on this surface
        public const int SURF_MONSLICK_W      =   0x8000000;

        public const int SURF_MONSLICK_N      =   0x10000000;
        public const int SURF_MONSLICK_E      =   0x20000000;
        public const int SURF_MONSLICK_S      =   0x40000000;
    }

    //
    // idRenderType
    //
    public static class idRenderType
    {
        // renderfx flags
        public const int RF_MINLIGHT      =   1 ;      // allways have some light (viewmodel, some items)
        public const int RF_THIRD_PERSON  =   2;       // don't draw through eyes, only mirrors (player bodies, chat sprites)
        public const int RF_FIRST_PERSON  =   4;       // only draw through eyes (view weapon, damage blood blob)
        public const int RF_DEPTHHACK     =   8;       // for view weapon Z crunching
        public const int RF_NOSHADOW      =   64;      // don't add stencil shadows

        public const int RF_LIGHTING_ORIGIN = 128;     // use refEntity->lightingOrigin instead of refEntity->origin
									        // for lighting.  This allows entities to sink into the floor
									        // with their origin going solid, and allows all parts of a
									        // player to get the same lighting
        public const int RF_SHADOW_PLANE   =  256;     // use refEntity->shadowPlane
        public const int RF_WRAP_FRAMES    =  512;     // mod the model frames by the maxframes to allow continuous
									        // animation without needing to know the frame count

        public const int RF_HILIGHT      =    ( 1 << 8 );  // more than RF_MINLIGHT.  For when an object is "Highlighted" (looked at/training identification/etc)
        public const int RF_BLINK        =    ( 1 << 9 );  // eyes in 'blink' state

        // refdef flags
        public const int RDF_NOWORLDMODEL =   1;       // used for player configuration screen
        public const int  RDF_HYPERSPACE  =   4;       // teleportation effect

        // Rafael
        public const int RDF_SKYBOXPORTAL =   8;

        public const int RDF_DRAWSKYBOX   =   16;      // the above marks a scene as being a 'portal sky'.  this flag says to draw it or not

        //----(SA)
        public const int RDF_UNDERWATER   =   (1 << 4);  // so the renderer knows to use underwater fog when the player is underwater
        public const int RDF_DRAWINGSKY   =   ( 1 << 5 );
        public const int RDF_SNOOPERVIEW  =   ( 1 << 6 );  //----(SA)	added
    }

    //
    // idSkin
    //
    public abstract class idSkin
    {

    }

    //
    // refEntityType_t
    //
    public enum refEntityType_t {
	    RT_MODEL,
	    RT_POLY,
	    RT_SPRITE,
	    RT_SPLASH,  // ripple effect
	    RT_BEAM,
	    RT_RAIL_CORE,
	    RT_RAIL_CORE_TAPER, // a modified core that creates a properly texture mapped core that's wider at one end
	    RT_RAIL_RINGS,
	    RT_LIGHTNING,
	    RT_PORTALSURFACE,       // doesn't draw anything, just info for portals

	    RT_MAX_REF_ENTITY_TYPE
    };

    //
    // idFog
    //
    public class idFog {
	    public int mode;                   // GL_LINEAR, GL_EXP
	    public int hint;                   // GL_DONT_CARE
	    public int startTime;              // in ms
	    public int finishTime;             // in ms
	    public idVector4 color;
	    public float start;                // near
        public float end;                  // far
	    public bool useEndForClip;     // use the 'far' value for the far clipping plane
	    public float density;              // 0.0-1.0
	    public bool registered;        // has this fog been set up?
        public bool drawsky;           // draw skybox
        public bool clearscreen;       // clear the GL color buffer

	    public int dirty;
    };

    //
    // idRenderEntity
    //
    public class idRenderEntity
    {
        public refEntityType_t reType;
	    public int renderfx;

	    public idModel hModel;               // opaque type outside refresh

	    // most recent data
	    public idVector3 lightingOrigin;          // so multi-part models can be lit identically (RF_LIGHTING_ORIGIN)
	    public float shadowPlane;              // projection shadows go here, stencils go slightly lower

	    public idMatrix axis;                 // rotation vectors
	    public idMatrix torsoAxis;            // rotation vectors for torso section of skeletal animation
	    public bool nonNormalizedAxes;     // axis are not normalized, i.e. they have scale
	    public idVector3 origin;                // also used as MODEL_BEAM's "from"
	    public int frame;                      // also used as MODEL_BEAM's diameter
	    public int torsoFrame;                 // skeletal torso can have frame independant of legs frame

	    public idVector3 scale;       //----(SA)	added

	    // previous data for frame interpolation
	    public idVector3 oldorigin;             // also used as MODEL_BEAM's "to"
	    public int oldframe;
	    public int oldTorsoFrame;
	    public float backlerp;                 // 0.0 = current, 1.0 = old
	    public float torsoBacklerp;

	    // texturing
	    public int skinNum;                    // inline skin index
	    public idSkin customSkin;           // NULL for default skin
	    public idSkin customShader;         // use one image for the entire thing

	    // misc
	    byte shaderR;             // colors used by rgbgen entity shaders
        byte shaderG;
        byte shaderB;
        byte shaderA;
	    public idVector2 shaderTexCoord;        // texture coordinates used by tcMod entity modifiers
	    public float shaderTime;               // subtracted from refdef time to control effect start times

	    // extra sprite information
	    public float radius;
	    public float rotation;

	    // Ridah
	    public idVector3 fireRiseDir;

	    // Ridah, entity fading (gibs, debris, etc)
	    public int fadeStartTime, fadeEndTime;

	    public float hilightIntensity;         //----(SA)	added

	    public int reFlags;

	    public int entityNum;                  // currentState.number, so we can attach rendering effects to specific entities (Zombie)
    }

    // font support
    public struct glyphInfo_t {
	    public int height;     // number of scan lines
	    public int top;        // top of glyph in buffer
	    public int bottom;     // bottom of glyph in buffer
	    public int pitch;      // width for copying
	    public int xSkip;      // x adjustment
	    public int imageWidth; // width of actual image
	    public int imageHeight; // height of actual image
	    public float s;        // x offset in image where glyph starts
	    public float t;        // y offset in image where glyph starts
	    public float s2;
	    public float t2;
	    public idMaterial glyph; // handle to the shader with the glyph
        public string shaderName;
    };

    //
    // idOrientation
    //
    public struct idOrientation {
	    public idVector3 origin;
	    public idMatrix axis;
    };

    //
    // idFont
    //
    public class idFont {
        public const int GLYPH_START  =  0;
        public const int GLYPH_END    = 255;
        public const int GLYPH_CHARSTART = 32;
        public const int GLYPH_CHAREND   = 127;
        public const int GLYPHS_PER_FONT = GLYPH_END - GLYPH_START + 1;

	    public glyphInfo_t[] glyphs = new glyphInfo_t [GLYPHS_PER_FONT];
	    public float glyphScale;
        public string name;
        public int hashValue;
    };

    public class idRefdef
    {
        public int x, y, width, height;
	    public float fov_x, fov_y;
	    public idVector3 vieworg = new idVector3();
	    public idMatrix viewaxis = new idMatrix();             // transformation matrix

	    public int time;           // time in milliseconds for shader effects and other time dependent rendering issues
	    public int rdflags;                    // RDF_NOWORLDMODEL, etc

	    // 1 bits will prevent the associated area from rendering at all
	    //byte areamask[MAX_MAP_AREA_BYTES];




	    // text messages for deform text shaders
	    //char text[MAX_RENDER_STRINGS][MAX_RENDER_STRING_LENGTH];


    //----(SA)	added (needed to pass fog infos into the portal sky scene)
	    //glfog_t glfog;
    //----(SA)	end
    }

    //
    // idWorld
    //
    public abstract class idWorld
    {
        // Allocates a refdef.
        public abstract idRefdef AllocRefdef();

        // Allocates a render entity that is attached to this world.
        public abstract idRenderEntity AllocRenderEntity(ref idRefdef refdef);

        // Renders the world.
        public abstract void RenderScene(idRefdef refdef);

        // LoadBrushModel
        public abstract idModel LoadBrushModel(string name);

        // Tests to see if a point is visible from a certain location.
        public abstract bool isPointInPVS(idVector3 pvsorigin, idVector3 point);

        // Tests to see if a point + bounding box is visible.
        public abstract bool isBoundsInPVS(idVector3 pvsorigin, idVector3 point, idBounds bounds);

        // Returns the loaded cm handle.
        public abstract idCollisionModel cm();
    }

    //
    // idMaterial
    //
    public abstract class idMaterial
    {
        public abstract string GetName();
    }

    //
    // idMaterialManager
    //
    public abstract class idMaterialManager
    {
        public abstract void Init();
        public abstract void Shutdown();
        public abstract idMaterial FindMaterial(string name, int lightmapIndex);
    }

    //
    // idImage
    //
    public abstract class idImage
    {
        public abstract string Name();
        public abstract void BlitImageData(ref Color[] data);
        public abstract void BlitImageData(ref byte[] data);
        public abstract int Width();
        public abstract int Height();
        public abstract void SetWrapState(SamplerState WrapClampMode);
        public abstract object GetDeviceHandle();
    }

    //
    // idImageManager
    //
    public abstract class idImageManager
    {
        public abstract void Init();
        public abstract void Shutdown();
        public abstract idImage FindImage(string qpath);
        public abstract idImage FindImageFile(string qpath, bool mipmap, bool picmap, SamplerState wrapClampMode);
        public abstract idImage CreateImage(string name, Color[] pic, int width, int height, bool mipmap, bool allowPicmip, SamplerState WrapClampMode);
        public abstract idImage CreateDXTImage(string name, byte[] pic, int width, int height, bool mipmap, bool allowPicmip, SamplerState WrapClampMode);
        public abstract void DestroyImage(ref idImage image);
    }

    //
    // idDrawVertex
    //
    public struct idDrawVertex
    {
        public idVector3 xyz;
        public idVector2 st;
        public idVector2 lightmapST;
        public idVector3 tangent;
        public idVector3 binormal;
        public idVector3 normal;
        public idVector4 color;

        public idDrawVertex(int unused)
        {
            xyz = new idVector3();
            st = new idVector2();
            lightmapST = new idVector2();
            tangent = new idVector3();
            binormal = new idVector3();
            normal = new idVector3();
            color = new idVector4(255, 255, 255, 255);
        }
    }

    public static class idColor
    {
        public const char Q_COLOR_ESCAPE = '^';

        public static bool IsColorString( string s, int basePos )
        {
            if( s == null || s.Length < 1 )
            {
                return false;
            }

            if( s[basePos + 0] != Q_COLOR_ESCAPE )
            {
                return false;
            }

            if( s[basePos + 1] == Q_COLOR_ESCAPE )
            {
                return false;
            }

            return true;
        }

        public const char COLOR_BLACK   =  '0';
        public const char COLOR_RED     =  '1';
        public const char COLOR_GREEN   =  '2';
        public const char COLOR_YELLOW  =  '3';
        public const char COLOR_BLUE    =  '4';
        public const char COLOR_CYAN    =  '5';
        public const char COLOR_MAGENTA =  '6';
        public const char COLOR_WHITE   =  '7';

        //
        // ColorIndex
        //
        public static int ColorIndex( char c )
        {
            return ( ( ( c ) - '0' ) & 7 );
        }

        public const string S_COLOR_BLACK  = "^0";
        public const string S_COLOR_RED    = "^1";
        public const string S_COLOR_GREEN  = "^2";
        public const string S_COLOR_YELLOW = "^3";
        public const string S_COLOR_BLUE   = "^4";
        public const string S_COLOR_CYAN   = "^5";
        public const string S_COLOR_MAGENTA = "^6";
        public const string S_COLOR_WHITE   = "^7";

        public static idVector4[] g_color_table = new idVector4[8]
        {
	        new idVector4(0.0f, 0.0f, 0.0f, 1.0f),
	        new idVector4(1.0f, 0.0f, 0.0f, 1.0f),
	        new idVector4(0.0f, 1.0f, 0.0f, 1.0f),
	        new idVector4(1.0f, 1.0f, 0.0f, 1.0f),
	        new idVector4(0.0f, 0.0f, 1.0f, 1.0f),
	        new idVector4(0.0f, 1.0f, 1.0f, 1.0f),
	        new idVector4(1.0f, 0.0f, 1.0f, 1.0f),
	        new idVector4(1.0f, 1.0f, 1.0f, 1.0f)
        };
    }

    //
    // idModel
    //
    public abstract class idModel
    {
        public abstract string GetName();
        public abstract void GetModelBounds(out idVector3 mins, out idVector3 maxs);
        public abstract int GetNumFrames();
        public abstract int GetTag(string name, int startframe, int endframe, int index, ref idOrientation orientation);
    }

    //
    // idModelManager
    //
    public abstract class idModelManager
    {
        // Initilizes the model manager.
        public abstract void Init();

        // Shutsdown the model manager.
        public abstract void Shutdown();

        // Loads a model.
        public abstract idModel LoadModel(string name);

        
    }

    //
    // idRenderSystem
    //
    public abstract class idRenderSystem
    {
        public const int MAX_CORONAS  =   32;          //----(SA)	not really a reason to limit this other than trying to keep a reasonable count
        public const int MAX_DLIGHTS  =   32;          // can't be increased, because bit flags are used on surfaces
        public const int MAX_ENTITIES = 1023;        // can't be increased without changing drawsurf bit packing

        public abstract void Init();
        public abstract int GetViewportWidth();
        public abstract int GetViewportHeight();
        public abstract idVideo LoadVideo(string filename);
        public abstract void BeginFrame();
        public abstract void EndFrame();
        public abstract void DrawStrechPic(int x, int y, int width, int height, idImage image);
        public abstract idFont RegisterFont(string name, int pointSize);
        public abstract void SetColor(float r, float g, float b, float a);
        public abstract void DrawStretchPic(float x, float y, float w, float h, float s1, float t1, float s2, float t2, idMaterial material);
        public abstract idWorld AllocWorld();
        public abstract idWorld LoadWorld(string mappath);
        public abstract idSkin LoadSkin(string skinpath);
    }
}
