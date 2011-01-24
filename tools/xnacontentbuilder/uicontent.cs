using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using idLib.Engine.Content.ui;

namespace xnacontentbuilder
{
    //
    // idUIContentManager
    //
    public class idUIContentManager : idContentManager
    {
        public List<int> fileTableLookup = new List<int>();
        string[] textureFileTypes = new string[] { ".menu" };
        idUserInterfaceImporter uiImporter = new idUserInterfaceImporter();
        idUserInterfaceProcessor uiProcessor = new idUserInterfaceProcessor();

        public override void Init()
        {
            Program.Print("...UI Content Processor.\n");
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
            Program.Print("UI Asset Count: " + fileTableLookup.Count + "\n");
        }

        public override void BuildAssets()
        {
            Program.LockPrintPosition();
            for (int i = 0; i < fileTableLookup.Count; i++)
            {
                float loadpercent;
                BinaryWriter output;
                idUserInterfaceFile uiInput;
                idUserInterfaceCompiledContent uiOutput;

                loadpercent = (float)i / (float)fileTableLookup.Count;
                loadpercent *= 100;

                Program.contentBuilder.CurrentBuildFile = Program.contentBuilder[fileTableLookup[i]];
                if (Program.contentBuilder.ShouldGenerateOutputFile(Program.contentBuilder.CurrentBuildFile) == false)
                {
                    continue;
                }

                // Open the file for writing
                output = Program.contentBuilder.OpenCurrentBuildFileForWriting();

                Program.Print("Building UI(" + (int)loadpercent + "%): " + Program.contentBuilder.CurrentBuildFile + "                          \n");

                // Import and process the BSP.
                uiInput = uiImporter.Import(Program.contentBuilder.CurrentBuildFile, Program.contentBuilder.XnaImporter);
                uiOutput = uiProcessor.Process(uiInput, Program.contentBuilder.XnaProcessor);

                uiOutput.assets.WriteBinaryFile(ref output);
                output.Write(uiOutput.menudefpool.Count);

                for (int c = 0; c < uiOutput.menudefpool.Count; c++)
                {
                    uiOutput.menudefpool[c].WriteBinaryFile(ref output);
                }

                output.Close();
            }

            Program.UnlockPrintPosition();
        }
    }
}
