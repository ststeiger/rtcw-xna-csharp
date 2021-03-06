﻿/*
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

// Interface.cs (c) 2010 JV Software 
// CG Game interface class.
//

namespace idLib.Game.Client
{
    //
    // idClientGamePublic
    //
    public abstract class idClientGamePublic
    {
        public abstract void Init();
        public abstract void Shutdown();
        public abstract void DrawMainMenu();
        public abstract void DrawLoadingScreen(bool forceRefresh);
        public abstract void DrawConnectScreen();
        public abstract void Frame();
        public abstract void ParseConfigString(string cfgstr);
        public abstract void BeginGame(string mappath);
        public abstract void HandleUIMouseEvent(int x, int y);
        public abstract void HandleMouseEvent(float x, float y);
        public abstract void HandleUIKeyEvent(int key, bool down);
        public abstract void HandleKeyEvent(int key, bool down);
        public abstract void NetworkRecieveSnapshot(ref entityState_t entity);
    }
}
