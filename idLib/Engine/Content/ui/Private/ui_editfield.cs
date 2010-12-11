// ui_editfield.cs (c) 2010 JV Software
//

using System;

namespace idLib.Engine.Content.ui.Private
{
    //
    // idUserInterfaceEditField
    //
    public class idUserInterfaceEditField : idUserInterfaceDefBase
    {
	    public float minVal = 0;                   //	edit field limits
        public float maxVal = 0;                   //
        public float defVal = 0;                   //
        public float range = 0;                    //
        public int maxChars = 0;                   // for edit fields
        public int maxPaintChars = 0;              // for edit fields
        public int paintOffset = 0;                //

        public override void WriteBinaryFile(ref System.IO.BinaryWriter writer)
        {
            writer.Write(minVal);
            writer.Write(maxVal);
            writer.Write(defVal);
            writer.Write(range);
            writer.Write(maxChars);
            writer.Write(maxPaintChars);
            writer.Write(paintOffset);
        }
    }
}
