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

// Render_image.cs (c) 2010 JV Software
//

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using idLib.Engine.Public;

namespace rtcw.Renderer
{
    //
    // idImageLocal
    //
    class idImageLocal : idImage
    {
        public string name;
        public int imagewidth;
        public int imageheight;

        private Texture2D tex2d;
        private SamplerState WrapClampMode = SamplerState.LinearWrap;

        //
        // Init
        //
        public void Init(string imgpath)
        {
            tex2d = Engine.fileSystem.ReadContent<Texture2D>(imgpath);
            imagewidth = tex2d.Width;
            imageheight = tex2d.Height;
            name = imgpath;
        }

        //
        // GetDeviceHandle
        //
        public override object GetDeviceHandle()
        {
            return tex2d;
        }

        //
        // Init
        //
        public void Init(string imgname, int width, int height, bool mipmap)
        {
            name = imgname;
            imagewidth  = width;
            imageheight = height;

            tex2d = new Texture2D(Globals.graphics3DDevice, width, height, mipmap, SurfaceFormat.Color);
        }

        //
        // BlitImageData
        //
        public override void BlitImageData(ref Color[] data)
        {
            Globals.backEnd.SyncRenderThread();

            Globals.graphics3DDevice.Textures[0] = null;
            tex2d.SetData<Color>(data);
        }

        //
        // SetWrapState
        //
        public override void SetWrapState(SamplerState WrapClampMode)
        {
            this.WrapClampMode = WrapClampMode;
        }

        //
        // Dispose
        //
        public void Dispose()
        {
            tex2d.Dispose();
        }

        //
        // Name
        //
        public override string Name()
        {
            return name;
        }

        //
        // Width
        //
        public override int Width()
        {
            return imagewidth;
        }

        //
        // Height
        //
        public override int Height()
        {
            return imageheight;
        }
    }
}
