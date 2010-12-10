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

// Video_public.cs (c) 2010 JV Software
//

using System;

namespace idLib.Engine.Public
{
    // cinematic states
    public enum e_status
    {
        FMV_IDLE,
        FMV_PLAY,       // play
        FMV_EOF,        // all other conditions, i.e. stop/EOF/abort
        FMV_ID_BLT,
        FMV_ID_IDLE,
        FMV_LOOPED,
        FMV_ID_WAIT
    };

    //
    // idVideo
    //
    public abstract class idVideo
    {
        public const int CIN_system    =  0x01;
        public const int CIN_loop      =  0x02;
        public const int CIN_hold      =  0x04;
        public const int CIN_silent    =  0x08;
        public const int CIN_shader    =  0x10;
        public const int CIN_letterBox = 0x20;
        public const int LETTERBOX_OFFSET = 105;

        public abstract void SetLooping(bool loop);
        public abstract void SetExtents(int x, int y, int w, int h);
        public abstract void DrawCinematic();
        public abstract void Dispose();
        public abstract void StopCinematic();
        public abstract e_status GetStatus();
    }
}
