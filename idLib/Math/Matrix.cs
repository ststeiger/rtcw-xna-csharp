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

// Matrix.cs (c) 2010 id Software
//

using System;
using Microsoft.Xna.Framework;


namespace idLib.Math
{
    //
    // idMatrix
    //
    public struct idMatrix
    {
        public static idMatrix IdenityMatrix = new idMatrix( 1, 0, 0, 0,
                                                             0, 1, 0, 0,
                                                             0, 0, 1, 0,
                                                             0, 0, 0, 1 );
        
        Matrix m;
        
        public idMatrix(Matrix m)
        {
            this.m = m;
        }

        public idMatrix(float m11, float m12, float m13, float m14, float m21, float m22, float m23, float m24, float m31, float m32, float m33, float m34, float m41, float m42, float m43, float m44)
        {
            m = new Matrix(m11, m12, m13, m14, m21, m22, m23, m24, m31, m32, m33, m34, m41, m42, m43, m44);
        }

        public bool IsRotated()
        {
            return m != IdenityMatrix.m;
        }

        public idMatrix Transpose()
        {
            return new idMatrix(Matrix.Transpose(m));
        }

        public void Set(Matrix m)
        {
            this.m = m;
        }

        /*
        ================
        MatrixMultiply
         
        jv - I promise this will go away i just needed something that wouldn't multiply the transform part of the 4x4 matrix.
        ================
        */
        public void AxisMultiply( idMatrix in1, idMatrix in2 ) 
        {
	        m.M11 = in1[0][0] * in2[0][0] + in1[0][1] * in2[1][0] +
				        in1[0][2] * in2[2][0];
	        m.M12 = in1[0][0] * in2[0][1] + in1[0][1] * in2[1][1] +
				        in1[0][2] * in2[2][1];
	        m.M13 = in1[0][0] * in2[0][2] + in1[0][1] * in2[1][2] +
				        in1[0][2] * in2[2][2];
	        m.M21 = in1[1][0] * in2[0][0] + in1[1][1] * in2[1][0] +
				        in1[1][2] * in2[2][0];
	        m.M22 = in1[1][0] * in2[0][1] + in1[1][1] * in2[1][1] +
				        in1[1][2] * in2[2][1];
	        m.M23 = in1[1][0] * in2[0][2] + in1[1][1] * in2[1][2] +
				        in1[1][2] * in2[2][2];
	        m.M31 = in1[2][0] * in2[0][0] + in1[2][1] * in2[1][0] +
				        in1[2][2] * in2[2][0];
	        m.M32 = in1[2][0] * in2[0][1] + in1[2][1] * in2[1][1] +
				        in1[2][2] * in2[2][1];
            m.M33 = in1[2][0] * in2[0][2] + in1[2][1] * in2[1][2] +
				        in1[2][2] * in2[2][2];
        }

        public static idVector3	operator*( idMatrix mat, idVector3 vec ) {
	        return new idVector3(
		            mat[ 0 ].X * vec.X + mat[ 1 ].X * vec.Y + mat[ 2 ].X * vec.Z,
		            mat[ 0 ].Y * vec.X + mat[ 1 ].Y * vec.Y + mat[ 2 ].Y * vec.Z,
		            mat[ 0 ].Z * vec.X + mat[ 1 ].Z * vec.Y + mat[ 2 ].Z * vec.Z );
        }

        public static idVector3 operator *(idVector3 vec, idMatrix mat)
        {
            return new idVector3(
                    mat[0].X * vec.X + mat[1].X * vec.Y + mat[2].X * vec.Z,
                    mat[0].Y * vec.X + mat[1].Y * vec.Y + mat[2].Y * vec.Z,
                    mat[0].Z * vec.X + mat[1].Z * vec.Y + mat[2].Z * vec.Z);
        }

        

        public void ScaleMatrixAxis(float scale)
        {
            float m41 = m.M41;
            float m42 = m.M42;
            float m43 = m.M43;
            float m44 = m.M44;

            m = m * scale;
            m.M33 += 1.0f - scale;

            m.M41 = m41;
            m.M42 = m42;
            m.M43 = m43;
            m.M44 = 1.0f;
        }

        public void SetFromTranslation(idVector3 vec)
        {
            m = Matrix.Identity;
            m.M41 = vec.X;
            m.M42 = vec.Y;
            m.M43 = vec.Z;
        }

        public void SetFromAxisAndTranslation(idMatrix axis, idVector3 translation)
        {
            SetFromTranslation(translation);

            m.M11 = axis.M11;
            m.M12 = axis.M12;
            m.M13 = axis.M13;
            m.M14 = axis.M14;

            m.M21 = axis.M21;
            m.M22 = axis.M22;
            m.M23 = axis.M23;
            m.M24 = axis.M24;

            m.M31 = axis.M31;
            m.M32 = axis.M32;
            m.M33 = axis.M33;
            m.M34 = axis.M34;
        }

        public void SetFromScaledAxisAndTranslation(idMatrix axis, float scale, idVector3 vec)
        {
            SetFromAxisAndTranslation(axis, vec);
            ScaleMatrixAxis(scale);
        }

        public void SetFromScale(float scale)
        {
            m = Matrix.Identity;
            m.M41 = scale;
            m.M42 = scale;
            m.M43 = scale;
        }

        public void TransformVector(idVector3 src, ref idVector3 dst)
        {
            idMatrix m = this;
            dst[0] = m[0][0] * src[0] + m[0][1] * src[1] + m[0][2] * src[2] + m[0][3];
            dst[1] = m[1][0] * src[0] + m[1][1] * src[1] + m[1][2] * src[2] + m[1][3];
            dst[2] = m[2][0] * src[0] + m[2][1] * src[1] + m[2][2] * src[2] + m[2][3];
        }

        //
        // SetValFromIndex
        //
        private void SetValFromIndex(int index, int index2, float val)
        {
            switch (index)
            {
                case 0:

                    break;

                case 1:

                    break;

                case 2:

                    break;

                case 3:

                    break;
            }
        }

        public float this[int index, int index2]
        {
            set
            {
                SetValFromIndex(index, index2, value);
            }
        }

        public idVector3 this[int index]
        {
            get
            {
                idVector3 indexvec = new idVector3();
                if (index == 0)
                {
                    indexvec.X = m.M11;
                    indexvec.Y = m.M12;
                    indexvec.Z = m.M13;
                }
                else if (index == 1)
                {
                    indexvec.X = m.M21;
                    indexvec.Y = m.M22;
                    indexvec.Z = m.M23;
                }
                else if (index == 2)
                {
                    indexvec.X = m.M31;
                    indexvec.Y = m.M32;
                    indexvec.Z = m.M33;
                }
                else
                {
                    throw new Exception( "matrix range invalid" );
                }

                return indexvec;
            }
        }

        //
        // Matrix
        //
        public static implicit operator Matrix(idMatrix matrix)
        {
            return matrix.m;
        }

        public void ToForward(ref idVector3 v)
        {
            v.X = m.M11;
            v.Y = m.M12;
            v.Z = m.M13;
        }

        public void SetForward(float v, float v1, float v2)
        {
            m.M11 = v;
            m.M12 = v1;
            m.M13 = v2;
        }

        public void ToRight(ref idVector3 v)
        {
            v.X = m.M21;
            v.Y = m.M22;
            v.Z = m.M23;
        }

        public void SetRight(float v, float v1, float v2)
        {
            m.M21 = v;
            m.M22 = v1;
            m.M23 = v2;
        }

        public void ToUp(ref idVector3 v)
        {
            v.X = m.M31;
            v.Y = m.M32;
            v.Z = m.M33;
        }

        public void SetUp(float v, float v1, float v2)
        {
            m.M31 = v;
            m.M32 = v1;
            m.M33 = v2;
        }

        public float M11
        {
            get
            {
                return m.M11;
            }
            set
            {
                m.M11 = value;
            }
        }

        public float M12
        {
            get
            {
                return m.M12;
            }
            set
            {
                m.M12 = value;
            }
        }

        public float M13
        {
            get
            {
                return m.M13;
            }
            set
            {
                m.M13 = value;
            }
        }

        public float M14
        {
            get
            {
                return m.M14;
            }
            set
            {
                m.M14 = value;
            }
        }


        public float M21
        {
            get
            {
                return m.M21;
            }
            set
            {
                m.M21 = value;
            }
        }


        public float M22
        {
            get
            {
                return m.M22;
            }
            set
            {
                m.M22 = value;
            }
        }

        public float M23
        {
            get
            {
                return m.M23;
            }
            set
            {
                m.M23 = value;
            }
        }

        public float M24
        {
            get
            {
                return m.M24;
            }
            set
            {
                m.M24 = value;
            }
        }

        public float M31
        {
            get
            {
                return m.M31;
            }
            set
            {
                m.M31 = value;
            }
        }


        public float M32
        {
            get
            {
                return m.M32;
            }
            set
            {
                m.M32 = value;
            }
        }

        public float M33
        {
            get
            {
                return m.M33;
            }
            set
            {
                m.M33 = value;
            }
        }

        public float M34
        {
            get
            {
                return m.M34;
            }
            set
            {
                m.M34 = value;
            }
        }

        public float M41
        {
            get
            {
                return m.M41;
            }
            set
            {
                m.M41 = value;
            }
        }

        public float M42
        {
            get
            {
                return m.M42;
            }
            set
            {
                m.M42 = value;
            }
        }


        public float M43
        {
            get
            {
                return m.M43;
            }
            set
            {
                m.M43 = value;
            }
        }

        public float M44
        {
            get
            {
                return m.M44;
            }
            set
            {
                m.M44 = value;
            }
        }
    }
}
