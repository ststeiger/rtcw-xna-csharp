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

// Net_Live.cs (c) 2010 JV Software
//

//#define USE_XBOXLIVE // comment in for non-phone builds

using System;
#if USE_XBOXLIVE
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;
#endif
using idLib.Engine.Public;
using idLib.Engine.Public.Net;

#if !USE_XBOXLIVE
using System.IO;

using PacketReader = idLib.Engine.Public.Net.idMsgReader;
using Gamer = idLib.Engine.Public.Net.idMsgWriter;
#endif

namespace rtcw.Net
{
    //
    // idNetLiveAddress
    //
    class idNetLiveAddress : idNetAdress
    {
#if USE_XBOXLIVE
        LocalNetworkGamer livegamer;
#else
        PacketReader[] netpackets = new PacketReader[256];
        int numPacketsInQueue = 0;
#endif
        idNetAddressType addrType;

        //
        // hasPendingData
        //
        public bool hasPendingData
        {
#if USE_XBOXLIVE
            get
            {
                return livegamer.IsDataAvailable;
            }
#else
            get
            {
                return (numPacketsInQueue != 0);
            }
#endif
        }

#if !USE_XBOXLIVE
        //
        // ShiftNetworkPacketQueue
        //
        public void ShiftNetworkPacketQueue()
        {
            netpackets[0].Dispose();
            for (int i = 1; i < 256; i++)
            {
                if (netpackets[i] == null)
                    break;

                netpackets[i - 1] = netpackets[i];
                netpackets[i] = null;
            }
        }

#endif

        //
        // GetNextIncommingPacket
        //
        public int GetNextIncommingPacket(ref PacketReader buffer)
        {
#if USE_XBOXLIVE
            return livegamer.ReceiveData(buffer, out sender);
#else
            if (numPacketsInQueue <= 0)
            {
                return -1;
            }

            buffer = netpackets[0];
            numPacketsInQueue--;

            return buffer.Length;
#endif
        }

        //
        // idNetLiveAddress
        //
        public idNetLiveAddress(Gamer gamer, idNetAddressType addrType)
        {
#if USE_XBOXLIVE
            livegamer = (LocalNetworkGamer)gamer;
#endif
            this.addrType = addrType;
        }

        //
        // SendReliablePacketToAddress
        //
        public void SendReliablePacketToAddress(idNetLiveAddress addr, ref idMsgWriter msg)
        {
#if USE_XBOXLIVE
            livegamer.SendData(msg.Buffer, SendDataOptions.Reliable, addr.livegamer);
#else
            netpackets[numPacketsInQueue++] = new PacketReader(msg.Buffer);
#endif
        }

        //
        // GetAddress
        //
        public override string GetAddress()
        {
#if USE_XBOXLIVE
            return livegamer.Gamertag;
#else
            return "idPlayer";
#endif
        }

        //
        // GetType
        //
        public override idNetAddressType GetType()
        {
            return addrType;
        }
    }

    //
    // idNetProtocolLive
    //
    class idNetProtocolLive 
    {
        idNetLiveAddress loopBackAddress;
#if USE_XBOXLIVE
        NetworkSession liveNetworkSession;
#endif
        //byte[] packet_buffer = new byte[4996];
        PacketReader packet_buffer;

        string localProfileName;

        //
        // idNetProtocolLive
        //
        public idNetProtocolLive(string localProfileName)
        {
            this.localProfileName = localProfileName;
        }

        //
        // CreateServer
        //
        public void CreateServer(int maxClients)
        {
#if USE_XBOXLIVE
            Gamer localGamer;

            // Create the network session - default to loopback adapter for now.
            liveNetworkSession = NetworkSession.Create(NetworkSessionType.Local, 2, 1);
            Engine.common.Printf("Net_CreateServer: Loopback Server Created for " + maxClients + " " + maxClients + "\n");

            // Find the local gamer so we can create the loopback address.
            localGamer = FindGamer(localProfileName);
            if (localGamer == null)
            {
                Engine.common.ErrorFatal("idNetLive_Init: Failed to create local gamer...\n");
            }

            loopBackAddress = new idNetLiveAddress(localGamer, idNetAddressType.NA_LOOPBACK);
#else
            loopBackAddress = new idNetLiveAddress(null, idNetAddressType.NA_LOOPBACK);
#endif
        }

        //
        // SendReliablePacketToAddress
        //
        public void SendReliablePacketToAddress(idNetSource dst, idNetAdress addr, ref idMsgWriter msg)
        {
            msg.WriteDst(dst);
            loopBackAddress.SendReliablePacketToAddress((idNetLiveAddress)addr, ref msg);
        }

        //
        // GetLoopBackAddress
        //
        public idNetAdress GetLoopBackAddress()
        {
            return loopBackAddress;
        }

        //
        // ConnectToSession
        //
        public void ConnectToSession(idNetAdress addr)
        {
            idNetLiveAddress liveAddr = (idNetLiveAddress)addr;

            if (liveAddr.GetType() == idNetAddressType.NA_LOOPBACK)
            {
                Engine.common.Printf("idNetLive_ConnectToSession: Connected to loopback adapter\n");
                return;
            }
        }

        //
        // GetNextPendingPacket
        //
        private idMsgReader lastPacket;
        private idNetSource lastPacketSource;
        private idNetAdress lastPacketAddr;
        public idMsgReader GetNextPendingPacket(idNetSource src, out idNetAdress addr)
        {
            int packetLen = 0;

            // Network session hasn't been created yet so there shouldn't be anything pending.
#if USE_XBOXLIVE
            if (liveNetworkSession == null)
            {
                addr = null;
                return null;
            }
#endif
            if (lastPacket != null && lastPacketSource == src)
            {
                idMsgReader packet = lastPacket;
                lastPacket = null;
                addr = lastPacketAddr;
                return packet;
            }
#if USE_XBOXLIVE
            // Update the live network session.
            liveNetworkSession.Update();
#endif
            if (loopBackAddress == null || loopBackAddress.hasPendingData == false)
            {
                addr = null;
                return null;
            }

            // We shouldn't get blank packets.
            packetLen = loopBackAddress.GetNextIncommingPacket(ref packet_buffer);
            if (packetLen <= 0)
            {
                Engine.common.Warning("Net_NextPacket: PacketLen <= 0\n");
                addr = null;
                return null;
            }

            if (lastPacket == null)
            {
                lastPacket = new idMsgReader(packet_buffer.ReadBytes(packet_buffer.Length));

#if !USE_XBOXLIVE
                loopBackAddress.ShiftNetworkPacketQueue();
#endif

                idNetSource packetSrc = (idNetSource)lastPacket.ReadByte();

                if (packetSrc != src)
                {
                    lastPacketSource = packetSrc;
                    lastPacketAddr = loopBackAddress; // hack.

                    addr = null;
                    return null;
                }
            }
            else
            {
                Engine.common.ErrorFatal("idNetLive_NextPacket: Packet with unknown dst...\n");
                addr = null;
                return null;
            }

            idMsgReader packet2 = lastPacket;
            lastPacket = null;
            addr = loopBackAddress;
            return packet2;
        }

        //
        // FindGamer
        //
        private Gamer FindGamer(string gamerName)
        {
#if USE_XBOXLIVE
            return liveNetworkSession.LocalGamers[0];
#else
            return null;
#endif
        }
    }
}
