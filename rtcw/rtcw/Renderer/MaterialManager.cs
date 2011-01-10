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

// MaterialManager.cs (c) 2010 JV Software
//

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using idLib.Engine.Public;
using idLib.Engine.Content;
using idLib;
using idLib.Math;
using rtcw.Renderer.Backend.Iterators;

namespace rtcw.Renderer
{
    //
    // idMaterialTableReader
    //
    class idMaterialTableReader : ContentTypeReader<idMaterialLookupTable>
    {
        //
        // jvFileList
        //
        protected override idMaterialLookupTable Read(ContentReader input, idMaterialLookupTable existingInstance)
        {
            idMaterialLookupTable list = new idMaterialLookupTable();

            int numMaterials = input.ReadInt32();

            for (int i = 0; i < numMaterials; i++)
            {
                idMaterialCached mtrCached = new idMaterialCached();
                mtrCached.hashValue = input.ReadInt32();
                mtrCached.buffer = input.ReadString();
                list.AddMaterialToCache(mtrCached);
            }

            return list;
        }
    }

    //
    // idMaterialLocal
    //
    class idMaterialLocal : idMaterial
    {
        //
        // idMaterialLocal
        //
        public idMaterialLocal(int lightmapIndex)
        {
            shader = new idMaterialBase();
            shader.lightmapIndex = lightmapIndex;
        }

        // this table is also present in q3map
        public struct infoParm_t {
            public infoParm_t( string name, int clearSolid, int surfaceFlags, uint contents )
            {
                this.name = name;
                this.clearSolid = clearSolid;
                this.surfaceFlags = surfaceFlags;
                this.contents = contents;
            }
	        public string name;
	        public int clearSolid, surfaceFlags;
            public uint contents;
        };

        public static infoParm_t[] infoParms = new infoParm_t[]{
	        // server relevant contents

        //----(SA)	modified
	        new infoParm_t("clipmissile",  1,  0, surfaceFlags.CONTENTS_MISSILECLIP),       // impact only specific weapons (rl, gl)
        //----(SA)	end

        // RF, AI sight
	        new infoParm_t("ai_nosight",   1,  0,  surfaceFlags.CONTENTS_AI_NOSIGHT),
	        new infoParm_t("clipshot", 1,  0,  surfaceFlags.CONTENTS_CLIPSHOT),         // stops bullets
        // RF, end

	        new infoParm_t("water",        1,  0,  surfaceFlags.CONTENTS_WATER ),
	        new infoParm_t("slag",     1,  0,  surfaceFlags.CONTENTS_SLIME ),       // uses the CONTENTS_SLIME flag, but the shader reference is changed to 'slag'
	        // to idendify that this doesn't work the same as 'slime' did.
	        // (slime hurts instantly, slag doesn't)
        //	new infoParm_t("slime",		1,	0,	CONTENTS_SLIME ),		// mildly damaging
	        new infoParm_t("lava",     1,  0,  surfaceFlags.CONTENTS_LAVA ),        // very damaging
	        new infoParm_t("playerclip",   1,  0,  surfaceFlags.CONTENTS_PLAYERCLIP ),
	        new infoParm_t("monsterclip",  1,  0,  surfaceFlags.CONTENTS_MONSTERCLIP ),
	        new infoParm_t("nodrop",       1,  0,  surfaceFlags.CONTENTS_NODROP ),      // don't drop items or leave bodies (death fog, lava, etc)
	        new infoParm_t("nonsolid", 1,  surfaceFlags.SURF_NONSOLID,  0),                     // clears the solid flag

	        // utility relevant attributes
	        new infoParm_t("origin",       1,  0,  surfaceFlags.CONTENTS_ORIGIN ),      // center of rotating brushes
	        new infoParm_t("trans",        0,  0,  surfaceFlags.CONTENTS_TRANSLUCENT ), // don't eat contained surfaces
	        new infoParm_t("detail",       0,  0,  surfaceFlags.CONTENTS_DETAIL ),      // don't include in structural bsp
	        new infoParm_t("structural",   0,  0,  surfaceFlags.CONTENTS_STRUCTURAL ),  // force into structural bsp even if trnas
	        new infoParm_t("areaportal",   1,  0,  surfaceFlags.CONTENTS_AREAPORTAL ),  // divides areas
	        new infoParm_t("clusterportal", 1,0,  surfaceFlags.CONTENTS_CLUSTERPORTAL ),    // for bots
	        new infoParm_t("donotenter",  1,  0,  surfaceFlags.CONTENTS_DONOTENTER ),       // for bots

	        // Rafael - nopass
	        new infoParm_t("donotenterlarge", 1, 0,    surfaceFlags.CONTENTS_DONOTENTER_LARGE ), // for larger bots

	        new infoParm_t("fog",          1,  0,  surfaceFlags.CONTENTS_FOG),          // carves surfaces entering
	        new infoParm_t("sky",          0,  surfaceFlags.SURF_SKY,       0 ),        // emit light from an environment map
	        new infoParm_t("lightfilter",  0,  surfaceFlags.SURF_LIGHTFILTER, 0 ),      // filter light going through it
	        new infoParm_t("alphashadow",  0,  surfaceFlags.SURF_ALPHASHADOW, 0 ),      // test light on a per-pixel basis
	        new infoParm_t("hint",     0,  surfaceFlags.SURF_HINT,      0 ),        // use as a primary splitter

	        // server attributes
	        new infoParm_t("slick",            0,  surfaceFlags.SURF_SLICK,     0 ),
	        new infoParm_t("noimpact",     0,  surfaceFlags.SURF_NOIMPACT,  0 ),        // don't make impact explosions or marks
	        new infoParm_t("nomarks",          0,  surfaceFlags.SURF_NOMARKS,   0 ),        // don't make impact marks, but still explode
	        new infoParm_t("ladder",           0,  surfaceFlags.SURF_LADDER,    0 ),
	        new infoParm_t("nodamage",     0,  surfaceFlags.SURF_NODAMAGE,  0 ),

	        new infoParm_t("monsterslick", 0,  surfaceFlags.SURF_MONSTERSLICK,  0),     // surf only slick for monsters

        //	new infoParm_t("flesh",		0,	SURF_FLESH,		0 ),
	        new infoParm_t("glass",        0,  surfaceFlags.SURF_GLASS,     0 ),    //----(SA)	added
	        new infoParm_t("ceramic",      0,  surfaceFlags.SURF_CERAMIC,   0 ),    //----(SA)	added

	        // steps
	        new infoParm_t("metal",        0,  surfaceFlags.SURF_METAL,     0 ),
	        new infoParm_t("metalsteps",   0,  surfaceFlags.SURF_METAL,     0 ),    // retain bw compatibility with Q3A metal shaders... (SA)
	        new infoParm_t("nosteps",      0,  surfaceFlags.SURF_NOSTEPS,   0 ),
	        new infoParm_t("woodsteps",    0,  surfaceFlags.SURF_WOOD,      0 ),
	        new infoParm_t("grasssteps",   0,  surfaceFlags.SURF_GRASS,     0 ),
	        new infoParm_t("gravelsteps",  0,  surfaceFlags.SURF_GRAVEL,    0 ),
	        new infoParm_t("carpetsteps",  0,  surfaceFlags.SURF_CARPET,    0 ),
	        new infoParm_t("snowsteps",    0,  surfaceFlags.SURF_SNOW,      0 ),
	        new infoParm_t("roofsteps",    0,  surfaceFlags.SURF_ROOF,      0 ),    // tile roof

	        new infoParm_t("rubble", 0, surfaceFlags.SURF_RUBBLE, 0 ),

	        // drawsurf attributes
	        new infoParm_t("nodraw",       0,  surfaceFlags.SURF_NODRAW,    0 ),    // don't generate a drawsurface (or a lightmap)
	        new infoParm_t("pointlight",   0,  surfaceFlags.SURF_POINTLIGHT, 0 ),   // sample lighting at vertexes
	        new infoParm_t("nolightmap",   0,  surfaceFlags.SURF_NOLIGHTMAP,0 ),        // don't generate a lightmap
	        new infoParm_t("nodlight", 0,  surfaceFlags.SURF_NODLIGHT, 0 ),     // don't ever add dynamic lights

	        new infoParm_t("monsterslicknorth",    0, surfaceFlags.SURF_MONSLICK_N,0),
	        new infoParm_t("monsterslickeast", 0, surfaceFlags.SURF_MONSLICK_E,0),
	        new infoParm_t("monsterslicksouth",    0, surfaceFlags.SURF_MONSLICK_S,0),
	        new infoParm_t("monsterslickwest", 0, surfaceFlags.SURF_MONSLICK_W,0)

        };
        public idMaterialBase shader;
        private string name;

        public static idMaterialBase GetMaterialBase( ref idMaterial mtr )
        {
            return ((idMaterialLocal)mtr).shader;
        }

        public static idMaterialBase GetMaterialBase(idMaterial mtr)
        {
            return ((idMaterialLocal)mtr).shader;
        }

        /*
        ===============
        NameToAFunc
        ===============
        */
        private int NameToAFunc( string funcname ) {
	        if ( funcname == "GT0" ) {
		        return Globals.GLS_ATEST_GT_0;
	        } else if ( funcname == "LT128" )    {
		        return Globals.GLS_ATEST_LT_80;
	        } else if ( funcname == "GE128" )    {
		        return Globals.GLS_ATEST_GE_80;
	        }

	        Engine.common.Warning( "invalid alphaFunc name '%s' in shader '%s'\n", funcname, shader.name );
	        return 0;
        }

        /*
        ===================
        ParseTexMod
        ===================
        */
        private void ParseTexMod(string _text, ref shaderStage_t stage) {
	        idParser parser = new idParser( _text );
            string token;
	        texModInfo_t tmi;

	        if ( stage.bundle[0].numTexMods == idMaterialBase.TR_MAX_TEXMODS ) {
		        Engine.common.ErrorDrop( "too many tcMod stages in shader '%s'\n", shader.name );
		        return;
	        }

            stage.bundle[0].texMods[stage.bundle[0].numTexMods] = new texModInfo_t();
	        tmi = stage.bundle[0].texMods[stage.bundle[0].numTexMods];
	        stage.bundle[0].numTexMods++;

	        token = parser.NextToken.ToLower();

	        //
	        // swap
	        //
	        if ( token == "swap" ) { // swap S/T coords (rotate 90d)
		        tmi.type = texMod_t.TMOD_SWAP;
	        }
	        //
	        // turb
	        //
	        // (SA) added 'else' so it wouldn't claim 'swap' was unknown.
	        else if ( token == "turb" ) {
		        token = parser.NextToken;
		        if ( token == null ) {
			        Engine.common.Warning( "missing tcMod turb parms in shader '%s'\n", shader.name );
			        return;
		        }
		        tmi.wave.base_c = float.Parse( token );
		        token = parser.NextToken;
		        if ( token == null ) {
			        Engine.common.Warning( "missing tcMod turb in shader '%s'\n", shader.name );
			        return;
		        }
		        tmi.wave.amplitude = float.Parse( token );
                token = parser.NextToken;
		        if ( token[0] == 0 ) {
			        Engine.common.Warning( "missing tcMod turb in shader '%s'\n", shader.name );
			        return;
		        }
		        tmi.wave.phase = float.Parse( token );
                token = parser.NextToken;
		        if ( token[0] == 0 ) {
			        Engine.common.Warning( "missing tcMod turb in shader '%s'\n", shader.name );
			        return;
		        }
		        tmi.wave.frequency = float.Parse( token );

		        tmi.type = texMod_t.TMOD_TURBULENT;
	        }
	        //
	        // scale
	        //
	        else if ( token == "scale" ) {
                token = parser.NextToken;
		        if ( token[0] == 0 ) {
			        Engine.common.Warning( "missing scale parms in shader '%s'\n", shader.name );
			        return;
		        }
		        tmi.scale[0] = float.Parse( token );

                token = parser.NextToken;
		        if ( token[0] == 0 ) {
			        Engine.common.Warning( "missing scale parms in shader '%s'\n", shader.name );
			        return;
		        }
		        tmi.scale[1] = float.Parse( token );
                tmi.type = texMod_t.TMOD_SCALE;
	        }
	        //
	        // scroll
	        //
	        else if ( token == "scroll" ) {
                token = parser.NextToken;
		        if ( token[0] == 0 ) {
			        Engine.common.Warning( " missing scale scroll parms in shader '%s'\n", shader.name );
			        return;
		        }
		        tmi.scroll[0] = float.Parse( token );
                token = parser.NextToken;
		        if ( token[0] == 0 ) {
			        Engine.common.Warning( "missing scale scroll parms in shader '%s'\n", shader.name );
			        return;
		        }
		        tmi.scroll[1] = float.Parse( token );
		        tmi.type = texMod_t.TMOD_SCROLL;
	        }
	        //
	        // stretch
	        //
	        else if ( token ==  "stretch" ) {
                token = parser.NextToken;
		        if ( token[0] == 0 ) {
			        Engine.common.Warning( "missing stretch parms in shader '%s'\n", shader.name );
			        return;
		        }
		        tmi.wave.func = NameToGenFunc( token );

                token = parser.NextToken;
		        if ( token[0] == 0 ) {
			        Engine.common.Warning( "missing stretch parms in shader '%s'\n", shader.name );
			        return;
		        }
		        tmi.wave.base_c = float.Parse( token );

                token = parser.NextToken;
		        if ( token[0] == 0 ) {
			        Engine.common.Warning( "missing stretch parms in shader '%s'\n", shader.name );
			        return;
		        }
		        tmi.wave.amplitude = float.Parse( token );

                token = parser.NextToken;
		        if ( token[0] == 0 ) {
			        Engine.common.Warning( "missing stretch parms in shader '%s'\n", shader.name );
			        return;
		        }
                tmi.wave.phase = float.Parse(token);

                token = parser.NextToken;
		        if ( token[0] == 0 ) {
			        Engine.common.Warning( "missing stretch parms in shader '%s'\n", shader.name );
			        return;
		        }
                tmi.wave.frequency = float.Parse(token);

		        tmi.type = texMod_t.TMOD_STRETCH;
	        }
	        //
	        // transform
	        //
	        else if ( token ==  "transform" ) {
                token = parser.NextToken;
                tmi.matrix[0] = new idVector2();
                tmi.matrix[1] = new idVector2();
		        if ( token[0] == 0 ) {
			        Engine.common.Warning( "missing transform parms in shader '%s'\n", shader.name );
			        return;
		        }
                tmi.matrix[0][0] = float.Parse(token);

                token = parser.NextToken;
		        if ( token[0] == 0 ) {
			        Engine.common.Warning( "missing transform parms in shader '%s'\n", shader.name );
			        return;
		        }
                tmi.matrix[0][1] = float.Parse(token);

                token = parser.NextToken;
		        if ( token[0] == 0 ) {
			        Engine.common.Warning( "missing transform parms in shader '%s'\n", shader.name );
			        return;
		        }
                tmi.matrix[1][0] = float.Parse(token);

                token = parser.NextToken;
		        if ( token[0] == 0 ) {
			        Engine.common.Warning( "missing transform parms in shader '%s'\n", shader.name );
			        return;
		        }
                tmi.matrix[1][1] = float.Parse(token);

                token = parser.NextToken;
		        if ( token[0] == 0 ) {
			        Engine.common.Warning( "missing transform parms in shader '%s'\n", shader.name );
			        return;
		        }
                tmi.translate[0] = float.Parse(token);

                token = parser.NextToken;
		        if ( token[0] == 0 ) {
			        Engine.common.Warning( "missing transform parms in shader '%s'\n", shader.name );
			        return;
		        }
                tmi.translate[1] = float.Parse(token);

		        tmi.type = texMod_t.TMOD_TRANSFORM;
	        }
	        //
	        // rotate
	        //
	        else if ( token == "rotate" ) {
                token = parser.NextToken;
		        if ( token[0] == 0 ) {
			        Engine.common.Warning( "missing tcMod rotate parms in shader '%s'\n", shader.name );
			        return;
		        }
                tmi.rotateSpeed = float.Parse(token);
		        tmi.type = texMod_t.TMOD_ROTATE;
	        }
	        //
	        // entityTranslate
	        //
	        else if ( token == "entityTranslate" ) {
		        tmi.type = texMod_t.TMOD_ENTITY_TRANSLATE;
	        } else
	        {
		        Engine.common.Warning( "unknown tcMod '%s' in shader '%s'\n", token, shader.name );
	        }
        }

        //
        // ParseStage
        //
        private bool ParseStage(out shaderStage_t stage, ref idParser parser)
        {
            string token;
	        int depthMaskBits = Globals.GLS_DEPTHMASK_TRUE, atestBits = 0, depthFuncBits = 0;
            Blend blendSrcBits = Blend.One, blendDstBits = Blend.One;
	        bool depthMaskExplicit = false;

            stage = new shaderStage_t();
	        stage.active = true;

	        while ( true )
	        {
		        if(parser.ReachedEndOfBuffer == true )
                {
                    parser.UngetToken();

			         Engine.common.Warning("no matching '}' found - last token %s\n", parser.NextToken );
			        return false;
		        }

                token = parser.NextToken.ToLower();

		        if ( token == "}" ) {
			        break;
		        }
		        //
		        // check special case for map16/map32/mapcomp/mapnocomp (compression enabled)
		        if ( token == "map16" ) {    // only use this texture if 16 bit color depth
			        if ( Globals.colorBits <= 16 ) {
				        token = "map";   // use this map
			        } else {
                        token = parser.NextToken;   // ignore the map
				        continue;
			        }
		        } else if ( token == "map32" )    { // only use this texture if 16 bit color depth
			        if ( Globals.colorBits > 16 ) {
				        token = "map";   // use this map
			        } else {
				        token = parser.NextToken;   // ignore the map
				        continue;
			        }
		        } else if ( token == "mapcomp" )    { // only use this texture if compression is enabled
			       // if ( glConfig.textureCompression ) {
				   //     token = "map";   // use this map
			      //  } else {
				       token = parser.NextToken;    // ignore the map
				        continue;
			       // }
		        } else if ( token == "mapnocomp" )    { // only use this texture if compression is not available or disabled
			       // if ( !glConfig.textureCompression ) {
				        token = "map";   // use this map
			       // } else {
				    //    COM_ParseExt( text, qfalse );   // ignore the map
				    //    continue;
			       // }
		        } else if ( token == "animmapcomp" )    { // only use this texture if compression is enabled
			        //if ( glConfig.textureCompression && r_ext_compressed_textures->integer ) {
				     //   token = "animmap";   // use this map
			       // } else {
				    //    while ( token[0] )
					 //       COM_ParseExt( text, qfalse );   // ignore the map
                            parser.ParseRestOfLine();
				        continue;
			       // }
		        } else if ( token == "animmapnocomp" )    { // only use this texture if compression is not available or disabled
			        //if ( !glConfig.textureCompression ) {
				        token = "animmap";   // use this map
			        //} else {
				    //    while ( token[0] )
					 //       COM_ParseExt( text, qfalse );   // ignore the map
				        continue;
			        //}
		        }
		        //
		        // map <name>
		        //
		        if ( token == "map"  ) {
			        token = parser.NextToken;
                    stage.bundle[0] = new textureBundle_t();

        //----(SA)	fixes startup error and allows polygon shadows to work again
			        if ( token == "$whiteimage" || token == "*white" ) {
        //----(SA)	end
				        stage.bundle[0].image[0] = Globals.tr.whiteImage;
			        }
        //----(SA) added
			        else if ( token == "$dlight" ) {
				        stage.bundle[0].image[0] = Globals.tr.dlightImage;
			        }
        //----(SA) end
			        else if ( token == "$lightmap" ) {
				        stage.bundle[0].isLightmap = true;
				        if ( shader.lightmapIndex < 0 ) {
					        stage.bundle[0].image[0] = Globals.tr.whiteImage;
				        } else {
					        stage.bundle[0].image[0] = Globals.tr.lightmaps[shader.lightmapIndex];
				        }
			        } else
			        {
        //----(SA)	modified
        				stage.bundle[0].image[0] = Engine.imageManager.FindImageFile( token, !shader.noMipMaps, !shader.noPicMip, SamplerState.LinearWrap );
				        //stage.bundle[0].image[0] = R_FindImageFileExt( token, !shader.noMipMaps, !shader.noPicMip, shader.characterMip, GL_REPEAT );
        //----(SA)	end
				        if ( stage.bundle[0].image[0] == null ) {
					        Engine.common.Warning("R_FindImageFile could not find '%s' in shader '%s'\n", token, shader.name );
					        return false;
				        }
			        }
		        }
		        //
		        // clampmap <name>
		        //
		        else if ( token == "clampmap" ) {
			        token = parser.NextToken;
                    stage.bundle[0] = new textureBundle_t();
			        stage.bundle[0].image[0] =  Engine.imageManager.FindImageFile( token, !shader.noMipMaps, !shader.noPicMip, SamplerState.LinearClamp );
			        if ( stage.bundle[0].image[0] == null ) {
					    Engine.common.Warning("R_FindImageFile could not find '%s' in shader '%s'\n", token, shader.name );
					    return false;
				    }
		        }
		        //
		        // animMap <frequency> <image1> .... <imageN>
		        //
		        else if ( token == "animmap" ) {
			        token = parser.GetNextTokenFromLine();
			        if ( token == null ) {
				        Engine.common.Warning( "missing parameter for 'animMmap' keyword in shader '%s'\n", shader.name );
				        return false;
			        }
                    stage.bundle[0] = new textureBundle_t();
			        stage.bundle[0].imageAnimationSpeed = float.Parse( token );

			        // parse up to MAX_IMAGE_ANIMATIONS animations
			        while ( true ) {
				        int num;

				        token = parser.GetNextTokenFromLine();
				        if ( token == null ) {
					        break;
				        }
				        num = stage.bundle[0].numImageAnimations;
				        if ( num < idMaterialBase.MAX_IMAGE_ANIMATIONS ) {
					        stage.bundle[0].image[num] = Engine.imageManager.FindImageFile( token, !shader.noMipMaps, !shader.noPicMip, SamplerState.LinearWrap );
					        if ( stage.bundle[0].image[num] == null ) {
						        Engine.common.Warning("R_FindImageFile could not find '%s' in shader '%s'\n", token, shader.name );
						        return false;
					        }
					        stage.bundle[0].numImageAnimations++;
				        }
			        }
		        } else if ( token == "videomap" )    {
			        token = parser.GetNextTokenFromLine();
			        if ( token == null ) {
				        Engine.common.Warning( "missing parameter for 'videoMmap' keyword in shader '%s'\n", shader.name );
				        return false;
			        }
			        //stage.bundle[0].videoMapHandle = ri.CIN_PlayCinematic( token, 0, 0, 256, 256, ( CIN_loop | CIN_silent | CIN_shader ) );
			        if ( stage.bundle[0].videoMapHandle != -1 ) {
				        stage.bundle[0].isVideoMap = true;
				        //stage.bundle[0].image[0] = tr.scratchImage[stage->bundle[0].videoMapHandle];
			        }
		        }
		        //
		        // alphafunc <func>
		        //
		        else if ( token == "alphafunc" ) {
			        token = parser.GetNextTokenFromLine();
			        if ( token == null ) {
				        Engine.common.Warning( "missing parameter for 'alphaFunc' keyword in shader '%s'\n", shader.name );
				        return false;
			        }

			        atestBits = NameToAFunc( token );
		        }
		        //
		        // depthFunc <func>
		        //
		        else if ( token == "depthfunc" ) {
			        token = parser.GetNextTokenFromLine();

			        if ( token == null ) {
				        Engine.common.Warning("missing parameter for 'depthfunc' keyword in shader '%s'\n", shader.name );
				        return false;
			        }

			        if ( token == "lequal" ) {
				        depthFuncBits = 0;
			        } else if ( token == "equal" )    {
				        depthFuncBits = Globals.GLS_DEPTHFUNC_EQUAL;
			        } else
			        {
				        Engine.common.Warning( "unknown depthfunc '%s' in shader '%s'\n", token, shader.name );
				        //continue;
			        }
		        }
		        //
		        // detail
		        //
		        else if ( token == "detail" ) {
			        stage.isDetail = true;
		        }
		        //
		        // fog
		        //
		        else if ( token == "fog" ) {
			        token = parser.GetNextTokenFromLine();
			        if ( token == null ) {
				        Engine.common.Warning("missing parm for fog in shader '%s'\n", shader.name );
				        return false;
			        }
			        if ( token == "on" ) {
				        stage.isFogged = true;
			        } else {
				        stage.isFogged = false;
			        }
		        }
		        //
		        // blendfunc <srcFactor> <dstFactor>
		        // or blendfunc <add|filter|blend>
		        //
		        else if ( token == "blendfunc" ) {
			        token = parser.GetNextTokenFromLine();
			        if ( token == null) {
				        Engine.common.Warning("missing parm for blendFunc in shader '%s'\n", shader.name );
				        return false;
			        }

                    stage.useBlending = true;

			        // check for "simple" blends first
			        if ( token == "add" ) {
                        blendSrcBits = Blend.One;// Globals.GLS_SRCBLEND_ONE;
                        blendDstBits = Blend.One;//Globals.GLS_DSTBLEND_ONE;
			        } else if ( token == "filter" ) {
                        blendSrcBits = Blend.DestinationColor;//Globals.GLS_SRCBLEND_DST_COLOR;
				        blendDstBits = Blend.Zero; //Globals.GLS_DSTBLEND_ZERO;
			        } else if ( token == "blend" ) {
				        blendSrcBits = Blend.SourceAlpha; //Globals.GLS_SRCBLEND_SRC_ALPHA;
				        blendDstBits = Blend.InverseSourceAlpha; //Globals.GLS_DSTBLEND_ONE_MINUS_SRC_ALPHA;
			        } else {
				        // complex double blends
				        blendSrcBits = NameToSrcBlendMode( token );

				        token = parser.GetNextTokenFromLine();
				        if ( token == null ) {
					        Engine.common.Warning( "missing parm for blendFunc in shader '%s'\n", shader.name );
					        return false;
				        }
				        blendDstBits = NameToDstBlendMode( token );
			        }

                    // GL_SRC_ALPHA GL_ONE_MINUS_SRC_ALPHA?
                //    if ((blendSrcBits == Blend.One && blendDstBits == Blend.Zero) || (blendSrcBits == Blend.SourceAlpha && blendDstBits == Blend.InverseSourceAlpha))
                 //   {
               //         stage.blendState = BlendState.AlphaBlend;
               //     }
                //    else
                //    {
                    //GL_DST_COLOR GL_ZERO

                    if (blendSrcBits == Blend.DestinationColor && blendDstBits == Blend.Zero)
                    {
                        stage.useBlending = false;
                    }
                    else
                    {
                        stage.blendState = new BlendState();

                        stage.blendState.AlphaSourceBlend = (Blend)blendSrcBits;
                        stage.blendState.ColorSourceBlend = (Blend)blendSrcBits;

                        stage.blendState.AlphaDestinationBlend = (Blend)blendDstBits;
                        stage.blendState.ColorDestinationBlend = (Blend)blendDstBits;
                        // clear depth mask for blended surfaces
                        
                    }

                    if (!depthMaskExplicit)
                    {
                        depthMaskBits = 0;
                    }
                  //  }

			        
		        }
		        //
		        // rgbGen
		        //
		        else if ( token == "rgbgen" ) {
			        token = parser.GetNextTokenFromLine().ToLower();
			        if ( token == null ) {
				        Engine.common.Warning( "missing parameters for rgbGen in shader '%s'\n", shader.name );
				        return false;
			        }

			        if ( token == "wave" ) {
				        ParseWaveForm( ref parser, out stage.rgbWave );
				        stage.rgbGen = colorGen_t.CGEN_WAVEFORM;
			        } else if ( token == "const" )    {
                        parser.NextVector3( ref stage.constantColor );
                        stage.constantColor *= 255;

                        stage.rgbGen = colorGen_t.CGEN_CONST;
			        } else if ( token == "identity"  )    {
				        stage.rgbGen = colorGen_t.CGEN_IDENTITY;
			        } else if ( token == "identitylighting"  )    {
				        stage.rgbGen = colorGen_t.CGEN_IDENTITY_LIGHTING;
			        } else if ( token == "entity"  )    {
				        stage.rgbGen = colorGen_t.CGEN_ENTITY;
			        } else if ( token == "oneminusentity"  )    {
				        stage.rgbGen = colorGen_t.CGEN_ONE_MINUS_ENTITY;
			        } else if ( token == "vertex"  )    {
				        stage.rgbGen = colorGen_t.CGEN_VERTEX;
				        if ( stage.alphaGen == 0 ) {
					        stage.alphaGen = alphaGen_t.AGEN_VERTEX;
				        }
			        } else if ( token == "exactvertex"  )    {
				        stage.rgbGen = colorGen_t.CGEN_EXACT_VERTEX;
			        } else if ( token == "lightingdiffuse"  )    {
				        stage.rgbGen = colorGen_t.CGEN_LIGHTING_DIFFUSE;
			        } else if ( token == "oneminusvertex"  )    {
				        stage.rgbGen = colorGen_t.CGEN_ONE_MINUS_VERTEX;
			        } else
			        {
				        Engine.common.Warning( "unknown rgbGen parameter '%s' in shader '%s'\n", token, shader.name );
				        continue;
			        }
		        }
		        //
		        // alphaGen
		        //
		        else if ( token == "alphagen" ) {
			        token = parser.GetNextTokenFromLine().ToLower();
			        if ( token == null ) {
				        Engine.common.Warning( "missing parameters for alphaGen in shader '%s'\n", shader.name );
				        continue;
			        }

			        if ( token == "wave" ) {
				        ParseWaveForm( ref parser, out stage.alphaWave );
				        stage.alphaGen = alphaGen_t.AGEN_WAVEFORM;
			        } else if ( token == "const" )    {
				        token = parser.GetNextTokenFromLine();
				        stage.constantColorAlpha = 255 * float.Parse( token );
				        stage.alphaGen = alphaGen_t.AGEN_CONST;
			        } else if ( token == "identity"  )    {
				        stage.alphaGen = alphaGen_t.AGEN_IDENTITY;
			        } else if ( token == "entity" )    {
				        stage.alphaGen = alphaGen_t.AGEN_ENTITY;
			        } else if ( token == "oneminusentity" )    {
				        stage.alphaGen = alphaGen_t.AGEN_ONE_MINUS_ENTITY;
			        }
			        // Ridah
			        else if ( token == "normalzfade" ) {
				        stage.alphaGen = alphaGen_t.AGEN_NORMALZFADE;
				        token = parser.GetNextTokenFromLine();
				        if ( token != null ) {
					        stage.constantColorAlpha = 255 * float.Parse( token );
				        } else {
					        stage.constantColorAlpha = 255;
				        }

				        token = parser.GetNextTokenFromLine();
				        if ( token != null ) {
					        stage.zFadeBounds[0] = float.Parse( token );    // lower range
					        token = parser.GetNextTokenFromLine();
					        stage.zFadeBounds[1] = float.Parse( token );    // upper range
				        } else {
					        stage.zFadeBounds[0] = -1.0f;   // lower range
					        stage.zFadeBounds[1] =  1.0f;   // upper range
				        }

			        }
			        // done.
			        else if ( token == "vertex"  ) {
				        stage.alphaGen = alphaGen_t.AGEN_VERTEX;
			        } else if ( token == "lightingspecular" )    {
				        stage.alphaGen = alphaGen_t.AGEN_LIGHTING_SPECULAR;
			        } else if ( token == "oneMinusvertex" )    {
				        stage.alphaGen = alphaGen_t.AGEN_ONE_MINUS_VERTEX;
			        } else if ( token == "portal" )    {
				        stage.alphaGen = alphaGen_t.AGEN_PORTAL;
				        token = parser.GetNextTokenFromLine();
				        if ( token == null ) {
					        shader.portalRange = 256;
					        Engine.common.Warning( "missing range parameter for alphaGen portal in shader '%s', defaulting to 256\n", shader.name );
				        } else
				        {
					        shader.portalRange = float.Parse( token );
				        }
			        } else
			        {
				        Engine.common.Warning( "unknown alphaGen parameter '%s' in shader '%s'\n", token, shader.name );
				        continue;
			        }
		        }
		        //
		        // tcGen <function>
		        //
		        else if ( token == "texgen" || token == "tcgen"  ) {
			        token = parser.GetNextTokenFromLine().ToLower();
			        if ( token == null ) {
				        Engine.common.Warning("missing texgen parm in shader '%s'\n", shader.name );
				        continue;
			        }

			        if ( token == "environment" ) {
				        stage.bundle[0].tcGen = texCoordGen_t.TCGEN_ENVIRONMENT_MAPPED;
			        } else if ( token == "firerisenv" )    {
				        stage.bundle[0].tcGen = texCoordGen_t.TCGEN_FIRERISEENV_MAPPED;
			        } else if ( token == "lightmap" )    {
				        stage.bundle[0].tcGen = texCoordGen_t.TCGEN_LIGHTMAP;
			        } else if ( token == "texture" || token == "base" )     {
				        stage.bundle[0].tcGen = texCoordGen_t.TCGEN_TEXTURE;
			        } else if ( token == "vector" )    {
                        stage.bundle[0].tcGenVectors[0] = new idVector3();
                        parser.NextVector3( ref stage.bundle[0].tcGenVectors[0] );

                        stage.bundle[0].tcGenVectors[1] = new idVector3();
                        parser.NextVector3( ref stage.bundle[0].tcGenVectors[1] );

				        stage.bundle[0].tcGen = texCoordGen_t.TCGEN_VECTOR;
			        } else
			        {
				        Engine.common.Warning( "unknown texgen parm in shader '%s'\n", shader.name );
			        }
		        }
		        //
		        // tcMod <type> <...>
		        //
		        else if ( token == "tcmod" ) {
			        string buffer = "";

			        while ( true )
			        {
                        token = parser.GetNextTokenFromLine();
				        if ( token == null ) {
					        break;
				        }
                        buffer += token + " ";
			        }

			        ParseTexMod( buffer, ref stage );

			        continue;
		        }
		        //
		        // depthmask
		        //
		        else if ( token == "depthwrite" ) {
			        depthMaskBits = Globals.GLS_DEPTHMASK_TRUE;
			        depthMaskExplicit = true;

			        continue;
		        } else
		        {
			        Engine.common.Warning( "unknown parameter '%s' in shader '%s'\n", token, shader.name );
			        return false;
		        }
	        }

	        //
	        // if cgen isn't explicitly specified, use either identity or identitylighting
	        //
	        if ( stage.rgbGen == colorGen_t.CGEN_BAD ) {
		        if ( blendSrcBits == 0 ||
			         blendSrcBits == Blend.One ||
			         blendSrcBits == Blend.SourceAlpha ) {
			        stage.rgbGen = colorGen_t.CGEN_IDENTITY_LIGHTING;
		        } else {
			        stage.rgbGen = colorGen_t.CGEN_IDENTITY;
		        }
	        }


	        //
	        // implicitly assume that a GL_ONE GL_ZERO blend mask disables blending
	        //
	        if ( ( blendSrcBits == Blend.One ) &&
		         ( blendDstBits == Blend.Zero ) ) 
            {
               stage.useBlending = false;
   //             shader.sort = (float)shaderSort_t.SS_SEE_THROUGH;
		        depthMaskBits = Globals.GLS_DEPTHMASK_TRUE;
	        }

            // decide which agens we can skip
	        if ( stage.alphaGen == (alphaGen_t)colorGen_t.CGEN_IDENTITY ) {
		        if ( stage.rgbGen == colorGen_t.CGEN_IDENTITY
			         || stage.rgbGen == colorGen_t.CGEN_LIGHTING_DIFFUSE ) {
			        stage.alphaGen = alphaGen_t.AGEN_SKIP;
		        }
	        }

	        //
	        // compute state bits
	        //
	        stage.stateBits = (depthMaskBits |
					        //   blendSrcBits | blendDstBits |
					           atestBits |
					           depthFuncBits);

            

            return true;
        }

        //
        // FinishShader
        //
        private void FinishShader()
        {
            int stage, i;
	        bool hasLightmapStage = false;
	        bool vertexLightmap = false;

	        //
	        // set sky stuff appropriate
	        //
	        if ( shader.isSky ) {
		     //   shader.sort = (float)shaderSort_t.SS_ENVIRONMENT;
	        }

	        //
	        // set polygon offset
	        //
	        if ( shader.polygonOffset && shader.sort <= 0 ) {
		     //   shader.sort = (float)shaderSort_t.SS_DECAL;
	        }

	        //
	        // set appropriate stage information
	        //
	        for ( stage = 0; stage < idMaterialBase.MAX_SHADER_STAGES; stage++ ) {
		        shaderStage_t pStage = shader.stages[stage];

		        if ( pStage == null || !pStage.active ) {
			        break;
		        }

		        // check for a missing texture
		        if ( pStage.bundle[0].image[0] == null ) {
			        Engine.common.Warning( "Shader %s has a stage with no image\n", shader.name );
			        pStage.active = false;
			        continue;
		        }

		        //
		        // ditch this stage if it's detail and detail textures are disabled
		        //
// JV - detail textures always enabled.
#if false
		        if ( pStage.isDetail && !r_detailTextures->integer ) {
			        if ( stage < ( MAX_SHADER_STAGES - 1 ) ) {
				        memmove( pStage, pStage + 1, sizeof( *pStage ) * ( MAX_SHADER_STAGES - stage - 1 ) );
				        // kill the last stage, since it's now a duplicate
				        for ( i = MAX_SHADER_STAGES - 1; i > stage; i-- ) {
					        if ( stages[i].active ) {
						        memset(  &stages[i], 0, sizeof( *pStage ) );
						        break;
					        }
				        }
				        stage--;    // the next stage is now the current stage, so check it again
			        } else {
				        memset( pStage, 0, sizeof( *pStage ) );
			        }
			        continue;
		        }
#endif
// JV end
		        //
		        // default texture coordinate generation
		        //
		        if ( pStage.bundle[0].isLightmap ) {
			        if ( pStage.bundle[0].tcGen == texCoordGen_t.TCGEN_BAD ) {
                        pStage.bundle[0].tcGen = texCoordGen_t.TCGEN_LIGHTMAP;
			        }
			        hasLightmapStage = true;
		        } else {
                    if (pStage.bundle[0].tcGen == texCoordGen_t.TCGEN_BAD)
                    {
                        pStage.bundle[0].tcGen = texCoordGen_t.TCGEN_TEXTURE;
			        }
		        }


		        // not a true lightmap but we want to leave existing
		        // behaviour in place and not print out a warning
		        //if (pStage->rgbGen == CGEN_VERTEX) {
		        //  vertexLightmap = qtrue;
		        //}



		        //
		        // determine sort order and fog color adjustment
		        //
                if (pStage.useBlending)
                {
			        // don't screw with sort order if this is a portal or environment
                    if (shader.sort <= 0)
                    {
                        // see through item, like a grill or grate
                        if ((((int)pStage.stateBits) & Globals.GLS_DEPTHMASK_TRUE) != 0)
                        {
                         //   shader.sort = (float)shaderSort_t.SS_SEE_THROUGH;
                        }
                        else
                        {
                        //    shader.sort = (float)shaderSort_t.SS_BLEND0;
                        }
                    }
                    else
                    {
                     //   shader.sort++;
                    }
		        }
	        }

	        // there are times when you will need to manually apply a sort to
	        // opaque alpha tested shaders that have later blend passes
	        if ( shader.sort <= 0 ) {
           //     shader.sort = (float)shaderSort_t.SS_OPAQUE;
	        }

	        //
	        // if we are in r_vertexLight mode, never use a lightmap texture
	        //
// jv - vertex lighting disabled.
#if false
	        if ( stage > 1 && ( ( r_vertexLight->integer && !r_uiFullScreen->integer ) || glConfig.hardwareType == GLHW_PERMEDIA2 ) ) {
		        VertexLightingCollapse();
		        stage = 1;
		        hasLightmapStage = qfalse;
	        }
#endif
// jv end
	        //
	        // look for multitexture potential
	        //
	        if ( stage > 1 && CollapseMultitexture() ) {
		        stage--;
	        }

	        if ( shader.lightmapIndex >= 0 && !hasLightmapStage ) {
		        if ( vertexLightmap ) {
			        Engine.common.Warning( "shader '%s' has VERTEX forced lightmap!\n", shader.name );
		        } else {
                    Engine.common.Warning("shader '%s' has lightmap but no lightmap stage!\n", shader.name);
			        shader.lightmapIndex = -1;
		        }
	        }


	        //
	        // compute number of passes
	        //
	        shader.numUnfoggedPasses = stage;

	        // fogonly shaders don't have any normal passes
	        if ( stage == 0 ) {
             //   shader.sort = (float)shaderSort_t.SS_FOG;
	        }

	        // determine which stage iterator function is appropriate
	        ComputeStageIteratorFunc();

	        // RF default back to no compression for next shader
#if false
	        if ( r_ext_compressed_textures->integer == 2 ) {
		        tr.allowCompress = qfalse;
	        }
#endif
        }

        /*
        ========================================================================================

        SHADER OPTIMIZATION AND FOGGING

        ========================================================================================
        */

        /*
        ===================
        ComputeStageIteratorFunc

        See if we can use on of the simple fastpath stage functions,
        otherwise set to the generic stage function
        ===================
        */
        private void ComputeStageIteratorFunc() {
	        shader.optimalStageIteratorFunc = StageIteratorGeneric.Iterator;

	        //
	        // see if this should go into the sky path
	        //
	        if ( shader.isSky ) {
                shader.optimalStageIteratorFunc = StageIteratorSky.Iterator;
                return;
	        }

	        //
	        // see if this can go into an optimized LM, multitextured path
	        //
	        if ( shader.numUnfoggedPasses == 1 ) {
		        if ( ( shader.stages[0].rgbGen == colorGen_t.CGEN_IDENTITY ) && ( shader.stages[0].alphaGen == alphaGen_t.AGEN_IDENTITY ) ) {
                    if (shader.stages[0].bundle[0].tcGen == texCoordGen_t.TCGEN_TEXTURE && shader.stages[0].bundle[1] != null &&
                         shader.stages[0].bundle[1].tcGen == texCoordGen_t.TCGEN_LIGHTMAP)
                    {
				        if ( !shader.polygonOffset ) {
					        if ( shader.numDeforms == 0 ) {
                                shader.optimalStageIteratorFunc = StageIteratorLightmappedMultitexture.Iterator;
                                return;
					        }
				        }
			        }
		        }
	        }
        }

        struct collapse_t {
	        public int blendA;
	        public int blendB;

            public BlendState multitextureEnv;
	        public int multitextureBlend;

            public collapse_t(int ba, int bb, BlendState me, int mb)
            {
                blendA = ba;
                blendB = bb;
                multitextureEnv = me;
                multitextureBlend = mb;
            }
        };

        static collapse_t[] collapse = new collapse_t[] {
	        new collapse_t( 0, Globals.GLS_DSTBLEND_SRC_COLOR | Globals.GLS_SRCBLEND_ZERO,
	          BlendState.NonPremultiplied, 0 ),

	        new collapse_t( 0, Globals.GLS_DSTBLEND_ZERO | Globals.GLS_SRCBLEND_DST_COLOR,
	          BlendState.NonPremultiplied, 0 ),

	        new collapse_t( Globals.GLS_DSTBLEND_ZERO | Globals.GLS_SRCBLEND_DST_COLOR, Globals.GLS_DSTBLEND_ZERO | Globals.GLS_SRCBLEND_DST_COLOR,
	          BlendState.NonPremultiplied, Globals.GLS_DSTBLEND_ZERO | Globals.GLS_SRCBLEND_DST_COLOR ),

	        new collapse_t( Globals.GLS_DSTBLEND_SRC_COLOR | Globals.GLS_SRCBLEND_ZERO, Globals.GLS_DSTBLEND_ZERO | Globals.GLS_SRCBLEND_DST_COLOR,
	          BlendState.NonPremultiplied, Globals.GLS_DSTBLEND_ZERO | Globals.GLS_SRCBLEND_DST_COLOR ),

	        new collapse_t( Globals.GLS_DSTBLEND_ZERO | Globals.GLS_SRCBLEND_DST_COLOR, Globals.GLS_DSTBLEND_SRC_COLOR | Globals.GLS_SRCBLEND_ZERO,
	          BlendState.NonPremultiplied, Globals.GLS_DSTBLEND_ZERO | Globals.GLS_SRCBLEND_DST_COLOR ),

	        new collapse_t( Globals.GLS_DSTBLEND_SRC_COLOR | Globals.GLS_SRCBLEND_ZERO, Globals.GLS_DSTBLEND_SRC_COLOR | Globals.GLS_SRCBLEND_ZERO,
	          BlendState.NonPremultiplied, Globals.GLS_DSTBLEND_ZERO | Globals.GLS_SRCBLEND_DST_COLOR ),

	        new collapse_t( 0, Globals.GLS_DSTBLEND_ONE | Globals.GLS_SRCBLEND_ONE,
	          BlendState.Additive, 0 ),

	        new collapse_t( Globals.GLS_DSTBLEND_ONE | Globals.GLS_SRCBLEND_ONE, Globals.GLS_DSTBLEND_ONE | Globals.GLS_SRCBLEND_ONE,
	          BlendState.Additive, Globals.GLS_DSTBLEND_ONE | Globals.GLS_SRCBLEND_ONE ),
        #if false
	        new collapse_t( 0, GLS_DSTBLEND_ONE_MINUS_SRC_ALPHA | GLS_SRCBLEND_SRC_ALPHA,
	          GL_DECAL, 0 ),
        #endif
        };

        //
        // ShiftStagesDown
        //
        private void ShiftStagesDown(int basestage)
        {
            for (int i = basestage+1; i < shader.stages.Length; i++)
            {
                shader.stages[i - 1] = shader.stages[i];
            }
        }

        /*
        ================
        CollapseMultitexture

        Attempt to combine two stages into a single multitexture stage
        FIXME: I think modulated add + modulated add collapses incorrectly
        =================
        */
        public bool CollapseMultitexture()
        {
	        int abits, bbits;
	        int i;
	        textureBundle_t tmpBundle;

	        // make sure both stages are active
	        if ( !shader.stages[0].active || !shader.stages[1].active ) {
		        return false;
	        }

	        abits = shader.stages[0].stateBits;
	        bbits = shader.stages[1].stateBits;

	        // make sure that both stages have identical state other than blend modes
	        if ( ( abits & ~( Globals.GLS_DSTBLEND_BITS | Globals.GLS_SRCBLEND_BITS | Globals.GLS_DEPTHMASK_TRUE ) ) !=
		         ( bbits & ~( Globals.GLS_DSTBLEND_BITS | Globals.GLS_SRCBLEND_BITS | Globals.GLS_DEPTHMASK_TRUE ) ) ) {
		        return false;
	        }

	        abits &= ( Globals.GLS_DSTBLEND_BITS | Globals.GLS_SRCBLEND_BITS );
	        bbits &= ( Globals.GLS_DSTBLEND_BITS | Globals.GLS_SRCBLEND_BITS );

	        // search for a valid multitexture blend function
	        for ( i = 0; i < collapse.Length ; i++ ) {
		        if ( abits == collapse[i].blendA
			         && bbits == collapse[i].blendB ) {
			        break;
		        }
	        }

            int lastNonLightmappedStage = 0;
            for (i = 0; i < shader.stages.Length; i++)
            {
                if (shader.stages[i] == null)
                {
                    break;
                }

                if (shader.stages[i].bundle[0].isLightmap)
                {
                    // If the first stage is a lightmap, move stage 1 bundle 1 to bundle 0.
                    if (lastNonLightmappedStage == 0)
                    {
#if false
                        shader.stages[i].bundle[1] = shader.stages[i + 1].bundle[0];
#endif
                        tmpBundle = shader.stages[i].bundle[0]; // Set tmpBundle as the current image stage(the lightmap).
                        shader.stages[i].bundle[0] = shader.stages[i + 1].bundle[0]; // Diffuse map taken from the next stage.
                        shader.stages[i].bundle[1] = tmpBundle; // Bundle 1 is the lightmap.

                        ShiftStagesDown(i + 1);
                    }
                    else
                    {
#if false
                        tmpBundle = shader.stages[i - 1].bundle[0];
                        shader.stages[i - 1].bundle[0] = shader.stages[i].bundle[0];
                        shader.stages[i - 1].bundle[1] = tmpBundle;
#endif
                        shader.stages[i - 1].bundle[1] = shader.stages[i].bundle[0]; 
                        ShiftStagesDown(i);
                    }

                    return true;
                }
                else
                {
                    lastNonLightmappedStage++;
                }
            }

	        // nothing found
	        if ( /*collapse[i].blendA == -1*/ i >= collapse.Length ) {
		        return false;
	        }

	        // GL_ADD is a separate extension
// JV - not needed for directx
#if false
	        if ( collapse[i].multitextureEnv == BlendState.Additive && !glConfig.textureEnvAddAvailable ) {
		        return qfalse;
	        }
#endif
// JV END
	        // make sure waveforms have identical parameters
	        if ( ( shader.stages[0].rgbGen != shader.stages[1].rgbGen ) ||
		         ( shader.stages[0].alphaGen != shader.stages[1].alphaGen ) ) {
		        return false;
	        }

	        // an add collapse can only have identity colors
	        if ( collapse[i].multitextureEnv == BlendState.Additive && shader.stages[0].rgbGen != colorGen_t.CGEN_IDENTITY ) {
		        return false;
	        }

	        if ( shader.stages[0].rgbGen == colorGen_t.CGEN_WAVEFORM ) {
		        if ( shader.stages[0].rgbWave == shader.stages[1].rgbWave ) {
			        return false;
		        }
	        }
	        if ( shader.stages[0].alphaGen == (alphaGen_t)colorGen_t.CGEN_WAVEFORM ) {
		        if ( shader.stages[0].alphaWave == shader.stages[1].alphaWave ) {
			        return false;
		        }
	        }

	        // set the new blend state bits
	        shader.multitextureEnv = collapse[i].multitextureEnv;
	        shader.stages[0].stateBits &= ~( Globals.GLS_DSTBLEND_BITS | Globals.GLS_SRCBLEND_BITS );
	        shader.stages[0].stateBits |= collapse[i].multitextureBlend;


	      //  memmove( &stages[1], &stages[2], sizeof( stages[0] ) * ( MAX_SHADER_STAGES - 2 ) );
	      //  memset( &stages[MAX_SHADER_STAGES - 1], 0, sizeof( stages[0] ) );

	        return true;
        }


        /*
        ===============
        NameToSrcBlendMode
        ===============
        */
        private Blend NameToSrcBlendMode( string name ) {
            name = name.ToUpper();

	        if ( name == "GL_ONE" ) {
		        return Blend.One;
	        } else if ( name == "GL_ZERO" )    {
                return Blend.Zero;
	        } else if ( name == "GL_DST_COLOR" )    {
                return Blend.DestinationColor;
	        } else if ( name == "GL_ONE_MINUS_DST_COLOR" )    {
                return Blend.InverseSourceColor;
	        } else if ( name == "GL_SRC_ALPHA" )    {
                return Blend.SourceAlpha;
	        } else if ( name == "GL_ONE_MINUS_SRC_ALPHA" )    {
                return Blend.InverseSourceAlpha;
	        } else if ( name == "GL_DST_ALPHA" )    {
                return Blend.DestinationAlpha;
	        } else if ( name == "GL_ONE_MINUS_DST_ALPHA" )    {
                return Blend.InverseDestinationAlpha;
	        } else if ( name == "GL_SRC_ALPHA_SATURATE"  )    {
                return Blend.SourceAlphaSaturation;
	        }

	        Engine.common.Warning( "unknown blend mode '%s' in shader '%s', substituting GL_ONE\n", name, shader.name );
            return Blend.One;
        }

        /*
        ===============
        NameToDstBlendMode
        ===============
        */
        private Blend NameToDstBlendMode( string name ) {
            name = name.ToUpper();
	        if ( name == "GL_ONE" ) {
		        return Blend.One;
	        } else if ( name == "GL_ZERO" )    {
                return Blend.Zero;
	        } else if ( name == "GL_SRC_ALPHA" )    {
                return Blend.SourceAlpha;
	        } else if ( name == "GL_ONE_MINUS_SRC_ALPHA" )    {
                return Blend.InverseSourceAlpha;
	        } else if ( name == "GL_DST_ALPHA" )    {
                return Blend.DestinationAlpha;
	        } else if ( name == "GL_ONE_MINUS_DST_ALPHA" )    {
                return Blend.InverseDestinationAlpha;
	        } else if ( name == "GL_SRC_COLOR" )    {
                return Blend.SourceColor;
	        } else if ( name == "GL_ONE_MINUS_SRC_COLOR" )    {
                return Blend.InverseSourceColor;
	        }

            Engine.common.Warning("unknown blend mode '%s' in shader '%s', substituting GL_ONE\n", name, shader.name);
            return Blend.One;
        }

        /*
        ===============
        NameToGenFunc
        ===============
        */
        private genFunc_t NameToGenFunc( string funcname ) {
            funcname = funcname.ToLower();
	        if ( funcname == "sin"  ) {
                return genFunc_t.GF_SIN;
	        } else if ( funcname == "square"  )    {
                return genFunc_t.GF_SQUARE;
	        } else if ( funcname == "triangle"  )    {
                return genFunc_t.GF_TRIANGLE;
	        } else if ( funcname =="sawtooth"  )    {
                return genFunc_t.GF_SAWTOOTH;
	        } else if ( funcname == "inversesawtooth"  )    {
                return genFunc_t.GF_INVERSE_SAWTOOTH;
	        } else if ( funcname =="noise"  )    {
                return genFunc_t.GF_NOISE;
	        }

	        Engine.common.Warning("invalid genfunc name '%s' in shader '%s'\n", funcname, shader.name );
            return genFunc_t.GF_SIN;
        }

        /*
        ===================
        ParseWaveForm
        ===================
        */
        private void ParseWaveForm(ref idParser parser, out waveForm_t wave) {
	        string token;
            
            wave = new waveForm_t();

	        token = parser.NextToken;
	        wave.func = NameToGenFunc( token );

	        // BASE, AMP, PHASE, FREQ
            wave.base_c = parser.NextFloat;
            wave.amplitude = parser.NextFloat;
            wave.phase = parser.NextFloat;
            wave.frequency = parser.NextFloat;
        }

        //
        // ParseDeform
        //
        private void ParseDeform(ref idParser parser)
        {
            string token;
            deformStage_t ds;

            if (shader.numDeforms == idMaterialBase.MAX_SHADER_DEFORMS)
            {
                Engine.common.Warning("MAX_SHADER_DEFORMS in '%s'\n", shader.name);
                return;
            }

            shader.deforms[shader.numDeforms] = new deformStage_t();
            ds = shader.deforms[shader.numDeforms];
            shader.numDeforms++;

            token = parser.NextToken;
            if (token == "projectionShadow")
            {
                ds.deformation = deform_t.DEFORM_PROJECTION_SHADOW;
                return;
            }

            if (token == "autosprite")
            {
                ds.deformation = deform_t.DEFORM_AUTOSPRITE;
                return;
            }

            if (token == "autosprite2")
            {
                ds.deformation = deform_t.DEFORM_AUTOSPRITE2;
                return;
            }

            if (token == "text")
            {
                int n;

                n = token[4] - '0';
                if (n < 0 || n > 7)
                {
                    n = 0;
                }
                ds.deformation = deform_t.DEFORM_TEXT0 + n;
                return;
            }

            if (token == "bulge")
            {
                ds.bulgeWidth = parser.NextFloat;
                ds.bulgeHeight = parser.NextFloat;
                ds.bulgeSpeed = parser.NextFloat;

                ds.deformation = deform_t.DEFORM_BULGE;
                return;
            }

            if (token == "wave")
            {
                token = parser.NextToken;

                if (float.Parse(token) != 0)
                {
                    ds.deformationSpread = 1.0f / float.Parse(token);
                }
                else
                {
                    ds.deformationSpread = 100.0f;
                    Engine.common.Warning("illegal div value of 0 in deformVertexes command for shader '%s'\n", shader.name);
                }

                ParseWaveForm(ref parser, out ds.deformationWave);
                ds.deformation = deform_t.DEFORM_WAVE;
                return;
            }

            if (token == "normal")
            {
                ds.deformationWave.amplitude = parser.NextFloat;
                ds.deformationWave.frequency = parser.NextFloat;

                ds.deformation = deform_t.DEFORM_NORMALS;
                return;
            }

            if (token == "move")
            {
                int i;

                ds.moveVector = new idVector3();
                for (i = 0; i < 3; i++)
                {
                    ds.moveVector[i] = parser.NextFloat;
                }

                ParseWaveForm(ref parser, out ds.deformationWave);
                ds.deformation = deform_t.DEFORM_MOVE;
                return;
            }

            Engine.common.Warning("unknown deformVertexes subtype '%s' found in shader '%s'\n", token, shader.name);
        }

        //
        // ParseSurfaceParm
        //
        private void ParseSurfaceParm(ref idParser parser)
        {
            string token;
	        int numInfoParms = infoParms.Length;
	        int i;

	        token = parser.NextToken;
	        for ( i = 0 ; i < numInfoParms ; i++ ) {
		        if ( token == infoParms[i].name ) {
			        shader.surfaceFlags |= infoParms[i].surfaceFlags;
			        shader.contentFlags |= (int)infoParms[i].contents;
        #if false
			        if ( infoParms[i].clearSolid != 0 ) {
				        si->contents &= ~CONTENTS_SOLID;
			        }
        #endif
			        break;
		        }
	        }
        }

        //
        // ParseSkyParms
        //
        private string[] suf = new string[6]{"rt", "bk", "lf", "ft", "up", "dn"};
        private void ParseSkyParms(ref idParser parser)
        {
            string token;
            string pathname;
	        int i;

	        // outerbox
            token = parser.NextToken;
	        if ( token != "-" ) {
		        for ( i = 0 ; i < 6 ; i++ ) {
                    pathname = token + "_" + suf[i];
                    if (Engine.fileSystem.FileExists(pathname + ".xnb") == false)
                    {
                        shader.sky.outerbox[i] = Globals.tr.defaultImage;
                        continue;
                    }

			        shader.sky.outerbox[i] = Engine.imageManager.FindImageFile(pathname, true, true, SamplerState.LinearClamp );
			        if ( shader.sky.outerbox[i] == null ) {
				        shader.sky.outerbox[i] = Globals.tr.defaultImage;
			        }
		        }
	        }

	        // cloudheight
            shader.sky.cloudHeight = float.Parse(parser.NextToken);
	        if ( shader.sky.cloudHeight <= 0 ) {
		        shader.sky.cloudHeight = 512;
	        }
	        //R_InitSkyTexCoords( shader.sky.cloudHeight );


	        // innerbox
            token = parser.NextToken;
	        if ( token != "-" ) {
		        for ( i = 0 ; i < 6 ; i++ ) {
                    pathname = token + "_" + suf[i];
                    shader.sky.innerbox[i] = Engine.imageManager.FindImageFile(pathname, true, true, SamplerState.LinearClamp);
                    if (shader.sky.innerbox[i] == null)
                    {
                        shader.sky.innerbox[i] = Globals.tr.defaultImage;
                    }
		        }
	        }

            shader.isSky = true;
        }

        //
        //R_SetFog 
        // 
        private void R_SetFog(int fogvar, int var1, int var2, float r, float g, float b, float density)
        {

        }

        //
        // ParseSort
        //
        private void ParseSort(ref idParser parser)
        {
            string token;

            token = parser.NextToken.ToLower();

            if (token == "portal")
            {
            //    shader.sort = (float)shaderSort_t.SS_PORTAL;
            }
            else if (token ==  "sky")
            {
            //    shader.sort = (float)shaderSort_t.SS_ENVIRONMENT;
            }
            else if (token ==  "opaque")
            {
            //    shader.sort = (float)shaderSort_t.SS_OPAQUE;
            }
            else if (token == "decal")
            {
           //     shader.sort = (float)shaderSort_t.SS_DECAL;
            }
            else if (token == "seeThrough")
            {
           //     shader.sort = (float)shaderSort_t.SS_SEE_THROUGH;
            }
            else if (token == "banner")
            {
            //    shader.sort = (float)shaderSort_t.SS_BANNER;
            }
            else if (token == "additive")
            {
           //     shader.sort = (float)shaderSort_t.SS_BLEND1;
            }
            else if (token == "nearest")
            {
           //     shader.sort = (float)shaderSort_t.SS_NEAREST;
            }
            else if (token == "underwater")
            {
            //    shader.sort = (float)shaderSort_t.SS_UNDERWATER;
            }
            else
            {
                float.Parse(token);
            }
        }

        //
        // ParseShader
        //
        public bool ParseShader(string name, string buffer)
        {
            idParser parser = new idParser(buffer);
            int s = 0;
            this.name = name;
            shader.name = name;

           // parser.ExpectNextToken("{");

            while (true)
            {
                string token = parser.NextToken;

                if (parser.ReachedEndOfBuffer == true || token == null)
                {
                    // This is allowed because the trailing bracket is removed by the content builder.
                   // Engine.common.Warning("no concluding '}' in shader %s\n", name);
                   // return false;
                    FinishShader();
                    return true;
                }

                token = token.ToLower();

                // end of shader definition
		        if ( token == "}" ) {
			        break;
		        }
		        // stage definition
		        else if ( token == "{" ) {
			        if ( !ParseStage( out shader.stages[s], ref parser ) ) {
				        return false;
			        }
			        shader.stages[s].active = true;
			        s++;
			        continue;
		        }
		        // skip stuff that only the QuakeEdRadient needs
		        else if ( token.Contains( "qer" ) ) {
			        parser.ParseRestOfLine();
			        continue;
		        }
		        // sun parms
		        else if ( token == "q3map_sun" ) {
			        float a, b;

			        Globals.tr.sunLight = new idLib.Math.idVector3();
			        Globals.tr.sunLight[0] = float.Parse( parser.NextToken );
                    Globals.tr.sunLight[1] = float.Parse( parser.NextToken );
                    Globals.tr.sunLight[2] = float.Parse( parser.NextToken );

                    Globals.tr.sunLight.Normalize();
			        
			        a = float.Parse( parser.NextToken );
                    Globals.tr.sunLight *= a;

			        a = float.Parse( parser.NextToken );
			        a = (float)(a / 180 * System.Math.PI);

                    b = float.Parse( parser.NextToken );
			        b = (float)(b / 180 * System.Math.PI);

                    Globals.tr.sunDirection = new idLib.Math.idVector3();
			        Globals.tr.sunDirection[0] = (float)(Math.Cos( a ) * Math.Cos( b ));
			        Globals.tr.sunDirection[1] = (float)(Math.Sin( a ) * Math.Cos( b ));
			        Globals.tr.sunDirection[2] = (float)(Math.Sin( b ));
		        } else if ( token == "deformvertexes"  )    {
			        ParseDeform( ref parser );
			        continue;
		        } else if ( token == "tesssize" )    {
			        parser.ParseRestOfLine();
			        continue;
		        } else if ( token == "clamptime" )    {
			        token = parser.NextToken;
			        if ( token != null ) {
				        shader.clampTime = float.Parse( token );
			        }
		        }
		        // skip stuff that only the q3map needs
		        else if ( token.Contains("q3map") ) {
			        parser.ParseRestOfLine();
			        continue;
		        }
		        // skip stuff that only q3map or the server needs
		        else if ( token == "surfaceparm" ) {
			        ParseSurfaceParm( ref parser );
			        continue;
		        }
		        // no mip maps
		        else if ( token == "nomipmaps" ) {
			        shader.noMipMaps = true;
			        shader.noPicMip = true;
			        continue;
		        }
		        // no picmip adjustment
		        else if ( token == "nopicmip" ) {
			        shader.noPicMip = true;
			        continue;
		        }
		        // character picmip adjustment
		        else if ( token == "picmip2"  ) {
			        shader.characterMip = true;
			        continue;
		        }
		        // polygonOffset
		        else if ( token == "polygonoffset" ) {
			        shader.polygonOffset = true;
			        continue;
		        }
		        // entityMergable, allowing sprite surfaces from multiple entities
		        // to be merged into one batch.  This is a savings for smoke
		        // puffs and blood, but can't be used for anything where the
		        // shader calcs (not the surface function) reference the entity color or scroll
		        else if ( token == "entitymergable" ) {
			        shader.entityMergable = true;
			        continue;
		        }
		        // fogParms
		        else if ( token == "fogparms" ) {
                    shader.fogParms = new fogParms_t();
                    shader.fogParms.color = new idLib.Math.idVector3();
                    parser.NextVector3(ref shader.fogParms.color);

			        token = parser.NextToken;
			        shader.fogParms.depthForOpaque = float.Parse( token );

			        // skip any old gradient directions
			        //SkipRestOfLine( text );
			        continue;
		        }
		        // portal
		        else if ( token == "portal" ) {
			      //  shader.sort = (float)shaderSort_t.SS_PORTAL;
			        continue;
		        }
		        // skyparms <cloudheight> <outerbox> <innerbox>
		        else if ( token == "skyparms" ) {
			        ParseSkyParms( ref parser );
			        continue;
		        }
		        // This is fixed fog for the skybox/clouds determined solely by the shader
		        // it will not change in a level and will not be necessary
		        // to force clients to use a sky fog the server says to.
		        // skyfogvars <(r,g,b)> <dist>
		        else if ( token == "skyfogvars" ) {
			        idVector3 fogColor = new idVector3();

                    parser.NextVector3( ref fogColor );
			        float fog_density = float.Parse(parser.NextToken);

			        if ( fog_density > 1 ) {
				        Engine.common.Warning( "last value for skyfogvars is 'density' which needs to be 0.0-1.0\n" );
				        continue;
			        }

			        R_SetFog( (int)fogType.FOG_SKY, 0, 5, fogColor[0], fogColor[1], fogColor[2], fog_density );
			        continue;
		        } else if ( token == "sunshader" )        {
			        Globals.tr.sunShaderName = parser.NextToken;
		        }
        //----(SA)	added
		        else if ( token == "lightgridmulamb" ) { // ambient multiplier for lightgrid
                    float lightgrid_ambient = float.Parse(parser.NextToken);

			        if ( lightgrid_ambient > 0 ) {
				        Globals.tr.lightGridMulAmbient = lightgrid_ambient;
			        }
		        } else if ( token == "lightgridmuldir" )        { // directional multiplier for lightgrid
			        float lightgrid_dir = float.Parse(parser.NextToken);

			        if ( lightgrid_dir > 0 ) {
				        Globals.tr.lightGridMulDirected = lightgrid_dir;
			        }
		        }
        //----(SA)	end
		        else if ( token == "waterfogvars"  ) {
			        idVector3 watercolor = new idVector3();
			        float fogvar;
			        string fogString = null;

                    parser.NextVector3(ref watercolor);
                    token = parser.NextToken;

			        fogvar = float.Parse( token );

			        //----(SA)	right now allow one water color per map.  I'm sure this will need
			        //			to change at some point, but I'm not sure how to track fog parameters
			        //			on a "per-water volume" basis yet.

			        if ( fogvar == 0 ) {       // '0' specifies "use the map values for everything except the fog color
				        // TODO
			        } else if ( fogvar > 1 )      { // distance "linear" fog
				        fogString =  "0 " + fogvar + " 1.1 " + watercolor[0] + " " + watercolor[1] + " " + watercolor[2] + " 200";
        //				R_SetFog(FOG_WATER, 0, fogvar, watercolor[0], watercolor[1], watercolor[2], 1.1);
			        } else {                      // density "exp" fog
                        fogString = "0 5 " + fogvar + " " + watercolor[0] + " " + watercolor[1] + " " + watercolor[2] + " 200";
        //				R_SetFog(FOG_WATER, 0, 5, watercolor[0], watercolor[1], watercolor[2], fogvar);
			        }

        //		near
        //		far
        //		density
        //		r,g,b
        //		time to complete
			        Engine.cvarManager.Cvar_Set( "r_waterFogColor", fogString, true );

			        continue;
		        }
		        // fogvars
		        else if ( token == "fogvars" ) {
			        idVector3 fogColor = new idVector3();
			        float fogDensity;
			        int fogFar;

                    parser.NextVector3(ref fogColor);
                    token = parser.NextToken;
			        
			        //----(SA)	NOTE:	fogFar > 1 means the shader is setting the farclip, < 1 means setting
			        //					density (so old maps or maps that just need softening fog don't have to care about farclip)

			        fogDensity = float.Parse( token );
			        if ( fogDensity >= 1 ) { // linear
				        fogFar      = (int)fogDensity;
			        } else {
				        fogFar      = 5;
			        }

        //			R_SetFog(FOG_MAP, 0, fogFar, fogColor[0], fogColor[1], fogColor[2], fogDensity);
//			        ri.Cvar_Set( "r_mapFogColor", va( "0 %d %f %f %f %f 0", fogFar, fogDensity, fogColor[0], fogColor[1], fogColor[2] ) );
                    string fog_colorstr = "0 " + fogFar + " " + fogDensity + " " + fogColor[0] + " " + fogColor[1] + " " + fogColor[2];
                    Engine.cvarManager.Cvar_Set( "r_mapFogColor", fog_colorstr, true );
        //			R_SetFog(FOG_CMD_SWITCHFOG, FOG_MAP, 50, 0, 0, 0, 0);

			        continue;
		        }
		        // done.
		        // Ridah, allow disable fog for some shaders
		        else if ( token == "nofog" ) {
			        shader.noFog = true;
			        continue;
		        }
		        // done.
		        // RF, allow each shader to permit compression if available
		        else if ( token == "allowcompress" ) {
			        Globals.tr.allowCompress = 1;
			        continue;
		        } else if ( token == "nocompress" )   {
			        Globals.tr.allowCompress = -1;
			        continue;
		        }
		        // done.
		        // light <value> determines flaring in q3map, not needed here
		        else if ( token == "light" ) {
                    token = parser.NextToken;
			        continue;
		        }
		        // cull <face>
		        else if ( token == "cull" ) {
                    token = parser.NextToken;

			        if ( token == "none" || token == "twosided" || token == "disable" ) {
				        shader.cullType = cullType_t.CT_TWO_SIDED;
			        } else if ( token == "back" || token == "backside" || token == "backsided" )      {
                        shader.cullType = cullType_t.CT_BACK_SIDED;
			        } else
			        {
				        Engine.common.Warning("invalid cull parm '%s' in shader '%s'\n", token, shader.name );
			        }
			        continue;
		        }
		        // sort
		        else if ( token == "sort" ) {
			        ParseSort( ref parser );
			        continue;
		        } else
		        {
			        Engine.common.Warning( "unknown general shader parameter '%s' in '%s'\n", token, shader.name );
			        return false;
		        }
            }

            

            //
            // ignore shaders that don't have any stages, unless it is a sky or fog
            //
            if (s == 0 && !shader.isSky && (shader.contentFlags & surfaceFlags.CONTENTS_FOG) == 0)
            {
                return false;
            }

            FinishShader();
            shader.explicitlyDefined = true;

            return true;
        }

        //
        // GetName
        //
        public override string GetName()
        {
            return name;
        }
    }

    //
    // idMaterialManagerLocal
    //
    public class idMaterialManagerLocal : idMaterialManager
    {
        idMaterialLookupTable mtrLookupTable = new idMaterialLookupTable();
        List<idMaterial> mtrpool = new List<idMaterial>();

        //
        // Init
        //
        public override void Init()
        {
            idFileList mtrList = Engine.fileSystem.ListFiles("scripts", ".mtr");

            Engine.common.Printf("Loading Materials...\n");
            for (int i = 0; i < mtrList.Count; i++)
            {
                Engine.common.Printf("...%s\n", mtrList[i]);
                idMaterialLookupTable shader = Engine.fileSystem.ReadContent<idMaterialLookupTable>("scripts/" + mtrList[i]);
                mtrLookupTable.CombineTable(ref shader);
            }

            Engine.fileSystem.FreeFileList(ref mtrList);
        }

        //
        // Shutdown
        //
        public override void Shutdown()
        {
            
        }

        //
        // CreateMaterial
        //
        private idMaterial CreateMaterial(string name, string buffer, int lightmapIndex)
        {
            idMaterialLocal mtr = new idMaterialLocal(lightmapIndex);

            if (!mtr.ParseShader(name, buffer))
            {
                return null;
            }

            mtr.shader.hashValue = idString.GenerateHashValue(name);

            mtrpool.Add(mtr);

            return mtrpool[mtrpool.Count - 1];
        }

        //
        // CreateImplicitMaterial
        //
        private idMaterial CreateImplicitMaterial(string name, string defaultImage, int lightmapIndex)
        {
            string defaultMtrLightmapped = "{" + "\n" +
                                        "map " + defaultImage + "\n" +
                                      //  "blendFunc GL_DST_COLOR GL_ZERO" + "\n" +
                                        "rgbGen identity" + "\n" +
                                    "}" + "\n" +
                                    "{" + "\n" +
                                     "map $lightmap" + "\n" +
                                     "rgbGen identity" + "\n" +
                                   //  "  tcMod scale 1 1" + "\n" +
                                  //   "   tcMod turb 0 .1 0 .1" + "\n" +
                                    "}" + "\n" +
                                "}\n";
            string defaultMtr =  "{" + "\n" +
                            "map " + defaultImage + "\n" +
               //             "blendFunc GL_DST_COLOR GL_ZERO" + "\n" +
                            "rgbGen identity" + "\n" +
                        "}" + "\n" +
                    "}\n";
            Engine.common.Warning("Creating implicit material for " + name + "\n");
            if (lightmapIndex < 0)
            {
                return CreateMaterial(name, defaultMtr, lightmapIndex);
            }

            return CreateMaterial(name, defaultMtrLightmapped, lightmapIndex);
        }

        //
        // FindMaterial
        //
        public override idMaterial FindMaterial(string name, int lightmapIndex)
        {
            int hashValue = idString.GenerateHashValue(name);
            // Check to see if the material is already loaded.
            for (int i = 0; i < mtrpool.Count; i++)
            {
                if (idMaterialLocal.GetMaterialBase(mtrpool[i]).hashValue == hashValue)
                {
                    return mtrpool[i];
                }
            }

            // Try to find the material in the table and load it in.
            string mtrbuffer = mtrLookupTable.FindMaterialInTable(hashValue);

            // Try the shader name with out the extension if its present.
            if (mtrbuffer == null)
            {
                string name2 = Engine.fileSystem.RemoveExtensionFromPath(name);
                int newHashVal = idString.GenerateHashValue(name2);
                mtrbuffer = mtrLookupTable.FindMaterialInTable(newHashVal);
            }

            if (mtrbuffer != null)
            {
                idMaterial mtr = CreateMaterial(name, mtrbuffer, lightmapIndex);

                if (mtr == null)
                {
                    Engine.common.Warning("Failed to load shader %s...\n", name);
                    return CreateImplicitMaterial(name, "*white", lightmapIndex);
                }

                return mtr;
            }

            // Check to see if we can find a valid image.
            string filepath = Engine.fileSystem.RemoveExtensionFromPath(name);
            if (Engine.fileSystem.FileExists(filepath + ".xnb") == false)
            {
                Engine.common.Warning("Failed to find texture " + filepath + "\n");
                return CreateImplicitMaterial(name, "*white", lightmapIndex);
            }

            return CreateImplicitMaterial(name, filepath, lightmapIndex);
        }
    }
}
