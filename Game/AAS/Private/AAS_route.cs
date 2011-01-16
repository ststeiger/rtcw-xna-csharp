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

// AAS_route.cs (c) 2010 JV Software
//

using idLib.Math;
using idLib.Engine.Public;

namespace Game.AAS.Private
{
    //
    // idAASRoute
    //
    public class idAASRoute
    {
        idAASRouteCacheFile routecache = new idAASRouteCacheFile();

        //index to retrieve travel flag for a travel type
        public int[] travelflagfortype;

        //routing update
        private aas_routingupdate_t[] areaupdate;
        private aas_routingupdate_t[] portalupdate;

        //reversed reachability links
        private aas_reversedreachability_t[] reversedreachability;

        //array of size numclusters with cluster cache
        private aas_routingcache_t[,] clusterareacache;
        private aas_routingcache_t[] portalcache;

        // travel times within areas.
        private aas_areatraveltime_t[] areatraveltimes;

        //maximum travel time through portals
        private int[] portalmaxtraveltimes;

        //travel time in hundreths of a second = distance * 100 / speed
        private const float DISTANCEFACTOR_CROUCH   =    1.3f;    //crouch speed = 100
        private const float DISTANCEFACTOR_SWIM     =    1f;       //should be 0.66, swim speed = 150
        private const float DISTANCEFACTOR_WALK = 0.33f;   //walk speed = 300

        // Ridah, scale traveltimes with ground steepness of area
        private const float GROUNDSTEEPNESS_TIMESCALE =  20f;  // this is the maximum scale, 1 being the usual for a flat ground

        //cache refresh time
        private const float CACHE_REFRESHTIME    =   15.0f;    //15 seconds refresh time

        //maximum number of routing updates each frame
        private const float MAX_FRAMEROUTINGUPDATES  =   100;

        //
        // InitTravelFlagFromType
        //
        private void InitTravelFlagFromType() {
	        int i;

            travelflagfortype = new int[aas_traveltype.MAX_TRAVELTYPES];

            for (i = 0; i < aas_traveltype.MAX_TRAVELTYPES; i++)
	        {
                travelflagfortype[i] = idAASTravelFlags.TFL_INVALID;
	        } //end for

            travelflagfortype[aas_traveltype.TRAVEL_INVALID] = idAASTravelFlags.TFL_INVALID;
            travelflagfortype[aas_traveltype.TRAVEL_WALK] = idAASTravelFlags.TFL_WALK;
            travelflagfortype[aas_traveltype.TRAVEL_CROUCH] = idAASTravelFlags.TFL_CROUCH;
            travelflagfortype[aas_traveltype.TRAVEL_BARRIERJUMP] = idAASTravelFlags.TFL_BARRIERJUMP;
            travelflagfortype[aas_traveltype.TRAVEL_JUMP] = idAASTravelFlags.TFL_JUMP;
            travelflagfortype[aas_traveltype.TRAVEL_LADDER] = idAASTravelFlags.TFL_LADDER;
            travelflagfortype[aas_traveltype.TRAVEL_WALKOFFLEDGE] = idAASTravelFlags.TFL_WALKOFFLEDGE;
            travelflagfortype[aas_traveltype.TRAVEL_SWIM] = idAASTravelFlags.TFL_SWIM;
            travelflagfortype[aas_traveltype.TRAVEL_WATERJUMP] = idAASTravelFlags.TFL_WATERJUMP;
            travelflagfortype[aas_traveltype.TRAVEL_TELEPORT] = idAASTravelFlags.TFL_TELEPORT;
            travelflagfortype[aas_traveltype.TRAVEL_ELEVATOR] = idAASTravelFlags.TFL_ELEVATOR;
            travelflagfortype[aas_traveltype.TRAVEL_ROCKETJUMP] = idAASTravelFlags.TFL_ROCKETJUMP;
            travelflagfortype[aas_traveltype.TRAVEL_BFGJUMP] = idAASTravelFlags.TFL_BFGJUMP;
            travelflagfortype[aas_traveltype.TRAVEL_GRAPPLEHOOK] = idAASTravelFlags.TFL_GRAPPLEHOOK;
            travelflagfortype[aas_traveltype.TRAVEL_DOUBLEJUMP] = idAASTravelFlags.TFL_DOUBLEJUMP;
            travelflagfortype[aas_traveltype.TRAVEL_RAMPJUMP] = idAASTravelFlags.TFL_RAMPJUMP;
            travelflagfortype[aas_traveltype.TRAVEL_STRAFEJUMP] = idAASTravelFlags.TFL_STRAFEJUMP;
            travelflagfortype[aas_traveltype.TRAVEL_JUMPPAD] = idAASTravelFlags.TFL_JUMPPAD;
            travelflagfortype[aas_traveltype.TRAVEL_FUNCBOB] = idAASTravelFlags.TFL_FUNCBOB;
        }

        //
        // AllocRoutingUpdate
        //
        private void AllocRoutingUpdate() {      
            areaupdate = new aas_routingupdate_t[Level.aas.AASWorld.aasfile.numareas];

	        //allocate memory for the portal update fields
	        portalupdate = new aas_routingupdate_t[Level.aas.AASWorld.aasfile.numportals + 1];
        }

        //
        // ClearTables
        //
        private void ClearTables()
        {
            //free routing update fields if already existing
            areaupdate = null;
            portalupdate = null;
            reversedreachability = null;
            clusterareacache = null;
            portalmaxtraveltimes = null;
            Engine.common.ForceGCCollect();
        }

        int AAS_ClusterAreaNum(int cluster, int areanum)
        {
            int side, areacluster;

            areacluster = Level.aas.AASWorld.aasfile.areasettings[areanum].cluster;
            if (areacluster > 0)
            {
                return Level.aas.AASWorld.aasfile.areasettings[areanum].clusterareanum;
            }
            else
            {
                /*#ifdef ROUTING_DEBUG
                        if ((*aasworld).portals[-areacluster].frontcluster != cluster &&
                                (*aasworld).portals[-areacluster].backcluster != cluster)
                        {
                            botimport.Print(PRT_ERROR, "portal %d: does not belong to cluster %d\n"
                                                            , -areacluster, cluster);
                        } //end if
                #endif //ROUTING_DEBUG*/
                if (Level.aas.AASWorld.aasfile.portals[-areacluster].frontcluster != cluster)
                {
                    side = 1;
                }
                else
                {
                    side = 0;
                }
                return Level.aas.AASWorld.aasfile.portals[-areacluster].clusterareanum[side];
            } //end else
        } //end of the function AAS_ClusterAreaNum

        private bool AAS_AreaRouteToGoalArea(int areanum, idVector3 origin, int goalareanum, int travelflags, ref int traveltime, ref int reachnum) {
	        int clusternum, goalclusternum, portalnum, i, clusterareanum, bestreachnum;
	        int t, besttime;
	        aas_portal_t portal;
	        aas_cluster_t cluster;
	        aas_routingcache_t areacache, portalcache;
	        aas_reachability_t reach;
	        int pPortalnum;

	        if ( areanum == goalareanum ) {
		        traveltime = 1;
		        reachnum = 0;
		        return true;
	        } 

	        if ( areanum <= 0 || areanum >= Level.aas.AASWorld.aasfile.numareas ) {
		        Engine.common.Warning( "AAS_AreaTravelTimeToGoalArea: areanum %d out of range\n", areanum );
		        return false;
	        }

            if (goalareanum <= 0 || goalareanum >= Level.aas.AASWorld.aasfile.numareas)
            {
                Engine.common.Warning("AAS_AreaTravelTimeToGoalArea: goalareanum %d out of range\n", goalareanum);
		        return false;
	        }

	        //make sure the routing cache doesn't grow to large
	      //  while ( routingcachesize > max_routingcachesize ) {
		 //       if ( !AAS_FreeOldestCache() ) {
		//	        break;
		//        }
	   //     }
	        //
            if (Level.aas.AAS_AreaDoNotEnter(areanum) || Level.aas.AAS_AreaDoNotEnter(goalareanum))
            {
                travelflags |= idAASTravelFlags.TFL_DONOTENTER;
	        }
            if (Level.aas.AAS_AreaDoNotEnterLarge(areanum) || Level.aas.AAS_AreaDoNotEnterLarge(goalareanum))
            {
                travelflags |= idAASTravelFlags.TFL_DONOTENTER_LARGE;
	        } 
	          
            //NOTE: the number of routing updates is limited per frame
	          /*
	          if ((*aasworld).frameroutingupdates > MAX_FRAMEROUTINGUPDATES)
	          {
          #ifdef DEBUG
		          //Log_Write("WARNING: AAS_AreaTravelTimeToGoalArea: frame routing updates overflowed");
          #endif
		          return 0;
	          } //end if
	          */
	          //
            clusternum = Level.aas.AASWorld.aasfile.areasettings[areanum].cluster;
            goalclusternum = Level.aas.AASWorld.aasfile.areasettings[goalareanum].cluster;
	        //check if the area is a portal of the goal area cluster
	        if ( clusternum < 0 && goalclusternum > 0 ) {
                portal = Level.aas.AASWorld.aasfile.portals[-clusternum];
		        if ( portal.frontcluster == goalclusternum ||
			         portal.backcluster == goalclusternum ) {
			        clusternum = goalclusternum;
		        } //end if
	        } //end if
	          //check if the goalarea is a portal of the area cluster
	        else if ( clusternum > 0 && goalclusternum < 0 ) {
                portal = Level.aas.AASWorld.aasfile.portals[-goalclusternum];
		        if ( portal.frontcluster == clusternum ||
			         portal.backcluster == clusternum ) {
			        goalclusternum = clusternum;
		        } //end if
	        } //end if
	          //if both areas are in the same cluster
	          //NOTE: there might be a shorter route via another cluster!!! but we don't care
	        if ( clusternum > 0 && goalclusternum > 0 && clusternum == goalclusternum ) {
		        //
		        areacache = routecache.GetAreaRoutingCache( clusternum, goalareanum, travelflags, false );
		        // RF, note that the routing cache might be NULL now since we are restricting
		        // the updates per frame, hopefully rejected cache's will be requested again
		        // when things have settled down
		        if ( areacache == null ) {
			        return false;
		        }
		        //the number of the area in the cluster
		        clusterareanum = AAS_ClusterAreaNum( clusternum, areanum );
		        //the cluster the area is in
                cluster = Level.aas.AASWorld.aasfile.clusters[clusternum];
		        //if the area is NOT a reachability area
		        if ( clusterareanum >= cluster.numreachabilityareas ) {
			        return false;
		        }
		        //if it is possible to travel to the goal area through this cluster
                if (areacache.traveltimes[clusterareanum] != 0)
                {
                    reachnum = Level.aas.AASWorld.aasfile.areasettings[areanum].firstreachablearea +
						        areacache.reachabilities[clusterareanum];
			        //
			        if ( origin == idVector3.vector_origin) {
				        traveltime = areacache.traveltimes[clusterareanum];
				        return true;
			        }
			        //
                    reach = Level.aas.AASWorld.aasfile.reachability[reachnum];
			        traveltime = areacache.traveltimes[clusterareanum] +
						          AAS_AreaTravelTime( areanum, origin, reach.start );
			        return true;
		        } //end if
	        } //end if
	          //
            clusternum = Level.aas.AASWorld.aasfile.areasettings[areanum].cluster;
            goalclusternum = Level.aas.AASWorld.aasfile.areasettings[goalareanum].cluster;
	        //if the goal area is a portal
	        if ( goalclusternum < 0 ) {
		        //just assume the goal area is part of the front cluster
                portal = Level.aas.AASWorld.aasfile.portals[-goalclusternum];
		        goalclusternum = portal.frontcluster;
	        } //end if
	          //get the portal routing cache
	        portalcache = routecache.GetPortalRoutingCache( goalclusternum, goalareanum, travelflags );
	        //if the area is a cluster portal, read directly from the portal cache
	        if ( clusternum < 0 ) {
		        traveltime = portalcache.traveltimes[-clusternum];
                reachnum = Level.aas.AASWorld.aasfile.areasettings[areanum].firstreachablearea +
					        portalcache.reachabilities[-clusternum];
		        return true;
	        }
	        //
	        besttime = 0;
	        bestreachnum = -1;
	        //the cluster the area is in
	        cluster = Level.aas.AASWorld.aasfile.clusters[clusternum];
	        //current area inside the current cluster
	        clusterareanum = AAS_ClusterAreaNum( clusternum, areanum );
	        //if the area is NOT a reachability area
	        if ( clusterareanum >= cluster.numreachabilityareas ) {
		        return false;
	        }
	        //
	        pPortalnum = Level.aas.AASWorld.aasfile.portalindex[cluster.firstportal];
	        //find the portal of the area cluster leading towards the goal area
	        for ( i = 0; i < cluster.numportals; i++, pPortalnum++ )
	        {
		        portalnum = pPortalnum;
		        //if the goal area isn't reachable from the portal
		        if ( portalcache.traveltimes[portalnum] == 0 ) {
			        continue;
		        }
		        //
		        portal = Level.aas.AASWorld.aasfile.portals[portalnum];
		        // if the area in disabled
                if ((Level.aas.AASWorld.aasfile.areasettings[portal.areanum].areaflags & aas_areaflags.AREA_DISABLED) != 0)
                {
			        continue;
		        }
		        //get the cache of the portal area
		        areacache = routecache.GetAreaRoutingCache( clusternum, portal.areanum, travelflags, false );
		        // RF, this may be NULL if we were unable to calculate the cache this frame
		        if ( areacache == null) {
			        return false;
		        }
		        //if the portal is NOT reachable from this area
		        if ( areacache.traveltimes[clusterareanum] == 0 ) {
			        continue;
		        }
		        //total travel time is the travel time the portal area is from
		        //the goal area plus the travel time towards the portal area
		        t = portalcache.traveltimes[portalnum] + areacache.traveltimes[clusterareanum];
		        //FIXME: add the exact travel time through the actual portal area
		        //NOTE: for now we just add the largest travel time through the area portal
		        //		because we can't directly calculate the exact travel time
		        //		to be more specific we don't know which reachability is used to travel
		        //		into the portal area when coming from the current area
                t += portalmaxtraveltimes[portalnum];
		        //
		        // Ridah, needs to be up here
		        reachnum = Level.aas.AASWorld.aasfile.areasettings[areanum].firstreachablearea +
					        areacache.reachabilities[clusterareanum];

        //botimport.Print(PRT_MESSAGE, "portal reachability: %i\n", (int)areacache->reachabilities[clusterareanum] );

		        if ( origin.X != 0 && origin.Y != 0 && origin.Z != 0 ) {
			        reach = Level.aas.AASWorld.aasfile.reachability[reachnum];
			        t += AAS_AreaTravelTime( areanum, origin, reach.start );
		        } 
		         //if the time is better than the one already found
		        if ( besttime == 0|| t < besttime ) {
			        bestreachnum = reachnum;
			        besttime = t;
		        } 
	        } //end for
	          // Ridah, check a route was found
	        if ( bestreachnum < 0 ) {
		        return false;
	        }
	        reachnum = bestreachnum;
	        traveltime = besttime;
	        return true;
        } 

        public int AAS_AreaTravelTimeToGoalAreaCheckLoop(int areanum, idVector3 origin, int goalareanum, int travelflags, int loopareanum)
        {
            int traveltime = 0, reachnum = 0;
            aas_reachability_t reach;

            if (AAS_AreaRouteToGoalArea(areanum, origin, goalareanum, travelflags, ref traveltime, ref reachnum))
            {
                reach = Level.aas.AASWorld.aasfile.reachability[reachnum];
                if (loopareanum != 0 && reach.areanum == loopareanum)
                {
                    return 0;   // going here will cause a looped route
                }
                return traveltime;
            }
            return 0;
        }

        //
        // BotValidTravel
        //
        private bool BotValidTravel(idVector3 origin, aas_reachability_t reach, int travelflags)
        {
            //if the reachability uses an unwanted travel type
            if ((Level.aas.AAS_TravelFlagForType(reach.traveltype) & ~travelflags) != 0)
            {
                return false;
            }
            //don't go into areas with bad travel types
            if ((Level.aas.AASWorld.aasroute.AAS_AreaContentsTravelFlag(reach.areanum) & ~travelflags) != 0)
            {
                return false;
            }
            return true;
        } //end of the function BotValidTravel

        //
        // BotGetReachabilityToGoal
        //
        public int BotGetReachabilityToGoal(idVector3 origin, int areanum, idEntity entity, int lastgoalareanum, int lastareanum,
                                              int[] avoidreach, float[] avoidreachtimes, int[] avoidreachtries, Game.AI.idAIGoal goal, int travelflags, int movetravelflags)
        {
            int t, besttime, bestreachnum, reachnum;
            aas_reachability_t reach;

            //if not in a valid area
            if (areanum == 0)
            {
                return 0;
            }
            //
            if (Level.aas.AAS_AreaDoNotEnter(areanum) || Level.aas.AAS_AreaDoNotEnter(goal.areanum))
            {
                travelflags |= idAASTravelFlags.TFL_DONOTENTER;
                movetravelflags |= idAASTravelFlags.TFL_DONOTENTER;
            } //end if
            if (Level.aas.AAS_AreaDoNotEnterLarge(areanum) || Level.aas.AAS_AreaDoNotEnterLarge(goal.areanum))
            {
                travelflags |= idAASTravelFlags.TFL_DONOTENTER_LARGE;
                movetravelflags |= idAASTravelFlags.TFL_DONOTENTER_LARGE;
            } //end if
            //use the routing to find the next area to go to
            besttime = 0;
            bestreachnum = 0;
            //
            for (reachnum = Level.aas.AAS_NextAreaReachability(areanum, 0); reachnum != 0;
                  reachnum = Level.aas.AAS_NextAreaReachability(areanum, reachnum))
            {
#if AVOIDREACH
		        int i;
		        //check if it isn't an reachability to avoid
		        for ( i = 0; i < MAX_AVOIDREACH; i++ )
		        {
			        if ( avoidreach[i] == reachnum && avoidreachtimes[i] >= AAS_Time() ) {
				        break;
			        }
		        } //end for
		        if ( i != MAX_AVOIDREACH && avoidreachtries[i] > AVOIDREACH_TRIES ) {
			        continue;
		        } //end if
#endif //AVOIDREACH
                //get the reachability from the number
                Level.aas.ReachabilityFromNum(reachnum, out reach);
                //NOTE: do not go back to the previous area if the goal didn't change
                //NOTE: is this actually avoidance of local routing minima between two areas???
                if (lastgoalareanum == goal.areanum && reach.areanum == lastareanum)
                {
                    continue;
                }
                //if (AAS_AreaContentsTravelFlag(reach.areanum) & ~travelflags) continue;
                //if the travel isn't valid
                if (!BotValidTravel(origin, reach, movetravelflags))
                {
                    continue;
                }
                //RF, ignore disabled areas
                if (Level.aas.AreaReachability(reach.areanum) == 0)
                {
                    continue;
                }
                //get the travel time (ignore routes that leads us back our current area)
                t = AAS_AreaTravelTimeToGoalAreaCheckLoop(reach.areanum, reach.end, goal.areanum, travelflags, areanum);
                //if the goal area isn't reachable from the reachable area
                if (t == 0)
                {
                    continue;
                }

                // Ridah, if this sends us to a looped route, ignore it
                //if (AAS_AreaTravelTimeToGoalArea(areanum, reach.start, goal->areanum, travelflags) + reach.traveltime < t)
                //	continue;

                //add the travel time towards the area
                // Ridah, not sure why this was disabled, but it causes looped links in the route-cache
                // RF, update.. seems to work better like this....
                t += reach.traveltime; // + AAS_AreaTravelTime(areanum, origin, reach.start);
                //t += reach.traveltime + AAS_AreaTravelTime(areanum, origin, reach.start);

                // Ridah, if there exists other entities in this area, avoid it
                //		if (reach.areanum != goal->areanum && AAS_IsEntityInArea( entnum, goal->entitynum, reach.areanum )) {
                //			t += 50;
                //		}

                //if the travel time is better than the ones already found
                if (besttime == 0 || t < besttime)
                {
                    besttime = t;
                    bestreachnum = reachnum;
                } //end if
            } //end for
            //
            return bestreachnum;
        }


        public int AAS_AreaContentsTravelFlag(int areanum)
        {
            int contents, tfl;

            contents = Level.aas.AASWorld.aasfile.areasettings[areanum].contents;
            tfl = 0;
            if ((contents & aas_areacontents.AREACONTENTS_WATER) != 0)
            {
                return tfl |= idAASTravelFlags.TFL_WATER;
            }
            else if ((contents & aas_areacontents.AREACONTENTS_SLIME) != 0)
            {
                return tfl |= idAASTravelFlags.TFL_SLIME;
            }
            else if ((contents & aas_areacontents.AREACONTENTS_LAVA) != 0)
            {
                return tfl |= idAASTravelFlags.TFL_LAVA;
            }
            else { tfl |= idAASTravelFlags.TFL_AIR; }
            if ((contents & aas_areacontents.AREACONTENTS_DONOTENTER_LARGE) != 0)
            {
                tfl |= idAASTravelFlags.TFL_DONOTENTER_LARGE;
            }
            if ((contents & aas_areacontents.AREACONTENTS_DONOTENTER) != 0)
            {
                return tfl |= idAASTravelFlags.TFL_DONOTENTER;
            }
            return tfl;
        }

        //
        // CreateReversedReachability
        //
        private void CreateReversedReachability() {
	        int i, n, revindex, revlinknum;
	        aas_reversedlink_t revlink;
	        aas_reachability_t reach;
	        aas_areasettings_t settings;

	        //allocate memory for the reversed reachability links
            reversedreachability = new aas_reversedreachability_t[Level.aas.AASWorld.aasfile.reachabilitysize];
            for (i = 0; i < reversedreachability.Length; i++)
            {
                reversedreachability[i] = new aas_reversedreachability_t(Level.aas.AASWorld.aasfile.numareas);
            }
            revindex = 0;
            revlinknum = 0;

	        //check all other areas for reachability links to the area
	        for ( i = 1; i < Level.aas.AASWorld.aasfile.numareas; i++ )
	        {
		        //settings of the area
                settings = Level.aas.AASWorld.aasfile.areasettings[i];
		        //check the reachability links
		        for ( n = 0; n < settings.numreachableareas; n++ )
		        {
			        //reachability link
                    reach = Level.aas.AASWorld.aasfile.reachability[settings.firstreachablearea + n];
			        //
                    if (revlinknum >= Level.aas.AASWorld.aasfile.numareas)
                    {
                        revindex++;
                        revlinknum = 0;
                    }
                    revlink = reversedreachability[revindex][revlinknum];
                    revlinknum++;
			        //
			        revlink.areanum = i;
			        revlink.linknum = settings.firstreachablearea + n;
                    revlink.next = reversedreachability[reach.areanum].first;
			        reversedreachability[reach.areanum].first = revlink;
			        reversedreachability[reach.areanum].numlinks++;
		        } //end for
	        } //end for
        } //end of the function AAS_CreateReversedReachability

        //
        // InitClusterAreaCache
        //
        private void InitClusterAreaCache() {
	        int i, c, size;

	        //
	        for ( size = 0, i = 0; i < Level.aas.AASWorld.aasfile.numclusters; i++ )
	        {
                size += Level.aas.AASWorld.aasfile.clusters[i].numareas;
	        } 
	          
            // two dimensional array with pointers for every cluster to routing cache
	        // for every area in that cluster
            clusterareacache = new aas_routingcache_t[Level.aas.AASWorld.aasfile.numclusters, size];
            for (i = 0; i < Level.aas.AASWorld.aasfile.numclusters; i++)
            {
                for (c = 0; c < size; c++)
                {
                    clusterareacache[i, c] = new aas_routingcache_t();
                }
            }
        }

        //
        // InitPortalCache
        //
        private void InitPortalCache()
        {
            portalcache = new aas_routingcache_t[Level.aas.AASWorld.aasfile.numareas];
        }

        //
        // AAS_AreaGroundSteepnessScale
        //
        private float AAS_AreaGroundSteepnessScale( int areanum ) {
            return (1.0f + Level.aas.AASWorld.aasfile.areasettings[areanum].groundsteepness * (float)(GROUNDSTEEPNESS_TIMESCALE - 1));
        }

        //
        //  AAS_AreaTravelTime
        //
        private ushort AAS_AreaTravelTime(int areanum, idVector3 start, idVector3 end)
        {
	        int intdist;
	        float dist;
	        idVector3 dir;

            dir = start - end; 
            dist = dir.Length();
	        // Ridah, factor in the groundsteepness now
	        dist *= AAS_AreaGroundSteepnessScale( areanum );

	        //if crouch only area
	        if ( Level.aas.AASWorld.CanAreaCrouch( areanum ) ) {
		        dist *= DISTANCEFACTOR_CROUCH;
	        }
	        //if swim area
            else if (Level.aas.AASWorld.CanAreaSwim(areanum))
            {
		        dist *= DISTANCEFACTOR_SWIM;
	        }
	        //normal walk area
	        else {dist *= DISTANCEFACTOR_WALK;}
	        //
	        intdist = (int) System.Math.Ceiling( dist );
	        //make sure the distance isn't zero
	        if ( intdist <= 0 ) {
		        intdist = 1;
	        }
	        return (ushort)intdist;
        } //end of the function AAS_AreaTravelTime

        //
        // CalculateAreaTravelTimes
        //
        private void CalculateAreaTravelTimes() {
	        int i, l, n, size;
	        idVector3 end;
	        aas_reversedreachability_t revreach;
	        aas_reversedlink_t revlink;
	        aas_reachability_t reach;
	        aas_areasettings_t settings;
         //   int ptrnum = 0;
          //  int ptrnum2 = 0;

            // Allocate all the travel times.
            areatraveltimes = new aas_areatraveltime_t[Level.aas.AASWorld.aasfile.numareas];
            for( i = 0; i < Level.aas.AASWorld.aasfile.numareas; i++ )
            {
                revreach = reversedreachability[i];
		        //settings of the area
		        settings = Level.aas.AASWorld.aasfile.areasettings[i];

                areatraveltimes[i] = new aas_areatraveltime_t( settings.numreachableareas );
                for( l = 0; l < areatraveltimes[i].Count; l++ )
                {
                    areatraveltimes[i][l].Init( revreach.numlinks );
                }
            }

         //   ptrnum++;
	        //calcluate the travel times for all the areas
	        for ( i = 0; i < Level.aas.AASWorld.aasfile.numareas; i++ )
	        {
		        //reversed reachabilities of this area
		        revreach = reversedreachability[i];
		        //settings of the area
		        settings = Level.aas.AASWorld.aasfile.areasettings[i];
		        //
            //    ptrnum2++;

		        //
		        for ( l = 0; l < settings.numreachableareas; l++ )
		        {
                    reach = Level.aas.AASWorld.aasfile.reachability[settings.firstreachablearea + l];

                  //  areatraveltimes[i][l] = areatraveltimes[ptrnum2][ptrnum];
                //    ptrnum ++;

			        //reachability link
			        //
			        for ( n = 0, revlink = revreach.first; revlink != null; revlink = revlink.next, n++ )
			        {
                        end = Level.aas.AASWorld.aasfile.reachability[revlink.linknum].end;

				        //
				        areatraveltimes[i][l][n] = AAS_AreaTravelTime( i, end, reach.start );
			        }
		        }
	        }
        }

        //
        // PortalMaxTravelTime
        //
        private int PortalMaxTravelTime(int portalnum)
        {
            int l, n, t, maxt;
            aas_portal_t portal;
            aas_reversedreachability_t revreach;
            aas_reversedlink_t revlink;
            aas_areasettings_t settings;

            portal = Level.aas.AASWorld.aasfile.portals[portalnum];
            //reversed reachabilities of this portal area
            revreach = reversedreachability[portal.areanum];
            //settings of the portal area
            settings = Level.aas.AASWorld.aasfile.areasettings[portal.areanum];
            //
            maxt = 0;
            for (l = 0; l < settings.numreachableareas; l++)
            {
                for (n = 0, revlink = revreach.first; revlink != null; revlink = revlink.next, n++)
                {
                    t = areatraveltimes[portal.areanum][l][n];
                    if (t > maxt)
                    {
                        maxt = t;
                    }
                }
            }
            return maxt;
        }

        //
        // InitPortalMaxTravelTimes
        //
        private void InitPortalMaxTravelTimes() {
	        int i, numportals;

            numportals = Level.aas.AASWorld.aasfile.numportals;
            portalmaxtraveltimes = new int[numportals];

	        for ( i = 0; i < numportals; i++ )
	        {
		        portalmaxtraveltimes[i] = PortalMaxTravelTime( i );
	        }
        }

        //
        // LoadCachedRoute
        //
        public bool Init(string mappath)
        {
            Engine.common.Printf("...Setting up AAS Routing\n");

            ClearTables();

            // Init the travel flag from type table.
            InitTravelFlagFromType();

            // initialize the routing update fields
            AllocRoutingUpdate();

            // create reversed reachability links used by the routing update algorithm
	        CreateReversedReachability();

            // initialize the cluster cache
            InitClusterAreaCache();

            // initialize portal cache
            InitPortalCache();

            // initialize the area travel times
	        CalculateAreaTravelTimes();

            // calculate the maximum travel times through portals
            InitPortalMaxTravelTimes();

            // Load the route cache, todo we need to build it ourselves.
            if (!routecache.LoadRouteCache(mappath))
            {
                return false;
            }
            
            return true;
        }

        //
        // aas_linkareatraveltime_t
        //
        class aas_linkareatraveltime_t
        {
            private ushort[] links;

            //
            // Init
            //
            public void Init(int numlinks)
            {
                links = new ushort[numlinks];
            }

            public ushort this[int index]
            {
                get
                {
                    return links[index];
                }

                set
                {
                    links[index] = value;
                }
            }

            public int Count
            {
                get
                {
                    return links.Length;
                }
            }

            //
            // Clear
            //
            public void Clear()
            {
                links = null;
            }
        }

        //
        // aas_areatraveltime_t
        //
        class aas_areatraveltime_t
        {
            private aas_linkareatraveltime_t[] reachableAreaTravelTime;

            //
            // aas_areatraveltime_t
            //
            public aas_areatraveltime_t(int numreachableareas)
            {
                reachableAreaTravelTime = new aas_linkareatraveltime_t[numreachableareas];

                for (int i = 0; i < numreachableareas; i++)
                {
                    reachableAreaTravelTime[i] = new aas_linkareatraveltime_t();
                }
            }

            //
            // Clear
            //
            public void Clear()
            {
                for (int i = 0; i < reachableAreaTravelTime.Length; i++)
                {
                    reachableAreaTravelTime[i].Clear();
                    reachableAreaTravelTime[i] = null;
                }
            }

            //
            // Count
            //
            public int Count
            {
                get
                {
                    return reachableAreaTravelTime.Length;
                }
            }

            //
            // this
            //
            public aas_linkareatraveltime_t this[int index]
            {
                get
                {
                    return reachableAreaTravelTime[index];
                }
                set
                {
                    reachableAreaTravelTime[index] = value;
                }
            }
        }

        //
        // aas_routingupdate_t
        //
        class aas_routingupdate_t
        {
	        public int cluster;
	        public int areanum;                                //area number of the update
	        public idVector3 start;                               //start point the area was entered
	        public ushort tmptraveltime;           //temporary travel time
	        public ushort[] areatraveltimes;        //travel times within the area
	        public bool inlist;                            //true if the update is in the list
	        public aas_routingupdate_t next;
            public aas_routingupdate_t prev;
        };

        //reversed reachability link
        class aas_reversedlink_t
        {
	        public int linknum;                                //the aas_areareachability_t
	        public int areanum;                                //reachable from this area
	        public aas_reversedlink_t next;            //next link
        };

        //reversed area reachability
        class aas_reversedreachability_t
        {
            private aas_reversedlink_t[] linkpool;

	        public int numlinks;
            public aas_reversedlink_t first;

            public aas_reversedlink_t this[int index]
            {
                get
                {
                    return linkpool[index];
                }
                set
                {
                    linkpool[index] = value;
                }
            }

            //
            // aas_reversedreachability_t
            //
            public aas_reversedreachability_t(int size)
            {
                aas_reversedlink_t link;

                linkpool = new aas_reversedlink_t[size];
                for (int i = 0; i < size; i++)
                {
                    linkpool[i] = new aas_reversedlink_t();
                }
                /*
                first = linkpool[0];
                link = first;
                for (int i = 1; i < size; i++)
                {
                    link.next = linkpool[i];
                    link = link.next;
                }
                */
            }
        };
    }
}
