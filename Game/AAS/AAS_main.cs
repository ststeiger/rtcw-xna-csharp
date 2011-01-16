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

// AAS_main.cs (c) 2010 JV Software
//

using idLib.Math;
using idLib.Engine.Public;
using Game.AAS.Private;

namespace Game.AAS
{
    //
    // idAAS
    //
    public class idAAS
    {
        internal bool aasInit = false;
        internal idAASWorld[] aasworlds = new idAASWorld[2];
        internal int currentWorldNum = -1;
        internal idAASMove aasmover;

        private const float BBOX_NORMAL_EPSILON  = 0.001f;
        private const float ON_EPSILON           = 0; //0.0005
        private const float TRACEPLANE_EPSILON = 0.125f;

        public idAASMove move
        {
            get
            {
                return aasmover;
            }
        }

        //
        // AASWorld
        //
        public idAASWorld AASWorld
        {
            get
            {
                return aasworlds[currentWorldNum];
            }
        }

        //
        // SetCurrentAASWorld
        //
        public void SetCurrentAASWorld(int worldnum)
        {
            currentWorldNum = worldnum;
        }

        private aas_tracestack_t[] tracestack = new aas_tracestack_t[127];
        public int TraceAreas( idVector3 start, idVector3 end, ref int[] areas, idVector3[] points, int maxareas ) {
	        int side, nodenum, tmpplanenum;
	        int numareas;
	        float front, back, frac;
	        idVector3 cur_start, cur_end, cur_mid;
	        
	        int tstack_p;
	        aas_node_t aasnode;
	        aas_plane_t plane;

	        numareas = 0;
	        areas[0] = 0;
	        tstack_p = 0;

            // jv - do this so the compiler will shut up.
            cur_start = idVector3.vector_origin;
            cur_end = idVector3.vector_origin;
            cur_mid = idVector3.vector_origin;

	        //we start with the whole line on the stack
            tracestack[tstack_p].start = start;
            tracestack[tstack_p].end = end;


	        tracestack[tstack_p].planenum = 0;
	        //start with node 1 because node zero is a dummy for a solid leaf
	        tracestack[tstack_p].nodenum = 1;      //starting at the root of the tree
	        tstack_p++;

	        while ( true )
	        {
		        //pop up the stack
		        tstack_p--;
		        //if the trace stack is empty (ended up with a piece of the
		        //line to be traced in an area)
		        if ( tstack_p < 0 ) {
			        return numareas;
		        } //end if
		          //number of the current node to test the line against
		        nodenum = tracestack[tstack_p].nodenum;
		        //if it is an area
		        if ( nodenum < 0 ) {
			        if ( -nodenum > AASWorld.aasfile.numareasettings ) {
				        Engine.common.Warning( "AAS_TraceAreas: -nodenum = %d out of range\n", -nodenum );
				        return numareas;
			        } //end if
			        //botimport.Print(PRT_MESSAGE, "areanum = %d, must be %d\n", -nodenum, AAS_PointAreaNum(start));
			        areas[numareas] = -nodenum;
			        if ( points != null ) {
                        points[numareas] = tracestack[tstack_p].start;
			        }
			        numareas++;
			        if ( numareas >= maxareas ) {
				        return numareas;
			        }
			        continue;
		        } //end if
		          //if it is a solid leaf
		        if ( nodenum == 0 ) {
			        continue;
		        } //end if
		        if ( nodenum > AASWorld.aasfile.numnodes ) {
			        Engine.common.Warning( "AAS_TraceAreas: nodenum out of range\n" );
			        return numareas;
		        } //end if
	           //the node to test against
		        aasnode = AASWorld.aasfile.nodes[nodenum];
		        //start point of current line to test against node
                cur_start = tracestack[tstack_p].start;
		        //end point of the current line to test against node
                cur_end = tracestack[tstack_p].end;
		        //the current node plane
                plane = AASWorld.aasfile.planes[aasnode.planenum];

		        switch ( plane.type )
		        {/*FIXME: wtf doesn't this work? obviously the node planes aren't always facing positive!!!
			        //check for axial planes
			        case PLANE_X:
			        {
				        front = cur_start[0] - plane->dist;
				        back = cur_end[0] - plane->dist;
				        break;
			        } //end case
			        case PLANE_Y:
			        {
				        front = cur_start[1] - plane->dist;
				        back = cur_end[1] - plane->dist;
				        break;
			        } //end case
			        case PLANE_Z:
			        {
				        front = cur_start[2] - plane->dist;
				        back = cur_end[2] - plane->dist;
				        break;
			        } //end case*/
		            default:     //gee it's not an axial plane
		            {
			            front = idMath.DotProduct( cur_start, plane.normal ) - plane.dist;
                        back = idMath.DotProduct(cur_end, plane.normal) - plane.dist;
			            break;
		            } 
		        } //end switch

		        //if the whole to be traced line is totally at the front of this node
		        //only go down the tree with the front child
		        if ( front > 0 && back > 0 ) {
			        //keep the current start and end point on the stack
			        //and go down the tree with the front child
			        tracestack[tstack_p].nodenum = aasnode.children[0];
			        tstack_p++;
                    if (tstack_p >= 127)
                    {
				        Engine.common.Warning("AAS_TraceAreas: stack overflow\n" );
				        return numareas;
			        } //end if
		        } //end if
		          //if the whole to be traced line is totally at the back of this node
		          //only go down the tree with the back child
		        else if ( front <= 0 && back <= 0 ) {
			        //keep the current start and end point on the stack
			        //and go down the tree with the back child
                    tracestack[tstack_p].nodenum = aasnode.children[1];
			        tstack_p++;
			        if ( tstack_p >= 127 ) {
				        Engine.common.Warning("AAS_TraceAreas: stack overflow\n" );
				        return numareas;
			        } //end if
		        } //end if
		          //go down the tree both at the front and back of the node
		        else
		        {
                    tmpplanenum = tracestack[tstack_p].planenum;
			        //calculate the hitpoint with the node (split point of the line)
			        //put the crosspoint TRACEPLANE_EPSILON pixels on the near side
			        if ( front < 0 ) {
				        frac = ( front ) / ( front - back );
			        } else { frac = ( front ) / ( front - back );}
			        if ( frac < 0 ) {
				        frac = 0;
			        } else if ( frac > 1 ) {
				        frac = 1;
			        }
			        //frac = front / (front-back);
			        //
			        cur_mid[0] = cur_start[0] + ( cur_end[0] - cur_start[0] ) * frac;
			        cur_mid[1] = cur_start[1] + ( cur_end[1] - cur_start[1] ) * frac;
			        cur_mid[2] = cur_start[2] + ( cur_end[2] - cur_start[2] ) * frac;

        //			AAS_DrawPlaneCross(cur_mid, plane->normal, plane->dist, plane->type, LINECOLOR_RED);
			        //side the front part of the line is on
                    //side = front < 0;
                    if (front < 0)
                    {
                        side = 1;
                    }
                    else
                    {
                        side = 0;
                    }
			        //first put the end part of the line on the stack (back side)
                    tracestack[tstack_p].start = cur_mid;
			        //not necesary to store because still on stack
			        //VectorCopy(cur_end, tstack_p->end);
                    tracestack[tstack_p].planenum = aasnode.planenum;
                    tracestack[tstack_p].nodenum = aasnode.children[side ^ 1];
			        tstack_p++;
			        if ( tstack_p >= 127 ) {
				        Engine.common.Warning( "AAS_TraceAreas: stack overflow\n" );
				        return numareas;
			        }
			        //now put the part near the start of the line on the stack so we will
			        //continue with thats part first. This way we'll find the first
			        //hit of the bbox
                    tracestack[tstack_p].start = cur_start;
                    tracestack[tstack_p].end = cur_mid;
                    tracestack[tstack_p].planenum = tmpplanenum;
                    tracestack[tstack_p].nodenum = aasnode.children[side];
			        tstack_p++;
			        if ( tstack_p >= 127 ) {
                        Engine.common.Warning("AAS_TraceAreas: stack overflow\n");
				        return numareas;
			        } 
		        } 
	        } 
        } //end of the function AAS_TraceAreas

        //
        // PointAreaNum
        //
        public int PointAreaNum( idVector3 point ) {
	        int nodenum;
	        float dist;
	        aas_node_t node;
	        aas_plane_t plane;

	        //start with node 1 because node zero is a dummy used for solid leafs
	        nodenum = 1;
	        while ( nodenum > 0 )
	        {
		        if ( nodenum >= AASWorld.aasfile.numnodes ) {
			        Engine.common.Warning( "nodenum = %d >= (*aasworld).numnodes = %d\n", nodenum, AASWorld.aasfile.numnodes );
			        return 0;
		        } 
		        node = AASWorld.aasfile.nodes[nodenum];

		        if ( node.planenum < 0 || node.planenum >= AASWorld.aasfile.numplanes ) {
			        Engine.common.Warning( "node->planenum = %d >= (*aasworld).numplanes = %d\n", node.planenum, AASWorld.aasfile.numplanes );
			        return 0;
		        }

		        plane = AASWorld.aasfile.planes[node.planenum];
		        dist = idMath.DotProduct( point, plane.normal ) - plane.dist;
		        if ( dist > 0 ) {
			        nodenum = node.children[0];
		        } else { nodenum = node.children[1];}
	        } //end while
	        if ( nodenum == 0 ) {
		        Engine.common.Warning( "in solid\n" );
		        return 0;
	        } //end if
	        return -nodenum;
        }

        //
        // AAS_TraceClientBBox
        //
        public idTrace AAS_TraceClientBBox( idVector3 start, idVector3 end, int presencetype, int passent ) {
	        int side, nodenum, tmpplanenum;
	        float front, back, frac;
	        idVector3 cur_start, cur_end, cur_mid, v1, v2, v1n;
	        int tstack_p;
	        aas_node_t aasnode;
	        aas_plane_t plane;
	        idTrace trace = new idTrace();

            // jv - do this so the compiler will shut up.
            cur_start = idVector3.vector_origin;
            cur_end = idVector3.vector_origin;
            cur_mid = idVector3.vector_origin;
            v1 = idVector3.vector_origin;
            v2 = idVector3.vector_origin;
            v1n = idVector3.vector_origin;

	        tstack_p = 0;
	        //we start with the whole line on the stack
            tracestack[tstack_p].start = start;
            tracestack[tstack_p].end = end;
	        
	        tracestack[tstack_p].planenum = 0;
	        //start with node 1 because node zero is a dummy for a solid leaf
	        tracestack[tstack_p].nodenum = 1;      //starting at the root of the tree
	        tstack_p++;

	        while ( true )
	        {
		        //pop up the stack
		        tstack_p--;
		        //if the trace stack is empty (ended up with a piece of the
		        //line to be traced in an area)
		        if ( tstack_p < 0 ) {
			        tstack_p++;
			        //nothing was hit
			        trace.startsolid = false;
			        trace.fraction = 1.0f;
			        //endpos is the end of the line
                    trace.endpos = end;
			        //nothing hit
			        trace.entityNum = 0;
			        trace.area = 0;
			        trace.planenum = 0;
			        return trace;
		        } //end if
		          //number of the current node to test the line against
		        nodenum = tracestack[tstack_p].nodenum;
		        //if it is an area
		        if ( nodenum < 0 ) {
			        if ( -nodenum > AASWorld.aasfile.numareasettings ) {
				        Engine.common.Warning( "AAS_TraceBoundingBox: -nodenum out of range\n" );
				        return trace;
			        } //end if
			        //botimport.Print(PRT_MESSAGE, "areanum = %d, must be %d\n", -nodenum, AAS_PointAreaNum(start));
			        //if can't enter the area because it hasn't got the right presence type
			        if ( ( AASWorld.aasfile.areasettings[-nodenum].presencetype & presencetype ) == 0 ) {
				        //if the start point is still the initial start point
				        //NOTE: no need for epsilons because the points will be
				        //exactly the same when they're both the start point
				        if ( tracestack[tstack_p].start[0] == start[0] &&
					         tracestack[tstack_p].start[1] == start[1] &&
					         tracestack[tstack_p].start[2] == start[2] ) {
					        trace.startsolid = true;
					        trace.fraction = 0.0f;
				        } //end if
				        else
				        {
					        trace.startsolid = false;
                            v1 = end - start;
                            v1n = v1;
					        v2 = tracestack[tstack_p].start - start;
					        trace.fraction = v2.Length() / v1n.Normalize();
                            tracestack[tstack_p].start = (tracestack[tstack_p].start + v1) * -0.125f;
				        } //end else

                        trace.endpos = tracestack[tstack_p].start;
				        trace.entityNum = 0;
				        trace.area = -nodenum;
        //				VectorSubtract(end, start, v1);
				        trace.planenum = tracestack[tstack_p].planenum;
				        //always take the plane with normal facing towards the trace start
				        plane = AASWorld.aasfile.planes[trace.planenum];
				        if ( idMath.DotProduct( v1, plane.normal ) > 0 ) {
					        trace.planenum ^= 1;
				        }
				        return trace;
			        } //end if
                    /*
			        else
			        {
				        if ( passent >= 0 ) {
					        if ( AAS_AreaEntityCollision( -nodenum, tstack_p->start,
												          tstack_p->end, presencetype, passent,
												          &trace ) ) {
						        if ( !trace.startsolid ) {
							        VectorSubtract( end, start, v1 );
							        VectorSubtract( trace.endpos, start, v2 );
							        trace.fraction = VectorLength( v2 ) / VectorLength( v1 );
						        } //end if
						        return trace;
					        } //end if
				        } //end if
			        } //end else
                    */
			        trace.lastarea = -nodenum;
			        continue;
		        } //end if
		          //if it is a solid leaf
		        if ( nodenum == 0 ) {
			        //if the start point is still the initial start point
			        //NOTE: no need for epsilons because the points will be
			        //exactly the same when they're both the start point
			        if ( tracestack[tstack_p].start[0] == start[0] &&
				         tracestack[tstack_p].start[1] == start[1] &&
				         tracestack[tstack_p].start[2] == start[2] ) {
				        trace.startsolid = true;
				        trace.fraction = 0.0f;
			        } //end if
			        else
			        {
				        trace.startsolid = false;
                        v1 = end - start;
                        v1n = v1;
                        v2 = tracestack[tstack_p].start - start;
				        trace.fraction = v2.Length() / v1n.Normalize();

				        tracestack[tstack_p].start = (tracestack[tstack_p].start + v1) * -0.125f;
			        } //end else

                    trace.endpos = tracestack[tstack_p].start;
			        trace.entityNum = 0;
			        trace.area = 0; //hit solid leaf
        //			VectorSubtract(end, start, v1);
			        trace.planenum = tracestack[tstack_p].planenum;
			        //always take the plane with normal facing towards the trace start
			        plane = AASWorld.aasfile.planes[trace.planenum];
			        if ( idMath.DotProduct( v1, plane.normal ) > 0 ) {
				        trace.planenum ^= 1;
			        }
			        return trace;
		        } //end if
		        if ( nodenum > AASWorld.aasfile.numnodes ) {
			        Engine.common.Warning( "AAS_TraceBoundingBox: nodenum out of range\n" );
			        return trace;
		        } 

                //the node to test against
		        aasnode = AASWorld.aasfile.nodes[nodenum];
		        //start point of current line to test against node
                cur_start = tracestack[tstack_p].start;
		        //end point of the current line to test against node
                cur_end = tracestack[tstack_p].end;
		        //the current node plane
		        plane = AASWorld.aasfile.planes[aasnode.planenum];

		        switch ( plane.type )
		        {/*FIXME: wtf doesn't this work? obviously the axial node planes aren't always facing positive!!!
			        //check for axial planes
			        case PLANE_X:
			        {
				        front = cur_start[0] - plane->dist;
				        back = cur_end[0] - plane->dist;
				        break;
			        } //end case
			        case PLANE_Y:
			        {
				        front = cur_start[1] - plane->dist;
				        back = cur_end[1] - plane->dist;
				        break;
			        } //end case
			        case PLANE_Z:
			        {
				        front = cur_start[2] - plane->dist;
				        back = cur_end[2] - plane->dist;
				        break;
			        } //end case*/
		            default:     //gee it's not an axial plane
		            {
			            front = idMath.DotProduct( cur_start, plane.normal ) - plane.dist;
			            back = idMath.DotProduct( cur_end, plane.normal ) - plane.dist;
			            break;
		            }     //end default
		        } //end switch

		        //calculate the hitpoint with the node (split point of the line)
		        //put the crosspoint TRACEPLANE_EPSILON pixels on the near side
		        if ( front < 0 ) {
			        frac = ( front + TRACEPLANE_EPSILON ) / ( front - back );
		        } else { frac = ( front - TRACEPLANE_EPSILON ) / ( front - back );}
		        //if the whole to be traced line is totally at the front of this node
		        //only go down the tree with the front child
		        if ( ( front >= -ON_EPSILON && back >= -ON_EPSILON ) ) {
			        //keep the current start and end point on the stack
			        //and go down the tree with the front child
                    tracestack[tstack_p].nodenum = aasnode.children[0];
			        tstack_p++;
			        if ( tstack_p >= 127 ) {
				        Engine.common.Warning( "AAS_TraceBoundingBox: stack overflow\n" );
				        return trace;
			        } //end if
		        } //end if
		          //if the whole to be traced line is totally at the back of this node
		          //only go down the tree with the back child
		        else if ( ( front < ON_EPSILON && back < ON_EPSILON ) ) {
			        //keep the current start and end point on the stack
			        //and go down the tree with the back child
                    tracestack[tstack_p].nodenum = aasnode.children[1];
			        tstack_p++;
			        if ( tstack_p >= 127 ) {
				        Engine.common.Warning( "AAS_TraceBoundingBox: stack overflow\n" );
				        return trace;
			        } //end if
		        } //end if
		          //go down the tree both at the front and back of the node
		        else
		        {
                    tmpplanenum = tracestack[tstack_p].planenum;
			        //
			        if ( frac < 0 ) {
				        frac = 0.001f; //0
			        } else if ( frac > 1 )                         {
				        frac = 0.999f; //1
			        }
			        //frac = front / (front-back);
			        //
			        cur_mid[0] = cur_start[0] + ( cur_end[0] - cur_start[0] ) * frac;
			        cur_mid[1] = cur_start[1] + ( cur_end[1] - cur_start[1] ) * frac;
			        cur_mid[2] = cur_start[2] + ( cur_end[2] - cur_start[2] ) * frac;

        //			AAS_DrawPlaneCross(cur_mid, plane->normal, plane->dist, plane->type, LINECOLOR_RED);
			        //side the front part of the line is on
                    if (front < 0)
                    {
                        side = 1;
                    }
                    else
                    {
                        side = 0;
                    }
			        //first put the end part of the line on the stack (back side)
                    tracestack[tstack_p].start = cur_mid;
			        //not necesary to store because still on stack
			        //VectorCopy(cur_end, tstack_p->end);
                    tracestack[tstack_p].planenum = aasnode.planenum;
                    tracestack[tstack_p].nodenum = aasnode.children[side ^ 1];
			        tstack_p++;
			        if ( tstack_p >= 127 ) {
				        Engine.common.Warning( "AAS_TraceBoundingBox: stack overflow\n" );
				        return trace;
			        } //end if
			          //now put the part near the start of the line on the stack so we will
			          //continue with thats part first. This way we'll find the first
			          //hit of the bbox
                    tracestack[tstack_p].start = cur_start;
                    tracestack[tstack_p].end = cur_mid;
                    tracestack[tstack_p].planenum = tmpplanenum;
                    tracestack[tstack_p].nodenum = aasnode.children[side];
			        tstack_p++;
			        if ( tstack_p >= 127 ) {
				        Engine.common.Warning( "AAS_TraceBoundingBox: stack overflow\n" );
				        return trace;
			        } //end if
		        } //end else
	        } //end while
        //	return trace;
        } //end of the function AAS_TraceClientBBox

        public bool IsPointOnGround(idVector3 origin, int presencetype, int passent)
        {
            idTrace trace;
            idVector3 end, up;
            aas_plane_t plane;

            up = idVector3.vector_origin;
            up[2] = 1.0f;

            end = origin;
            end[2] -= 10;

            trace = AAS_TraceClientBBox(origin, end, presencetype, passent);

            //if in solid
            if (trace.startsolid)
            {
                return true;                  //qfalse;
            }
            //if nothing hit at all
            if (trace.fraction >= 1.0)
            {
                return false;
            }
            //if too far from the hit plane
            if (origin[2] - trace.endpos[2] > 10)
            {
                return false;
            }
            //check if the plane isn't too steep
            plane = AASWorld.aasfile.planes[trace.planenum];
            if (idMath.DotProduct(plane.normal, up) < 0.7f)
            {
                return false;
            }
            //the bot is on the ground
            return true;
        } //end of the function AAS_OnGround

        //
        // LoadAASForWorld
        //
        public void LoadAASForWorld(string mapname)
        {
            Engine.common.Printf("-------- AAS_Init ---------\n");

            // Load the map AAS files
            for (int i = 0; i < 2; i++)
            {
                SetCurrentAASWorld(i);

                aasworlds[i] = new idAASWorld();

                if (aasworlds[i].Init(mapname + "_b" + i) == false)
                {
                    Engine.common.Warning("AAS_Init: Failed to load %s AI will be disabled...\n", mapname + "_b" + i);
                    return;
                }
            }

            aasmover = new idAASMove();

            Engine.common.Printf("AAS Loaded Successfully...\n");

            aasInit = true;
        }
    }


    struct aas_tracestack_t
    {
	    public idVector3 start;       //start point of the piece of line to trace
	    public idVector3 end;         //end point of the piece of line to trace
	    public int planenum;       //last plane used as splitter
	    public int nodenum;        //node found after splitting with planenum
    };

    public class idAASTravelFlags
    {
        //travel flags
        public const int  TFL_INVALID          =   0x0000001;   //traveling temporary not possible
        public const int  TFL_WALK             =   0x0000002;   //walking
        public const int  TFL_CROUCH           =   0x0000004;   //crouching
        public const int  TFL_BARRIERJUMP      =   0x0000008;   //jumping onto a barrier
        public const int  TFL_JUMP             =   0x0000010;   //jumping
        public const int  TFL_LADDER           =   0x0000020;   //climbing a ladder
        public const int  TFL_WALKOFFLEDGE     =   0x0000080;   //walking of a ledge
        public const int  TFL_SWIM             =   0x0000100;   //swimming
        public const int  TFL_WATERJUMP        =   0x0000200;   //jumping out of the water
        public const int  TFL_TELEPORT         =   0x0000400;   //teleporting
        public const int  TFL_ELEVATOR         =   0x0000800;   //elevator
        public const int  TFL_ROCKETJUMP       =   0x0001000;   //rocket jumping
        public const int  TFL_BFGJUMP          =   0x0002000;   //bfg jumping
        public const int  TFL_GRAPPLEHOOK      =   0x0004000;   //grappling hook
        public const int  TFL_DOUBLEJUMP       =   0x0008000;   //double jump
        public const int  TFL_RAMPJUMP         =   0x0010000;   //ramp jump
        public const int  TFL_STRAFEJUMP       =   0x0020000;   //strafe jump
        public const int  TFL_JUMPPAD          =   0x0040000;   //jump pad
        public const int  TFL_AIR              =   0x0080000;   //travel through air
        public const int  TFL_WATER            =   0x0100000;   //travel through water
        public const int  TFL_SLIME            =   0x0200000;   //travel through slime
        public const int  TFL_LAVA             =   0x0400000;   //travel through lava
        public const int  TFL_DONOTENTER       =   0x0800000;   //travel through donotenter area
        public const int  TFL_FUNCBOB          =   0x1000000;   //func bobbing
        public const int  TFL_DONOTENTER_LARGE =   0x2000000;   //travel through donotenter area
    }
}
