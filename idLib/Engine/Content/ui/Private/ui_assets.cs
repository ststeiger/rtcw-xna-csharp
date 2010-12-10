// ui_assets.cs (c) 2010 JV Software
//

using System;
using idLib.Math;

namespace idLib.Engine.Content.ui.Private
{
    public class idUserInterfaceCachedAssets
    {
	    public string fontStr;
	    public string cursorStr;
	    public string gradientStr;
	    
        public string textFont;
        public int textFontSize;

	    public string smallFont;
        public int smallFontSize;

	    public string bigFont;
        public int bigFontSize;

	    public string handwritingFont;
        public int handwritingFontSize;

	    public string cursor;
	    public string gradientBar;
	    public string scrollBarArrowUp;
	    public string scrollBarArrowDown;
	    public string scrollBarArrowLeft;
	    public string scrollBarArrowRight;
	    public string scrollBar;
	    public string scrollBarThumb;
	    public string buttonMiddle;
	    public string buttonInside;
	    public string solidBox;
	    public string sliderBar;
	    public string sliderThumb;
	    public string  menuEnterSound;
	    public string menuExitSound;
	    public string menuBuzzSound;
	    public string itemFocusSound;
	    public float fadeClamp;
	    public int fadeCycle;
	    public float fadeAmount;
	    public float shadowX;
	    public float shadowY;
	    public idVector4 shadowColor;
	    public float shadowFadeClamp;
	    public bool fontRegistered;

	    // player settings
	    //qhandle_t fxBasePic;
	    //qhandle_t fxPic[7];
	    //qhandle_t crosshairShader[NUM_CROSSHAIRS];
    }
}
