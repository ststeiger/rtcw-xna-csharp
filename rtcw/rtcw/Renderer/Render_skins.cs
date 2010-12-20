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

// Render_skins.cs (c) 2010 JV Software
//

using System.Collections.Generic;
using idLib;
using idLib.Math;
using idLib.Engine.Public;
using rtcw.Renderer.Models;

namespace rtcw.Renderer
{
    //
    // idSkinSurface
    //
    struct idSkinSurface {
        public string name;
	    public idMaterial shader;
    };

    //
    // idSkinModel
    //
    struct idSkinModel {
	    public string type;          // md3_lower, md3_lbelt, md3_rbelt, etc.
	    public string model;          // lower.md3, belt1.md3, etc.
    };

    //
    // idSkinLocal
    //
    class idSkinLocal : idSkin
    {
        public const int MAX_PART_MODELS = 5;
        public string name;               // game path, including extension
	    public int numSurfaces;
	    public int numModels;

        public idSkinSurface[] surfaces;
        public idSkinModel[] models;

        public idVector3 scale = new idVector3(); //----(SA)	added

        //
        // idSkinLocal
        //
        public idSkinLocal(string name)
        {
            this.name = name;
            surfaces = new idSkinSurface[idModelMD3.MD3_MAX_SURFACES];
            models = new idSkinModel[MAX_PART_MODELS];
        }

        //
        // InitFromFile
        //
        public void InitFromFile(ref idFile file)
        {
            idParser parser;
            string skinbuffer;
            string token;

            // If not a .skin file, load as a single shader
	        if ( name.Contains( ".skin" ) == false ) {
		        numSurfaces   = 0;
		        numModels     = 0;    //----(SA) added
		        numSurfaces   = 1;
                surfaces[0] = new idSkinSurface();
                surfaces[0].shader = Engine.materialManager.FindMaterial(name, -1);
		        return;
	        }

            // jv - This probably isn't the proper way to do this, but just convert all the commas to spaces
            skinbuffer = file.ReadString(file.Length());
            skinbuffer = skinbuffer.Replace(',', ' ');
            // jv - end

            parser = new idParser(skinbuffer);

            while (parser.ReachedEndOfBuffer == false)
            {
                string surfname;

                token = parser.NextToken;
                if (token == null || token.Length <= 0)
                {
                    break;
                }

                surfname = token;

                if ( token.Contains( "tag_" ) ) {
			        continue;
		        }
		        else if ( token.Contains( "md3_" ) ) {  // this is specifying a model
                    idSkinModel model = new idSkinModel();

                    model.type = token;

			        // get the model name
			        model.model = parser.NextToken;
			        models[numModels++] = model;
			        continue;
		        }
        //----(SA)	added
		        else if ( token.Contains("playerscale") ) {
			        float scaleval = float.Parse(parser.NextToken);
			        scale[0] = scaleval;   // uniform scaling for now
			        scale[1] = scaleval;
			        scale[2] = scaleval;
			        continue;
		        }
        //----(SA) end

		        // parse the shader name
		        token = parser.NextToken;
                idSkinSurface surf = new idSkinSurface();
                surf.name = surfname;
                surf.shader = Engine.materialManager.FindMaterial( token, -1 );
                numSurfaces++;
            }

            parser.Dispose();
        }
    }

    //
    // idSkinManager
    //
    class idSkinManager
    {
        List<idSkinLocal> skins = new List<idSkinLocal>();

        //
        // RegisterSkin
        //
        public idSkin RegisterSkin(string path)
        {
            idSkinLocal skin;
            idFile skinFile;

            // Check to see if the skin has already been loaded.
            for (int i = 0; i < skins.Count; i++)
            {
                if (skins[i].name == path)
                {
                    return skins[i];
                }
            }
            
            // Load in the skin.
            skinFile = Engine.fileSystem.OpenFileRead(path, true);
            if (skinFile == null)
            {
                Engine.common.Warning("R_RegisterSkin: Failed to find skin %s \n", path);
                return null;
            }

            // Parse the skin.
            skin = new idSkinLocal(path);
            skin.InitFromFile(ref skinFile);
            skins.Add(skin);

            return skins[skins.Count - 1];
        }
    }
}
