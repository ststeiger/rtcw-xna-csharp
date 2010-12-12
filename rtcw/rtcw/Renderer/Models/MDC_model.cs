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

// MDC_Model.cs (c) 2010 JV Software
//

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;


using idLib.Engine.Public;
using idLib.Math;

namespace rtcw.Renderer.Models
{
    //
    // idModelMDS
    //
    public class idModelMDC : idModelLocal
    {
        public const int MDC_IDENT     =      ( ( 'C' << 24 ) + ( 'P' << 16 ) + ( 'D' << 8 ) + 'I' );
        public const float MDC_TAG_ANGLE_SCALE = ( 360.0f / 32700.0f );
        public const int MDC_VERSION   =      2;

        private mdcHeader_t header = new mdcHeader_t();
        private md3Frame_t[] frames;
        private mdcTag_t[] tags;
        private mdcSurface_t[] surfaces;


        //
        // ParseHeader
        //
        private void ParseHeader(ref idFile f)
        {
            header.ident = MDC_IDENT;
	        header.version = f.ReadInt();
	        header.name = f.ReadString(Engine.MAX_QPATH);           // model name
	        header.flags = f.ReadInt();
	        header.numFrames = f.ReadInt();
	        header.numTags = f.ReadInt();
	        header.numSurfaces = f.ReadInt();
	        header.numSkins = f.ReadInt();
	        header.ofsFrames = f.ReadInt();                  // offset for first frame, stores the bounds and localOrigin
	        header.ofsTagNames = f.ReadInt();                // numTags
	        header.ofsTags = f.ReadInt();                    // numFrames * numTags
	        header.ofsSurfaces = f.ReadInt();                // first surface, others follow
	        header.ofsEnd = f.ReadInt();                     // end of file
        }

        //
        // idModelMDC
        //
        public idModelMDC(ref idFile f)
        {
            // Set the model name.
            name = f.GetFullFilePath();

            // Parse the header.
            ParseHeader(ref f);

            // Alloc and parse the frames - these are the same format as MD3 models.
            frames = new md3Frame_t[header.numFrames];
            f.Seek(idFileSeekOrigin.FS_SEEK_SET, header.ofsFrames);

            for (int i = 0; i < header.numFrames; i++)
            {
                ParseMD3Frame(ref f, out frames[i]);
            }

            // Load in all the tags.
            tags = new mdcTag_t[header.numTags];
            f.Seek(idFileSeekOrigin.FS_SEEK_SET, header.ofsTags);

            for (int i = 0; i < header.numFrames * header.numTags; i++)
            {
                tags[i] = new mdcTag_t(ref f);
            }

            // Load in all the surfaces.
            surfaces = new mdcSurface_t[header.numSurfaces];
            f.Seek(idFileSeekOrigin.FS_SEEK_SET, header.ofsSurfaces);

            int surfpos = header.ofsSurfaces;
            for (int i = 0; i < header.numSurfaces; i++)
            {
                // Parse the surface.
                mdcSurface_t surf = new mdcSurface_t(ref f);

                // change to surface identifier
                surf.type = surfaceType_t.SF_MDC;

                // lowercase the surface name so skin compares are faster
                surf.name = surf.name.ToLower();

                // register the shaders
                //shader = (md3Shader_t*)((byte*)surf + surf->ofsShaders);
                f.Seek(idFileSeekOrigin.FS_SEEK_SET, surfpos + surf.ofsShaders);
                surf.materials = new idMaterial[surf.numShaders];
                for (int j = 0; j < surf.numShaders; j++)
                {
                    string shader_name = md3Shader_t.ParseShader(ref f);
                    surf.materials[j] = Engine.materialManager.FindMaterial(shader_name, -1);
                }

                // Load in all the triangles.
                f.Seek(idFileSeekOrigin.FS_SEEK_SET, surfpos + surf.ofsTriangles);
                surf.indexes = new short[surf.numTriangles * 3];
                for (int j = 0; j < surf.numTriangles * 3; j+=3)
                {
                    surf.indexes[j + 0] = (short)f.ReadInt();
                    surf.indexes[j + 1] = (short)f.ReadInt();
                    surf.indexes[j + 2] = (short)f.ReadInt();
                }
                
                // Load in the MD3 texture coordinates.
                f.Seek(idFileSeekOrigin.FS_SEEK_SET, surfpos + surf.ofsSt);

                surf.AllocVertexes(surf.numVerts * surf.numBaseFrames);
                ParseMD3TextureCoords(surf.numVerts, surf.numBaseFrames, ref f, ref surf.vertexes);
                
                // swap all the XyzNormals
                f.Seek(idFileSeekOrigin.FS_SEEK_SET, surfpos + surf.ofsXyzNormals);
                ParseMD3Vertexes(surf.numVerts, surf.numBaseFrames, ref f, ref surf.vertexes);

                // swap all the XyzCompressed
                //xyzComp = (mdcXyzCompressed_t*)((byte*)surf + surf->ofsXyzCompressed);
                f.Seek(idFileSeekOrigin.FS_SEEK_SET, surfpos + surf.ofsXyzCompressed);
                surf.xyzCompressedIndexPool = new mdcXyzCompressed_t[surf.numVerts * surf.numCompFrames];
                for (int j = 0; j < surf.numVerts * surf.numCompFrames; j++)
                {
                    surf.xyzCompressedIndexPool[j] = new mdcXyzCompressed_t();
                    surf.xyzCompressedIndexPool[j].ofsVec = (uint)f.ReadInt();
                }

                // swap the frameBaseFrames
                //ps = (short*)((byte*)surf + surf->ofsFrameBaseFrames);
                f.Seek(idFileSeekOrigin.FS_SEEK_SET, surfpos + surf.ofsFrameBaseFrames);
                surf.baseFrames = new short[header.numFrames];
                for (int j = 0; j < header.numFrames; j++)
                {
                    surf.baseFrames[j] = f.ReadShort();
                }

                // swap the frameCompFrames
                f.Seek(idFileSeekOrigin.FS_SEEK_SET, surfpos + surf.ofsFrameCompFrames);
                surf.compFrames = new short[header.numFrames];
                for (int j = 0; j < header.numFrames; j++)
                {
                    surf.compFrames[j] = f.ReadShort();
                }
                surfpos += surf.ofsEnd;
                surfaces[i] = surf;
            }
        }

        //
        // GetName
        //
        public override string GetName()
        {
            return name;
        }
    }
}
