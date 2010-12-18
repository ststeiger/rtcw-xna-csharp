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

// Plane.cs (c) 2010 id Software
//

using System;

namespace idLib.Math
{
    //
    // idPlane
    //
    public class idPlane
    {
        public const int PLANE_X         = 0;
        public const int PLANE_Y         = 1;
        public const int PLANE_Z         = 2;
        public const int PLANE_NON_AXIAL = 3;

        public idVector3 Normal = new idVector3();
        public float Dist;
        public int Type;
        public int SignBits = 0;

        public const int PLANESIDE_FRONT = 0;
        public const int PLANESIDE_BACK = 1;
        public const int PLANESIDE_ON = 2;
        public const int PLANESIDE_CROSS = 3;

        public void FitThroughPoint( idVector3 p ) {
	        Dist = -( Normal * p );
        }

        public float Distance(idVector3 v ) {
            return Normal.X * v.X + Normal.Y * v.Y + Normal.Z * v.Z + Dist;
        }

        public void SetPlaneSignbits() {
	        int bits, j;

	        // for fast box on planeside test
	        bits = 0;
	        for ( j = 0 ; j < 3 ; j++ ) {
		        if ( Normal[j] < 0 ) {
			        bits |= 1 << j;
		        }
	        }
            SignBits = bits;
        }

        public void SetPlaneType()
        {
            Type = (Normal[0] == 1.0 ? PLANE_X : (Normal[1] == 1.0 ? PLANE_Y : (Normal[2] == 1.0 ? PLANE_Z : PLANE_NON_AXIAL)));
        }
    }
}
