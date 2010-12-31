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

// physics_move.cs (c) 2010 JV Software
//

using idLib.Game;
using idLib.Math;
using idLib.Engine.Public;

namespace Game.Physics
{
    //
    // idPlayerPhysics
    //

    /*
     *
     * FIXME: This is exact wrong way to do this, but I just have it here so I can test other things
     * 
    */
    public class idPlayerPhysics
    {
        idVector3 velocity = new idVector3();
        bool isWalking = false;

        /*
        ==================
        PM_Friction

        Handles both ground friction and water friction
        ==================
        */
        private void Physics_Friction()
        {
            float speed = 0;
            float drop = 0;
            float newspeed = 0;

            if (isWalking)
            {
                velocity[2] = 0; // ignore slope movement
            }

            speed = velocity.Length();
            if (speed <= 0)
            {
                velocity[0] = 0;
                velocity[1] = 0;     // allow sinking underwater
                // FIXME: still have z friction underwater?
                return;
            }

            // scale the velocity
            newspeed = speed - drop;
            if (newspeed < 0)
            {
                newspeed = 0;
            }
            newspeed /= speed;
            velocity *= newspeed;
        }

        /*
        ================
        PM_UpdateViewAngles

        This can be used as another entry point when only the viewangles
        are being updated isntead of a full move
        ================
        */
        private void UpdateViewAngles( ref entityState_t ps, idUsercmd cmd ) {   //----(SA)	modified
	        short temp;
	        int i;

            ps.angles2[0] = cmd.pitch;
            ps.angles2[1] = cmd.yaw;
        }

        /*
       ============
       PM_CmdScale

       Returns the scale factor to apply to cmd movements
       This allows the clients to use axial -127 to 127 values for all directions
       without getting a sqrt(2) distortion in speed.
       ============
       */
        private float PM_CmdScale(idUsercmd cmd, bool isAI)
        {
            int max;
            float total;
            float scale;

            if (isAI)
            {
                /*
                // restrict AI character movements (don't strafe or run backwards as fast as they can run forwards)
                if (cmd.forwardmove < -64.0)
                {
                    cmd.forwardmove = -64;
                }
                if (cmd.rightmove > 64.0)
                {
                    cmd.rightmove = 64;
                }
                else if (cmd.rightmove < -64.0)
                {
                    cmd.rightmove = -64;
                }
                */
            }

            max = (int)System.Math.Abs(cmd.ForwardMove);
            if ((int)System.Math.Abs(cmd.RightMove) > max)
            {
                max = (int)System.Math.Abs(cmd.RightMove);
            }
            if ((int)System.Math.Abs(cmd.UpMove) > max)
            {
                max = (int)System.Math.Abs(cmd.UpMove);
            }
            if (max <= 0)
            {
                return 0;
            }

            total = (float)System.Math.Abs(cmd.ForwardMove * cmd.ForwardMove
                          + cmd.RightMove * cmd.RightMove + cmd.UpMove * cmd.UpMove);
#if WINDOWS_PHONE
            scale = (float)(/*pm.speed*/ 680 * max / (127.0 * total));
#else
            scale = (float)(/*pm.speed*/ 340 * max / (127.0 * total));
#endif
        //    if ((pm.cmd.buttons & usercmd_t.BUTTON_SPRINT) != 0 && pm.sprintTime > 50)
        //    {
         //       scale *= 1.2f;
          //  }
           // else
           // {
                scale *= 1.8f;
           // }

            return scale;
        }

        //
        // PM_Move
        //
        private void PM_Move(ref entityState_t ps, bool gravity)
        {
            ps.origin = ps.origin + velocity;
        }

        //
        // WalkPhysics
        //
        private void WalkPhysics(ref entityState_t ps, idUsercmd cmd)
        {
            idMatrix movematrix;
            idVector3 wishvel = new idVector3();
            float scale;

            movematrix = ps.angles2.ToAxis();

            // Apply friction.
            Physics_Friction();

            scale = PM_CmdScale(cmd, false);

            for (int i = 0; i < 3; i++)
            {
                wishvel[i] = movematrix[0][i] * cmd.ForwardMove + movematrix[1][i] * cmd.RightMove;
            }

            wishvel.Normalize();
            velocity = wishvel * scale;

            //PM_Accelerate(ref pm, wishvel, wishspeed, pm_accelerate);

            PM_Move(ref ps, false);
        }

        //
        // Move
        //
        public void Move(ref idPhysicsPlayerState pmove)
        {
            velocity[0] = pmove.cmd.ForwardMove;
            velocity[1] = pmove.cmd.RightMove;
            velocity[2] = pmove.cmd.UpMove;

            // Update the players view angles.
            UpdateViewAngles(ref pmove.ps, pmove.cmd);

            WalkPhysics(ref pmove.ps, pmove.cmd);
        }
    }
}
