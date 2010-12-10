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

// Common_public.cs (c) 2010 JV Software
//

using System;

namespace idLib.Engine.Public
{
    //
    // idCommon
    //
    public abstract class idCommon
    {
        // Initilizes the engine and parses the command line.
        public abstract void Init(string cmdLine);

        public abstract void Com_PushEvent( sysEvent_t new_event );
        public abstract int Com_Milliseconds();
        public abstract int ScaledMilliseconds();

        // Prints a debug message to the console.
        public abstract void Printf(string fmt, params object[] args);

        // Prints a developer only message.
        public abstract void DPrintf(string fmt, params object[] args);

        // Prints WARNING + message to the console.
        public abstract void Warning(string fmt, params object[] args);

        // Causes a C# throw, this only occurs if the error is too fatal to recover from.
        public abstract void ErrorFatal(string fmt, params object[] args);

        // Drops the client if a game is active and displays a message on the menu.
        public abstract void ErrorDrop(string fmt, params object[] args);

        // Called once per frame, this should ONLY be called from system.
        public abstract void Frame(int frameTime, int totalGameTime);
    }
}
