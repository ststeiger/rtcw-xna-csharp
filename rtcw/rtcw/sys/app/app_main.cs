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

// app_main.cs (c) 2010 JV Software
//

using System;
using Microsoft.Xna.Framework;

//#define USE_XBOXLIVE // comment in for non-phone builds
#if USE_XBOXLIVE
using Microsoft.Xna.Framework.GamerServices;
#endif

using idLib.Engine.Public;
using rtcw.sys;
using rtcw.Renderer;
using rtcw.Framework;
using rtcw.Net;
using rtcw.Sound;
using rtcw.CM;

namespace rtcw.sys.app
{
    //
    // idApp
    //
    public class idApp : Microsoft.Xna.Framework.Game
    {
        public static string cmdline = "";
        GraphicsDeviceManager graphics;
        public static idApp app;

        //
        // idApp
        //
        public idApp()
        {
            // Init any components that need to be attached to the application
            InitAppAttachedObjects();
            app = this;

#if WINDOWS_PHONE
            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);
#endif
        }

        //
        // AllocSystemManagers
        //
        private void AllocSystemManagers()
        {
            Engine.Sys = new idSysLocal();
            Engine.RenderSystem = new idRenderSystemLocal(graphics);
            Engine.common = new idCommonLocal();
            Engine.cvarManager = new idCVarManagerLocal();
            Engine.fileSystem = new idFileSystemLocal(Content);
            Engine.cmdSystem = new idCmdManagerLocal();
            Engine.net = new idNetworkLocal();
            Engine.soundManager = new idSoundManagerLocal();
            Engine.modelManager = new idModelManagerLocal();
            Engine.collisionModelManager = new idCollisionModelManagerLocal();
            Engine.usercmd = new idUsercmdManagerLocal();
        }

        //
        // InitAppAttachedObjects
        // 
        private void InitAppAttachedObjects()
        {
            graphics = new GraphicsDeviceManager(this);
            //graphics.PreferMultiSampling = true;
#if USE_XBOXLIVE
            Components.Add(new GamerServicesComponent(this));
#endif
#if WINDOWS
            //graphics.PreferredBackBufferHeight = 600;
            //graphics.PreferredBackBufferWidth = 800;
            //graphics.ApplyChanges();
#endif
        }

        //
        // EndDraw
        //
        protected override void EndDraw()
        {
            // We do our own device present.
        }

        //
        // Initialize
        //
        protected override void Initialize()
        {
            // Let XNA finish all component initilzation.
            base.Initialize();

            // Alloc all the system mangers, this CAN'T be done in the constructor because objects,
            // such as graphics device won't be valid till Initilize is called.
            AllocSystemManagers();

            // Init the system manager which will init all the needed engine components.
            Engine.common.Init(cmdline);
        }

        //
        // Draw
        //
        protected override void Draw(GameTime gameTime)
        {
#if WINDOWS
            // On windows only process certain things if the app is active.
            Engine.Sys.SetWindowAttributes(IsActive, Window.ClientBounds);
#endif
            // The system class handles the any need required updates before the common frame,
            // this handles all incoming messaging, etc.
            Engine.Sys.Frame(gameTime.IsRunningSlowly, gameTime.ElapsedGameTime.Milliseconds, gameTime.TotalGameTime.Milliseconds);

            // Let the XNA framework draw anything thats still needs to be pushed to screen,
            // and handle any backend OS messaging.
            //base.Draw(gameTime);
        }
    }

    static class Program
    {
        //
        // Main - entry point for the Xbox 360 and Windows builds.
        //
#if WINDOWS || XBOX
        static void Main(string[] args)
        {
            idApp app;
            int numArgs = 0;

            foreach (string s in args)
            {
                idApp.cmdline += args[numArgs++] + " ";
            }

            app = new idApp();

            app.Run();
        }
#endif
    }
}
