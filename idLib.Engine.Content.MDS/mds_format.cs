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

// MDS_format.cs (c) 2010 JV Software
//

using System;
using System.IO;
using System.Collections.Generic;

using idLib.Math;
using idLib.Engine.Public;

namespace idLib.Engine.Content.MDS
{
    static class idFileHelper
    {
        //
        // ReadVector3
        //
        public static void ReadVector3(ref BinaryReader file, ref idVector3 v)
        {
            v.X = file.ReadSingle();
            v.Y = file.ReadSingle();
            v.Z = file.ReadSingle();
        }

        //
        // WriteVector3
        //
        public static void WriteVector3(ref BinaryWriter file, ref idVector3 v)
        {
            file.Write((float)v.X);
            file.Write((float)v.Y);
            file.Write((float)v.Z);
        }

        //
        // ReadString
        //
        public static string ReadString(ref BinaryReader file, int len)
        {
            byte[] buffer = file.ReadBytes(len);
            string s = "";

            for (int i = 0; i < buffer.Length; i++)
            {
                if ((char)buffer[i] == '\0')
                    break;
                s += (char)buffer[i];
            }

            return s; //ASCIIEncoding.ASCII.GetString(buffer).Trim('\0');
        }
    }
    struct mdsWeight_t
    {
        public int boneIndex;              // these are indexes into the boneReferences,
        public float boneWeight;           // not the global per-frame bone list
        public idVector3 offset;

        public void InitFromFile(ref BinaryReader file)
        {
            boneIndex = file.ReadInt32();
            boneWeight = file.ReadSingle();
            offset = new idVector3();
            offset.X = file.ReadSingle();
            offset.Y = file.ReadSingle();
            offset.Z = file.ReadSingle();
        }

        public const int SIZE = sizeof(byte) + sizeof(float) + idVector3.Size;

        public void WriteToFile(ref BinaryWriter file)
        {
            if(boneIndex >= 256)
            {
                throw new Exception("mdsWeight_t: Bone Index out of range\n" );
            }
            file.Write((byte)boneIndex);
            file.Write(boneWeight);
            file.Write(offset.X);
            file.Write(offset.Y);
            file.Write(offset.Z);
        }
    };

    struct mdsVertex_t
    {
        public idVector3 normal;
        public idVector2 texCoords;
        public int numWeights;
        public int fixedParent;            // stay equi-distant from this parent
        public float fixedDist;
        public mdsWeight_t[] weights;     // variable sized

        public void InitFromFile(ref BinaryReader file)
        {
            normal = new idVector3();
            texCoords = new idVector2();

            normal.X = file.ReadSingle();
            normal.Y = file.ReadSingle();
            normal.Z = file.ReadSingle();

            texCoords.X = file.ReadSingle();
            texCoords.Y = file.ReadSingle();

            numWeights = file.ReadInt32();
            fixedParent = file.ReadInt32();
            fixedDist = file.ReadInt32();

            weights = new mdsWeight_t[4];

            for (int i = 0; i < numWeights; i++)
            {
                weights[i].InitFromFile(ref file);
            }
        }

        const int SIZE = idVector3.Size + idVector2.Size + sizeof(byte);// + sizeof(short);

        public int GetSize()
        {
            return SIZE + (numWeights * mdsWeight_t.SIZE);
        }

        public void WriteToFile(ref BinaryWriter file)
        {
            file.Write((float)normal.X);
            file.Write((float)normal.Y);
            file.Write((float)normal.Z);

            file.Write((float)texCoords.X);
            file.Write((float)texCoords.Y);

            file.Write((byte)numWeights);
            if (fixedParent >= sizeof(short))
            {
                throw new Exception("Fixed parent value out of range.\n");
            }
          //  file.Write((short)fixedParent);
            for (int i = 0; i < numWeights; i++)
            {
                weights[i].WriteToFile(ref file);
            }
        }
    };

    class mdsSurface_t
    {
        public int ident;

        //char name[MAX_QPATH];           // polyset name
        //char shader[MAX_QPATH];
        public string name;
        public string shader;

        public int minLod;

        public int ofsHeader;                  // this will be a negative number

        public int numVertexes;
        public int ofsVerts;

        public int numIndexes;
        public int ofsTriangles;

        public int ofsCollapseMap;           // numVerts * int

        // Bone references are a set of ints representing all the bones
        // present in any vertex weights for this surface.  This is
        // needed because a model may have surfaces that need to be
        // drawn at different sort times, and we don't want to have
        // to re-interpolate all the bones for each surface.
        public int numBoneReferences;
        public int ofsBoneReferences;

        public int ofsEnd;                     // next surface follows

// These are not included in the mds format
        public int startVertex;
        public int startIndex;
        public int startBoneRef;
// end

        //
        // InitFromFile
        //
        public void InitFromFile(ref BinaryReader file)
        {
            ident = file.ReadInt32();
            name = idFileHelper.ReadString(ref file, Engine.Public.Engine.MAX_QPATH);
            shader = idFileHelper.ReadString(ref file, Engine.Public.Engine.MAX_QPATH);

            file.ReadInt32(); // shaderIndex <-- skipped.
            minLod = file.ReadInt32();
            ofsHeader = file.ReadInt32();
            numVertexes = file.ReadInt32();
            ofsVerts = file.ReadInt32();
            numIndexes = file.ReadInt32() * 3; // we read all the indexes not just the triangles.
            ofsTriangles = file.ReadInt32();
            ofsCollapseMap = file.ReadInt32();
            numBoneReferences = file.ReadInt32();
            ofsBoneReferences = file.ReadInt32();
            ofsEnd = file.ReadInt32();
        }

        public int GetSize()
        {
            return (name.Length + 1) + (shader.Length + 1) + (sizeof(int) * 2) + (sizeof(short) * 3) + sizeof(short);
        }

        public int GetSizeOfIndexes()
        {
            return numIndexes * sizeof(short);
        }

        public int GetSizeOfVertexes(ref List<mdsVertex_t> verts)
        {
            int size = 0;
            for (int i = startVertex; i < startVertex + numVertexes; i++)
            {
                size += verts[i].GetSize();
            }

            return size;
        }

        public void WriteToFile(ref BinaryWriter file)
        {
            file.Write(name);
            file.Write(shader);
            file.Write((short)numVertexes);
            //file.Write(ofsVerts);
            file.Write((short)(numIndexes / 3));
            file.Write((short)numBoneReferences);
            //file.Write(ofsBoneReferences);
            //file.Write((short)ofsEnd);
        }
    };

    struct mdsBoneFrameCompressed_t
    {
        public short[] angles;            // to be converted to axis at run-time (this is also better for lerping)
        public short[] ofsAngles;         // PITCH/YAW, head in this direction from parent to go to the offset position

        //
        // InitFromFile
        //
        public void InitFromFile(ref BinaryReader file)
        {
            angles = new short[4];
            ofsAngles = new short[2];
            for (int i = 0; i < 4; i++)
            {
                angles[i] = file.ReadInt16();
            }

            for (int i = 0; i < 2; i++)
            {
                ofsAngles[i] = file.ReadInt16();
            }
        }

        //
        // WriteToFile
        //
        public void WriteToFile(ref BinaryWriter file)
        {
            for (int i = 0; i < 4; i++)
            {
                file.Write(angles[i]);
            }

            for (int i = 0; i < 2; i++)
            {
                file.Write(ofsAngles[i]);
            }
        }
    };

    struct mdsFrame_t
    {
        public idBounds bounds;              // bounds of all surfaces of all LOD's for this frame
        public idVector3 localOrigin;             // midpoint of bounds, used for sphere cull
        public float radius;                   // dist from localOrigin to corner
        public idVector3 parentOffset;            // one bone is an ascendant of all other bones, it starts the hierachy at this position
        public mdsBoneFrameCompressed_t[] bones;              // [numBones]

        //
        // InitFromFile
        // 
        public void InitFromFile(ref BinaryReader file, int numBones)
        {
            idVector3 mins = new idVector3();
            idVector3 maxs = new idVector3();

            localOrigin = new idVector3();
            parentOffset = new idVector3();

            idFileHelper.ReadVector3(ref file, ref mins);
            idFileHelper.ReadVector3(ref file, ref maxs);
            bounds = new idBounds(mins, maxs);
            idFileHelper.ReadVector3(ref file, ref localOrigin);
            radius = file.ReadSingle();
            idFileHelper.ReadVector3(ref file, ref parentOffset);

            bones = new mdsBoneFrameCompressed_t[numBones];
            for (int i = 0; i < numBones; i++)
            {
                bones[i].InitFromFile(ref file);
            }
        }

        //
        // WriteBounds
        //
        public void WriteBounds(ref BinaryWriter file)
        {
            for (int i = 0; i < 3; i++)
            {
                file.Write((short)bounds.Mins[i]);
            }

            for (int i = 0; i < 3; i++)
            {
                file.Write((short)bounds.Maxs[i]);
            }

            file.Write((float)radius);
        }

        //
        // WriteToFile
        //
        public void WriteToFile(ref BinaryWriter file)
        {
            
            idFileHelper.WriteVector3(ref file, ref parentOffset);
            for (int i = 0; i < bones.Length; i++)
            {
                bones[i].WriteToFile(ref file);
            }
        }
    };

    struct mdsTag_t
    {
        //char name[MAX_QPATH];           // name of tag
        public string name;
        public float torsoWeight;
        public int boneIndex;                  // our index in the bones

        public void InitFromFile(ref BinaryReader file)
        {
            name = idFileHelper.ReadString(ref file, Engine.Public.Engine.MAX_QPATH);
            torsoWeight = file.ReadSingle();
            boneIndex = file.ReadInt32();
        }

        public void WriteToFile(ref BinaryWriter file)
        {
            file.Write(name);
            file.Write(torsoWeight);
            file.Write((byte)boneIndex);
        }
    };

    struct mdsBoneInfo_t
    {
        //char name[MAX_QPATH];           // name of bone
        public string name;
        public int parent;                     // not sure if this is required, no harm throwing it in
        public float torsoWeight;              // scale torso rotation about torsoParent by this
        public float parentDist;
        public int flags;

        public void InitFromFile(ref BinaryReader file)
        {
            name = idFileHelper.ReadString(ref file, Engine.Public.Engine.MAX_QPATH);
            parent = file.ReadInt32();
            torsoWeight = file.ReadSingle();
            parentDist = file.ReadSingle();
            flags = file.ReadInt32();
        }

        public void WriteToFile(ref BinaryWriter file)
        {
            file.Write(name);
            if (parent >= 255)
            {
                throw new Exception("mdsBoneInfo_t parent out of range: " + parent);
            }
            file.Write((byte)parent);
            file.Write((float)torsoWeight);
            file.Write((float)parentDist);
            file.Write((byte)flags);
        }
    };

    struct mdsHeader_t
    {
        public int ident;
        public int version;

        public string name; //char name[MAX_QPATH];           // model name

        public float lodScale;
        public float lodBias;

        // frames and bones are shared by all levels of detail
        public int numFrames;
        public int numBones;
        public int ofsFrames;                  // md4Frame_t[numFrames]
        public int ofsBones;                   // mdsBoneInfo_t[numBones]
        public int torsoParent;                // index of bone that is the parent of the torso

        public int numSurfaces;
        public int ofsSurfaces;

        // tag data
        public int numTags;
        public int ofsTags;                    // mdsTag_t[numTags]

        public int ofsEnd;                     // end of file

        //
        // InitFromFile
        //
        public void InitFromFile(ref BinaryReader file)
        {
            // Load the version first becuase the ident should already have been read.
            ident = file.ReadInt32();
            version = file.ReadInt32();

            name = idFileHelper.ReadString(ref file, Engine.Public.Engine.MAX_QPATH);
            lodScale = file.ReadSingle();
            lodBias = file.ReadSingle();

            numFrames = file.ReadInt32();
            numBones = file.ReadInt32();
            ofsFrames = file.ReadInt32();
            ofsBones = file.ReadInt32();
            torsoParent = file.ReadInt32();

            numSurfaces = file.ReadInt32();
            ofsSurfaces = file.ReadInt32();

            numTags = file.ReadInt32();
            ofsTags = file.ReadInt32();

            ofsEnd = file.ReadInt32();
        }

        public void WriteToFile(ref BinaryWriter file)
        {
            file.Write(ident);
            file.Write((short)numFrames);
            file.Write((byte)numBones);
          //  file.Write(ofsFrames);
         //   file.Write(ofsBones);
            file.Write((byte)torsoParent);
            file.Write((short)numSurfaces);
         //   file.Write(ofsSurfaces);
            file.Write((short)numTags);
        //    file.Write(ofsTags);
        //    file.Write(ofsEnd);
        }
    };
}
