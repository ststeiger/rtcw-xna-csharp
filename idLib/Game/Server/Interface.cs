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

// Interface.cs (c) 2010 JV Software 
// Game interface class.
//

using idLib.Engine.Public.Net;

namespace idLib.Game.Server
{
    //
    // idGamePublic
    //
    public abstract class idGamePublic
    {
        //
        // per-level limits
        //
        public const int MAX_CLIENTS    =     128;     // absolute limit
        public const int MAX_LOCATIONS  =     64;

        // jv - these are no longer sent over as a 8bit, but you shouldn't change these anyway.
        public const int MAX_MODELS     =     256; 
        public const int MAX_SOUNDS     =     256;
        public const int MAX_SKINS      =     256;
        // jv end

        public const int GENTITYNUM_BITS  =   10;      // don't need to send any more
        //#define	GENTITYNUM_BITS		11		// don't need to send any more		(SA) upped 4/21/2001 adjusted: tr_local.h (802-822), tr_main.c (1501), sv_snapshot (206)
        public const int MAX_GENTITIES = (1 << GENTITYNUM_BITS);

        public abstract void Init(string mapname, int levelTime, int randomSeed, int restart);
        public abstract void Shutdown(bool restart);
        public abstract void ClientConnect(int clientNum, string clientname, bool firstTime, bool isBot);
        public abstract void ClientBegin(int clientNum);
        public abstract string GetConfigString();
        public abstract void Frame();
    }
}
