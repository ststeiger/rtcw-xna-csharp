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

// entity_state.cs (c) 2010 JV Software 
using idLib.Math;
using idLib.Game.AI;
using idLib.Engine.Public.Net;

namespace idLib.Game
{
    // entityState_t is the information conveyed from the server
    // in an update message about entities that the client will
    // need to render in some way
    // Different eTypes may use the information in different ways
    // The messages are delta compressed, so it doesn't really matter if
    // the structure size is fairly large
    //
    // NOTE: all fields in here must be 32 bits (or those within sub-structures)

    public class entityState_t {
        //
        // ReadPacket
        //
        public void ReadPacket(ref idMsgReader msg)
        {
            number = msg.ReadInt();
            eType = (entityType_t)msg.ReadByte();
            eFlags = msg.ReadInt();
            pos.ReadPacket(ref msg);
            apos.ReadPacket(ref msg);
            time = msg.ReadInt();
            time2 = msg.ReadInt();

            msg.ReadVector3(ref origin);
            msg.ReadVector3(ref origin2);
            msg.ReadVector3(ref angles);
            msg.ReadVector3(ref angles2);

            otherEntityNum = msg.ReadInt();
            otherEntityNum2 = msg.ReadInt();

            groundEntityNum = msg.ReadInt();

            constantLight = msg.ReadInt();
            dl_intensity = msg.ReadInt();
            loopSound = msg.ReadInt();

            modelindex = msg.ReadInt();
            modelindex2 = msg.ReadInt();
            clientNum = msg.ReadInt();
            frame = msg.ReadInt();

            solid = msg.ReadInt();
            eventnum = msg.ReadInt();
            eventParm = msg.ReadInt();

            eventSequence = msg.ReadInt();

            powerups = msg.ReadInt();
            weapon = msg.ReadInt();
            legsAnim = msg.ReadInt();
            torsoAnim = msg.ReadInt();

            density = msg.ReadInt();
            dmgFlags = msg.ReadInt();

            onFireStart = msg.ReadInt();
            onFireEnd = msg.ReadInt();

            aiChar = msg.ReadInt();
            teamNum = msg.ReadInt();

            effect1Time = msg.ReadInt();
            effect2Time = msg.ReadInt();
            effect3Time = msg.ReadInt();

            aiState = (aistateEnum_t)msg.ReadByte();
            animMovetype = msg.ReadInt();
        }

        //
        // WritePacket
        //
        public void WritePacket(ref idMsgWriter msg)
        {
            msg.WriteInt(number);
            msg.WriteByte((byte)eType);
            msg.WriteInt(eFlags);
            pos.WritePacket(ref msg);
            apos.WritePacket(ref msg);
            msg.WriteInt(time);
            msg.WriteInt(time2);
            msg.WriteVector3(ref origin);
            msg.WriteVector3(ref origin2);
            msg.WriteVector3(ref angles);
            msg.WriteVector3(ref angles2);

            msg.WriteInt(otherEntityNum); 
	        msg.WriteInt(otherEntityNum2);

	        msg.WriteInt(groundEntityNum);  

	        msg.WriteInt(constantLight); 
	        msg.WriteInt(dl_intensity);  
	        msg.WriteInt(loopSound);      

	        msg.WriteInt(modelindex);
	        msg.WriteInt(modelindex2);
	        msg.WriteInt(clientNum); 
	        msg.WriteInt(frame);

	        msg.WriteInt(solid);   
	        msg.WriteInt(eventnum);
	        msg.WriteInt(eventParm);

	        msg.WriteInt(eventSequence); 

            msg.WriteInt(powerups);  
            msg.WriteInt(weapon);      
            msg.WriteInt(legsAnim); 
            msg.WriteInt(torsoAnim); 

            msg.WriteInt(density); 

            msg.WriteInt(dmgFlags);    

            msg.WriteInt(onFireStart);
            msg.WriteInt(onFireEnd);

            msg.WriteInt(aiChar);
            msg.WriteInt(teamNum);

            msg.WriteInt(effect1Time);
            msg.WriteInt(effect2Time);
            msg.WriteInt(effect3Time);

	        msg.WriteByte((byte)aiState);

            msg.WriteInt(animMovetype);  
        }

        public const int NET_SIZE = (sizeof(int) * 32) + 2 + (trajectory_t.NET_SIZE * 2) + (idVector3.Size * 4);

	    public int number;             // entity index
        public entityType_t eType;              // entityType_t
	    public int eFlags;

	    public trajectory_t pos = new trajectory_t();       // for calculating position
	    public trajectory_t apos = new trajectory_t();      // for calculating angles

	    public int time;
	    public int time2;

	    public idVector3 origin = new idVector3();
	    public idVector3 origin2 = new idVector3();

	    public idVector3 angles = new idVector3();
	    public idVector3 angles2 = new idVector3();

	    public int otherEntityNum;     // shotgun sources, etc
	    public int otherEntityNum2;

	    public int groundEntityNum;        // -1 = in air

	    public int constantLight;      // r + (g<<8) + (b<<16) + (intensity<<24)
	    public int dl_intensity;       // used for coronas
	    public int loopSound;          // constantly loop this sound

	    public int modelindex;
	    public int modelindex2;
	    public int clientNum;          // 0 to (MAX_CLIENTS - 1), for players and corpses
	    public int frame;

	    public int solid;              // for client side prediction, trap_linkentity sets this properly

	    // old style events, in for compatibility only
	    public int eventnum;
	    public int eventParm;

	    public int eventSequence;      // pmove generated events
	    //public int events[MAX_EVENTS];
	    //public int eventParms[MAX_EVENTS];

	    // for players
        public int powerups;           // bit flags
        public int weapon;             // determines weapon and flash model, etc
        public int legsAnim;           // mask off ANIM_TOGGLEBIT
        public int torsoAnim;          // mask off ANIM_TOGGLEBIT
    //	int		weapAnim;		// mask off ANIM_TOGGLEBIT	//----(SA)	removed (weap anims will be client-side only)

        public int density;            // for particle effects

        public int dmgFlags;           // to pass along additional information for damage effects for players/ Also used for cursorhints for non-player entities

	    // Ridah
        public int onFireStart, onFireEnd;

        public int aiChar, teamNum;

        public int effect1Time, effect2Time, effect3Time;

	    aistateEnum_t aiState;

        public int animMovetype;       // clients can't derive movetype of other clients for anim scripting system
    };
}
