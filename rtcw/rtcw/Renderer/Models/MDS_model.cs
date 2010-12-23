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

// MDS_Model.cs (c) 2010 JV Software
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
    public class idModelMDS : idModelLocal
    {
        mdsHeader_t header;
        mdsFrame_t[] frames;
        mdsTag_t[] tags;
        mdsSurface_t[] surfaces;
        mdsBoneInfo_t[] bones;

        List<int> collapseMaps = new List<int>();
        List<int> boneRefs = new List<int>();

        List<mdsVertex_t> vertexes = new List<mdsVertex_t>();
        List<short> indexes = new List<short>();

        public const int MDS_IDENT         =  ( ( 'W' << 24 ) + ( 'S' << 16 ) + ( 'D' << 8 ) + 'M' );
        public const int MDS_VERSION       =  4;
        public const int MDS_MAX_VERTS     =  6000;
        public const int MDS_MAX_TRIANGLES =  8192;
        public const int MDS_MAX_BONES    =   128;
        public const int MDS_MAX_SURFACES =   32;
        public const int MDS_MAX_TAGS     =   128;

        public const float MDS_TRANSLATION_SCALE =  ( 1.0f / 64 );
        public const int BONEFLAG_TAG     =   1;       // this bone is actually a tag

        //
        // idModelMDS
        //
        public idModelMDS(ref idFile f)
        {
            header = new mdsHeader_t();
            name = f.GetFullFilePath();

            // Load in the MDS header.
            header.InitFromFile(ref f);
            if (header.numFrames < 1)
            {
                Engine.common.ErrorFatal("R_LoadMDS: has no frames\n");
            }

            // Alloc the frames.
            frames = new mdsFrame_t[header.numFrames];

            // Load in the frames.
            f.Seek(idFileSeekOrigin.FS_SEEK_SET, header.ofsFrames);
            for (int i = 0; i < header.numFrames; i++)
            {
                frames[i].InitFromFile(ref f, header.numBones);
            }

            // Alloc the tags.
            tags = new mdsTag_t[header.numTags];

            // Load in the tags.
            f.Seek(idFileSeekOrigin.FS_SEEK_SET, header.ofsTags);
            for (int i = 0; i < header.numTags; i++)
            {
                tags[i].InitFromFile(ref f);
            }

            // Load in the bones, and alloc
            bones = new mdsBoneInfo_t[header.numBones];
            f.Seek(idFileSeekOrigin.FS_SEEK_SET, header.ofsBones);
            for (int i = 0; i < header.numBones; i++)
            {
                bones[i].InitFromFile(ref f);
            }

            // Alloc the surfaces.
            surfaces = new mdsSurface_t[header.numSurfaces];

            // Load in all the surfaces.
            int surfpos = header.ofsSurfaces;
            for (int i = 0; i < header.numSurfaces; i++)
            {
                mdsSurface_t surf = new mdsSurface_t();

                // Set the read position to the next surface, and parse the surface header.
                f.Seek(idFileSeekOrigin.FS_SEEK_SET, surfpos);
                surf.InitFromFile(ref f);

                // Load in the material for the surface.
                if (surf.shader.Length > 0)
                {
                    surf.materials = new idMaterial[1];
                    surf.materials[0] = Engine.materialManager.FindMaterial(surf.shader, -1);
                }

                // Load in the indexes.
                surf.startIndex = indexes.Count;
                f.Seek(idFileSeekOrigin.FS_SEEK_SET, surfpos + surf.ofsTriangles);
                for (int d = 0; d < surf.numIndexes; d++)
                {
                    indexes.Add((short)f.ReadInt());
                }

                // Load in the vertexes.
                surf.startVertex = vertexes.Count;
                f.Seek(idFileSeekOrigin.FS_SEEK_SET, surfpos + surf.ofsVerts);
                for (int d = 0; d < surf.numVertexes; d++)
                {
                    mdsVertex_t vert = new mdsVertex_t();

                    vert.InitFromFile(ref f);
                    vertexes.Add(vert);
                }

                // Load in the surfaces collapse map.
                surf.startCollapseMap = collapseMaps.Count;
                f.Seek(idFileSeekOrigin.FS_SEEK_SET, surfpos + surf.ofsCollapseMap);
                for (int d = 0; d < surf.numVertexes; d++)
                {
                    collapseMaps.Add(f.ReadInt());
                }

                // Load in the surfaces bone references.
                surf.startBoneRef = boneRefs.Count;
                f.Seek(idFileSeekOrigin.FS_SEEK_SET, surfpos + surf.ofsBoneReferences);
                for (int d = 0; d < surf.numBoneReferences; d++)
                {
                    boneRefs.Add(f.ReadInt());
                }

                surfaces[i] = surf;
                surfpos += surf.ofsEnd;
            }
        }

        //
        // GetNumFrames
        //
        public override int GetNumFrames()
        {
            return 0;
        }

        //
        // GetName
        //
        public override string GetName()
        {
            return name;
        }

        //
        // TessModel
        //
        public override void TessModel(ref idRenderEntityLocal entity)
        {
            
        }
    }
}
