using idLib.Math;
using idLib.Engine.Public;
using Game.AAS.Private;

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

        //
        // BotMoveToGoal
        //
        public void BotMoveToGoal(ref aas_moveresult_t result, ref aas_movestate_t ms, Game.AI.idAIGoal goal, int travelflags)
        {
            // Clear the move result.
            ClearMoveResult(ref result);

            //remove some of the move flags
            ms.moveflags &= ~(aas_moveflags.MFL_SWIMMING | aas_moveflags.MFL_AGAINSTLADDER);
            //set some of the move flags
            //NOTE: the MFL_ONGROUND flag is also set in the higher AI
            if (Level.aas.IsPointOnGround(ms.origin, ms.presencetype, ms.entitynum))
            {
                ms.moveflags |= aas_moveflags.MFL_ONGROUND;
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
	    public int entitynum;                              //entity number of the bot
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
	    public int avoidreach;             //reachabilities to avoid
	    public float avoidreachtimes;      //times to avoid the reachabilities
	    public int avoidreachtries;        //number of tries before avoiding
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
