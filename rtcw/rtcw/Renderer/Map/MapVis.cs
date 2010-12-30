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

        int viewCluster = -1;
        int numClusters = -1;
        int clusterBytes = -1;

        int[] markSurfaces;

        //
        // idMapVis
        //
        public idMapVis(byte[] visbuffer, idRenderNode[] nodes, int clusterBytes, int numClusters, int[] markSurfaces)
        {
            visibility = visbuffer;
            this.nodes = nodes;
            this.clusterBytes = clusterBytes;
            this.numClusters = numClusters;
            this.markSurfaces = markSurfaces;
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

            node = nodes[0];
            while (true)
            {
                if (node.contents != -1)
                {
                    break;
                }
                plane = node.plane;
                d = (p * plane.Normal) - plane.Dist;
                if (d > 0)
                {
                    node = node.children[0];
                }
                else
                {
                    node = node.children[1];
                }
            }

            return node;
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
		       
		        // add the individual surfaces
		        mark = node.firstmarksurface;
		        c = node.nummarksurfaces;
		        while ( c > 0 ) {
			        // the surface may have already been added if it
			        // spans multiple leafs
                    surfaces[markSurfaces[mark]].visCount = Globals.visCount;
			        mark++;
                    c--;
		        }
	        }
        }

        //
        // SetSurfacesVisibility
        //
        public void SetSurfacesVisibility(idVector3 pvsOrigin, ref idDrawSurface[] surfaces)
        {
            MarkLeavesInPVS(pvsOrigin);
            RecursiveWorldNode(nodes[0], ref surfaces );
        }

        /*
        ==================
        TestPointInPVS
         
        Checks to see if a point is in the points PVS.
        ==================
        */
        public bool TestPointInPVS(idVector3 p1, idVector3 p2)
        {
            idRenderNode leaf;
            int vis;

            if (p1 == p2)
            {
                return true;
            }

            leaf = R_PointInLeaf(p1);
            vis  = R_ClusterPVS(leaf.cluster);
            leaf = R_PointInLeaf(p2);

            byte vischeck = visibility[vis + (leaf.cluster >> 3)];
            if ((vischeck & (1 << (leaf.cluster & 7))) == 0)
            {
                return false;
            }

            return true;
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
		        if ( ( visibility[vis + (cluster >> 3)] & ( 1 << ( cluster & 7 ) ) ) == 0) {
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