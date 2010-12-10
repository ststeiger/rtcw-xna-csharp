// ui_window.cs (c) 2010 JV Software
//

using System;
using idLib.Math;

namespace idLib.Engine.Content.ui.Private
{
    //
    // idUserInterfaceWindow
    //
    public class idUserInterfaceWindow
    {
        public idUserInterfaceRectangle rect;                 // client coord rectangle
        public idUserInterfaceRectangle rectClient;           // screen coord rectangle
	    public string name;               //
	    public string model;              //
	    public string group;              // if it belongs to a group
        public string cinematicName;      // cinematic name
	    public int cinematic;                  // cinematic handle
        public int style;                      //
        public int border;                     //
        public int ownerDraw;                  // ownerDraw style
        public int ownerDrawFlags;             // show flags for ownerdraw items
        public float borderSize;               //
        public int flags;                      // visible, focus, mouseover, cursor
        public idUserInterfaceRectangle rectEffects;          // for various effects
        public idUserInterfaceRectangle rectEffects2;         // for various effects
        public int offsetTime;                 // time based value for various effects
        public int nextTime;                   // time next effect should cycle
	    public idVector4 foreColor;               // text color
	    public idVector4 backColor;               // border color
	    public idVector4 borderColor;             // border color
	    public idVector4 outlineColor;            // border color
    }
}
