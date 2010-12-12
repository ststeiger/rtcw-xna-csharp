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

// ui_item.cs (c) 2010 JV Software
//

using System;
using System.IO;
using idLib.Math;
using idLib.Engine.Content.ui.Private;

namespace idLib.Engine.Content.ui
{
    //
    // idUserInterfaceMenuDef
    //
     public class idUserInterfaceMenuDef : idUserInterfaceItem
    {
         //
         // ReadBinaryFile
         //
         public override void ReadBinaryFile(ref BinaryReader reader)
         {
             if (reader.ReadString() != ui_globals.MENU_BINARY_HEADER)
             {
                 throw new Exception("Expected menu header while parsing menudef");
             }
             window.ReadBinaryFile(ref reader);
             fullScreen = reader.ReadBoolean();
             itemCount = reader.ReadInt32();
             fontIndex = reader.ReadInt32();
             cursorItem = reader.ReadInt32();
             fadeCycle = reader.ReadInt32();
             fadeClamp = reader.ReadInt32();
             fadeAmount = reader.ReadInt32();
             onOpen = reader.ReadString();
             onClose = reader.ReadString();
             onESC = reader.ReadString();

             for (int i = 0; i < 255; i++)
             {
                 string t = reader.ReadString();

                 if (t == "NULL")
                 {
                     break;
                 }

                 onKey[i] = t;
             }

             soundName = reader.ReadString();
             onROQDone = reader.ReadString();

             for (int i = 0; i < 4; i++)
             {
                 focusColor[i] = reader.ReadSingle();
             }
             for (int i = 0; i < 4; i++)
             {
                 disableColor[i] = reader.ReadSingle();
             }

             for (int i = 0; i < itemCount; i++)
             {
                 items[i] = new idUserInterfaceItem();
                 items[i].ReadBinaryFile(ref reader);
             }
             
         }
       public override void WriteBinaryFile(ref BinaryWriter writer)
       {
           writer.Write(ui_globals.MENU_BINARY_HEADER);
           window.WriteBinaryFile(ref writer);
           writer.Write(fullScreen);
           writer.Write(itemCount);
           writer.Write(fontIndex);
           writer.Write(cursorItem);
           writer.Write(fadeCycle);
           writer.Write(fadeClamp);
           writer.Write(fadeAmount);
           writer.Write(onOpen);
           writer.Write(onClose);
           writer.Write(onESC);

           foreach (string s in onKey)
           {
               if (s == null)
               {
                   writer.Write("NULL");
                   break;
               }
               writer.Write(s);
           }
           writer.Write(soundName);
           writer.Write(onROQDone);

           for (int i = 0; i < 4; i++)
           {
               writer.Write(focusColor[i]);
           }
           for (int i = 0; i < 4; i++)
           {
               writer.Write(disableColor[i]);
           }

           for (int i = 0; i < itemCount; i++)
           {
               items[i].WriteBinaryFile(ref writer);
           }
       }

         public bool fullScreen = false;            // covers entire screen
        public int itemCount = 0;                  // number of items;
        public int fontIndex = 0;                  //
        public int cursorItem = 0;                 // which item as the cursor
        public int fadeCycle = 0;                  //
        public float fadeClamp = 0;                //
        public float fadeAmount = 0;               //
        public string onOpen = "";             // run when the menu is first opened
        public string onClose = "";            // run when the menu is closed
        public string onESC = "";              // run when the menu is closed
        public string[] onKey = new string[255];         // NERVE - SMF - execs commands when a key is pressed
        public string soundName = "";          // background loop sound for menu
        public string onROQDone = "";          //----(SA)	added.  callback for roqs played from menus

        public idVector4 focusColor = new idVector4();              // focus color for items
        public idVector4 disableColor = new idVector4();            // focus color for items
        public idUserInterfaceItem[] items = new idUserInterfaceItem[ui_globals.MAX_MENUITEMS]; // items this menu contains
    }
}
