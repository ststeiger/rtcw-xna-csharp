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

// CollisionModelManager.cs (c) 2010 JV Software
//

using System.Collections.Generic;
using idLib.Engine.Public;

namespace rtcw.CM
{
    //
    // idCollisionModelManagerLocal
    //
    public class idCollisionModelManagerLocal : idCollisionModelManager
    {
        List<idCollisionModelLocal> cm_pool = new List<idCollisionModelLocal>();
        
        //
        // FindCollisionModel
        //
        private idCollisionModelLocal FindCollisionModel(string mappath)
        {
            for (int i = 0; i < cm_pool.Count; i++)
            {
                if (cm_pool[i].GetName() == mappath)
                {
                    return cm_pool[i];
                }
            }

            return null;
        }

        //
        // LoadCollisonModelFromBsp
        //
        public override idCollisionModel LoadCollisonModelFromBsp(string mappath, object world)
        {
            idCollisionModelLocal cm;

            // Check to see if this model has already been loaded.
            cm = FindCollisionModel(mappath);
            if (cm != null)
            {
                return cm;
            }

            // Allocate a new collision model and attach a map to it.
            cm = new idCollisionModelLocal();
            cm.LoadFromFile(mappath, world);

            cm_pool.Add(cm);
            return cm_pool[cm_pool.Count - 1];
        }
    }
}
