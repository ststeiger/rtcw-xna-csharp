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
	    public float minVal;                   //	edit field limits
        public float maxVal;                   //
        public float defVal;                   //
        public float range;                    //
        public int maxChars;                   // for edit fields
        public int maxPaintChars;              // for edit fields
        public int paintOffset;                //
    }
}
