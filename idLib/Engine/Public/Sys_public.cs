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

    // in order from highest priority to lowest
    // if none of the catchers are active, bound key strings will be executed
    public static class keyCatch
    {
        public static int CONSOLE   =   0x0001;
        public static int UI        =   0x0002;
        public static int MESSAGE   =   0x0004;
        public static int CGAME     =   0x0008;
    }

    //
    // these are the key numbers that should be passed to KeyEvent
    //

    // normal keys should be passed as lowercased ascii
    public enum keyNum
    {
	    K_TAB = 9,
	    K_ENTER = 13,
	    K_ESCAPE = 27,
	    K_SPACE = 32,

	    K_BACKSPACE = 127,

	    K_COMMAND = 128,
	    K_CAPSLOCK,
	    K_POWER,
	    K_PAUSE,

	    K_UPARROW,
	    K_DOWNARROW,
	    K_LEFTARROW,
	    K_RIGHTARROW,

	    K_ALT,
	    K_CTRL,
	    K_SHIFT,
	    K_INS,
	    K_DEL,
	    K_PGDN,
	    K_PGUP,
	    K_HOME,
	    K_END,

	    K_F1,
	    K_F2,
	    K_F3,
	    K_F4,
	    K_F5,
	    K_F6,
	    K_F7,
	    K_F8,
	    K_F9,
	    K_F10,
	    K_F11,
	    K_F12,
	    K_F13,
	    K_F14,
	    K_F15,

	    K_KP_HOME,
	    K_KP_UPARROW,
	    K_KP_PGUP,
	    K_KP_LEFTARROW,
	    K_KP_5,
	    K_KP_RIGHTARROW,
	    K_KP_END,
	    K_KP_DOWNARROW,
	    K_KP_PGDN,
	    K_KP_ENTER,
	    K_KP_INS,
	    K_KP_DEL,
	    K_KP_SLASH,
	    K_KP_MINUS,
	    K_KP_PLUS,
	    K_KP_NUMLOCK,
	    K_KP_STAR,
	    K_KP_EQUALS,

	    K_MOUSE1,
	    K_MOUSE2,
	    K_MOUSE3,
	    K_MOUSE4,
	    K_MOUSE5,

	    K_MWHEELDOWN,
	    K_MWHEELUP,

	    K_JOY1,
	    K_JOY2,
	    K_JOY3,
	    K_JOY4,
	    K_JOY5,
	    K_JOY6,
	    K_JOY7,
	    K_JOY8,
	    K_JOY9,
	    K_JOY10,
	    K_JOY11,
	    K_JOY12,
	    K_JOY13,
	    K_JOY14,
	    K_JOY15,
	    K_JOY16,
	    K_JOY17,
	    K_JOY18,
	    K_JOY19,
	    K_JOY20,
	    K_JOY21,
	    K_JOY22,
	    K_JOY23,
	    K_JOY24,
	    K_JOY25,
	    K_JOY26,
	    K_JOY27,
	    K_JOY28,
	    K_JOY29,
	    K_JOY30,
	    K_JOY31,
	    K_JOY32,

	    K_AUX1,
	    K_AUX2,
	    K_AUX3,
	    K_AUX4,
	    K_AUX5,
	    K_AUX6,
	    K_AUX7,
	    K_AUX8,
	    K_AUX9,
	    K_AUX10,
	    K_AUX11,
	    K_AUX12,
	    K_AUX13,
	    K_AUX14,
	    K_AUX15,
	    K_AUX16,

	    K_LAST_KEY      // this had better be <256!
    };

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
    // idSysModule
    //
    public abstract class idSysModule
    {
        public abstract T AllocClass<T>(string classname);
        public abstract void UnloadAndDispose();
    }

    //
    // idThread
    //
    public abstract class idThread
    {
        public abstract bool isRunning();
        public abstract void Start(object data);
        public abstract void Resume();
        public abstract void Pause();
        public abstract void Stop();
    }

    //
    // idSys
    //
    public abstract class idSys
    {
        public delegate void ThreadFunc_t();

        public abstract void Init();
        public abstract void Frame(bool appIsRunningSlowly, int frameTime, int appElapsedTime);
        public abstract sysEvent_t GetEvent();
        public abstract int Sys_Milliseconds();
        public abstract idSysModule LoadDLL(string path);
        public abstract void DrawInputDebug();
        public abstract idThread CreateThread(string threadName, ThreadFunc_t func);
        public abstract void DestroyThread(ref idThread thread);
#if WINDOWS
        public abstract void SetWindowAttributes(bool appHasFocus, Microsoft.Xna.Framework.Rectangle rect);
#endif
    }
}
