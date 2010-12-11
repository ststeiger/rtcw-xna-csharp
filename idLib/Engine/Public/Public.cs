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

// Public.cs (c) 2010 JV Software
//

using System;

namespace idLib.Engine.Public
{
    //
    // connstate_t
    //
    public enum connstate_t{
	    CA_UNINITIALIZED,
	    CA_DISCONNECTED,    // not talking to a server
	    CA_AUTHORIZING,     // not used any more, was checking cd key
	    CA_CONNECTING,      // sending request packets to the server
	    CA_CHALLENGING,     // sending challenge packets to the server
	    CA_CONNECTED,       // netchan_t established, getting gamestate
	    CA_LOADING,         // only during cgame initialization, never during main loop
	    CA_PRIMED,          // got gamestate, waiting for first frame
	    CA_ACTIVE,          // game views should be displayed
	    CA_CINEMATIC        // playing a cinematic or a static pic, not connected to a server
    };

    //
    // Engine
    //
    // All engine modules reference this static class.
    // A user mod should never modify this file
    public static class Engine
    {
        public const string BASEGAME = "main";  // Remember if this changes, update the content project as well. 
        public const string Q3_VERSION = "Wolf 1.41";
#if WINDOWS
        public const string CPUSTRING = "win-x86";
#elif WINDOWS_PHONE
        public const string CPUSTRING = "win7-ARM";
#elif XBOX360
        public const string CPUSTRING = "xbox-Xeon";
#endif
        public const string BUILDNUM = "1";
        public const int PROTOCOL_VERSION = 2;
        public const string MASTER_SERVER_ADDR = "master.gmistudios.com";

        //
        // these aren't needed by any of the VMs.  put in another header?
        //
        public const int MAX_MAP_AREA_BYTES   =   32;

        // All public class managers that all modules use throughout the application.
        public static idSys             Sys;            // All non portable system functions.
        public static idRenderSystem    RenderSystem;   // Exposed rendering functions.
        public static idCommon          common;         // All common engine interfaces.
        public static idCVarManager     cvarManager;    // Cvar manager for a global variable implementation.
        public static idFileSystem      fileSystem;     // Responsible for all engine IO calls.
        public static idCmdManager      cmdSystem;
        public static idNetwork         net;
        public static idImageManager    imageManager;   // This is only valid when rendersystem is valid.
        public static idMaterialManager materialManager; // This is only valid when rendersystem is valid.
        public static idSoundManager    soundManager;
        public static idUserInterfaceManager ui;
    }
}
