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

// Net_msg.cs (c) 2010 JV Software
//

using System.IO;
using idLib.Math;

namespace idLib.Engine.Public.Net
{
    //
    // idMsgWriter
    //
    public class idMsgWriter
    {
        byte[] buffer;
        BinaryWriter writer;

        //
        // msgLen
        //
        public idMsgWriter(int msgLen)
        {
            buffer = new byte[msgLen];
            writer = new BinaryWriter(new MemoryStream(buffer));
            writer.Write((byte)0); // For the destinitation byte.
        }

        public void WriteDst(idNetSource src)
        {
            writer.BaseStream.Position = 0;
            writer.Write((byte)src);
        }

        public void SetPosition(int pos)
        {
            writer.BaseStream.Position = pos;
        }

        public void WriteInt(int val)
        {
            writer.Write(val);
        }

        public void WriteUInt(uint val)
        {
            writer.Write(val);
        }

        public void WriteShort(short val)
        {
            writer.Write(val);
        }

        public void WriteLong(long val)
        {
            writer.Write(val);
        }

        public void WriteFloat(float val)
        {
            writer.Write(val);
        }

        public void WriteDouble(double val)
        {
            writer.Write(val);
        }

        public void WriteByte(byte val)
        {
            writer.Write(val);
        }

        public void WriteBytes(byte[] val)
        {
            writer.Write(val);
        }

        public void WriteString(string val)
        {
            writer.Write(val);
        }

        public void WriteChars(char[] val)
        {
            writer.Write(val);
        }
        public void WriteVector2(ref idVector2 v)
        {
            WriteFloat(v.X);
            WriteFloat(v.Y);
        }
        public void WriteVector3(ref idVector3 v)
        {
            WriteFloat(v.X);
            WriteFloat(v.Y);
            WriteFloat(v.Z);
        }

        //
        // Dispose
        //
        public void Dispose()
        {
            writer.Close();
            buffer = null;
        }

        //
        // Buffer
        //
        public byte[] Buffer
        {
            get
            {
                return buffer;
            }
        }
    }

    //
    // idMsgReader
    //
    public class idMsgReader
    {
        BinaryReader reader;

        //
        // idMsgReader
        //
        public idMsgReader(byte[] buffer)
        {
            reader = new BinaryReader(new MemoryStream( buffer ));
        }

        public int ReadInt()
        {
            return reader.ReadInt32();
        }

        public uint ReadUInt()
        {
            return reader.ReadUInt32();
        }

        public short ReadShort()
        {
            return reader.ReadInt16();
        }

        public long ReadLong()
        {
            return reader.ReadInt64();
        }

        public float ReadFloat()
        {
            return reader.ReadSingle();
        }

        public double ReadDouble()
        {
            return reader.ReadDouble();
        }

        public byte ReadByte()
        {
            return reader.ReadByte();
        }

        public byte[] ReadBytes(int len)
        {
            return reader.ReadBytes(len);
        }

        public string ReadString()
        {
            return reader.ReadString();
        }

        public char[] ReadChars(int len)
        {
            return reader.ReadChars(len);
        }
        public void ReadVector2(ref idVector2 v)
        {
            v.X = ReadFloat();
            v.Y = ReadFloat();
        }
        public void ReadVector3(ref idVector3 v)
        {
            v.X = ReadFloat();
            v.Y = ReadFloat();
            v.Z = ReadFloat();
        }

        //
        // Dispose
        //
        public void Dispose()
        {
            reader.Dispose();
        }
    }
}
