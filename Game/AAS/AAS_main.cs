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

             //   if (aasworlds[i].Init(mapname + "_b" + i) == false)
            //    {
                    Engine.common.Warning("AAS_Init: Failed to load %s AI will be disabled...\n", mapname + "_b" + i);
                    return;
          //      }
            }

            aasInit = true;
        }

        
    }

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
