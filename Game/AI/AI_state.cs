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

// AI_state.cs (c) 2010 JV Software
//

using idLib.Math;

namespace Game.AI
{
    public enum aistateEnum_t
    {
	    AISTATE_RELAXED,
	    AISTATE_QUERY,
	    AISTATE_ALERT,
	    AISTATE_COMBAT,

	    MAX_AISTATES
    };

    public enum movestate_t
    {
	    MS_DEFAULT,
	    MS_WALK,
	    MS_RUN,
	    MS_CROUCH
    };
    //
    public enum movestateType_t
    {
	    MSTYPE_NONE,
	    MSTYPE_TEMPORARY,
	    MSTYPE_PERMANENT
    };

     // the main cast structure
     public class idAICastState
     {
	       //bot_state_t     *bs;
	        public idEntity entity;

	        public int aasWorldIndex;              // set this according to our bounding box type

	        // Cast specific information follows. Add to this as needed, this way the bot_state_t structure
	        // remains untouched.

	        public int aiCharacter;
	        public int aiFlags;
	        public int lastThink;                  // time they last thinked, so we can vary the think times
	        public int actionFlags;                // cast AI specific movement flags
	        public int lastPain, lastPainDamage;
	        public int travelflags;
	        public int thinkFuncChangeTime;

            public aistateEnum_t aiState;
            public movestate_t movestate;              // walk, run, crouch etc (can be specified in a script)
            public movestateType_t movestateType;      // temporary, permanent, etc

	      //  float attributes[AICAST_MAX_ATTRIBUTES];
	        // these define the abilities of each cast AI

            public delegate void idAIFunc();

            public idAIFunc aifunc;

	        // scripting system
#if false
	        int numCastScriptEvents;
	        cast_script_event_t     *castScriptEvents;  // contains a list of actions to perform for each event type
	        cast_script_status_t castScriptStatus;      // current status of scripting
	        cast_script_status_t castScriptStatusCurrent;       // scripting status to use for backups
	        cast_script_status_t castScriptStatusBackup;    // perm backup of status of scripting, only used by backup and restore commands
	        int scriptCallIndex;                        // inc'd each time a script is called
	        int scriptAnimTime, scriptAnimNum;                          // last time an anim was played using scripting
	        // the accumulation buffer
	        int scriptAccumBuffer[MAX_SCRIPT_ACCUM_BUFFERS];

	        //
	        cast_weapon_info_t  *weaponInfo;    // FIXME: make this a list, so they can have multiple weapons?
	        cast_visibility_t vislist[MAX_CLIENTS];         // array of all other client entities, allocated at level start-up
	        int weaponFireTimes[MAX_WEAPONS];

	        char    *( *aifunc )( struct cast_state_s *cs );            //current AI function
	        char    *( *oldAifunc )( struct cast_state_s *cs );         // just so we can restore the last aiFunc if required

	        char    *( *aifuncAttack1 )( struct cast_state_s *cs );     //use this battle aifunc for monster_attack1
	        char    *( *aifuncAttack2 )( struct cast_state_s *cs );     //use this battle aifunc for monster_attack2
	        char    *( *aifuncAttack3 )( struct cast_state_s *cs );     //use this battle aifunc for monster_attack2

	        void ( *painfunc )( gentity_t *ent, gentity_t *attacker, int damage, vec3_t point );
	        void ( *deathfunc )( gentity_t *ent, gentity_t *attacker, int damage, int mod ); //----(SA)	added mod
	        void ( *sightfunc )( gentity_t *ent, gentity_t *other, int lastSight );

	        //int		(*getDeathAnim)(gentity_t *ent, gentity_t *attacker, int damage);
	        void ( *sightEnemy )( gentity_t *ent, gentity_t *other );
	        void ( *sightFriend )( gentity_t *ent, gentity_t *other );

	        void ( *activate )( int entNum, int activatorNum );


	        //
	        // !!! NOTE: make sure any entityNum type variables get initialized
	        //		to -1 in AICast_CreateCharacter(), or they'll be defaulting to
	        //		the player (index 0)
	        //

	        // goal/AI stuff
#endif
            public bool isWalking;
	        public idEntity followEntity;
	        public float followDist;
	        public bool followIsGoto;      // we are really just going to the entity, but should wait until scripting tells us we can stop
	        public int followTime;             // if this runs out, the scripting has probably been interupted
	        public bool followSlowApproach;
#if false
	        int leaderNum;              // entnum of player we are following

	        float speedScale;           // so we can vary movement speed

	        float combatGoalTime;
	        vec3_t combatGoalOrigin;

	        int lastGetHidePos;
	        int startAttackCount;       // incremented each time we start a standing attack
								        // used to make sure we only find a combat spot once per attack
	        int combatSpotAttackCount;
	        int combatSpotDelayTime;
	        int startBattleChaseTime;

	        int blockedTime;            // time they were last blocked by a solid entity
	        int obstructingTime;        // time that we should move so we are not obstructing someone else
	        vec3_t obstructingPos;

	        int blockedAvoidTime;
	        float blockedAvoidYaw;

	        int deathTime;
	        int rebirthTime, revivingTime;

	        // battle values
	        int enemyHeight;
	        int enemyDist;

	        vec3_t takeCoverPos, takeCoverEnemyPos;
	        int takeCoverTime;

	        int attackSpotTime;

	        int triggerReleaseTime;

	        int lastWeaponFired;        // set each time a weapon is fired. used to detect when a weapon has been fired from within scripting
	        vec3_t lastWeaponFiredPos;
	        int lastWeaponFiredWeaponNum;

	        // idle behaviour stuff
	        int lastEnemy, nextIdleAngleChange;
	        float idleYawChange, idleYaw;

	        qboolean crouchHideFlag;

	        int doorMarker, doorEntNum;

	        // Rafael
	        int attackSNDtime;
	        int attacksnd;
	        int painSoundTime;
	        int firstSightTime;
	        qboolean secondDeadTime;
	        // done

	        int startGrenadeFlushTime;
	        int lockViewAnglesTime;
	        int grenadeFlushEndTime;
	        int grenadeFlushFiring;

	        int dangerEntity;
	        int dangerEntityValidTime;      // dangerEntity is valid until this time expires
	        vec3_t dangerEntityPos;         // dangerEntity is predicted to end up here
	        int dangerEntityTimestamp;      // time this danger was recorded
	        float dangerDist;

	        int mountedEntity;              // mg42, etc that we have mounted
	        int inspectBodyTime;
	        vec3_t startOrigin;

	        int damageQuota;
	        int damageQuotaTime;

	        int dangerLastGetAvoid;
	        int lastAvoid;

	        int doorMarkerTime, doorMarkerNum, doorMarkerDoor;

	        int pauseTime;                  // absolutely don't move move while this is > level.time

	        aicast_checkattack_cache_t checkAttackCache;

	        int secretsFound;

	        int attempts;

	        qboolean grenadeGrabFlag;       // if this is set, we need to play the anim before we can grab it

	        vec3_t lastMoveToPosGoalOrg;    // if this changes, we should reset the Bot Avoid Reach

	        int noAttackTime;               // used by dynamic AI to stop attacking for set time

	        int lastRollMove;
	        int lastFlipMove;

	        vec3_t stimFlyAttackPos;

	        int lastDodgeRoll;              // last time we rolled to get out of our enemies direct aim
	        int battleRollTime;

	        vec3_t viewlock_viewangles;
	        int grenadeKickWeapon;

	        int animHitCount;               // for stepping through the frames on which to inflict damage

	        int totalPlayTime, lastLoadTime;

	        int queryStartTime, queryCountValidTime, queryCount, queryAlertSightTime;

	        int lastScriptSound;

	        int inspectNum;

	        int scriptPauseTime;

	        int bulletImpactEntity;
	        int bulletImpactTime;           // last time we heard/saw a bullet impact
	        int bulletImpactIgnoreTime;
	        vec3_t bulletImpactStart, bulletImpactEnd;

	        int audibleEventTime;
	        vec3_t audibleEventOrg;
	        int audibleEventEnt;

	        int battleChaseMarker, battleChaseMarkerDir;

	        int lastBattleHunted;           // last time an enemy decided to hunt us
	        int battleHuntPauseTime, battleHuntViewTime;

	        int lastAttackCrouch;

	        int lastMoveThink;          // last time we ran our ClientThink()

	        int numEnemies;             // last count of enemies that are currently pursuing us

	        int noReloadTime;           // dont reload prematurely until this time has expired

	        int lastValidAreaNum[2];        // last valid area within each AAS world
	        int lastValidAreaTime[2];       // time we last got the area
#endif
	        public int weaponNum;              // our current weapon
            public int enemyNum;               // our current enemy
            public idVector3 ideal_viewangles, viewangles;
#if false
	        usercmd_t lastucmd;
#endif
            public int attackcrouch_time;
            public int bFlags;
#if false
	        int deadSinkStartTime;

	        int lastActivate;

	        vec3_t loperLeapVel;
	        // -------------------------------------------------------------------------------------------
	        // if working on a post release patch, new variables should ONLY be inserted after this point
	        // -------------------------------------------------------------------------------------------
#endif
        };
}
