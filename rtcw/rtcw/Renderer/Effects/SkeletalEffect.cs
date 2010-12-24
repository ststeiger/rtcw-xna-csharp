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

// SkeletalEffect.cs (c) 2010 JV Software
//

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using idLib.Engine.Public;

namespace rtcw.Renderer.Effects
{
    //
    // idSkeletalEffect
    //
    public class idSkeletalEffect
    {
        Effect d3deffect;

        Matrix world;
        Matrix view;
        Matrix projection;

        int boneparem = -1;
        int textureparem = -1;
        int modelviewmatrixparem = -1;

        bool dirty = false;

        //
        // idSkeletalEffect
        //
        public idSkeletalEffect()
        {
            d3deffect = Engine.fileSystem.ReadContent<Effect>("effects/SkeletalEffect");
            world = Matrix.Identity;
            view = Matrix.Identity;
            projection = Matrix.Identity;

            boneparem = FindParametor("Bones");
            textureparem = FindParametor("Texture");
            modelviewmatrixparem = FindParametor("ModelViewProjectionMatrix");
        }

        public void Apply()
        {
            if(dirty)
            {
                d3deffect.Parameters[modelviewmatrixparem].SetValue(world * view * projection);
                dirty = false;
            }

            d3deffect.CurrentTechnique.Passes[0].Apply();
        }

        public void SetBoneMatrixes(Matrix[] bones)
        {
            d3deffect.Parameters[boneparem].SetValue(bones);
        }

        //
        // FindParametor
        //
        private int FindParametor(string name)
        {
            int num = 0;
            foreach (EffectParameter parem in d3deffect.Parameters)
            {
                if (parem.Name == name)
                {
                    return num;
                }
                num++;
            }

            return -1;
        }

        public Texture2D Texture
        {
            set
            {
                Globals.graphics3DDevice.Textures[0] = value;
                d3deffect.Parameters[textureparem].SetValue(value);
            }
        }

        public Matrix Projection
        {
            get
            {
                return projection;
            }
            set
            {
                projection = value;
                dirty = true;
            }
        }

        public Matrix View
        {
            get
            {
                return view;
            }
            set
            {
                view = value;
                dirty = true;
            }
        }

        public Matrix World
        {
            get
            {
                return world;
            }
            set
            {
                world = value;
                dirty = true;
            }
        }
    }
}
