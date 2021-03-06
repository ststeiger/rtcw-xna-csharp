﻿/*
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

// map.cs (c) 2010 id Software
//

//#define PATCH_STITCHING

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using idLib;
using idLib.Math;
using idLib.Engine.Public;

using rtcw.Renderer.Models;

namespace rtcw.Renderer.Map
{
    //
    // idMap
    //
    public class idMap
    {
        // ---------- Persistant BSP Storage -----------
        //public idImage[] lightmaps;
        public idWorldFog_t[] fogs;

        public VertexBuffer vertexBuffer;
        public IndexBuffer indexBuffer;
        public idDrawSurface[] drawSurfs;

        public idVector3 lightGridSize;
        public idVector3 lightGridInverseSize;
        public idVector3 lightGridOrigin;
        public idVector3 lightGridBounds;
        public Color[] lightGridData;

        public idRenderNode[] nodes;
        public int numDecisionNodes;

        private int[] markSurfaces;

        public int[] leafbrushes;

        public int clusterBytes;
        public int numClusters;
        public idMapVis vis;

        public string entityString;

        public idModelBrush[] bmodels;
        public idPlane[] planes;

        public idMapFormat.idMapBrush[] brushes;
        public idMapFormat.idMapBrushSide[] brushsides;

        public idMapFormat.idMapShader[] shaders;

        public int numNodes;

        // ---------- Temporary BSP Storage -----------
        List<idDrawVertex> drawVerts;
        List<short> drawIndexes;

        idMapFormat.idMapHeader header;        
        //idMapFormat.idMapFog[] fogs;
        
        //idMapFormat.idMapModel[] bmodels;

        public idCollisionModel collisionmodel;

        private string name;
        private string baseName;

        //
        // LumpCount
        //
        private int LumpCount(idMapFormat.lump_t lump, int lump_size)
        {
            return lump.filelen / lump_size;
        }

        //
        // SetLumpPosition
        //
        private void SetLumpPosition(idMapFormat.lump_t lump, ref idFile bspFile)
        {
            bspFile.Seek(idFileSeekOrigin.FS_SEEK_SET, lump.fileofs);
        }

        /*
        =================
        LoadFogs
        =================
        */
        private void LoadFogs(ref idFile bspFile, idMapFormat.lump_t lump, idMapFormat.lump_t brushesLump, idMapFormat.lump_t sidesLump)
        {
            int count, brushesCount, sidesCount;
            int firstSide;
            int sideNum;
            int planeNum;
            float d;
            idMaterial shader;
            idMapFormat.idMapFog foglump = new idMapFormat.idMapFog();

            // Load in the bsp fog lump.
            count = LumpCount(lump, idMapFormat.idMapFog.LUMP_SIZE);
            fogs = new idWorldFog_t[count];

            // Load in the bsp brushes.
            brushesCount = LumpCount(brushesLump, idMapFormat.idMapBrush.LUMP_SIZE);
            SetLumpPosition(brushesLump, ref bspFile);
            brushes = new idMapFormat.idMapBrush[brushesCount];
            for (int i = 0; i < brushesCount; i++)
            {
                brushes[i].InitFromFile(ref bspFile);

                brushes[i].contents = shaders[brushes[i].shaderNum].contentFlags;
                brushes[i].checkcount = 0;
            }

            // Load in the brush sides.
            sidesCount = LumpCount(sidesLump, idMapFormat.idMapBrushSide.LUMP_SIZE);
            SetLumpPosition(sidesLump, ref bspFile);
            brushsides = new idMapFormat.idMapBrushSide[sidesCount];
            for (int i = 0; i < sidesCount; i++)
            {
                brushsides[i].InitFromFile(ref bspFile);
                brushsides[i].surfaceFlags = shaders[brushsides[i].shaderNum].contentFlags;
            }

            SetLumpPosition(lump, ref bspFile);
            for (int i = 0; i < count; i++)
            {
                foglump.InitFromFile(ref bspFile);

                fogs[i] = new idWorldFog_t();
                fogs[i].originalBrushNumber = foglump.brushNum;

                if (fogs[i].originalBrushNumber >= brushesCount)
                {
                    Engine.common.ErrorFatal("fog brushNumber out of range");
                }

                firstSide = brushes[fogs[i].originalBrushNumber].firstSide;

                if (firstSide > sidesCount - 6)
                {
                    Engine.common.ErrorFatal("fog brush sideNumber out of range");
                }

                // brushes are always sorted with the axial sides first
                sideNum = firstSide + 0;
                planeNum = brushsides[sideNum].planeNum;
                fogs[i].bounds[0][0] = -planes[planeNum].Dist;

                sideNum = firstSide + 1;
                planeNum = brushsides[sideNum].planeNum;
                fogs[i].bounds[1][0] = planes[planeNum].Dist;

                sideNum = firstSide + 2;
                planeNum = brushsides[sideNum].planeNum;
                fogs[i].bounds[0][1] = -planes[planeNum].Dist;

                sideNum = firstSide + 3;
                planeNum = brushsides[sideNum].planeNum;
                fogs[i].bounds[1][1] = planes[planeNum].Dist;

                sideNum = firstSide + 4;
                planeNum = brushsides[sideNum].planeNum;
                fogs[i].bounds[0][2] = -planes[planeNum].Dist;

                sideNum = firstSide + 5;
                planeNum = brushsides[sideNum].planeNum;
                fogs[i].bounds[1][2] = planes[planeNum].Dist;

                // get information from the shader for fog parameters
                shader = Engine.materialManager.FindMaterial(foglump.shader, -1);

                fogs[i].parms = idMaterialLocal.GetMaterialBase(ref shader).fogParms;

                fogs[i].colorInt = idMapFormat.ColorBytes4(fogs[i].parms.color[0] * Globals.tr.identityLight,
                                             fogs[i].parms.color[1] * Globals.tr.identityLight,
                                             fogs[i].parms.color[2] * Globals.tr.identityLight, 1.0f);

                d = fogs[i].parms.depthForOpaque < 1 ? 1 : fogs[i].parms.depthForOpaque;
                fogs[i].tcScale = 1.0f / (d * 8);

                // set the gradient vector
                sideNum = foglump.visibleSide;

                if (sideNum == -1)
                {
                    fogs[i].hasSurface = false;
                }
                else
                {
                    fogs[i].hasSurface = true;
                    planeNum = brushsides[firstSide + sideNum].planeNum;

                    //VectorSubtract( vec3_origin, s_worldData.planes[ planeNum ].normal, out->surface );
                    fogs[i].surface.X = -planes[planeNum].Normal.X;
                    fogs[i].surface.Y = -planes[planeNum].Normal.Y;
                    fogs[i].surface.Z = -planes[planeNum].Normal.Z;

                    fogs[i].surface[3] = -planes[planeNum].Dist;
                }
            }
        }

        /*
        =================
        LoadShaders
        =================
        */
        private void LoadShaders(ref idFile bspFile, idMapFormat.lump_t lump) {
            int count = 0;

            SetLumpPosition(lump, ref bspFile);
            count = LumpCount(lump, idMapFormat.idMapShader.LUMP_SIZE);

            shaders = new idMapFormat.idMapShader[count];

            for (int i = 0; i < count; i++)
            {
                shaders[i].InitFromFile(ref bspFile);
	        }
        }


        /*
        =================
        LoadShaders
        =================
        */
        private void LoadPlanes(ref idFile bspFile, idMapFormat.lump_t lump)
        {
            int count = 0;
            idMapFormat.idMapPlane bspPlane = new idMapFormat.idMapPlane();

            SetLumpPosition(lump, ref bspFile);
            count = LumpCount(lump, idMapFormat.idMapPlane.LUMP_SIZE);

            planes = new idPlane[count * 2];

            for (int i = 0; i < count; i++)
            {
                int bits = 0;
                bspPlane.InitFromFile(ref bspFile);

                for( int j = 0; j < 3; j++ )
                {
                    if ( bspPlane.normal[j] < 0 ) {
				        bits |= 1 << j;
			        }
                }

                planes[i] = new idPlane();
                planes[i].Normal = bspPlane.normal;
                planes[i].Dist = bspPlane.dist;
                planes[i].SetPlaneType();
                planes[i].SignBits = bits;
            }

            for (int i = count; i < planes.Length; i++)
            {
                planes[i] = new idPlane();
            }
        }

        /*
        ================
        R_LoadEntities
        ================
        */
        private void LoadEntities(ref idFile bspFile, idMapFormat.lump_t lump)
        {
	        string p, token, s;
	        string keyname;
            string value;
            idParser parser;
	        
	        lightGridSize[0] = 64;
	        lightGridSize[1] = 64;
	        lightGridSize[2] = 128;
#if false // disabled
            SetLumpPosition(lump, ref bspFile);

	        // store for reference by the cgame
            entityString = bspFile.ReadString(lump.filelen + 1);

            parser = new idParser(entityString);
            token = parser.NextToken;
	        if ( token == null || token != "{" ) {
		        return;
	        }

	        // only parse the world spawn
	        while ( true ) {
		        // parse key
                token = parser.NextToken;
                if (token == null || parser.ReachedEndOfBuffer == true || token == "}")
                {
			        break;
		        }

                keyname = token;

		        // parse value
                // parse key
                token = parser.NextToken;
                if (token == null || parser.ReachedEndOfBuffer == true || token == "}")
                {
                    break;
                }
                value = token;

		        // check for remapping of shaders for vertex lighting
		        s = "vertexremapshader";
		        if ( keyname.Contains( s ) ) {
                    int semiColonIndex = s.IndexOf(';');
                    if (semiColonIndex <= 0)
                    {
				        Engine.common.Warning("no semi colon in vertexshaderremap '%s'\n", value );
				        break;
			        }
                    semiColonIndex++;
                   // s = s.CopyTo
			       // if ( r_vertexLight->integer ) {
				   //     R_RemapShader( value, s, "0" );
			       // }
			        continue;
		        }
		        // check for remapping of shaders
                /*
		        s = "remapshader";
		        if ( !Q_strncmp( keyname, s, strlen( s ) ) ) {
			        s = strchr( value, ';' );
			        if ( !s ) {
				        ri.Printf( PRINT_WARNING, "WARNING: no semi colon in shaderremap '%s'\n", value );
				        break;
			        }
			        *s++ = 0;
			        R_RemapShader( value, s, "0" );
			        continue;
		        }
                */
		        // check for a different grid size
		        if ( keyname == "gridsize" ) {
                    lightGridSize[0] = parser.NextFloat;
                    lightGridSize[1] = parser.NextFloat;
                    lightGridSize[2] = parser.NextFloat;
			        continue;
		        }
	        }

            parser.Dispose();
#endif
        }

        /*
        ==============
        R_FindLightGridBounds
        ==============
        */
        private void R_FindLightGridBounds( out idVector3 mins, out idVector3 maxs ) {
#if false // disabled in rtcw code base
	        world_t *w;
	        msurface_t  *surf;
	        srfSurfaceFace_t *surfFace;
#endif
        //	cplane_t	*plane;
#if false // disabled in rtcw code base
	        struct shader_s     *shd;

	        bool foundGridBrushes = false;
	        int i,j;
#endif

        //----(SA)	temp - disable this whole thing for now

            bmodels[0].GetModelBounds(out mins, out maxs);

#if false // disabled in rtcw code base
	        return;
        //----(SA)	temp




	        ClearBounds( mins, maxs );

        // wrong!
	        for ( i = 0; i < w->bmodels[0].numSurfaces; i++ ) {
		        surf = w->bmodels[0].firstSurface + i;
		        shd = surf->shader;

		        if ( !( *surf->data == SF_FACE ) ) {
			        continue;
		        }

		        if ( !( shd->contentFlags & CONTENTS_LIGHTGRID ) ) {
			        continue;
		        }

		        foundGridBrushes = qtrue;
	        }


        // wrong!
	        for ( i = 0; i < w->numsurfaces; i++ ) {
		        surf = &w->surfaces[i];
		        shd = surf->shader;
		        if ( !( *surf->data == SF_FACE ) ) {
			        continue;
		        }

		        if ( !( shd->contentFlags & CONTENTS_LIGHTGRID ) ) {
			        continue;
		        }

		        foundGridBrushes = qtrue;

		        surfFace = ( srfSurfaceFace_t * )surf->data;

		        for ( j = 0; j < surfFace->numPoints; j++ ) {
			        AddPointToBounds( surfFace->points[j], mins, maxs );
		        }

	        }

	        // go through brushes looking for lightgrid
        //	for ( i = 0 ; i < numbrushes ; i++ ) {
        //		db = &dbrushes[i];
        //
        //		if (!(dshaders[db->shaderNum].contentFlags & CONTENTS_LIGHTGRID)) {
        //			continue;
        //		}
        //
        //		foundGridBrushes = qtrue;
        //
        //		// go through light grid surfaces for bounds
        //		for ( j = 0 ; j < db->numSides ; j++ ) {
        //			s = &dbrushsides[ db->firstSide + j ];
        //
        //			surfmin[0] = -dplanes[ dbrushsides[ db->firstSide + 0 ].planeNum ].dist - 1;
        //			surfmin[1] = -dplanes[ dbrushsides[ db->firstSide + 2 ].planeNum ].dist - 1;
        //			surfmin[2] = -dplanes[ dbrushsides[ db->firstSide + 4 ].planeNum ].dist - 1;
        //			surfmax[0] = dplanes[ dbrushsides[ db->firstSide + 1 ].planeNum ].dist + 1;
        //			surfmax[1] = dplanes[ dbrushsides[ db->firstSide + 3 ].planeNum ].dist + 1;
        //			surfmax[2] = dplanes[ dbrushsides[ db->firstSide + 5 ].planeNum ].dist + 1;
        //			AddPointToBounds (surfmin, mins, maxs);
        //			AddPointToBounds (surfmax, mins, maxs);
        //		}
        //	}


        //----(SA)	temp
	        foundGridBrushes = qfalse;  // disable this whole thing for now
        //----(SA)	temp

	        if ( !foundGridBrushes ) {
		        VectorCopy( w->bmodels[0].bounds[0], mins );
		        VectorCopy( w->bmodels[0].bounds[1], maxs );
	        }
#endif
        }

        /*
        ================
        LoadLightGrid
        ================
        */
        private void LoadLightGrid(ref idFile bspFile, idMapFormat.lump_t lump)
        {
            int i;
            idVector3 maxs = new idVector3();
            int numGridPoints;
            //	float	*wMins, *wMaxs;
            idVector3 wMins, wMaxs;

            lightGridInverseSize[0] = 1.0f / lightGridSize[0];
            lightGridInverseSize[1] = 1.0f / lightGridSize[1];
            lightGridInverseSize[2] = 1.0f / lightGridSize[2];

            //----(SA)	modified
            R_FindLightGridBounds(out wMins, out wMaxs);
            //	wMins = w->bmodels[0].bounds[0];
            //	wMaxs = w->bmodels[0].bounds[1];
            //----(SA)	end

            for (i = 0; i < 3; i++)
            {
                lightGridOrigin[i] = lightGridSize[i] * (float)System.Math.Ceiling(wMins[i] / lightGridSize[i]);
                maxs[i] = lightGridSize[i] * (float)System.Math.Floor(wMaxs[i] / lightGridSize[i]);
                lightGridBounds[i] = (maxs[i] - lightGridOrigin[i]) /lightGridSize[i] + 1;
            }

            numGridPoints = (int)(lightGridBounds[0] * lightGridBounds[1] * lightGridBounds[2]);

            if (lump.filelen != numGridPoints * 8)
            {
                Engine.common.Warning("light grid mismatch\n");
                lightGridData = null;
                return;
            }

            SetLumpPosition(lump, ref bspFile);
            lightGridData = new Color[lump.filelen];
            for (i = 0; i < lump.filelen; i += 4)
            {
                lightGridData[i] = new Color();
                lightGridData[i].R = bspFile.ReadByte();
                lightGridData[i].G = bspFile.ReadByte();
                lightGridData[i].B = bspFile.ReadByte();
                lightGridData[i].A = bspFile.ReadByte();
            }


            // deal with overbright bits
            for (i = 0; i < numGridPoints; i++)
            {
               // idMapFormat.ColorShiftLightingBytes(lightGridData, i * 8, i * 8, ref lightGridData);
               // idMapFormat.ColorShiftLightingBytes(lightGridData, i * 8 + 3, i * 8 + 3, ref lightGridData);
            }
        }

        /*
        =================
        R_LoadSubmodels
        =================
        */
        private void LoadSubmodels(ref idFile bspFile, idMapFormat.lump_t lump) 
        {
            int count = 0;
            idMapFormat.idMapModel mapmodel = new idMapFormat.idMapModel();

            SetLumpPosition(lump, ref bspFile);
            count = LumpCount(lump, idMapFormat.idMapModel.LUMP_SIZE );

            bmodels = new idModelBrush[count];

            for( int i = 0; i < count; i++ )
            {
                idModelBrush model;

		        model = Globals.AllocModelBrush("*" + i, this);
                mapmodel.InitFromFile( ref bspFile );

                model.SetModelBounds( mapmodel.mins, mapmodel.maxs );
                model.SetSurfaceParams( mapmodel.firstSurface, mapmodel.numSurfaces );
                bmodels[i] = model;
            }
        }

        //
        // LoadMarksurfaces
        //
        private void LoadMarksurfaces(ref idFile bspFile, idMapFormat.lump_t lump)
        {
            int count = 0;

            SetLumpPosition(lump, ref bspFile);
            count = LumpCount(lump, sizeof(int));

            markSurfaces = new int[count];

            for (int i = 0; i < count; i++)
            {
                markSurfaces[i] = bspFile.ReadInt();
            }
        }

        //
        // LoadLeafBrushes
        //
        private void LoadLeafBrushes(ref idFile bspFile, idMapFormat.lump_t lump)
        {
            int count = 0;

            SetLumpPosition(lump, ref bspFile);
            count = LumpCount(lump, sizeof(int));

            leafbrushes = new int[count];

            for (int i = 0; i < count; i++)
            {
                leafbrushes[i] = bspFile.ReadInt();
            }
        }

        /*
        =================
        LoadVisibility
        =================
        */
        private void LoadVisibility(ref idFile bspFile, idMapFormat.lump_t lump)
        {
            int len = 0;

            SetLumpPosition(lump, ref bspFile);
            len = lump.filelen;
            if (len <= 0)
            {
                return;
            }

            numClusters = bspFile.ReadInt();
            clusterBytes = bspFile.ReadInt();

            vis = new idMapVis(bspFile.ReadBytes(lump.filelen - 8), nodes, clusterBytes, numClusters, markSurfaces, numNodes);
        }

        /*
        =================
        LoadShaders
        =================
        */
        public const int LIGHTMAP_SIZE = 128;
        public const int LIGHTMAP_LUMP_SIZE = LIGHTMAP_SIZE * LIGHTMAP_SIZE * 3;
        private void LoadLightmaps(ref idFile bspFile, idMapFormat.lump_t lump)
        {
            int count = 0;
            float maxIntensity = 0;
	        double sumIntensity = 0;
            Color[] image = new Color[LIGHTMAP_SIZE * LIGHTMAP_SIZE];
            idVector3 outrgb = new idVector3();

            count = lump.filelen; //LumpCount(lump,LIGHTMAP_LUMP_SIZE );
            SetLumpPosition( lump, ref bspFile );

            Globals.tr.lightmaps = new idImage[count];

            if (count == 1)
            {
                //FIXME: HACK: maps with only one lightmap turn up fullbright for some reason.
                //this avoids this, but isn't the correct solution.
                count++;
            }

            for ( int i = 0 ; i < count ; i++ ) {
#if false
		        // expand the 24 bit on-disk to 32 bit
		        byte[] buf_p = bspFile.ReadBytes( LIGHTMAP_LUMP_SIZE );

		        if ( Globals.r_lightmap.GetValueInteger() == 2 ) { // color code by intensity as development tool	(FIXME: check range)
			        for ( int j = 0; j < LIGHTMAP_SIZE * LIGHTMAP_SIZE; j++ )
			        {
				        float r = buf_p[j * 3 + 0];
				        float g = buf_p[j * 3 + 1];
				        float b = buf_p[j * 3 + 2];
				        float intensity;
				        

				        intensity = 0.33f * r + 0.685f * g + 0.063f * b;

				        if ( intensity > 255 ) {
					        intensity = 1.0f;
				        } else {
					        intensity /= 255.0f;
				        }

				        if ( intensity > maxIntensity ) {
					        maxIntensity = intensity;
				        }

				        idMapFormat.HSVtoRGB( intensity, 1.00f, 0.50f, ref outrgb );

                        image[j * 4] = new Color(outrgb[0] * 255, outrgb[1] * 255, outrgb[2] * 255, 255);

				        sumIntensity += intensity;
			        }
		        } 
                else {
			        for ( int j = 0 ; j < LIGHTMAP_SIZE * LIGHTMAP_SIZE; j++ ) {
                        idMapFormat.ColorShiftLightingBytes(buf_p, j * 3, j, ref image);
				        image[j].A = 255;
			        }
		        }
                Globals.tr.lightmaps[i] = Engine.imageManager.CreateImage("*lightmap" + i, image, LIGHTMAP_SIZE, LIGHTMAP_SIZE, false, false, SamplerState.LinearClamp);
#else
                // DXT images are already expanded.
                int dxtImageSize = bspFile.ReadInt();
                byte[] buf_p = bspFile.ReadBytes(dxtImageSize);
                Globals.tr.lightmaps[i] = Engine.imageManager.CreateDXTImage("*lightmap" + i, buf_p, LIGHTMAP_SIZE, LIGHTMAP_SIZE, false, false, SamplerState.LinearClamp);
#endif
	        }
#if false
            if (Globals.r_lightmap.GetValueInteger() == 2)
            {
		        Engine.common.Printf( "Brightest lightmap value: %d\n", ( int ) ( maxIntensity * 255 ) );
	        }
#endif
        }

        /*
        =================
        SetParent
        =================
        */
        private void SetParent(ref idRenderNode node, idRenderNode parent)
        {
            node.parent = parent;
            if (node.contents != -1)
            {
                return;
            }
            SetParent(ref node.children[0], node);
            SetParent(ref node.children[1], node);
        }

        /*
        =================
        LoadNodesAndLeafs
        =================
        */
        private void LoadNodesAndLeafs(ref idFile bspFile, idMapFormat.lump_t lump, idMapFormat.lump_t leaflump) {
	        int i, j, p;

            idMapFormat.idMapNode[] inNodes;
            idMapFormat.idMapLeaf[] inLeafs;
            
	        int numLeafs;

            SetLumpPosition( lump, ref bspFile );
            numNodes = LumpCount( lump, idMapFormat.idMapNode.LUMP_SIZE );
            inNodes = new idMapFormat.idMapNode[numNodes];
            for( i = 0; i < numNodes; i++ )
            {
                inNodes[i].InitFromFile( ref bspFile );
            }

            SetLumpPosition( leaflump, ref bspFile );
            numLeafs = LumpCount( leaflump, idMapFormat.idMapLeaf.LUMP_SIZE );
            inLeafs = new idMapFormat.idMapLeaf[numLeafs];
            for (i = 0; i < numLeafs; i++)
            {
                inLeafs[i].InitFromFile( ref bspFile );
            }

            // jv- We allocate the nodes before we set them up so we can set the children and repartent everything properly.
	        nodes = new idRenderNode[numNodes + numLeafs];
            for (i = 0; i < numNodes + numLeafs; i++)
            {
                nodes[i] = new idRenderNode();
            }
	        numDecisionNodes = numNodes;

	        // load nodes
	        for ( i = 0 ; i < numNodes; i++ )
	        {
                idRenderNode nodeOut = nodes[i];
                idMapFormat.idMapNode nodeIn = inNodes[i];

		        for ( j = 0 ; j < 3 ; j++ )
		        {
			        nodeOut.mins[j] = nodeIn.mins[j];
                    nodeOut.maxs[j] = nodeIn.maxs[j];
		        }

		        p = nodeIn.planeNum;
                nodeOut.plane = planes[p];

		        nodeOut.contents = idRenderNode.CONTENTS_NODE;  // differentiate from leafs

		        for ( j = 0 ; j < 2 ; j++ )
		        {
			        p = nodeIn.children[j];
			        if ( p >= 0 ) {
				        nodeOut.children[j] = nodes[p];
			        } else {
				        nodeOut.children[j] = nodes[numNodes + ( -1 - p )];
			        }
                    nodeOut.childrenhandles[j] = p;
		        }

                nodes[i] = nodeOut;
	        }

	        // load leafs
	        for ( i = numNodes ; i < numNodes + numLeafs ; i++ )
	        {
                idRenderNode nodeOut = nodes[i];
                idMapFormat.idMapLeaf inLeaf = inLeafs[i - numNodes];

		        for ( j = 0 ; j < 3 ; j++ )
		        {
			        nodeOut.mins[j] = inLeaf.mins[j];
			        nodeOut.maxs[j] = inLeaf.maxs[j];
		        }

		        nodeOut.cluster = inLeaf.cluster;
		        nodeOut.area = inLeaf.area;

		        if ( nodeOut.cluster >= numClusters ) {
			        numClusters = nodeOut.cluster + 1;
		        }

		        nodeOut.firstmarksurface = inLeaf.firstLeafSurface;
                nodeOut.nummarksurfaces = inLeaf.numLeafSurfaces;

                nodeOut.firstBrushSurface = inLeaf.firstLeafBrush;
                nodeOut.numBrushSurfaces = inLeaf.numLeafBrushes;
               
                nodes[i] = nodeOut;
	        }

            inNodes = null;
            inLeafs = null;

	        // chain decendants
	        SetParent( ref nodes[0], null );
        }

        /*
        ===============
        ParseFace
        ===============
        */
        private void ParseFace(idMapFormat.idMapSurface ds, idMapFormat.idMapVertex[] verts, out idDrawSurface surfResult)
        {
            idDrawSurfaceFace surf = new idDrawSurfaceFace();

            surf.materials = new idMaterial[1];
            surf.materials[0] = Engine.materialManager.FindMaterial(shaders[ds.shaderNum].shader, ds.lightmapNum);

            surf.fogIndex = ds.fogNum;

            surf.startIndex = ds.firstIndex;
            surf.startVertex = ds.firstVert;
            surf.numIndexes = ds.numIndexes;
            surf.numVertexes = ds.numVerts;
            surf.type = surfaceType_t.SF_FACE;

            // take the plane information from the lightmap vector
            for (int i = 0; i < 3; i++)
            {
                surf.plane.Normal[i] = ds.lightmapVecs[2][i];
            }
            idVector3 surfPoint0 = drawVerts[drawIndexes[surf.startIndex]].xyz;
            surf.plane.Dist = surf.plane.Normal * surfPoint0; 

            surf.plane.SetPlaneSignbits();
            surf.plane.SetPlaneType();

            surfResult = surf;
        }

        /*
        ===============
        ParseTriSurf
        ===============
        */
        private void ParseTriSurf(idMapFormat.idMapSurface ds, idMapFormat.idMapVertex[] verts, out idDrawSurface surfResult)
        {
            idDrawSurfaceTri surf = new idDrawSurfaceTri();

            surf.materials = new idMaterial[1];
            surf.materials[0] = Engine.materialManager.FindMaterial(shaders[ds.shaderNum].shader, ds.lightmapNum);

            surf.fogIndex = ds.fogNum;

            surf.startIndex = ds.firstIndex;
            surf.startVertex = ds.firstVert;
            surf.numIndexes = ds.numIndexes;
            surf.numVertexes = ds.numVerts;
            surf.type = surfaceType_t.SF_FACE;

            surfResult = surf;
        }

        /*
        ===============
        ParseMesh
        ===============
        */
        private void ParseMesh(idMapFormat.idMapSurface ds, idMapFormat.idMapVertex[] verts, out idDrawSurface surf) {
	        int width, height;
            idBounds bounds;
	        idVector3 tmpVec;

            idGridSurface gridsurf;

	        // we may have a nodraw surface, because they might still need to
	        // be around for movement clipping
            if ((shaders[ds.shaderNum].surfaceFlags & surfaceFlags.SURF_NODRAW) != 0)
            {
                surf = null;
		        return;
	        }

            width = ds.patchWidth;
	        height = ds.patchHeight;

	        // pre-tesseleate
            gridsurf = MapCurveTessalator.SubdividePatchToGrid(width, height, ds.firstVert, drawVerts.ToArray());

            // Load in the material for the surface.
            gridsurf.materials = new idMaterial[1];
            gridsurf.materials[0] = Engine.materialManager.FindMaterial(shaders[ds.shaderNum].shader, ds.lightmapNum);

            // Set the fog index
            gridsurf.fogIndex = ds.fogNum + 1;

	        // copy the level of detail origin, which is the center
	        // of the group of all curves that must subdivide the same
	        // to avoid cracking
            bounds = new idBounds(ds.lightmapVecs[0], ds.lightmapVecs[0] + ds.lightmapVecs[1]);
            gridsurf.lodOrigin = bounds.Maxs * 0.5f;
            tmpVec = bounds.Mins - gridsurf.lodOrigin;
            gridsurf.lodRadius = tmpVec.Length();

            surf = gridsurf;
        }

        /*
        ===============
        ParseFlare
        ===============
        */
         private void ParseFlare(idMapFormat.idMapSurface ds, idMapFormat.idMapVertex[] verts, out idDrawSurface surf) {
	        idDrawSurfaceFlare      flareSurf = new idDrawSurfaceFlare();
	        int i;

	        // get fog volume
	        flareSurf.fogIndex = ds.fogNum + 1;

	        // get shader
            flareSurf.materials = new idMaterial[1];
	        flareSurf.materials[0] = Engine.materialManager.FindMaterial( shaders[ds.shaderNum].shader, -1 ); // justin - fixme this should be LIGHTMAP_BY_VERTEX
	       // if ( r_singleShader->integer && !surf->shader->isSky ) {
		     //   surf->shader = tr.defaultShader;
	        //}

            flareSurf.type = surfaceType_t.SF_FLARE;

	        for ( i = 0 ; i < 3 ; i++ ) {
                flareSurf.origin[i] = ds.lightmapOrigin[i];
                flareSurf.color[i] = ds.lightmapVecs[0][i];
                flareSurf.normal[i] = ds.lightmapVecs[2][i];
	        }

            surf = flareSurf;
        }

        //
        // TessPatchSurface
        //
        private void TessPatchSurface(ref idGridSurface cv)
        {
            int i, j;
	        int rows, irows, vrows;
	        int used;
	        int[] widthTable = new int[MapCurveTessalator.MAX_GRID_SIZE];
	        int[] heightTable = new int[MapCurveTessalator.MAX_GRID_SIZE];
	        float lodError;
            int numVertexes=0;
            int numIndexes = 0;
	        int lodWidth, lodHeight;
	        bool needsNormal;


	        // determine the allowable discrepance
            lodError = 0.53f; //LodErrorForVolume( cv->lodOrigin, cv->lodRadius );

	        // determine which rows and columns of the subdivision
	        // we are actually going to use
	        widthTable[0] = 0;
	        lodWidth = 1;
	        for ( i = 1 ; i < cv.width - 1 ; i++ ) {
		        if ( cv.widthLodError[i] <= lodError ) {
			        widthTable[lodWidth] = i;
			        lodWidth++;
		        }
	        }
	        widthTable[lodWidth] = cv.width - 1;
	        lodWidth++;

	        heightTable[0] = 0;
	        lodHeight = 1;
	        for ( i = 1 ; i < cv.height - 1 ; i++ ) {
		        if ( cv.heightLodError[i] <= lodError ) {
			        heightTable[lodHeight] = i;
			        lodHeight++;
		        }
	        }
	        heightTable[lodHeight] = cv.height - 1;
	        lodHeight++;


	        // very large grids may have more points or indexes than can be fit
	        // in the tess structure, so we may have to issue it in multiple passes

	        used = 0;
	        rows = 0;

            

            cv.startVertex = drawVerts.Count;
            cv.startIndex = drawIndexes.Count;

	        while ( used < lodHeight - 1 ) {

                // Jv - this is just stupid, just a quick hack so everything works.
                vrows = (idMaterialBase.SHADER_MAX_VERTEXES - numVertexes) / (lodWidth);
                irows = (idMaterialBase.SHADER_MAX_INDEXES - numIndexes) / (lodWidth * 6);

		        rows = irows;
		        if ( vrows < irows + 1 ) {
			        rows = vrows - 1;
		        }
		        if ( used + rows > lodHeight ) {
			        rows = lodHeight - used;
		        }

		        for ( i = 0 ; i < rows ; i++ ) {
			        for ( j = 0 ; j < lodWidth ; j++ ) {
                        idDrawVertex vert = new idDrawVertex();
                        idDrawVertex dv;
                        int dvpos = 0;

                        dvpos = heightTable[ used + i ] * cv.width + widthTable[ j ];
                        dv = cv.verts[dvpos];

                        vert.xyz = dv.xyz;
                        vert.st = dv.st;
                        vert.lightmapST = dv.lightmapST;
                        vert.normal = dv.normal;
                        vert.tangent = dv.tangent;
                        vert.binormal = dv.binormal;

                        drawVerts.Add(vert);
                    //    numVertexes++;
			        }
		        }


		        // add the indexes
		        {
			        int w, h;

			        h = rows - 1;
			        w = lodWidth - 1;
			        for ( i = 0 ; i < h ; i++ ) {
				        for ( j = 0 ; j < w ; j++ ) {
					        int v1, v2, v3, v4;

					        // vertex order to be reckognized as tristrips
					        v1 = numVertexes + i * lodWidth + j + 1;
					        v2 = v1 - 1;
					        v3 = v2 + lodWidth;
					        v4 = v3 + 1;

                            drawIndexes.Add((short)v2);
                            drawIndexes.Add((short)v3);
                            drawIndexes.Add((short)v1);

                            drawIndexes.Add((short)v1);
                            drawIndexes.Add((short)v3);
                            drawIndexes.Add((short)v4);

					        numIndexes += 6;
				        }
			        }
		        }
		        used += rows - 1;

                numVertexes += rows * lodWidth;
	        }

            cv.numIndexes = numIndexes;
            cv.numVertexes = numVertexes;

            cv.verts = null;
        }

        //
        // TessAllPatchSurfaces
        //
        private void TessAllPatchSurfaces()
        {
            int vertexCount = drawVerts.Count;
            int indexCount = drawIndexes.Count;
            int numSurfsTessalated = 0;

            for (int i = 0; i < drawSurfs.Length; i++)
            {
                if (drawSurfs[i].type == surfaceType_t.SF_GRID)
                {
                    idGridSurface surf = (idGridSurface)drawSurfs[i];
                    TessPatchSurface(ref surf);
                    drawSurfs[i] = surf;

                    numSurfsTessalated++;
                }
            }

            Engine.common.Printf("TessAllPatchSurfaces: NumSurfs %d NumVertexes %d NumIndexes %d\n", numSurfsTessalated, drawVerts.Count - vertexCount, drawIndexes.Count - indexCount);
        }

        //
        // PrepareSurface
        //
        private idVector3 polyOffsetConstant = new idVector3(0.1f, 0.1f, 0.1f);
        private void PrepareSurface(ref idDrawSurface surface)
        {
            idMaterialBase mtr = idMaterialLocal.GetMaterialBase(surface.materials[0]);

            // If the surface requires the verts to be offseted by the normal, we have to do that here,
            // its easier than displacing it at runtime.
            if (mtr.polygonOffset)
            {
                for (int i = surface.startVertex; i < surface.startVertex + surface.numVertexes; i++)
                {
                    idDrawVertex vert = drawVerts[i];
                    vert.xyz = vert.xyz + (polyOffsetConstant * drawVerts[i].normal);
                    drawVerts[i] = vert;
                }

                // This isn't right at all...
                surface.sort = -0x80000;
            }

            if (mtr.cullType == cullType_t.CT_TWO_SIDED)
            {
                // This isn't right at all...
                surface.sort = -0x140000;
            }
        }

        /*
        ===============
        LoadSurfaces
        ===============
        */
        private void LoadSurfaces(ref idFile bspFile, idMapFormat.lump_t surfs, idMapFormat.lump_t verts, idMapFormat.lump_t indexLump)
        {
	        idMapFormat.idMapSurface[] inSurf;
	        idMapFormat.idMapVertex[] dv;
	        int count;
	        int numFaces, numMeshes, numTriSurfs, numFlares, numVerts, numIndexes;
	        int i;

	        numFaces = 0;
	        numMeshes = 0;
	        numTriSurfs = 0;
	        numFlares = 0;
            numVerts = 0;
            numIndexes = 0;

            // Read in the map surfaces.
            SetLumpPosition( surfs, ref bspFile );
            count = LumpCount( surfs, idMapFormat.idMapSurface.LUMP_SIZE );
            drawSurfs = new idDrawSurface[count];
            inSurf = new idMapFormat.idMapSurface[count];
            for( i = 0; i < count; i++ )
            {
                inSurf[i].InitFromFile( ref bspFile );
            }

            SetLumpPosition( verts, ref bspFile );
            numVerts = LumpCount( verts, idMapFormat.idMapVertex.LUMP_SIZE );
            dv = new idMapFormat.idMapVertex[numVerts];
            drawVerts = new List<idDrawVertex>();
            //drawVerts = new idDrawVertex[numVerts];
            for (i = 0; i < numVerts; i++)
            {
                idDrawVertex v = new idDrawVertex();

                dv[i].InitFromFile( ref bspFile );

                v.xyz = dv[i].xyz;
                v.st = dv[i].st;
                v.lightmapST = dv[i].lightmap;
                v.normal = dv[i].normal;
                int r = dv[i].color[0];
                int g = dv[i].color[1];
                int b = dv[i].color[2];

                v.color.X = r;
                v.color.Y = g;
                v.color.Z = b;
                v.color.W = dv[i].color[3];
                v.color.Normalize();
                drawVerts.Add(v);
            }

            SetLumpPosition(indexLump, ref bspFile );
            numIndexes = LumpCount( indexLump, sizeof( int ) );
            drawIndexes = new List<short>();
            for (i = 0; i < numIndexes; i++)
            {
                drawIndexes.Add((short)bspFile.ReadInt());
            }

	        for ( i = 0 ; i < count ; i++ ) {
                if ((shaders[inSurf[i].shaderNum].surfaceFlags & surfaceFlags.SURF_NODRAW) != 0)
                {
                    drawSurfs[i] = null;
                    continue;
                }

		        switch ( (idMapFormat.idMapSurfaceType)inSurf[i].surfaceType ) {
		        case idMapFormat.idMapSurfaceType.MST_PATCH:
			        ParseMesh( inSurf[i], dv, out drawSurfs[i] );
			        numMeshes++;
			        break;
		        case idMapFormat.idMapSurfaceType.MST_TRIANGLE_SOUP:
			        ParseTriSurf( inSurf[i], dv, out drawSurfs[i] );
			        numTriSurfs++;
			        break;
		        case idMapFormat.idMapSurfaceType.MST_PLANAR:
                    ParseFace(inSurf[i], dv, out drawSurfs[i]);
			        numFaces++;
			        break;
		        case idMapFormat.idMapSurfaceType.MST_FLARE:
			        ParseFlare( inSurf[i], dv, out drawSurfs[i] );
			        numFlares++;
			        break;
		        default:
			        Engine.common.ErrorFatal( "Bad surfaceType" );
                    break;
		        }

                PrepareSurface(ref drawSurfs[i]);
	        }

        #if PATCH_STITCHING
	        R_StitchAllPatches();
        #endif

	        //R_FixSharedVertexLodError();

        #if PATCH_STITCHING
	        R_MovePatchSurfacesToHunk();
        #endif

            // Tessalate the patch surfaces, upload them to the end of the vertex/index buffers,
            // and correct the patch surface vert/index buffers and destroy the stored control points.
            TessAllPatchSurfaces();

            inSurf = null;
            dv = null;

	        Engine.common.Printf("...loaded %d faces, %i meshes, %i trisurfs, %i flares\n",
			           numFaces, numMeshes, numTriSurfs, numFlares );
        }

        //
        // BuildVertexIndexBuffer
        //
        private void BuildVertexIndexBuffer()
        {
            Engine.common.Printf("R_BuildMapVertexIndexBuffer: Creating Vertex/Index Buffer...\n");
            vertexBuffer = new VertexBuffer(Globals.graphics3DDevice, idRenderGlobals.idDrawVertexDeclaration, drawVerts.Count, BufferUsage.WriteOnly);
            vertexBuffer.SetData<idDrawVertex>(drawVerts.ToArray());

            indexBuffer = new IndexBuffer(Globals.graphics3DDevice, IndexElementSize.SixteenBits, drawIndexes.Count, BufferUsage.WriteOnly);
            indexBuffer.SetData<short>(drawIndexes.ToArray());
        }

        //
        // DisposeOfNonPersistantMemory
        //
        private void DisposeOfNonPersistantMemory()
        {
            Engine.common.Printf("R_MapDisposeOfNonPersistantMemory: Destroying Non Persistant Data...\n");

            header.lumps = null;
            drawVerts.Clear();
            drawIndexes.Clear();
            drawVerts = null;
            drawIndexes = null;
            shaders = null;
         //   brushes = null;
         //   brushsides = null;

            // Force the GC to reclaim the unneeded memory.
            Engine.common.ForceGCCollect();
        }

        //
        // MapLoadIsActive
        //
        private bool MapLoadIsActive()
        {
            return (lightGridData == null);
        }

        //
        // UpdateLoadScreenThread
        //
        private bool loadScreenThreadActive = false;
        private void UpdateLoadScreenThread()
        {
            loadScreenThreadActive = true;
            while (MapLoadIsActive())
            {
                Globals.UpdateLoadingScreen();
                System.Threading.Thread.Sleep(60);
            }
            loadScreenThreadActive = false;
        }

        //
        // SyncLoadScreenThread
        //
        private void SyncLoadScreenThread()
        {
            while (loadScreenThreadActive == true)
            {
                System.Threading.Thread.Sleep(10);
            }
        }

        //
        // RenderMap
        //
        public float RenderMap(idVector3 pvsorigin)
        {
            float zFar;

            zFar = vis.SetSurfacesVisibility(pvsorigin, ref drawSurfs);

            Globals.SetVertexIndexBuffers(vertexBuffer, indexBuffer);
            Globals.SortSurfaces(0, ref drawSurfs);

            return zFar;
        }

        //
        // SetSurfaceSort
        //
        private void SetSurfaceSort()
        {
            for (int i = 0; i < drawSurfs.Length; i++)
            {
                idDrawSurface surf = drawSurfs[i];
                float fartherLength = 0;
                int fartherIndex = 0;

                //int index = drawIndexes[surf.startIndex];
                for (int c = 1; c < surf.numVertexes; c++)
                {
                    idVector3 p = drawVerts[surf.startVertex + fartherIndex].xyz - drawVerts[surf.startVertex + c].xyz;

                    float v = p.Length();

                    if (v > fartherLength)
                    {
                        fartherLength = v;
                        fartherIndex = c;
                    }
                }

                idVector3 midPoint = drawVerts[surf.startVertex].xyz - drawVerts[surf.startVertex + fartherIndex].xyz;

                if (surf.sort == 0)
                {
                    surf.sort += midPoint.LengthSqr();
                }
                else
                {
                    surf.sort -= midPoint.LengthSqr();
                }
                drawSurfs[i] = surf;
            }
        }

        //
        // LoadMap
        //
        public void LoadMap(string mappath)
        {
            idFile bspFile;
            idThread loadScreenThread;

            Engine.common.Printf("------ R_LoadMap ------\n");

            // Create the loadscreen thread and start it.
            loadScreenThread = Engine.Sys.CreateThread("map_thread", () => UpdateLoadScreenThread());
            loadScreenThread.Start(null);

            baseName = "maps/" + mappath;
            name = baseName + ".xnb";

            bspFile = Engine.fileSystem.OpenEncryptedFileRead(name, true);

            // This should have been caught long before this.
            if (bspFile == null)
            {
                Engine.common.ErrorFatal("R_LoadMap: Failed to open map %s \n", mappath);
            }

            // Load in the header and ensure the iden and version's are valid.
            header.InitFromFile(ref bspFile);

            // load into heap
            
            LoadShaders(ref bspFile, header.lumps[idMapFormat.LUMP_SHADERS]);
            LoadLightmaps(ref bspFile, header.lumps[idMapFormat.LUMP_LIGHTMAPS]);
            LoadPlanes(ref bspFile, header.lumps[idMapFormat.LUMP_PLANES]);
            LoadFogs(ref bspFile, header.lumps[idMapFormat.LUMP_FOGS], header.lumps[idMapFormat.LUMP_BRUSHES], header.lumps[idMapFormat.LUMP_BRUSHSIDES]);
            LoadSurfaces(ref bspFile, header.lumps[idMapFormat.LUMP_SURFACES], header.lumps[idMapFormat.LUMP_DRAWVERTS], header.lumps[idMapFormat.LUMP_DRAWINDEXES]);
           // 
            LoadMarksurfaces(ref bspFile, header.lumps[idMapFormat.LUMP_LEAFSURFACES]);
            LoadNodesAndLeafs(ref bspFile, header.lumps[idMapFormat.LUMP_NODES], header.lumps[idMapFormat.LUMP_LEAFS]);
            LoadSubmodels(ref bspFile, header.lumps[idMapFormat.LUMP_MODELS]);
            LoadVisibility(ref bspFile, header.lumps[idMapFormat.LUMP_VISIBILITY]);
            LoadEntities(ref bspFile, header.lumps[idMapFormat.LUMP_ENTITIES]);
            LoadLightGrid(ref bspFile, header.lumps[idMapFormat.LUMP_LIGHTGRID]);

            // Load CM lumps.
            LoadLeafBrushes(ref bspFile, header.lumps[idMapFormat.LUMP_LEAFBRUSHES]);

            // Calculate the surface sort value.
            SetSurfaceSort();

            // Close the bsp file.
            Engine.fileSystem.CloseFile(ref bspFile);

            // Wait for the loading thread to exit.
            SyncLoadScreenThread();

            // Build the vertex/index buffer from drawverts/drawindexes.
            BuildVertexIndexBuffer();

            // Destroy anything we don't need to keep, and free up non-needed memory.
            DisposeOfNonPersistantMemory();

            // Init the collision model, I'm coupling the collision with the map to avoid having to parse the map file twice.
            // Since were not supporting dedicated servers this won't be a problem.
            collisionmodel = Engine.collisionModelManager.LoadCollisonModelFromBsp(mappath, this);

            Engine.common.Printf("Map Loaded Successfully...\n");
        }
    }
}
