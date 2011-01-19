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

// ImageManager.cs (c) 2010 JV Software
//

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using idLib;
using idLib.Engine.Public;

namespace rtcw.Renderer
{
    //
    // idImageManagerLocal
    //
    public class idImageManagerLocal : idImageManager
    {
        List<idImageLocal> hashTable;
        const int DEFAULT_SIZE = 16;

        private byte[] s_intensitytable = new byte[256];
        private char[] s_gammatable = new char[256];

        //
        // AllocImage
        //
        private idImageLocal AllocImage(string name, ref bool uniqueImage)
        {
            int hashValue = idString.GenerateHashValue(name);

            for (int i = 0; i < hashTable.Count; i++)
            {
                if (hashTable[i].hashcode == hashValue && name.Contains("*") == false)
                {
                    uniqueImage = false;
                    return hashTable[i];
                }
            }

            uniqueImage = true;

            idImageLocal image = new idImageLocal();
            image.hashcode = hashValue;
            hashTable.Add(image);
            return hashTable[hashTable.Count - 1];
        }

        //
        // CreateImage
        //
        public override idImage CreateImage(string name, Color[] pic, int width, int height, bool mipmap, bool allowPicmip, SamplerState WrapClampMode)
        {
            bool isUniqueImage = false;

            // Remove the file extension if its present.
            if (!name.Contains("*"))
            {
                name = Engine.fileSystem.RemoveExtensionFromPath(name);
            }

            idImageLocal image = AllocImage(name, ref isUniqueImage);
            if (isUniqueImage == false)
            {
                Engine.common.Warning("R_CreateImage: Create image already defined, use blit instead\n");
                return image;
            }

            image.Init(name, width, height, mipmap, false);
            image.SetWrapState(WrapClampMode);
            image.BlitImageData(ref pic);

            return image;
        }

        //
        // CreateImage
        //
        public override idImage CreateDXTImage(string name, byte[] pic, int width, int height, bool mipmap, bool allowPicmip, SamplerState WrapClampMode)
        {
            bool isUniqueImage = false;

            // Remove the file extension if its present.
            if (!name.Contains("*"))
            {
                name = Engine.fileSystem.RemoveExtensionFromPath(name);
            }

            idImageLocal image = AllocImage(name, ref isUniqueImage);
            if (isUniqueImage == false)
            {
                Engine.common.Warning("R_CreateImage: Create image already defined, use blit instead\n");
                return image;
            }

            image.Init(name, width, height, mipmap, true);
            image.SetWrapState(WrapClampMode);
            image.BlitImageData(ref pic);

            return image;
        }

        //
        // FindImage
        //
        public override idImage FindImage(string qpath)
        {
            for (int i = 0; i < hashTable.Count; i++)
            {
                if (hashTable[i].Name() == qpath)
                {
                    return hashTable[i];
                }
            }

            return null;
        }

        //
        // FindImageFile
        //
        public override idImage FindImageFile(string qpath, bool mipmap, bool picmap, SamplerState wrapClampMode)
        {
            idFile imagefile;
            bool isUniqueImage = false;

            idImageLocal image = AllocImage(qpath, ref isUniqueImage);
            if (isUniqueImage == false)
            {
                return image;
            }

            imagefile = Engine.fileSystem.OpenFileRead(Engine.fileSystem.RemoveExtensionFromPath(qpath) + ".xnb", true);
            if (imagefile == null)
            {
                Engine.common.Warning("R_FindImageFile: Failed to find image %s \n", qpath);
                return FindImage("*white");
            }

            image.Init(qpath, ref imagefile);
            image.SetWrapState(wrapClampMode);

            Engine.fileSystem.CloseFile(ref imagefile);

            return image;
        }

        //
        // DestroyImage
        //
        public override void DestroyImage(ref idImage image)
        {
            for (int i = 0; i < hashTable.Count; i++)
            {
                if (hashTable[i].Name() == image.Name())
                {
                    hashTable[i].Dispose();
                    image = null;
                    return;
                }
            }

            Engine.common.Warning("R_DestoryImage: Image " + image.Name() + " wasn't allocated by the imagemanager.\n");
            ((idImageLocal)image).Dispose();
        }

        //
        // CreateBuiltinImages
        //
        private void CreateBuiltinImages()
        {
            Color[] data = new Color[DEFAULT_SIZE * DEFAULT_SIZE];

            for (int i = 0; i < DEFAULT_SIZE * DEFAULT_SIZE; i++)
            {
                data[i] = new Color(255, 255, 255, 255);
            }

            Globals.tr.whiteImage = CreateImage("*white", data, DEFAULT_SIZE, DEFAULT_SIZE, false, false, SamplerState.LinearWrap);

            // with overbright bits active, we need an image which is some fraction of full color,
            // for default lightmaps, etc
            for (int i = 0; i < DEFAULT_SIZE * DEFAULT_SIZE; i++)
            {
                data[i].R = (byte)Globals.tr.identityLightByte;
                data[i].G = (byte)Globals.tr.identityLightByte;
                data[i].B = (byte)Globals.tr.identityLightByte;
                data[i].A = 255;
            }

            Globals.tr.identityLightImage = CreateImage("*identityLight", data, DEFAULT_SIZE, DEFAULT_SIZE, false, false, SamplerState.LinearWrap);

            for (int i = 0; i < 32; i++)
            {
                // scratchimage is usually used for cinematic drawing
                Globals.tr.scratchImage[i] = CreateImage("*scratch", data, DEFAULT_SIZE, DEFAULT_SIZE, false, true, SamplerState.LinearClamp);
            }
        }

        /*
        ===============
        R_SetColorMappings
        ===============
        */
        private void R_SetColorMappings() {
	        int i, j;
	        float g;
	        int inf;
	        int shift;

	        // setup the overbright lighting
	        Globals.tr.overbrightBits =  Globals.r_overBrightBits.GetValueInteger();
	        //if ( !glConfig.deviceSupportsGamma ) {
		    //    tr.overbrightBits = 0;      // need hardware gamma for overbright
	        //}

	        // never overbright in windowed mode
	        //if ( !glConfig.isFullscreen ) {
		    //    tr.overbrightBits = 0;
	        //}

	        // allow 2 overbright bits in 24 bit, but only 1 in 16 bit
            if ( Globals.tr.overbrightBits > 2 ) {
			   Globals.tr.overbrightBits = 2;
		    }

	        if ( Globals.tr.overbrightBits < 0 ) {
		        Globals.tr.overbrightBits = 0;
	        }

	        Globals.tr.identityLight = 1.0f / ( 1 << Globals.tr.overbrightBits );
	        Globals.tr.identityLightByte = (int)(255 * Globals.tr.identityLight);


	        if ( Globals.r_intensity.GetValueFloat() <= 1 ) {
		        Engine.cvarManager.Cvar_Set( "r_intensity", "1", true );
	        }

	        if ( Globals.r_gamma.GetValueFloat() < 0.5f ) {
		        Engine.cvarManager.Cvar_Set( "r_gamma", "0.5", true );
	        } else if (Globals.r_gamma.GetValueFloat()  > 3.0f ) {
		        Engine.cvarManager.Cvar_Set( "r_gamma", "3.0", true );
	        }

	        g = Globals.r_gamma.GetValueFloat();

	        shift = Globals.tr.overbrightBits;

	        for ( i = 0; i < 256; i++ ) {
		        if ( g == 1 ) {
			        inf = i;
		        } else {
			        inf = (int)(255 * (float)System.Math.Pow( i / 255.0f, 1.0f / g ) + 0.5f);
		        }
		        inf <<= shift;
		        if ( inf < 0 ) {
			        inf = 0;
		        }
		        if ( inf > 255 ) {
			        inf = 255;
		        }
		        s_gammatable[i] = (char)inf;
	        }

	        for ( i = 0 ; i < 256 ; i++ ) {
		        j = i * Globals.r_intensity.GetValueInteger();
		        if ( j > 255 ) {
			        j = 255;
		        }
		        s_intensitytable[i] = (byte)j;
	        }

	        //if ( glConfig.deviceSupportsGamma ) {
		        //GLimp_SetGamma( s_gammatable, s_gammatable, s_gammatable );
	        //}
        }

        //
        // Shutdown
        //
        public override void Shutdown()
        {
            throw new NotImplementedException();
        }

        //
        // Init
        //
        public override void Init()
        {
            hashTable = new List<idImageLocal>();

            // build brightness translation tables
            R_SetColorMappings();

            // create default texture and white texture
            CreateBuiltinImages();
        }
    }
}
