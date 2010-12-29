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

// Net_Main.cs (c) 2010 JV Software
//

//#define USE_XBOXLIVE // comment in for non-phone builds

using System;
#if USE_XBOXLIVE
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;
#endif
using idLib.Engine.Public;

namespace rtcw.Net
{
    //
    // idNetworkLocal
    //
    public class idNetworkLocal : idNetwork
    {
        idCVar showpackets;
        idCVar showdrop;
        idCVar liveProfileName;
        idCVar net_trialmode;

        idNetProtocolLive live;

        //
        // Init
        //
        public override void Init()
        {
#if USE_XBOXLIVE
            // Show the sign in screen, but don't force online only profiles.
            if (LiveGuideVisible() == false && (Gamer.SignedInGamers[0] == null/* || Gamer.SignedInGamers[0].Gamertag.Contains("Player") == true*/))
            {
                Guide.ShowSignIn(1, false);
                Engine.common.Printf("Net_Init: Live Guide is Active...\n");
                return;
            }

            // Wait to init till the guide is hidden.
            if (Gamer.SignedInGamers[0] == null || LiveGuideVisible() == true)
            {
                return;
            }
#endif
            // Wait for any of the live stuff to clear so it won't currupt our vertex/index buffers.
            // This is a ugly hack...
            System.Threading.Thread.Sleep(10); 

            // Setup the various network cvars.
            showpackets = Engine.cvarManager.Cvar_Get("showpackets", "0", idCVar.CVAR_TEMP);
            showdrop = Engine.cvarManager.Cvar_Get("showdrop", "0", idCVar.CVAR_TEMP);
#if USE_XBOXLIVE
            liveProfileName = Engine.cvarManager.Cvar_Get("net_liveprofile", Gamer.SignedInGamers[0].Gamertag, idCVar.CVAR_ROM);

            if (Guide.IsTrialMode == true)
            {
                net_trialmode = Engine.cvarManager.Cvar_Get("net_trialmode", "1", idCVar.CVAR_ROM);
                Engine.common.Printf("\n\n********* Trial Mode Enabled **********\n\n");
            }
            else
            {
                net_trialmode = Engine.cvarManager.Cvar_Get("net_trialmode", "0", idCVar.CVAR_ROM);
                Engine.common.Printf("Net_Init: Registered Version Detected...\n");
            }

            live = new idNetProtocolLive(liveProfileName.GetValue());
#else
            live = new idNetProtocolLive("idPlayer");
            liveProfileName = Engine.cvarManager.Cvar_Get("net_liveprofile", "idPlayer", idCVar.CVAR_ROM);
#endif
            Engine.common.Printf("Net_Init: Signed in with profile " + liveProfileName.GetValue() + "\n");
        }

        //
        // LiveGuideVisible
        //
        public override bool LiveGuideVisible()
        {
#if USE_XBOXLIVE
            return Guide.IsVisible;
#else
            return false;
#endif
        }

        //
        // LiveTrialMode
        //
        public override bool LiveTrialMode()
        {
            return (net_trialmode.GetValueInteger() != 0);
        }

        //
        // SendReliablePacketToAddress
        //
        public override void SendReliablePacketToAddress(idNetSource dst, idNetAdress addr, ref idLib.Engine.Public.Net.idMsgWriter msg)
        {
            live.SendReliablePacketToAddress(dst, addr, ref msg);
        }

        //
        // GetLoopBackAddress
        //
        public override idNetAdress GetLoopBackAddress()
        {
            return live.GetLoopBackAddress();
        }

        //
        // ConnectToSession
        //
        public override void ConnectToSession(idNetAdress addr)
        {
            live.ConnectToSession(addr);
        }

        //
        // CreateServer
        //
        public override void CreateServer(int maxclients)
        {
            live.CreateServer(maxclients);
        }

        //
        // GetLoopPacket
        //
        public override bool GetLoopPacket(idNetSource src, out idNetAdress addr, out idLib.Engine.Public.Net.idMsgReader msg)
        {
            msg = live.GetNextPendingPacket(src, out addr);

            if (msg == null)
                return false;

            return true;
        }
    }
}
