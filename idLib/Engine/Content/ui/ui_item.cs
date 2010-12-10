// ui_item.cs (c) 2010 JV Software
//

using System;
using idLib.Engine.Public;
using idLib.Engine.Content.ui.Private;

namespace idLib.Engine.Content.ui
{
    //
    // idUserInterfaceItem
    //
    public class idUserInterfaceItem
    {
        public idUserInterfaceWindow window;                  // common positional, border, style, layout info
        public idUserInterfaceRectangle textRect;             // rectangle the text ( if any ) consumes
        public int type;                       // text, button, radiobutton, checkbox, textfield, listbox, combo
        public int alignment;                  // left center right
        public int font;                       //		//----(SA)	added
        public int textalignment;              // ( optional ) alignment for text within rect based on text width
        public float textalignx;               // ( optional ) text alignment x coord
        public float textaligny;               // ( optional ) text alignment x coord
        public float textscale;                // scale percentage from 72pts
        public int textStyle;                  // ( optional ) style, normal and shadowed are it for now
        public string text;               // display text
        public bool textSavegameInfo;      //----(SA)	added
        public idUserInterfaceMenuDef parent;                   // menu owner
        public object asset;                // handle to asset
        public string mouseEnterText;     // mouse enter script
        public string mouseExitText;      // mouse exit script
        public string mouseEnter;         // mouse enter script
        public string mouseExit;          // mouse exit script
        public string action;             // select script
        public string onAccept;           // NERVE - SMF - run when the users presses the enter key
        public string onFocus;            // select script
        public string leaveFocus;         // select script
        public string cvar;               // associated cvar
        public string cvarTest;           // associated cvar for enable actions
        public string enableCvar;         // enable, disable, show, or hide based on value, this can contain a list
        public int cvarFlags;                  //	what type of action to take on cvarenables
        public idSound focusSound;
        public int numColors;                  // number of color ranges
        public idUserInterfaceColorRangeDef[] colorRanges = new idUserInterfaceColorRangeDef[ui_globals.MAX_COLOR_RANGES];
        public int colorRangeType;             // either
        public float special;                  // used for feeder id's etc.. diff per type
        public int cursorPos;                  // cursor position in characters
	    public idUserInterfaceDefBase typeData;                 // type specific data ptr's
    }
}
