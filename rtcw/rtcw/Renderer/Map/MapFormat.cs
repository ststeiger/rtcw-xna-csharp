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

// mapformat.cs (c) 2010 jv Software
//

using Microsoft.Xna.Framework;
using idLib.Math;
using idLib.Engine.Public;


namespace rtcw.Renderer.Map
{
    //
    // idMapFormat
    //
    public class idMapFormat
    {
        //
        // ColorBytes4
        //
        public static uint ColorBytes4( float rf, float gf, float bf, float af ) {
	        uint i;

	        int r = (int)(rf * 255);
	        int g = (int)(gf * 255);
	        int b = (int)(bf * 255);
	        int a = (int)(af * 255);

	        return (uint)((r)|(g<<8)|(b<<16)|(255<<24));
        }

        /*
        ===============
        ColorShiftLightingBytes

        ===============
        */
        public static void ColorShiftLightingBytes( byte[] inrgb, int inpos, int colorpos, ref Color[] color ) {
	        int shift, r, g, b;

	        // shift the color data based on overbright range
	        shift = Globals.r_mapOverBrightBits.GetValueInteger(); // - tr.overbrightBits;

	        // shift the data based on overbright range
	        r = inrgb[inpos + 0] << shift;
	        g = inrgb[inpos + 1] << shift;
	        b = inrgb[inpos + 2] << shift;

	        // normalize by color instead of saturating to white
	        if ( ( r | g | b ) > 255 ) {
		        int max;

		        max = r > g ? r : g;
		        max = max > b ? max : b;
		        r = r * 255 / max;
		        g = g * 255 / max;
		        b = b * 255 / max;
	        }

            color[colorpos] = new Color( r, g, b, inrgb[inpos] );
        }

        /*
        ===============
        ColorShiftLightingBytes

        ===============
        */
        public static void ColorShiftLightingBytes( Color[] inrgb, int inpos, int colorpos, ref Color[] color ) {
	        int shift, r, g, b;

	        // shift the color data based on overbright range
	        shift = Globals.r_mapOverBrightBits.GetValueInteger(); // - tr.overbrightBits;

	        // shift the data based on overbright range
	        r = inrgb[inpos].R << shift;
	        g = inrgb[inpos].G << shift;
	        b = inrgb[inpos].B << shift;

	        // normalize by color instead of saturating to white
	        if ( ( r | g | b ) > 255 ) {
		        int max;

		        max = r > g ? r : g;
		        max = max > b ? max : b;
		        r = r * 255 / max;
		        g = g * 255 / max;
		        b = b * 255 / max;
	        }

            color[colorpos].R = (byte)r;
            color[colorpos].G = (byte)g;
            color[colorpos].B = (byte)b;
            color[colorpos].A = (byte)inrgb[inpos + 3].A;
        }

        //
        // HSVtoRGB
        //
        public static void HSVtoRGB( float h, float s, float v, ref idVector3 rgb ) {
	        int i;
	        float f;
	        float p, q, t;

	        h *= 5;

	        i = (int)System.Math.Floor( h );
	        f = h - i;

	        p = v * ( 1 - s );
	        q = v * ( 1 - s * f );
	        t = v * ( 1 - s * ( 1 - f ) );

	        switch ( i )
	        {
	        case 0:
		        rgb[0] = v;
		        rgb[1] = t;
		        rgb[2] = p;
		        break;
	        case 1:
		        rgb[0] = q;
		        rgb[1] = v;
		        rgb[2] = p;
		        break;
	        case 2:
		        rgb[0] = p;
		        rgb[1] = v;
		        rgb[2] = t;
		        break;
	        case 3:
		        rgb[0] = p;
		        rgb[1] = q;
		        rgb[2] = v;
		        break;
	        case 4:
		        rgb[0] = t;
		        rgb[1] = p;
		        rgb[2] = v;
		        break;
	        case 5:
		        rgb[0] = v;
		        rgb[1] = p;
		        rgb[2] = q;
		        break;
	        }
        }

        public const int BSP_IDENT =  ( ( 'P' << 24 ) + ( 'S' << 16 ) + ( 'B' << 8 ) + 'I' );
        // little-endian "IBSP"

        public const int BSP_VERSION  =       47;  // updated (9/12/2001) to sync everything up pre-beta


        // there shouldn't be any problem with increasing these values at the
        // expense of more memory allocation in the utilities
        //#define	MAX_MAP_MODELS		0x400
        public const int AX_MAP_MODELS     =  0x800;
        public const int MAX_MAP_BRUSHES   =  0x8000;
        public const int MAX_MAP_ENTITIES  =  0x800;
        public const int MAX_MAP_ENTSTRING =  0x40000;
        public const int MAX_MAP_SHADERS   =  0x400;

        public const int MAX_MAP_AREAS     =  0x100;   // MAX_MAP_AREA_BYTES in q_shared must match!
        public const int MAX_MAP_FOGS      =  0x100;
        public const int MAX_MAP_PLANES    =  0x20000;
        public const int MAX_MAP_NODES     =  0x20000;
        public const int MAX_MAP_BRUSHSIDES = 0x20000;
        public const int MAX_MAP_LEAFS      = 0x20000;
        public const int MAX_MAP_LEAFFACES  = 0x20000;
        public const int MAX_MAP_LEAFBRUSHES = 0x40000;
        public const int MAX_MAP_PORTALS     = 0x20000;
        public const int MAX_MAP_LIGHTING    = 0x800000;
        public const int MAX_MAP_LIGHTGRID   = 0x800000;
        public const int MAX_MAP_VISIBILITY  = 0x200000;

        public const int MAX_MAP_DRAW_SURFS  = 0x20000;
        public const int MAX_MAP_DRAW_VERTS  = 0x80000;
        public const int MAX_MAP_DRAW_INDEXES  =  0x80000;


        // key / value pair sizes in the entities lump
        public const int MAX_KEY          =   32;
        public const int MAX_VALUE        =   1024;

        // the editor uses these predefined yaw angles to orient entities up or down
        public const int ANGLE_UP         =   -1;
        public const int ANGLE_DOWN       =   -2;

        public const int LIGHTMAP_WIDTH   =   128;
        public const int LIGHTMAP_HEIGHT  =   128;

        public const int MAX_WORLD_COORD  =   ( 128 * 1024 );
        public const int MIN_WORLD_COORD  =   ( -128 * 1024 );
        public const int WORLD_SIZE       =   ( MAX_WORLD_COORD - MIN_WORLD_COORD );

        public const int LUMP_ENTITIES    =   0;
        public const int LUMP_SHADERS     =   1;
        public const int LUMP_PLANES      =   2;
        public const int LUMP_NODES       =   3;
        public const int LUMP_LEAFS       =   4;
        public const int LUMP_LEAFSURFACES =  5;
        public const int LUMP_LEAFBRUSHES  =  6;
        public const int LUMP_MODELS       =  7;
        public const int LUMP_BRUSHES      =  8;
        public const int LUMP_BRUSHSIDES   =  9;
        public const int LUMP_DRAWVERTS    =  10;
        public const int LUMP_DRAWINDEXES  =  11;
        public const int LUMP_FOGS         =  12;
        public const int LUMP_SURFACES     =  13;
        public const int LUMP_LIGHTMAPS    =  14;
        public const int LUMP_LIGHTGRID    =  15;
        public const int LUMP_VISIBILITY   =  16;
        public const int HEADER_LUMPS      =  17;
        
        //
        // lump_t
        //
        public struct lump_t {
	        public int fileofs, filelen;
        };

        //
        // idMapSurfaceType
        //
        public enum idMapSurfaceType {
	        MST_BAD,
	        MST_PLANAR,
	        MST_PATCH,
	        MST_TRIANGLE_SOUP,
	        MST_FLARE
        };

        //
        // idMapHeader
        //
        public struct idMapHeader {
	        public int ident;
	        public int version;

	        public lump_t[] lumps;

            public void InitFromFile( ref idFile file )
            {
                file.Seek(idFileSeekOrigin.FS_SEEK_SET, 0);
                ident = file.ReadInt();
                version = file.ReadInt();

                if(ident != BSP_IDENT)
                {
                    Engine.common.ErrorFatal( "R_InitMapFromFile: Bsp Identifcation invalid.\n" );
                    return;
                }

                if(version != BSP_VERSION)
                {
                    Engine.common.ErrorFatal( "R_InitMapFromFile: Bsp version invalid expected " + BSP_VERSION + " bsp version " + version + "\n" );
                    return;
                }

                lumps = new lump_t[ HEADER_LUMPS ];
                for( int i = 0; i < HEADER_LUMPS; i++ )
                {
                    lumps[i].fileofs = file.ReadInt();
                    lumps[i].filelen = file.ReadInt();
                }
            }
        };

        //
        // idMapModel
        //
        public struct idMapModel {
	        public idVector3 mins;
            public idVector3 maxs;
	        public int firstSurface;
            public int numSurfaces;
	        public int firstBrush;
            public int numBrushes;

            public const int LUMP_SIZE = idVector3.Size + idVector3.Size + (sizeof(int) * 4);

            public void InitFromFile( ref idFile file )
            {
                file.ReadVector3( ref mins );
                file.ReadVector3( ref maxs );

                firstSurface = file.ReadInt();
                numSurfaces = file.ReadInt();
                firstBrush = file.ReadInt();
                numBrushes = file.ReadInt();
            }
        };

        //
        // idMapShader
        //
        public struct idMapShader {
	        //char shader[MAX_QPATH];
            public string shader;
	        public int surfaceFlags;
	        public int contentFlags;

            public const int LUMP_SIZE = Engine.MAX_QPATH + (sizeof(int) * 2);

            public void InitFromFile( ref idFile file )
            {
                shader = file.ReadString( Engine.MAX_QPATH );
                surfaceFlags = file.ReadInt();
                contentFlags = file.ReadInt();
            }
        };

        // planes x^1 is allways the opposite of plane x

        //
        // idMapPlane
        //
        public struct idMapPlane {
	        public idVector3 normal;
	        public float dist;

            public const int LUMP_SIZE = idVector3.Size + sizeof(float);

            public void InitFromFile( ref idFile file )
            {
                file.ReadVector3( ref normal );
                dist = file.ReadFloat();
            }
        };

        //
        // idMapNode
        //
        public struct idMapNode {
	        public int planeNum;
	        public int[] children;            // negative numbers are -(leafs+1), not nodes
	        public int[] mins;                // for frustom culling
	        public int[] maxs;

            public const int LUMP_SIZE = sizeof(int) * 9;


            public void InitFromFile( ref idFile file )
            {
                children = new int[2];
                mins = new int[3];
                maxs = new int[3];

                planeNum = file.ReadInt();

                for( int i = 0; i < 2; i++ )
                {
                    children[i] = file.ReadInt();
                }

                for( int i = 0; i < 3; i++ )
                {
                    mins[i] = file.ReadInt();
                }

                for( int i = 0; i < 3; i++ )
                {
                    maxs[i] = file.ReadInt();
                }
            }
        };

        //
        // idMapLeaf
        //
        public struct idMapLeaf {
	        public int cluster;                    // -1 = opaque cluster (do I still store these?)
	        public int area;

	        public int[] mins;                    // for frustum culling
	        public int[] maxs;

	        public int firstLeafSurface;
	        public int numLeafSurfaces;

	        public int firstLeafBrush;
	        public int numLeafBrushes;

            public const int LUMP_SIZE = sizeof(int) * 12;

            public void InitFromFile( ref idFile file )
            {
                mins = new int[3];
                maxs = new int[3];

                cluster = file.ReadInt();
                area = file.ReadInt();

                for( int i = 0; i < 3; i++ )
                {
                    mins[i] = file.ReadInt();
                }

                for( int i = 0; i < 3; i++ )
                {
                    maxs[i] = file.ReadInt();
                }

                firstLeafSurface = file.ReadInt();
                numLeafSurfaces = file.ReadInt();
                firstLeafBrush  = file.ReadInt();
                numLeafBrushes  = file.ReadInt();
            }
        };

        //
        // idMapBrushSide
        //
        public struct idMapBrushSide {
	        public int planeNum;                   // positive plane side faces out of the leaf
	        public int shaderNum;

            public const int LUMP_SIZE = sizeof(int) * 2;

            public void InitFromFile( ref idFile file )
            {
                planeNum = file.ReadInt();
                shaderNum = file.ReadInt();
            }
        };

        //
        // idMapBrush
        //
        public struct idMapBrush {
	        public int firstSide;
	        public int numSides;
	        public int shaderNum;              // the shader that determines the contents flags

            public const int LUMP_SIZE = sizeof(int) * 3;

            public void InitFromFile( ref idFile file )
            {
                firstSide = file.ReadInt();
                numSides = file.ReadInt();
                shaderNum = file.ReadInt();
            }
        };

        //
        // idMapFog
        //
        public struct idMapFog {
	        //char shader[MAX_QPATH];
            public string shader;
	        public int brushNum;
	        public int visibleSide;            // the brush side that ray tests need to clip against (-1 == none)

            public const int LUMP_SIZE = Engine.MAX_QPATH + sizeof(int) * 2;

            public void InitFromFile( ref idFile file )
            {
                shader = file.ReadString( Engine.MAX_QPATH );
                brushNum = file.ReadInt();
                visibleSide = file.ReadInt();
            }
        };

        //
        // dMapVertex
        //
        public struct idMapVertex {
	        public idVector3 xyz;
	        public idVector2 st;
	        public idVector2 lightmap;
	        public idVector3 normal;
	        public byte[] color; // 4 bytes

            public const int LUMP_SIZE = (idVector3.Size * 2) + (idVector2.Size * 2) + 4;

            public void InitFromFile( ref idFile file )
            {

                file.ReadVector3( ref xyz );
                file.ReadVector2( ref st );
                file.ReadVector2( ref lightmap );
                file.ReadVector3( ref normal );

                color = file.ReadBytes( 4 );
            }
        };

        //
        // idMapSurface
        //
        public struct idMapSurface {
            public int shaderNum;
            public int fogNum;
            public int surfaceType;

            public int firstVert;
            public int numVerts;

            public int firstIndex;
            public int numIndexes;

            public int lightmapNum;
            public int lightmapX, lightmapY;
            public int lightmapWidth, lightmapHeight;

	        public idVector3 lightmapOrigin;
	        public idVector3[] lightmapVecs; //[3];         // for patches, [0] and [1] are lodbounds

            public int patchWidth;
            public int patchHeight;

            public const int LUMP_SIZE = (idVector3.Size * 4) + (sizeof(int) * 14);

            public void InitFromFile(ref idFile file)
            {
                lightmapVecs = new idVector3[3];

                shaderNum = file.ReadInt();
                fogNum = file.ReadInt();
                surfaceType = file.ReadInt();
                firstVert = file.ReadInt();
                numVerts = file.ReadInt();
                firstIndex = file.ReadInt();
                numIndexes = file.ReadInt();
                lightmapNum = file.ReadInt();
                lightmapX = file.ReadInt();
                lightmapY = file.ReadInt();
                lightmapWidth = file.ReadInt();
                lightmapHeight = file.ReadInt();
                file.ReadVector3(ref lightmapOrigin);

                for (int i = 0; i < 3; i++)
                {
                    file.ReadVector3(ref lightmapVecs[i]);
                }

                patchWidth = file.ReadInt();
                patchHeight = file.ReadInt();
            }
        };
    }
}
