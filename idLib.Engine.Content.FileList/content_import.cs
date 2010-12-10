// content_import.cs (c) 2010 JV Software
//

using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;

using idLib.Engine.Content;

namespace idLib.Engine.Content.FileList
{
    [ContentImporter(".txt", DisplayName = "FileList Importer", DefaultProcessor = "File List Processor")]
    public class jvDirectoryImporter : ContentImporter<idFileList>
    {
        public override idFileList Import(string filename, ContentImporterContext context)
        {
            DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(filename));
            FileInfo[] rgFiles = di.GetFiles("*.*");
            idFileList fileList = new idFileList();

            foreach (FileInfo fi in rgFiles)
            {
                if (fi.Name.Contains( "filelist" ) == false)
                {
                    fileList.AddFileToList(Path.GetFileNameWithoutExtension(fi.Name));
                }
            }

            return fileList;
        }
    }
}
