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

// Render_Main.cs (c) 2010 JV Software
//

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using idLib.Engine.Public;
using idLib.Math;

using rtcw.Framework;
using rtcw.Renderer.Backend;
using rtcw.Renderer.Images;
using rtcw.Renderer.Models;
using rtcw.Renderer.Map;
namespace rtcw.Renderer
{
    //
    // Globals
    //
    static class Globals
    {
        public static idRenderGlobals tr;
        public static idRenderBackend backEnd;
        public static shaderCommands_t tess;

        public static GraphicsDevice graphics3DDevice;
        public static GraphicsDeviceManager graphicsManager;

        public static idCVar r_flareSize;
        public static idCVar r_flareFade;

        public static idCVar r_railWidth;
        public static idCVar r_railCoreWidth;
        public static idCVar r_railSegmentLength;

        public static idCVar r_ignoreFastPath;

        public static idCVar r_verbose;
        public static idCVar r_ignore;

        public static idCVar r_displayRefresh;

        public static idCVar r_detailTextures;

        public static idCVar r_znear;
        public static idCVar r_zfar;

        public static idCVar r_smp;
        public static idCVar r_showSmp;
        public static idCVar r_skipBackEnd;

        public static idCVar r_ignorehwgamma;
        public static idCVar r_measureOverdraw;

        public static idCVar r_inGameVideo;
        public static idCVar r_fastsky;
        public static idCVar r_drawSun;
        public static idCVar r_dynamiclight;
        public static idCVar r_dlightBacks;
        public static idCVar r_dlightScale; //----(SA)	added

        public static idCVar r_waterFogColor;   //----(SA)	added
        public static idCVar r_mapFogColor;
        public static idCVar r_savegameFogColor;    //----(SA)	added

        public static idCVar r_lodbias;
        public static idCVar r_lodscale;

        public static idCVar r_norefresh;
        public static idCVar r_drawentities;
        public static idCVar r_drawworld;
        public static idCVar r_speeds;
        public static idCVar r_fullbright;
        public static idCVar r_novis;
        public static idCVar r_nocull;
        public static idCVar r_facePlaneCull;
        public static idCVar r_showcluster;
        public static idCVar r_nocurves;

        public static idCVar r_allowExtensions;

        public static idCVar r_ext_compressed_textures;
        public static idCVar r_ext_gamma_control;
        public static idCVar r_ext_multitexture;
        public static idCVar r_ext_compiled_vertex_array;
        public static idCVar r_ext_texture_env_add;

        //----(SA)	added
        public static idCVar r_ext_texture_filter_anisotropic;

        public static idCVar r_ext_NV_fog_dist;
        public static idCVar r_nv_fogdist_mode;

        public static idCVar r_ext_ATI_pntriangles;
        public static idCVar r_ati_truform_tess;        //
        public static idCVar r_ati_truform_normalmode;  // linear/quadratic
        public static idCVar r_ati_truform_pointmode;   // linear/cubic
        //----(SA)	end

        public static idCVar r_ati_fsaa_samples;        //DAJ valids are 1, 2, 4

        public static idCVar r_ignoreGLErrors;
        public static idCVar r_logFile;

        public static idCVar r_stencilbits;
        public static idCVar r_depthbits;
        public static idCVar r_colorbits;
        public static idCVar r_stereo;
        public static idCVar r_primitives;
        public static idCVar r_texturebits;

        public static idCVar r_drawBuffer;
        //public static idCVar r_glDriver;
        //public static idCVar r_glIgnoreWicked3D;
        public static idCVar r_lightmap;
        public static idCVar r_vertexLight;
        public static idCVar r_uiFullScreen;
        public static idCVar r_shadows;
        public static idCVar r_portalsky;   //----(SA)	added
        public static idCVar r_flares;
        public static idCVar r_mode;
        public static idCVar r_nobind;
        public static idCVar r_singleShader;
        public static idCVar r_roundImagesDown;
        public static idCVar r_lowMemTextureSize;
        public static idCVar r_lowMemTextureThreshold;
        public static idCVar r_colorMipLevels;
        public static idCVar r_picmip;
        public static idCVar r_picmip2;
        public static idCVar r_showtris;
        public static idCVar r_showsky;
        public static idCVar r_shownormals;
        public static idCVar r_finish;
        public static idCVar r_clear;
        public static idCVar r_swapInterval;
        public static idCVar r_textureMode;
        public static idCVar r_offsetFactor;
        public static idCVar r_offsetUnits;
        public static idCVar r_gamma;
        public static idCVar r_intensity;
        public static idCVar r_lockpvs;
        public static idCVar r_noportals;
        public static idCVar r_portalOnly;

        public static idCVar r_subdivisions;
        public static idCVar r_lodCurveError;

        public static idCVar r_fullscreen;

        public static idCVar r_customwidth;
        public static idCVar r_customheight;
        public static idCVar r_customaspect;

        public static idCVar r_overBrightBits;
        public static idCVar r_mapOverBrightBits;

        public static idCVar r_debugSurface;
        public static idCVar r_simpleMipMaps;

        public static idCVar r_showImages;

        public static idCVar r_ambientScale;
        public static idCVar r_directedScale;
        public static idCVar r_debugLight;
        public static idCVar r_debugSort;
        public static idCVar r_printShaders;
        public static idCVar r_saveFontData;

        // Ridah
        public static idCVar r_cache;
        public static idCVar r_cacheShaders;
        public static idCVar r_cacheModels;
        public static idCVar r_compressModels;
        public static idCVar r_exportCompressedModels;

        public static idCVar r_cacheGathering;

        public static idCVar r_buildScript;

        public static idCVar r_bonesDebug;
        // done.

        // Rafael - wolf fog
        public static idCVar r_wolffog;
        // done

        public static idCVar r_highQualityVideo;
        public static idCVar r_rmse;

        public static idCVar r_maxpolys;
        public static int max_polys;
        public static idCVar r_maxpolyverts;
        public static int max_polyverts;

        public const int GLS_SRCBLEND_ZERO = 0x00000001;
        public const int GLS_SRCBLEND_ONE                  =      0x00000002;
        public const int GLS_SRCBLEND_DST_COLOR            =      0x00000003;
        public const int GLS_SRCBLEND_ONE_MINUS_DST_COLOR  =      0x00000004;
        public const int GLS_SRCBLEND_SRC_ALPHA            =      0x00000005;
        public const int GLS_SRCBLEND_ONE_MINUS_SRC_ALPHA  =      0x00000006;
        public const int GLS_SRCBLEND_DST_ALPHA            =      0x00000007;
        public const int GLS_SRCBLEND_ONE_MINUS_DST_ALPHA  =      0x00000008;
        public const int GLS_SRCBLEND_ALPHA_SATURATE       =      0x00000009;
        public const int GLS_SRCBLEND_BITS                 =  0x0000000f;

        public const int GLS_DSTBLEND_ZERO  =                     0x00000010;
        public const int GLS_DSTBLEND_ONE  =                      0x00000020;
        public const int GLS_DSTBLEND_SRC_COLOR =                 0x00000030;
        public const int GLS_DSTBLEND_ONE_MINUS_SRC_COLOR  =      0x00000040;
        public const int GLS_DSTBLEND_SRC_ALPHA   =               0x00000050;
        public const int GLS_DSTBLEND_ONE_MINUS_SRC_ALPHA  =      0x00000060;
        public const int GLS_DSTBLEND_DST_ALPHA  =                0x00000070;
        public const int GLS_DSTBLEND_ONE_MINUS_DST_ALPHA =       0x00000080;
        public const int GLS_DSTBLEND_BITS    =               0x000000f0;

        public const int GLS_DEPTHMASK_TRUE  =                    0x00000100;

        public const int GLS_POLYMODE_LINE =                      0x00001000;

        public const int GLS_DEPTHTEST_DISABLE   =                0x00010000;
        public const int GLS_DEPTHFUNC_EQUAL  =                   0x00020000;

        public const int GLS_FOG_DISABLE    =                     0x00020000;  //----(SA)	added

        public const int GLS_ATEST_GT_0      =                    0x10000000;
        public const int GLS_ATEST_LT_80     =                    0x20000000;
        public const int GLS_ATEST_GE_80     =                    0x40000000;
        public const int  GLS_ATEST_BITS     =                 0x70000000;

        public const int GLS_DEFAULT    =     GLS_DEPTHMASK_TRUE;

        public static int colorBits = 32;

        //
        // SetVertexIndexBuffers
        //
        public static void SetVertexIndexBuffers( VertexBuffer vertexBuffer, IndexBuffer indexBuffer )
        {
            idRenderCommand cmd = backEnd.GetCommandBuffer();

            cmd.type = renderCommandType.RC_SET_VERTEXINDEXBUFFER;
            cmd.vertexBuffer = vertexBuffer;
            cmd.indexBuffer = indexBuffer;
        }

        //
        // AllocModelBrush
        //
        public static idModelBrush AllocModelBrush(string name)
        {
            return ((idModelManagerLocal)Engine.modelManager).AllocBrushModel(name);
        }

        //
        // UpdateLoadingScreen
        //
        public static void UpdateLoadingScreen()
        {
            ((idCommonLocal)Engine.common).UpdateLoadingScreen();
        }

        //
        // SortSurfaces
        //
        public static void SortSurfaces<T>(int vertexOffset, ref T[] surfaces) where T : idDrawSurface
        {
            idRenderCommand cmd = backEnd.GetCommandBuffer();

            cmd.type = renderCommandType.RC_DRAW_SURFS;

            cmd.firstDrawSurf = backEnd.NumSurfaces;
            cmd.numDrawSurfs = surfaces.Length;
            cmd.vertexOffset = vertexOffset;

            for (int i = 0; i < cmd.numDrawSurfs; i++)
            {
                backEnd.AddDrawSurface( (idDrawSurface)surfaces[i] );
            }
        }
    }

    //
    // idRenderSystemLocal
    //
    public class idRenderSystemLocal : idRenderSystem
    {
        private GraphicsDevice _graphicsDevice;

        //
        // idRenderSystemLocal
        //
        public idRenderSystemLocal(GraphicsDeviceManager xnaDevice)
        {
            _graphicsDevice = xnaDevice.GraphicsDevice;
            Globals.graphicsManager = xnaDevice;
            Globals.graphics3DDevice = _graphicsDevice;
        }

        /*
        =================
        R_InitFogTable
        =================
        */
        private void InitFogTable() {
	        int i;
	        float d;
	        float exp;

	        exp = 0.5f;

	        for ( i = 0 ; i < idRenderGlobals.FOG_TABLE_SIZE ; i++ ) {
                d = (float)System.Math.Pow((float)i / (idRenderGlobals.FOG_TABLE_SIZE - 1), exp);

		        Globals.tr.fogTable[i] = d;
	        }
        }

        //
        // Register
        //
        private void Register()
        {
            //
	        // latched and archived variables
	        //
	        //Globals.r_glDriver = Engine.cvarManager.Cvar_Get( "r_glDriver", OPENGL_DRIVER_NAME, idCVar.CVAR_ARCHIVE | idCVar.CVAR_LATCH );
	        Globals.r_allowExtensions = Engine.cvarManager.Cvar_Get( "r_allowExtensions", "1", idCVar.CVAR_ARCHIVE | idCVar.CVAR_LATCH );
	        Globals.r_ext_compressed_textures = Engine.cvarManager.Cvar_Get( "r_ext_compressed_textures", "1", idCVar.CVAR_ARCHIVE | idCVar.CVAR_LATCH );   // (SA) ew, a spelling change I missed from the missionpack
	        Globals.r_ext_gamma_control = Engine.cvarManager.Cvar_Get( "r_ext_gamma_control", "1", idCVar.CVAR_ARCHIVE | idCVar.CVAR_LATCH );
	        Globals.r_ext_multitexture = Engine.cvarManager.Cvar_Get( "r_ext_multitexture", "1", idCVar.CVAR_ARCHIVE | idCVar.CVAR_LATCH );
	        Globals.r_ext_compiled_vertex_array = Engine.cvarManager.Cvar_Get( "r_ext_compiled_vertex_array", "1", idCVar.CVAR_ARCHIVE | idCVar.CVAR_LATCH );
	        //Globals.r_glIgnoreWicked3D = Engine.cvarManager.Cvar_Get( "r_glIgnoreWicked3D", "0", idCVar.CVAR_ARCHIVE | idCVar.CVAR_LATCH );

	        Globals.r_picmip = Engine.cvarManager.Cvar_Get( "r_picmip", "1", idCVar.CVAR_ARCHIVE | idCVar.CVAR_LATCH );
	        Globals.r_picmip2 = Engine.cvarManager.Cvar_Get( "r_picmip2", "2", idCVar.CVAR_ARCHIVE | idCVar.CVAR_LATCH );   // used for character skins picmipping at a different level from the rest of the game
	        Globals.r_roundImagesDown = Engine.cvarManager.Cvar_Get( "r_roundImagesDown", "1", idCVar.CVAR_ARCHIVE | idCVar.CVAR_LATCH );
	        Globals.r_lowMemTextureSize = Engine.cvarManager.Cvar_Get( "r_lowMemTextureSize", "0", idCVar.CVAR_ARCHIVE | idCVar.CVAR_LATCH );
	        Globals.r_lowMemTextureThreshold = Engine.cvarManager.Cvar_Get( "r_lowMemTextureThreshold", "15.0", idCVar.CVAR_ARCHIVE | idCVar.CVAR_LATCH );
	        Globals.r_rmse = Engine.cvarManager.Cvar_Get( "r_rmse", "0.0", idCVar.CVAR_ARCHIVE | idCVar.CVAR_LATCH );
	        Globals.r_colorMipLevels = Engine.cvarManager.Cvar_Get( "r_colorMipLevels", "0", idCVar.CVAR_LATCH );
	        //AssertCvarRange( r_picmip, 0, 16, qtrue );
	        //AssertCvarRange( r_picmip2, 0, 16, qtrue );
	        Globals.r_detailTextures = Engine.cvarManager.Cvar_Get( "r_detailtextures", "1", idCVar.CVAR_ARCHIVE | idCVar.CVAR_LATCH );
	        Globals.r_texturebits = Engine.cvarManager.Cvar_Get( "r_texturebits", "0", idCVar.CVAR_ARCHIVE | idCVar.CVAR_LATCH );
	        Globals.r_colorbits = Engine.cvarManager.Cvar_Get( "r_colorbits", "0", idCVar.CVAR_ARCHIVE | idCVar.CVAR_LATCH );
	        Globals.r_stereo = Engine.cvarManager.Cvar_Get( "r_stereo", "0", idCVar.CVAR_ARCHIVE | idCVar.CVAR_LATCH );

	        Globals.r_stencilbits = Engine.cvarManager.Cvar_Get( "r_stencilbits", "8", idCVar.CVAR_ARCHIVE | idCVar.CVAR_LATCH );

            Globals.r_depthbits = Engine.cvarManager.Cvar_Get( "r_depthbits", "0", idCVar.CVAR_ARCHIVE | idCVar.CVAR_LATCH );
	        Globals.r_overBrightBits = Engine.cvarManager.Cvar_Get( "r_overBrightBits", "1", idCVar.CVAR_ARCHIVE | idCVar.CVAR_LATCH );
	        Globals.r_ignorehwgamma = Engine.cvarManager.Cvar_Get( "r_ignorehwgamma", "1", idCVar.CVAR_ARCHIVE | idCVar.CVAR_LATCH );    //----(SA) changed this to default to '1' for Drew
	        Globals.r_mode = Engine.cvarManager.Cvar_Get( "r_mode", "-1", idCVar.CVAR_ARCHIVE | idCVar.CVAR_LATCH );
	        Globals.r_fullscreen = Engine.cvarManager.Cvar_Get( "r_fullscreen", "0", idCVar.CVAR_ARCHIVE | idCVar.CVAR_LATCH );
	        Globals.r_customwidth = Engine.cvarManager.Cvar_Get( "r_customwidth", "" + GetViewportWidth(), idCVar.CVAR_ARCHIVE | idCVar.CVAR_LATCH );
	        Globals.r_customheight = Engine.cvarManager.Cvar_Get( "r_customheight", "" + GetViewportHeight(), idCVar.CVAR_ARCHIVE | idCVar.CVAR_LATCH );
	        Globals.r_customaspect = Engine.cvarManager.Cvar_Get( "r_customaspect", "1", idCVar.CVAR_ARCHIVE | idCVar.CVAR_LATCH );
	        Globals.r_simpleMipMaps = Engine.cvarManager.Cvar_Get( "r_simpleMipMaps", "1", idCVar.CVAR_ARCHIVE | idCVar.CVAR_LATCH );
	        Globals.r_vertexLight = Engine.cvarManager.Cvar_Get( "r_vertexLight", "0", idCVar.CVAR_ARCHIVE | idCVar.CVAR_LATCH );
	        Globals.r_uiFullScreen = Engine.cvarManager.Cvar_Get( "r_uifullscreen", "0", 0 );
	        Globals.r_subdivisions = Engine.cvarManager.Cvar_Get( "r_subdivisions", "4", idCVar.CVAR_ARCHIVE | idCVar.CVAR_LATCH );

	        Globals.r_smp = Engine.cvarManager.Cvar_Get( "r_smp", "0", idCVar.CVAR_ARCHIVE | idCVar.CVAR_LATCH );

	        Globals.r_ignoreFastPath = Engine.cvarManager.Cvar_Get( "r_ignoreFastPath", "1", idCVar.CVAR_ARCHIVE | idCVar.CVAR_LATCH );

	        //
	        // temporary latched variables that can only change over a restart
	        //
	        Globals.r_displayRefresh = Engine.cvarManager.Cvar_Get( "r_displayRefresh", "0", idCVar.CVAR_LATCH );
	        //AssertCvarRange( r_displayRefresh, 0, 200, qtrue );
	        Globals.r_fullbright = Engine.cvarManager.Cvar_Get( "r_fullbright", "0", idCVar.CVAR_LATCH | idCVar.CVAR_CHEAT );
	        Globals.r_mapOverBrightBits = Engine.cvarManager.Cvar_Get( "r_mapOverBrightBits", "2", idCVar.CVAR_LATCH );
	        Globals.r_intensity = Engine.cvarManager.Cvar_Get( "r_intensity", "1", idCVar.CVAR_LATCH );
	        Globals.r_singleShader = Engine.cvarManager.Cvar_Get( "r_singleShader", "0", idCVar.CVAR_CHEAT | idCVar.CVAR_LATCH );

	        //
	        // archived variables that can change at any time
	        //
	        Globals.r_lodCurveError = Engine.cvarManager.Cvar_Get( "r_lodCurveError", "250", idCVar.CVAR_ARCHIVE );
	        Globals.r_lodbias = Engine.cvarManager.Cvar_Get( "r_lodbias", "0", idCVar.CVAR_ARCHIVE );
	        Globals.r_flares = Engine.cvarManager.Cvar_Get( "r_flares", "1", idCVar.CVAR_ARCHIVE );
	        Globals.r_znear = Engine.cvarManager.Cvar_Get( "r_znear", "4", idCVar.CVAR_CHEAT );
	        //AssertCvarRange( r_znear, 0.001f, 200, qtrue );
        //----(SA)	added
	        Globals.r_zfar = Engine.cvarManager.Cvar_Get( "r_zfar", "0", idCVar.CVAR_CHEAT );
        //----(SA)	end
	        Globals.r_ignoreGLErrors = Engine.cvarManager.Cvar_Get( "r_ignoreGLErrors", "1", idCVar.CVAR_ARCHIVE );
	        Globals.r_fastsky = Engine.cvarManager.Cvar_Get( "r_fastsky", "0", idCVar.CVAR_ARCHIVE );
	        Globals.r_inGameVideo = Engine.cvarManager.Cvar_Get( "r_inGameVideo", "1", idCVar.CVAR_ARCHIVE );
	        Globals.r_drawSun = Engine.cvarManager.Cvar_Get( "r_drawSun", "1", idCVar.CVAR_ARCHIVE );
	        Globals.r_dynamiclight = Engine.cvarManager.Cvar_Get( "r_dynamiclight", "1", idCVar.CVAR_ARCHIVE );
	        Globals.r_dlightScale = Engine.cvarManager.Cvar_Get( "r_dlightScale", "1.0", idCVar.CVAR_ARCHIVE );   //----(SA)	added
	        Globals.r_dlightBacks = Engine.cvarManager.Cvar_Get( "r_dlightBacks", "1", idCVar.CVAR_ARCHIVE );
	        Globals.r_finish = Engine.cvarManager.Cvar_Get( "r_finish", "0", idCVar.CVAR_ARCHIVE );
	        Globals.r_textureMode = Engine.cvarManager.Cvar_Get( "r_textureMode", "GL_LINEAR_MIPMAP_NEAREST", idCVar.CVAR_ARCHIVE );
	        Globals.r_swapInterval = Engine.cvarManager.Cvar_Get( "r_swapInterval", "0", idCVar.CVAR_ARCHIVE );

            Globals.r_gamma = Engine.cvarManager.Cvar_Get("r_gamma", "1.3", idCVar.CVAR_ARCHIVE);

            Globals.r_facePlaneCull = Engine.cvarManager.Cvar_Get("r_facePlaneCull", "1", idCVar.CVAR_ARCHIVE);

            Globals.r_railWidth = Engine.cvarManager.Cvar_Get("r_railWidth", "16", idCVar.CVAR_ARCHIVE);
            Globals.r_railCoreWidth = Engine.cvarManager.Cvar_Get("r_railCoreWidth", "1", idCVar.CVAR_ARCHIVE);
            Globals.r_railSegmentLength = Engine.cvarManager.Cvar_Get("r_railSegmentLength", "32", idCVar.CVAR_ARCHIVE);

            Globals.r_waterFogColor = Engine.cvarManager.Cvar_Get("r_waterFogColor", "0", idCVar.CVAR_ROM);  //----(SA)	added
            Globals.r_mapFogColor = Engine.cvarManager.Cvar_Get("r_mapFogColor", "0", idCVar.CVAR_ROM);  //----(SA)	added
            Globals.r_savegameFogColor = Engine.cvarManager.Cvar_Get("r_savegameFogColor", "0", idCVar.CVAR_ROM);    //----(SA)	added


            Globals.r_primitives = Engine.cvarManager.Cvar_Get("r_primitives", "0", idCVar.CVAR_ARCHIVE);

            Globals.r_ambientScale = Engine.cvarManager.Cvar_Get("r_ambientScale", "0.5", idCVar.CVAR_CHEAT);
            Globals.r_directedScale = Engine.cvarManager.Cvar_Get("r_directedScale", "1", idCVar.CVAR_CHEAT);

	        //
	        // temporary variables that can change at any time
	        //
            Globals.r_showImages = Engine.cvarManager.Cvar_Get("r_showImages", "0", idCVar.CVAR_TEMP);

            Globals.r_debugLight = Engine.cvarManager.Cvar_Get("r_debuglight", "0", idCVar.CVAR_TEMP);
            Globals.r_debugSort = Engine.cvarManager.Cvar_Get("r_debugSort", "0", idCVar.CVAR_CHEAT);
            Globals.r_printShaders = Engine.cvarManager.Cvar_Get("r_printShaders", "0", 0);
            Globals.r_saveFontData = Engine.cvarManager.Cvar_Get("r_saveFontData", "0", 0);

	        // Ridah
	        // show_bug.cgi?id=440
	        // NOTE TTimo: r_cache is disabled by default in SP
            Engine.cvarManager.Cvar_Set("r_cache", "0", true);
            Globals.r_cache = Engine.cvarManager.Cvar_Get("r_cache", "1", idCVar.CVAR_LATCH);  // leaving it as this for backwards compability. but it caches models and shaders also
        // (SA) disabling cacheshaders
            Engine.cvarManager.Cvar_Set("r_cacheShaders", "0", true);
            Globals.r_cacheShaders = Engine.cvarManager.Cvar_Get("r_cacheShaders", "0", idCVar.CVAR_LATCH);
        //----(SA)	end

            Globals.r_cacheModels = Engine.cvarManager.Cvar_Get("r_cacheModels", "1", idCVar.CVAR_LATCH);
            Globals.r_compressModels = Engine.cvarManager.Cvar_Get("r_compressModels", "0", 0);     // converts MD3 -> MDC at run-time
            Globals.r_exportCompressedModels = Engine.cvarManager.Cvar_Get("r_exportCompressedModels", "0", 0); // saves compressed models
            Globals.r_cacheGathering = Engine.cvarManager.Cvar_Get("cl_cacheGathering", "0", 0);
            Globals.r_buildScript = Engine.cvarManager.Cvar_Get("com_buildscript", "0", 0);
            Globals.r_bonesDebug = Engine.cvarManager.Cvar_Get("r_bonesDebug", "0", idCVar.CVAR_CHEAT);
	        // done.

	        // Rafael - wolf fog
            Globals.r_wolffog = Engine.cvarManager.Cvar_Get("r_wolffog", "1", 0);
	        // done

            Globals.r_nocurves = Engine.cvarManager.Cvar_Get("r_nocurves", "0", idCVar.CVAR_CHEAT);
            Globals.r_drawworld = Engine.cvarManager.Cvar_Get("r_drawworld", "1", idCVar.CVAR_CHEAT);
            Globals.r_lightmap = Engine.cvarManager.Cvar_Get("r_lightmap", "0", idCVar.CVAR_CHEAT);
            Globals.r_portalOnly = Engine.cvarManager.Cvar_Get("r_portalOnly", "0", idCVar.CVAR_CHEAT);

            Globals.r_flareSize = Engine.cvarManager.Cvar_Get("r_flareSize", "40", idCVar.CVAR_CHEAT);
            Globals.r_flareFade = Engine.cvarManager.Cvar_Get("r_flareFade", "5", idCVar.CVAR_CHEAT);

            Globals.r_showSmp = Engine.cvarManager.Cvar_Get("r_showSmp", "0", idCVar.CVAR_CHEAT);
            Globals.r_skipBackEnd = Engine.cvarManager.Cvar_Get("r_skipBackEnd", "0", idCVar.CVAR_CHEAT);

            Globals.r_measureOverdraw = Engine.cvarManager.Cvar_Get("r_measureOverdraw", "0", idCVar.CVAR_CHEAT);
            Globals.r_lodscale = Engine.cvarManager.Cvar_Get("r_lodscale", "5", idCVar.CVAR_CHEAT);
            Globals.r_norefresh = Engine.cvarManager.Cvar_Get("r_norefresh", "0", idCVar.CVAR_CHEAT);
            Globals.r_drawentities = Engine.cvarManager.Cvar_Get("r_drawentities", "1", idCVar.CVAR_CHEAT);
            Globals.r_ignore = Engine.cvarManager.Cvar_Get("r_ignore", "1", idCVar.CVAR_CHEAT);
            Globals.r_nocull = Engine.cvarManager.Cvar_Get("r_nocull", "0", idCVar.CVAR_CHEAT);
            Globals.r_novis = Engine.cvarManager.Cvar_Get("r_novis", "0", idCVar.CVAR_CHEAT);
            Globals.r_showcluster = Engine.cvarManager.Cvar_Get("r_showcluster", "0", idCVar.CVAR_CHEAT);
            Globals.r_speeds = Engine.cvarManager.Cvar_Get("r_speeds", "0", idCVar.CVAR_CHEAT);
            Globals.r_verbose = Engine.cvarManager.Cvar_Get("r_verbose", "0", idCVar.CVAR_CHEAT);
            Globals.r_logFile = Engine.cvarManager.Cvar_Get("r_logFile", "0", idCVar.CVAR_CHEAT);
            Globals.r_debugSurface = Engine.cvarManager.Cvar_Get("r_debugSurface", "0", idCVar.CVAR_CHEAT);
            Globals.r_nobind = Engine.cvarManager.Cvar_Get("r_nobind", "0", idCVar.CVAR_CHEAT);
            Globals.r_showtris = Engine.cvarManager.Cvar_Get("r_showtris", "0", idCVar.CVAR_CHEAT);
            Globals.r_showsky = Engine.cvarManager.Cvar_Get("r_showsky", "0", idCVar.CVAR_CHEAT);
            Globals.r_shownormals = Engine.cvarManager.Cvar_Get("r_shownormals", "0", idCVar.CVAR_CHEAT);
            Globals.r_clear = Engine.cvarManager.Cvar_Get("r_clear", "0", idCVar.CVAR_CHEAT);
            Globals.r_offsetFactor = Engine.cvarManager.Cvar_Get("r_offsetfactor", "-1", idCVar.CVAR_CHEAT);
            Globals.r_offsetUnits = Engine.cvarManager.Cvar_Get("r_offsetunits", "-2", idCVar.CVAR_CHEAT);
            Globals.r_drawBuffer = Engine.cvarManager.Cvar_Get("r_drawBuffer", "GL_BACK", idCVar.CVAR_CHEAT);
            Globals.r_lockpvs = Engine.cvarManager.Cvar_Get("r_lockpvs", "0", idCVar.CVAR_CHEAT);
            Globals.r_noportals = Engine.cvarManager.Cvar_Get("r_noportals", "0", idCVar.CVAR_CHEAT);
            Globals.r_shadows = Engine.cvarManager.Cvar_Get("cg_shadows", "1", 0);
            Globals.r_shadows = Engine.cvarManager.Cvar_Get("cg_shadows", "1", 0);
            Globals.r_portalsky = Engine.cvarManager.Cvar_Get("cg_skybox", "1", 0);

            Globals.r_maxpolys = Engine.cvarManager.Cvar_Get("r_maxpolys", "" + idRenderGlobals.MAX_POLYS, 0);
            Globals.r_maxpolyverts = Engine.cvarManager.Cvar_Get("r_maxpolyverts", "" + idRenderGlobals.MAX_POLYVERTS, 0);

            Globals.r_highQualityVideo = Engine.cvarManager.Cvar_Get("r_highQualityVideo", "1", idCVar.CVAR_ARCHIVE);
        }


        //
        // Init
        //
        public override void Init()
        {
            Engine.common.Printf("----- R_Init -----\n");

            // clear all our internal state
            Globals.tr = new idRenderGlobals();
            Globals.backEnd = new idRenderBackend();
            Globals.tess = new shaderCommands_t();

            for( int i = 0; i < Globals.tess.constantColor255.Length; i++ )
            {
                Globals.tess.constantColor255[i] = new idVector4(255, 255, 255, 255);
            }

            //
            // init function tables
            //
            for (int i = 0; i < idRenderGlobals.FUNCTABLE_SIZE; i++)
            {
                Globals.tr.sinTable[i] = (float)System.Math.Sin(idMath.DEG2RAD(i * 360.0f / ((float)(idRenderGlobals.FUNCTABLE_SIZE - 1))));
                Globals.tr.squareTable[i] = (i < idRenderGlobals.FUNCTABLE_SIZE / 2) ? 1.0f : -1.0f;
                Globals.tr.sawToothTable[i] = (float)i / idRenderGlobals.FUNCTABLE_SIZE;
                Globals.tr.inverseSawToothTable[i] = 1.0f - Globals.tr.sawToothTable[i];

                if (i < idRenderGlobals.FUNCTABLE_SIZE / 2)
                {
                    if (i < idRenderGlobals.FUNCTABLE_SIZE / 4)
                    {
                        Globals.tr.triangleTable[i] = (float)i / (idRenderGlobals.FUNCTABLE_SIZE / 4);
                    }
                    else
                    {
                        Globals.tr.triangleTable[i] = 1.0f - Globals.tr.triangleTable[i - idRenderGlobals.FUNCTABLE_SIZE / 4];
                    }
                }
                else
                {
                    Globals.tr.triangleTable[i] = -Globals.tr.triangleTable[i - idRenderGlobals.FUNCTABLE_SIZE / 2];
                }
            }

            InitFogTable();

            Noise.Init();

            Register();

            Engine.imageManager = new idImageManagerLocal();
            Engine.imageManager.Init();

            Engine.materialManager = new idMaterialManagerLocal();
            Engine.materialManager.Init();

            Engine.common.Printf( "----- finished R_Init -----\n");
        }

        //
        // LoadVideo
        //
        public override idVideo LoadVideo(string filename)
        {
            idFile vidFile;
            filename = Engine.fileSystem.RemoveExtensionFromPath(filename);
            if (filename.Contains("video/") == false)
            {
                filename = "video/" + filename;
            }

            idFile movieFile = Engine.fileSystem.OpenFileRead(filename + ".roq", true);
            return new idVideoLocal( movieFile );
        }

        //
        // BeginFrame
        //
        public override void BeginFrame()
        {
        }

        //
        // EndFrame
        //
        public override void EndFrame()
        {
	        idRenderCommand cmd = Globals.backEnd.GetCommandBuffer();
	        if ( cmd == null ) {
		        return;
	        }
            cmd.type = renderCommandType.RC_SWAP_BUFFERS;

	        Globals.backEnd.IssueRenderCommands( true );

	        // use the other buffers next frame, because another CPU
	        // may still be rendering into the current ones
	        Globals.backEnd.ToggleSmpFrame();
        }

        //
        // AdjustFrom640
        //
        private void AdjustFrom640(ref float x, ref float y, ref float w, ref float h)
        {
            float yscale = GetViewportHeight() * (1.0f / 480.0f);
            float xscale = GetViewportWidth() * (1.0f / 640.0f);

            //*x = *x * DC->scale + DC->bias;
            x *= xscale;
            y *= yscale;
            w *= xscale;
            h *= yscale;
        }

        //
        // DrawStrechPic
        //
        public override void DrawStrechPic(int x, int y, int width, int height, idImage image)
        {
            idRenderCommand cmd;
            if (image == null)
            {
                return;
            }
            cmd = Globals.backEnd.GetCommandBuffer();

            cmd.type = renderCommandType.RC_STRETCH_IMAGE;
            cmd.x = x;
            cmd.y = y;
            cmd.w = width;
            cmd.h = height;
            cmd.image = image;

            //AdjustFrom640(ref cmd.x, ref cmd.y, ref cmd.w, ref cmd.h);
        }

        //
        // SetColor
        //
        public override void SetColor(float r, float g, float b, float a)
        {
            idRenderCommand cmd = Globals.backEnd.GetCommandBuffer();

            cmd.type = renderCommandType.RC_SET_COLOR;
            cmd.color[0] = r;
            cmd.color[1] = g;
            cmd.color[2] = b;
            cmd.color[3] = a;
        }

        //
        // DrawStretchPic
        //
        public override void DrawStretchPic(float x, float y, float w, float h, float s1, float t1, float s2, float t2, idMaterial material) 
        {

            idRenderCommand cmd;
            if (material == null)
            {
                return;
            }

            cmd = Globals.backEnd.GetCommandBuffer();
            cmd.type = renderCommandType.RC_STRETCH_PIC;
            cmd.shader = material;
	        cmd.x = x;
	        cmd.y = y;
	        cmd.w = w;
	        cmd.h = h;
	        cmd.s1 = s1;
	        cmd.t1 = t1;
	        cmd.s2 = s2;
	        cmd.t2 = t2;

            AdjustFrom640(ref cmd.x, ref cmd.y, ref cmd.w, ref cmd.h);
        }

        //
        // GetViewportWidth
        //
        public override int GetViewportWidth()
        {
            return _graphicsDevice.Viewport.Width;
        }

        //
        // GetViewportHeight
        //
        public override int GetViewportHeight()
        {
            return _graphicsDevice.Viewport.Height;
        }

        //
        // AllocWorld
        //
        public override idWorld AllocWorld()
        {
            idWorldLocal world = new idWorldLocal();

            Globals.tr.worlds.Add(world);

            return Globals.tr.worlds[Globals.tr.worlds.Count - 1];
        }

        //
        // LoadWorld
        //
        public override idWorld LoadWorld(string mappath)
        {
            idWorldLocal world = new idWorldLocal(mappath);

            Globals.tr.worlds.Add(world);

            return Globals.tr.worlds[Globals.tr.worlds.Count - 1];
        }

        //
        // LoadWorldEntityString
        //
        public override string LoadWorldEntityString(string mappath)
        {
            idMap map = new idMap();
            string entityString;

            entityString = map.LoadMapEntityString(mappath);

            map = null;
            return entityString;
        }

        //
        // RegisterFont
        //
        public override idFont RegisterFont(string filename, int pointSize)
        {
            idFont font;
	        int i;
	        string name;
	        float dpi = 72;                                         //
	        float glyphScale =  72.0f / dpi;        // change the scale to be relative to 1 based on 72 dpi ( so dpi of 144 means a scale of .5 )

	        if ( pointSize <= 0 ) {
		        pointSize = 12;
	        }
	        // we also need to adjust the scale based on point size relative to 48 points as the ui scaling is based on a 48 point font
	        glyphScale *= 48.0f / pointSize;

	        // make sure the render thread is stopped
	        //R_SyncRenderThread();

	        if ( Globals.tr.registeredFontCount >= idRenderGlobals.MAX_FONTS ) {
		        Engine.common.Warning("RE_RegisterFont: Too many fonts registered already.\n" );
		        return null;
	        }

            // jv - fix me this is stupid :/.
	        //Com_sprintf( name, sizeof( name ), "fonts/fontImage_%i.dat",pointSize );
            name = "fonts/fontImage_" + pointSize + ".dat";
	        for ( i = 0; i < Globals.tr.registeredFontCount; i++ ) {
		        if ( name == Globals.tr.registeredFont[i].name ) {
			        return Globals.tr.registeredFont[i];
		        }
	        }

            idFile _file = Engine.fileSystem.OpenFileRead( name, true );
            if(_file == null)
            {
                Engine.common.Warning( "R_RegisterFont: Failed to open font " + name + "\n" );
                return null;
            }
		   
            font = new idFont();
		    for ( i = 0; i < idFont.GLYPHS_PER_FONT; i++ ) {
                font.glyphs[i] = new glyphInfo_t();
                font.glyphs[i].height      = _file.ReadInt();
                font.glyphs[i].top         = _file.ReadInt();
                font.glyphs[i].bottom      = _file.ReadInt();
                font.glyphs[i].pitch       = _file.ReadInt();
                font.glyphs[i].xSkip       = _file.ReadInt();
                font.glyphs[i].imageWidth  = _file.ReadInt();
                font.glyphs[i].imageHeight = _file.ReadInt();
                font.glyphs[i].s           = _file.ReadFloat();
                font.glyphs[i].t           = _file.ReadFloat();
                font.glyphs[i].s2          = _file.ReadFloat();
                font.glyphs[i].t2          = _file.ReadFloat();
                _file.ReadInt(); // font.glyphs[i].glyph - placeholder not needed.
                font.glyphs[i].shaderName  = _file.ReadString( 32 );
                if (font.glyphs[i].shaderName.Length > 0)
                {
                    font.glyphs[i].glyph = Engine.materialManager.FindMaterial(font.glyphs[i].shaderName, -1);
                    //Engine.common.Printf("Loading Font Texture: " + font.glyphs[i].shaderName + "\n");
                }
		    }
		    font.glyphScale = _file.ReadFloat();
            font.name = name;

            Globals.tr.registeredFont[Globals.tr.registeredFontCount] = font;
            return Globals.tr.registeredFont[Globals.tr.registeredFontCount++];
        }
    }
}
