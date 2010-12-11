// ui_multidef.cs (C) 2010 JV Software
//

using System;

namespace idLib.Engine.Content.ui.Private
{
    //
    // idUserInterfaceMultiDef
    //
    public class idUserInterfaceMultiDef : idUserInterfaceDefBase
    {
	    public string[] cvarList = new string[ui_globals.MAX_MULTI_CVARS];
	    public string[] cvarStr = new string[ui_globals.MAX_MULTI_CVARS];
	    public float[] cvarValue = new float[ui_globals.MAX_MULTI_CVARS];
	    public int count = 0;
	    public bool strDef = false;

        public override void WriteBinaryFile(ref System.IO.BinaryWriter writer)
        {
            writer.Write(count);
            writer.Write(strDef);
            for (int i = 0; i < count; i++)
            {
                if (cvarList[i] != null)
                {
                    writer.Write(cvarList[i]);
                }
                else
                {
                    writer.Write(" ");
                }
            }
            for (int i = 0; i < count; i++)
            {
                if (cvarStr[i] != null)
                {
                    writer.Write(cvarStr[i]);
                }
                else
                {
                    writer.Write(" ");
                }
            }

            for (int i = 0; i < count; i++)
            {
                if (cvarValue[i] != null)
                {
                    writer.Write(cvarValue[i]);
                }
                else
                {
                    writer.Write(" ");
                }
            }
        }
    }
}
