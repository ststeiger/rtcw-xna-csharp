// ui_columninfo.cs (c) 2010 JV Software
//

using System;

namespace idLib.Engine.Content.ui.Private
{
    //
    // idUserInterfaceColumnInfo
    //
    public class idUserInterfaceColumnInfo
    {
	    public int pos = 0;
	    public int width = 0;
	    public int maxChars = 0;

        public void WriteBinary(ref System.IO.BinaryWriter writer)
        {
            writer.Write(pos);
            writer.Write(width);
            writer.Write(maxChars);
        }
    }
}
