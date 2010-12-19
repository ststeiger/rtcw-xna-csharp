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

// bounds.cs (c) 2010 id Software
//

using idLib.Math;

namespace idLib
{
    //
    // idBounds
    //
    public class idBounds
    {
        idVector3 mins;
        idVector3 maxs;

        //
        // Mins
        //
        public idVector3 Mins
        {
            get
            {
                return mins;
            }
        }

        //
        // Maxs
        //
        public idVector3 Maxs
        {
            get
            {
                return maxs;
            }
        }

        //
        // idBounds
        //
        public idBounds()
        {
            mins = new idVector3();
            maxs = new idVector3();

            ClearBounds();
        }

        //
        // idBounds
        //
        public idBounds(idVector3 mins, idVector3 maxs)
        {
            this.mins = mins;
            this.maxs = maxs;
        }

        //
        // AddPointToBounds
        //
        public void AddPointToBounds( idVector3 v) {
	        if ( v[0] < mins[0] ) {
		        mins[0] = v[0];
	        }
	        if ( v[0] > maxs[0] ) {
		        maxs[0] = v[0];
	        }

	        if ( v[1] < mins[1] ) {
		        mins[1] = v[1];
	        }
	        if ( v[1] > maxs[1] ) {
		        maxs[1] = v[1];
	        }

	        if ( v[2] < mins[2] ) {
		        mins[2] = v[2];
	        }
	        if ( v[2] > maxs[2] ) {
		        maxs[2] = v[2];
	        }
        }

        //
        // ClearBounds
        //
        public void ClearBounds()
        {
            mins[0] = mins[1] = mins[2] = 99999;
            maxs[0] = maxs[1] = maxs[2] = -99999;
        }
    }
}
