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

// decoration.cs (c) 2010 JV Software
//

using idLib.Engine.Public;

namespace Game.Entities.Props
{
    //
    // idEntityPropDecoration
    //
    public class idEntityPropDecoration : idEntity
    {
        float framenum = 0;
        int maxFrames = 0;

        //
        // Spawn
        //
        public override void Spawn()
        {
            if (model != null)
            {
                state.modelindex = Level.net.ModelIndex(model);
            }
            else
            {
                state.modelindex = Level.net.ModelIndex(model2);
                model = model2;
            }
            state.eType = idLib.Game.entityType_t.ET_GENERAL;
        }

        //
        // Frame
        //
        public override void Frame()
        {
            if (state.modelindex < 0)
                return;

            if (hModel == null)
            {
                hModel = Engine.modelManager.LoadModel(model);
                maxFrames = (int)(hModel.GetNumFrames() / 1.4f); // hack for curtains.
            }
            else
            {
                framenum += 0.5f;
                if (framenum >= maxFrames)
                {
                    framenum = 0;
                }

                state.frame = (int)framenum;
            }

            LinkEntity();
        }
    }
}
