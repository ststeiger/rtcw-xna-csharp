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

        public virtual void WriteBinaryFile(ref BinaryWriter writer)
        {
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
            writer.Write(type);
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
            if (typeData != null)
            {
                typeData.WriteBinaryFile(ref writer);
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
        public string asset_shader = "";
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
        //public idSound focusSound=null;
        public string focusSound = "";
        public int numColors=0;                  // number of color ranges
        public idUserInterfaceColorRangeDef[] colorRanges = new idUserInterfaceColorRangeDef[ui_globals.MAX_COLOR_RANGES];
        public int colorRangeType = 0;             // either
        public float special = 0;                  // used for feeder id's etc.. diff per type
        public int cursorPos = 0;                  // cursor position in characters
	    public idUserInterfaceDefBase typeData;                 // type specific data ptr's
    }
}
