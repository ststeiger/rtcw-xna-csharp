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

// Files_public.cs (c) 2010 JV Software
//

using System;
using System.IO;
using idLib.Engine.Content;

namespace idLib.Engine.Public
{
    //
    // idFileSeekOrigin
    //
    public enum idFileSeekOrigin
    {
        FS_SEEK_SET,
        FS_SEEK_CUR,
        FS_SEEK_END
    }

    //
    // idFile
    //
    public abstract class idFile
    {
        public abstract int Length();

        // forces flush on files we're writing to.
        public abstract void Flush();
        
        // Returns the position in the file we are at.
        public abstract int Tell();

        // Sets the file read at a certain position, and returns the position.
        public abstract int Seek(idFileSeekOrigin origin, int offset);

        // Returns the full path of this file.
        public abstract string GetFullFilePath();

        // returns the full filename and path without the file extension.
        public abstract string GetFileNameAndPathWithoutExtension();

        // Little edian supported read/write functions.
        public abstract int         ReadInt();
        public abstract uint        ReadUInt();
        public abstract short       ReadShort();
        public abstract long        ReadLong();
        public abstract float       ReadFloat();
        public abstract double      ReadDouble();
        public abstract byte[]      ReadBytes(int len);
        public abstract string      ReadString();
        public abstract string      ReadString(int len);
        public abstract char[]      ReadChars(int len);

        public abstract void        WriteInt(int val);
        public abstract void        WriteShort(short val);
        public abstract void        WriteLong(long val);
        public abstract void        WriteFloat(float val);
        public abstract void        WriteDouble(double val);
        public abstract void        WriteBytes(byte[] buffer);
        public abstract void        WriteString(string str);

        public abstract Type        GetClassType();
    }

    //
    // idFileSystem
    //
    public abstract class idFileSystem
    {
        public abstract bool isInitialized();

        public abstract void Init();
        public abstract void Shutdown();

        public abstract string RemoveExtensionFromPath(string _filename);

        public abstract void ConditionalRestart( int checksumFeed );
        public abstract void Restart( int checksumFeed );
        // shutdown and restart the filesystem so changes to fs_gamedir can take effect
        public abstract idFileList ListFiles(string directory, string extension);
        // directory should not have either a leading or trailing /
        // if extension is "/", only subdirectories will be returned
        // the returned files will not include any directories or /

        public abstract string GetDLLPath(string dllName);

        public abstract void FreeFileList( ref idFileList list );
        public abstract bool FileExists( string file );

        public abstract bool HasExtension(string path, string extension);

        public abstract void CreateDirectory(string path);

        public abstract int     FS_LoadStack();
// JV BEGIN - removed because the phone doesn't support searching.
        //int     FS_GetFileList(  const char *path, const char *extension, char *listbuf, int bufsize );
        //int     FS_GetModList(  char *listbuf, int bufsize );
// JV end
        public abstract idFile  OpenFileWrite( string qpath );
        // will properly create any needed paths and deal with seperater character issues

        public abstract idFile OpenFileRead( string qpath, bool uniqueFILE );
        // if uniqueFILE is true, then a new FILE will be fopened even if the file
        // is found in an already open pak file.  If uniqueFILE is false, you must call
        // FS_FCloseFile instead of fclose, otherwise the pak FILE would be improperly closed
        // It is generally safe to always set uniqueFILE to true, because the majority of
        // file IO goes through FS_ReadFile, which Does The Right Thing already.

        public abstract bool FileIsInPAK( string filename, ref int pChecksum );
        // returns 1 if a file is in the PAK file, otherwise -1

        public abstract bool Delete( string filename );    // only works inside the 'save' directory (for deleting savegames/images)


        public abstract void CloseFile( ref idFile file );
        // note: you can't just fclose from another DLL, due to MS libc issues

        public abstract int ReadFile(string qpath, out byte[] buffer);
        // returns the length of the file
        // a null buffer will just return the file length without loading
        // as a quick check for existance. -1 length == not present
        // A 0 byte will always be appended at the end, so string ops are safe.
        // the buffer should be considered read-only, because it may be cached
        // for other uses.

        public abstract void FreeFile( ref byte[] buffer );
        // frees the memory returned by FS_ReadFile

        public abstract int WriteFile( string qpath, byte[] buffer, int size );
        // writes a complete file, creating any subdirectories needed

        //void QDECL FS_Printf( fileHandle_t f, const char *fmt, ... );
        // like fprintf

        public abstract void RenameStorageFile( string from, string to );
// JV - xna specfic content loader.
        public abstract T ReadContent<T>(string qpath);
// jv end

        //void    FS_CopyFileOS(  char *from, char *to ); //DAJ
    }
}
