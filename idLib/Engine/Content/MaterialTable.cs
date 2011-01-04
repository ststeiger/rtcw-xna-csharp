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

// MaterialTable.cs (c) 2010 JV Software
//

using System;
using System.Collections.Generic;

namespace idLib.Engine.Content
{
    //
    // idMaterialLookup
    //
    public class idMaterialCached
    {
        public int hashValue;
        public string buffer;
    }

    //
    // idMaterialLookupTable
    //
    public class idMaterialLookupTable
    {
        List<idMaterialCached> mtrcache = new List<idMaterialCached>();

        public int Count
        {
            get
            {
                return mtrcache.Count;
            }
        }

        public string FindMaterialInTable(int hashValue)
        {
            for (int i = 0; i < mtrcache.Count; i++)
            {
                if (mtrcache[i].hashValue == hashValue)
                {
                    return mtrcache[i].buffer;
                }
            }

            return null;
        }

        public idMaterialCached GetMaterial(int index)
        {
            return mtrcache[index];
        }

        public void AddMaterialToCache(idMaterialCached mtr)
        {
            mtrcache.Add(mtr);
        }

        public void CombineTable(ref idMaterialLookupTable table)
        {
            for (int i = 0; i < table.Count; i++)
            {
                AddMaterialToCache(table.mtrcache[i]);
            }

            table.Dispose();
        }

        public void Dispose()
        {
            mtrcache.Clear();
        }
    }
}
