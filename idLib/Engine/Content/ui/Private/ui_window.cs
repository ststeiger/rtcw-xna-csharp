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
// ui_window.cs (c) 2010 JV Software
//

using System;
using idLib.Math;
using idLib.Engine.Public;

namespace idLib.Engine.Content.ui.Private
{
    //
    // idUserInterfaceWindow
    //
    public class idUserInterfaceWindow
    {
        public idUserInterfaceWindow()
        {
            // Allocate the window for the menu and set the default values.
            borderSize = 1;
            foreColor[0] = foreColor[1] = foreColor[2] = foreColor[3] = 1.0f;
            cinematic = -1;
        }

        public void ReadBinaryFile(ref System.IO.BinaryReader reader)
        {
            rect.ReadBinaryFile(ref reader);
            rectClient.ReadBinaryFile(ref reader);
            name = reader.ReadString();
            model = reader.ReadString();
            group = reader.ReadString();
            cinematicName = reader.ReadString();
            cinematic = reader.ReadInt32();
            style = reader.ReadInt32();
            border = reader.ReadInt32();
            ownerDraw = reader.ReadInt32();
            ownerDrawFlags = reader.ReadInt32();
            borderSize = reader.ReadInt32();
            flags = reader.ReadInt32();
            offsetTime = reader.ReadInt32();
            nextTime = reader.ReadInt32();
            //rectEffects.WriteBinaryFile(ref writer);
            // rectEffects2.WriteBinaryFile(ref writer);

            for (int i = 0; i < 4; i++)
            {
                foreColor[i] = reader.ReadSingle();
            }

            for (int i = 0; i < 4; i++)
            {
                backColor[i] = reader.ReadSingle();
            }

            for (int i = 0; i < 4; i++)
            {
                borderColor[i] = reader.ReadSingle();
            }

            for (int i = 0; i < 4; i++)
            {
                outlineColor[i] = reader.ReadSingle();
            }

            background = reader.ReadString();
        }

        public void WriteBinaryFile(ref System.IO.BinaryWriter writer)
        {
            rect.WriteBinaryFile(ref writer);
            rectClient.WriteBinaryFile(ref writer);
            writer.Write(name);
            writer.Write(model);
            writer.Write(group);
            writer.Write(cinematicName);
            writer.Write(cinematic);
            writer.Write(style);
            writer.Write(border);
            writer.Write(ownerDraw);
            writer.Write(ownerDrawFlags);
            writer.Write(borderSize);
            writer.Write(flags);
            //rectEffects.WriteBinaryFile(ref writer);
           // rectEffects2.WriteBinaryFile(ref writer);
            writer.Write(offsetTime);
            writer.Write(nextTime);

            for (int i = 0; i < 4; i++)
            {
                writer.Write(foreColor[i]);
            }

            for (int i = 0; i < 4; i++)
            {
                writer.Write(backColor[i]);
            }

            for (int i = 0; i < 4; i++)
            {
                writer.Write(borderColor[i]);
            }

            for (int i = 0; i < 4; i++)
            {
                writer.Write(outlineColor[i]);
            }

            writer.Write(background);
        }

        public idUserInterfaceRectangle rect = new idUserInterfaceRectangle();                 // client coord rectangle
        public idUserInterfaceRectangle rectClient = new idUserInterfaceRectangle();           // screen coord rectangle
	    public string name = "";               //
        public string model = "";              //
        public idModel modelHandle;

        public string group = "";              // if it belongs to a group
        public string cinematicName = "";      // cinematic name
        public idVideo cinematicHandle;

        public int cinematic = 0;                  // cinematic handle
        public int style = 0;                      //
        public int border = 0;                     //
        public int ownerDraw = 0;                  // ownerDraw style
        public int ownerDrawFlags = 0;             // show flags for ownerdraw items
        public float borderSize = 0;               //
        public int flags = 0;                      // visible, focus, mouseover, cursor
        public idUserInterfaceRectangle rectEffects;          // for various effects
        public idUserInterfaceRectangle rectEffects2;         // for various effects
        public int offsetTime = 0;                 // time based value for various effects
        public int nextTime = 0;                   // time next effect should cycle
	    public idVector4 foreColor = new idVector4();               // text color
        public idVector4 backColor = new idVector4();               // border color
        public idVector4 borderColor = new idVector4();             // border color
        public idVector4 outlineColor = new idVector4();            // border color
        public string background = "";
    }
}
