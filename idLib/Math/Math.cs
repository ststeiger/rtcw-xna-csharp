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

using System;
using Microsoft.Xna.Framework;

namespace idLib.Math
{
    public static class idMath
    {
        public const float PI = 3.14159265f;
        public const float M_DEG2RAD = PI / 180.0f;
        public const float CIRCLE_APPROXIMATION_LENGTH		=	64.0f;

        // angle indexes
        public const int PITCH = 0;       // up / down
        public const int YAW       =          1;       // left / right
        public const int ROLL      =          2;       // fall over

        public static float DEG2RAD(float f)
        {
            return f * M_DEG2RAD;
        }

        /*
        =================
        AngleNormalize360

        returns angle normalized to the range [0 <= angle < 360]
        =================
        */
        public static float AngleNormalize360(float angle)
        {
            return (360.0f / 65536) * ((int)(angle * (65536 / 360.0)) & 65535);
        }


        /*
        =================
        AngleNormalize180

        returns angle normalized to the range [-180 < angle <= 180]
        =================
        */
        public static float AngleNormalize180(float angle)
        {
            angle = AngleNormalize360(angle);
            if (angle > 180.0)
            {
                angle -= 360.0f;
            }
            return angle;
        }

        /*
        ==============
        AngleDifference
        ==============
        */
        public static float AngleDifference(float ang1, float ang2)
        {
            float diff;

            diff = ang1 - ang2;
            if (ang1 > ang2)
            {
                if (diff > 180.0)
                {
                    diff -= 360.0f;
                }
            }
            else
            {
                if (diff < -180.0)
                {
                    diff += 360.0f;
                }
            }
            return diff;
        }



        public static float DotProduct(idVector3 v1, idVector3 v2)
        {
            return ((v1[0] * v2[0]) + (v1[1] * v2[1]) + (v1[2] * v2[2]));
        }

        public static void SinCos( float a, ref float s, ref float c ) {
	        s = (float)System.Math.Sin( a );
            c = (float)System.Math.Cos(a);
        }
    }
}
