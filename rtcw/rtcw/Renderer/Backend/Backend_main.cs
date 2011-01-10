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

// Backend_main.cs (c) 2010 JV Software
//

// No matter how many sorting methods I try its always too damn slow :/.
#define USE_QSORT 

using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using idLib.Engine.Public;
using rtcw.Renderer.Models;

namespace rtcw.Renderer.Backend
{
    //
    // idRenderBackend
    //
    public class idRenderBackend
    {
        backEndState_t state = new backEndState_t();
        backEndData_t[]  backEndData = new backEndData_t[idRenderGlobals.SMP_FRAMES];

        private int r_firstSceneDrawSurf = 0;

        private int r_numdlights = 0;
        private int r_firstSceneDlight = 0;

        private int r_numcoronas = 0;
        private int r_firstSceneCorona = 0;

        private int r_numentities = 0;
        private int r_firstSceneEntity = 0;

        private int r_numpolys = 0;
        private int r_firstScenePoly = 0;

        private int r_numpolyverts = 0;

        private int skyboxportal = 0;
        private int drawskyboxportal = 0;

        
        private idDrawVertex[] quadVertexes = new idDrawVertex[4];
        private short[] quadIndexes = new short[] { 0, 1, 2, 0, 2, 3 };

        private Viewport viewport;

        //
        // idRenderBackend
        //
        public idRenderBackend()
        {
            for (int i = 0; i < idRenderGlobals.SMP_FRAMES; i++)
            {
                backEndData[i] = new backEndData_t(i);
            }
            for (int i = 0; i < 4; i++)
            {
                quadVertexes[i] = new idDrawVertex();
            }

            Shade.Init();
            

            state.smpFrame = 0;
        }

        //
        // AddDrawSurface
        //
        public void AddDrawSurface(idDrawSurface surf)
        {
            backEndData[smpFrame].PushDrawSurface(surf);
        }

        //
        // NumSurfaces
        //
        public int NumSurfaces
        {
            get
            {
                return backEndData[smpFrame].DrawSurfsCount;
            }
        }

        //
        // smpFrame
        //
        public int smpFrame
        {
            get
            {
                return state.smpFrame;
            }
        }

        //
        // AllocRefDef
        //
        public idRefdefLocal AllocRefDef()
        {
            return backEndData[smpFrame].GetNextEntity();
        }

        //
        // idRenderCommand
        //
        public idRenderCommand GetCommandBuffer()
        {
            if (backEndData[state.smpFrame].numRenderCommands >= idRenderGlobals.MAX_RENDER_COMMANDS)
            {
                Engine.common.ErrorFatal("R_GetCommandBuffer: Numrendercommands > MAX_RENDER_COMMANDS\n");
            }

            if (backEndData[state.smpFrame].commands[backEndData[state.smpFrame].numRenderCommands] == null)
            {
                backEndData[state.smpFrame].commands[backEndData[state.smpFrame].numRenderCommands] = new idRenderCommand();
            }

            return backEndData[state.smpFrame].commands[backEndData[state.smpFrame].numRenderCommands++];
        }


        //
        // Cmd_DrawStrechImage
        //
        private void Cmd_DrawStrechImage(idRenderCommand cmd)
        {
            Shade.Set2DOrthoMode();

            // Setup our quad coordinates.
            quadVertexes[0].xyz.X = cmd.x;
            quadVertexes[0].xyz.Y = cmd.y;
            quadVertexes[0].st.X = 0.5f / cmd.w;
            quadVertexes[0].st.Y = 0.5f / cmd.h;

            quadVertexes[1].xyz.X = cmd.x + cmd.w;
            quadVertexes[1].xyz.Y = cmd.y;
            quadVertexes[1].st.X = ( cmd.h - 0.5f ) / cmd.h;
            quadVertexes[1].st.Y = 0.5f / cmd.w;

            quadVertexes[2].xyz.X = cmd.x + cmd.w;
            quadVertexes[2].xyz.Y = cmd.y + cmd.h;
            quadVertexes[2].st.X = (cmd.h - 0.5f) / cmd.h;
            quadVertexes[2].st.Y = (cmd.w - 0.5f) / cmd.w;

            quadVertexes[3].xyz.X = cmd.x;
            quadVertexes[3].xyz.Y = cmd.y + cmd.h;
            quadVertexes[3].st.X = 0.5f / cmd.h;
            quadVertexes[3].st.Y = (cmd.w - 0.5f) / cmd.w;

            Shade.BindImage(cmd.image);
            Shade.DrawPrimitives(4, 6, quadVertexes, quadIndexes);
        }

        //
        // BeginSurface
        //
        private void BeginSurface(idMaterial material, int fogNum)
        {
            Globals.tess.numIndexes = 0;
            Globals.tess.numVertexes = 0;
            Globals.tess.shader = idMaterialLocal.GetMaterialBase( ref material );
            Globals.tess.fogNum = fogNum;
            Globals.tess.dlightBits = 0;
            Globals.tess.currentStageIteratorFunc = Globals.tess.shader.optimalStageIteratorFunc;
            Globals.tess.vertexBufferStart = 0;
            Globals.tess.indexBufferStart = 0;
            Globals.tess.indexBufferSize = 0;
            Globals.tess.vertexBufferSize = 0;
        }

        //
        // SetViewportAndScissor
        //
        private void SetViewportAndScissor(idRefdefLocal viewParms)
        {
            if (viewport.Width == viewParms.width && viewport.Height == viewParms.height && viewParms.x == viewport.X && viewParms.y == viewport.Y)
                return;

            viewport.MaxDepth = 1.0f;
            viewport.MinDepth = 0.0f;
            viewport.Width = viewParms.width;
            viewport.Height = viewParms.height;
            viewport.X = viewParms.x;
            viewport.Y = viewParms.y;

            Globals.graphics3DDevice.Viewport = viewport;

	        // set the window clipping
	      //  qglViewport(    backEnd.viewParms.viewportX,
			//		        backEnd.viewParms.viewportY,
				//	        backEnd.viewParms.viewportWidth,
					//        backEnd.viewParms.viewportHeight );

        // TODO: insert handling for widescreen?  (when looking through camera)
	        //qglScissor(     backEnd.viewParms.viewportX,
				//	        backEnd.viewParms.viewportY,
				//	        backEnd.viewParms.viewportWidth,
				//	        backEnd.viewParms.viewportHeight );
        }

        //
        // EndSurface
        //
        private void EndSurface()
        {
            if( Globals.tess.indexBufferSize == 0 && Globals.tess.numIndexes == 0 )
            {
                return;
            }

            // Setup culling for this shader.
            Shade.SetCullMode(Globals.tess.shader.cullType);

            if (Globals.tess.shader.polygonOffset)
            {
             //   Globals.graphics3DDevice.DepthStencilState = DepthStencilState.None;
            }

            //
            // call off to shader specific tess end function
            //
            if (Globals.tess.shader.fogParms != null)
            {
                Shade.EnableFog(Globals.tess.shader.fogParms);
            }
            else
            {
                Globals.tess.currentStageIteratorFunc();
            }

            if (Globals.tess.shader.polygonOffset)
            {
              //  Globals.graphics3DDevice.DepthStencilState = DepthStencilState.Default;
            }

            // Reset numverts and num indexes.
            Globals.tess.numVertexes = 0;
            Globals.tess.numIndexes = 0;
            Globals.tess.indexBufferSize = 0;
            Globals.tess.frame = 0;
            Globals.tess.startVertex = 0;
            Globals.tess.startIndex = 0;
            Globals.tess.vertexBufferStart = 0;
            Globals.tess.vertexBufferSize = 0;
        }

        //
        // SetSurfaceOffset
        //
        public void SetSurfaceOffset(int surfoffset)
        {
            backEndData[smpFrame].SetSurfaceOffset(surfoffset);
        }


        //
        // Cmd_DrawStrechMaterial
        //
        private void Cmd_DrawStrechMaterial(idRenderCommand cmd)
        {
            Shade.Set2DOrthoMode();

            if (Globals.tess.shader == null || cmd.shader.GetName() != Globals.tess.shader.name)
            {
                if (Globals.tess.numIndexes > 0)
                {
                    EndSurface();
                }

                BeginSurface(cmd.shader, 0);
            }

            Globals.tess.UploadVertex(cmd.x, cmd.y, 0, cmd.s1, cmd.t1);
            Globals.tess.UploadVertex(cmd.x + cmd.w, cmd.y, 0, cmd.s2, cmd.t1);
            Globals.tess.UploadVertex(cmd.x + cmd.w, cmd.y + cmd.h, 0, cmd.s2, cmd.t2);
            Globals.tess.UploadVertex(cmd.x, cmd.y + cmd.h, 0, cmd.s1, cmd.t2);

            for (int i = 0; i < quadIndexes.Length; i++)
            {
                Globals.tess.UploadIndex(quadIndexes[i]);
            }

            EndSurface();
        }

        //
        // Cmd_SwapBuffers
        //
        private void Cmd_SwapBuffers(idRenderCommand cmd)
        {
            // Finish any surfaces that are left.
            if (Globals.tess.numIndexes > 0)
            {
                EndSurface();
            }

            Shade.DisableFog();

            Globals.graphics3DDevice.Present();
        }

        //
        // Cmd_SetColor
        //
        private void Cmd_SetColor(idRenderCommand cmd)
        {
            Shade.SetColor(cmd.color[0], cmd.color[1], cmd.color[2], cmd.color[3]);
        }

        //
        // SmpThreadHasWork
        //
        private bool SmpThreadHasWork(int smpFrame)
        {
            return backEndData[smpFrame].executingFrameBuffer;
        }

        //
        // NumSmpCommands
        //
        private int GetNumOfSmpCommands( int smpFrame )
        {
            return backEndData[smpFrame].numRenderCommands;
        }

        /*
        =================
        RB_BeginDrawingView

        Any mirrored or portaled views have already been drawn, so prepare
        to actually render the visible surfaces for this view
        =================
        */
        private void RB_BeginDrawingView()
        {
            SetViewportAndScissor(state.refdef);

            if (state.refdef.rdflags == idRenderType.RF_DEPTHHACK || state.refdef.rdflags == idRenderType.RDF_NOWORLDMODEL)
            {
                Globals.graphics3DDevice.Clear(ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
            }
        }

        //
        // DrawSurface
        //
        private void DrawSurface(idDrawSurface surf, int offset)
        {
            BeginSurface(surf.materials[0], 0);

            Globals.tess.vertexBufferStart = surf.startVertex; ;
            Globals.tess.indexBufferStart = surf.startIndex;
            Globals.tess.indexBufferSize = surf.numIndexes;
            Globals.tess.vertexBufferSize = surf.numVertexes;
            Globals.tess.frame = offset;

            EndSurface();
        }

        //
        // DrawSurface
        //
        private void DrawUserSurface(idDrawSurface surf, int offset)
        {
            BeginSurface(surf.materials[0], 0);

            Globals.tess.startVertex = surf.startVertex; ;
            Globals.tess.startIndex = surf.startIndex;
            Globals.tess.numIndexes = surf.numIndexes;
            Globals.tess.numVertexes = surf.numVertexes;
            Globals.tess.frame = offset;

            EndSurface();
        }

        //
        // Cmd_RenderScene
        //
        private void Cmd_RenderScene(int smpFrame, idRenderCommand cmd)
        {
            idRefdefLocal refdef;
            int refentity_num = -2;

            // Sort out draw surfaces.
#if USE_QSORT
            if (cmd.refdef.rdflags != idRenderType.RDF_NOWORLDMODEL)
            {
                backEndData[smpFrame].SortSurfaces(cmd.startSurface);
            }
#endif
            // Everyone surface here should have the same refdef.
            refdef = cmd.refdef;

            state.refdef = refdef;
            RB_BeginDrawingView();

            Shade.PushDrawMatrix(refdef);

            // Render any skeletal models that weren't tessalated yet. 
            for (int i = 0; i < refdef.num_entities; i++)
            {
                idDrawSurface[] surfaces;

                if (refdef.entities[i].hModel != null)
                {
                    surfaces = ((idModelLocal)refdef.entities[i].hModel).BackendTessModel(refdef.entities[i].frame, refdef.entities[i].torsoFrame);

                    if (surfaces != null)
                    {
                        Shade.CreateTranslateRotateMatrix(refdef.entities[i]);

                        for (int c = 0; c < surfaces.Length; c++)
                        {
                            DrawUserSurface(surfaces[c], 0);
                        }
                    }
                }
            }

            backEndData[smpFrame].SetBaseSourceSurface(cmd.startSurface);
#if !USE_QSORT
            for (shaderSort_t sort = shaderSort_t.SS_BAD + 1; sort < shaderSort_t.SS_NEAREST; sort++)
            {
#endif
                // Render the rest of the tessalated data.
                for (int i = cmd.startSurface; i < cmd.endSurface; i++)
                {
                    idSortSurface sortsurf = backEndData[smpFrame].GetNextSortSurface();
                    idDrawSurface surf = backEndData[smpFrame].GetDrawSurface(sortsurf.index);

                    // Update the modelviewproj matrix if we have a different surf with a different matrix.
                    if (sortsurf.entitymatrix != refentity_num)
                    {
                        Shade.BindVertexIndexBuffer(sortsurf.vertexBuffer, sortsurf.indexBuffer);

                        if (sortsurf.entitymatrix >= 0)
                        {
                            Shade.CreateTranslateRotateMatrix(refdef.entities[sortsurf.entitymatrix]);
                        }
                        else
                        {
                            Shade.PushDrawMatrix(refdef);
                        }

                        refentity_num = sortsurf.entitymatrix;
                        
                    }
                    DrawSurface(surf, sortsurf.offset);
                }
#if !USE_QSORT
            }
#endif
            
        }

        //
        // SetVertexIndexBuffers
        //
        public void SetVertexIndexBuffers(VertexBuffer vertexBuffer, IndexBuffer indexBuffer)
        {
            backEndData[smpFrame].SetVertexIndexBuffers(vertexBuffer, indexBuffer);
        }

        //
        // GetRenderCommand
        //
        private idRenderCommand GetRenderCommand(int smpFrame, int index)
        {
            return backEndData[smpFrame].commands[index];
        }

        //
        // IssueRenderCommandsWorker
        //
        public void IssueRenderCommandsWorker(int smpFrame)
        {
            while (true)
            {
                bool smpIsActive = (Globals.r_smp.GetValueInteger() != 0);

                if (SmpThreadHasWork(smpFrame) || smpIsActive == false)
                {
                    for (int i = 0; i < GetNumOfSmpCommands( smpFrame ); i++)
                    {

                        idRenderCommand cmd = GetRenderCommand( smpFrame, i );
                        switch (cmd.type)
                        {
                            case renderCommandType.RC_SET_COLOR:
                                Cmd_SetColor(cmd);
                                break;
                            case renderCommandType.RC_STRETCH_IMAGE:
                                Cmd_DrawStrechImage(cmd);
                                break;
                            case renderCommandType.RC_SWAP_BUFFERS:
                                Cmd_SwapBuffers(cmd);
                                break;
                            case renderCommandType.RC_RENDER_SCENE:
                                Cmd_RenderScene(smpFrame, cmd);
                                break;
                            case renderCommandType.RC_STRETCH_PIC:
                                Cmd_DrawStrechMaterial(cmd);
                                break;
                            default:
                                Engine.common.ErrorFatal("R_IssueRenderCommandsWorker: Unknown command type\n");
                                break;
                        }
                    }

                    // Were down wait for the next command list.
                    Globals.backEnd.backEndData[smpFrame].Pause();

                    if (Globals.r_smp.GetValueInteger() == 0)
                    {
                        break;
                    }
                }
                else
                {
                    Thread.Sleep(1);
                }
            }
        }

        //
        // SyncRenderThread
        //
        public bool SyncRenderThread()
        {
            int smpWaitThread = state.smpFrame;

            if (Globals.r_smp.GetValueInteger() != 0)
            {
                smpWaitThread = state.smpFrame ^ 1;
            }

            if (Globals.r_smp.GetValueInteger() == 0)
                return true;

            // Wait for the other smp thread to finish.
            while (backEndData[smpWaitThread].SmpRunning)
            {
                Thread.Sleep(1);
            }

            return true;
        }

        //
        // SetSurfaceRefdef
        //
        public void SetSurfaceRefdef(int refdefhandle)
        {
            backEndData[state.smpFrame].SetSurfaceRefdef(refdefhandle);
        }

        //
        // SetSurfaceEntity
        //
        public void SetSurfaceEntity(int entityhandle)
        {
            backEndData[state.smpFrame].SetSurfaceEntity(entityhandle);
        }

        //
        // ResetFrame
        //
        private void ResetFrame()
        {
            for (int i = 0; i < backEndData[state.smpFrame].numRenderCommands; i++)
            {
                backEndData[state.smpFrame].commands[i].bones = null;
            }
            backEndData[state.smpFrame].numRenderCommands = 0;

            r_firstSceneDrawSurf = 0;

            r_numdlights = 0;
            r_firstSceneDlight = 0;

            r_numcoronas = 0;
            r_firstSceneCorona = 0;

            r_numentities = 0;
            r_firstSceneEntity = 0;

            r_numpolys = 0;
            r_firstScenePoly = 0;

            r_numpolyverts = 0;
        }

        //
        // IssueRenderCommands
        //
        public void IssueRenderCommands(bool runPerformanceCounters)
        {
            if (Globals.r_smp.GetValueInteger() == 0)
            {
                IssueRenderCommandsWorker(0);
                return;
            }
            SyncRenderThread();

            backEndData[state.smpFrame].Execute();
        }

        //
        // ToggleSMPFrame
        //
        public void ToggleSmpFrame()
        {
            if (Globals.r_smp.GetValueInteger() != 0)
            {
                // use the other buffers next frame, because another CPU
                // may still be rendering into the current ones
                state.smpFrame ^= 1;
            }
            else
            {
                state.smpFrame = 0;
            }

            ResetFrame();
        }
    }

    //
    // backEndCounters_t
    //
    struct backEndCounters_t
    {
        public int c_surfaces, c_shaders, c_vertexes, c_indexes, c_totalIndexes;
        public float c_overDraw;

        public int c_dlightVertexes;
        public int c_dlightIndexes;

        public int c_flareAdds;
        public int c_flareTests;
        public int c_flareRenders;

        public int msec;               // total msec for backend run
    };

    // all state modified by the back end is seperated
    // from the front end state
    struct backEndState_t
    {
        public int smpFrame;
        public idRefdefLocal refdef;
        public viewParms_t viewParms;
        public orientationr_t or;
        public backEndCounters_t pc;
        public bool isHyperspace;
        public idRenderEntityLocal currentEntity;
        public bool skyRenderedThisView;       // flag for drawing sun

        public bool projection2D;      // if qtrue, drawstretchpic doesn't need to change modes
        public byte color2D_r;
        public byte color2D_g;
        public byte color2D_b;
        public byte color2D_a;
        public bool vertexes2D;        // shader needs to be finished
        public idRenderEntityLocal entity2D;     // currentEntity will point at this when doing 2D rendering
    };

    //
    // idSortSurface
    //
    struct idSortSurface
    {
        public int sort;
        public int index;
        public int entitymatrix;
        public int refdef;
        public int offset;
        public int numChildSurfaces;
        public int startChildSurface;

        public VertexBuffer vertexBuffer;
        public IndexBuffer indexBuffer;

        public void SetFromSurface(idSortSurface surf)
        {
            sort = surf.sort;
            index = surf.index;
            entitymatrix = surf.entitymatrix;
            refdef = surf.refdef;
            offset = surf.offset;
        }
    }

    // all of the information needed by the back end must be
    // contained in a backEndData_t.  This entire structure is
    // duplicated so the front and back end can run in parallel
    // on an SMP machine
    class backEndData_t {
	    private idDrawSurface[] drawSurfsInternal;

        private idSortSurface[] sortedSurfaces;
        private idSortSurface[] sortedChildSurfaces;
        private int numDrawSurfs;
        public int numSortedSurfs;
        public int numChildSurfs;
        public int lastChildEntity = -1;

        public dlight_t[] dlights;
        public corona_t[] coronas;          //----(SA)

        public idRefdefLocal[] entities;
        public int numEntities;

	    public srfPoly_t[] polys;
	    public polyVert_t[] polyVerts;
        public idRenderCommand[] commands;
        public int numRenderCommands;
        public bool executingFrameBuffer;
        private idThread smpthread;
        private int smpFrameNum;

        private int surfrefdef = -1;
        private int surfentity = -1;
        private int surfoffset = 0;

        private int currentSortSurface = 0;

        private VertexBuffer surfVertexBuffer;
        private IndexBuffer surfIndexBuffer;

        //
        // SetSurfaceRefdef
        //
        public void SetSurfaceRefdef(int refdefhandle)
        {
            surfrefdef = refdefhandle;
            surfentity = -1;
            surfoffset = 0;
            lastChildEntity = -2;
        }

        //
        // SetSurfaceEntity
        //
        public void SetSurfaceEntity(int entityhandle)
        {
            surfoffset = 0;
            surfentity = entityhandle;
        }

        //
        // SetSurfaceOffset
        //
        public void SetSurfaceOffset(int surfoffset)
        {
            this.surfoffset = surfoffset;
        }

        //
        // UpdateRefdef
        //
        public void UpdateRefdef(idRefdefLocal refdef)
        {
            entities[refdef.refnum] = refdef;
        }

        private void q_sort( int left, int right)
        {
            int i = left - 1,
                j = right;

            while (true)
            {
                float d = sortedSurfaces[left].sort;
                do i++; while (sortedSurfaces[i].sort < d);
                do j--; while (sortedSurfaces[j].sort > d);

                if (i < j)
                {
                    idSortSurface tmp = sortedSurfaces[i];
                    sortedSurfaces[i] = sortedSurfaces[j];
                    sortedSurfaces[j] = tmp;
                }
                else
                {
                    if (left < j) q_sort(left, j);
                    if (++j < right) q_sort( j, right);
                    return;
                }
            }
        }

        //
        // SortSurfaces
        //
        public void SortSurfaces(int baseVert)
        {
            if ((numSortedSurfs - baseVert) < 1)
                return;

            q_sort(baseVert, numSortedSurfs - 1);
        }



        //
        // GetDrawSurface
        //
        public idDrawSurface GetDrawSurface(int index)
        {
            return drawSurfsInternal[index];
        }

        //
        // GetSortSurface
        //
        public idSortSurface GetNextSortSurface()
        {
            idSortSurface sortedSurface = sortedSurfaces[currentSortSurface];
            if (sortedSurface.numChildSurfaces <= 0)
            {
                currentSortSurface++; // Go to the next command.
                if (sortedSurface.startChildSurface > 0)
                {
                    return GetNextSortSurface();
                }
                return sortedSurface;
            }

            int childIndex = sortedSurfaces[currentSortSurface].startChildSurface++;
            sortedSurfaces[currentSortSurface].numChildSurfaces--;

            return sortedChildSurfaces[childIndex];
        }

        //
        // SetBaseSourceSurface
        //
        public void SetBaseSourceSurface(int baseval)
        {
           // currentSortSurface = baseval;
        }

        public backEndData_t(int smpFrameNum )
        {
       //     for (int i = 0; i < idRenderGlobals.MAX_RENDER_COMMANDS; i++)
        //    {
       //         commands[i] = new idRenderCommand();
       //     }
            drawSurfsInternal = new idDrawSurface[idRenderGlobals.MAX_DRAWSURFS];
            sortedSurfaces = new idSortSurface[idRenderGlobals.MAX_DRAWSURFS];
            sortedChildSurfaces = new idSortSurface[idRenderGlobals.MAX_DRAWSURFS / 4];
            numDrawSurfs = 0;

            dlights = new dlight_t[idRenderSystem.MAX_DLIGHTS];
            coronas = new corona_t[idRenderSystem.MAX_CORONAS];
            entities = new idRefdefLocal[idRenderSystem.MAX_ENTITIES];
            polys = new srfPoly_t[idRenderGlobals.MAX_POLYS];
            polyVerts = new polyVert_t[idRenderGlobals.MAX_POLYVERTS];
            commands = new idRenderCommand[idRenderGlobals.MAX_RENDER_COMMANDS];

            numRenderCommands = 0;
            numEntities = 0;
            smpthread = null;
            executingFrameBuffer = false;
            this.smpFrameNum = smpFrameNum;
        }

        //
        // InitSortSurface
        //
        private void InitSortSurface(ref idSortSurface surf)
        {
            idLib.Math.idVector3 view = idLib.Math.idVector3.vector_origin;
            float viewSort;

            for (int i = 0; i < 3; i++)
            {
                view += entities[surfrefdef].vieworg + entities[surfrefdef].viewaxis[i];
            }

            viewSort = view.Length();

            surf.sort = (int)(viewSort - drawSurfsInternal[numDrawSurfs].sort);

            surf.index = numDrawSurfs;

            surf.offset = surfoffset;
            surf.refdef = surfrefdef;
            surf.entitymatrix = surfentity;

            surf.vertexBuffer = surfVertexBuffer;
            surf.indexBuffer = surfIndexBuffer;

            surf.startChildSurface = 0;
            surf.numChildSurfaces = 0;
        }


        //
        // PushDrawSurface
        //
        public void PushDrawSurface(idDrawSurface surf)
        {
            drawSurfsInternal[numDrawSurfs] = surf;
            
            // Group non map surfaces(entity surfaces) 
            if (surfentity >= 0)
            {
                int childSurfNum;

                // If were on a different entity, setup a new sort surface if not setup the first entity surface for children.
                if (lastChildEntity != surfentity)
                {
                    InitSortSurface(ref sortedSurfaces[numSortedSurfs]);

                    sortedSurfaces[numSortedSurfs].startChildSurface = numChildSurfs;
                    sortedSurfaces[numSortedSurfs].numChildSurfaces = 0;

                    numSortedSurfs++;
                    lastChildEntity = surfentity;
                }

                // Get the next child surf handle.
                childSurfNum = sortedSurfaces[numSortedSurfs-1].startChildSurface + sortedSurfaces[numSortedSurfs-1].numChildSurfaces;

                // Init the child surface.
                InitSortSurface(ref sortedChildSurfaces[childSurfNum]);

                // Increment the number of child surfaces.
                sortedSurfaces[numSortedSurfs-1].numChildSurfaces++;
                numChildSurfs++;
            }
            else
            {
                InitSortSurface(ref sortedSurfaces[numSortedSurfs]);

                numSortedSurfs++;
                lastChildEntity = -1;
            }

            numDrawSurfs++;
        }

        //
        // DrawSurfsCount
        //
        public int DrawSurfsCount
        {
            get
            {
                return numDrawSurfs;
            }
        }

        //
        // SmpRunning
        //
        public bool SmpRunning
        {
            get
            {
                return executingFrameBuffer;
            }
        }

        //
        // SetVertexIndexBuffers
        //
        public void SetVertexIndexBuffers(VertexBuffer vertexBuffer, IndexBuffer indexBuffer)
        {
            surfVertexBuffer = vertexBuffer;
            surfIndexBuffer = indexBuffer;
        }

        //
        // GetNextEntity
        //
        public idRefdefLocal GetNextEntity()
        {
            if (entities[numEntities] == null)
            {
                entities[numEntities] = new idRefdefLocal();
            }

            entities[numEntities].refnum = numEntities;
           
            entities[numEntities].areamaskModified = false;
            entities[numEntities].floatTime = 0;
            entities[numEntities].fov_x = 0;
            entities[numEntities].fov_y = 0;
            entities[numEntities].height = 0;
            entities[numEntities].num_entities = 0;
            entities[numEntities].rdflags = 0;
            entities[numEntities].time = 0;
            entities[numEntities].width = 0;
            entities[numEntities].x = 0;
            entities[numEntities].y = 0;
            
            return entities[numEntities++];
        }

        //
        // Pause
        //
        public void Pause()
        {
            executingFrameBuffer = false;
            numEntities = 0;
            numDrawSurfs = 0;
            numSortedSurfs = 0;
            numChildSurfs = 0;
            currentSortSurface = 0;
            lastChildEntity = -1;
        }

        //
        // Execute
        //
        public void Execute()
        {
            executingFrameBuffer = true;

            if (smpthread == null)
            {
                smpthread = Engine.Sys.CreateThread("idRenderSmp" + smpFrameNum, () => Globals.backEnd.IssueRenderCommandsWorker(smpFrameNum));
                smpthread.Start(null);
            }
        }
    };
}
