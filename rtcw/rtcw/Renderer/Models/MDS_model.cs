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

// MDS_Model.cs (c) 2010 JV Software
//

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;


using idLib.Engine.Public;
using idLib.Math;

namespace rtcw.Renderer.Models
{
    //
    // idModelMDS
    //
    public class idModelMDS : idModelLocal
    {
        public const int MDS_IDENT         =  ( ( 'W' << 24 ) + ( 'S' << 16 ) + ( 'D' << 8 ) + 'M' );
        public const int MDS_VERSION       =  4;
        public const int MDS_MAX_VERTS     =  6000;
        public const int MDS_MAX_TRIANGLES =  8192;
        public const int MDS_MAX_BONES    =   128;
        public const int MDS_MAX_SURFACES =   32;
        public const int MDS_MAX_TAGS     =   128;

        public const float MDS_TRANSLATION_SCALE =  ( 1.0f / 64 );
        public const int BONEFLAG_TAG     =   1;       // this bone is actually a tag

        //
        // idModelMDS
        //
        public idModelMDS(ref idFile f)
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
    }
}
