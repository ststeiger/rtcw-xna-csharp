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

// cgame_globals.cs (c) 2010 JV Software
//

using idLib.Math;
using idLib.Engine.Public;
using idLib.Game.Client;
using idLib.Game.Server;

namespace cgame
{
    //
    // Globals
    //
    public static class Globals
    {
        public static idWorld       world;

        public static idModel[]     models = new idModel[idGamePublic.MAX_MODELS];
        public static int numModels = 0;

        public static idSound[]     sounds = new idSound[idGamePublic.MAX_SOUNDS];
        public static int numSounds = 0;

        public static idSkin[]      skins = new idSkin[idGamePublic.MAX_SKINS];
        public static int numSkins = 0;

        public static bool waitingToEnterWorld = false;
        public static bool viewPacketRecv = false; // set to true the first time the viewposition is set for the localview.
        public static float waitArrowFrame = 0;

        public static int localViewEntity = -1;
        public static bool inCinematic = false;

        public static idView localview = new idView();
    }
}
