// ui_assets.cs (c) 2010 JV Software
//

using System;
using idLib.Math;

namespace idLib.Engine.Content.ui.Private
{
    public class idUserInterfaceCachedAssets
    {
     
	    public string fontStr = "";
	    public string cursorStr = "";
	    public string gradientStr = "";
	    
        public string textFont = "";
        public int textFontSize = 0;

	    public string smallFont = "";
        public int smallFontSize = 0;

	    public string bigFont = "";
        public int bigFontSize = 0;

	    public string handwritingFont = "";
        public int handwritingFontSize = 0;

	    public string cursor = "";
        public string gradientBar = "";
        public string scrollBarArrowUp = "";
        public string scrollBarArrowDown = "";
        public string scrollBarArrowLeft = "";
        public string scrollBarArrowRight = "";
        public string scrollBar = "";
        public string scrollBarThumb = "";
        public string buttonMiddle = "";
        public string buttonInside = "";
        public string solidBox = "";
        public string sliderBar = "";
        public string sliderThumb = "";
        public string menuEnterSound = "";
        public string menuExitSound = "";
        public string menuBuzzSound = "";
        public string itemFocusSound = "";
	    public float fadeClamp = 0;
	    public int fadeCycle = 0;
	    public float fadeAmount = 0;
	    public float shadowX = 0;
	    public float shadowY = 0;
	    public idVector4 shadowColor = new idVector4();
	    public float shadowFadeClamp = 0;
	    public bool fontRegistered = false;

        public void WriteBinaryFile(ref System.IO.BinaryWriter writer)
        {
            writer.Write(fontStr);
            writer.Write(cursorStr);
            writer.Write(textFont);
            writer.Write(textFontSize);
            writer.Write(smallFont);
            writer.Write(smallFontSize);
            writer.Write(bigFont);
            writer.Write(bigFontSize);
            writer.Write(handwritingFont);
            writer.Write(handwritingFontSize);
            writer.Write(cursor);
            writer.Write(gradientBar);
            writer.Write(scrollBarArrowUp);
            writer.Write(scrollBarArrowDown);
            writer.Write(scrollBarArrowLeft);
            writer.Write(scrollBarArrowRight);
            writer.Write(scrollBar);
            writer.Write(scrollBarThumb);
            writer.Write(buttonMiddle);
            writer.Write(buttonInside);
            writer.Write(solidBox);
            writer.Write(sliderBar);
            writer.Write(sliderThumb);
            writer.Write(menuEnterSound);
            writer.Write(menuExitSound);
            writer.Write(menuBuzzSound);
            writer.Write(itemFocusSound);
            writer.Write(fadeClamp);
            writer.Write(fadeCycle);
            writer.Write(fadeAmount);
            writer.Write(shadowX);
            writer.Write(shadowY);
            for (int i = 0; i < 4; i++)
            {
                writer.Write(shadowColor[i]);
            }
            writer.Write(shadowFadeClamp);
            writer.Write(fontRegistered);
        }

	    // player settings
	    //qhandle_t fxBasePic;
	    //qhandle_t fxPic[7];
	    //qhandle_t crosshairShader[NUM_CROSSHAIRS];
    }
}
