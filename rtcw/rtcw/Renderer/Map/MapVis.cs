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

// mapvis.cs (c) 2010 jv Software
//

using idLib;
using idLib.Math;
using idLib.Engine.Public;

namespace rtcw.Renderer.Map
{
    //
    // idMapVis
    //
    public class idMapVis
    {
        byte[] visibility;
        idRenderNode[] nodes;

        int[] boxleafs = new int[128];
        int numLeafsForBox = 0;

        int viewCluster = -1;
        int numClusters = -1;
        int clusterBytes = -1;
        int numNodes;

        int[] markSurfaces;
        private idVector3[] visBounds = new idVector3[2];

        //
        // idMapVis
        //
        public idMapVis(byte[] visbuffer, idRenderNode[] nodes, int clusterBytes, int numClusters, int[] markSurfaces, int numNodes)
        {
            visibility = visbuffer;
            this.nodes = nodes;
            this.clusterBytes = clusterBytes;
            this.numClusters = numClusters;
            this.markSurfaces = markSurfaces;
            this.numNodes = numNodes;
        }

        /*
        ===============
        R_PointInLeaf
        ===============
        */
        private idRenderNode R_PointInLeaf(idVector3 p)
        {
            idRenderNode node;
            float d;
            idPlane plane;
            int num = 0;

            
            while (true)
            {
                if (num < 0)
                {
                    break;
                }

                node = nodes[num];
                plane = node.plane;

                if (plane.Type < 3)
                {
                    d = p[plane.Type] - plane.Dist;
                }
                else
                {
                    d = idMath.DotProduct(plane.Normal, p) - plane.Dist;
                }
                if (d < 0)
                {
                    num = node.childrenhandles[1];
                }
                else
                {
                    num = node.childrenhandles[0];
                }
            }

            return nodes[numNodes + (-1 - num)];
        }

        /*
        ==============
        R_ClusterPVS
        ==============
        */
        private int R_ClusterPVS(int cluster)
        {
            return cluster * clusterBytes;
        }

        /*
        ================
        RecursiveWorldNode
        ================
        */
        private void RecursiveWorldNode(idRenderNode node, ref idDrawSurface[] surfaces) 
        {

	        do {
		        // if the node wasn't marked as potentially visible, exit
		        if ( node.visframe != Globals.visCount ) {
			        return;
		        }

                if ( node.contents != -1 ) {
			        break;
		        }

		        // recurse down the children, front side first
		        RecursiveWorldNode( node.children[0], ref surfaces );

		        // tail recurse
		        node = node.children[1];
	        } while ( true );

	        {
		        // leaf node, so add mark surfaces
		        int c, mark;

                // add to z buffer bounds
                if (node.mins[0] < visBounds[0][0])
                {
                    visBounds[0][0] = node.mins[0];
                }
                if (node.mins[1] < visBounds[0][1])
                {
                    visBounds[0][1] = node.mins[1];
                }
                if (node.mins[2] < visBounds[0][2])
                {
                    visBounds[0][2] = node.mins[2];
                }

                if (node.maxs[0] > visBounds[1][0])
                {
                    visBounds[1][0] = node.maxs[0];
                }
                if (node.maxs[1] > visBounds[1][1])
                {
                    visBounds[1][1] = node.maxs[1];
                }
                if (node.maxs[2] > visBounds[1][2])
                {
                    visBounds[1][2] = node.maxs[2];
                }
		       
		        // add the individual surfaces
		        mark = node.firstmarksurface;
		        c = node.nummarksurfaces;
                idPlane plane = node.parent.plane;
                float sort = plane.Dist;
		        while ( c > 0 ) {
			        // the surface may have already been added if it
			        // spans multiple leafs
                    surfaces[markSurfaces[mark]].visCount = Globals.visCount;
                  //  surfaces[markSurfaces[mark]].sort = sort;
			        mark++;
                    c--;
		        }
	        }
        }

        /*
        ==============
        SetFarClip
        ==============
        */
        private float SetFarClip(idVector3 pvsOrigin) {
	        float farthestCornerDistance = 0;
	        int i;

	        //
	        // set far clipping planes dynamically
	        //
	        farthestCornerDistance = 0;
	        for ( i = 0; i < 8; i++ )
	        {
		        idVector3 v = idVector3.vector_origin;
                idVector3 vecTo = idVector3.vector_origin;
		        float distance;

		        if ( (i & 1) != 0 ) {
			        v[0] = visBounds[0][0];
		        } else
		        {
			        v[0] = visBounds[1][0];
		        }

		        if ( (i & 2) != 0 ) {
			        v[1] = visBounds[0][1];
		        } else
		        {
			        v[1] = visBounds[1][1];
		        }

		        if ( (i & 4) != 0 ) {
			        v[2] = visBounds[0][2];
		        } else
		        {
			        v[2] = visBounds[1][2];
		        }

               // vecTo = v - pvsOrigin;
                vecTo = v;

		        distance = vecTo[0] * vecTo[0] + vecTo[1] * vecTo[1] + vecTo[2] * vecTo[2];

		        if ( distance > farthestCornerDistance ) {
			        farthestCornerDistance = distance;
		        }
	        }

	        return (float)System.Math.Sqrt( farthestCornerDistance );
        }

        //
        // SetSurfacesVisibility
        //
        public float SetSurfacesVisibility(idVector3 pvsOrigin, ref idDrawSurface[] surfaces)
        {
            // Reset the visibily boundry.
            visBounds[0] = idVector3.vector_origin;
            visBounds[1] = idVector3.vector_origin;

            MarkLeavesInPVS(pvsOrigin);
            RecursiveWorldNode(nodes[0], ref surfaces );

            //return SetFarClip(pvsOrigin);
            return 1000.0f;
        }

        /*
        =============
        CM_BoxLeafnums

        Fills in a list of all the leafs touched
        =============
        */
        private void BoxLeafnums(int nodenum, idVector3 mins, idVector3 maxs)
        {
            idPlane plane;
            idRenderNode node;
            int s;

            while (true)
            {
                if (nodenum < 0)
                {
                    boxleafs[numLeafsForBox++] = numNodes + (-1 - nodenum);
                    return;
                }

                node = nodes[nodenum];
                plane = node.plane;
                s = plane.BoxOnPlaneSide(mins, maxs);
                if (s == 1)
                {
                    nodenum = node.childrenhandles[0];
                }
                else if (s == 2)
                {
                    nodenum = node.childrenhandles[1];
                }
                else
                {
                    // go down both
                    BoxLeafnums(node.childrenhandles[0], mins, maxs);
                    nodenum = node.childrenhandles[1];
                }

            }
        }


        //
        // TestBoundsInPVS
        //
        public bool TestBoundsInPVS(idVector3 p1, idVector3 p2, idBounds bounds)
        {
            numLeafsForBox = 0;

            BoxLeafnums(0, p2 + bounds.Mins, p2 + bounds.Maxs);

            if (numLeafsForBox <= 0)
            {
                return false;
            }

            for (int i = 0; i < numLeafsForBox; i++)
            {
                if (TestLeafInPVS(p1, nodes[boxleafs[i]]))
                {
                    return true;
                }
            }

            return false;
        }

        //
        // TestLeafInPVS
        //
        private bool TestLeafInPVS(idVector3 p1, idRenderNode p2leaf)
        {
            idRenderNode leaf;
            int vis;

            leaf = R_PointInLeaf(p1);
            vis = R_ClusterPVS(leaf.cluster);

            if ((visibility[vis + (p2leaf.cluster >> 3)] & (1 << (p2leaf.cluster & 7))) == 0)
            {
                return false;
            }

            return true;
        }

        /*
        ==================
        TestPointInPVS
         
        Checks to see if a point is in the points PVS.
        ==================
        */
        public bool TestPointInPVS(idVector3 p1, idVector3 p2)
        {
            if (p1 == p2)
            {
                return true;
            }

            return TestLeafInPVS(p1, R_PointInLeaf(p2));
        }

        /*
        ===============
        MarkLeavesInPVS

        Mark the leaves and nodes that are in the PVS for the current
        cluster
        ===============
        */
        private void MarkLeavesInPVS( idVector3 pvsOrigin ) {
	        idRenderNode leaf, parent;
	        int i;
	        int cluster;
            int vis;

	        // lockpvs lets designers walk around to determine the
	        // extent of the current pvs
	        if ( Globals.r_lockpvs.GetValueInteger() == 1 ) {
		        return;
	        }

	        // current viewcluster
            leaf = R_PointInLeaf(pvsOrigin);
	        cluster = leaf.cluster;

	        // if the cluster is the same and the area visibility matrix
	        // hasn't changed, we don't need to mark everything again

	        // if r_showcluster was just turned on, remark everything
	        if ( viewCluster == cluster ) {
		        return;
	        }

	        Globals.visCount++;
	        viewCluster = cluster;
#if false
	        if ( r_novis->integer || tr.viewCluster == -1 ) {
		        for ( i = 0 ; i < tr.world->numnodes ; i++ ) {
			        if ( tr.world->nodes[i].contents != CONTENTS_SOLID ) {
				        tr.world->nodes[i].visframe = tr.visCount;
			        }
		        }
		        return;
	        }
#endif
	        vis = R_ClusterPVS( viewCluster );

	        for ( i = 0; i < nodes.Length ; i++ ) {
                leaf = nodes[i];

		        cluster = leaf.cluster;
		        if ( cluster < 0 || cluster >= numClusters ) {
			        continue;
		        }

		        // check general pvs
		        if (( visibility[vis + (cluster >> 3)] & ( 1 << ( cluster & 7 ) ) ) == 0) {
			        continue;
		        }

		        // check for door connection
#if false
		        if ( ( tr.refdef.areamask[leaf->area >> 3] & ( 1 << ( leaf->area & 7 ) ) ) ) {
			        continue;       // not visible
		        }
#endif
		        parent = leaf;
		        do {
                    if (parent.visframe == Globals.visCount)
                    {
				        break;
			        }
			        parent.visframe = Globals.visCount;
			        parent = parent.parent;
		        } while ( parent != null );
	        }
        }

    }
}