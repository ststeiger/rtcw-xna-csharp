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

// Cvar_Public.cs (c) 2010 JV Software
//

using System;

namespace idLib.Engine.Public
{
    //
    // idCVar
    //
    public abstract class idCVar
    {
        public abstract string GetName();
        public abstract void ResetVar();

        public abstract void SetValue(string val, bool force);
        public abstract void SetValueFloat(float val);
        public abstract void SetValueInt(int val);
        public abstract void SetValueShort(short val);
        public abstract void SetValueBool(bool val);

        
        public abstract string GetValue();
        public abstract bool   GetValueBool();
        public abstract float  GetValueFloat();
        public abstract int    GetValueInteger();
        public abstract short  GetValueShort();

        /*
        ==========================================================

        CVARS (console variables)

        Many variables can be used for cheating purposes, so when
        cheats is zero, force all unspecified variables to their
        default values.
        ==========================================================
        */

        public const int CVAR_ARCHIVE     = 1;   // set to cause it to be saved to vars.rc
								        // used for system variables, not for player
								        // specific configurations
        public const int CVAR_USERINFO    =   2;   // sent to server on connect or change
        public const int CVAR_SERVERINFO  =   4;   // sent in response to front end requests
        public const int CVAR_SYSTEMINFO  =   8;   // these cvars will be duplicated on all clients
        public const int CVAR_INIT        =   16;  // don't allow change from console at all,
								        // but can be set from the command line
        public const int CVAR_LATCH       =   32;  // will only change when C code next does
								        // a Cvar_Get(), so it can't be changed
								        // without proper initialization.  modified
								        // will be set, even though the value hasn't
								        // changed yet
        public const int CVAR_ROM          =   64;  // display only, cannot be set by user at all
        public const int CVAR_USER_CREATED =  128; // created by a set command
        public const int CVAR_TEMP         =  256; // can be set even when cheats are disabled, but is not archived
        public const int CVAR_CHEAT        =  512; // can not be changed if cheats are disabled
        public const int CVAR_NORESTART    =  1024;    // do not clear when a cvar_restart is issued
    }

    //
    // idCVarManager
    //
    public abstract class idCVarManager
    {
        public abstract idCVar Cvar_Get(string cvar_name, string cvar_value, int cvar_flags);
        public abstract idCVar Cvar_Set(string var_name, string value, bool force);
        public abstract void WriteVariables(ref idFile f);
        public abstract void Init();
    }
}
