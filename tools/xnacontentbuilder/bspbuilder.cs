using System;
using System.Collections.Generic;
using System.IO;
using idLib.Engine.Content.BSP;

namespace xnacontentbuilder
{
    //
    // idBspManager
    //
    public class idBspContentManager : idContentManager
    {
        public List<int> fileTableLookup = new List<int>();
        string[] textureFileTypes = new string[] { ".bsp" };
        idBspImporter bspImporter;
        idBSPProcessor bspProcessor;

        public override void Init()
        {
            Program.Print("...BSP Content Processor.\n");
            bspImporter = new idBspImporter();
            bspProcessor = new idBSPProcessor();
        }

        public override bool AddAsset(string path, int offset)
        {
            foreach (string fileExtension in textureFileTypes)
            {
                if (path.Contains(fileExtension))
                {
                    fileTableLookup.Add(offset);
                    return true;
                }
            }

            return false;
        }

        public override void PrintStats()
        {
            Program.Print("BSP Asset Count: " + fileTableLookup.Count + "\n");
        }

        public override void BuildAssets()
        {
            Program.LockPrintPosition();
            for (int i = 0; i < fileTableLookup.Count; i++)
            {
                float loadpercent;
                BinaryWriter output;
                idBsp bsp;
                BinaryWriter memfile = new BinaryWriter(new MemoryStream());

                loadpercent = (float)i / (float)fileTableLookup.Count;
                loadpercent *= 100;

                Program.contentBuilder.CurrentBuildFile = Program.contentBuilder[fileTableLookup[i]];
                if (Program.contentBuilder.ShouldGenerateOutputFile(Program.contentBuilder.CurrentBuildFile) == false)
                {
                    continue;
                }

                // Open the file for writing
                output = Program.contentBuilder.OpenCurrentBuildFileForWriting();

                Program.Print("Building BSP(" + (int)loadpercent + "%): " + Program.contentBuilder.CurrentBuildFile + "                          \n");

                // Import and process the BSP.
                bsp = bspImporter.Import( Program.contentBuilder.CurrentBuildFile, Program.contentBuilder.XnaImporter );
                bsp = bspProcessor.Process( bsp, Program.contentBuilder.XnaProcessor );
                
                bsp.WriteBSP(ref memfile);
                output.Write(SevenZip.Compression.LZMA.SevenZipHelper.Compress(((MemoryStream)memfile.BaseStream).ToArray()));
                memfile.Dispose();

                output.Close();
            }
            Program.UnlockPrintPosition();
        }
    }
}
