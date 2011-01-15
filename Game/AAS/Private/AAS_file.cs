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

// AAS_format.cs (c) 2010 JV Software
//

using idLib.Math;
using idLib.Engine.Public;
using idLib.Engine.Public.Net; // need this for idMsgRead

namespace Game.AAS.Private
{
        //====== additional information ======
    /*

    -	when a node child is a solid leaf the node child number is zero
    -	two adjacent areas (sharing a plane at opposite sides) share a face
	    this face is a portal between the areas
    -	when an area uses a face from the faceindex with a positive index
	    then the face plane normal points into the area
    -	the face edges are stored counter clockwise using the edgeindex
    -	two adjacent convex areas (sharing a face) only share One face
	    this is a simple result of the areas being convex
    -	the convex areas can't have a mixture of ground and gap faces
	    other mixtures of faces in one area are allowed
    -	areas with the AREACONTENTS_CLUSTERPORTAL in the settings have
	    cluster number zero
    -	edge zero is a dummy
    -	face zero is a dummy
    -	area zero is a dummy
    -	node zero is a dummy
    */
    //NOTE:	int = default signed
    //					default long

    //
    // idAASFile
    //
    public class idAASFile
    {
        //bounding boxes
        public int numbboxes;
        public aas_bbox_t[] bboxes;
        //vertexes
        public aas_vertextable_t vertexes;
        //planes
        public int numplanes;
        public aas_plane_t[] planes;
        //edges
        public int numedges;
        public aas_edge_t[] edges;
        //edge index
        public aas_edgeindextable_t edgeindex;
        //faces
        public int numfaces;
        public aas_face_t[] faces;
        //face index
        public aas_faceindextable_t faceindex;
        //convex areas
        public int numareas;
        public aas_area_t[] areas;
        //convex area settings
        public int numareasettings;
        public aas_areasettings_t[] areasettings;
        //reachablity list
        public int reachabilitysize;
        public aas_reachability_t[] reachability;
        //nodes of the bsp tree
        public int numnodes;
        public aas_node_t[] nodes;
        //cluster portals
        public int numportals;
        public aas_portal_t[] portals;
        //cluster portal index
        public aas_portalindextable_t portalindex;
        //clusters
        public int numclusters;
        public aas_cluster_t[] clusters;

        //
        // SetLumpPosition
        //
        private void LoadAASLump<T>(aas_lump_t aaslump, ref idFile bspFile, out int numLumps, out T[] lumps) where T : aas_lumpinterface_t, new()
        {
            T baselump;

            baselump = new T();
            numLumps = aaslump.filelen / baselump.GetSize();

            bspFile.Seek(idFileSeekOrigin.FS_SEEK_SET, aaslump.fileofs);

            lumps = new T[numLumps];
            baselump.InitFromFile(ref bspFile);
            lumps[0] = baselump;
            for (int i = 1; i < numLumps; i++)
            {
                baselump = new T();
                baselump.InitFromFile(ref bspFile);
                lumps[i] = baselump;
            }
        }

        //
        // ParseAASFile
        //
        public bool ParseAASFile(string path)
        {
            aas_header_t header = new aas_header_t();
            idFile file;
            
            file = Engine.fileSystem.OpenFileRead(path, true);
            if (file == null)
            {
                Engine.common.Warning("AAS_InitFromFile: Failed to load %s \n", path);
                return false;
            }

            Engine.common.Printf("...parsing AAS\n");

            // Load in the AAS header ensure its valid, and load the lump tables.
            header.InitFromFile(ref file);

            //load the lumps:
            //bounding boxes
            LoadAASLump<aas_bbox_t>(header.lumps[aas_lumptype.AASLUMP_BBOXES], ref file, out numbboxes, out bboxes);
            //vertexes
            file.Seek(idFileSeekOrigin.FS_SEEK_SET, header.lumps[aas_lumptype.AASLUMP_VERTEXES].fileofs);
            vertexes = new aas_vertextable_t();
            vertexes.InitFromFile(ref file, header.lumps[aas_lumptype.AASLUMP_VERTEXES].filelen / idVector3.Size);
            //planes
            LoadAASLump<aas_plane_t>(header.lumps[aas_lumptype.AASLUMP_PLANES], ref file, out numplanes, out planes);
            //edges
            LoadAASLump<aas_edge_t>(header.lumps[aas_lumptype.AASLUMP_EDGES], ref file, out numedges, out edges);

            //edgeindex
            file.Seek(idFileSeekOrigin.FS_SEEK_SET, header.lumps[aas_lumptype.AASLUMP_EDGEINDEX].fileofs);
            edgeindex = new aas_edgeindextable_t();
            edgeindex.InitFromFile(ref file, header.lumps[aas_lumptype.AASLUMP_EDGEINDEX].filelen / sizeof(int));

            //faces
            LoadAASLump<aas_face_t>(header.lumps[aas_lumptype.AASLUMP_FACES], ref file, out numfaces, out faces);
            //faceindex
            file.Seek(idFileSeekOrigin.FS_SEEK_SET, header.lumps[aas_lumptype.AASLUMP_FACEINDEX].fileofs);
            faceindex = new aas_faceindextable_t();
            faceindex.InitFromFile(ref file, header.lumps[aas_lumptype.AASLUMP_FACEINDEX].filelen / sizeof(int));

            //convex areas
            LoadAASLump<aas_area_t>(header.lumps[aas_lumptype.AASLUMP_AREAS], ref file, out numareas, out areas);

            //area settings
            LoadAASLump<aas_areasettings_t>(header.lumps[aas_lumptype.AASLUMP_AREASETTINGS], ref file, out numareasettings, out areasettings);

            //reachability list
            LoadAASLump<aas_reachability_t>(header.lumps[aas_lumptype.AASLUMP_REACHABILITY], ref file, out reachabilitysize, out reachability);

            //nodes
            LoadAASLump<aas_node_t>(header.lumps[aas_lumptype.AASLUMP_NODES], ref file, out numnodes, out nodes);

            //cluster portals
            LoadAASLump<aas_portal_t>(header.lumps[aas_lumptype.AASLUMP_PORTALS], ref file, out numportals, out portals);

            //cluster portal index
            file.Seek(idFileSeekOrigin.FS_SEEK_SET, header.lumps[aas_lumptype.AASLUMP_PORTALINDEX].fileofs);
            portalindex = new aas_portalindextable_t();
            portalindex.InitFromFile(ref file, header.lumps[aas_lumptype.AASLUMP_PORTALINDEX].filelen / sizeof(int));

            //clusters
            LoadAASLump<aas_cluster_t>(header.lumps[aas_lumptype.AASLUMP_CLUSTERS], ref file, out numclusters, out clusters);

            Engine.common.Printf("AAS Loaded Successfully...\n");
            Engine.fileSystem.CloseFile(ref file);
            return true;
        }
    }

    //presence types
    public static class aas_presencetype
    {
        public const int PRESENCE_NONE         =      1;
        public const int PRESENCE_NORMAL       =      2;
        public const int PRESENCE_CROUCH       =      4;
    }

    //travel types
    public static class aas_traveltype
    {
         public const int TRAVEL_INVALID        =      1;       //temporary not possible
         public const int TRAVEL_WALK           =      2;       //walking
         public const int TRAVEL_CROUCH         =      3;       //crouching
         public const int TRAVEL_BARRIERJUMP    =      4;       //jumping onto a barrier
         public const int TRAVEL_JUMP           =      5;       //jumping
         public const int TRAVEL_LADDER         =      6;       //climbing a ladder
         public const int TRAVEL_WALKOFFLEDGE   =      7;       //walking of a ledge
         public const int TRAVEL_SWIM           =      8;       //swimming
         public const int TRAVEL_WATERJUMP      =      9;       //jump out of the water
         public const int TRAVEL_TELEPORT       =      10;      //teleportation
         public const int TRAVEL_ELEVATOR       =      11;      //travel by elevator
         public const int TRAVEL_ROCKETJUMP     =      12;      //rocket jumping required for travel
         public const int TRAVEL_BFGJUMP        =      13;      //bfg jumping required for travel
         public const int TRAVEL_GRAPPLEHOOK    =      14;      //grappling hook required for travel
         public const int TRAVEL_DOUBLEJUMP     =      15;      //double jump
         public const int TRAVEL_RAMPJUMP       =      16;      //ramp jump
         public const int TRAVEL_STRAFEJUMP     =      17;      //strafe jump
         public const int TRAVEL_JUMPPAD        =      18;      //jump pad
         public const int TRAVEL_FUNCBOB        =      19;      //func bob
         public const int MAX_TRAVELTYPES       =      32;
    };

    //additional travel flags
    public static class aas_travelflags
    {
        public const int TRAVELTYPE_MASK        =    0xFFFFFF;
        public const int TRAVELFLAG_NOTTEAM1    =     ( 1 << 24 );
        public const int TRAVELFLAG_NOTTEAM2    =     ( 2 << 24 );
    };

    //face flags
    public static class aas_faceflags
    {
        public const int FACE_SOLID            =      1;       //just solid at the other side
        public const int FACE_LADDER           =      2;       //ladder
        public const int FACE_GROUND           =      4;       //standing on ground when in this face
        public const int FACE_GAP              =      8;       //gap in the ground
        public const int FACE_LIQUID           =      16;
        public const int FACE_LIQUIDSURFACE    =      32;
    }

    //area contents
    public static class aas_areacontents
    {
        public const int  AREACONTENTS_WATER         =     1;
        public const int  AREACONTENTS_LAVA          =     2;
        public const int  AREACONTENTS_SLIME         =     4;
        public const int  AREACONTENTS_CLUSTERPORTAL =     8;
        public const int  AREACONTENTS_TELEPORTAL    =     16;
        public const int  AREACONTENTS_ROUTEPORTAL   =     32;
        public const int  AREACONTENTS_TELEPORTER    =     64;
        public const int  AREACONTENTS_JUMPPAD       =     128;
        public const int  AREACONTENTS_DONOTENTER    =     256;
        public const int  AREACONTENTS_VIEWPORTAL    =     512;
        // Rafael - nopass
        public const int  AREACONTENTS_DONOTENTER_LARGE =  1024;
        public const int  AREACONTENTS_MOVER            =  2048;

        //number of model of the mover inside this area
        public const int  AREACONTENTS_MODELNUMSHIFT    =  24;
        public const int  AREACONTENTS_MAXMODELNUM      =  0xFF;
        public const int  AREACONTENTS_MODELNUM         =  ( AREACONTENTS_MAXMODELNUM << AREACONTENTS_MODELNUMSHIFT );
    }

    //area flags
    public static class aas_areaflags
    {
        public const int  AREA_GROUNDED         =      1;       //bot can stand on the ground
        public const int  AREA_LADDER           =      2;       //area contains one or more ladder faces
        public const int  AREA_LIQUID           =      4;       //area contains a liquid
        // Ridah
        public const int  AREA_DISABLED         =      8;
        public const int  AREA_USEFORROUTING    =      1024;
    };

    //aas file header lumps    
    public static class aas_lumptype
    {
        public const int AASLUMP_BBOXES          =    0;
        public const int AASLUMP_VERTEXES        =    1;
        public const int AASLUMP_PLANES          =    2;
        public const int AASLUMP_EDGES           =    3;
        public const int AASLUMP_EDGEINDEX       =    4;
        public const int AASLUMP_FACES           =    5;
        public const int AASLUMP_FACEINDEX       =    6;
        public const int AASLUMP_AREAS           =    7;
        public const int AASLUMP_AREASETTINGS    =    8;
        public const int AASLUMP_REACHABILITY    =    9;
        public const int AASLUMP_NODES           =    10;
        public const int AASLUMP_PORTALS         =    11;
        public const int AASLUMP_PORTALINDEX     =    12;
        public const int AASLUMP_CLUSTERS        =    13;
        public const int AAS_LUMPS               =    14;
    }

    //========== bounding box =========

    //bounding box
    public struct aas_bbox_t : aas_lumpinterface_t
    {
	    public int presencetype;
	    public int flags;
	    public idVector3 mins;
        public idVector3 maxs;

        public const int Size = (sizeof(int) * 2) + (idVector3.Size * 2);

        public int GetSize()
        {
            return Size;
        }

        public void InitFromFile( ref idFile file )
        {
            mins = idVector3.vector_origin;
            maxs = idVector3.vector_origin;

            presencetype = file.ReadInt();
            flags = file.ReadInt();
            file.ReadVector3(ref mins);
            file.ReadVector3(ref maxs);
        }
    };

    //============ settings ===========

    //reachability to another area
    public struct aas_reachability_t : aas_lumpinterface_t
    {
	    public int areanum;                        //number of the reachable area
	    public int facenum;                        //number of the face towards the other area
	    public int edgenum;                        //number of the edge towards the other area
	    public idVector3 start;                       //start point of inter area movement
	    public idVector3 end;                         //end point of inter area movement
	    public int traveltype;                 //type of travel required to get to the area
	    public int traveltime; //travel time of the inter area movement

        public const int Size = (sizeof(int) * 5) + (idVector3.Size * 2);

        public int GetSize()
        {
            return Size;
        }

        public void InitFromFile(ref idFile file)
        {
            start = idVector3.vector_origin;
            end = idVector3.vector_origin;

            areanum = file.ReadInt();
            facenum = file.ReadInt();
            edgenum = file.ReadInt();

            file.ReadVector3(ref start);
            file.ReadVector3(ref end);

            traveltype = file.ReadInt();
            traveltime = file.ReadInt();
        }
    };

    //area settings
    public struct aas_areasettings_t : aas_lumpinterface_t
    {
	    //could also add all kind of statistic fields
	    public int contents;                       //contents of the convex area
	    public int areaflags;                      //several area flags
	    public int presencetype;                   //how a bot can be present in this convex area
	    public int cluster;                        //cluster the area belongs to, if negative it's a portal
	    public int clusterareanum;             //number of the area in the cluster
	    public int numreachableareas;          //number of reachable areas from this one
	    public int firstreachablearea;         //first reachable area in the reachable area index
	    // Ridah, add a ground steepness stat, so we can avoid terrain when we can take a close-by flat route
	    public float groundsteepness;          // 0 = flat, 1 = steep

        public const int Size = (sizeof(int) * 7) + sizeof(float);

        public int GetSize()
        {
            return Size;
        }

        public void InitFromFile( ref idFile file )
        {
            contents = file.ReadInt();
            areaflags = file.ReadInt();
            presencetype = file.ReadInt();
            cluster = file.ReadInt();
            clusterareanum = file.ReadInt();
            numreachableareas = file.ReadInt();
            firstreachablearea = file.ReadInt();
            groundsteepness = file.ReadFloat();
        }
    };

    //cluster portal
    public struct aas_portal_t : aas_lumpinterface_t
    {
	    public int areanum;                        //area that is the actual portal
	    public int frontcluster;                   //cluster at front of portal
	    public int backcluster;                    //cluster at back of portal
	    public int[] clusterareanum;          //number of the area in the front and back cluster

        public const int Size = sizeof(int) * 5;

        public int GetSize()
        {
            return Size;
        }

        public void InitFromFile( ref idFile file )
        {
            areanum = file.ReadInt();
            frontcluster = file.ReadInt();
            backcluster = file.ReadInt();

            clusterareanum = new int[2];
            clusterareanum[0] = file.ReadInt();
            clusterareanum[1] = file.ReadInt();
        }
    };

    //cluster portal index
    public struct aas_portalindextable_t
    {
        int[] portalindextable;

        public int Count
        {
            get
            {
                return portalindextable.Length;
            }
        }

        public int this[int index]
        {
            get
            {
                return portalindextable[index];
            }
        }
        
        public void InitFromFile( ref idFile file, int numPortalIndexes )
        {
            portalindextable = new int[numPortalIndexes];
            for( int i = 0; i < numPortalIndexes; i++ )
            {
                portalindextable[i] = file.ReadInt();
            }
        }
    }

    //cluster
    public struct aas_cluster_t : aas_lumpinterface_t
    {
	    public int numareas;                       //number of areas in the cluster
	    public int numreachabilityareas;           //number of areas with reachabilities
	    public int numportals;                     //number of cluster portals
	    public int firstportal;                    //first cluster portal in the index

        public const int Size = sizeof(int) * 4;

        public int GetSize()
        {
            return Size;
        }

        public void InitFromFile( ref idFile file )
        {
            numareas = file.ReadInt();
            numreachabilityareas = file.ReadInt();
            numportals = file.ReadInt();
            firstportal = file.ReadInt();
        }
    };

    //============ 3d definition ============
    public struct aas_vertextable_t 
    {
        idVector3[] aasvertextable;

        public idVector3 this[int index]
        {
            get
            {
                return aasvertextable[index];
            }
        }


        public int Count
        {
            get
            {
                return aasvertextable.Length;
            }
        }

        //
        // InitFromFile
        //
        public void InitFromFile( ref idFile file, int numVertexes )
        {
            aasvertextable = new idVector3[numVertexes];

            for( int i = 0; i < numVertexes; i++ )
            {
                aasvertextable[i] = idVector3.vector_origin;
                file.ReadVector3( ref aasvertextable[i] );
            }
        }
    }

    //just a plane in the third dimension
    public struct aas_plane_t : aas_lumpinterface_t
    {
	    public idVector3 normal;                      //normal vector of the plane
	    public float dist;                         //distance of the plane (normal vector * distance = point in plane)
	    public int type;

        public const int Size = sizeof(int) + sizeof(float) + idVector3.Size;

        public int GetSize()
        {
            return Size;
        }

        public void InitFromFile( ref idFile file )
        {
            file.ReadVector3( ref normal );
            dist = file.ReadFloat();
            type = file.ReadInt();
        }
    };

    //edge
    public struct aas_edge_t : aas_lumpinterface_t
    {
	    public int[] v;                           //numbers of the vertexes of this edge

        public const int Size = sizeof(int) * 2;

        public int GetSize()
        {
            return Size;
        }

        public void InitFromFile(ref idFile file)
        {
            v = new int[2];

            v[0] = file.ReadInt();
            v[1] = file.ReadInt();
        }
    };

    //edge index, negative if vertexes are reversed
    public struct aas_edgeindextable_t
    {
        int[] edgeindextable;

        public int this[int index]
        {
            get
            {
                return edgeindextable[index];
            }
        }

        public int Count
        {
            get
            {
                return edgeindextable.Length;
            }
        }

        public void InitFromFile( ref idFile file, int numIndexes )
        {
            edgeindextable = new int[numIndexes];

            for( int i = 0; i < numIndexes; i++ )
            {
                edgeindextable[i] = file.ReadInt();
            }
        }
    }

    //a face bounds a convex area, often it will also seperate two convex areas
    public struct aas_face_t : aas_lumpinterface_t
    {
	    public int planenum;                       //number of the plane this face is in
	    public int faceflags;                      //face flags (no use to create face settings for just this field)
	    public int numedges;                       //number of edges in the boundary of the face
	    public int firstedge;                      //first edge in the edge index
	    public int frontarea;                      //convex area at the front of this face
	    public int backarea;                       //convex area at the back of this face

        public const int Size = sizeof( int ) * 6;

        public int GetSize()
        {
            return Size;
        }

        public void InitFromFile( ref idFile file )
        {
            planenum = file.ReadInt();
            faceflags = file.ReadInt();
            numedges = file.ReadInt();
            firstedge = file.ReadInt();
            frontarea = file.ReadInt();
            backarea = file.ReadInt();
        }
    };

    //face index, stores a negative index if backside of face
    public struct aas_faceindextable_t
    {
        int[] faceindextable;

        public int Count
        {
            get
            {
                return faceindextable.Length;
            }
        }

        public int this[int index]
        {
            get
            {
                return faceindextable[index];
            }
        }

        public void InitFromFile( ref idFile file, int numFaces )
        {
            faceindextable = new int[numFaces];

            for (int i = 0; i < numFaces; i++)
            {
                faceindextable[i] = file.ReadInt();
            }
        }
    }

    //convex area with a boundary of faces
    public struct aas_area_t : aas_lumpinterface_t
    {
	    public int areanum;                        //number of this area
	    //3d definition
	    public int numfaces;                       //number of faces used for the boundary of the convex area
	    public int firstface;                      //first face in the face index used for the boundary of the convex area
	    public idVector3 mins;                        //mins of the convex area
	    public idVector3 maxs;                        //maxs of the convex area
	    public idVector3 center;                      //'center' of the convex area

        public const int Size = (sizeof(int) * 3) + (idVector3.Size * 3);

        public int GetSize()
        {
            return Size;
        }

        public void InitFromFile(ref idFile file)
        {
            areanum = file.ReadInt();
            numfaces = file.ReadInt();
            firstface = file.ReadInt();

            mins = idVector3.vector_origin;
            maxs = idVector3.vector_origin;
            center = idVector3.vector_origin;

            file.ReadVector3(ref mins);
            file.ReadVector3(ref maxs);
            file.ReadVector3(ref center);
        }
    };

    //nodes of the bsp tree
    public struct aas_node_t : aas_lumpinterface_t
    {
        public const int Size = sizeof(int) * 3;
	    public int planenum;
	    public int[] children;                    //child nodes of this node, or convex areas as leaves when negative
										    //when a child is zero it's a solid leaf

        public int GetSize()
        {
            return Size;
        }

        public void InitFromFile( ref idFile file )
        {
            children = new int[2];

            planenum = file.ReadInt();
            children[0] = file.ReadInt();
            children[1] = file.ReadInt();
        }
    };

    //=========== aas file ===============

    //header lump
    public struct aas_lump_t
    {
	    public int fileofs;
	    public int filelen;

        public const int Size = sizeof(int) * 2;
    } ;

    public interface aas_lumpinterface_t
    {
        void InitFromFile( ref idFile file );
        int GetSize();
    }

    //aas file header
    public struct aas_header_t
    {
        public const int AASID = ( ( 'S' << 24 ) + ( 'A' << 16 ) + ( 'A' << 8 ) + 'E' );
        public const int AASVERSION = 8;

        public const int Size = (sizeof(int) * 3) + (aas_lump_t.Size * aas_lumptype.AAS_LUMPS);

	    public int ident;
	    public int version;
	    public int bspchecksum;
	    //data entries
	    public aas_lump_t[] lumps;

        //
        // AAS_DecryptHeader
        //
        private void AAS_DecryptHeader( ref idFile file, int size ) {
	        int i;
            idMsgReader buffer;
            byte[] data = new byte[size];

            for (i = 0; i < size; i++)
            {
                sbyte b = file.ReadSignedByte();
                b ^= (sbyte)(i * 119);
                data[i] = (byte)b;
            } //end for

            buffer = new idMsgReader(data);

            bspchecksum = buffer.ReadInt();

            for (i = 0; i < aas_lumptype.AAS_LUMPS; i++)
            {
                lumps[i].fileofs = buffer.ReadInt();
                lumps[i].filelen = buffer.ReadInt();
            }

            buffer.Dispose();
        }

        //
        // InitFromFile
        //
        public void InitFromFile( ref idFile file )
        {
            lumps = new aas_lump_t[aas_lumptype.AAS_LUMPS];

            ident = file.ReadInt();
            version = file.ReadInt();
            

            // Sanity check to ensure this is a AAS file.
            if(ident != AASID)
            {
                Engine.common.ErrorFatal( "AAS file ident is invalid...\n" );
            }

            // Check the AAS file version.
            if(version != AASVERSION)
            {
                Engine.common.ErrorFatal( "AAS Version is invalid, found %d expected %d \n", version, AASVERSION);
            }

            // Decrypt the lump bytes, and load the lumps.
            AAS_DecryptHeader(ref file, (aas_lump_t.Size * aas_lumptype.AAS_LUMPS) + 4);
        }
    };




}
