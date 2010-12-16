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

// Vector.cs (c) 2010 id Software
//

using System;
using Microsoft.Xna.Framework;

namespace idLib.Math
{
    //
    // idVector2
    //
    public struct idVector2
    {
        public float X;
        public float Y;

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    default:
                        throw new Exception("vector3 size of 3 only");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    default:
                        throw new Exception("vector3 size of 3 only");
                }
            }
        }
    }

    //
    // idVector3
    //
    public struct idVector3
    {
        public float X;
        public float Y;
        public float Z;

        public static idVector3 vector_origin = new idVector3(0, 0, 0);

        public idVector3( float x, float y, float z )
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public float this[int index]
        {
            get
            {
                switch(index)
                {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    case 2:
                        return Z;
                    default:
                        throw new Exception( "vector3 size of 3 only" );
                }
            }
            set
            {
                switch(index)
                {
                    case 0:
                        X = value;
                    break;
                    case 1:
                        Y = value;
                    break;
                    case 2:
                        Z = value;
                    break;
                    default:
                        throw new Exception( "vector3 size of 3 only" );
                }
            }
        }

        public idVector3 Cross( idVector3 a ) {
	        return new idVector3( Y * a.Z - Z * a.Y, Z * a.X - X * a.Z, X * a.Y - Y * a.X );
        }

        public float Length() {
	        return ( float )System.Math.Sqrt( X * X + Y * Y + Z * Z );
        }

        public float LengthSqr() {
	        return ( X * X + Y * Y + Z * Z );
        }

        float InvSqrt(float x)
        {
            float xhalf = 0.5f * x;
            int i = BitConverter.ToInt32(BitConverter.GetBytes(x), 0);
            i = 0x5f3759df - (i >> 1);
            x = BitConverter.ToSingle(BitConverter.GetBytes(i), 0);
            x = x * (1.5f - xhalf * x * x);
            return x;
        }

        public float Normalize()
        {
            float sqrLength, invLength;

	        sqrLength = X * X + Y * Y + Z * Z;
            invLength = InvSqrt(sqrLength);
	        X *= invLength;
	        Y *= invLength;
	        Z *= invLength;
	        return invLength * sqrLength;
        }

	    public static idVector3	operator-(idVector3 vec) 
        {
            return new idVector3( -vec.X, -vec.Y, -vec.Z );
        }
	    
	    public static float	operator*( idVector3 vec, idVector3 a ) 
        {
            return vec.X * a.X + vec.Y * a.Y + vec.Z * a.Z;
        }

	    public static idVector3	operator*( idVector3 vec, float a )
        {
            return new idVector3( vec.X * a, vec.Y * a, vec.Z * a );
        }

        public static bool operator ==(idVector3 vec, idVector3 vec2)
        {
            return vec.Equals(vec2);
        }

        public static bool operator !=(idVector3 vec, idVector3 vec2)
        {
            return !vec.Equals(vec2);
        }

        public static idVector3 operator -(idVector3 vec, int a)
        {
            return new idVector3(vec.X - a, vec.Y - a, vec.Z - a);
        }

        public static idVector3 operator *(float a, idVector3 vec)
        {
            return new idVector3(vec.X * a, vec.Y * a, vec.Z * a);
        }

        public static idVector3	operator/( idVector3 vec, float a )
        {
            return new idVector3( vec.X / a, vec.Y / a, vec.Z / a );
        }

        public static idVector3	operator+( idVector3 vec, idVector3 a )
        {
            return new idVector3( vec.X + a.X, vec.Y + a.Y, vec.Z + a.Z );
        }

        public static idVector3	operator-( idVector3 vec, idVector3 a )
        {
            return new idVector3( vec.X - a.X, vec.Y - a.Y, vec.Z - a.Z );
        }

        public override bool Equals(object o)
        {
            idVector3 vec = (idVector3)o;
            if (X != vec.X)
            {
                return false;
            }
            if (Y != vec.Y)
            {
                return false;
            }
            if (Z != vec.Z)
            {
                return false;
            }
            return true;
        }

        public idVector3 Cross( idVector3 a, idVector3 b ) {
	        X = a.Y * b.Z - a.Z * b.Y;
	        Y = a.Z * b.X - a.X * b.Z;
	        Z = a.X * b.Y - a.Y * b.X;

	        return this;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = 0;
                result = (result * 397) ^ (int)X;
                result = (result * 397) ^ (int)Y;
                result = (result * 397) ^ (int)Z;
                return result;
            }
        }

        public void NormalVectors( ref idVector3 left, ref idVector3 down ) {
	        float d;

	        d = X * X + Y * Y;
	        if ( d == 0 ) {
		        left[0] = 1;
		        left[1] = 0;
		        left[2] = 0;
	        } else {
		        d = InvSqrt( d );
		        left[0] = -Y * d;
		        left[1] = X * d;
		        left[2] = 0;
	        }
	        down = left.Cross( this );
        }

        //
        // ToAxis
        //
        public idMatrix ToAxis()
        {
            float angle;
            float sr, sp, sy, cr, cp, cy;

            angle = (float)(Y * (System.Math.PI * 2 / 360));
            sy = (float)System.Math.Sin((double)angle);
            cy = (float)System.Math.Cos((double)angle);
            angle = (float)(X * (System.Math.PI * 2 / 360));
            sp = (float)System.Math.Sin((double)angle);
            cp = (float)System.Math.Cos((double)angle);
            angle = (float)(Z * (System.Math.PI * 2 / 360));
            sr = (float)System.Math.Sin((double)angle);
            cr = (float)System.Math.Cos((double)angle);

            idMatrix axis = new idMatrix();

            // Forward.
            axis.M11 = cp * cy;
            axis.M12 = cp * sy;
            axis.M13 = -sp;
            axis.M14 = 0;

            // Right
            axis.M21 = -(-1 * sr * sp * cy + -1 * cr * -sy);
            axis.M22 = -(-1 * sr * sp * sy + -1 * cr * cy);
            axis.M23 = -(-1 * sr * cp);
            axis.M24 = 0;

            // Right
            axis.M31 = (cr * sp * cy + -sr * -sy);
            axis.M32 = (cr * sp * sy + -sr * cy);
            axis.M33 = cr * cp;
            axis.M34 = 0;

            axis.M44 = 1;

            return axis;
        }
    }

        //
    // idVector3
    //
    public struct idVector4
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public static idVector4 vector_origin = new idVector4(0, 0, 0, 0);

        public idVector4(float x, float y, float z, float w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    case 2:
                        return Z;
                    case 3:
                        return W;
                    default:
                        throw new Exception("vector4 size of 4 only");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    case 2:
                        Z = value;
                        break;
                    case 3:
                        W = value;
                        break;
                    default:
                        throw new Exception("vector4 size of 4 only");
                }
            }
        }
    }
}
