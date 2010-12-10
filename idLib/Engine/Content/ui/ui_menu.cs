// ui_item.cs (c) 2010 JV Software
//

using System;
using idLib.Math;
using idLib.Engine.Content.ui.Private;

namespace idLib.Engine.Content.ui
{
    //
    // idUserInterfaceMenuDef
    //
    public class idUserInterfaceMenuDef
    {
        public idUserInterfaceWindow window;
        public string font;              // font
        public bool fullScreen;            // covers entire screen
        public int itemCount;                  // number of items;
        public int fontIndex;                  //
        public int cursorItem;                 // which item as the cursor
        public int fadeCycle;                  //
        public float fadeClamp;                //
        public float fadeAmount;               //
        public string onOpen;             // run when the menu is first opened
        public string onClose;            // run when the menu is closed
        public string onESC;              // run when the menu is closed
        public string[] onKey = new string[255];         // NERVE - SMF - execs commands when a key is pressed
        public string soundName;          // background loop sound for menu
        public string onROQDone;          //----(SA)	added.  callback for roqs played from menus

        public idVector4 focusColor;              // focus color for items
        public idVector4 disableColor;            // focus color for items
        public idUserInterfaceItem[] items = new idUserInterfaceItem[ui_globals.MAX_MENUITEMS]; // items this menu contains
    }
}
