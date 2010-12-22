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

// UsercmdManager.cs (c) 2010 JV Software
//

using idLib.Engine.Public;

namespace rtcw.Framework
{
    //
    // idUsercmdManagerLocal
    //
    public class idUsercmdManagerLocal : idUsercmdManager
    {
        private const int MAX_STORED_CMDS = 64;
        private idUsercmd[] cmdpool = new idUsercmd[MAX_STORED_CMDS];
        private int cmdnum = -1; // I set this to -1 so this will error out if Init isn't called.

        private short[] mousedelta;
        private int forwardmove;
        private int rightmove;
        private int upmove;

        //
        // Init
        //
        public override void Init()
        {
            mousedelta = new short[2];
            InitNewCommand();
        }

        //
        // InitNewCommand
        //
        private void InitNewCommand()
        {
            cmdnum++;
            if (cmdnum >= MAX_STORED_CMDS)
            {
                cmdnum = 0;
            }

            forwardmove = 0;
            rightmove = 0;
            upmove = 0;

            mousedelta[0] = 0;
            mousedelta[1] = 0;
            cmdpool[cmdnum].InitCommand(Engine.Sys.Sys_Milliseconds());
        }

        //
        // MouseEvent
        //
        public override void MouseEvent(int x, int y)
        {
            mousedelta[0] += (short)x;
            mousedelta[1] += (short)y;
        }

        //
        // ForwardButtonDown
        //
        private int ForwardButtonDown(byte key)
        {
            // Todo I need to fix this for bindings.
            if (key == (char)'W') {
                return 1;
            }

            return 0;
        }

        //
        // BackButtonDown
        //
        private int BackButtonDown(byte key)
        {
            // Todo I need to fix this for bindings.
            if (key == (char)'S')
            {
                return 1;
            }

            return 0;
        }

        //
        // LeftMoveButtonDown
        //
        private int LeftMoveButtonDown(byte key)
        {
            // Todo I need to fix this for bindings.
            if (key == (char)'A')
            {
                return 1;
            }

            return 0;
        }

        //
        // RightMoveButtonDown
        //
        private int RightMoveButtonDown(byte key)
        {
            // Todo I need to fix this for bindings.
            if (key == (char)'D')
            {
                return 1;
            }

            return 0;
        }

        //
        // KeyEvent
        //
        public override void KeyEvent(byte key, bool down)
        {
            int movespeed = 5;

            forwardmove += movespeed * ForwardButtonDown(key);
            forwardmove -= movespeed * BackButtonDown(key);

            rightmove += movespeed * LeftMoveButtonDown(key);
            rightmove -= movespeed * RightMoveButtonDown(key);
        }

        //
        // GetCurrentCommand
        //
        public override idUsercmd GetCurrentCommand()
        {
            int curcmd = cmdnum;

            cmdpool[curcmd].SetMove(forwardmove, rightmove, upmove);
            cmdpool[curcmd].SetViewAngles(ref mousedelta[0], ref mousedelta[1]);

            // Init the next command.
            InitNewCommand();

            return cmdpool[curcmd];
        }
    }
}
