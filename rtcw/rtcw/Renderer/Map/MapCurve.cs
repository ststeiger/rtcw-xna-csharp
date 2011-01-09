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

// mapcurve.cs (c) 2010 jv Software
//

#define PATCH_STITCHING

using idLib;
using idLib.Math;
using idLib.Engine.Public;

namespace rtcw.Renderer.Map
{
    /*

    This file does all of the processing necessary to turn a raw grid of points
    read from the map file into a srfGridMesh_t ready for rendering.

    The level of detail solution is direction independent, based only on subdivided
    distance from the true curve.

    Only a single entry point:

    idGridSurface SubdividePatchToGrid( int width, int height, int startVertPos, drawVert_t points[] ) {

    */

    //
    // idGrid
    //
    public class idGridSurface : idDrawSurface {
	    // culling information
        public idBounds bounds;
	    public idVector3 localOrigin;
	    public float meshRadius;

	    // lod information, which may be different
	    // than the culling information to allow for
	    // groups of curves that LOD as a unit
        public idVector3 lodOrigin;
	    public float lodRadius;
        public int lodFixed;
        public int lodStitched;

	    // vertexes, needed for calculating the final tess
        public int width, height;
        public float[] widthLodError;
        public float[] heightLodError;
	    public idDrawVertex[] verts;            // variable sized
    };

    static class MapCurveTessalator
    {
        public const int MAX_FACE_POINTS  =   64;

        public const int MAX_PATCH_SIZE   =   32;          // max dimensions of a patch mesh in map file
        public const int MAX_GRID_SIZE    =   65;          // max dimensions of a grid mesh in memory

        /*
        =================
        MakeMeshNormals

        Handles all the complicated wrapping and degenerate cases
        =================
        */
        private static int[,] neighbors = new int[8,2]{
		   {0,1}, {1,1}, {1,0}, {1,-1}, {0,-1}, {-1,-1}, {-1,0}, {-1,1}
	    };
        private static void MakeMeshNormals( int width, int height, ref idDrawVertex[,] ctrl ) {
	        int i, j, k, dist;
	        idVector3 normal;
	        idVector3 sum;
	        int count;
	        idVector3 basev;
	        idVector3 delta;
	        int x, y;
	        idDrawVertex  dv;
	        idVector3[] around = new idVector3[8];
            idVector3 temp;
	        bool[] good = new bool[8];
	        bool wrapWidth, wrapHeight;
	        float len;

	        wrapWidth = false;
	        for ( i = 0 ; i < height ; i++ ) {
                delta = ctrl[i,0].xyz - ctrl[i,width - 1].xyz;
                len = delta.LengthSqr();
		        if ( len > 1.0 ) {
			        break;
		        }
	        }
	        if ( i == height ) {
		        wrapWidth = true;
	        }

	        wrapHeight = false;
	        for ( i = 0 ; i < width ; i++ ) {
                delta = ctrl[i, 0].xyz - ctrl[i, width - 1].xyz;
                len = delta.LengthSqr();
                if (len > 1.0)
                {
                    break;
                }
	        }
	        if ( i == width ) {
		        wrapHeight = true;
	        }


	        for ( i = 0 ; i < width ; i++ ) {
		        for ( j = 0 ; j < height ; j++ ) {
			        count = 0;
			        dv = ctrl[j,i];

                    basev = dv.xyz;
			        for ( k = 0 ; k < 8 ; k++ ) {
                        around[k].X = 0;
                        around[k].Y = 0;
                        around[k].Z = 0;

				        good[k] = false;

				        for ( dist = 1 ; dist <= 3 ; dist++ ) {
					        x = i + neighbors[k,0] * dist;
					        y = j + neighbors[k,1] * dist;
					        if ( wrapWidth ) {
						        if ( x < 0 ) {
							        x = width - 1 + x;
						        } else if ( x >= width ) {
							        x = 1 + x - width;
						        }
					        }
					        if ( wrapHeight ) {
						        if ( y < 0 ) {
							        y = height - 1 + y;
						        } else if ( y >= height ) {
							        y = 1 + y - height;
						        }
					        }

					        if ( x < 0 || x >= width || y < 0 || y >= height ) {
						        break;                  // edge of patch
					        }
                            temp = ctrl[y, x].xyz - basev;
					        
					        if ( temp.Normalize() == 0 ) {
						        continue;               // degenerate edge, get more dist
					        } else {
						        good[k] = true;
                                around[k] = temp;
						        break;                  // good edge
					        }
				        }

                        ctrl[j, i] = dv;
			        }

                    sum.X = 0;
                    sum.Y = 0;
                    sum.Z = 0;
			        for ( k = 0 ; k < 8 ; k++ ) {
				        if ( !good[k] || !good[( k + 1 ) & 7] ) {
					        continue;   // didn't get two points
				        }
                        normal = around[(k + 1) & 7].Cross(around[k]);

				        if ( normal.Normalize() == 0 ) {
					        continue;
				        }
                        sum = normal + sum;
				        count++;
			        }
			        if ( count == 0 ) {
        //printf("bad normal\n");
				        count = 1;
			        }
                    dv.normal = sum;
                    dv.normal.Normalize();
		        }
	        }
        }

        /*
        ============
        LerpDrawVert
        ============
        */
        private static void LerpDrawVert( idDrawVertex a, idDrawVertex b, ref idDrawVertex outvert ) {
	        outvert.xyz[0] = 0.5f * ( a.xyz[0] + b.xyz[0] );
	        outvert.xyz[1] = 0.5f * ( a.xyz[1] + b.xyz[1] );
	        outvert.xyz[2] = 0.5f * ( a.xyz[2] + b.xyz[2] );

	        outvert.st[0] = 0.5f * ( a.st[0] + b.st[0] );
	        outvert.st[1] = 0.5f * ( a.st[1] + b.st[1] );

	        outvert.lightmapST[0] = 0.5f * ( a.lightmapST[0] + b.lightmapST[0] );
	        outvert.lightmapST[1] = 0.5f * ( a.lightmapST[1] + b.lightmapST[1] );
#if false
	        outvert.color[0] = ( a.color[0] + b.color[0] ) >> 1;
	        outvert.color[1] = ( a.color[1] + b.color[1] ) >> 1;
	        outvert.color[2] = ( a.color[2] + b.color[2] ) >> 1;
	        outvert.color[3] = ( a.color[3] + b.color[3] ) >> 1;
#endif
        }

        /*
        ============
        Transpose
        ============
        */
        private static void Transpose( int width, int height, ref idDrawVertex[,] ctrl) {
	        int i, j;
	        idDrawVertex temp;

	        if ( width > height ) {
		        for ( i = 0 ; i < height ; i++ ) {
			        for ( j = i + 1 ; j < width ; j++ ) {
				        if ( j < height ) {
					        // swap the value
					        temp = ctrl[j,i];
					        ctrl[j,i] = ctrl[i,j];
					        ctrl[i,j] = temp;
				        } else {
					        // just copy
					        ctrl[j,i] = ctrl[i,j];
				        }
			        }
		        }
	        } else {
		        for ( i = 0 ; i < width ; i++ ) {
			        for ( j = i + 1 ; j < height ; j++ ) {
				        if ( j < width ) {
					        // swap the value
					        temp = ctrl[i,j];
					        ctrl[i,j] = ctrl[j,i];
					        ctrl[j,i] = temp;
				        } else {
					        // just copy
					        ctrl[i,j] = ctrl[j,i];
				        }
			        }
		        }
	        }

        }

        /*
        ==================
        PutPointsOnCurve
        ==================
        */
        private static void PutPointsOnCurve(ref idDrawVertex[,] ctrl, int width, int height ) {
	        int i, j;
            idDrawVertex prev = new idDrawVertex();
            idDrawVertex next = new idDrawVertex();

	        for ( i = 0 ; i < width ; i++ ) {
		        for ( j = 1 ; j < height ; j += 2 ) {
			        LerpDrawVert( ctrl[j,i], ctrl[j + 1,i], ref prev );
			        LerpDrawVert( ctrl[j,i], ctrl[j - 1,i], ref next );
			        LerpDrawVert( prev, next, ref ctrl[j,i] );
		        }
	        }


	        for ( j = 0 ; j < height ; j++ ) {
		        for ( i = 1 ; i < width ; i += 2 ) {
			        LerpDrawVert( ctrl[j,i], ctrl[j,i + 1], ref prev );
			        LerpDrawVert( ctrl[j,i], ctrl[j,i - 1], ref next );
			        LerpDrawVert( prev, next, ref ctrl[j,i] );
		        }
	        }
        }

        /*
        =================
        InvertErrorTable
        =================
        */
        private static void InvertErrorTable( ref float[,] errorTable, int width, int height ) {
	        int i, c;
	        float[,] copy = new float[2,MAX_GRID_SIZE];
            for (i = 0; i < 2; i++)
            {
                for (c = 0; c < MAX_GRID_SIZE; c++)
                {
                    copy[i, c] = errorTable[i, c];
                }
            }


	        for ( i = 0 ; i < width ; i++ ) {
		        errorTable[1,i] = copy[0,i];  //[width-1-i];
	        }

	        for ( i = 0 ; i < height ; i++ ) {
		        errorTable[0,i] = copy[1,height - 1 - i];
	        }

        }

        /*
        ============
        InvertCtrl
        ============
        */
        private static void InvertCtrl( int width, int height, ref idDrawVertex[,] ctrl ) {
	        int i, j;
            idDrawVertex temp;

	        for ( i = 0 ; i < height ; i++ ) {
		        for ( j = 0 ; j < width / 2 ; j++ ) {
			        temp = ctrl[i,j];
			        ctrl[i,j] = ctrl[i,width - 1 - j];
			        ctrl[i,width - 1 - j] = temp;
		        }
	        }
        }

        /*
        =================
        CreateSurfaceGridMesh
        =================
        */
        private static idGridSurface CreateSurfaceGridMesh( int width, int height, idDrawVertex[,] ctrl, float[,] errorTable) {
	        int i, j, size;
            idDrawVertex vert;
	        idVector3 tmpVec;
            idGridSurface grid;

	        // copy the results out to a grid
	        //size = ( width * height - 1 ) * sizeof( drawVert_t ) + sizeof( *grid );
            

        #if PATCH_STITCHING
            grid = new idGridSurface();
            grid.verts = new idDrawVertex[width * height];

            grid.widthLodError = new float[width];
            for (i = 0; i < width; i++)
            {
                grid.widthLodError[i] = errorTable[0, i];
            }

            grid.heightLodError = new float[height];
            for (i = 0; i < height; i++)
            {
                grid.heightLodError[i] = errorTable[1, i];
            }
        #else
	        grid = ri.Hunk_Alloc( size, h_low );
	        memset( grid, 0, size );

	        grid->widthLodError = ri.Hunk_Alloc( width * 4, h_low );
	        memcpy( grid->widthLodError, errorTable[0], width * 4 );

	        grid->heightLodError = ri.Hunk_Alloc( height * 4, h_low );
	        memcpy( grid->heightLodError, errorTable[1], height * 4 );
        #endif

	        grid.width = width;
	        grid.height = height;
	        grid.type = surfaceType_t.SF_GRID;
            grid.bounds = new idBounds();
	        for ( i = 0 ; i < width ; i++ ) {
		        for ( j = 0 ; j < height ; j++ ) {
                    vert = ctrl[j, i];
                    grid.verts[j * width + i] = vert;

			        grid.bounds.AddPointToBounds( vert.xyz );
		        }
	        }

	        // compute local origin and bounds
            grid.localOrigin = grid.bounds.Mins + grid.bounds.Maxs;
            grid.localOrigin = grid.localOrigin * 0.5f;
            tmpVec = grid.bounds.Mins - grid.localOrigin;
            grid.meshRadius = tmpVec.Length();

            grid.lodOrigin = grid.localOrigin;
	        grid.lodRadius = grid.meshRadius;
	        //
	        return grid;
        }

        //
        // SubdividePatchToGrid
        //
        public static idGridSurface SubdividePatchToGrid( int width, int height, int startVertPos, idDrawVertex[] points )
        {
            int t;
            float len, maxLen;
            idDrawVertex[,] ctrl = new idDrawVertex[MAX_GRID_SIZE, MAX_GRID_SIZE];
            float[,] errorTable = new float[2, MAX_GRID_SIZE];
            idVector3 midxyz = new idVector3();
            idDrawVertex prev = new idDrawVertex();
            idDrawVertex next = new idDrawVertex();
            idDrawVertex mid = new idDrawVertex();



            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    ctrl[j, i] = points[startVertPos + (j * width + i)];
                }
            }

            for ( int dirnum = 0 ; dirnum < 2 ; dirnum++ ) 
            {
		        for ( int j = 0 ; j < MAX_GRID_SIZE ; j++ ) {
			        errorTable[dirnum, j] = 0;
		        }

		        // horizontal subdivisions
		        for ( int j = 0 ; j + 2 < width ; j += 2 ) {
			        // check subdivided midpoints against control points

			        // FIXME: also check midpoints of adjacent patches against the control points
			        // this would basically stitch all patches in the same LOD group together.

			        maxLen = 0;
			        for ( int i = 0 ; i < height ; i++ ) {
				        idVector3 dir;
				        idVector3 projected;
				        float d;

				        // calculate the point on the curve
				        for ( int l = 0 ; l < 3 ; l++ ) {
					        midxyz[l] = ( ctrl[i,j].xyz[l] + ctrl[i,j + 1].xyz[l] * 2
								          + ctrl[i,j + 2].xyz[l] ) * 0.25f;
				        }

				        // see how far off the line it is
				        // using dist-from-line will not account for internal
				        // texture warping, but it gives a lot less polygons than
				        // dist-from-midpoint
                        midxyz = midxyz - ctrl[i,j].xyz;
                        dir = ctrl[i,j + 2].xyz - ctrl[i,j].xyz;
                        dir.Normalize();

                        d = midxyz * dir;
                        projected = dir * d;
				        midxyz = midxyz - projected;


				        len = midxyz.LengthSqr();            // we will do the sqrt later

				        if ( len > maxLen ) {
					        maxLen = len;
				        }
			        }

			        maxLen = (float)System.Math.Sqrt( maxLen );
			        // if all the points are on the lines, remove the entire columns
			        if ( maxLen < 0.1f ) {
				        errorTable[dirnum,j + 1] = 999;
				        continue;
			        }

			        // see if we want to insert subdivided columns
			        if ( width + 2 > MAX_GRID_SIZE ) {
				        errorTable[dirnum,j + 1] = 1.0f / maxLen;
				        continue;   // can't subdivide any more
			        }

			        if ( maxLen <= Globals.r_subdivisions.GetValueInteger() ) {
				        errorTable[dirnum,j + 1] = 1.0f / maxLen;
				        continue;   // didn't need subdivision
			        }

			        errorTable[dirnum,j + 2] = 1.0f / maxLen;

			        // insert two columns and replace the peak
			        width += 2;
			        for ( int i = 0 ; i < height ; i++ ) {
                        

				        LerpDrawVert( ctrl[i,j], ctrl[i,j + 1], ref prev );
				        LerpDrawVert( ctrl[i,j + 1], ctrl[i,j + 2], ref next );
				        LerpDrawVert( prev, next, ref mid );

				        for ( int k = width - 1 ; k > j + 3 ; k-- ) {
					        ctrl[i,k] = ctrl[i,k - 2];
				        }
				        ctrl[i,j + 1] = prev;
				        ctrl[i,j + 2] = mid;
				        ctrl[i,j + 3] = next;
			        }

			        // back up and recheck this set again, it may need more subdivision
			        j -= 2;

		        }

		        Transpose( width, height, ref ctrl );
		        t = width;
		        width = height;
		        height = t;
	        }


	        // put all the aproximating points on the curve
	        PutPointsOnCurve( ref ctrl, width, height );

	        // cull out any rows or columns that are colinear
	        for ( int i = 1 ; i < width - 1 ; i++ ) {
		        if ( errorTable[0,i] != 999 ) {
			        continue;
		        }
		        for ( int j = i + 1 ; j < width ; j++ ) {
			        for ( int k = 0 ; k < height ; k++ ) {
				        ctrl[k,j - 1] = ctrl[k,j];
			        }
			        errorTable[0,j - 1] = errorTable[0,j];
		        }
		        width--;
	        }

	        for ( int i = 1 ; i < height - 1 ; i++ ) {
		        if ( errorTable[1,i] != 999 ) {
			        continue;
		        }
		        for ( int j = i + 1 ; j < height ; j++ ) {
                    for (int k = 0; k < width; k++)
                    {
				        ctrl[j - 1,k] = ctrl[j,k];
			        }
			        errorTable[1,j - 1] = errorTable[1,j];
		        }
		        height--;
	        }

        #if true
	        // flip for longest tristrips as an optimization
	        // the results should be visually identical with or
	        // without this step
	        if ( height > width ) {
		        Transpose( width, height, ref ctrl );
		        InvertErrorTable( ref errorTable, width, height );
		        t = width;
		        width = height;
		        height = t;
		        InvertCtrl( width, height, ref ctrl );
	        }
        #endif

	        // calculate normals
	        MakeMeshNormals( width, height, ref ctrl );

	        return CreateSurfaceGridMesh( width, height, ctrl, errorTable );
        }
    }
}
