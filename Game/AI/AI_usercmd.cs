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

// AI_usercmd.cs (c) 2010 JV Software
//

using idLib.Math;
using idLib.Engine.Public;

namespace Game.AI
{
    //the bot input, will be converted to an usercmd_t
    public struct idAIInput
    {
	    public float thinktime;        //time since last output (in seconds)
	    public idVector3 dir;             //movement direction
	    public float speed;            //speed in the range [0, 400]
	    public idVector3 viewangles;      //the view angles
	    public int actionflags;        //one of the ACTION_? flags
	    public int weapon;             //weapon to use
    };

    //
    // idAIUsercmd
    //
    public class idAIUsercmd
    {
        protected idUsercmd ucmd = new idUsercmd();
        protected idAIInput bi = new idAIInput();

        private const int MAX_USERMOVE = 400;

        //
        // Init
        //
        public void Init()
        {
            
        }

        //
        // idVector3
        //
        public void EA_Move(idVector3 dir, float speed)
        {
            bi.dir = dir;
            //cap speed
            if (speed > MAX_USERMOVE)
            {
                speed = MAX_USERMOVE;
            }
            else if (speed < -MAX_USERMOVE)
            {
                speed = -MAX_USERMOVE;
            }
            bi.speed = speed;
        }

        private void InputToUserCommand( idAICastState cs ) {
	        idVector3 angles, forward, right, up;
	        short temp;
	        int j;
	        sbyte movechar;
	        idEntity ent;
            idMatrix anglematrix;

            ent = cs.entity;
            angles = idVector3.vector_origin;

	        //clear the whole structure
            ucmd.InitCommand(0);
#if false // jv - implement me.
	        //
	        //Com_Printf("dir = %f %f %f speed = %f\n", bi->dir[0], bi->dir[1], bi->dir[2], bi->speed);
	        //the duration for the user command in milli seconds
	        ucmd.serverTime = serverTime;
	        //crouch/movedown
	        if ( aiDefaults[cs->aiCharacter].attributes[ATTACK_CROUCH] ) {    // only crouch if this character is physically able to
		        if ( cs->bs->cur_ps.groundEntityNum != ENTITYNUM_NONE && bi->actionflags & ACTION_CROUCH ) {
			        ucmd->upmove -= 127;
		        }
	        }
	        //
	        // actions not effected by script pausing
	        //
	        // set zoom button
	        if ( cs->aiFlags & AIFL_ZOOMING ) {
		        ucmd->wbuttons |= WBUTTON_ZOOM;
	        }
	        //set the buttons
	        if ( bi->actionflags & ACTION_ATTACK ) {
		        vec3_t ofs;
		        // don't fire if we are not facing the right direction yet
		        if ( ( cs->triggerReleaseTime < level.time ) &&
			         (   ( cs->lockViewAnglesTime >= level.time ) ||
				         ( fabs( AngleDifference( cs->ideal_viewangles[YAW], cs->viewangles[YAW] ) ) < 20 ) ) &&
			         // check for radid luger firing by skilled users (release fire between shots)
			         ( ( ( level.time + cs->entityNum * 500 ) / 2000 ) % 2 || !( rand() % ( 1 + g_gameskill.integer ) ) || ( cs->attributes[ATTACK_SKILL] < 0.5 ) || ( cs->weaponNum != WP_LUGER ) || ( cs->bs->cur_ps.weaponTime == 0 ) || ( cs->bs->cur_ps.releasedFire ) ) ) {
			        ucmd->buttons |= BUTTON_ATTACK;
			        // do some swaying around for some weapons
			        AICast_WeaponSway( cs, ofs );
			        VectorAdd( bi->viewangles, ofs, bi->viewangles );
		        }
	        }
	        //
	        //set the view angles
	        //NOTE: the ucmd->angles are the angles WITHOUT the delta angles
	        ucmd->angles[PITCH] = ANGLE2SHORT( bi->viewangles[PITCH] );
	        ucmd->angles[YAW] = ANGLE2SHORT( bi->viewangles[YAW] );
	        ucmd->angles[ROLL] = ANGLE2SHORT( bi->viewangles[ROLL] );
	        //subtract the delta angles
	        for ( j = 0; j < 3; j++ ) {
		        temp = ucmd->angles[j] - delta_angles[j];
		        ucmd->angles[j] = temp;
	        }


        //----(SA)	modified slightly for DM/DK
	        ucmd->weapon = bi->weapon;
	        //
	        // relaxed mode show no weapons
	        if ( cs->aiState <= AISTATE_QUERY ) {
		        if ( WEAPS_ONE_HANDED & ( 1 << ucmd->weapon ) ) { // one-handed wepons don't draw, others do
			        ucmd->weapon = WP_NONE;
		        }
	        }
        //----(SA)	end

	        //
	        if ( bi->actionflags & ACTION_GESTURE ) {
		        ucmd->buttons |= BUTTON_GESTURE;
	        }
	        if ( bi->actionflags & ACTION_RELOAD ) {
		        ucmd->wbuttons |= WBUTTON_RELOAD;
	        }
	        //
	        // if we are locked down, don't do anything
	        //
	        if ( cs->pauseTime > level.time ) {
		        return;
	        }
	        //
	        // if scripted pause, no movement
	        //
	        if ( cs->castScriptStatus.scriptNoMoveTime > level.time ) {
		        return;
	        }
	        //
	        // if viewlock, wait until we are facing ideal angles before we move
	        if ( cs->aiFlags & AIFL_VIEWLOCKED ) {
		        if ( fabs( AngleDifference( cs->ideal_viewangles[YAW], cs->viewangles[YAW] ) ) > 10 ) {
			        return;
		        }
	        }
	        //
	        if ( bi->actionflags & ACTION_DELAYEDJUMP ) {
		        bi->actionflags |= ACTION_JUMP;
		        bi->actionflags &= ~ACTION_DELAYEDJUMP;
	        }
#endif
	        //
	        // only move if we are in combat or we are facing where our ideal angles
	        if ( bi.speed != 0 ) {
#if false
		        if ( ( !( cs->aiFlags & AIFL_WALKFORWARD ) && cs->enemyNum >= 0 ) || ( ( ucmd->forwardmove >= 0 ) && fabs( AngleNormalize180( AngleDifference( cs->ideal_viewangles[YAW], cs->viewangles[YAW] ) ) ) < 60 ) ) {
#else
                if (System.Math.Abs(idMath.AngleNormalize180(idMath.AngleDifference(cs.ideal_viewangles[idMath.YAW], cs.viewangles[idMath.YAW]))) < 60)
                {
#endif
			        //NOTE: movement is relative to the REAL view angles
			        //get the horizontal forward and right vector
			        //get the pitch in the range [-180, 180]
			        if ( bi.dir[2] != 0 ) {
                        angles[idMath.PITCH] = bi.viewangles[idMath.PITCH];
                    }
                    else { 
                        angles[idMath.PITCH] = 0; 
                    }
                    angles[idMath.YAW] = bi.viewangles[idMath.YAW];
                    angles[idMath.ROLL] = 0;

                    anglematrix = angles.ToAxis();
			        
			        //bot input speed is in the range [0, 400]
			        bi.speed = bi.speed * 127 / 400;

                    forward = anglematrix[0];
                    right = anglematrix[1];
                    up = anglematrix[2];

			        //set the view independent movement
			        ucmd.ForwardMove = (int)(idMath.DotProduct( forward, bi.dir ) * bi.speed);
                    ucmd.RightMove = (int)(idMath.DotProduct(right, bi.dir) * bi.speed);

			        // RF, changed this to fix stim soldier flying attack
			      //  if ( !ucmd->upmove ) { // only change it if we don't already have an upmove set
				   //     ucmd->upmove = DotProduct( up, bi->dir ) * bi->speed;
			     //   }
			        //if (!ucmd->upmove)	// only change it if we don't already have an upmove set
			        //	ucmd->upmove = abs(forward[2]) * bi->dir[2] * bi->speed;
		        }
	        }
	        //
	        //normal keyboard movement
	      //  if ( cs.actionFlags & CASTACTION_WALK ) {
		        movechar = 70;
	      //  } else {
		  //      movechar = 127;
	    //    }
            if ((bi.actionflags & idBotActionFlags.ACTION_MOVEFORWARD) != 0)
            {
		        ucmd.ForwardMove = movechar;
            }
#if false
	        if ( !( cs->aiFlags & AIFL_WALKFORWARD ) || ( !cs->bs->cur_ps.groundEntityNum || cs->bs->cur_ps.groundEntityNum == ENTITYNUM_NONE ) ) {   // only do other movements if we are allowed to
		        if ( bi->actionflags & ACTION_MOVEBACK ) {
			        ucmd->forwardmove = -movechar;
		        }
		        if ( bi->actionflags & ACTION_MOVELEFT ) {
			        ucmd->rightmove = -movechar;
		        }
		        if ( bi->actionflags & ACTION_MOVERIGHT ) {
			        ucmd->rightmove = movechar;
		        }
	        }
	        // prevent WALKFORWARD AI from moving backwards

	        if ( cs.aiFlags & AIFL_WALKFORWARD ) {
		        if ( ucmd.forwardmove < 0 ) {
			        ucmd.forwardmove = 0;
		        }
	        }
	        //jump/moveup
            if ((bi.actionflags & idBotActionFlags.ACTION_JUMP) != 0)
            {
		        ucmd.upmove = 127;                                 // JUMP always takes preference over ducking
	        }
            if ((bi.actionflags & idBotActionFlags.ACTION_MOVEDOWN) != 0)
            {
		        ucmd.upmove = -127;                                    // JUMP always takes preference over ducking
	        }
            if ((bi.actionflags & idBotActionFlags.ACTION_MOVEUP) != 0)
            {
		        ucmd.upmove = 127;                                     // JUMP always takes preference over ducking
	        }
#endif
            //
	        //Com_Printf("forward = %d right = %d up = %d\n", ucmd.forwardmove, ucmd.rightmove, ucmd.upmove);
        }

        //
        // GetNextBotCommand
        //
        public idUsercmd GetNextBotCommand(idAICastState cs)
        {
            InputToUserCommand(cs);
            return ucmd;
        }
    }
}
