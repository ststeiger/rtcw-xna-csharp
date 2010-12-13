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

// StageIterator_generic.cs (c) 2010 JV Software
//

using System;

namespace rtcw.Renderer.Backend.Iterators
{
    //
    // StageIteratorGeneric
    //
    public static class StageIteratorGeneric
    {
        //
        // Iterator
        //
        public static void Iterator()
        {
            for (int i = 0; i < idMaterialBase.MAX_SHADER_STAGES; i++)
            {
                shaderStage_t stage;

                if (Globals.tess.shader.stages[i] == null)
                {
                    break;
                }

                stage = Globals.tess.shader.stages[i];

                Shade.SetMaterialStageState(ref stage);

                for (int c = 0; c < idMaterialBase.NUM_TEXTURE_BUNDLES; c++)
                {
                    if (stage.bundle[i] == null)
                    {
                        break;
                    }

                    Shade.BindImage(ref stage.bundle[i].image[0]);
                    Shade.DrawTess();
                }
            }
        }
    }
}
