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
	    public int count;
	    public bool strDef;
    }
}
