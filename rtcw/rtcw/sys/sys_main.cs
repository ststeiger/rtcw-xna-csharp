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

// sys_main.cs (c) 2010 JV Software
//

using System;
using System.IO;
using idLib.Engine.Public;

namespace rtcw.sys
{
    //
    // idSysLocal
    //
    public class idSysLocal : idSys
    {
        private const int MAX_QUED_EVENTS  =   256;
        private const int MASK_QUED_EVENTS =   ( MAX_QUED_EVENTS - 1 );

        private sysEvent_t[] eventQue = new sysEvent_t[MAX_QUED_EVENTS];
        private int eventHead, eventTail;

        private sysEvent_t emptySysEvent = new sysEvent_t();
        private idSystemInput sysInput;
//byte sys_packetReceived[MAX_MSGLEN];

        //
        // Init
        //
        public override void Init()
        {
            sysInput = new idSystemInput();
            sysInput.Init();
        }

        //
        // Sys_Milliseconds
        //
        int sysAppElapsedTime = 0;
        int sys_timeBase = 0;
        bool time_initialized = false;
        public override int Sys_Milliseconds()
        {
            return sysAppElapsedTime;
        }

        //
        // Frame
        //
        public override void Frame(bool appIsRunningSlowly, int frameTime, int appElapsedTime)
        {
            sysAppElapsedTime = appElapsedTime;

            if (appIsRunningSlowly)
            {
                //Engine.common.Warning("Game is running too slow\n");
                //return;
            }

            sysInput.Frame();

            Engine.common.Frame(frameTime, appElapsedTime);
        }

        /*
        ================
        Sys_QueEvent

        A time of 0 will get the current time
        Ptr should either be null, or point to a block of data that can
        be freed by the game later.
        ================
        */
        public void Sys_QueEvent(int time, sysEventType_t type, int value, int value2, int ptrLength, BinaryReader ptr)
        {
            sysEvent_t ev;

            if (eventQue[eventHead & MASK_QUED_EVENTS] == null)
            {
                eventQue[eventHead & MASK_QUED_EVENTS] = new sysEvent_t();
            }
            ev = eventQue[eventHead & MASK_QUED_EVENTS];
            if (eventHead - eventTail >= MAX_QUED_EVENTS)
            {
                Engine.common.Printf("Sys_QueEvent: overflow\n");
                // we are discarding an event, but don't leak memory
                if (ev.evPtr.ptr != null)
                {
                    ev.evPtr.ptr.Dispose();
                    ev.evPtr.ptr = null;
                }
                eventTail++;
            }

            eventHead++;

            if (time == 0)
            {
                time = Sys_Milliseconds();
            }

            ev.evTime = time;
            ev.evType = type;
            ev.evValue = value;
            ev.evValue2 = value2;
            ev.evPtrLength = ptrLength;
            ev.evPtr.ptr = ptr;
        }

        //
        // GetEvent
        //
        public override sysEvent_t GetEvent()
        {
            // return if we have data
            if (eventHead > eventTail)
            {
                eventTail++;
                return eventQue[(eventTail - 1) & MASK_QUED_EVENTS];
            }

            emptySysEvent.evTime = Sys_Milliseconds();
            return emptySysEvent;
        }
    }
}
