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

// MD3_Model.cs (c) 2010 JV Software
//

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;


using idLib.Engine.Public;
using idLib.Math;

namespace rtcw.Renderer.Models
{
    //
    // idModelMD3
    //
    public class idModelMD3 : idModelLocal
    {
        public const int MD3_IDENT         =  ( ( '3' << 24 ) + ( 'P' << 16 ) + ( 'D' << 8 ) + 'I' );
        public const int MD3_VERSION       =  15;

        // limits
        public const int MD3_MAX_LODS      =  3;
        public const int MD3_MAX_TRIANGLES =  8192;    // per surface
        public const int MD3_MAX_VERTS     =  4096;    // per surface
        public const int MD3_MAX_SHADERS   =  256 ;    // per surface
        public const int MD3_MAX_FRAMES    =  1024;    // per model
        public const int MD3_MAX_SURFACES  =  32  ;    // per model
        public const int MD3_MAX_TAGS      =  16  ;    // per frame

        // vertex scales
        public const float MD3_XYZ_SCALE   =    ( 1.0f / 64 );

        //
        // idModelMD3
        //
        public idModelMD3(ref idFile f)
        {
            name = f.GetFullFilePath();
        }

        //
        // GetName
        //
        public override string GetName()
        {
            return name;
        }

        //
        // GetNumFrames
        //
        public override int GetNumFrames()
        {
            return 0;
        }
    }
}
