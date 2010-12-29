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

// MDS_math.cs (c) 2010 JV Software
//

using idLib.Math;

namespace rtcw.Renderer.Models
{
    //
    // SkeletalMath
    // This is wrong place to put this, todo deprecate me.
    //
    static class SkeletalMath
    {
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

        public static void LocalMatrixTransformVector( idVector3 invec, idMatrix mat, ref idVector3 outvec ) {
	        outvec[ 0 ] = invec[ 0 ] * mat[ 0 ][ 0 ] + invec[ 1 ] * mat[ 0 ][ 1 ] + invec[ 2 ] * mat[ 0 ][ 2 ];
	        outvec[ 1 ] = invec[ 0 ] * mat[ 1 ][ 0 ] + invec[ 1 ] * mat[ 1 ][ 1 ] + invec[ 2 ] * mat[ 1 ][ 2 ];
	        outvec[ 2 ] = invec[ 0 ] * mat[ 2 ][ 0 ] + invec[ 1 ] * mat[ 2 ][ 1 ] + invec[ 2 ] * mat[ 2 ][ 2 ];
        }

        public static void LocalMatrixTransformVectorTranslate(idVector3 invec, idMatrix mat, idVector3 tr, ref idVector3 outvec ) {
	        outvec[ 0 ] = invec[ 0 ] * mat[ 0 ][ 0 ] + invec[ 1 ] * mat[ 0 ][ 1 ] + invec[ 2 ] * mat[ 0 ][ 2 ] + tr[ 0 ];
	        outvec[ 1 ] = invec[ 0 ] * mat[ 1 ][ 0 ] + invec[ 1 ] * mat[ 1 ][ 1 ] + invec[ 2 ] * mat[ 1 ][ 2 ] + tr[ 1 ];
	        outvec[ 2 ] = invec[ 0 ] * mat[ 2 ][ 0 ] + invec[ 1 ] * mat[ 2 ][ 1 ] + invec[ 2 ] * mat[ 2 ][ 2 ] + tr[ 2 ];
        }

        public static void LocalScaledMatrixTransformVector( idVector3 invec, float s, idMatrix mat, ref idVector3 outvec ) {
	        outvec[ 0 ] = ( 1.0f - s ) * invec[ 0 ] + s * ( invec[ 0 ] * mat[ 0 ][ 0 ] + invec[ 1 ] * mat[ 0 ][ 1 ] + invec[ 2 ] * mat[ 0 ][ 2 ] );
	        outvec[ 1 ] = ( 1.0f - s ) * invec[ 1 ] + s * ( invec[ 0 ] * mat[ 1 ][ 0 ] + invec[ 1 ] * mat[ 1 ][ 1 ] + invec[ 2 ] * mat[ 1 ][ 2 ] );
	        outvec[ 2 ] = ( 1.0f - s ) * invec[ 2 ] + s * ( invec[ 0 ] * mat[ 2 ][ 0 ] + invec[ 1 ] * mat[ 2 ][ 1 ] + invec[ 2 ] * mat[ 2 ][ 2 ] );
        }

        public static void LocalScaledMatrixTransformVectorTranslate( idVector3 invec, float s, idMatrix mat, idVector3 tr, ref idVector3 outvec ) {
	        outvec[ 0 ] = ( 1.0f - s ) * invec[ 0 ] + s * ( invec[ 0 ] * mat[ 0 ][ 0 ] + invec[ 1 ] * mat[ 0 ][ 1 ] + invec[ 2 ] * mat[ 0 ][ 2 ] + tr[ 0 ] );
	        outvec[ 1 ] = ( 1.0f - s ) * invec[ 1 ] + s * ( invec[ 0 ] * mat[ 1 ][ 0 ] + invec[ 1 ] * mat[ 1 ][ 1 ] + invec[ 2 ] * mat[ 1 ][ 2 ] + tr[ 1 ] );
	        outvec[ 2 ] = ( 1.0f - s ) * invec[ 2 ] + s * ( invec[ 0 ] * mat[ 2 ][ 0 ] + invec[ 1 ] * mat[ 2 ][ 1 ] + invec[ 2 ] * mat[ 2 ][ 2 ] + tr[ 2 ] );
        }

        public static void LocalScaledMatrixTransformVectorFullTranslate( idVector3 invec, float s, idMatrix mat, idVector3 tr, ref idVector3 outvec ) {
	        outvec[ 0 ] = ( 1.0f - s ) * invec[ 0 ] + s * ( invec[ 0 ] * mat[ 0 ][ 0 ] + invec[ 1 ] * mat[ 0 ][ 1 ] + invec[ 2 ] * mat[ 0 ][ 2 ] ) + tr[ 0 ];
	        outvec[ 1 ] = ( 1.0f - s ) * invec[ 1 ] + s * ( invec[ 0 ] * mat[ 1 ][ 0 ] + invec[ 1 ] * mat[ 1 ][ 1 ] + invec[ 2 ] * mat[ 1 ][ 2 ] ) + tr[ 1 ];
	        outvec[ 2 ] = ( 1.0f - s ) * invec[ 2 ] + s * ( invec[ 0 ] * mat[ 2 ][ 0 ] + invec[ 1 ] * mat[ 2 ][ 1 ] + invec[ 2 ] * mat[ 2 ][ 2 ] ) + tr[ 2 ];
        }

        public static void LocalAddScaledMatrixTransformVectorFullTranslate( idVector3 invec, float s, idMatrix mat, idVector3 tr, ref idVector3 outvec ) {
	        outvec[ 0 ] += s * ( invec[ 0 ] * mat[ 0 ][ 0 ] + invec[ 1 ] * mat[ 0 ][ 1 ] + invec[ 2 ] * mat[ 0 ][ 2 ] ) + tr[ 0 ];
	        outvec[ 1 ] += s * ( invec[ 0 ] * mat[ 1 ][ 0 ] + invec[ 1 ] * mat[ 1 ][ 1 ] + invec[ 2 ] * mat[ 1 ][ 2 ] ) + tr[ 1 ];
	        outvec[ 2 ] += s * ( invec[ 0 ] * mat[ 2 ][ 0 ] + invec[ 1 ] * mat[ 2 ][ 1 ] + invec[ 2 ] * mat[ 2 ][ 2 ] ) + tr[ 2 ];
        }

        private static idVector3 outvecTranslated = new idVector3();
        public static idVector3 LocalAddScaledMatrixTransformVectorTranslate( idVector3 invec, float s, idMatrix mat ) {
            outvecTranslated[0] = s * (invec[0] * mat[0][0] + invec[1] * mat[0][1] + invec[2] * mat[0][2] + mat.M41);
            outvecTranslated[1] = s * (invec[0] * mat[1][0] + invec[1] * mat[1][1] + invec[2] * mat[1][2] + mat.M42);
            outvecTranslated[2] = s * (invec[0] * mat[2][0] + invec[1] * mat[2][1] + invec[2] * mat[2][2] + mat.M43);

            return outvecTranslated;
        }

        public static void LocalAddScaledMatrixTransformVector( idVector3 invec, float s, idMatrix mat, ref idVector3 outvec  ) {
	        outvec[ 0 ] += s * ( invec[ 0 ] * mat[ 0 ][ 0 ] + invec[ 1 ] * mat[ 0 ][ 1 ] + invec[ 2 ] * mat[ 0 ][ 2 ] );
	        outvec[ 1 ] += s * ( invec[ 0 ] * mat[ 1 ][ 0 ] + invec[ 1 ] * mat[ 1 ][ 1 ] + invec[ 2 ] * mat[ 1 ][ 2 ] );
	        outvec[ 2 ] += s * ( invec[ 0 ] * mat[ 2 ][ 0 ] + invec[ 1 ] * mat[ 2 ][ 1 ] + invec[ 2 ] * mat[ 2 ][ 2 ] );
        }

        public static float LAVangle;
        //static float		sr; // TTimo: unused
        public static float sp, sy;
        //static float    cr; // TTimo: unused
        public static float cp, cy;

        public static void LocalAngleVector( idVector3 angles, ref idVector3 forward ) {
	        LAVangle = angles[1] * ( (float)(System.Math.PI) * 2 / 360 );
	        sy = (float)System.Math.Sin( LAVangle );
	        cy = (float)System.Math.Cos( LAVangle );
	        LAVangle = angles[0] * ( (float)(System.Math.PI) * 2 / 360 );
	        sp = (float)System.Math.Sin( LAVangle );
	        cp = (float)System.Math.Cos( LAVangle );

	        forward[0] = cp * cy;
	        forward[1] = cp * sy;
	        forward[2] = -sp;
        }

        public static void LocalVectorMA( idVector3 org, float dist, idVector3 vec, ref idVector3 outvec ) {
	        outvec[0] = org[0] + dist * vec[0];
	        outvec[1] = org[1] + dist * vec[1];
	        outvec[2] = org[2] + dist * vec[2];
        }

        public static float SHORT2ANGLE( short x )  {
            return ( ( x ) * ( 360.0f / 65536.0f ) );
        }

        public static void ANGLES_SHORT_TO_FLOAT(ref idVector3 pf, short[] sh )
        {
            pf[0] = SHORT2ANGLE(sh[0]);
            pf[1] = SHORT2ANGLE(sh[1]);
            pf[2] = SHORT2ANGLE(sh[2]);
        }

        public static void SLerp_Normal( idVector3 from, idVector3 to, float tt, ref idVector3 outvec ) {
	        float ft = 1.0f - tt;

	        outvec[0] = from[0] * ft + to[0] * tt;
	        outvec[1] = from[1] * ft + to[1] * tt;
	        outvec[2] = from[2] * ft + to[2] * tt;

	        outvec.Normalize();
        }
    }
}
