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
