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

// FileSystem.cs (c) 2010 JV Software
//

using System;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework.Content;

using rtcw.Framework.Files;
using idLib.Engine.Content;
using idLib.Engine.Public;

namespace rtcw.Framework
{
    //
    // idFileListReader
    //
    class idFileListReader : ContentTypeReader<idFileList>
    {
        //
        // jvFileList
        //
        protected override idFileList Read(ContentReader input, idFileList existingInstance)
        {
            idFileList list = new idFileList();

            int numFiles = input.ReadInt32();

            for (int i = 0; i < numFiles; i++)
            {
                list.AddFileToList(input.ReadString());
            }

            return list;
        }
    }

    //
    // idFileSystemLocal
    //
    public class idFileSystemLocal : idFileSystem
    {
        private ContentManager _contentManager; // XNA's content manager for content with a valid XNA content project.
        private IsolatedStorageFile _storage;  // Isolated storage container for storage of application updates, savegames, etc.

        private const int MAX_NUM_OPENFILES = 20; // There can only be a maximum of 20 files opened at a time.
        private bool isInit = false;
        
        private int fs_loadStack = 0;

        private idFile[] filepool = new idFile[MAX_NUM_OPENFILES]; // Pool of all open files.

        //
        // idFileSystemLocal
        //
        public idFileSystemLocal(ContentManager contentManager)
        {
            _contentManager = contentManager;
            _contentManager.RootDirectory = Engine.BASEGAME;

#if WINDOWS
            _storage = IsolatedStorageFile.GetUserStoreForDomain();
#else
            _storage = IsolatedStorageFile.GetUserStoreForApplication();
#endif
            
        }

        //
        // ListFiles
        //
        public override idFileList ListFiles(string directory, string extension)
        {
            idFileList list = ReadContent<idFileList>(directory + "/" + "filelist");

            if (list == null)
            {
                return null;
            }

            return list;
        }

        //
        // FreeFileList
        //
        public override void  FreeFileList(ref idFileList list)
        {
            list.Dispose();
        }

        //
        // GetDLLPath
        //
        public override string GetDLLPath(string dllName)
        {
            if (!FileExists(dllName))
            {
                return null;
            }

            return Engine.BASEGAME + "\\" + dllName;
        }


        //
        // OpenContentFileStream
        // Opens a file from the app's content folder, it's assumed the mod folder,
        // is already included in qpath.
        //
        private FileStream OpenContentReadFileStream(string qpath)
        {
            FileStream _file;

            try
            {
                _file = new FileStream(_contentManager.RootDirectory + "/" + qpath, FileMode.Open, FileAccess.Read);
            }
            catch (FileNotFoundException)
            {
                return null;
            }

            return _file;
        }

        //
        // OpenIsoloatedStorageReadFileStream
        //
        private IsolatedStorageFileStream OpenIsoloatedStorageReadFileStream(string qpath)
        {
            IsolatedStorageFileStream _file;

            try
            {
                _file = _storage.OpenFile(qpath, FileMode.Open, FileAccess.Read);
            }
            catch (FileNotFoundException)
            {
                return null;
            }

            _file.Close();

            return _file;
        }

        //
        // HasExtension
        // 
        public override bool HasExtension(string path, string extension)
        {
            string exten = System.IO.Path.GetExtension(path);

            if (exten == extension)
            {
                return true;
            }

            return false;
        }

        //
        // FileExists
        //
        public override bool FileExists(string file)
        {
            idFile f = OpenFileRead(file, true);
            if (f == null)
            {
                // Try without the extension.
                file = Path.GetDirectoryName(file) + "/" + Path.GetFileNameWithoutExtension(file);
                f = OpenFileRead(file, true);
                if (f == null)
                {
                    return false;
                }
            }

            CloseFile(ref f);

            return true;
        }

        //
        // FileIsInPack
        //
        public override bool FileIsInPAK(string filename, ref int pChecksum)
        {
            // Todo fixme!
            return false; 
        }

        //
        // CloseFile
        //
        public override void CloseFile(ref idFile file)
        {
            int fileHandle;
            for (fileHandle = 0; fileHandle < MAX_NUM_OPENFILES; fileHandle++)
            {
                if (filepool[fileHandle] != null && filepool[fileHandle] == file)
                {
                    if (file.GetClassType() == typeof(idFile_Memory))
                    {
                       // fs_loadStack -= file.Length();

                        // Dispose of the file, perform all needed cleanup.
                        ((idFile_Memory)file).Dispose();

                        file = null;
                        filepool[fileHandle] = null;
                        return;
                    }
                    else
                    {
                        Engine.common.ErrorFatal("idFileManager::CloseFile: File has a unknown or invalid class type");
                    }
                }
            }

            // If we tried to close a file that wasn't allocated by the manager, error out.
            Engine.common.ErrorFatal("Failed to close a non allocated file");
        }

        //
        // OpenFileRead
        //
        public override idFile OpenFileRead(string qpath, bool uniqueFILE)
        {
            FileStream _fileStream;
            int fileHandle;

            // Check to see if the file is already opened.
            for (fileHandle = 0; fileHandle < MAX_NUM_OPENFILES; fileHandle++)
            {
                if (filepool[fileHandle] != null && filepool[fileHandle].GetFullFilePath() == qpath)
                {
                    return filepool[fileHandle];
                }
            }

            // Try to find a vacent slot to store the opened file.
            for (fileHandle = 0; fileHandle < MAX_NUM_OPENFILES; fileHandle++)
            {
                if (filepool[fileHandle] == null)
                {
                    break;
                }
            }

            if (fileHandle == MAX_NUM_OPENFILES)
            {
                Engine.common.Warning("Maximum opened files reached\n");
                return null;
            }

            // Replace \\ with /
            qpath = qpath.Replace('\\', '/');

            // Try to open the file from the users content folder.
            _fileStream = OpenContentReadFileStream(qpath);
            if (_fileStream == null)
            {
                // Try to open the file from the content folder.
                //_fileStream = OpenIsoloatedStorageReadFileStream(qpath);
                if (_fileStream == null)
                {
                    return null;
                }
            }

            filepool[fileHandle] = new idFile_Memory(qpath, _fileStream);
            fs_loadStack += filepool[fileHandle].Length();

            return filepool[fileHandle];
        }

        //
        // ReadFile
        //
        public override int ReadFile(string qpath, out byte[] buffer)
        {
            idFile _f;
            
            // If we can't open the file, just return -1.
            _f = OpenFileRead(qpath, true);
            if (_f == null)
            {
                buffer = null;
                return -1;
            }

            buffer = new byte[_f.Length()];
            return _f.Length();
        }

        //
        // FreeFile
        //
        public override void FreeFile(ref byte[] buffer)
        {
            buffer = null;
        }

        //
        // RemoveExtensionFromPath
        //
        public override string RemoveExtensionFromPath( string _filename )
        {
            if (_filename.Contains("\\"))
            {
                return Path.GetDirectoryName(_filename) + "\\" + Path.GetFileNameWithoutExtension(_filename);
            }
            return Path.GetDirectoryName(_filename) + "/" + Path.GetFileNameWithoutExtension(_filename);
        }

        //
        // ReadContent
        //
        public override T ReadContent<T>(string qpath)
        {
            string path = RemoveExtensionFromPath(qpath);
#if !XBOX_360
            if (FileExists(path + ".xnb") == false)
            {
                Engine.common.Warning("FS_ReadContent: File not found " + qpath + "\n");
                return default(T);
            }
#endif
            try
            {
                return _contentManager.Load<T>(path);
            }
            catch (Exception e)
            {
                Engine.common.Warning("FS_ReadContent: " + e.ToString() + "\n");
                return default(T);
            }
        }

        //
        // isInitialized
        //
        public override bool isInitialized()
        {
            return isInit;
        }

        //
        // Init
        //
        public override void Init()
        {
            // Startup the filesystem.
            Startup(Engine.BASEGAME);

            isInit = true;
        }

        //
        // Startup
        //
        private void Startup(string gameName)
        {
            Engine.common.Printf("----- FS_Startup -----\n");
        }

        //
        // CreateDirectory
        //
        public override void CreateDirectory(string path)
        {
            string directory = "";

            for (int i = 0; i < path.Length; i++)
            {
                if (path[i] == '/' || path[i] == '\\')
                {
                    if (directory.Length != 0)
                    {
                        try
                        {
                            _storage.CreateDirectory(directory);
                        }
                        catch (IsolatedStorageException)
                        {
                            // Assume this was thrown because the directory exists already.
                        }
                        directory = "";
                    }
                }
                else
                {
                    directory += path;
                }
            }
        }

        //
        // OpenFileWrite
        //
        public override idFile OpenFileWrite(string qpath)
        {
            FileStream _file;
            int fileHandle = -1;

            // Ensure the mod directory and all the folders needed for the filepath.
            CreateDirectory(Engine.BASEGAME + "/" + qpath);

            // Try to open the file for writing.
            try
            {
                _file = _storage.OpenFile(qpath, FileMode.OpenOrCreate, FileAccess.Write);
            }
            catch (IsolatedStorageException)
            {
                Engine.common.ErrorFatal("Failed to open %s for writing\n", qpath);
                return null;
            }

            // Try to find a vacent slot to store the file.
            for (fileHandle = 0; fileHandle < MAX_NUM_OPENFILES; fileHandle++)
            {
                if (filepool[fileHandle] == null)
                {
                    break;
                }
            }

            filepool[fileHandle] = new idFile_Memory(qpath, _file);

            return filepool[fileHandle];
        }

        //
        // RenameStorageFile
        //
        public override void RenameStorageFile(string from, string to)
        {
            if (FileExists(from) == false)
            {
                Engine.common.Warning("Failed to rename %s to %s because the file can't be found\n", from, to);
                return;
            }
#if !XBOX360
            _storage.MoveFile(from, to);
#endif
        }

        //
        // WriteFile
        //
        public override int WriteFile(string qpath, byte[] buffer, int size)
        {
            idFile _file;

            // Open up the file for writing.
            _file = OpenFileWrite(qpath);
            if (_file == null)
            {
                return -1;
            }

            _file.WriteBytes(buffer);
            _file.Flush();

            CloseFile(ref _file);

            return size;
        }

        //
        // FS_LoadStack
        //
        public override int FS_LoadStack()
        {
            return fs_loadStack;
        }

        //
        // Delete
        //
        public override bool Delete(string filename)
        {
            try
            {
                _storage.DeleteFile(filename);
            }
            catch (IsolatedStorageException)
            {
                return false;
            }

            return true;
        }

        //
        // Shutdown
        //
        public override void Shutdown()
        {
            // Close any open files.
            for (int fileHandle = 0; fileHandle < MAX_NUM_OPENFILES; fileHandle++)
            {
                if (filepool[fileHandle] != null)
                {
                    CloseFile(ref filepool[fileHandle]);
                }
            }

            isInit = false;
        }

        //
        // Restart
        //
        public override void Restart(int checksumFeed)
        {
            Shutdown();
        }

        //
        // ConditionalRestart
        //
        public override void ConditionalRestart(int checksumFeed)
        {
            Shutdown();
        }
    }
}
