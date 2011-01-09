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

        private const float MDC_MAX_OFS = 127.0f;
        private const float MDC_DIST_SCALE = 0.05f;    // lower for more accuracy, but less range

        private mdcHeader_t header = new mdcHeader_t();
        private md3Frame_t[] frames;
        private mdcTag_t[] tags;
        private mdcTagName_t[] tagnames;
        private mdcSurface_t[] surfaces;
        private int numVertsPerFrame = 0;


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

            // Load in the names for the tags.
            tagnames = new mdcTagName_t[header.numFrames * header.numTags];
            f.Seek(idFileSeekOrigin.FS_SEEK_SET, header.ofsTagNames);
            for (int i = 0; i < header.numFrames * header.numTags; i++)
            {
                tagnames[i].InitFromFile(ref f);
            }


            // Load in all the tags.
            tags = new mdcTag_t[header.numFrames * header.numTags];
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

                idDrawSurface drawSurf = (idDrawSurface)surf;
                InitSurface(surf.numVerts, surf.numBaseFrames, surf.numTriangles * 3, ref drawSurf);
                surf = (mdcSurface_t)drawSurf;

                // Load in all the triangles.
                f.Seek(idFileSeekOrigin.FS_SEEK_SET, surfpos + surf.ofsTriangles);
                for (int j = 0; j < surf.numTriangles; j++)
                {
                    for (int d = 0; d < 3; d++)
                    {
                        drawIndexes.Add((short)f.ReadInt());
                    }
                }
                
                // Load in the MD3 texture coordinates.
                f.Seek(idFileSeekOrigin.FS_SEEK_SET, surfpos + surf.ofsSt);
                ParseMD3TextureCoords(surf.startVertex, surf.numVerts, surf.numBaseFrames, ref f);
                
                // swap all the XyzNormals
                f.Seek(idFileSeekOrigin.FS_SEEK_SET, surfpos + surf.ofsXyzNormals);
                ParseMD3Vertexes(surf.startVertex, surf.numVerts, surf.numBaseFrames, ref f);

                surf.vertexSort = drawVertexes[surf.startVertex + drawIndexes[surf.startIndex]].xyz.LengthSqr();

                // swap all the XyzCompressed
                //xyzComp = (mdcXyzCompressed_t*)((byte*)surf + surf->ofsXyzCompressed);
                f.Seek(idFileSeekOrigin.FS_SEEK_SET, surfpos + surf.ofsXyzCompressed);
                surf.xyzCompressedIndexPool = new mdcXyzCompressed_t[surf.numVerts * surf.numCompFrames];
                for (int j = 0; j < surf.numVerts * surf.numCompFrames; j++)
                {
                    surf.xyzCompressedIndexPool[j] = new mdcXyzCompressed_t();
                    surf.xyzCompressedIndexPool[j].ofsVec = f.ReadUInt();
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

            

            // Build our vertex and index buffers.
            BuildVertexIndexBuffer();
        }

        //
        // ShiftDrawOrderHack
        //
        private void ShiftDrawOrderHack()
        {
            // Move the blended surfaces to draw after everything else has been drawn,
            // this is a NASTY hack and its slow.
            int i = 0;
            while (true)
            {
                idMaterialBase mtr;
                if (i >= surfaces.Length)
                    break;

                mtr = idMaterialLocal.GetMaterialBase(surfaces[i].materials[0]);
                if (mtr.stages[0].useBlending == true)
                {
                    if (i == surfaces.Length - 1)
                        break;

                    if (surfaces[i].vertexSort != 0)
                    {
                        i++;
                        continue;
                    }

                    mdcSurface_t tmp = surfaces[i];
                    surfaces[i] = surfaces[i + 1];
                    surfaces[i + 1] = tmp;
                    surfaces[i].vertexSort = 1;
                    i++;
                }

                i++;
            }
        }

        //
        // MDC_DecodeXyzCompressed
        //
        private void MDC_DecodeXyzCompressed(uint ofsVec, ref idVector3 newOfsVec)
        {
            newOfsVec[0] = ( (float)( ( ofsVec ) & 255 ) - MDC_MAX_OFS ) * MDC_DIST_SCALE;
	        newOfsVec[1] = ( (float)( ( ofsVec >> 8 ) & 255 ) - MDC_MAX_OFS ) * MDC_DIST_SCALE;
	        newOfsVec[2] = ( (float)( ( ofsVec >> 16 ) & 255 ) - MDC_MAX_OFS ) * MDC_DIST_SCALE;
        }

        //
        // ComputeCompressedFrames
        //
        private void ComputeCompressedFrames()
        {
            idVector3 newOfsVec = new idVector3();

            
            for (int i = 0; i < header.numFrames; i++)
            {
                numVertsPerFrame = 0;
                for (int surfaceNum = 0; surfaceNum < surfaces.Length; surfaceNum++)
                {
                    idDrawSurface surf = surfaces[surfaceNum];
                    int baseFrame = ((mdcSurface_t)surf).baseFrames[i] * surf.numVertexes;
                    short compframe = ((mdcSurface_t)surf).compFrames[i];
                    int baseXyzCompFrame = compframe * surf.numVertexes;
                    
                    for (int v = surf.startVertex; v < surf.startVertex + surf.numVertexes; v++, baseXyzCompFrame++)
                    {
                        idDrawVertex vert = drawVertexes[v + baseFrame];
                        mdcXyzCompressed_t xyzCompFrame;

                        // Scale the model vertexes.
                        vert.xyz *= idModelMD3.MD3_XYZ_SCALE;

                        numVertsPerFrame++;

                        // If the frame isn't compressed just continue.
                        if (compframe < 0)
                        {
                            renderVertexes.Add(vert);
                            continue;
                        }

                        // Get the compressed frame index and decode it.
                        xyzCompFrame = ((mdcSurface_t)surf).xyzCompressedIndexPool[baseXyzCompFrame];
                        MDC_DecodeXyzCompressed(xyzCompFrame.ofsVec, ref newOfsVec);

                        vert.xyz += newOfsVec;
                        //drawVertexes[v + baseFrame] = vert;
                        renderVertexes.Add(vert);
                    }
                }
            }
        }

        //
        // InternalGetTag
        //
        private int InternalGetTag(int frame, string name, int startTagIndex, out mdcTag_t tag)
        {
            int i;

            if (frame >= GetNumFrames())
            {
                // it is possible to have a bad frame while changing models, so don't error
                frame = GetNumFrames() - 1;
            }

            if (startTagIndex > tags.Length)
            {
                tag = null;
                return -1;
            }

            for (i = 0; i < header.numTags; i++)
            {
                if ((i >= startTagIndex) && tagnames[i].name == name)
                {
                    break;  // found it
                }
            }

            if (i >= header.numTags)
            {
                tag = null;
                return -1;
            }

            tag = tags[frame * header.numTags + i];
            return i;
        }

        //
        // GetTag
        //
        public override int GetTag(string name, int startframe, int endframe, int index, ref idOrientation orientation)
        {
            // psuedo-compressed MDC tags
		    mdcTag_t    cstart, cend;
            md3Tag_t    start, end;
            idVector3 sangles, eangles;
            int retval;

            // So the compiler will shut up.
            start.origin = idVector3.vector_origin;
            end.origin = idVector3.vector_origin;
            sangles = idVector3.vector_origin;
            eangles = idVector3.vector_origin;

            retval = InternalGetTag( startframe, name, index, out cstart);
            retval = InternalGetTag( endframe, name, index, out cend);

		    // uncompress the MDC tags into MD3 style tags
		    if ( cstart != null && cend != null ) {
			    for ( int i = 0; i < 3; i++ ) {
				    start.origin[i] = (float)cstart.xyz[i] * idModelMD3.MD3_XYZ_SCALE;
                    end.origin[i] = (float)cend.xyz[i] * idModelMD3.MD3_XYZ_SCALE;
                    sangles[i] = (float)cstart.angles[i] * MDC_TAG_ANGLE_SCALE;
                    eangles[i] = (float)cend.angles[i] * MDC_TAG_ANGLE_SCALE;
			    }

                start.axis = sangles.ToAxis();
                end.axis = eangles.ToAxis();
		    } else {
                return -1;
		    }

            orientation.origin = end.origin;
            orientation.axis = end.axis;

            return retval;
        }

        //
        // GetNumFrames
        //
        public override int GetNumFrames()
        {
            return header.numFrames;
        }

        //
        // BuildVertexIndexBuffer
        //
        public override void BuildVertexIndexBuffer()
        {
            ComputeCompressedFrames();
            base.BuildVertexIndexBuffer();
        }

        //
        // TessModel
        // 
        public override void TessModel(ref idRenderEntityLocal entity)
        {
            int i;
            Globals.SetVertexIndexBuffers(vertexBuffer, indexBuffer);
            for (i = 0; i < surfaces.Length; i++)
            {
                surfaces[i].visCount = -1;
                surfaces[i].sort = entity.origin.LengthSqr();
                surfaces[i].vertexSort = 0;
            }

            ShiftDrawOrderHack();
            
            Globals.SortSurfaces(entity.frame * numVertsPerFrame, ref surfaces);
        }

        //
        // GetModelBounds
        //
        public override void GetModelBounds(out idVector3 mins, out idVector3 maxs)
        {
            mins = frames[0].mins;
            maxs = frames[0].maxs;
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
