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

// cgame_main.cs (c) 2010 JV Software
//

using idLib;
using idLib.Engine.Public;
using idLib.Game.Client;
using idLib.Game;
using idLib.Engine.Public.Net;

namespace cgame
{
    //
    // idClientGame
    //
    public class idClientGame : idClientGamePublic
    {
        idUserInterface mainMenu;
        idUserInterface connectUI;
        idUserInterface briefingUI;
        
#if WINDOWS_PHONE
        idUserInterface hud_phonecontrols;
#endif

#if WINDOWS
        idUserInterface[] kinect_paused = new idUserInterface[2];
        idCVar kinect_playerisavtive;
        idCVar sys_kinect;
#endif

        //
        // idClientGame
        //
        public idClientGame(int version)
        {
            Engine.common.Printf("Client Game module loaded...\n");
        }

        //
        // Init
        //
        public override void Init()
        {
            // Load in the main menu.
            mainMenu = Engine.ui.FindUserInterface("main");
            if (mainMenu == null)
            {
                Engine.common.ErrorFatal("Failed to load the mainmenu\n");
                return;
            }

            // Load the loading menu.
            connectUI = Engine.ui.FindUserInterface("Connect");
            if (connectUI == null)
            {
                Engine.common.ErrorFatal("Failed to load the loading menu.\n");
                return;
            }

            briefingUI = Engine.ui.FindUserInterface("briefing");
            if (briefingUI == null)
            {
                Engine.common.ErrorFatal("Failed to load the briefing menu.\n");
                return;
            }

#if WINDOWS_PHONE
            hud_phonecontrols = Engine.ui.FindUserInterface("phone_controls");
            if (hud_phonecontrols == null)
            {
                Engine.common.ErrorFatal("Failed to load the phone hud controls.\n");
                return;
            }
#endif

#if WINDOWS
            kinect_paused[0] = Engine.ui.FindUserInterface("kinectpaused");
            if (kinect_paused[0] == null)
            {
                Engine.common.ErrorFatal("Failed to load the kinect_paused menu.\n");
                return;
            }

            kinect_paused[1] = Engine.ui.FindUserInterface("kinectpaused2");
            if (kinect_paused[1] == null)
            {
                Engine.common.ErrorFatal("Failed to load the kinect_paused2 menu.\n");
                return;
            }

            sys_kinect = Engine.cvarManager.Cvar_Get("sys_kinect", "0", idCVar.CVAR_ROM);
            kinect_playerisavtive = Engine.cvarManager.Cvar_Get("kinect_playerisavtive", "0", idCVar.CVAR_CHEAT);
#endif
            Globals.white = Engine.imageManager.FindImage("*white");
        }

        //
        // DrawLoadingScreen
        //
        private int baseHunk = 0;
        public override void DrawLoadingScreen(bool forceRefresh)
        {
            float percentDone = 0;
            
            int expectedHunk = Engine.cvarManager.Cvar_Get("com_expectedhunkusage", "0", 0).GetValueInteger();

            if (forceRefresh)
            {
                Engine.RenderSystem.BeginFrame();
            }

            // Draw the briefing UI.
            briefingUI.Draw();

            // Draw the loading bar over the briefing.
            float bar_x = 200;
            float bar_y = 468;
            float bar_w = 240;
            float bar_h = 10;

            if (baseHunk == 0)
            {
                baseHunk = Engine.fileSystem.FS_LoadStack();
            }

            // show the percent complete bar if we are not waiting for the user to click a button to continue.
            if (Globals.waitingToEnterWorld == false)
            {
                if (expectedHunk > 0)
                {
                    percentDone = (float)(Engine.fileSystem.FS_LoadStack() - baseHunk) / (float)(expectedHunk);
                    if (percentDone > 0.97f)
                    {
                        percentDone = 0.97f;
                    }

                    briefingUI.HorizontalPercentBar(bar_x, bar_y, bar_w, bar_h, percentDone);
                }
            }
            else
            {
                if (Globals.waitArrowFrame <= 2)
                {
                    briefingUI.SetItemVisible("but2", true);
                    briefingUI.SetItemVisible("but2_alt", false);
                }
                else if (Globals.waitArrowFrame <= 4)
                {
                    briefingUI.SetItemVisible("but2", false);
                    briefingUI.SetItemVisible("but2_alt", true);
                }
                else
                {
                    Globals.waitArrowFrame = 0.0f;
                }

                Globals.waitArrowFrame += 0.3f;
            }

            if (forceRefresh)
            {
                Engine.RenderSystem.EndFrame();
            }
        }

        //
        // ParseConfigString
        //
        public override void ParseConfigString(string cfgstr)
        {
            idParser parser = new idParser(cfgstr);
            string prevToken = "";

            while (parser.ReachedEndOfBuffer == false)
            {
                string token = parser.NextToken;

                if (token == null || token.Length <= 0)
                    break;

                if (token == "model")
                {
                    string modelName = parser.NextToken;

                    // Check to see if this model is a brush model.
                    if (modelName[0] == '*')
                    {
                        Globals.models[Globals.numModels++] = Globals.world.LoadBrushModel(modelName);
                    }
                    else
                    {
                        Globals.models[Globals.numModels++] = Engine.modelManager.LoadModel(modelName);
                    }
                }
                else if (token == "skin")
                {
                    Globals.skins[Globals.numSkins++] = Engine.RenderSystem.LoadSkin(parser.NextToken);
                }
                else if (token == "sound")
                {
                    Globals.sounds[Globals.numSounds++] = Engine.soundManager.LoadSound(parser.NextToken);
                }
                else if (token == "localClient")
                {
                    Globals.localViewEntity = parser.NextInt;
                }
                else
                {
                    Engine.common.ErrorFatal("CG_ParseConfigString: Unknown or unexpected token in network packet %s-%s \n", token, prevToken);
                }

                prevToken = token;
            }

            // Set the keycatcher so the UI will pick up controller events.
            Engine.common.SetKeyCatcher(keyCatch.CGAME);

            Globals.waitingToEnterWorld = true;
            Globals.waitArrowFrame = 0;
        }

        //
        // ClientReady
        //
        private void ClientReady()
        {
            idMsgWriter msg = new idMsgWriter(idNetwork.netcmd_enterworldmsg.Length + 4);
            msg.WriteString( idNetwork.netcmd_enterworldmsg );
            Engine.net.SendReliablePacketToAddress(idNetSource.NS_SERVER, Engine.net.GetLoopBackAddress(), ref msg);
            msg.Dispose();
        }

        //
        // BeginGame
        //
        public override void BeginGame(string mappath)
        {
            RegisterGraphics(mappath);
        }

        //
        // RegisterGraphics
        //
        private void RegisterGraphics(string mappath)
        {
            Globals.world = Engine.RenderSystem.LoadWorld(mappath);
        }

        //
        // DrawConnectScreen
        //
        public override void DrawConnectScreen()
        {
            connectUI.Draw();
        }

        //
        // Shutdown
        //
        public override void Shutdown()
        {
            
        }

        //
        // DrawMainMenu
        //
        public override void DrawMainMenu()
        {
#if WINDOWS
            if (kinect_playerisavtive.GetValueInteger() < 2 && sys_kinect.GetValueInteger() != 0)
            {
                kinect_paused[kinect_playerisavtive.GetValueInteger()].Draw();
                return;
            }
#endif 

            // Set the keycatcher so the UI will pick up controller events.
            Engine.common.SetKeyCatcher(keyCatch.UI);

            // Draw the mainmenu.
            mainMenu.Draw();
        }

        //
        // HandleMouseEvent
        //
        public override void HandleMouseEvent(float x, float y)
        {
#if WINDOWS_PHONE
            hud_phonecontrols.HandleMouseEvent((int)x, (int)y);
#endif
        }

        //
        // HandleUIMouseEvent
        //
        public override void HandleUIMouseEvent(int x, int y)
        {
            mainMenu.HandleMouseEvent(x, y);
        }

        //
        // HandleKeyEvent
        //
        public override void HandleKeyEvent(int key, bool down)
        {
            if (Globals.waitingToEnterWorld == true)
            {
                ClientReady();

                // Reset the keycatcher so user cmds can get generated.
                Engine.common.SetKeyCatcher(0);

                Globals.waitingToEnterWorld = false;
            }
            else
            {
#if WINDOWS_PHONE
            hud_phonecontrols.HandleKeyEvent((keyNum)key, down);
#endif
            }
        }

        //
        // HandleUIKeyEvent
        //
        public override void HandleUIKeyEvent(int key, bool down)
        {
            mainMenu.HandleKeyEvent((keyNum)key, down);
        }

        //
        // NetworkRecieveSnapshot
        //
        public override void NetworkRecieveSnapshot(ref entityState_t entity)
        {
            // If we need too allocate a new refdef for the localview.
            Globals.localview.GenerateRefdef();

            switch (entity.eType)
            {
                case entityType_t.ET_GENERAL:
                    EntitySnapshot.GeneralEntity(ref entity);
                    break;
                case entityType_t.ET_PLAYER:
                    EntitySnapshot.PlayerEntiy(ref entity);
                    break;
            }
            
        }

        //
        // UpdateUsercmd
        //
        private void UpdateUsercmd()
        {
            idMsgWriter msg;
            idUsercmd usercmd;
            
            // If we haven't got the localviewentity yet send anything to the server.
            if(Globals.localViewEntity < 0)
            {
                return;
            }

            usercmd = Engine.usercmd.GetCurrentCommand();

            msg = new idMsgWriter(idUsercmd.Size + idNetwork.netcmd_usercmd.Length + 4 + 4);
            msg.WriteString(idNetwork.netcmd_usercmd);
            msg.WriteInt(Globals.localViewEntity);

            usercmd.WritePacket(ref msg);

            // Send the command to the server
            Engine.net.SendReliablePacketToAddress(idNetSource.NS_SERVER, Engine.net.GetLoopBackAddress(), ref msg);

            // Dispose of the msg
            msg.Dispose();
        }

        //
        // Frame
        //
        public override void Frame()
        {
            // If we are waiting for the user to click a button to continue, just wait cause the server still thinks
            // were loading.
            if (Globals.waitingToEnterWorld == true || Globals.viewPacketRecv == false)
            {
                DrawLoadingScreen(false);
                return;
            }

            // Get the current user cmd and send it to the server.
#if false
            UpdateUsercmd();
#endif
            // Draw the world through the current view.
            Globals.localview.DrawView();

            if(Globals.inCinematic == false)
            {
    #if WINDOWS_PHONE
                hud_phonecontrols.Draw();
    #endif  
            }
        }
    }
}
