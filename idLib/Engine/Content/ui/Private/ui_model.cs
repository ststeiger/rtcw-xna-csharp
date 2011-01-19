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

// ui_model.cs (c) 2010 JV Software
//

using System;
using idLib.Math;
using idLib.Engine.Public;

namespace idLib.Engine.Content.ui.Private
{
    //
    // idUserInterfaceModel
    //
    public class idUserInterfaceModel : idUserInterfaceDefBase
    {
        public int angle = 0;
        public idVector3 origin = new idVector3();
        public float fov_x = 0;
        public float fov_y = 0;
        public int rotationSpeed = 0;

        public int animated = 0;
        public int startframe = 0;
        public int numframes = 0;
        public int loopframes = 0;
        public int fps = 0;

        public int frame = 0;
        public int oldframe = 0;
        public float backlerp = 0;
	    public int frameTime = 0;

        public override void WriteBinaryFile(ref System.IO.BinaryWriter writer)
        {
            writer.Write(angle);
            for (int i = 0; i < 3; i++)
            {
                writer.Write(origin[i]);
            }
            writer.Write(fov_x);
            writer.Write(fov_y);
            writer.Write(rotationSpeed);
            writer.Write(animated);
            writer.Write(startframe);
            writer.Write(numframes);
            writer.Write(loopframes);
            writer.Write(fps);
            writer.Write(frame);
            writer.Write(oldframe);
            writer.Write(backlerp);
            writer.Write(frameTime);
        }

        public override void ReadBinaryFile(ref idFile reader)
        {
            angle = reader.ReadInt();
            for (int i = 0; i < 3; i++)
            {
                origin[i] = reader.ReadFloat();
            }
            fov_x = reader.ReadFloat();
            fov_y = reader.ReadFloat();
            rotationSpeed = reader.ReadInt();
            animated = reader.ReadInt();
            startframe = reader.ReadInt();
            numframes = reader.ReadInt();
            loopframes = reader.ReadInt();
            fps = reader.ReadInt();
            frame = reader.ReadInt();
            oldframe = reader.ReadInt();
            backlerp = reader.ReadInt();
            frameTime = reader.ReadInt();
        }
    }
}
