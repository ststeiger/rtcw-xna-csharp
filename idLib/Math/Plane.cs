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

        /*
        ==================
        BoxOnPlaneSide

        Returns 1, 2, or 1 + 2

        // this is the slow, general version
        int BoxOnPlaneSide2 (vec3_t emins, vec3_t emaxs, struct cplane_s *p)
        {
	        int		i;
	        float	dist1, dist2;
	        int		sides;
	        vec3_t	corners[2];

	        for (i=0 ; i<3 ; i++)
	        {
		        if (p->normal[i] < 0)
		        {
			        corners[0][i] = emins[i];
			        corners[1][i] = emaxs[i];
		        }
		        else
		        {
			        corners[1][i] = emins[i];
			        corners[0][i] = emaxs[i];
		        }
	        }
	        dist1 = DotProduct (p->normal, corners[0]) - p->dist;
	        dist2 = DotProduct (p->normal, corners[1]) - p->dist;
	        sides = 0;
	        if (dist1 >= 0)
		        sides = 1;
	        if (dist2 < 0)
		        sides |= 2;

	        return sides;
        }

        ==================
        */

        public int BoxOnPlaneSide(idVector3 emins, idVector3 emaxs)
        {
	        float dist1, dist2;
	        int sides;

        // fast axial cases
	        if ( Type < 3 ) {
		        if ( Dist <= emins[Type] ) {
			        return 1;
		        }
		        if ( Dist >= emaxs[Type] ) {
			        return 2;
		        }
		        return 3;
	        }

        // general case
            switch (SignBits)
	        {
	        case 0:
		        dist1 = Normal[0] * emaxs[0] + Normal[1] * emaxs[1] + Normal[2] * emaxs[2];
		        dist2 = Normal[0] * emins[0] + Normal[1] * emins[1] + Normal[2] * emins[2];
		        break;
	        case 1:
		        dist1 = Normal[0] * emins[0] + Normal[1] * emaxs[1] + Normal[2] * emaxs[2];
		        dist2 = Normal[0] * emaxs[0] + Normal[1] * emins[1] + Normal[2] * emins[2];
		        break;
	        case 2:
		        dist1 = Normal[0] * emaxs[0] + Normal[1] * emins[1] + Normal[2] * emaxs[2];
		        dist2 = Normal[0] * emins[0] + Normal[1] * emaxs[1] + Normal[2] * emins[2];
		        break;
	        case 3:
		        dist1 = Normal[0] * emins[0] + Normal[1] * emins[1] + Normal[2] * emaxs[2];
		        dist2 = Normal[0] * emaxs[0] + Normal[1] * emaxs[1] + Normal[2] * emins[2];
		        break;
	        case 4:
		        dist1 = Normal[0] * emaxs[0] + Normal[1] * emaxs[1] + Normal[2] * emins[2];
		        dist2 = Normal[0] * emins[0] + Normal[1] * emins[1] + Normal[2] * emaxs[2];
		        break;
	        case 5:
		        dist1 = Normal[0] * emins[0] + Normal[1] * emaxs[1] + Normal[2] * emins[2];
		        dist2 = Normal[0] * emaxs[0] + Normal[1] * emins[1] + Normal[2] * emaxs[2];
		        break;
	        case 6:
		        dist1 = Normal[0] * emaxs[0] + Normal[1] * emins[1] + Normal[2] * emins[2];
		        dist2 = Normal[0] * emins[0] + Normal[1] * emaxs[1] + Normal[2] * emaxs[2];
		        break;
	        case 7:
		        dist1 = Normal[0] * emins[0] + Normal[1] * emins[1] + Normal[2] * emins[2];
		        dist2 = Normal[0] * emaxs[0] + Normal[1] * emaxs[1] + Normal[2] * emaxs[2];
		        break;
	        default:
		        dist1 = dist2 = 0;      // shut up compiler
		        break;
	        }

	        sides = 0;
	        if ( dist1 >= Dist ) {
		        sides = 1;
	        }
	        if ( dist2 < Dist ) {
		        sides |= 2;
	        }

	        return sides;
        }

        public void SetPlaneType()
        {
            if (Normal.X == 1.0f)
            {
                Type = PLANE_X;
            }
            else if (Normal.Y == 1.0f)
            {
                Type = PLANE_Y;
            }
            else if (Normal.Z == 1.0f)
            {
                Type = PLANE_Z;
            }
            else
            {
                Type = PLANE_NON_AXIAL;
            }
        }
    }
}
