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

// trajectory.cs (c) 2010 JV Software 
using idLib.Math;
using idLib.Engine.Public.Net;

namespace idLib.Game
{
    //
    // trType_t
    //
    public enum trType_t {
	    TR_STATIONARY,
	    TR_INTERPOLATE,             // non-parametric, but interpolate between snapshots
	    TR_LINEAR,
	    TR_LINEAR_STOP,
	    TR_LINEAR_STOP_BACK,        //----(SA)	added.  so reverse movement can be different than forward
	    TR_SINE,                    // value = base + sin( time / duration ) * delta
	    TR_GRAVITY,
	    // Ridah
	    TR_GRAVITY_LOW,
	    TR_GRAVITY_FLOAT,           // super low grav with no gravity acceleration (floating feathers/fabric/leaves/...)
	    TR_GRAVITY_PAUSED,          //----(SA)	has stopped, but will still do a short trace to see if it should be switched back to TR_GRAVITY
	    TR_ACCELERATE,
	    TR_DECCELERATE
    };

    public class trajectory_t
    {
	    public trType_t trType;
        public int trTime;
        public int trDuration;             // if non 0, trTime + trDuration = stop time
    //----(SA)	removed
	    public idVector3 trBase = new idVector3();
        public idVector3 trDelta = new idVector3();             // velocity, etc
    //----(SA)	removed

        public const int NET_SIZE = (idVector3.Size * 2) + (sizeof(int) * 2) + 1;

        //
        // WritePacket
        //
        public void WritePacket(ref idMsgWriter msg)
        {
            msg.WriteByte((byte)trType);
            msg.WriteInt(trTime);
            msg.WriteInt(trDuration);
            msg.WriteVector3(ref trBase);
            msg.WriteVector3(ref trDelta);
        }

        //
        // ReadPacket
        //
        public void ReadPacket(ref idMsgReader msg)
        {
            trType = (trType_t)msg.ReadByte();
            trTime = msg.ReadInt();
            trDuration = msg.ReadInt();
            msg.ReadVector3(ref trBase);
            msg.ReadVector3(ref trDelta);
        }
    };
}
