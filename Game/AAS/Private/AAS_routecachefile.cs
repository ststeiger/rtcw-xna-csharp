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

// AAS_routecachefile.cs (c) 2010 JV Software
//

using idLib.Math;
using idLib.Engine.Public;

namespace Game.AAS.Private
{
    //
    // idAASRouteCacheFile
    //
    public class idAASRouteCacheFile
    {
        public aas_routingcache_t[] portalcache;
        public routeClusterCacheContainer_t[] clusterareacache;
        public aas_storedvisibility_t[] areavisibility;
        public aas_storedvisibility_t[] decompressedvis;
        public idVector3[] areawaypoints;

        //
        // idAASRouteCacheFile
        //
        public idAASRouteCacheFile()
        {

        }

        //
        // ClearRouteCache
        //
        public void ClearRouteCache()
        {

        }

        //
        // AAS_ClusterAreaNum
        //
        private int AAS_ClusterAreaNum(int cluster, int areanum)
        {
            int side, areacluster;

            areacluster = Level.aas.AASWorld.aasfile.areasettings[areanum].cluster;
            if (areacluster > 0)
            {
                return Level.aas.AASWorld.aasfile.areasettings[areanum].clusterareanum;
            }
            else
            {
                if (Level.aas.AASWorld.aasfile.portals[-areacluster].frontcluster != cluster)
                {
                    side = 1;
                }
                else
                {
                    side = 0;
                }
                return Level.aas.AASWorld.aasfile.portals[-areacluster].clusterareanum[side];
            }
        }

        //
        // InitClusterAreaCache
        //
        private void InitClusterAreaCache()
        {
            clusterareacache = new routeClusterCacheContainer_t[Level.aas.AASWorld.aasfile.numclusters];

            for (int i = 0; i < Level.aas.AASWorld.aasfile.numclusters; i++)
            {
                clusterareacache[i].Init(Level.aas.AASWorld.aasfile.clusters[i].numareas);
            }
        }

        //
        // LoadRouteCache
        //
        public bool LoadRouteCache(string path)
        {
            idFile file;
            routecacheheader_t header = new routecacheheader_t();
            
            file = Engine.fileSystem.OpenFileRead(path, true);
            if (file == null)
            {
                Engine.common.Warning("AAS_LoadRouteCache: Failed to open file %s \n", path);
                return false;
            }

            Engine.common.Printf("...parsing route cache\n");

            InitClusterAreaCache();

            // Load in the route cache header.
            header.InitFromFile(ref file);

            // Read all the portal caches.
            portalcache = new aas_routingcache_t[header.numareas];

            for (int i = 0; i < header.numportalcache; i++)
            {
                aas_routingcache_t cache = new aas_routingcache_t();
                cache.InitFromFile(ref file);
                cache.prev = null;
                if (portalcache[cache.areanum] != null) 
                {
                    portalcache[cache.areanum].prev = cache;
                }
                portalcache[cache.areanum] = cache;
                cache.next = portalcache[cache.areanum];
            }

            //read all the cluster area cache
            for (int i = 0; i < header.numareacache; i++)
            {
                aas_routingcache_t cache = new aas_routingcache_t();
                int clusterareanum;

                cache.InitFromFile(ref file);
                clusterareanum = AAS_ClusterAreaNum(cache.cluster, cache.areanum);
                cache.next = clusterareacache[cache.cluster][clusterareanum];
                cache.prev = null;
                if (clusterareacache[cache.cluster][clusterareanum] != null)
                {
                    clusterareacache[cache.cluster][clusterareanum].prev = cache;
                }
                clusterareacache[cache.cluster][clusterareanum] = cache;
            }

            areavisibility = new aas_storedvisibility_t[Level.aas.AASWorld.aasfile.numareas];
            decompressedvis = new aas_storedvisibility_t[Level.aas.AASWorld.aasfile.numareas];

            for (int i = 0; i < Level.aas.AASWorld.aasfile.numareas; i++)
            {
                int len;

                len = file.ReadInt();
                if (len > 0)
                {
                    areavisibility[i] = new aas_storedvisibility_t();
                    areavisibility[i].InitFromFile( len, ref file );
                }
            }

            areawaypoints = new idVector3[Level.aas.AASWorld.aasfile.numareas];
            for (int i = 0; i < Level.aas.AASWorld.aasfile.numareas; i++)
            {
                areawaypoints[i] = idVector3.vector_origin;
                file.ReadVector3(ref areawaypoints[i]);
            }
            Engine.fileSystem.CloseFile(ref file);

            return true;
        }
    }

    //
    // routeClusterCacheContainer_t
    //
    public struct routeClusterCacheContainer_t
    {
        aas_routingcache_t[] cache;

        //
        // aas_routingcache_t
        // 
        public aas_routingcache_t this[int index]
        {
            get
            {
                return cache[index];
            }
            set
            {
                cache[index] = value;
            }
        }

        //
        // Init
        //
        public void Init( int cacheSize )
        {
            cache = new aas_routingcache_t[cacheSize];
        }
    }

    //the route cache header
    //this header is followed by numportalcache + numareacache aas_routingcache_t
    //structures that store routing cache
    struct routecacheheader_t
    {
        public const int RCID = (('C' << 24) + ('R' << 16) + ('E' << 8) + 'M');
        public const int RCVERSION  = 15;

	    public int ident;
	    public int version;
	    public int numareas;
	    public int numclusters;
	    public int areacrc;
	    public int clustercrc;
	    public int reachcrc;
	    public int numportalcache;
	    public int numareacache;

        //
        // InitFromFile
        //
        public void InitFromFile(ref idFile file)
        {
            ident = file.ReadInt();
            version = file.ReadInt();
            numareas = file.ReadInt();
            numclusters = file.ReadInt();
            areacrc = file.ReadInt();
            clustercrc = file.ReadInt();
            reachcrc = file.ReadInt();
            numportalcache = file.ReadInt();
            numareacache = file.ReadInt();

            if (ident != RCID)
            {
                Engine.common.ErrorFatal("AASRoute_InitFromFile: Route cache has a invalid ident\n");
            }

            if (version != RCVERSION)
            {
                Engine.common.ErrorFatal("AASRoute_InitFromFile: Route cache invalid version.\n");
            }
        }
    };

    public class aas_storedvisibility_t
    {
        public byte[] visibility;

        public void InitFromFile(int size, ref idFile file)
        {
            visibility = file.ReadBytes(size);
        }
    }

    public class aas_routingcache_t
    {
	    public int cachesize;                                   //size of the routing cache
	    public float time;                                 //last time accessed or updated
	    public int cluster;                                //cluster the cache is for
	    public int areanum;                                //area the cache is created for
	    public idVector3 origin;                              //origin within the area
	    public float starttraveltime;                      //travel time to start with
	    public int travelflags;                            //combinations of the travel flags
	    public sbyte[] reachabilities;              //reachabilities used for routing
	    public ushort[] traveltimes;          //travel time for every area (variable sized)
        public aas_routingcache_t prev, next;

        public const int Size = 52;

        public void InitFromFile(ref idFile file)
        {
            int numreachabilities, numtraveltimes;
            int cacheEndPosition;

            cacheEndPosition = file.Tell();

            cachesize = file.ReadInt();

            cacheEndPosition += cachesize;

            time = file.ReadFloat();
            cluster = file.ReadInt();
            areanum = file.ReadInt();
            file.ReadVector3( ref origin );
            starttraveltime = file.ReadFloat();
            travelflags = file.ReadInt();

            numtraveltimes = (cachesize - Size) / 3 + 1;// The guy that did this got this by doing sizeof(ushort) + byte = 3 :/.
            traveltimes = new ushort[numtraveltimes];

            // Travel times than reachability table? wtf.
            for (int i = 0; i < numtraveltimes; i++)
            {
                traveltimes[i] = file.ReadUShort();
            }

            numreachabilities = cacheEndPosition - file.Tell(); // Rest of the cache belongs to the reachability array.
            reachabilities = new sbyte[numreachabilities];
            for (int i = 0; i < numreachabilities; i++)
            {
                reachabilities[i] = file.ReadSignedByte();
            }
        }
    };

    
}
