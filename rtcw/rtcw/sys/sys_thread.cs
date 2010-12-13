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

// sys_thread.cs (c) 2010 JV Software
//

using System;
using System.Threading;
using idLib.Engine.Public;

namespace rtcw.sys
{
    //
    // idThreadLocal
    //
    public class idThreadLocal : idThread
    {
        Thread thread;
        idSys.ThreadFunc_t func;
        string threadName;

        //
        // idThreadLocal
        //
        public idThreadLocal(string threadName, idSys.ThreadFunc_t func)
        {
            thread = new Thread(new ThreadStart( func ));
            this.threadName = threadName;
        }

        //
        // Start
        //
        public override void Start(object data)
        {
            if (isRunning())
            {
                Engine.common.Warning("idThread: " + threadName + " is already running.\n");
                return;
            }

            if (thread.ThreadState == ThreadState.Suspended)
            {
                thread.Resume();
            }
            else
            {
                thread.Start();
            }
        }

        //
        // Pause
        //
        public override void Pause()
        {
            if (isRunning() == false)
            {
                Engine.common.Warning("idThread: " + threadName + " is already stopped/paused.\n");
                return;
            }

            thread.Suspend();
        }

        //
        // Resume
        //
        public override void Resume()
        {
            thread.Resume();
        }

        //
        // Stop
        //
        public override void Stop()
        {
            throw new NotImplementedException();
        }

        //
        // isRunning
        //
        public override bool isRunning()
        {
            return (thread.ThreadState == ThreadState.Running);
        }

        //
        // Dispose
        //
        public void Dispose()
        {
            
        }
    }
}
