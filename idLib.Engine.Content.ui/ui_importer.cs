// ui_importer.cs (c) 2010 JV Software
//

using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using idLib;

namespace idLib.Engine.Content.ui
{
    //
    // idUserInterfaceFile
    // 
    public class idUserInterfaceFile
    {
        idParser parser;

        //
        // idUSerInterfaceFile
        //
        public idUserInterfaceFile(string filename)
        {
            BinaryReader fstream;
            string uibuffer;
            
            // Load in the entire UI file for parsing.
            fstream = new BinaryReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            uibuffer = new string( fstream.ReadChars((int)fstream.BaseStream.Length ) );

            // Alloc the parser from the stream.
            parser = new idParser(uibuffer);

            // Close the filestream.
            fstream.Close();
        }

        //
        // Parser
        //
        public idParser Parser
        {
            get
            {
                return parser;
            }
        }

        //
        // Dispose
        //
        public void Dispose()
        {
            parser.Dispose();
        }
    }

    //
    // idUserInterfaceImporter
    //
    [ContentImporter(".menu", DisplayName = "RTCW UserInterface Importer", DefaultProcessor = "RTCW UserInterface Processor")]
    public class idUserInterfaceImporter : ContentImporter<idUserInterfaceFile>
    {
        public override idUserInterfaceFile Import(string filename, ContentImporterContext context)
        {
            return new idUserInterfaceFile(filename);
        }
    }
}
