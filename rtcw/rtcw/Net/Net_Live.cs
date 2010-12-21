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

using System;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;
using idLib.Engine.Public;
using idLib.Engine.Public.Net;

namespace rtcw.Net
{
    //
    // idNetLiveAddress
    //
    class idNetLiveAddress : idNetAdress
    {
        LocalNetworkGamer livegamer;
        idNetAddressType addrType;

        //
        // hasPendingData
        //
        public bool hasPendingData
        {
            get
            {
                return livegamer.IsDataAvailable;
            }
        }

        //
        // GetNextIncommingPacket
        //
        public int GetNextIncommingPacket(ref PacketReader buffer, out NetworkGamer sender)
        {
            return livegamer.ReceiveData(buffer, out sender);
        }

        //
        // idNetLiveAddress
        //
        public idNetLiveAddress(Gamer gamer, idNetAddressType addrType)
        {
            livegamer = (LocalNetworkGamer)gamer;
            this.addrType = addrType;
        }

        //
        // SendReliablePacketToAddress
        //
        public void SendReliablePacketToAddress(idNetLiveAddress addr, ref idMsgWriter msg)
        {
            livegamer.SendData(msg.Buffer, SendDataOptions.Reliable, addr.livegamer);
        }

        //
        // GetAddress
        //
        public override string GetAddress()
        {
            return livegamer.Gamertag;
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
        NetworkSession liveNetworkSession;
        //byte[] packet_buffer = new byte[4996];
        PacketReader packet_buffer = new PacketReader();
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
            NetworkGamer sender;
            int packetLen;

            // Network session hasn't been created yet so there shouldn't be anything pending.
            if (liveNetworkSession == null)
            {
                addr = null;
                return null;
            }

            if (lastPacket != null && lastPacketSource == src)
            {
                idMsgReader packet = lastPacket;
                lastPacket = null;
                addr = lastPacketAddr;
                return packet;
            }

            // Update the live network session.
            liveNetworkSession.Update();

            if (loopBackAddress.hasPendingData == false)
            {
                addr = null;
                return null;
            }

            // We shouldn't get blank packets.
            packetLen = loopBackAddress.GetNextIncommingPacket(ref packet_buffer, out sender);
            if (packetLen <= 0)
            {
                Engine.common.Warning("Net_NextPacket: PacketLen <= 0\n");
                addr = null;
                return null;
            }

            if (lastPacket == null)
            {
                lastPacket = new idMsgReader(packet_buffer.ReadBytes(packet_buffer.Length));

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
            return liveNetworkSession.LocalGamers[0];
        }
    }
}
