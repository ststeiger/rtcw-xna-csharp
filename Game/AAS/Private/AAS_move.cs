using idLib.Math;
using idLib.Engine.Public;

using Game.AI;
using Game.AAS.Private;

// #define AVOIDREACH

namespace Game.AAS.Private
{
    //
    // idAASMove
    // 
    public class idAASMove
    {
        //
        // ClearMoveResult
        //
        private void ClearMoveResult(ref aas_moveresult_t moveresult)
        {
            moveresult.failure = 0;
            moveresult.type = 0;
            moveresult.blocked = 0;
            moveresult.blockentity = 0;
            moveresult.traveltype = 0;
            moveresult.flags = 0;
        }

        private int BotFuzzyPointReachabilityArea( idVector3 origin ) {
	        int firstareanum, j, x, y, z;
	        int numareas, areanum, bestareanum;
	        float dist, bestdist;
	        idVector3 v, end;

            int[] areas = new int[10];
            idVector3[] points = new idVector3[10];

	        firstareanum = 0;
	        areanum = Level.aas.PointAreaNum( origin );
	        if ( areanum != 0 ) {
		        firstareanum = areanum;
		        if ( Level.aas.AreaReachability( areanum ) != 0 ) {
			        return areanum;
		        }
	        }

            end = origin;
	        end[2] += 4;

	        numareas = Level.aas.TraceAreas( origin, end, ref areas, points, 10 );
	        for ( j = 0; j < numareas; j++ )
	        {
		        if ( Level.aas.AreaReachability( areas[j] ) != 0 ) {
			        return areas[j];
		        }
	        } //end for
	        bestdist = 999999;
	        bestareanum = 0;
	        //z = 0;
	        for ( z = 1; z >= -1; z -= 1 )
	        {
		        for ( x = 1; x >= -1; x -= 2 )
		        {
			        for ( y = 1; y >= -1; y -= 2 )
			        {
                        end = origin;

				        // Ridah, increased this for Wolf larger bounding boxes
				        end[0] += x * 256; //8;
				        end[1] += y * 256; //8;
				        end[2] += z * 200; //12;
				        numareas = Level.aas.TraceAreas( origin, end, ref areas, points, 10 );
				        for ( j = 0; j < numareas; j++ )
				        {
					        if ( Level.aas.AreaReachability( areas[j] ) != 0 ) {
                                v = points[j] - origin;
						        dist = v.Length();
						        if ( dist < bestdist ) {
							        bestareanum = areas[j];
							        bestdist = dist;
						        } 
					        } 
					        if ( firstareanum == 0) {
						        firstareanum = areas[j];
					        }
				        }
			        }
		        } 
		        if ( bestareanum != 0 ) {
			        return bestareanum;
		        }
	        }
	        return firstareanum;
        } 

        //
        // BotMoveInGoalArea
        //
        private void BotMoveInGoalArea( ref aas_moveresult_t result, ref aas_movestate_t ms, Game.AI.idAIGoal goal ) {
	        idVector3 dir, dirn;
	        float dist, speed;

            dir = idVector3.vector_origin;

	        ClearMoveResult(ref result);

	        //walk straight to the goal origin
	        dir[0] = goal.origin[0] - ms.origin[0];
	        dir[1] = goal.origin[1] - ms.origin[1];
	        if ( (ms.moveflags & aas_moveflags.MFL_SWIMMING) != 0 ) {
		        dir[2] = goal.origin[2] - ms.origin[2];
		        result.traveltype = aas_traveltype.TRAVEL_SWIM;
	        } //end if
	        else
	        {
		        dir[2] = 0;
		        result.traveltype = aas_traveltype.TRAVEL_WALK;
	        } 
	          
            dirn = dir;
	        dist = dirn.Normalize();
        // jv -fix me
	      //  if ( dist > 100 || ( goal.flags & GFL_NOSLOWAPPROACH ) ) {
		  //      dist = 100;
	     //   }
        // jv end
	        speed = 400 - ( 400 - 4 * dist );
	        if ( speed < 10 ) {
		        speed = 0;
	        }
	        //
	        //BotCheckBlocked( ms, dir, qtrue, &result );
	        //elemantary action move in direction
	        ((idBot)ms.entity).EA.EA_Move( dir, speed );
            result.movedir = dir;
	        //
	        //if ( ms->moveflags & MFL_SWIMMING ) {
		      //  Vector2Angles( dir, result.ideal_viewangles );
//		        result.flags |= MOVERESULT_SWIMVIEW;
	//        } //end if
	          //if (!debugline) debugline = botimport.DebugLineCreate();
	          //botimport.DebugLineShow(debugline, ms->origin, goal->origin, LINECOLOR_BLUE);
	          //
	        ms.lastreachnum = 0;
	        ms.lastareanum = 0;
	        ms.lastgoalareanum = goal.areanum;
            ms.lastorigin = ms.origin;
	        ms.lasttime = Level.aas.AAS_Time();

        }

        //
        // BotReachabilityTime
        //
        private int BotReachabilityTime(aas_reachability_t reach)
        {
            switch (reach.traveltype)
            {
                case aas_traveltype.TRAVEL_WALK: return 5;
                case aas_traveltype.TRAVEL_CROUCH: return 5;
                case aas_traveltype.TRAVEL_BARRIERJUMP: return 5;
                case aas_traveltype.TRAVEL_LADDER: return 6;
                case aas_traveltype.TRAVEL_WALKOFFLEDGE: return 5;
                case aas_traveltype.TRAVEL_JUMP: return 5;
                case aas_traveltype.TRAVEL_SWIM: return 5;
                case aas_traveltype.TRAVEL_WATERJUMP: return 5;
                case aas_traveltype.TRAVEL_TELEPORT: return 5;
                case aas_traveltype.TRAVEL_ELEVATOR: return 10;
                case aas_traveltype.TRAVEL_GRAPPLEHOOK: return 8;
                case aas_traveltype.TRAVEL_ROCKETJUMP: return 6;
                //case TRAVEL_BFGJUMP: return 6;
                case aas_traveltype.TRAVEL_JUMPPAD: return 10;
                case aas_traveltype.TRAVEL_FUNCBOB: return 10;
                default:
                    {
                        Engine.common.ErrorFatal( "travel type %d not implemented yet\n", reach.traveltype);
                        return 8;
                    }     //end case
            } //end switch
        }

        //
        // BotTravel_Walk
        //
        private aas_moveresult_t BotTravel_Walk(ref aas_movestate_t ms, aas_reachability_t reach)
        {
            float dist, speed;
            idVector3 hordir, hordirn;
            aas_moveresult_t result = new aas_moveresult_t();

            hordir = idVector3.vector_origin;

            //first walk straight to the reachability start
            hordir[0] = reach.start[0] - ms.origin[0];
            hordir[1] = reach.start[1] - ms.origin[1];
            hordir[2] = 0;
            dist = hordir.Normalize();
            //
            
            //
            // Ridah, tweaked this
            //	if (dist < 10)
            if (dist < 32)
            {
                //walk straight to the reachability end
                hordir[0] = reach.end[0] - ms.origin[0];
                hordir[1] = reach.end[1] - ms.origin[1];
                hordir[2] = 0;

                hordirn = hordir;
                dist = hordirn.Normalize();
            } //end if
            //if going towards a crouch area

            // Ridah, some areas have a 0 presence (!?!)
            //	if (!(AAS_AreaPresenceType(reach->areanum) & PRESENCE_NORMAL))
            if ((Level.aas.AASWorld.aasfile.areasettings[reach.areanum].presencetype & aas_presencetype.PRESENCE_CROUCH) != 0 &&
                     (Level.aas.AASWorld.aasfile.areasettings[reach.areanum].presencetype & aas_presencetype.PRESENCE_NORMAL) == 0)
            {
                //if pretty close to the reachable area
                if (dist < 20)
                {
                 //   EA_Crouch(ms->client);
                }
            } //end if
            //
          //  dist = BotGapDistance(ms->origin, hordir, ms->entitynum);
            //
            /*
            if (ms->moveflags & MFL_WALK)
            {
                if (dist > 0)
                {
                    speed = 200 - (180 - 1 * dist);
                }
                else { speed = 200; }
                EA_Walk(ms->client);
            } //end if
            else
            {
                if (dist > 0)
                {
                    speed = 400 - (360 - 2 * dist);
                }
                else { speed = 400; }
            } //end else
            */

            speed = 200;

            //elemantary action move in direction
            ((idBot)ms.entity).EA.EA_Move(hordir, speed);
            result.movedir = hordir;
            
            return result;
        }

        //
        // BotMoveToGoal
        //
        public void BotMoveToGoal(ref aas_moveresult_t result, ref aas_movestate_t ms, Game.AI.idAIGoal goal, int travelflags)
        {
            aas_reachability_t reach, lastreach;
            int reachnum;

            // Clear the move result.
            ClearMoveResult(ref result);

            //remove some of the move flags
            ms.moveflags &= ~(aas_moveflags.MFL_SWIMMING | aas_moveflags.MFL_AGAINSTLADDER);
            //set some of the move flags
            //NOTE: the MFL_ONGROUND flag is also set in the higher AI
            if (Level.aas.IsPointOnGround(ms.origin, ms.presencetype, ms.entity))
            {
                ms.moveflags |= aas_moveflags.MFL_ONGROUND;
            }

            //if the bot is on the ground, swimming or against a ladder
	        if ( (ms.moveflags & ( aas_moveflags.MFL_ONGROUND | aas_moveflags.MFL_SWIMMING | aas_moveflags.MFL_AGAINSTLADDER )) != 0 ) {
		        Level.aas.ReachabilityFromNum( ms.lastreachnum, out lastreach );
		        
                //reachability area the bot is in
		        ms.areanum = BotFuzzyPointReachabilityArea( ms.origin );
		        
                //if the bot is in the goal area
		        if ( ms.areanum == goal.areanum ) {
			        BotMoveInGoalArea( ref result, ref ms, goal );
			        return;
		        } //end if
		          //assume we can use the reachability from the last frame
		        reachnum = ms.lastreachnum;
		        //if there is a last reachability

        // Ridah, best calc it each frame, especially for Zombie, which moves so slow that we can't afford to take a long route
        //		reachnum = 0;
        // done.
		        if ( reachnum != 0 ) {
			        Level.aas.ReachabilityFromNum( reachnum, out reach );
			        //check if the reachability is still valid
			        if ( ( Level.aas.AAS_TravelFlagForType( reach.traveltype ) & travelflags ) == 0 ) {
				        reachnum = 0;
			        } //end if
			          //special grapple hook case
			        else if ( reach.traveltype == aas_traveltype.TRAVEL_GRAPPLEHOOK ) {
				        if ( ms.reachability_time < Level.aas.AAS_Time() ||
					         ( ms.moveflags & aas_moveflags.MFL_GRAPPLERESET ) != 0 ) {
					        reachnum = 0;
				        } //end if
			        } //end if
			          //special elevator case
			        else if ( reach.traveltype == aas_traveltype.TRAVEL_ELEVATOR || reach.traveltype == aas_traveltype.TRAVEL_FUNCBOB ) {
				      //  if ( ( result.flags & MOVERESULT_ONTOPOF_FUNCBOB ) ||
					 //        ( result.flags & MOVERESULT_ONTOPOF_FUNCBOB ) ) {
					 //       ms.reachability_time = AAS_Time() + 5;
				     //   } //end if
				          //if the bot was going for an elevator and reached the reachability area
				        if ( ms.areanum == reach.areanum ||
					         ms.reachability_time < Level.aas.AAS_Time() ) {
					        reachnum = 0;
				        } //end if
			        } //end if
			        else
			        {
				        //if the goal area changed or the reachability timed out
				        //or the area changed
				        if ( ms.lastgoalareanum != goal.areanum ||
					         ms.reachability_time < Level.aas.AAS_Time() ||
					         ms.lastareanum != ms.areanum ||
					         ( ( ms.lasttime > ( Level.aas.AAS_Time() - 0.5f ) ) && ( ms.origin.Distance( ms.lastorigin ) < 20 * ( Level.aas.AAS_Time() - ms.lasttime ) ) ) ) {
					        reachnum = 0;
					        //botimport.Print(PRT_MESSAGE, "area change or timeout\n");
				        } //end else if
			        } //end else
		        } //end if
		          //if the bot needs a new reachability
		        if ( reachnum == 0 ) {
			        //if the area has no reachability links
			        if ( Level.aas.AreaReachability( ms.areanum ) == 0 ) {
				        Engine.common.Warning( "area %d no reachability\n", ms.areanum );
			        } //end if
			          //get a new reachability leading towards the goal
			        reachnum = Level.aas.AASWorld.aasroute.BotGetReachabilityToGoal( ms.origin, ms.areanum, ms.entity,
												         ms.lastgoalareanum, ms.lastareanum,
												         ms.avoidreach, ms.avoidreachtimes, ms.avoidreachtries,
												         goal, travelflags, travelflags );
			        //the area number the reachability starts in
			        ms.reachareanum = ms.areanum;
			        //reset some state variables
			        ms.jumpreach = 0;                      //for TRAVEL_JUMP
			      //  ms.moveflags &= ~MFL_GRAPPLERESET; //for TRAVEL_GRAPPLEHOOK
			        //if there is a reachability to the goal
			        if ( reachnum != 0 ) {
				        Level.aas.ReachabilityFromNum( reachnum, out reach );
				        //set a timeout for this reachability
				        ms.reachability_time = Level.aas.AAS_Time() + BotReachabilityTime( reach );
				        //
				        //add the reachability to the reachabilities to avoid for a while
				        //BotAddToAvoidReach( ms, reachnum, AVOIDREACH_TIME );
			        } //end if
       
		        } //end else
		          //
		        ms.lastreachnum = reachnum;
		        ms.lastgoalareanum = goal.areanum;
		        ms.lastareanum = ms.areanum;
		        //if the bot has a reachability
		        if ( reachnum != 0 ) {
			        //get the reachability from the number
			        Level.aas.ReachabilityFromNum( reachnum, out reach );
			        result.traveltype = reach.traveltype;
			        //
			        switch ( reach.traveltype )
			        {
                        case aas_traveltype.TRAVEL_WALK: result = BotTravel_Walk(ref ms, reach); break;
#if false
                        case aas_traveltype.TRAVEL_CROUCH: result = BotTravel_Crouch(ms, &reach); break;
                        case aas_traveltype.TRAVEL_BARRIERJUMP: result = BotTravel_BarrierJump(ms, &reach); break;
                        case aas_traveltype.TRAVEL_LADDER: result = BotTravel_Ladder(ms, &reach); break;
                        case aas_traveltype.TRAVEL_WALKOFFLEDGE: result = BotTravel_WalkOffLedge(ms, &reach); break;
                        case aas_traveltype.TRAVEL_JUMP: result = BotTravel_Jump(ms, &reach); break;
                        case aas_traveltype.TRAVEL_SWIM: result = BotTravel_Swim(ms, &reach); break;
                        case aas_traveltype.TRAVEL_WATERJUMP: result = BotTravel_WaterJump(ms, &reach); break;
                        case aas_traveltype.TRAVEL_TELEPORT: result = BotTravel_Teleport(ms, &reach); break;
                        case aas_traveltype.TRAVEL_ELEVATOR: result = BotTravel_Elevator(ms, &reach); break;
                        case aas_traveltype.TRAVEL_GRAPPLEHOOK: result = BotTravel_Grapple(ms, &reach); break;
                        case aas_traveltype.TRAVEL_ROCKETJUMP: result = BotTravel_RocketJump(ms, &reach); break;
				        //case TRAVEL_BFGJUMP:
                        case aas_traveltype.TRAVEL_JUMPPAD: result = BotTravel_JumpPad(ms, &reach); break;
                        case aas_traveltype.TRAVEL_FUNCBOB: result = BotTravel_FuncBobbing(ms, &reach); break;
#endif
			            default:
			            {
				            Engine.common.ErrorFatal("travel type %d not implemented yet\n", reach.traveltype );
				            break;
			            }     //end case
			        } //end switch
		        } //end if
		        else
		        {
			        result.failure = 1;
		        } 
	        } 
        }
    }

    public class aas_moveflags
    {
        //move flags
        public const int MFL_BARRIERJUMP          =       1;       //bot is performing a barrier jump
        public const int MFL_ONGROUND             =       2;       //bot is in the ground
        public const int MFL_SWIMMING             =       4;       //bot is swimming
        public const int MFL_AGAINSTLADDER        =       8;       //bot is against a ladder
        public const int MFL_WATERJUMP            =       16;      //bot is waterjumping
        public const int MFL_TELEPORTED           =       32;      //bot is being teleported
        public const int MFL_ACTIVEGRAPPLE        =       64;      //bot is using the grapple hook
        public const int MFL_GRAPPLERESET         =       128;     //bot has reset the grapple
        public const int MFL_WALK                 =       256;     //bot should walk slowly
    }


    public struct aas_movestate_t
    {
	    //input vars (all set outside the movement code)
	    public idVector3 origin;                              //origin of the bot
	    public idVector3 velocity;                            //velocity of the bot
	    public idVector3 viewoffset;                          //view offset
	    public idEntity entity;                              //entity number of the bot
	    public int client;                                 //client number of the bot
	    public float thinktime;                            //time the bot thinks
	    public int presencetype;                           //presencetype of the bot
	    public idVector3 viewangles;                          //view angles of the bot
	    //state vars
	    public int areanum;                                //area the bot is in
	    public int lastareanum;                            //last area the bot was in
	    public int lastgoalareanum;                        //last goal area number
	    public int lastreachnum;                           //last reachability number
	    public idVector3 lastorigin;                          //origin previous cycle
	    public float lasttime;
	    public int reachareanum;                           //area number of the reachabilty
	    public int moveflags;                              //movement flags
	    public int jumpreach;                              //set when jumped
	    public float grapplevisible_time;                  //last time the grapple was visible
	    public float lastgrappledist;                      //last distance to the grapple end
	    public float reachability_time;                    //time to use current reachability
	    public int[] avoidreach;             //reachabilities to avoid
	    public float[] avoidreachtimes;      //times to avoid the reachabilities
	    public int[] avoidreachtries;        //number of tries before avoiding
    };

    public struct aas_moveresult_t
    {
        public int failure;                //true if movement failed all together
        public int type;                   //failure or blocked type
        public int blocked;                //true if blocked by an entity
        public int blockentity;            //entity blocking the bot
        public int traveltype;             //last executed travel type
        public int flags;                  //result flags
        public int weapon;                 //weapon used for movement
        public idVector3 movedir;             //movement direction
        public idVector3 ideal_viewangles;    //ideal viewangles for the movement
    };
}
