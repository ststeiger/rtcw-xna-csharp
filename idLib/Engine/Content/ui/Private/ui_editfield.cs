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

// ui_editfield.cs (c) 2010 JV Software
//

using System;
using idLib.Engine.Public;

namespace idLib.Engine.Content.ui.Private
{
    //
    // idUserInterfaceEditField
    //
    public class idUserInterfaceEditField : idUserInterfaceDefBase
    {
	    public float minVal = 0;                   //	edit field limits
        public float maxVal = 0;                   //
        public float defVal = 0;                   //
        public float range = 0;                    //
        public int maxChars = 0;                   // for edit fields
        public int maxPaintChars = 0;              // for edit fields
        public int paintOffset = 0;                //

        public override void WriteBinaryFile(ref System.IO.BinaryWriter writer)
        {
            writer.Write(minVal);
            writer.Write(maxVal);
            writer.Write(defVal);
            writer.Write(range);
            writer.Write(maxChars);
            writer.Write(maxPaintChars);
            writer.Write(paintOffset);
        }

        public override void ReadBinaryFile(ref idFile reader)
        {
            minVal = reader.ReadFloat();
            maxVal = reader.ReadFloat();
            defVal = reader.ReadFloat();
            range = reader.ReadFloat();
            maxChars = reader.ReadInt();
            maxPaintChars = reader.ReadInt();
            paintOffset = reader.ReadInt();
        }
    }
}
