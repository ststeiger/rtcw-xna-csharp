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

// entity_shared.cs (c) 2010 JV Software 

using idLib.Math;

namespace idLib.Game
{
    //
    // entityShared_t
    //
    public class entityShared_t
    {
	    public entityState_t s;                // communicated by server to clients

	    public bool linked;                // qfalse if not in any good cluster
        public int linkcount;

        public int svFlags;                    // SVF_NOCLIENT, SVF_BROADCAST, etc
        public int singleClient;               // only send to this client when SVF_SINGLECLIENT is set

        public bool bmodel;                // if false, assume an explicit mins / maxs bounding box
									    // only set by trap_SetBrushModel
        public idBounds bounds;
	    public int contents;                   // CONTENTS_TRIGGER, CONTENTS_SOLID, CONTENTS_BODY, etc
									    // a non-solid entity should set to 0

	    public idBounds absbounds;          // derived from mins/maxs and origin + rotation

	    // currentOrigin will be used for all collision detection and world linking.
	    // it will not necessarily be the same as the trajectory evaluation for the current
	    // time, because each entity must be moved one at a time after time is advanced
	    // to avoid simultanious collision issues
	    public idVector3 currentOrigin;
        public idVector3 currentAngles;

	    // when a trace call is made and passEntityNum != ENTITYNUM_NONE,
	    // an ent will be excluded from testing if:
	    // ent->s.number == passEntityNum	(don't interact with self)
	    // ent->s.ownerNum = passEntityNum	(don't interact with your own missiles)
	    // entity[ent->s.ownerNum].ownerNum = passEntityNum	(don't interact with other missiles from owner)
	    public int ownerNum;
        public int eventTime;
    };
}
