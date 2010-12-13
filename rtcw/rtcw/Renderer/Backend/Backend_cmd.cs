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

// Backend_Cmd.cs (c) 2010 JV Software
//

using System;
using idLib.Engine.Public;

namespace rtcw.Renderer.Backend
{
    //
    // renderCommandType
    // 
    public enum renderCommandType {
	    RC_SET_COLOR,
        RC_STRETCH_IMAGE,
	    RC_STRETCH_PIC,
	    RC_STRETCH_PIC_GRADIENT,    // (SA) added
	    RC_DRAW_SURFS,
	    RC_DRAW_BUFFER,
	    RC_SWAP_BUFFERS
    };

    //
    // idRenderCommand
    //
    public class idRenderCommand
    {
        public renderCommandType type;

        // RC_DRAW_BUFFER / endFrameCommand_t
        public int buffer;

        // RC_SET_COLOR
        public float[] color = new float[4];

        // RC_STRETCH_PIC
        public idMaterial shader;
        public idImage image;
	    public float x, y;
	    public float w, h;
	    public float s1, t1;
	    public float s2, t2;

	    public byte[] gradientColor = new byte[4];      // color values 0-255
	    public int gradientType;       //----(SA)	added

        public idRefdefLocal refdef;
        public viewParms_t viewParms;
        public idDrawSurface[] drawSurfs;
        public int numDrawSurfs;
    }
}
