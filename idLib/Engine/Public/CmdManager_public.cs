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


// CmdManager_public.cs (c) 2010 JV Software
//

using System;

namespace idLib.Engine.Public
{
    // paramters for command buffer stuffing
    public enum cbufExec_t
    {
	    EXEC_NOW,           // don't return until completed, a VM should NEVER use this,
						    // because some commands might cause the VM to be unloaded...
	    EXEC_INSERT,        // insert at current position, but don't run yet
	    EXEC_APPEND         // add to end of the command buffer (normal case)
    };

    //
    // idCmdManager
    //
    public abstract class idCmdManager
    {
        public delegate void xcommand_t();

        public abstract void Cbuf_AddText(string text);
        public abstract void Cbuf_InsertText(string text);
        public abstract void Cbuf_ExecuteText(cbufExec_t exec_when, string text);
        public abstract void Cbuf_Execute();

        public abstract void Init();
        public abstract void Cmd_ExecuteString( string text );
        public abstract int Cmd_Argc();
        public abstract string Cmd_Argv(int arg);
        public abstract string Cmd_Args();
        public abstract string Cmd_ArgsFrom(int arg);
        public abstract void Cmd_AddCommand( string cmd_name, xcommand_t function );
        public abstract void Cmd_RemoveCommand( string cmd_name );
        public abstract void Cmd_TokenizeString( string text_in );
    }
}
