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

// File_Memory.cs (c) 2010 JV Software
//

using System;
using System.IO;
using System.Text;
using idLib.Engine.Public;

namespace rtcw.Framework.Files
{
    //
    // idFile_Memory
    //
    public class idFile_Memory : idFile
    {
        private BinaryReader reader;
        private BinaryWriter writer;
        private string _filename;
        private byte[] buffer;

        //
        // idFile_Memory - read constructor.
        //
        public idFile_Memory(string filename, Stream fstream)
        {
            if (fstream.CanWrite)
            {
                writer = new BinaryWriter(fstream);
            }
            else
            {
                buffer = new byte[fstream.Length];

                // Read the entire buffer into memory.
                fstream.Read(buffer, 0, (int)fstream.Length);           
                fstream.Dispose();
            }
            _filename = filename;
        }

        public idFile_Memory(BinaryReader reader)
        {
            this.reader = reader;
            _filename = "_internal";
        }

        //
        // InitStream
        //
        public void InitStream()
        {
            if (reader != null)
            {
                return;
            }

            reader = new BinaryReader(new MemoryStream(buffer));
            buffer = null;
        }

        //
        // DecompressFile
        //
        public void DecompressCompiledFile()
        {
#if false
            // Skip the XNB header.
            Seek(idFileSeekOrigin.FS_SEEK_SET, 25);

            byte[] buffer = ReadBytes(Length() - 25);
            reader.Dispose();
            reader = null;

            reader = new BinaryReader(new MemoryStream(SevenZip.Compression.LZMA.SevenZipHelper.Decompress(buffer)));
            reader.BaseStream.Position = 0;
#else
            reader = new BinaryReader(new MemoryStream(SevenZip.Compression.LZMA.SevenZipHelper.Decompress(buffer)));
            buffer = null;
#endif
        }

        //
        // GetFullFilePath
        //
        public override string GetFullFilePath()
        {
            return _filename;
        }

        //
        // GetFileNameAndPathWithoutExtension
        //
        public override string GetFileNameAndPathWithoutExtension()
        {
            string dir;

            dir = Path.GetDirectoryName(_filename);

            if (dir.Length <= 0)
            {
                return Path.GetFileNameWithoutExtension(_filename);
            }
            return Path.GetDirectoryName(_filename) + "/" + Path.GetFileNameWithoutExtension(_filename);
        }

        //
        // GetClassType
        //
        public override Type GetClassType()
        {
            return typeof(idFile_Memory);
        }

        //
        // Equals
        //
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(idFile_Memory))
            {
                return false;
            }

            if (((idFile_Memory)obj)._filename != _filename)
            {
                return false;
            }

            return true;
        }

        //
        // GetHashCode
        //
        public override int GetHashCode()
        {
            return _filename.Length;
        }

        //
        // Flush
        //
        public override void Flush()
        {
            if (writer == null)
            {
                Engine.common.ErrorFatal( "idFile::Flush: Called on non writeable stream" );
                return;
            }

            writer.BaseStream.Flush();
        }

        //
        // Length
        //
        public override int Length()
        {
            if (reader != null)
            {
                return (int)reader.BaseStream.Length;
            }
            return (int)writer.BaseStream.Length;
        }

        public Stream BaseStream
        {
            get
            {
                return reader.BaseStream;
            }
        }

        //
        // Tell
        //
        public override int Tell()
        {
            if (reader != null)
            {
                return (int)reader.BaseStream.Position;
            }
            return (int)writer.BaseStream.Position;
        }

        //
        // Seek
        //
        public override int Seek(idFileSeekOrigin origin, int offset)
        {
            SeekOrigin seekVal = SeekOrigin.Begin;
            switch (origin)
            {
                case idFileSeekOrigin.FS_SEEK_CUR:
                    seekVal = SeekOrigin.Current;
                    break;

                case idFileSeekOrigin.FS_SEEK_END:
                    seekVal = SeekOrigin.End;
                    break;

                case idFileSeekOrigin.FS_SEEK_SET:
                    seekVal = SeekOrigin.Begin;
                    break;

                default:
                    Engine.common.ErrorFatal("idFile_Memory: Invalid seek option\n");
                    break;
            }

            if (reader != null)
            {
                reader.BaseStream.Seek(offset, seekVal);
            }
            else
            {
                writer.BaseStream.Seek(offset, seekVal);
            }

            return Tell();
        }

        //
        // ReadBytes
        //
        public override byte[] ReadBytes(int len)
        {
            return reader.ReadBytes(len);
        }

        //
        // ReadSignedByte
        //
        public override sbyte ReadSignedByte()
        {
            return reader.ReadSByte();
        }

        //
        // ReadChars
        //
        public override char[] ReadChars(int len)
        {
            return reader.ReadChars(len);
        }

        //
        // ReadDouble
        //
        public override double ReadDouble()
        {
            return reader.ReadDouble();
        }

        //
        // ReadUShort
        //
        public override ushort ReadUShort()
        {
            return reader.ReadUInt16();
        }

        //
        // ReadFloat
        //
        public override float ReadFloat()
        {
            return reader.ReadSingle();
        }

        //
        // ReadByte
        //
        public override byte ReadByte()
        {
            return reader.ReadByte();
        }

        //
        // ReadInt
        //
        public override int ReadInt()
        {
            return reader.ReadInt32();
        }

        //
        // ReadUInt
        //
        public override uint ReadUInt()
        {
            return reader.ReadUInt32();
        }

        //
        // ReadLong
        //
        public override long ReadLong()
        {
            return reader.ReadInt64();
        }

        //
        // ReadShort
        //
        public override short ReadShort()
        {
            return reader.ReadInt16();
        }

        //
        // ReadVector3
        // 
        public override void ReadVector3(ref idLib.Math.idVector3 v)
        {
            v.X = ReadFloat();
            v.Y = ReadFloat();
            v.Z = ReadFloat();
        }

        //
        // ReadVector2
        //
        public override void ReadVector2(ref idLib.Math.idVector2 v)
        {
            v.X = ReadFloat();
            v.Y = ReadFloat();
        }


        //
        // ReadString
        //
        public override string ReadString()
        {
            return reader.ReadString();
        }

        //
        // ReadString
        //
        public override string ReadString(int len)
        {
            byte[] buffer = ReadBytes(len);
#if false
            string s = "";

            for( int i = 0; i < buffer.Length; i++ )
            {
                if ((char)buffer[i] == '\0')
                    break;
                s += (char)buffer[i];
            }
            return s; //
#else

            return Encoding.UTF8.GetString(buffer, 0, len-1).Trim('\0'); // ASCIIEncoding.ASCII.GetString(buffer).Trim('\0');
#endif

        }

        //
        // WriteBytes
        //
        public override void WriteBytes(byte[] buffer)
        {
            writer.Write(buffer);
        }

        //
        // WriteDouble
        // 
        public override void WriteDouble(double val)
        {
            writer.Write(val);
        }

        //
        // WriteFloat
        //
        public override void WriteFloat(float val)
        {
            writer.Write(val);
        }

        //
        // WriteInt
        // 
        public override void WriteInt(int val)
        {
            writer.Write(val);
        }

        //
        // WriteLong
        //
        public override void WriteLong(long val)
        {
            writer.Write(val);
        }

        //
        // WriteShort
        // 
        public override void WriteShort(short val)
        {
            writer.Write(val);
        }

        //
        // WriteString
        //
        public override void WriteString(string str)
        {
            writer.Write(str);
        }

        //
        // Dispose
        //
        public void Dispose()
        {
            if (writer != null)
            {
                // Flush anything that still needs to be written to the HD.
                Flush();

                // Close the file.
                writer.Close();

                // Dipose of the file
                writer.Dispose();

                writer = null;
            }
            else if (reader != null)
            {
                reader.Dispose();
                reader = null;
            }
            else
            {
                Engine.common.ErrorFatal("idFile::Dispose: Dispose called on non alloced file");
            }
        }
    }
}
