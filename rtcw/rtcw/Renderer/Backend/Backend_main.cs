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

using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using idLib.Engine.Public;

namespace rtcw.Renderer.Backend
{
    //
    // idRenderBackend
    //
    public unsafe class idRenderBackend
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
                return backEndData[smpFrame].numDrawSurfs;
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
        // EndSurface
        //
        private void EndSurface()
        {
            if( Globals.tess.indexBufferSize == 0 && Globals.tess.numIndexes == 0 )
            {
                return;
            }

            //
            // call off to shader specific tess end function
            //
            Globals.tess.currentStageIteratorFunc();

            // Reset numverts and num indexes.
            Globals.tess.numVertexes = 0;
            Globals.tess.numIndexes = 0;
            Globals.tess.indexBufferSize = 0;
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

        //
        // Cmd_SetRefDef
        //
        private void Cmd_SetRefDef(idRenderCommand cmd)
        {
            Shade.PushDrawMatrix(cmd.refdef);
            state.refdef = cmd.refdef;
        }

        //
        // Cmd_SetEntityMatrix
        //
        private void Cmd_SetEntityMatrix(idRenderCommand cmd)
        {
            Shade.CreateTranslateRotateMatrix(cmd.entity);
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
            Globals.graphics3DDevice.Clear(ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
        }

        //
        // Cmd_DrawSurfs
        //
        private void Cmd_DrawSurfs(int smpFrame, idRenderCommand cmd)
        {
            RB_BeginDrawingView();

            // Draw all the surfaces.
            for (int i = cmd.firstDrawSurf; i < cmd.firstDrawSurf + cmd.numDrawSurfs; i++)
            {
                idDrawSurface surf = backEndData[smpFrame].drawSurfs[i];
                BeginSurface(surf.materials[0], 0);

                Globals.tess.vertexBufferStart = surf.startVertex;;
                Globals.tess.indexBufferStart = surf.startIndex;
                Globals.tess.indexBufferSize = surf.numIndexes;
                Globals.tess.vertexBufferSize = surf.numVertexes;

                EndSurface();
            }
        }

        //
        // Cmd_SetVertexIndexBuffer
        //
        private void Cmd_SetVertexIndexBuffer(idRenderCommand cmd)
        {
            Shade.BindVertexIndexBuffer(cmd.vertexBuffer, cmd.indexBuffer);
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
                if (SmpThreadHasWork( smpFrame ) || Globals.r_smp.GetValueInteger() == 0)
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
                            case renderCommandType.RC_DRAW_SURFS:
                                Cmd_DrawSurfs(smpFrame, cmd);
                                break;
                            case renderCommandType.RC_SET_VERTEXINDEXBUFFER:
                                Cmd_SetVertexIndexBuffer(cmd);
                                break;
                            case renderCommandType.RC_SET_REFDEF:
                                Cmd_SetRefDef(cmd);
                                break;
                            case renderCommandType.RC_SET_ENTITYMATRIX:
                                Cmd_SetEntityMatrix(cmd);
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
        // ResetFrame
        //
        private void ResetFrame()
        {
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

    // all of the information needed by the back end must be
    // contained in a backEndData_t.  This entire structure is
    // duplicated so the front and back end can run in parallel
    // on an SMP machine
    class backEndData_t {
	    public idDrawSurface[] drawSurfs;
        public int numDrawSurfs;

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



        public backEndData_t(int smpFrameNum )
        {
       //     for (int i = 0; i < idRenderGlobals.MAX_RENDER_COMMANDS; i++)
        //    {
       //         commands[i] = new idRenderCommand();
       //     }
            drawSurfs = new idDrawSurface[idRenderGlobals.MAX_DRAWSURFS];
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
        // PushDrawSurface
        //
        public void PushDrawSurface(idDrawSurface surf)
        {
            drawSurfs[numDrawSurfs++] = surf;
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
        // GetNextEntity
        //
        public idRefdefLocal GetNextEntity()
        {
            if (entities[numEntities] == null)
            {
                entities[numEntities] = new idRefdefLocal();
            }

            return entities[numEntities++];
        }

        //
        // Pause
        //
        public void Pause()
        {
            executingFrameBuffer = false;
            for (int i = 0; i < numEntities; i++)
            {
                entities[i].num_entities = 0;
            }
            numEntities = 0;
            numDrawSurfs = 0;
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
