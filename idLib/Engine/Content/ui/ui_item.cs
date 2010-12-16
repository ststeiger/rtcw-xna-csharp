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
using idLib.Engine.Public;
using idLib.Engine.Content.ui.Private;

namespace idLib.Engine.Content.ui
{
    //
    // idUserInterfaceItem
    //
    public class idUserInterfaceItem
    {
        public idUserInterfaceItem()
        {
            textscale = 0.55f;
        }

        //
        // AllocItemType
        //
        private void AllocItemType(ref BinaryReader reader)
        {
            if (typeData != null)
            {
                return;
            }

            if (type == ui_menudef.ITEM_TYPE_LISTBOX)
            {
                typeData = new idUserInterfaceListBox();
            }
            else if (type == ui_menudef.ITEM_TYPE_EDITFIELD || type == ui_menudef.ITEM_TYPE_NUMERICFIELD || type == ui_menudef.ITEM_TYPE_VALIDFILEFIELD || type == ui_menudef.ITEM_TYPE_YESNO || type == ui_menudef.ITEM_TYPE_BIND || type == ui_menudef.ITEM_TYPE_SLIDER || type == ui_menudef.ITEM_TYPE_TEXT)
            {
                typeData = new idUserInterfaceEditField();
                if (type == ui_menudef.ITEM_TYPE_EDITFIELD || type == ui_menudef.ITEM_TYPE_VALIDFILEFIELD)
                {
                    if (((idUserInterfaceEditField)typeData).maxPaintChars == 0)
                    {
                        ((idUserInterfaceEditField)typeData).maxPaintChars = ui_globals.MAX_EDITFIELD;
                    }
                }
            }
            else if (type == ui_menudef.ITEM_TYPE_MULTI)
            {
                typeData = new idUserInterfaceMultiDef();
            }
            else if (type == ui_menudef.ITEM_TYPE_MODEL)
            {
                typeData = new idUserInterfaceModel();
            }
            else if (type == ui_menudef.ITEM_TYPE_MENUMODEL)
            {
                typeData = new idUserInterfaceModel();
            }
            else
            {
                throw new Exception("Invalid item type");
            }
        }

        public virtual void ReadBinaryFile(ref BinaryReader reader)
        {
            if (reader.ReadString() != ui_globals.ITEM_BINARY_HEADER)
            {
                throw new Exception("Invalid item header while parsing item");
            }
            window.ReadBinaryFile(ref reader);
            textRect.ReadBinaryFile(ref reader);
            type = reader.ReadInt32();
            alignment = reader.ReadInt32();
            font = reader.ReadString();
            fontSize = reader.ReadInt32();
            textalignment = reader.ReadInt32();
            textalignx = reader.ReadSingle();
            textaligny = reader.ReadSingle();
            textscale = reader.ReadSingle();
            textStyle = reader.ReadInt32();
            text = reader.ReadString();
            textSavegameInfo = reader.ReadBoolean();
            asset_model = reader.ReadString();
            asset_shader = reader.ReadString();
            mouseEnterText = reader.ReadString();
            mouseExitText = reader.ReadString();
            mouseEnter = reader.ReadString();
            mouseExit = reader.ReadString();
            action = reader.ReadString();
            onAccept = reader.ReadString();
            onFocus = reader.ReadString();
            leaveFocus = reader.ReadString();
            cvar = reader.ReadString();
            cvarTest = reader.ReadString();
            enableCvar = reader.ReadString();
            cvarFlags = reader.ReadInt32();
            focusSound = reader.ReadString();
            numColors = reader.ReadInt32();

            for (int i = 0; i < numColors; i++)
            {
                colorRanges[i] = new idUserInterfaceColorRangeDef();
                colorRanges[i].ReadBinaryFile(ref reader);
            }
            colorRangeType = reader.ReadInt32();
            special = reader.ReadInt32();
            cursorPos = reader.ReadInt32();
            if (reader.ReadBoolean() == true)
            {
                AllocItemType(ref reader);
                typeData.ReadBinaryFile(ref reader);
            }
        }

        public virtual void WriteBinaryFile(ref BinaryWriter writer)
        {
            writer.Write(ui_globals.ITEM_BINARY_HEADER);
            window.WriteBinaryFile(ref writer);
            textRect.WriteBinaryFile(ref writer);
            writer.Write(type);
            writer.Write(alignment);
            writer.Write(font);
            writer.Write(fontSize);
            writer.Write(textalignment);
            writer.Write(textalignx);
            writer.Write(textaligny);
            writer.Write(textscale);
            writer.Write(textStyle);
            writer.Write(text);
            writer.Write(textSavegameInfo);
            writer.Write(asset_model);
            writer.Write(asset_shader);
            writer.Write(mouseEnterText);
            writer.Write(mouseExitText);
            writer.Write(mouseEnter);
            writer.Write(mouseExit);
            writer.Write(action);
            writer.Write(onAccept);
            writer.Write(onFocus);
            writer.Write(leaveFocus);
            writer.Write(cvar);
            writer.Write(cvarTest);
            writer.Write(enableCvar);
            writer.Write(cvarFlags);
            writer.Write(focusSound);
            writer.Write(numColors);
            for (int i = 0; i < numColors; i++)
            {
                if (colorRanges[i] == null)
                    throw new Exception("Colorrange Null");
                colorRanges[i].WriteBinaryFile(ref writer);
            }
            writer.Write(colorRangeType);
            writer.Write(special);
            writer.Write(cursorPos);
            if (typeData != null && type >=4)
            {
                writer.Write(true);
                typeData.WriteBinaryFile(ref writer);
            }
            else
            {
                writer.Write(false);
            }
        }

        public idUserInterfaceWindow window = new idUserInterfaceWindow();
        public idUserInterfaceRectangle textRect = new idUserInterfaceRectangle();             // rectangle the text ( if any ) consumes
        public int type = 0;                       // text, button, radiobutton, checkbox, textfield, listbox, combo
        public int alignment = 0;                  // left center right
        public string font = "";              // font
        public int fontSize = 0;                       //		//----(SA)	added
        public int textalignment = 0;              // ( optional ) alignment for text within rect based on text width
        public float textalignx = 0;               // ( optional ) text alignment x coord
        public float textaligny = 0;               // ( optional ) text alignment x coord
        public float textscale = 0;                // scale percentage from 72pts
        public int textStyle = 0;                  // ( optional ) style, normal and shadowed are it for now
        public string text = "";               // display text
        public bool textSavegameInfo = false;      //----(SA)	added
        public idUserInterfaceMenuDef parent = null;                   // menu owner
        public string asset_model = "";                // handle to asset
        public idModel model;

        public string asset_shader = "";
        public idMaterial material;

        public string mouseEnterText = "";     // mouse enter script
        public string mouseExitText = "";      // mouse exit script
        public string mouseEnter = "";         // mouse enter script
        public string mouseExit = "";          // mouse exit script
        public string action = "";             // select script
        public string onAccept = "";           // NERVE - SMF - run when the users presses the enter key
        public string onFocus = "";            // select script
        public string leaveFocus = "";         // select script
        public string cvar = "";               // associated cvar
        public string cvarTest = "";           // associated cvar for enable actions
        public string enableCvar = "";         // enable, disable, show, or hide based on value, this can contain a list
        public int cvarFlags = 0;                  //	what type of action to take on cvarenables
        public idSound focusSnd=null;
        public string focusSound = "";
        public int numColors=0;                  // number of color ranges
        public idUserInterfaceColorRangeDef[] colorRanges = new idUserInterfaceColorRangeDef[ui_globals.MAX_COLOR_RANGES];
        public int colorRangeType = 0;             // either
        public float special = 0;                  // used for feeder id's etc.. diff per type
        public int cursorPos = 0;                  // cursor position in characters
	    public idUserInterfaceDefBase typeData = null;                 // type specific data ptr's
    }
}
