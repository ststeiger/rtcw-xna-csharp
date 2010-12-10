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

// Sys_public.cs (c) 2010 JV Software
//

using System;
using System.IO;

namespace idLib.Engine.Public
{
    public class sysEventPtr_t
    {
        public BinaryReader ptr;
    }

    public enum sysEventType_t {
	    // bk001129 - make sure SE_NONE is zero
	    SE_NONE = 0,    // evTime is still valid
	    SE_KEY,     // evValue is a key code, evValue2 is the down flag
	    SE_CHAR,    // evValue is an ascii char
	    SE_MOUSE,   // evValue and evValue2 are reletive signed x / y moves
	    SE_JOYSTICK_AXIS,   // evValue is an axis number and evValue2 is the current state (-127 to 127)
	    SE_CONSOLE, // evPtr is a char*
	    SE_PACKET   // evPtr is a netadr_t followed by data bytes to evPtrLength
    };

    public class sysEvent_t
    {
        public int evTime;
        public sysEventType_t evType;
        public int evValue, evValue2;
        public int evPtrLength;                // bytes of data pointed to by evPtr, for journaling
        public sysEventPtr_t evPtr = new sysEventPtr_t();         // this must be manually freed if not NULL
    };

    //
    // idSys
    //
    public abstract class idSys
    {
        public abstract void Init();
        public abstract void Frame(bool appIsRunningSlowly, int frameTime, int appElapsedTime);
        public abstract sysEvent_t GetEvent();
        public abstract int Sys_Milliseconds();
    }
}
