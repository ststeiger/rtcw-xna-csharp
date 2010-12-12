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

// ModelBase.cs (c) 2010 JV Software
//

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;


using idLib.Engine.Public;
using idLib.Math;

namespace rtcw.Renderer.Models
{
    //
    // idModelLocal
    //
    public class idModelLocal : idModel
    {
        public string name;

        //
        // GetName
        //
        public override string GetName()
        {
            throw new NotImplementedException();
        }

        //
        // ParseMD3Vertexes
        //
        public void ParseMD3Vertexes(int numVerts, int numFrames, ref idFile f, ref idDrawVertex[] vertexes)
        {
            for (int i = 0; i < numVerts * numFrames; i++)
            {
                vertexes[i].xyz[0] = f.ReadShort();
                vertexes[i].xyz[1] = f.ReadShort();
                vertexes[i].xyz[2] = f.ReadShort();

                f.ReadShort(); // normal
            }
        }

        //
        // ParseMD3TextureCoords
        //
        public void ParseMD3TextureCoords(int numVerts, int numFrames, ref idFile f, ref idDrawVertex[] vertexes)
        {
            // Load in all the st coords, and when we are beyond numVerts, recursively set the ST's for following vertexes,
            // since ST's don't change between frames.
            for (int j = 0, a = 0; j < numVerts * numFrames; j++, a++)
            {
                if (j < numVerts)
                {
                    vertexes[j].st[0] = f.ReadFloat();
                    vertexes[j].st[1] = f.ReadFloat();
                }
                else
                {
                    if (a >= numVerts)
                    {
                        a = 0;
                    }

                    vertexes[j].st[0] = vertexes[a].st[0];
                    vertexes[j].st[1] = vertexes[a].st[1];
                }
            }
        }

        //
        // ParseMD3Frame
        //
        public void ParseMD3Frame(ref idFile f, out md3Frame_t frame)
        {
            frame = new md3Frame_t(ref f);
            if (name.Contains("sherman") || name.Contains( "mg42"))
            {
                frame.radius = 256;
                for (int j = 0; j < 3; j++)
                {
                    frame.mins[j] = 128;
                    frame.maxs[j] = -128;
                    //frame->localOrigin[j] = LittleFloat(frame->localOrigin[j]);
                }
            }
            /*
            else
            {
                for (j = 0; j < 3; j++)
                {
                    frame->bounds[0][j] = LittleFloat(frame->bounds[0][j]);
                    frame->bounds[1][j] = LittleFloat(frame->bounds[1][j]);
                    frame->localOrigin[j] = LittleFloat(frame->localOrigin[j]);
                }
            }
            */
        }
    }
}
