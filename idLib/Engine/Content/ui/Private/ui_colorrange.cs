// ui_colorrange.cs (c) 2010 JV Software
//

using System;
using idLib.Math;

namespace idLib.Engine.Content.ui.Private
{
    //
    // idUserInterfaceColorRangeDef
    //
    public class idUserInterfaceColorRangeDef : idUserInterfaceDefBase
    {
        public idVector4 color = new idVector4();
	    public int type = 0;
        public float low = 0;
        public float high = 0;

        public override void WriteBinaryFile(ref System.IO.BinaryWriter writer)
        {
            for (int i = 0; i < 4; i++)
            {
                writer.Write(color[i]);
            }

            writer.Write(type);
            writer.Write(low);
            writer.Write(high);
        }
    }
}
