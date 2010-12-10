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
        public int startPos;
        public int endPos;
        public int drawPadding;
        public int cursorPos;
        public float elementWidth;
        public float elementHeight;
        public int elementStyle;
        public int numColumns;
        public idUserInterfaceColumnInfo[] columnInfo = new idUserInterfaceColumnInfo[ui_globals.MAX_LB_COLUMNS];
	    public string doubleClick;
	    public bool notselectable;
    }
}
