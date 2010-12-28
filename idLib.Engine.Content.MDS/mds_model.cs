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
using System.IO;
using System.Collections.Generic;
using idLib.Engine.Public;

namespace idLib.Engine.Content.MDS
{
    //
    // idModelMDS
    //
    public class idModelMDS
    {
        mdsHeader_t header;
        mdsFrame_t[] frames;
        mdsTag_t[] tags;
        mdsSurface_t[] surfaces;
        mdsBoneInfo_t[] boneInfo;

        List<int> collapseMaps = new List<int>();
        List<int> boneRefsList = new List<int>();
        int[] boneRefs;

        List<mdsVertex_t> vertexes = new List<mdsVertex_t>();
        List<short> indexes = new List<short>();

        public const int MDS_IDENT = (('W' << 24) + ('S' << 16) + ('D' << 8) + 'M');
        public const int MDS_VERSION = 4;
        public const int MDS_MAX_VERTS = 6000;
        public const int MDS_MAX_TRIANGLES = 8192;
        public const int MDS_MAX_BONES = 75;
        public const int MDS_MAX_SURFACES = 32;
        public const int MDS_MAX_TAGS = 128;

        public const float MDS_TRANSLATION_SCALE = (1.0f / 64);
        public const int BONEFLAG_TAG = 1;       // this bone is actually a tag

        private int baseSurfaceOffset = 0;

        //
        // CalcSurfaceOffset
        //
        public void CalcSurfaceOffset(ref int offset, int len)
        {
            baseSurfaceOffset += len;
            offset = baseSurfaceOffset;
        }

        //
        // WriteMDSBinary
        //
        public void WriteMDSBinary(ref BinaryWriter f)
        {
            int startpos = (int)f.BaseStream.Position;

            // Write out the header, offsets will get fixed as we go on than we will re-write the header.
            header.WriteToFile(ref f);

            f.Write("DATA");

            // Write out the bounds for ONE frame only to save space.
            frames[0].WriteBounds(ref f);

            // Write out the frames.
            for (int i = 0; i < header.numFrames; i++)
            {
                frames[i].WriteToFile(ref f);
            }

            // Write out the tags.
            for (int i = 0; i < header.numTags; i++)
            {
                tags[i].WriteToFile(ref f);
            }

            // Write out the bones.
            for (int i = 0; i < header.numBones; i++)
            {
                boneInfo[i].WriteToFile(ref f);
            }

            // Write out all the surfaces.
            for (int i = 0; i < header.numSurfaces; i++)
            {
                // Calculate the offsets for the surface before we write it out.
                baseSurfaceOffset = 0;
                f.Write("SURF");
                /*
                CalcSurfaceOffset(ref surfaces[i].ofsTriangles, surfaces[i].GetSize());
                CalcSurfaceOffset(ref surfaces[i].ofsVerts, surfaces[i].GetSizeOfIndexes());
                CalcSurfaceOffset(ref surfaces[i].ofsBoneReferences, surfaces[i].GetSizeOfVertexes(ref vertexes));
                baseSurfaceOffset += surfaces[i].numBoneReferences * sizeof(byte);
                surfaces[i].ofsEnd = baseSurfaceOffset;
                */

                // Write out the surface header.
                surfaces[i].WriteToFile(ref f);

                // Write out the indexes.
                for (int d = surfaces[i].startIndex; d < surfaces[i].startIndex + surfaces[i].numIndexes; d++)
                {
                    f.Write((short)(indexes[d]));
                }

                // Write out the vertexes.
                for (int d = surfaces[i].startVertex; d < surfaces[i].startVertex + surfaces[i].numVertexes; d++)
                {
                    vertexes[d].WriteToFile(ref f);
                }

                for (int d = surfaces[i].startBoneRef; d < surfaces[i].startBoneRef + surfaces[i].numBoneReferences; d++)
                {
                    if (boneRefs[d] >= header.numBones)
                    {
                        throw new Exception("Bone ref out of range");
                    }

                    f.Write((byte)boneRefs[d]);
                }
            }

            f.BaseStream.Position = startpos;
            header.WriteToFile(ref f);
        }

        //
        // idModelMDS
        //
        public idModelMDS(BinaryReader f)
        {
            mdsVertex_t mdsVertex = new mdsVertex_t();

            header = new mdsHeader_t();

            // Load in the MDS header.
            header.InitFromFile(ref f);
            if (header.numFrames < 1)
            {
                throw new Exception("R_LoadMDS: has no frames\n");
            }

            // Alloc the frames.
            frames = new mdsFrame_t[header.numFrames];

            // Load in the frames.
            f.BaseStream.Position = header.ofsFrames;
            for (int i = 0; i < header.numFrames; i++)
            {
                frames[i].InitFromFile(ref f, header.numBones);
            }

            // Alloc the tags.
            tags = new mdsTag_t[header.numTags];

            // Load in the tags.
            f.BaseStream.Position = header.ofsTags;
            for (int i = 0; i < header.numTags; i++)
            {
                tags[i].InitFromFile(ref f);
            }

            if (header.numBones >= MDS_MAX_BONES)
            {
                throw new Exception("Model has too many bones...\n");
            }

            // Load in the bones, and alloc
            boneInfo = new mdsBoneInfo_t[header.numBones];
            f.BaseStream.Position = header.ofsBones;
            for (int i = 0; i < header.numBones; i++)
            {
                boneInfo[i].InitFromFile(ref f);
            }

            // Alloc the surfaces.
            surfaces = new mdsSurface_t[header.numSurfaces];

            // Load in all the surfaces.
            int surfpos = header.ofsSurfaces;
            for (int i = 0; i < header.numSurfaces; i++)
            {
                mdsSurface_t surf = new mdsSurface_t();

                // Set the read position to the next surface, and parse the surface header.
                f.BaseStream.Position = surfpos;
                surf.InitFromFile(ref f);

                // Load in the indexes.
                surf.startIndex = indexes.Count;
                f.BaseStream.Position = surfpos + surf.ofsTriangles;
                for (int d = 0; d < surf.numIndexes; d++)
                {
                    indexes.Add((short)f.ReadInt32());
                }

                // Load in the vertexes.
                surf.startVertex = vertexes.Count;
                f.BaseStream.Position = surfpos + surf.ofsVerts;
                for (int d = 0; d < surf.numVertexes; d++)
                {
                    mdsVertex_t vert = new mdsVertex_t();

                    vert.InitFromFile(ref f);

                    vertexes.Add(vert);
                }

                // Load in the surfaces collapse map.
                /*
                surf.startCollapseMap = collapseMaps.Count;
                f.Seek(idFileSeekOrigin.FS_SEEK_SET, surfpos + surf.ofsCollapseMap);
                for (int d = 0; d < surf.numVertexes; d++)
                {
                    collapseMaps.Add(f.ReadInt());
                }
                */

                // Load in the surfaces bone references.
                surf.startBoneRef = boneRefsList.Count;
                f.BaseStream.Position = surfpos + surf.ofsBoneReferences;
                for (int d = 0; d < surf.numBoneReferences; d++)
                {
                    boneRefsList.Add(f.ReadInt32());
                }

                surfaces[i] = surf;
                surfpos += surf.ofsEnd;
            }

            boneRefs = boneRefsList.ToArray();
            boneRefsList.Clear();
            boneRefsList = null;

            f.Close();
        }
    }
}