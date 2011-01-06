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

// CollisionModel.cs (c) 2010 JV Software
//

using idLib.Math;
using idLib.Engine.Public;

using rtcw.Renderer;
using rtcw.Renderer.Map;


namespace rtcw.CM
{
    // Used for oriented capsule collision detection
    public struct idTraceSphere
    {
	    public bool use;
	    public float radius;
	    public float halfheight;
	    public idVector3 offset;
    };

    //
    // idCollisionTraceWork
    //
    struct idCollisionTraceWork {
	    public idVector3 start;
	    public idVector3 end;
	    public idVector3[] size;         // size of the box being swept through the model
	    public idVector3[] offsets;      // [signbits][x] = either size[0][x] or size[1][x]
	    public float maxOffset;        // longest corner length from origin
        public idVector3 extents;         // greatest of abs(size[0]) and abs(size[1])
	    public idVector3[] bounds;       // enclosing box of start and end surrounding by size
        public idVector3 modelOrigin;     // origin of the model tracing through
	    public int contents;           // ored contents of the model tracing through
	    public bool isPoint;       // optimized case
	    public idTrace trace;          // returned from trace call
        public idTraceSphere sphere;

        public void Init()
        {
            size = new idVector3[2];
            offsets = new idVector3[8];
            bounds = new idVector3[2];
            sphere = new idTraceSphere();
        }

        public void Reset()
        {
            sphere.use = false;
            sphere.radius = 0.0f;
            sphere.offset = idVector3.vector_origin;
            sphere.halfheight = 0.0f;
            start = idVector3.vector_origin;
            end = idVector3.vector_origin;
            trace = idTrace.defaultTrace;
            trace.fraction = 1.0f;
            maxOffset = 0;
            contents = 0;
            isPoint = false;
            for (int i = 0; i < 2; i++)
            {
                size[i] = idVector3.vector_origin;
            }

            for (int i = 0; i < 8; i++)
            {
                offsets[i] = idVector3.vector_origin;
            }

            for (int i = 0; i < 2; i++)
            {
                bounds[i] = idVector3.vector_origin;
            }
        }
    } ;
    //
    // idCollisionModelLocal
    //
    public class idCollisionModelLocal : idCollisionModel
    {
        private string name;
        private idMap world;
        private static idCollisionTraceWork tw = new idCollisionTraceWork();
        private int checkcount = 0;
        private int c_brush_traces = 0;

        // keep 1/8 unit away to keep the position valid before network snapping
        // and to avoid various numeric issues
        private const float SURFACE_CLIP_EPSILON = (0.125f);

        //
        // AllocInternals
        //
        public static void AllocInternals()
        {
            tw.Init();
        }

        //
        // LoadFromFile
        //
        public void LoadFromFile(string mappath, object world)
        {
            name = mappath;

            Engine.common.Printf("Loading Collision Model...\n");
            this.world = (idMap)world;
        }

        /*
        ================
        CM_TraceThroughBrush
        ================
        */
        private void CM_TraceThroughBrush(idMapFormat.idMapBrush brush) {
	        int i;
	        idPlane plane, clipplane;
	        float dist;
	        float enterFrac, leaveFrac;
	        float d1, d2;
	        bool getout, startout;
	        float f;
	        idMapFormat.idMapBrushSide side;
            idMapFormat.idMapBrushSide? leadside;
	        float t;
	        idVector3 startp;
            idVector3 endp;

	        enterFrac = -1.0f;
	        leaveFrac = 1.0f;
	        clipplane = null;

	        if ( brush.numSides == 0 ) {
		        return;
	        }

	        c_brush_traces++;

	        getout = false;
	        startout = false;

	        leadside = null;

	        if ( tw.sphere.use ) {
		        //
		        // compare the trace against all planes of the brush
		        // find the latest time the trace crosses a plane towards the interior
		        // and the earliest time the trace crosses a plane towards the exterior
		        //
		        for ( i = 0; i < brush.numSides; i++ ) {
                    side = world.brushsides[brush.firstSide + i];
                    plane = world.planes[side.planeNum];

			        // adjust the plane distance apropriately for radius
			        dist = plane.Dist + tw.sphere.radius;

			        // find the closest point on the capsule to the plane
			        t = idMath.DotProduct( plane.Normal, tw.sphere.offset );
			        if ( t > 0 ) {
                        startp = tw.start - tw.sphere.offset;
                        endp = tw.end - tw.sphere.offset;
			        } else
			        {
                        startp = tw.start + tw.sphere.offset;
                        endp = tw.end + tw.sphere.offset;
			        }

                    d1 = idMath.DotProduct(startp, plane.Normal) - dist;
                    d2 = idMath.DotProduct(endp, plane.Normal) - dist;

			        if ( d2 > 0 ) {
				        getout = true; // endpoint is not in solid
			        }
			        if ( d1 > 0 ) {
				        startout = true;
			        }

			        // if completely in front of face, no intersection with the entire brush
			        if ( d1 > 0 && ( d2 >= SURFACE_CLIP_EPSILON || d2 >= d1 )  ) {
				        return;
			        }

			        // if it doesn't cross the plane, the plane isn't relevent
			        if ( d1 <= 0 && d2 <= 0 ) {
				        continue;
			        }

			        // crosses face
			        if ( d1 > d2 ) {  // enter
				        f = ( d1 - SURFACE_CLIP_EPSILON ) / ( d1 - d2 );
				        if ( f < 0 ) {
					        f = 0;
				        }
				        if ( f > enterFrac ) {
					        enterFrac = f;
					        clipplane = plane;
					        leadside = side;
				        }
			        } else {    // leave
				        f = ( d1 + SURFACE_CLIP_EPSILON ) / ( d1 - d2 );
				        if ( f > 1 ) {
					        f = 1;
				        }
				        if ( f < leaveFrac ) {
					        leaveFrac = f;
				        }
			        }
		        }
	        } else {
		        //
		        // compare the trace against all planes of the brush
		        // find the latest time the trace crosses a plane towards the interior
		        // and the earliest time the trace crosses a plane towards the exterior
		        //
		        for ( i = 0; i < brush.numSides; i++ ) {
			        side = world.brushsides[brush.firstSide + i];
			        plane = world.planes[side.planeNum];

			        // adjust the plane distance apropriately for mins/maxs
                    dist = plane.Dist - idMath.DotProduct(tw.offsets[plane.SignBits], plane.Normal);

                    d1 = idMath.DotProduct(tw.start, plane.Normal) - dist;
                    d2 = idMath.DotProduct(tw.end, plane.Normal) - dist;

			        if ( d2 > 0 ) {
				        getout = true; // endpoint is not in solid
			        }
			        if ( d1 > 0 ) {
				        startout = true;
			        }

			        // if completely in front of face, no intersection with the entire brush
			        if ( d1 > 0 && ( d2 >= SURFACE_CLIP_EPSILON || d2 >= d1 )  ) {
				        return;
			        }

			        // if it doesn't cross the plane, the plane isn't relevent
			        if ( d1 <= 0 && d2 <= 0 ) {
				        continue;
			        }

			        // crosses face
			        if ( d1 > d2 ) {  // enter
				        f = ( d1 - SURFACE_CLIP_EPSILON ) / ( d1 - d2 );
				        if ( f < 0 ) {
					        f = 0;
				        }
				        if ( f > enterFrac ) {
					        enterFrac = f;
					        clipplane = plane;
					        leadside = side;
				        }
			        } else {    // leave
				        f = ( d1 + SURFACE_CLIP_EPSILON ) / ( d1 - d2 );
				        if ( f > 1 ) {
					        f = 1;
				        }
				        if ( f < leaveFrac ) {
					        leaveFrac = f;
				        }
			        }
		        }
	        }

        //
	        // all planes have been checked, and the trace was not
	        // completely outside the brush
	        //
	        if ( !startout ) {    // original point was inside brush
		        tw.trace.startsolid = true;
		        if ( !getout ) {
			        tw.trace.allsolid = true;
			        tw.trace.fraction = 0;
		        }
		        return;
	        }

	        if ( enterFrac < leaveFrac ) {
		        if ( enterFrac > -1 && enterFrac < tw.trace.fraction ) {
			        if ( enterFrac < 0 ) {
				        enterFrac = 0;
			        }
			        tw.trace.fraction = enterFrac;
			        tw.trace.plane = clipplane;
			        tw.trace.surfaceFlags = leadside.Value.surfaceFlags;
			        tw.trace.contents = brush.contents;
		        }
	        }
        }


        /*
        ================
        CM_TraceThroughLeaf
        ================
        */
        private void CM_TraceThroughLeaf( idRenderNode leaf ) {
	        int k;
	        int brushnum;
	        idMapFormat.idMapBrush b;
	        // cPatch_t    *patch;

	        // trace line against all brushes in the leaf
	        for ( k = 0 ; k < leaf.numBrushSurfaces; k++ ) {
		        brushnum = world.leafbrushes[leaf.firstBrushSurface + k];

		        b = world.brushes[brushnum];
		        if ( b.checkcount == checkcount ) {
			        continue;   // already checked this brush in another leaf
		        }
		        b.checkcount = checkcount;

		        if ( ( b.contents & tw.contents ) == 0 ) {
			        continue;
		        }

		        CM_TraceThroughBrush( b );

                world.brushes[brushnum] = b;

		        if ( tw.trace.fraction == 0 ) {
			        return;
		        }
	        }

	        // trace line against all patches in the leaf
#if false
	        if ( !cm_noCurves->integer ) {
		        for ( k = 0 ; k < leaf->numLeafSurfaces ; k++ ) {
			        patch = cm.surfaces[ cm.leafsurfaces[ leaf->firstLeafSurface + k ] ];
			        if ( !patch ) {
				        continue;
			        }
			        if ( patch->checkcount == cm.checkcount ) {
				        continue;   // already checked this patch in another leaf
			        }
			        patch->checkcount = cm.checkcount;

			        if ( !( patch->contents & tw->contents ) ) {
				        continue;
			        }

			        CM_TraceThroughPatch( tw, patch );
			        if ( !tw->trace.fraction ) {
				        return;
			        }
		        }
	        }
#endif
        }

        /*
        ==================
        CM_TraceThroughTree

        Traverse all the contacted leafs from the start to the end position.
        If the trace is a point, they will be exactly in order, but for larger
        trace volumes it is possible to hit something in a later leaf with
        a smaller intercept fraction.
        ==================
        */
        private void CM_TraceThroughTree(int num, float p1f, float p2f, idVector3 p1, idVector3 p2)
        {
            idPlane plane;
            float t1, t2, offset;
            float frac, frac2;
            float idist;
            idVector3 mid = idVector3.vector_origin;
            int side;
            float midf;
            idRenderNode node;

            if (tw.trace.fraction <= p1f)
            {
                return;     // already hit something nearer
            }

            // if != -1, we are in a leaf node
            if (num < 0)
            {
                CM_TraceThroughLeaf(world.nodes[world.numNodes + (-1 - num)]);
                return;
            }

            //
            // find the point distances to the seperating plane
            // and the offset for the size of the box
            //
            node = world.nodes[num];
            plane = node.plane;

            // adjust the plane distance apropriately for mins/maxs
            if (plane.Type < 3)
            {
                t1 = p1[plane.Type] - plane.Dist;
                t2 = p2[plane.Type] - plane.Dist;
                offset = tw.extents[plane.Type];
            }
            else
            {
                t1 = idMath.DotProduct(plane.Normal, p1) - plane.Dist;
                t2 = idMath.DotProduct(plane.Normal, p2) - plane.Dist;
                if (tw.isPoint)
                {
                    offset = 0;
                }
                else
                {
                    /*
                    // an axial brush right behind a slanted bsp plane
                    // will poke through when expanded, so adjust
                    // by sqrt(3)
                    offset = fabs(tw->extents[0]*plane->normal[0]) +
                        fabs(tw->extents[1]*plane->normal[1]) +
                        fabs(tw->extents[2]*plane->normal[2]);

                    offset *= 2;
                    offset = tw->maxOffset;
                    */
                    // this is silly
                    offset = 2048;
                }
            }

            // see which sides we need to consider
            if (t1 >= offset + 1 && t2 >= offset + 1)
            {
                CM_TraceThroughTree(node.childrenhandles[0], p1f, p2f, p1, p2);
                return;
            }
            if (t1 < -offset - 1 && t2 < -offset - 1)
            {
                CM_TraceThroughTree(node.childrenhandles[1], p1f, p2f, p1, p2);
                return;
            }

            // put the crosspoint SURFACE_CLIP_EPSILON pixels on the near side
            if (t1 < t2)
            {
                idist = 1.0f / (t1 - t2);
                side = 1;
                frac2 = (t1 + offset + SURFACE_CLIP_EPSILON) * idist;
                frac = (t1 - offset + SURFACE_CLIP_EPSILON) * idist;
            }
            else if (t1 > t2)
            {
                idist = 1.0f / (t1 - t2);
                side = 0;
                frac2 = (t1 - offset - SURFACE_CLIP_EPSILON) * idist;
                frac = (t1 + offset + SURFACE_CLIP_EPSILON) * idist;
            }
            else
            {
                side = 0;
                frac = 1;
                frac2 = 0;
            }

            // move up to the node
            if (frac < 0)
            {
                frac = 0;
            }
            if (frac > 1)
            {
                frac = 1;
            }

            midf = p1f + (p2f - p1f) * frac;

            mid[0] = p1[0] + frac * (p2[0] - p1[0]);
            mid[1] = p1[1] + frac * (p2[1] - p1[1]);
            mid[2] = p1[2] + frac * (p2[2] - p1[2]);

            CM_TraceThroughTree(node.childrenhandles[side], p1f, midf, p1, mid);


            // go past the node
            if (frac2 < 0)
            {
                frac2 = 0;
            }
            if (frac2 > 1)
            {
                frac2 = 1;
            }

            midf = p1f + (p2f - p1f) * frac2;

            mid[0] = p1[0] + frac2 * (p2[0] - p1[0]);
            mid[1] = p1[1] + frac2 * (p2[1] - p1[1]);
            mid[2] = p1[2] + frac2 * (p2[2] - p1[2]);

            CM_TraceThroughTree(node.childrenhandles[side ^ 1], midf, p2f, mid, p2);
        }

        //
        // Trace
        //
        private void Trace(out idTrace results, idVector3 start, idVector3 end, idLib.idBounds bounds, int passEntityNum, int contentmask, bool useCapsole)
        {
            idVector3 offset;

            checkcount++;

            offset = idVector3.vector_origin;

            // Reset the trace worker for the new trace.
            tw.Reset();

            tw.sphere.use = useCapsole;

            // set basic parms
            tw.contents = contentmask;

            // adjust so that mins and maxs are always symetric, which
            // avoids some complications with plane expanding of rotated
            // bmodels
            for (int i = 0; i < 3; i++)
            {
                offset[i] = (bounds.Mins[i] + bounds.Maxs[i]) * 0.5f;
                tw.size[0][i] = bounds.Mins[i] - offset[i];
                tw.size[1][i] = bounds.Maxs[i] - offset[i];
                tw.start[i] = start[i] + offset[i];
                tw.end[i] = end[i] + offset[i];
            }

            tw.maxOffset = tw.size[1][0] + tw.size[1][1] + tw.size[1][2];

            // tw.offsets[signbits] = vector to apropriate corner from origin
            tw.offsets[0][0] = tw.size[0][0];
            tw.offsets[0][1] = tw.size[0][1];
            tw.offsets[0][2] = tw.size[0][2];

            tw.offsets[1][0] = tw.size[1][0];
            tw.offsets[1][1] = tw.size[0][1];
            tw.offsets[1][2] = tw.size[0][2];

            tw.offsets[2][0] = tw.size[0][0];
            tw.offsets[2][1] = tw.size[1][1];
            tw.offsets[2][2] = tw.size[0][2];

            tw.offsets[3][0] = tw.size[1][0];
            tw.offsets[3][1] = tw.size[1][1];
            tw.offsets[3][2] = tw.size[0][2];

            tw.offsets[4][0] = tw.size[0][0];
            tw.offsets[4][1] = tw.size[0][1];
            tw.offsets[4][2] = tw.size[1][2];

            tw.offsets[5][0] = tw.size[1][0];
            tw.offsets[5][1] = tw.size[0][1];
            tw.offsets[5][2] = tw.size[1][2];

            tw.offsets[6][0] = tw.size[0][0];
            tw.offsets[6][1] = tw.size[1][1];
            tw.offsets[6][2] = tw.size[1][2];

            tw.offsets[7][0] = tw.size[1][0];
            tw.offsets[7][1] = tw.size[1][1];
            tw.offsets[7][2] = tw.size[1][2];

            //
            // calculate bounds
            //
            if (tw.sphere.use)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (tw.start[i] < tw.end[i])
                    {
                        tw.bounds[0][i] = tw.start[i] - (float)System.Math.Abs(tw.sphere.offset[i]) - tw.sphere.radius;
                        tw.bounds[1][i] = tw.end[i] + (float)System.Math.Abs(tw.sphere.offset[i]) + tw.sphere.radius;
                    }
                    else
                    {
                        tw.bounds[0][i] = tw.end[i] - (float)System.Math.Abs(tw.sphere.offset[i]) - tw.sphere.radius;
                        tw.bounds[1][i] = tw.start[i] + (float)System.Math.Abs(tw.sphere.offset[i]) + tw.sphere.radius;
                    }
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    if (tw.start[i] < tw.end[i])
                    {
                        tw.bounds[0][i] = tw.start[i] + tw.size[0][i];
                        tw.bounds[1][i] = tw.end[i] + tw.size[1][i];
                    }
                    else
                    {
                        tw.bounds[0][i] = tw.end[i] + tw.size[0][i];
                        tw.bounds[1][i] = tw.start[i] + tw.size[1][i];
                    }
                }
            }
            //
            // check for position test special case
            //
            if (start[0] == end[0] && start[1] == end[1] && start[2] == end[2])
            {
                //throw new System.NotImplementedException();
            }
            else
            {
                //
                // check for point special case
                //
                if (tw.size[0][0] == 0 && tw.size[0][1] == 0 && tw.size[0][2] == 0)
                {
                    tw.isPoint = true;
                    tw.extents = idVector3.vector_origin;
                }
                else
                {
                    tw.isPoint = false;
                    tw.extents[0] = tw.size[1][0];
                    tw.extents[1] = tw.size[1][1];
                    tw.extents[2] = tw.size[1][2];
                }

                CM_TraceThroughTree(0, 0, 1, tw.start, tw.end);
            }

            // generate endpos from the original, unmodified start/end
            if (tw.trace.fraction == 1)
            {
                tw.trace.endpos = end;
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    tw.trace.endpos[i] = start[i] + tw.trace.fraction * (end[i] - start[i]);
                }
            }

            // Return the results.
            results = tw.trace;
        }

        //
        // BoxTrace
        //
        public override void BoxTrace(out idTrace results, idVector3 start, idVector3 end, idLib.idBounds bounds, int passEntityNum, int contentmask)
        {
            Trace(out results, start, end, bounds, passEntityNum, contentmask, false);
        }

        //
        // GetName
        //
        public override string GetName()
        {
            return name;
        }
    }
}
