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
// ui_listbox.cs (c) 2010 JV Software
//

using System;

namespace idLib.Engine.Content.ui.Private
{
    //
    // idUserInterfaceListBox
    //
    public class idUserInterfaceListBox : idUserInterfaceDefBase
    {
        public int startPos = 0;
        public int endPos = 0;
        public int drawPadding = 0;
        public int cursorPos = 0;
        public float elementWidth = 0;
        public float elementHeight = 0;
        public int elementStyle = 0;
        public int numColumns = 0;
        public idUserInterfaceColumnInfo[] columnInfo = new idUserInterfaceColumnInfo[ui_globals.MAX_LB_COLUMNS];
	    public string doubleClick = "";
	    public bool notselectable = false;

        public override void ReadBinaryFile(ref System.IO.BinaryReader reader)
        {
            startPos = reader.ReadInt32();
            endPos = reader.ReadInt32();
            drawPadding = reader.ReadInt32();
            cursorPos = reader.ReadInt32();
            elementWidth = reader.ReadSingle();
            elementHeight = reader.ReadSingle();
            elementStyle = reader.ReadInt32();
            numColumns = reader.ReadInt32();
            for (int i = 0; i < numColumns; i++)
            {
                columnInfo[i] = new idUserInterfaceColumnInfo();
                columnInfo[i].ReadBinary(ref reader);
            }

            doubleClick = reader.ReadString();
            notselectable = reader.ReadBoolean();
        }

        public override void WriteBinaryFile(ref System.IO.BinaryWriter writer)
        {
            writer.Write(startPos);
            writer.Write(endPos);
            writer.Write(drawPadding);
            writer.Write(cursorPos);
            writer.Write(elementWidth);
            writer.Write(elementHeight);
            writer.Write(elementStyle);
            writer.Write(numColumns);
            for (int i = 0; i < numColumns; i++)
            {
                columnInfo[i].WriteBinary(ref writer);
            }
            writer.Write(doubleClick);
            writer.Write(notselectable);
        }
    }
}
