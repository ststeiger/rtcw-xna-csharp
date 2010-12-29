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

// ModelManager.cs (c) 2010 JV Software
//

using System;
using System.Collections.Generic;

using idLib.Engine.Public;

using rtcw.Renderer.Map;
using rtcw.Renderer.Models;

namespace rtcw.Renderer
{
    //
    // idModelManagerLocal
    //
    public class idModelManagerLocal : idModelManager
    {
        List<idModel> modelpool = new List<idModel>();

        //
        // Init
        //
        public override void Init()
        {
            
        }

        //
        // Shutdown
        //
        public override void Shutdown()
        {
            
        }

        //
        // AllocBrushModel
        //
        public idModelBrush AllocBrushModel(string name, idMap map)
        {
            // Check to see if the model has already been loaded.
            for (int i = 0; i < modelpool.Count; i++)
            {
                if (modelpool[i].GetName() == name)
                {
                    return (idModelBrush)modelpool[i];
                }
            }

            return new idModelBrush(name, map);
        }

        //
        // LoadModel
        //
        public override idModel LoadModel(string qpath)
        {
            idModel model = null;
            idFile _file;
            int iden = -1;

            // Check to see if the model has already been loaded.
            for (int i = 0; i < modelpool.Count; i++)
            {
                if (modelpool[i].GetName() == qpath)
                {
                    return modelpool[i];
                }
            }

            if (qpath.Contains(".mds"))
            {
                // Try to open the model file.
                _file = Engine.fileSystem.OpenFileRead(Engine.fileSystem.RemoveExtensionFromPath(qpath) + ".xnb", true);

                if (_file == null)
                {
                    // Strip the extension and try to load it as a MDC.
                    qpath = Engine.fileSystem.RemoveExtensionFromPath(qpath) + ".mdc";
                    _file = Engine.fileSystem.OpenFileRead(qpath, true);

                    if (_file == null)
                    {
                        Engine.common.Warning("R_LoadModel: Failed to open model " + qpath + " defaulting...\n");
                        return null;
                    }
                }
                else
                {
                    _file.DecompressCompiledFile();
                }
            }
            else
            {
                // Try to open the model file.
                _file = Engine.fileSystem.OpenFileRead(qpath, true);

                // Fixme this should return the DEFAULT model!.
                if (_file == null)
                {
                    // Strip the extension and try to load it as a MDC.
                    qpath = Engine.fileSystem.RemoveExtensionFromPath(qpath) + ".mdc";
                    _file = Engine.fileSystem.OpenFileRead(qpath, true);

                    if (_file == null)
                    {
                        Engine.common.Warning("R_LoadModel: Failed to open model " + qpath + " defaulting...\n");
                        return null;
                    }
                }
            }

            // Load the first four bytes to determine what kind of model it is.
            iden = _file.ReadInt();
            switch (iden)
            {
                case idModelMD3.MD3_IDENT:
                    model = new idModelMD3(ref _file);
                    break;

                case idModelMDC.MDC_IDENT:
                    model = new idModelMDC(ref _file);
                    break;

                case idModelMDS.MDS_IDENT:
                    model = new idModelMDS(ref _file);
                    break;

                default:
                    Engine.common.ErrorFatal("R_LoadModel: Model " + qpath + " has a invalid iden\n");
                    return null;

            }

            Engine.fileSystem.CloseFile(ref _file);

            // Add the model and return it to the caller.
            modelpool.Add(model);
            return modelpool[modelpool.Count - 1];
        }
    }
}
