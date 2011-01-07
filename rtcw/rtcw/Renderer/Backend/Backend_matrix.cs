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

// Backend_matrix.cs (c) 2010 JV Software
//

using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using idLib.Engine.Public;

using rtcw.Renderer.Effects;

namespace rtcw.Renderer.Backend
{
    //
    // idRenderMatrix
    //
    class idRenderMatrix
    {
        public Matrix world = Matrix.Identity;
        public Matrix view;
        public Matrix projection;

        private Matrix s_flipMatrix = new Matrix(
            // convert from our coordinate system (looking down X)
            // to OpenGL's coordinate system (looking down -Z)
            0, 0, -1, 0,
            -1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 0, 1
        );

        //
        // Create2DOrthoMatrix
        // http://social.msdn.microsoft.com/Forums/en/xnagamestudioexpress/thread/7a2afd75-49f9-485d-99bd-1ed61dfbb1b4
        //
        public void Create2DOrthoMatrix( int width, int height )
        {
            world = Matrix.Identity;
            view = new Matrix(
                1.0f, 0.0f, 0.0f, 0.0f,
                0.0f, -1.0f, 0.0f, 0.0f,
                0.0f, 0.0f, -1.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f);

            projection = Matrix.CreateOrthographicOffCenter(0, width, -height, 0, 0, 1);
            
        }

        /*
       ===============
       R_SetupProjection
       ===============
       */
        public void SetupProjection(idRefdefLocal view, float zFar, float zNear)
        {
            float ymax = 0.5f * (float)Math.Tan(view.fov_y * Math.PI / 360.0f);
            float ymin = -ymax;

            float xmax = 0.5f * (float)Math.Tan(view.fov_x * Math.PI / 360.0f);
            float xmin = -xmax;

            float width = xmax - xmin;
            float height = ymax - ymin;
            float depth = zFar - zNear;

            projection.M11 = 2 * zNear / width;
            projection.M21 = 0;
            projection.M31 = (xmax + xmin) / width;	// normally 0
            projection.M41 = 0;

            projection.M12 = 0;
            projection.M22 = 2 * zNear / height;
            projection.M32 = (ymax + ymin) / height;	// normally 0
            projection.M42 = 0;

            projection.M13 = 0;
            projection.M23 = 0;
            projection.M33 = -(zFar + zNear) / depth;
            projection.M43 = -2 * zFar * zNear / depth;

            projection.M14 = 0;
            projection.M24 = 0;
            projection.M34 = -1;
            projection.M44 = 0;

            // D3D: Transpose our projection matrix.
            projection = Matrix.Transpose(projection);
        }

        //
        // SetEntityMatrix
        //
        public void SetEntityMatrix(idRenderEntityLocal refEnt)
        {
            world.M11 = refEnt.axis[0][0];
            world.M21 = refEnt.axis[1][0];
            world.M31 = refEnt.axis[2][0];


            world.M12 = refEnt.axis[0][1];
            world.M22 = refEnt.axis[1][1];
            world.M32 = refEnt.axis[2][1];


            world.M13 = refEnt.axis[0][2];
            world.M23 = refEnt.axis[1][2];
            world.M33 = refEnt.axis[2][2];

            world.M14 = 0;
            world.M24 = 0;
            world.M34 = 0;
            world.M44 = 1;

         //   world = Matrix.Transpose(world);

            world.M41 = refEnt.origin.X;
            world.M42 = refEnt.origin.Y;
            world.M43 = refEnt.origin.Z;
        }

        /*
       =============
       R_CreateViewMatrix

       D3D View Matrix:
        xaxis.x           yaxis.x           zaxis.x          0
        xaxis.y           yaxis.y           zaxis.y          0
        xaxis.z           yaxis.z           zaxis.z          0
       -dot(xaxis, eye)  -dot(yaxis, eye)  -dot(zaxis, eye)  1
       =============
       */
        public void CreateViewMatrix(idRefdefLocal view)
        {
            this.view = new Matrix();
            this.view.M11 = view.viewaxis[0][0];
            this.view.M21 = view.viewaxis[0][1];
            this.view.M31 = view.viewaxis[0][2];

            this.view.M12 = view.viewaxis[1][0];
            this.view.M22 = view.viewaxis[1][1];
            this.view.M32 = view.viewaxis[1][2];

            this.view.M13 = view.viewaxis[2][0];
            this.view.M23 = view.viewaxis[2][1];
            this.view.M33 = view.viewaxis[2][2];

            this.view.M14 = 0;
            this.view.M24 = 0;
            this.view.M34 = 0;
            this.view.M44 = 1;

            //view.vieworg[1] = -0.09f;
            float v1 = -view.vieworg[0] * this.view.M11 + -view.vieworg[1] * this.view.M21 + -view.vieworg[2] * this.view.M31;
            float v2 = -view.vieworg[0] * this.view.M12 + -view.vieworg[1] * this.view.M22 + -view.vieworg[2] * this.view.M32;
            float v3 = -view.vieworg[0] * this.view.M13 + -view.vieworg[1] * this.view.M23 + -view.vieworg[2] * this.view.M33;
           // this.view = Matrix.Transpose(this.view);

            this.view.M41 = v1; // v1;
            this.view.M42 = v2; // v2;
            this.view.M43 = v3; // v3;
            this.view *= s_flipMatrix;
        }

        //
        // SetAsActiveMatrix
        //
        public void SetAsActiveMatrix(ref BasicEffect effect)
        {
            effect.World = world;
            effect.Projection = projection;
            effect.View = view;
        }

        //
        // SetAsActiveMatrix
        //
        public void SetAsActiveMatrix(ref AlphaTestEffect effect)
        {
            effect.World = world;
            effect.Projection = projection;
            effect.View = view;
        }

        //
        // SetAsActiveMatrix
        //
        public void SetAsActiveMatrix(ref idSkeletalEffect effect)
        {
            effect.World = world;
            effect.Projection = projection;
            effect.View = view;
        }

        //
        // SetAsActiveMatrix
        //
        public void SetAsActiveMatrix(ref DualTextureEffect effect)
        {
            effect.World = world;
            effect.Projection = projection;
            effect.View = view;
        }
    };
}
