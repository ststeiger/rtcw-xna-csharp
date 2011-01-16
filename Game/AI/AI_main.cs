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

// gamemodel.cs (c) 2010 JV Software
//

using idLib.Math;
using idLib.Engine.Public;

using Game.AAS;
using Game.AAS.Private;
using Game.Entities.Player;

namespace Game.AI
{
    //
    // idBot
    //
    public class idBot : idPlayer
    {
        internal idAIUsercmd ea = new idAIUsercmd();
        internal idAICastState cs = new idAICastState();
        internal aas_movestate_t movestate = new aas_movestate_t();
        internal aas_moveresult_t moveresult = new aas_moveresult_t();

        internal const int AICAST_TFL_DEFAULT = idAASTravelFlags.TFL_DEFAULT & ~(idAASTravelFlags.TFL_JUMPPAD | idAASTravelFlags.TFL_ROCKETJUMP | idAASTravelFlags.TFL_BFGJUMP | idAASTravelFlags.TFL_GRAPPLEHOOK | idAASTravelFlags.TFL_DOUBLEJUMP | idAASTravelFlags.TFL_RAMPJUMP | idAASTravelFlags.TFL_STRAFEJUMP | idAASTravelFlags.TFL_LAVA);

        private const int SCRIPT_REACHGOAL_DIST = 8;
        private const int SCRIPT_REACHCAST_DIST = 64;

        //
        // idBot
        //
        public idBot() : base( false )
        {

        }

        //
        // EA
        //
        public idAIUsercmd EA
        {
            get
            {
                return ea;
            }
        }

        //
        // Spawn
        //
        public override void Spawn()
        {
            movestate.origin = state.origin;
            movestate.presencetype = aas_presencetype.PRESENCE_NORMAL;
            movestate.entity = this;
            cs.entity = this;
            ea.Init();
            base.Spawn();
        }

        //
        // AI_PushChaseGoal
        //
        private void AI_PushChaseGoal(idEntity entity, float reachdist, bool slowApproach)
        {
            cs.travelflags = AICAST_TFL_DEFAULT;
            cs.followEntity = entity;
            cs.followDist = reachdist;
            cs.followIsGoto = false;
            cs.followSlowApproach = slowApproach;
            cs.aifunc = AIGoal_Chase;
        }

        //
        // AIGoal_Chase
        //
        private void AIGoal_Chase()
        {
            idAIGoal goal = new idAIGoal();

            // Get the area num of the goal entity.
            goal.origin = cs.followEntity.state.origin;
            goal.areanum = BotPointAreaNum(goal.origin);
            goal.mins = idVector3.vector_origin + -8;
            goal.maxs = idVector3.vector_origin + 8;

            Level.aas.move.BotMoveToGoal(ref moveresult, ref movestate, goal, cs.travelflags);
        }

        //
        // MoveToEntity
        //
        public bool MoveToEntity(idEntity entity)
        {
            // Check to see if we made it to our destination.
            if (state.origin.Distance(entity.state.origin) < SCRIPT_REACHGOAL_DIST)
            { 
                return true;
            }

            AI_PushChaseGoal(entity, SCRIPT_REACHCAST_DIST, true);

            return false;
        }

        //
        // BotPointAreaNum
        //
        private int[] _botpointareas = new int[Level.aas.AASWorld.aasfile.numareas];
        private const int BOTAREA_JIGGLE_DIST = 32;
        internal int BotPointAreaNum(idVector3 origin)
        {
            int areanum, numareas;
            idVector3 end, ofs;

            // If the bot is in a valid area just return the area number.
            areanum = Level.aas.PointAreaNum(origin);
            if (areanum != 0)
            {
                return areanum;
            }

            end = origin;
            end[2] += 10;
            numareas = Level.aas.TraceAreas(origin, end, ref _botpointareas, null, 1);
            if (numareas > 0)
            {
                return _botpointareas[0];
            }

            ofs = idVector3.vector_origin;

            // Ridah, jiggle them around to look for a fuzzy area, helps LARGE characters reach destinations that are against walls
            ofs[2] = 10;
            for (ofs[0] = -BOTAREA_JIGGLE_DIST; ofs[0] <= BOTAREA_JIGGLE_DIST; ofs[0] += BOTAREA_JIGGLE_DIST * 2)
            {
                for (ofs[1] = -BOTAREA_JIGGLE_DIST; ofs[1] <= BOTAREA_JIGGLE_DIST; ofs[1] += BOTAREA_JIGGLE_DIST * 2)
                {
                    end = origin + ofs;
                    numareas = Level.aas.TraceAreas(origin, end, ref _botpointareas, null, 1);
                    if (numareas > 0)
                    {
                        return _botpointareas[0];
                    }
                }
            }

            Engine.common.Warning("BotPointAreaNum: Failed to find area for origin. \n");

            return 0;
        }

        //
        // EnterWorld
        //
        public override void EnterWorld()
        {
            
        }

        //
        // Frame
        //
        public override void Frame()
        {
            if (state.modelindex < 0)
                return;

            if (cs.aifunc != null)
            {
                cs.aifunc();
            }

            physicsState.cmd = ea.GetNextBotCommand(cs);
            physicsState.ps = state;

            // Run the base player frame.
            PlayerFrame();

            LinkEntity();
        }

       
    }

    public class idBotActionFlags
    {
       public const int ACTION_ATTACK = 1;
       public const int ACTION_USE          =    2;
       public const int ACTION_RESPAWN      =    4;
       public const int ACTION_JUMP         =    8;
       public const int ACTION_MOVEUP       =    8;
       public const int ACTION_CROUCH       =    16;
       public const int ACTION_MOVEDOWN     =    16;
       public const int ACTION_MOVEFORWARD  =    32;
       public const int ACTION_MOVEBACK     =    64;
       public const int ACTION_MOVELEFT     =    128;
       public const int ACTION_MOVERIGHT    =    256;
       public const int ACTION_DELAYEDJUMP  =    512;
       public const int ACTION_TALK         =    1024;
       public const int ACTION_GESTURE      =    2048;
       public const int ACTION_WALK         =    4096;
       public const int ACTION_RELOAD       =    8192;
    }
}
