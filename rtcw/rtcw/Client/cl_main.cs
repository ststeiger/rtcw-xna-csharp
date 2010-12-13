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

// cl_main.cs (c) 2010 JV Software
//

using System;
using idLib.Engine.Public;
using idLib.Game.Client;

namespace rtcw.Client
{
    static class CVars
    {
        public static idCVar cl_nodelta;
        public static idCVar cl_debugMove;
        public static idCVar com_cl_running;

        public static idCVar cl_noprint;
        public static idCVar cl_motd;

        public static idCVar rcon_client_password;
        public static idCVar rconAddress;

        public static idCVar cl_timeout;
        public static idCVar cl_maxpackets;
        public static idCVar cl_packetdup;
        public static idCVar cl_timeNudge;
        public static idCVar cl_showTimeDelta;
        public static idCVar cl_freezeDemo;

        public static idCVar cl_shownet;     // NERVE - SMF - This is referenced in msg.c and we need to make sure it is NULL
        public static idCVar cl_showSend;
        public static idCVar cl_timedemo;
        public static idCVar cl_avidemo;
        public static idCVar cl_forceavidemo;

        public static idCVar cl_freelook;
        public static idCVar cl_sensitivity;

        public static idCVar cl_mouseAccel;
        public static idCVar cl_showMouseRate;

        public static idCVar m_pitch;
        public static idCVar m_yaw;
        public static idCVar m_forward;
        public static idCVar m_side;
        public static idCVar m_filter;

        public static idCVar cl_activeAction;

        public static idCVar cl_motdString;

        public static idCVar cl_allowDownload;
        public static idCVar cl_conXOffset;
        public static idCVar cl_inGameVideo;

        public static idCVar cl_serverStatusResendTime;
        public static idCVar cl_trn;
        public static idCVar cl_missionStats;
        public static idCVar cl_waitForFire;

        public static idCVar cl_recoilPitch;
        public static idCVar cl_run;

        public static idCVar cl_yawspeed;
        public static idCVar cl_pitchspeed;
        public static idCVar cl_anglespeedkey;

        // NERVE - SMF - localization
        public static idCVar cl_language;
        public static idCVar cl_debugTranslation;
        // -NERVE - SMF
    }
    //
    // clientStatic_t
    //
    public class clientStatic_t
    {
        public connstate_t state;
        public int realtime;

        public int keyCatchers;                // bit flags

        public bool rendererStarted = false;
        public bool soundStarted = false;
        public bool soundRegistered = false;
        public bool uiStarted = false;
        public bool cgameStarted = false;

        public idMaterial charSetShader;
        public idMaterial whiteShader;
        public idMaterial consoleShader;
        public idMaterial consoleShader2;

        public idVideo videoFullScreen;
        public idImage whiteImage;
        public bool videoLetterBox = false;

        public idSysModule uivm;
        public idSysModule cgvm;

        public idClientGamePublic cgame;
    };

    //
    // idClientManager
    //
    public class idClientManager
    {
        private clientStatic_t cls = new clientStatic_t();

        /*
        ==============
        CL_PlayCinematic_f
        ==============
        */
        private void CL_PlayCinematic_f()
        {
            string arg, s;
	        bool holdatend;
	        int bits = idVideo.CIN_system;

	        Engine.common.DPrintf( "CL_PlayCinematic_f\n" );
	        if ( cls.state == connstate_t.CA_CINEMATIC ) {
                cls.videoFullScreen.Dispose();
	        }

            arg = Engine.cmdSystem.Cmd_Argv( 1 );
            s = Engine.cmdSystem.Cmd_Argv(2);

            cls.videoFullScreen = Engine.RenderSystem.LoadVideo(arg);
            if(cls.videoFullScreen == null )
            {
                Engine.common.Warning( "PlayCinematic: Failed to find cinematic %s\n", arg );
                return;
            }

            cls.state = connstate_t.CA_CINEMATIC;
	        holdatend = false;
	        if ( ( s != null && s[0] == '1' ) || arg == "demoend" || arg == "end" ) {
		        bits |= idVideo.CIN_hold;
	        }
            if (s != null && s[0] == '2')
            {
                bits |= idVideo.CIN_loop;
                cls.videoFullScreen.SetLooping( true );
	        }
            if (s != null && s[0] == '3')
            {
                bits |= idVideo.CIN_letterBox;
                cls.videoLetterBox = true;
            }
            else
            {
                cls.videoLetterBox = false;
            }

	        //S_StopAllSounds();
	        // make sure volume is up for cine
	        //S_FadeAllSounds( 1, 0 );

            


            if ((bits & idVideo.CIN_letterBox) != 0)
            {
                cls.videoFullScreen.SetExtents(0, idVideo.LETTERBOX_OFFSET, Engine.RenderSystem.GetViewportWidth(), Engine.RenderSystem.GetViewportHeight() - (idVideo.LETTERBOX_OFFSET * 2));
//		        CL_handle = CIN_PlayCinematic( arg, 0, LETTERBOX_OFFSET, SCREEN_WIDTH, SCREEN_HEIGHT - ( LETTERBOX_OFFSET * 2 ), bits );
	        } else {
	//	        CL_handle = CIN_PlayCinematic( arg, 0, 0, SCREEN_WIDTH, SCREEN_HEIGHT, bits );
	        }
        }

        //
        // InitRenderer
        //
        private void InitRenderer()
        {
            Engine.RenderSystem.Init();

            // load character sets
            cls.charSetShader = Engine.materialManager.FindMaterial("gfx/2d/bigchars", -1);
            cls.whiteShader = Engine.materialManager.FindMaterial("white", -1);
            cls.consoleShader = Engine.materialManager.FindMaterial("console", -1);
            cls.consoleShader2 = Engine.materialManager.FindMaterial("console2", -1);
        }

        //
        // InitUI
        //
        private void InitUI()
        {
            // Load the vm.
            cls.uivm = Engine.Sys.LoadDLL("ui");

            // Allocate the ui interface manager from the vm.
            Engine.ui = cls.uivm.AllocClass<idUserInterfaceManager>("ui.idUserInterfaceManagerLocal");

            // Init the user interface manager.
            Engine.ui.Init();
        }

        //
        // InitCGame
        //
        private void InitCGame()
        {
            // Load the vm.
            cls.cgvm = Engine.Sys.LoadDLL("cgame");

            // Allocate the cgame interface from the vm.
            cls.cgame = cls.cgvm.AllocClass<idClientGamePublic>("cgame.idClientGame");

            // Init cgame.
            cls.cgame.Init();

            cls.whiteImage = Engine.imageManager.FindImage("*white");
        }

        /*
        ============================
        CL_StartHunkUsers

        After the server has cleared the hunk, these will need to be restarted
        This is the only place that any of these functions are called from
        ============================
        */
        public void StartHunkUsers() {
	        if ( CVars.com_cl_running == null ) {
		        return;
	        }

            if (CVars.com_cl_running.GetValueInteger() == 0)
            {
		        return;
	        }

	        if ( !cls.rendererStarted ) {
		        cls.rendererStarted = true;
                InitRenderer();
	        }

	        if ( !cls.soundStarted ) {
                cls.soundStarted = true;
		        //S_Init();
	        }

	        if ( !cls.soundRegistered ) {
		        cls.soundRegistered = true;
		        //S_BeginRegistration();
	        }

	        if ( !cls.uiStarted ) {
		        cls.uiStarted = true;
                InitUI();
	        }

            if (!cls.cgameStarted)
            {
                cls.cgameStarted = true;
                InitCGame();
            }
        }

        //
        // Init
        //
        public void Init()
        {
            Engine.common.Printf("----- Client Initialization -----\n");

            cls.state = connstate_t.CA_DISCONNECTED;    // no longer CA_UNINITIALIZED

            cls.realtime = 0;

            //
            // register our variables
            //
            CVars.cl_noprint = Engine.cvarManager.Cvar_Get("cl_noprint", "0", 0);
            CVars.cl_motd = Engine.cvarManager.Cvar_Get("cl_motd", "1", 0);

            CVars.cl_timeout = Engine.cvarManager.Cvar_Get("cl_timeout", "200", 0);

            CVars.cl_timeNudge = Engine.cvarManager.Cvar_Get("cl_timeNudge", "0", idCVar.CVAR_TEMP);
            CVars.cl_shownet = Engine.cvarManager.Cvar_Get("cl_shownet", "0", idCVar.CVAR_TEMP);
            CVars.cl_showSend = Engine.cvarManager.Cvar_Get("cl_showSend", "0", idCVar.CVAR_TEMP);
            CVars.cl_showTimeDelta = Engine.cvarManager.Cvar_Get("cl_showTimeDelta", "0", idCVar.CVAR_TEMP);
            CVars.cl_freezeDemo = Engine.cvarManager.Cvar_Get("cl_freezeDemo", "0", idCVar.CVAR_TEMP);
            CVars.rcon_client_password = Engine.cvarManager.Cvar_Get("rconPassword", "", idCVar.CVAR_TEMP);
            CVars.cl_activeAction = Engine.cvarManager.Cvar_Get("activeAction", "", idCVar.CVAR_TEMP);

            CVars.cl_timedemo = Engine.cvarManager.Cvar_Get("timedemo", "0", 0);
            CVars.cl_avidemo = Engine.cvarManager.Cvar_Get("cl_avidemo", "0", 0);
            CVars.cl_forceavidemo = Engine.cvarManager.Cvar_Get("cl_forceavidemo", "0", 0);

            CVars.rconAddress = Engine.cvarManager.Cvar_Get("rconAddress", "", 0);

            CVars.cl_yawspeed = Engine.cvarManager.Cvar_Get("cl_yawspeed", "140", idCVar.CVAR_ARCHIVE);
            CVars.cl_pitchspeed = Engine.cvarManager.Cvar_Get("cl_pitchspeed", "140", idCVar.CVAR_ARCHIVE);
            CVars.cl_anglespeedkey = Engine.cvarManager.Cvar_Get("cl_anglespeedkey", "1.5", 0);

            CVars.cl_maxpackets = Engine.cvarManager.Cvar_Get("cl_maxpackets", "30", idCVar.CVAR_ARCHIVE);
            CVars.cl_packetdup = Engine.cvarManager.Cvar_Get("cl_packetdup", "1", idCVar.CVAR_ARCHIVE);

            CVars.cl_run = Engine.cvarManager.Cvar_Get("cl_run", "1", idCVar.CVAR_ARCHIVE);
            CVars.cl_sensitivity = Engine.cvarManager.Cvar_Get("sensitivity", "5", idCVar.CVAR_ARCHIVE);
            CVars.cl_mouseAccel = Engine.cvarManager.Cvar_Get("cl_mouseAccel", "0", idCVar.CVAR_ARCHIVE);
            CVars.cl_freelook = Engine.cvarManager.Cvar_Get("cl_freelook", "1", idCVar.CVAR_ARCHIVE);

            CVars.cl_showMouseRate = Engine.cvarManager.Cvar_Get("cl_showmouserate", "0", 0);

            CVars.cl_allowDownload = Engine.cvarManager.Cvar_Get("cl_allowDownload", "0", idCVar.CVAR_ARCHIVE);

            // init autoswitch so the ui will have it correctly even
            // if the cgame hasn't been started
            Engine.cvarManager.Cvar_Get("cg_autoswitch", "2", idCVar.CVAR_ARCHIVE);

            // Rafael - particle switch
            Engine.cvarManager.Cvar_Get("cg_wolfparticles", "1", idCVar.CVAR_ARCHIVE);
            // done

            CVars.cl_conXOffset = Engine.cvarManager.Cvar_Get("cl_conXOffset", "0", 0);
            CVars.cl_inGameVideo = Engine.cvarManager.Cvar_Get("r_inGameVideo", "1", idCVar.CVAR_ARCHIVE);

            CVars.cl_serverStatusResendTime = Engine.cvarManager.Cvar_Get("cl_serverStatusResendTime", "750", 0);

            // RF
            CVars.cl_recoilPitch = Engine.cvarManager.Cvar_Get("cg_recoilPitch", "0", idCVar.CVAR_ROM);

            CVars.m_pitch = Engine.cvarManager.Cvar_Get("m_pitch", "0.022", idCVar.CVAR_ARCHIVE);
            CVars.m_yaw = Engine.cvarManager.Cvar_Get("m_yaw", "0.022", idCVar.CVAR_ARCHIVE);
            CVars.m_forward = Engine.cvarManager.Cvar_Get("m_forward", "0.25", idCVar.CVAR_ARCHIVE);
            CVars.m_side = Engine.cvarManager.Cvar_Get("m_side", "0.25", idCVar.CVAR_ARCHIVE);
            CVars.m_filter = Engine.cvarManager.Cvar_Get("m_filter", "0", idCVar.CVAR_ARCHIVE);

            CVars.cl_motdString = Engine.cvarManager.Cvar_Get("cl_motdString", "", idCVar.CVAR_ROM);

            Engine.cvarManager.Cvar_Get("cl_maxPing", "800", idCVar.CVAR_ARCHIVE);

            // userinfo
            Engine.cvarManager.Cvar_Get("name", "Player", idCVar.CVAR_USERINFO | idCVar.CVAR_ARCHIVE);
            Engine.cvarManager.Cvar_Get("rate", "3000", idCVar.CVAR_USERINFO | idCVar.CVAR_ARCHIVE);
            Engine.cvarManager.Cvar_Get("snaps", "20", idCVar.CVAR_USERINFO | idCVar.CVAR_ARCHIVE);
            Engine.cvarManager.Cvar_Get("model", "bj2", idCVar.CVAR_USERINFO | idCVar.CVAR_ARCHIVE); // temp until we have an skeletal american model
            Engine.cvarManager.Cvar_Get("head", "default", idCVar.CVAR_USERINFO | idCVar.CVAR_ARCHIVE);
            Engine.cvarManager.Cvar_Get("color", "4", idCVar.CVAR_USERINFO | idCVar.CVAR_ARCHIVE);
            Engine.cvarManager.Cvar_Get("handicap", "100", idCVar.CVAR_USERINFO | idCVar.CVAR_ARCHIVE);
            Engine.cvarManager.Cvar_Get("sex", "male", idCVar.CVAR_USERINFO | idCVar.CVAR_ARCHIVE);
            Engine.cvarManager.Cvar_Get("cl_anonymous", "0", idCVar.CVAR_USERINFO | idCVar.CVAR_ARCHIVE);

            Engine.cvarManager.Cvar_Get("password", "", idCVar.CVAR_USERINFO);
            Engine.cvarManager.Cvar_Get("cg_predictItems", "1", idCVar.CVAR_USERINFO | idCVar.CVAR_ARCHIVE);

            //----(SA) added
            Engine.cvarManager.Cvar_Get("cg_autoactivate", "1", idCVar.CVAR_USERINFO | idCVar.CVAR_ARCHIVE);
            Engine.cvarManager.Cvar_Get("cg_emptyswitch", "0", idCVar.CVAR_USERINFO | idCVar.CVAR_ARCHIVE);
            //----(SA) end

            // cgame might not be initialized before menu is used
            Engine.cvarManager.Cvar_Get("cg_viewsize", "100", idCVar.CVAR_ARCHIVE);

            CVars.cl_missionStats = Engine.cvarManager.Cvar_Get("g_missionStats", "0", idCVar.CVAR_ROM);
            CVars.cl_waitForFire = Engine.cvarManager.Cvar_Get("cl_waitForFire", "0", idCVar.CVAR_ROM);

            // NERVE - SMF - localization
            CVars.cl_language = Engine.cvarManager.Cvar_Get("cl_language", "0", idCVar.CVAR_ARCHIVE);
            CVars.cl_debugTranslation = Engine.cvarManager.Cvar_Get("cl_debugTranslation", "0", 0);
            // -NERVE - SMF

            Engine.cmdSystem.Cmd_AddCommand("cinematic", CL_PlayCinematic_f);
            Engine.cmdSystem.Cmd_AddCommand("disconnect", CL_Disconnect_f);

            CVars.com_cl_running = Engine.cvarManager.Cvar_Set("cl_running", "1", true);

            Engine.common.Printf("----- Client Initialization Complete -----\n");
        }

        //
        // StopCinematic
        //
        private void StopCinematic()
        {
            if (cls.videoFullScreen != null)
            {
                cls.videoFullScreen.Dispose();
                cls.videoFullScreen = null;
            }
        }

        /*
        ==================
        CL_Disconnect_f
        ==================
        */
        private void CL_Disconnect_f() {
	        StopCinematic();

	        // RF, make sure loading variables are turned off
	        Engine.cvarManager.Cvar_Set( "savegame_loading", "0", true );
            Engine.cvarManager.Cvar_Set("g_reloading", "0", true);

            if (cls.state != connstate_t.CA_DISCONNECTED && cls.state != connstate_t.CA_CINEMATIC)
            {
		        Engine.common.ErrorDrop( "Disconnected from server" );
	        }

            cls.state = connstate_t.CA_DISCONNECTED;
        }

        /*
        ===================
        CL_KeyEvent

        Called by the system for both key up and key down events
        ===================
        */
        public void KeyEvent(int key, bool down, int time)
        {
            // escape is always handled special
            if (key == (int)keyNum.K_ESCAPE && down == true)
            {
                if ((cls.keyCatchers & keyCatch.UI) == 0)
                {
                    if (cls.state == connstate_t.CA_ACTIVE /*&& !clc.demoplaying*/)
                    {
                       // VM_Call(uivm, UI_SET_ACTIVE_MENU, UIMENU_INGAME);
                    }
                    else
                    {
                        CL_Disconnect_f();
                        cls.keyCatchers = keyCatch.UI;
                       // S_StopAllSounds();
                       // VM_Call(uivm, UI_SET_ACTIVE_MENU, UIMENU_MAIN);
                    }
                    return;
                }
            }
        }

        public void Frame()
        {
            Engine.RenderSystem.BeginFrame();

            if (cls.state == connstate_t.CA_CINEMATIC)
            {
                // Check to see if the video is done playing is so dispose of it and switch to the main menu.
                if (cls.videoFullScreen.GetStatus() == e_status.FMV_EOF /* || cls.videoFullScreen.GetStatus() == e_status.FMV_IDLE*/)
                {
                    CL_Disconnect_f();
                }
                else
                {
                    if (cls.videoLetterBox)
                    {
                        Engine.RenderSystem.SetColor(Microsoft.Xna.Framework.Color.Black);
                        Engine.RenderSystem.DrawStrechPic(0, 0, Engine.RenderSystem.GetViewportWidth(), idVideo.LETTERBOX_OFFSET, cls.whiteImage);
                        Engine.RenderSystem.DrawStrechPic(0, Engine.RenderSystem.GetViewportHeight() - idVideo.LETTERBOX_OFFSET, Engine.RenderSystem.GetViewportWidth(), idVideo.LETTERBOX_OFFSET, cls.whiteImage);
                        Engine.RenderSystem.SetColor(Microsoft.Xna.Framework.Color.White);
                    }
                    cls.videoFullScreen.DrawCinematic();
                }
            }
            else if (cls.state == connstate_t.CA_DISCONNECTED)
            {
                cls.cgame.DrawMainMenu();
            }

            Engine.RenderSystem.EndFrame();

            Engine.soundManager.Update();
        }
    }
}
