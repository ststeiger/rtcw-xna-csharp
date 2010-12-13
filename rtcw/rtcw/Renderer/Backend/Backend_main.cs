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

        //
        // idRenderBackend
        //
        public idRenderBackend()
        {
            for (int i = 0; i < idRenderGlobals.SMP_FRAMES; i++)
            {
                backEndData[i] = new backEndData_t(i);
            }

            state.smpFrame = 0;
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

            return backEndData[state.smpFrame].commands[backEndData[state.smpFrame].numRenderCommands++];
        }

        //
        // Cmd_DrawStrechImage
        //
        private void Cmd_DrawStrechImage(ref idRenderCommand cmd)
        {
            Globals.graphics2DDevice.Begin();
            Globals.graphics2DDevice.Draw((Texture2D)cmd.image.GetDeviceHandle(), new Rectangle((int)cmd.x, (int)cmd.y, (int)cmd.w, (int)cmd.h), Color.White);
            Globals.graphics2DDevice.End();
        }

        //
        // Cmd_SwapBuffers
        //
        private void Cmd_SwapBuffers(ref idRenderCommand cmd)
        {
            Globals.graphics3DDevice.Present();
        }

        //
        // GetBackendData
        //
        private backEndData_t GetBackendData(int smpFrame)
        {
            if (backEndData[smpFrame].executingFrameBuffer == false)
            {
                return null;
            }
            return backEndData[smpFrame];
        }

        //
        // IssueRenderCommandsWorker
        //
        public void IssueRenderCommandsWorker(int smpFrame)
        {
            backEndData_t backEnd;

            while (true)
            {
                backEnd = Globals.backEnd.GetBackendData(smpFrame);
                if (backEnd != null)
                {
                    for (int i = 0; i < backEnd.numRenderCommands; i++)
                    {
                        idRenderCommand cmd = backEnd.commands[i];

                        switch (cmd.type)
                        {
                            case renderCommandType.RC_STRETCH_IMAGE:
                                Cmd_DrawStrechImage(ref cmd);
                                break;
                            case renderCommandType.RC_SWAP_BUFFERS:
                                Cmd_SwapBuffers(ref cmd);
                                break;
                            default:
                                Engine.common.ErrorFatal("R_IssueRenderCommandsWorker: Unknown command type\n");
                                break;
                        }
                    }

                    // Were down wait for the next command list.
                    backEndData[smpFrame].executingFrameBuffer = false;

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

            // Wait for the other smp thread to finish.
            while (backEndData[smpWaitThread].SmpRunning)
            {
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
	    public idDrawSurface[] drawSurfs = new idDrawSurface[idRenderGlobals.MAX_DRAWSURFS];
        public dlight_t[] dlights = new dlight_t[idRenderSystem.MAX_DLIGHTS];
        public corona_t[] coronas = new corona_t[idRenderSystem.MAX_CORONAS];          //----(SA)
        public idRefdefLocal[] entities = new idRefdefLocal[idRenderSystem.MAX_ENTITIES];
	    public srfPoly_t[] polys = new srfPoly_t[idRenderGlobals.MAX_POLYS];
	    public polyVert_t[] polyVerts = new polyVert_t[idRenderGlobals.MAX_POLYVERTS];
        public idRenderCommand[] commands = new idRenderCommand[idRenderGlobals.MAX_RENDER_COMMANDS];
        public int numRenderCommands = 0;
        public bool executingFrameBuffer = false;
        private idThread smpthread;
        private int smpFrameNum;

        public backEndData_t(int smpFrameNum )
        {
            for (int i = 0; i < idRenderGlobals.MAX_RENDER_COMMANDS; i++)
            {
                commands[i] = new idRenderCommand();
            }

            this.smpFrameNum = smpFrameNum;
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
