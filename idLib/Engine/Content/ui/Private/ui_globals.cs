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

// ui_globals.cs (c) 2010 JV Software
//

using System;
using System.IO;

namespace idLib.Engine.Content.ui.Private
{
    //
    // idUserInterfaceDefBase
    //
    public abstract class idUserInterfaceDefBase
    {
        public abstract void ReadBinaryFile(ref BinaryReader reader);
        public abstract void WriteBinaryFile(ref BinaryWriter writer);
    }

    //
    // ui_globals
    //
    public static class ui_globals
    {
// JV - added these to ensure valid positions while reading the binary data.
        public const string ASSET_BINARY_HEADER = "idAsset";
        public const string MENU_BINARY_HEADER = "idMenu";
        public const string ITEM_BINARY_HEADER = "idItem";
// JV end

        public const int MAX_MENUNAME = 32;
        public const int MAX_ITEMTEXT = 64;
        public const int MAX_ITEMACTION = 64;
        public const int MAX_MENUDEFFILE = 4096;
        public const int MAX_MENUFILE = 32768;
        public const int MAX_MENUS = 64;
        public const int MAX_MENUITEMS = 256;
        public const int MAX_COLOR_RANGES = 10;
        public const int MAX_OPEN_MENUS = 16;

        public const int WINDOW_MOUSEOVER   =      0x00000001;  // mouse is over it, non exclusive
        public const int WINDOW_HASFOCUS    =      0x00000002;  // has cursor focus, exclusive
        public const int WINDOW_VISIBLE     =      0x00000004;  // is visible
        public const int WINDOW_GREY        =      0x00000008;  // is visible but grey ( non-active )
        public const int WINDOW_DECORATION  =      0x00000010;  // for decoration only, no mouse, keyboard, etc..
        public const int WINDOW_FADINGOUT   =      0x00000020;  // fading out, non-active
        public const int WINDOW_FADINGIN    =      0x00000040;  // fading in
        public const int WINDOW_MOUSEOVERTEXT  =   0x00000080;  // mouse is over it, non exclusive
        public const int WINDOW_INTRANSITION   =   0x00000100;  // window is in transition
        public const int WINDOW_FORECOLORSET   =   0x00000200;  // forecolor was explicitly set ( used to color alpha images or not )
        public const int WINDOW_HORIZONTAL     =   0x00000400;  // for list boxes and sliders, vertical is default this is set of horizontal
        public const int WINDOW_LB_LEFTARROW   =   0x00000800;  // mouse is over left/up arrow
        public const int WINDOW_LB_RIGHTARROW  =   0x00001000;  // mouse is over right/down arrow
        public const int WINDOW_LB_THUMB       =   0x00002000;  // mouse is over thumb
        public const int WINDOW_LB_PGUP        =   0x00004000;  // mouse is over page up
        public const int WINDOW_LB_PGDN        =   0x00008000;  // mouse is over page down
        public const int WINDOW_ORBITING       =   0x00010000;  // item is in orbit
        public const int WINDOW_OOB_CLICK      =   0x00020000;  // close on out of bounds click
        public const int WINDOW_WRAPPED        =   0x00040000;  // manually wrap text
        public const int WINDOW_AUTOWRAPPED    =   0x00080000;  // auto wrap text
        public const int WINDOW_FORCED         =   0x00100000;  // forced open
        public const int WINDOW_POPUP          =   0x00200000;  // popup
        public const int WINDOW_BACKCOLORSET   =   0x00400000;  // backcolor was explicitly set
        public const int WINDOW_TIMEDVISIBLE   =   0x00800000;  // visibility timing ( NOT implemented )
        public const int WINDOW_IGNORE_HUDALPHA  =  0x01000000;  // window will apply cg_hudAlpha value to colors unless this flag is set

        // CGAME cursor type bits
        public const int CURSOR_NONE         =     0x00000001;
        public const int CURSOR_ARROW        =     0x00000002;
        public const int CURSOR_SIZER        =    0x00000004;

        public const int STRING_POOL_SIZE   =  384 * 1024;

        public const int MAX_STRING_HANDLES = 4096;
        public const int MAX_SCRIPT_ARGS    =  12;
        public const int MAX_EDITFIELD      =  256;

        public const int MAX_LB_COLUMNS  = 16;
        public const int MAX_MULTI_CVARS = 32;
        public const int CVAR_ENABLE     = 0x00000001;
        public const int CVAR_DISABLE    = 0x00000002;
        public const int CVAR_SHOW       = 0x00000004;
        public const int CVAR_HIDE       = 0x00000008;

        public const int UI_MAX_TEXT_LINES = 64;

        public const float SCROLLBAR_SIZE      = 16.0f;
        public const float SLIDER_WIDTH        = 96.0f;
        public const float SLIDER_HEIGHT       = 16.0f;
        public const float SLIDER_THUMB_WIDTH  = 12.0f;
        public const float SLIDER_THUMB_HEIGHT = 20.0f;
        public const float NUM_CROSSHAIRS      = 10.0f;

        public const string ART_FX_BASE     =    "menu/art/fx_base";
        public const string ART_FX_BLUE     =    "menu/art/fx_blue";
        public const string ART_FX_CYAN     =    "menu/art/fx_cyan";
        public const string ART_FX_GREEN    =    "menu/art/fx_grn";
        public const string ART_FX_RED      =    "menu/art/fx_red";
        public const string ART_FX_TEAL     =    "menu/art/fx_teal";
        public const string ART_FX_WHITE    =    "menu/art/fx_white";
        public const string ART_FX_YELLOW   =    "menu/art/fx_yel";

        public const string ASSET_GRADIENTBAR = "ui/assets/gradientbar2.tga";
        public const string ASSET_SCROLLBAR            = "ui/assets/scrollbar.tga";
        public const string ASSET_SCROLLBAR_ARROWDOWN  = "ui/assets/scrollbar_arrow_dwn_a.tga";
        public const string ASSET_SCROLLBAR_ARROWUP    = "ui/assets/scrollbar_arrow_up_a.tga";
        public const string ASSET_SCROLLBAR_ARROWLEFT  = "ui/assets/scrollbar_arrow_left.tga";
        public const string ASSET_SCROLLBAR_ARROWRIGHT = "ui/assets/scrollbar_arrow_right.tga";
        public const string ASSET_SCROLL_THUMB         = "ui/assets/scrollbar_thumb.tga";
        public const string ASSET_SLIDER_BAR           = "ui/assets/slider2.tga";
        public const string ASSET_SLIDER_THUMB         = "ui/assets/sliderbutt_1.tga";
    }
}
