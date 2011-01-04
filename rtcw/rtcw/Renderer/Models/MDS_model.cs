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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using idLib.Engine.Public;
using idLib.Math;

using rtcw.Renderer.Backend;
using rtcw.Framework.Files;

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
        mdsBoneInfo_t[] boneInfo;

        List<int> collapseMaps = new List<int>();
        List<int> boneRefsList = new List<int>();
        int[] boneRefs;

        List<idDrawVertexSkin> vertexes = new List<idDrawVertexSkin>();
        List<short> indexes = new List<short>();

        idDrawVertex[] drawVertexes;

        public const int MDS_IDENT         =  ( ( 'W' << 24 ) + ( 'S' << 16 ) + ( 'D' << 8 ) + 'M' );
        public const int MDS_VERSION       =  4;
        public const int MDS_MAX_VERTS     =  6000;
        public const int MDS_MAX_TRIANGLES =  8192;
        public const int MDS_MAX_BONES    =   75;
        public const int MDS_MAX_SURFACES =   32;
        public const int MDS_MAX_TAGS     =   128;

        public const float MDS_TRANSLATION_SCALE =  ( 1.0f / 64 );
        public const int BONEFLAG_TAG     =   1;       // this bone is actually a tag

        //
        // MDSVertexToDrawVertex
        //
        private void MDSVertexToDrawVertex(mdsVertex_t mdsVertex, ref idDrawVertexSkin skinVertex)
        {
            // First copy over all the weights, all skinned vertexes have four weights, any trailing weights are one.
           // skinVertex.blendIndices = new int[4];
            for (int i = 0; i < 4; i++)
            {
                if (i >= mdsVertex.numWeights)
                {
                    skinVertex.weights[i] = 0.0f;
                    skinVertex.blendIndices[i] = 0;
                }
                else
                {
                    skinVertex.blendIndices[i] = mdsVertex.weights[i].boneIndex;
                    skinVertex.weights[i] = mdsVertex.weights[i].boneWeight;
                }
            }

            skinVertex.offset1 = mdsVertex.weights[0].offset;
            skinVertex.offset2 = mdsVertex.weights[1].offset;
            skinVertex.offset3 = mdsVertex.weights[2].offset;
            skinVertex.offset4 = mdsVertex.weights[3].offset;

            skinVertex.st = mdsVertex.texCoords;
          //  skinVertex.normal = mdsVertex.normal;
        }

        //
        // idModelMDS
        //
        public idModelMDS(ref idFile f)
        {
            mdsVertex_t mdsVertex = new mdsVertex_t();

            header = new mdsHeader_t();
            name = f.GetFullFilePath();

            // Load in the MDS header.
            header.InitFromFile(ref f);
            if (header.numFrames < 1)
            {
                Engine.common.ErrorFatal("R_LoadMDS: has no frames\n");
            }

            if (f.ReadString() != "DATA")
            {
                Engine.common.ErrorFatal("R_LoadMDS: Data string not present\n");
            }

            // Alloc the frames.
            frames = new mdsFrame_t[header.numFrames];

            frames[0].ReadBounds(ref f);

            // Load in the frames.
            //f.Seek(idFileSeekOrigin.FS_SEEK_SET, header.ofsFrames);
            for (int i = 0; i < header.numFrames; i++)
            {
                frames[i].InitFromFile(ref f, header.numBones);
            }

            // Alloc the tags.
            tags = new mdsTag_t[header.numTags];

            // Load in the tags.
           // f.Seek(idFileSeekOrigin.FS_SEEK_SET, header.ofsTags);
            for (int i = 0; i < header.numTags; i++)
            {
                tags[i].InitFromFile(ref f);
            }

            if (header.numBones >= MDS_MAX_BONES)
            {
                Engine.common.ErrorFatal("Model has too many bones...\n");
            }

            // Load in the bones, and alloc
            boneInfo = new mdsBoneInfo_t[header.numBones];
           // f.Seek(idFileSeekOrigin.FS_SEEK_SET, header.ofsBones);
            for (int i = 0; i < header.numBones; i++)
            {
                boneInfo[i].InitFromFile(ref f);
            }

            // Alloc the surfaces.
            surfaces = new mdsSurface_t[header.numSurfaces];

            // Load in all the surfaces.
           // int surfpos = header.ofsSurfaces;
            for (int i = 0; i < header.numSurfaces; i++)
            {
                mdsSurface_t surf = new mdsSurface_t();

                if (f.ReadString() != "SURF")
                {
                    Engine.common.ErrorFatal("R_LoadMDS: Surf string not present\n");
                }

                // Set the read position to the next surface, and parse the surface header.
               // f.Seek(idFileSeekOrigin.FS_SEEK_SET, surfpos);
                surf.InitFromFile(ref f);

                // Load in the material for the surface.
                surf.materials = new idMaterial[1];
                if (surf.shader.Length > 0)
                {
                    surf.materials[0] = Engine.materialManager.FindMaterial(surf.shader, -1);
                }

                // Load in the indexes.
                surf.startIndex = indexes.Count;
             //   f.Seek(idFileSeekOrigin.FS_SEEK_SET, surfpos + surf.ofsTriangles);
                for (int d = 0; d < surf.numIndexes; d++)
                {
                    indexes.Add(f.ReadShort());
                }

                // Load in the vertexes.
                surf.startVertex = vertexes.Count;
             //   f.Seek(idFileSeekOrigin.FS_SEEK_SET, surfpos + surf.ofsVerts);
                for (int d = 0; d < surf.numVertexes; d++)
                {
                    idDrawVertexSkin vert = new idDrawVertexSkin();

                    mdsVertex.InitFromFile(ref f);

                    // We have to convert the model vertex into a skinned vertex for GPU skinning.
                    MDSVertexToDrawVertex(mdsVertex, ref vert);
                    
                    vertexes.Add(vert);
                }

                /*
                // Load in the surfaces collapse map.
                surf.startCollapseMap = collapseMaps.Count;
                f.Seek(idFileSeekOrigin.FS_SEEK_SET, surfpos + surf.ofsCollapseMap);
                for (int d = 0; d < surf.numVertexes; d++)
                {
                    collapseMaps.Add(f.ReadInt());
                }
                */
                
                // Load in the surfaces bone references.
                surf.startBoneRef = boneRefsList.Count;
              //  f.Seek(idFileSeekOrigin.FS_SEEK_SET, surfpos + surf.ofsBoneReferences);
                for (int d = 0; d < surf.numBoneReferences; d++)
                {
                    boneRefsList.Add(f.ReadByte());
                }

                surfaces[i] = surf;
              //  surfpos += surf.ofsEnd;
            }

            boneRefs = boneRefsList.ToArray();
            boneRefsList.Clear();
            boneRefsList = null;

            drawVertexes = new idDrawVertex[vertexes.Count];

            vertexBuffer = new VertexBuffer(Globals.graphics3DDevice, idRenderGlobals.idDrawVertexDeclaration, vertexes.Count, BufferUsage.WriteOnly);
            indexBuffer = new IndexBuffer(Globals.graphics3DDevice, IndexElementSize.SixteenBits, indexes.Count, BufferUsage.WriteOnly);
            indexBuffer.SetData<short>(indexes.ToArray());

            for (int i = 0; i < vertexes.Count; i++)
            {
                drawVertexes[i] = new idDrawVertex();
            }

            // Build our vertex and index buffers.
            //BuildVertexIndexBuffer();


        }

        //
        // GetNumFrames
        //
        public override int GetNumFrames()
        {
            return header.numFrames;
        }

        /*
        ===============
        RecursiveBoneListAdd
        ===============
        */
        private void RecursiveBoneListAdd(int bi, ref int[] boneList, ref int numBones)
        {

            if (boneInfo[bi].parent >= 0)
            {

                RecursiveBoneListAdd(boneInfo[bi].parent, ref boneList, ref numBones);

            }

            boneList[numBones++] = bi;

        }

        //
        // GetTag
        //
        public override int GetTag(string name, int startframe, int endframe, int index, ref idOrientation orientation)
        {
            int i;
            int boneIndex = -1;
            int numBones = 0;

	        if ( index > header.numTags ) {
		        return -1;
	        }

	        // find the correct tag
            


	        for ( i = index; i < header.numTags; i++ ) {
		        if ( tags[i].name == name ) {
			        break;
		        }
	        }

	        if ( i >= header.numTags ) {
		        return -1;
	        }

            // now build the list of bones we need to calc to get this tag's bone information
            RecursiveBoneListAdd(tags[i].boneIndex, ref boneRefsTags, ref numBones);

	        // calc the bones
	        CalcBones(startframe, startframe, 0, numBones, boneRefsTags );

	        // now extract the orientation for the bone that represents our tag
            boneIndex = tags[i].boneIndex;
            orientation.axis.Set(bones[boneIndex]);
            orientation.origin.X = bones[boneIndex].M41;
            orientation.origin.Y = bones[boneIndex].M42;
            orientation.origin.Z = bones[boneIndex].M43;

            return i;
        }

        //
        // GetName
        //
        public override string GetName()
        {
            return name;
        }

        // What the hell?
        private Matrix[] bones = new Matrix[MDS_MAX_BONES];
        int lastTorsoBone = 0;
        private int[] boneRefsTags = new int[MDS_MAX_BONES];
        private static mdsBoneFrame_t nullframe;
        private static mdsBoneInfo_t nullBoneInfo;

        private static idVector3 angles = new idVector3();
        private static idVector3 tangles = new idVector3();
        private static idVector3 vec = new idVector3();
        private static idVector3 v2 = new idVector3();
        private static idVector3 torsoParentOffset = new idVector3();
        private static idVector3 parentBoneTranslation = new idVector3();
        private static idVector3 boneTranslation = new idVector3();

        //
        // CalcBone
        //
        private void CalcBone(idVector3 parentOffset, mdsBoneFrameCompressed_t[] cBoneList, mdsBoneFrameCompressed_t[] cBoneListTorso, int boneNum)
        {
            mdsBoneInfo_t thisBoneInfo = boneInfo[boneNum];
            bool isTorso = false;
            bool fullTorso = false;
            mdsBoneFrameCompressed_t cTBonePtr, cBonePtr;
            mdsBoneInfo_t parentBoneInfo = nullBoneInfo;
            
            float diff;

            if (thisBoneInfo.torsoWeight != 0)
            {
                lastTorsoBone = boneNum;
                isTorso = true;
                if (thisBoneInfo.torsoWeight == 1.0f)
                {
                    fullTorso = true;
                }
            }
            else
            {
                isTorso = false;
                fullTorso = false;
            }

            cTBonePtr = cBoneListTorso[lastTorsoBone];
            cBonePtr = cBoneList[boneNum];

            // we can assume the parent has already been uncompressed for this frame + lerp
            if (thisBoneInfo.parent >= 0)
            {
                parentBoneTranslation.X = bones[thisBoneInfo.parent].M41;
                parentBoneTranslation.Y = bones[thisBoneInfo.parent].M42;
                parentBoneTranslation.Z = bones[thisBoneInfo.parent].M43;
                parentBoneInfo = boneInfo[thisBoneInfo.parent];
            }

            // rotation
	        if ( fullTorso ) {
		        SkeletalMath.ANGLES_SHORT_TO_FLOAT( ref angles, cTBonePtr.angles );
	        } else {
		        SkeletalMath.ANGLES_SHORT_TO_FLOAT( ref angles, cBonePtr.angles );
		        if ( isTorso ) {
			        SkeletalMath.ANGLES_SHORT_TO_FLOAT( ref tangles, cTBonePtr.angles );

			        // blend the angles together
			        for ( int j = 0; j < 3; j++ ) {
				        diff = tangles[j] - angles[j];
				        if ( System.Math.Abs( diff ) > 180 ) {
					        diff = SkeletalMath.AngleNormalize180( diff );
				        }
				        angles[j] = angles[j] + thisBoneInfo.torsoWeight * diff;
			        }
		        }
	        }

            bones[boneNum] = angles.ToAxis();


            // translation
            if(thisBoneInfo.parent >= 0)
            {
                if ( fullTorso ) {
                    angles.X = SkeletalMath.SHORT2ANGLE(cTBonePtr.ofsAngles[0]);
                    angles.Y = SkeletalMath.SHORT2ANGLE(cTBonePtr.ofsAngles[1]);
                    angles.Z = 0;

			        SkeletalMath.LocalAngleVector( angles, ref vec );
                    SkeletalMath.LocalVectorMA(parentBoneTranslation, thisBoneInfo.parentDist, vec, ref boneTranslation);

                    bones[boneNum].M41 = boneTranslation.X;
                    bones[boneNum].M42 = boneTranslation.Y;
                    bones[boneNum].M43 = boneTranslation.Z;
		        } else {

			        angles.X = SkeletalMath.SHORT2ANGLE(cBonePtr.ofsAngles[0]);
                    angles.Y = SkeletalMath.SHORT2ANGLE(cBonePtr.ofsAngles[1]);
                    angles.Z = 0;

			        SkeletalMath.LocalAngleVector( angles, ref vec );

			        if ( isTorso ) {
				        tangles.X = SkeletalMath.SHORT2ANGLE(cTBonePtr.ofsAngles[0]);
                        tangles.Y = SkeletalMath.SHORT2ANGLE(cTBonePtr.ofsAngles[1]);
                        tangles.Z = 0;

				        SkeletalMath.LocalAngleVector( tangles, ref v2 );

				        // blend the angles together
				        SkeletalMath.SLerp_Normal( vec, v2, thisBoneInfo.torsoWeight, ref vec );
                        SkeletalMath.LocalVectorMA(parentBoneTranslation, thisBoneInfo.parentDist, vec, ref boneTranslation);
                        bones[boneNum].M41 = boneTranslation.X;
                        bones[boneNum].M42 = boneTranslation.Y;
                        bones[boneNum].M43 = boneTranslation.Z;

			        } else {    // legs bone
                        SkeletalMath.LocalVectorMA(parentBoneTranslation, thisBoneInfo.parentDist, vec, ref boneTranslation);
                        bones[boneNum].M41 = boneTranslation.X;
                        bones[boneNum].M42 = boneTranslation.Y;
                        bones[boneNum].M43 = boneTranslation.Z;
			        }
		        }
            }
            else
            {
                bones[boneNum].M41 = parentOffset[0];
                bones[boneNum].M42 = parentOffset[1];
                bones[boneNum].M43 = parentOffset[2];
            }

            if (boneNum == header.torsoParent)
            { // this is the torsoParent

                torsoParentOffset.X = bones[boneNum].M41;
                torsoParentOffset.Y = bones[boneNum].M42;
                torsoParentOffset.Z = bones[boneNum].M43;
            }
        }

        //
        // CalcBones
        //
        private void CalcBones(int frameNum, int torsoFrameNum, int startBone, int numBones, int[] boneRefList)
        {
            mdsFrame_t frame = frames[frameNum];
            mdsFrame_t torsoFrame = frames[torsoFrameNum];
            idVector3 t;
            idMatrix m1, m2;

            for (int i = 0; i < numBones; i++)
            {
                int boneRef = boneRefList[startBone + i];

                // find our parent, and make sure it has been calculated
                if (boneInfo[boneRef].parent >= 0 )
                {
                    CalcBone(frame.parentOffset, frame.bones, torsoFrame.bones, boneInfo[boneRef].parent);
                }
                CalcBone(frame.parentOffset, frame.bones, torsoFrame.bones, boneRef);
            }

            // adjust for torso rotations
            float torsoWeight = 0;
            /*
            for (int i = 0; i < numBones; i++)
            {
                int boneRef = boneRefs[i];
                mdsBoneInfo_t thisBoneInfo = boneInfo[boneRef];

                // add torso rotation
                if (thisBoneInfo.torsoWeight > 0)
                {
                    if ((thisBoneInfo.flags & BONEFLAG_TAG) == 0)
                    {

                        // 1st multiply with the bone->matrix
                        // 2nd translation for rotation relative to bone around torso parent offset
                        t = bones[boneRef].translation - torsoParentOffset;

                        m1.SetFromAxisAndTranslation(bones[boneRef].matrix, t);
                        // 3rd scaled rotation
                        // 4th translate back to torso parent offset
                        // use previously created matrix if available for the same weight
                        if (torsoWeight != thisBoneInfo.torsoWeight)
                        {
                            m2.SetFromScaledAxisAndTranslation(torsoAxis, thisBoneInfo.torsoWeight);
                            torsoWeight = thisBoneInfo.torsoWeight;
                        }
                        // multiply matrices to create one matrix to do all calculations
                        Matrix4MultiplyInto3x3AndTranslation(m2, m1, bonePtr->matrix, bonePtr->translation);

                    }
                    else
                    {    // tag's require special handling

                        // rotate each of the axis by the torsoAngles
                        LocalScaledMatrixTransformVector(bonePtr->matrix[0], thisBoneInfo->torsoWeight, torsoAxis, tmpAxis[0]);
                        LocalScaledMatrixTransformVector(bonePtr->matrix[1], thisBoneInfo->torsoWeight, torsoAxis, tmpAxis[1]);
                        LocalScaledMatrixTransformVector(bonePtr->matrix[2], thisBoneInfo->torsoWeight, torsoAxis, tmpAxis[2]);
                        memcpy(bonePtr->matrix, tmpAxis, sizeof(tmpAxis));

                        // rotate the translation around the torsoParent
                        VectorSubtract(bonePtr->translation, torsoParentOffset, t);
                        LocalScaledMatrixTransformVector(t, thisBoneInfo->torsoWeight, torsoAxis, bonePtr->translation);
                        VectorAdd(bonePtr->translation, torsoParentOffset, bonePtr->translation);

                    }
                }
            }
            */
        }

        //
        // BuildVertexIndexBuffer
        //
        public override void BuildVertexIndexBuffer()
        {
            Globals.graphics3DDevice.SetVertexBuffer(null);
            vertexBuffer.SetData<idDrawVertex>(drawVertexes);
        }

        //
        // SetBoneBuffer
        //
        private void SetBoneBuffer()
        {
            idRenderCommand cmd = Globals.backEnd.GetCommandBuffer();

            cmd.type = renderCommandType.RC_SET_BONEMATRIX;
            cmd.bones = bones;
            //cmd.vertexBuffer = vertexBuffer;
            //cmd.indexBuffer = indexBuffer;
        }

        //
        // GenerateBonesForFrame
        //
        private void GenerateBonesForFrame(int frame, int torsoFrame)
        {
            for (int i = 0; i < surfaces.Length; i++)
            {
                CalcBones(frame, torsoFrame, surfaces[i].startBoneRef, surfaces[i].numBoneReferences, boneRefs);
            }
        }

        //
        // GenerateSurfaces
        //
        private void GenerateSurfaces()
        {
            idMatrix m = new idMatrix();

            // This is only temporary to get the build on to the phone.
            for (int i = 0; i < vertexes.Count; i++)
            {
                idDrawVertex vert = drawVertexes[i];
                idDrawVertexSkin input = vertexes[i];

                m.Set(bones[(int)input.blendIndices[0]]);
                vert.xyz = SkeletalMath.LocalAddScaledMatrixTransformVectorTranslate(input.offset1, input.weights[0], m);

                m.Set(bones[(int)input.blendIndices[1]]);
                vert.xyz += SkeletalMath.LocalAddScaledMatrixTransformVectorTranslate(input.offset2, input.weights[1], m);

                m.Set(bones[(int)input.blendIndices[2]]);
                vert.xyz += SkeletalMath.LocalAddScaledMatrixTransformVectorTranslate(input.offset3, input.weights[2], m);

                m.Set(bones[(int)input.blendIndices[3]]);
                vert.xyz += SkeletalMath.LocalAddScaledMatrixTransformVectorTranslate(input.offset4, input.weights[3], m);

                vert.st = input.st;

                drawVertexes[i] = vert;
            }

           // BuildVertexIndexBuffer();
        }

        //
        // idDrawSurface
        //
        public override idDrawSurface[] BackendTessModel(int frame, int torsoFrame)
        {
            GenerateBonesForFrame(frame, torsoFrame);
            GenerateSurfaces();

            Array.Copy(drawVertexes, Globals.tess.drawVerts, drawVertexes.Length);
            Array.Copy(indexes.ToArray(), Globals.tess.indexes, indexes.Count);

         //   Buffer.BlockCopy(drawVertexes, 0, Globals.tess.drawVerts, 0,drawVertexes.Length);
         //   Buffer.BlockCopy(indexes.ToArray(), 0, Globals.tess.indexes, 0, indexes.Count);

            Globals.tess.numVertexes = drawVertexes.Length;
            Globals.tess.numIndexes = indexes.Count;

            return surfaces;
        }

        //
        // TessModel
        //
        public override void TessModel(ref idRenderEntityLocal entity)
        {
            idSkinLocal skin = (idSkinLocal)entity.customSkin;
            

            // Calculate the bones for each surface.
            for (int i = 0; i < surfaces.Length; i++)
            {
                if (skin != null)
                {
                    surfaces[i].materials[0] = skin.surfaces[i].shader;
                }

                surfaces[i].visCount = -1;
            }

            idRenderCommand cmd = Globals.backEnd.GetCommandBuffer();
            cmd.type = renderCommandType.RC_DRAW_TESSBUFFER;
            cmd.frame = entity.frame;
            cmd.torsoFrame = entity.torsoFrame;
            cmd.model = this;

         //   Globals.SetVertexIndexBuffers(vertexBuffer, indexBuffer);
            //SetBoneBuffer();
            //Globals.SortSurfaces(0, ref surfaces);
        }
    }
}
