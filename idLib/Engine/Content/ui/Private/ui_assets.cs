/*
===========================================================================

Return to Castle Wolfenstein XNA Managed C# Port
Copyright (c) 2010 JV Software
Copyright (C) 1999-2010 id Software LLC, a ZeniMax Media company. 

This file is part of the Return to Castle Wolfenstein XNA Managed C# Port GPL Source Code.  

RTCW C# Source Code is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

RTCW C# Source Code is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with RTCW C# Source Code.  If not, see <www.gnu.org/licenses/>.

In addition, the RTCW SP Source Code is also subject to certain additional terms. 
You should have received a copy of these additional terms immediately following the terms 
and conditions of the GNU General Public License which accompanied the RTCW C# Source Code.  
If not, please request a copy in writing from id Software at the address below.

If you have questions concerning this license or the applicable additional terms, you may contact in writing 
id Software LLC, c/o ZeniMax Media Inc., Suite 120, Rockville, Maryland 20850 USA.

===========================================================================
*/

// ui_assets.cs (c) 2010 JV Software
//

using System;
using idLib.Math;
using idLib.Engine.Public;

namespace idLib.Engine.Content.ui.Private
{
    //
    // idUserInterfaceCachedAssetHandles
    //
    public class idUserInterfaceCachedAssetHandles
    {
        public idFont textFont;
        public idFont smallFont;
        public idFont bigFont;
        public idFont handwritingFont;

        public idMaterial cursor;
        public idMaterial gradientBar;
        public idMaterial scrollBarArrowUp;
        public idMaterial scrollBarArrowDown;
        public idMaterial scrollBarArrowLeft;
        public idMaterial scrollBarArrowRight;
        public idMaterial scrollBar;
        public idMaterial scrollBarThumb;
        public idMaterial buttonMiddle;
        public idMaterial buttonInside;
        public idMaterial solidBox;
        public idMaterial sliderBar;
        public idMaterial sliderThumb;
        public idSound menuEnterSound;
        public idSound menuExitSound;
        public idSound menuBuzzSound;
        public idSound itemFocusSound;
    }

    public class idUserInterfaceCachedAssets
    {
        public idUserInterfaceCachedAssetHandles handles = new idUserInterfaceCachedAssetHandles();
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

        public void ReadBinaryFile(ref idFile reader)
        {
            if( reader.ReadString() != ui_globals.ASSET_BINARY_HEADER)
            {
                throw new Exception("Invalid Asset header");
            }

            fontStr = reader.ReadString();
            cursorStr = reader.ReadString();
            gradientStr = reader.ReadString();
            textFont = reader.ReadString();
            textFontSize = reader.ReadInt();
            smallFont = reader.ReadString();
            smallFontSize = reader.ReadInt();
            bigFont = reader.ReadString();
            bigFontSize = reader.ReadInt();
            handwritingFont = reader.ReadString();
            handwritingFontSize = reader.ReadInt();
            cursor = reader.ReadString();
            gradientBar = reader.ReadString();
            scrollBarArrowUp = reader.ReadString();
            scrollBarArrowDown = reader.ReadString();
            scrollBarArrowLeft = reader.ReadString();
            scrollBarArrowRight = reader.ReadString();
            scrollBar = reader.ReadString();
            scrollBarThumb = reader.ReadString();
            buttonMiddle = reader.ReadString();
            buttonInside = reader.ReadString();
            solidBox = reader.ReadString();
            sliderBar = reader.ReadString();
            sliderThumb = reader.ReadString();
            menuEnterSound = reader.ReadString();
            menuExitSound = reader.ReadString();
            menuBuzzSound = reader.ReadString();
            itemFocusSound = reader.ReadString();
            fadeClamp = reader.ReadFloat();
            fadeCycle = reader.ReadInt();
            fadeAmount = reader.ReadFloat();
            shadowX = reader.ReadInt();
            shadowY = reader.ReadInt();

            for (int i = 0; i < 4; i++)
            {
                shadowColor[i] = reader.ReadFloat();
            }
            shadowFadeClamp = reader.ReadFloat();
            fontRegistered = reader.ReadBoolean();
        }

        public void WriteBinaryFile(ref System.IO.BinaryWriter writer)
        {
            writer.Write(ui_globals.ASSET_BINARY_HEADER);
            writer.Write(fontStr);
            writer.Write(cursorStr);
            writer.Write(gradientStr);
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
