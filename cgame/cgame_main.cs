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

// cgame_main.cs (c) 2010 JV Software
//

using System;
using idLib.Engine.Public;
using idLib.Game.Client;

namespace cgame
{
    //
    // idClientGame
    //
    public class idClientGame : idClientGamePublic
    {
        idUserInterface mainMenu;
        idUserInterface connectUI;
        idUserInterface briefingUI;


        //
        // idClientGame
        //
        public idClientGame(int version)
        {
            Engine.common.Printf("Client Game module loaded...\n");
        }

        //
        // Init
        //
        public override void Init()
        {
            // Load in the main menu.
            mainMenu = Engine.ui.FindUserInterface("main");
            if (mainMenu == null)
            {
                Engine.common.ErrorFatal("Failed to load the mainmenu\n");
                return;
            }

            // Load the loading menu.
            connectUI = Engine.ui.FindUserInterface("Connect");
            if (connectUI == null)
            {
                Engine.common.ErrorFatal("Failed to load the loading menu.\n");
                return;
            }

            briefingUI = Engine.ui.FindUserInterface("briefing");
            if (connectUI == null)
            {
                Engine.common.ErrorFatal("Failed to load the briefing menu.\n");
                return;
            }
        }

        //
        // DrawLoadingScreen
        //
        private int baseHunk = 0;
        public override void DrawLoadingScreen()
        {
            float percentDone = 0;
            
            int expectedHunk = Engine.cvarManager.Cvar_Get("com_expectedhunkusage", "0", 0).GetValueInteger();

            // Draw the briefing UI.
            briefingUI.Draw();

            // Draw the loading bar over the briefing.
            float bar_x = 200;
            float bar_y = 468;
            float bar_w = 240;
            float bar_h = 10;

            if (baseHunk == 0)
            {
                baseHunk = Engine.fileSystem.FS_LoadStack();
            }

            // show the percent complete bar
            if (expectedHunk > 0)
            {
                percentDone = (float)(baseHunk - Engine.fileSystem.FS_LoadStack()) / (float)(expectedHunk);
                if (percentDone > 0.97f)
                {
                    percentDone = 0.97f;
                }

                briefingUI.HorizontalPercentBar(bar_x, bar_y, bar_w, bar_h, percentDone);
            }
        }

        //
        // DrawConnectScreen
        //
        public override void DrawConnectScreen()
        {
            connectUI.Draw();
        }

        //
        // Shutdown
        //
        public override void Shutdown()
        {
            
        }

        //
        // DrawMainMenu
        //
        public override void DrawMainMenu()
        {
            // Set the keycatcher so the UI will pick up controller events.
            Engine.common.SetKeyCatcher(keyCatch.UI);

            // Draw the mainmenu.
            mainMenu.Draw();
        }

        //
        // HandleMouseEvent
        //
        public override void HandleMouseEvent(int x, int y)
        {
            
        }

        //
        // HandleUIMouseEvent
        //
        public override void HandleUIMouseEvent(int x, int y)
        {
            mainMenu.HandleMouseEvent(x, y);
        }

        //
        // HandleKeyEvent
        //
        public override void HandleKeyEvent(int key, bool down)
        {
            
        }

        //
        // HandleUIKeyEvent
        //
        public override void HandleUIKeyEvent(int key, bool down)
        {
            mainMenu.HandleKeyEvent((keyNum)key, down);
        }

        //
        // Frame
        //
        public override void Frame()
        {
            
        }
    }
}
