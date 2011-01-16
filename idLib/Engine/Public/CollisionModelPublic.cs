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

// CollisionModelPublic.cs (c) 2010 JV Software
//

using idLib.Math;

namespace idLib.Engine.Public
{
    // a trace is returned when a box is swept through the world
    public struct idTrace {
	    public bool allsolid;      // if true, plane is not valid
        public bool startsolid;    // if true, the initial point was in a solid area
	    public float fraction;         // time completed, 1.0 = didn't hit anything
	    public idVector3 endpos;          // final position
	    public idPlane plane;         // surface normal at impact, transformed to world space
	    public int surfaceFlags;           // surface hit
        public int contents;           // contents on other side of surface hit
        public int entityNum;          // entity the contacted sirface is a part of
        public int area;
        public int planenum;
        public int lastarea;

        public static idTrace defaultTrace = new idTrace();
    };

    //
    // idCollisionModel
    //
    public abstract class idCollisionModel
    {
        public abstract void BoxTrace(out idTrace results, idVector3 start, idVector3 end, idBounds bounds, int passEntityNum, int contentmask);
        public abstract string GetName();
    }

    //
    // idCollisionModelManager
    //
    public abstract class idCollisionModelManager
    {
        public abstract idCollisionModel LoadCollisonModelFromBsp(string mappath, object world);
    }
}
