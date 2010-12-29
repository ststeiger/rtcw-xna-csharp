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

// sys_module.cs (c) 2010 JV SOftware
//

using System;
using System.IO;
//using System.Security.Policy;
using System.Reflection;
using idLib.Engine.Public;

namespace rtcw.sys
{
    //
    // idSysModuleLocal
    //
    class idSysModuleLocal : idSysModule
    {
        AppDomain moduledomain;
        Assembly assembly;

        //
        // idSysModuleLocal
        //
        public idSysModuleLocal(string path)
        {
            assembly = Assembly.Load(path);
        }

        //
        // AllocClass
        //
        public override T AllocClass<T>(string classname)
        {
            Type classtype;
            Type[] argsTypes = new Type[] { typeof(int) };
            ConstructorInfo constr;

            classtype = assembly.GetType(classname);
            constr = classtype.GetConstructor(argsTypes);

            object[] param = new object[] { 0 };
            return (T)constr.Invoke(param);
        }

        //
        // UnloadAndDispose
        //
        public override void UnloadAndDispose()
        {
            throw new NotImplementedException();
            //AppDomain.Unload(moduledomain);
        }
    }
}
