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
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

using idLib;
using idLib.Engine.Public;
using idLib.Math;

namespace rtcw.Renderer.Models
{
    struct mdsWeight_t
    {
        public int boneIndex;              // these are indexes into the boneReferences,
        public float boneWeight;           // not the global per-frame bone list
        public idVector3 offset;

        public void InitFromFile(ref idFile file)
        {
            boneIndex = file.ReadInt();
            boneWeight = file.ReadFloat();
            offset = new idVector3();
            file.ReadVector3(ref offset);
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

        public void InitFromFile(ref idFile file)
        {
            normal = new idVector3();
            texCoords = new idVector2();

            file.ReadVector3(ref normal);
            file.ReadVector2(ref texCoords);
            numWeights = file.ReadInt();
            fixedParent = file.ReadInt();
            fixedDist = file.ReadInt();

            weights = new mdsWeight_t[numWeights];

            for (int i = 0; i < numWeights; i++)
            {
                weights[i].InitFromFile(ref file);
            }
        }
    };

    class mdsSurface_t : idDrawSurface
    {
        public int ident;

        //char name[MAX_QPATH];           // polyset name
        //char shader[MAX_QPATH];
        public string name;
        public string shader;
        //public int shaderIndex;                // for in-game use

        public int minLod;

        public int ofsHeader;                  // this will be a negative number

        //public int numVerts;
        public int ofsVerts;

        //public int numTriangles;
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

// jv - added these are NOT part of the MDS format, stored for internal usage.
        public int startCollapseMap;
        public int startBoneRef;
// jv end

        //
        // InitFromFile
        //
        public void InitFromFile(ref idFile file)
        {
            ident = file.ReadInt();
            name = file.ReadString(Engine.MAX_QPATH);
            shader = file.ReadString(Engine.MAX_QPATH);

            file.ReadInt(); // shaderIndex <-- skipped.
            minLod = file.ReadInt();
            ofsHeader = file.ReadInt();
            numVertexes = file.ReadInt();
            ofsVerts = file.ReadInt();
            numIndexes = file.ReadInt() * 3; // we read all the indexes not just the triangles.
            ofsTriangles = file.ReadInt();
            ofsCollapseMap = file.ReadInt();
            numBoneReferences = file.ReadInt();
            ofsBoneReferences = file.ReadInt();
            ofsEnd = file.ReadInt();
        }
    };

    struct mdsBoneFrameCompressed_t
    {
        public short[] angles;            // to be converted to axis at run-time (this is also better for lerping)
        public short[] ofsAngles;         // PITCH/YAW, head in this direction from parent to go to the offset position

        //
        // InitFromFile
        //
        public void InitFromFile(ref idFile file)
        {
            angles = new short[4];
            ofsAngles = new short[2];
            for (int i = 0; i < 4; i++)
            {
                angles[i] = file.ReadShort();
            }

            for (int i = 0; i < 2; i++)
            {
                ofsAngles[i] = file.ReadShort();
            }
        }
    };

    // NOTE: this only used at run-time
    class mdsBoneFrame_t
    {
        //float matrix[3][3];             // 3x3 rotation
        public idMatrix matrix = new idMatrix();
        public idVector3 translation = new idVector3();             // translation vector
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
        public void InitFromFile(ref idFile file, int numBones)
        {
            idVector3 mins = new idVector3();
            idVector3 maxs = new idVector3();

            localOrigin = new idVector3();
            parentOffset = new idVector3();

            file.ReadVector3(ref mins);
            file.ReadVector3(ref maxs);
            bounds = new idBounds(mins, maxs);
            file.ReadVector3(ref localOrigin);
            radius = file.ReadFloat();
            file.ReadVector3(ref parentOffset);

            bones = new mdsBoneFrameCompressed_t[numBones];
            for (int i = 0; i < numBones; i++)
            {
                bones[i].InitFromFile(ref file);
            }
        }
    };

    struct mdsTag_t
    {
        //char name[MAX_QPATH];           // name of tag
        public string name;
        public float torsoWeight;
        public int boneIndex;                  // our index in the bones

        public void InitFromFile(ref idFile file)
        {
            name = file.ReadString(Engine.MAX_QPATH);
            torsoWeight = file.ReadFloat();
            boneIndex = file.ReadInt();
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

        public void InitFromFile(ref idFile file)
        {
            name = file.ReadString(Engine.MAX_QPATH);
            parent = file.ReadInt();
            torsoWeight = file.ReadFloat();
            parentDist = file.ReadFloat();
            flags = file.ReadInt();
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
        public void InitFromFile(ref idFile file)
        {
            // Load the version first becuase the ident should already have been read.
            version = file.ReadInt();
            if (version != idModelMDS.MDS_VERSION)
            {
                Engine.common.ErrorFatal("R_LoadMDS: MDS Version invalid expected %d but found %d \n", idModelMDS.MDS_VERSION, version);
                return;
            }

            name = file.ReadString(Engine.MAX_QPATH);
            lodScale = file.ReadFloat();
            lodBias = file.ReadFloat();

            numFrames = file.ReadInt();
            numBones = file.ReadInt();
            ofsFrames = file.ReadInt();
            ofsBones = file.ReadInt();
            torsoParent = file.ReadInt();

            numSurfaces = file.ReadInt(); 
            ofsSurfaces = file.ReadInt();

            numTags = file.ReadInt();
            ofsTags = file.ReadInt();

            ofsEnd = file.ReadInt();
        }
    };
}
