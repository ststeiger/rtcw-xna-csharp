using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Game.Content.Scripts;
using idLib.Game;

namespace xnacontentbuilder
{
    //
    // idGameScriptManager
    //
    public class idGameScriptContentManager : idContentManager
    {
        public List<int> fileTableLookup = new List<int>();
        string[] textureFileTypes = new string[] { ".script", ".ai" };
        idScriptProcessor scriptprocessor = new idScriptProcessor();
        idGameScriptImporter scriptimporter = new idGameScriptImporter();

        public override void Init()
        {
            Program.Print("...Level/AI Script Content Processor.\n");
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
            Program.Print("Level/AI Script Asset Count: " + fileTableLookup.Count + "\n");
        }

        public override void BuildAssets()
        {
            Program.LockPrintPosition();
            for (int i = 0; i < fileTableLookup.Count; i++)
            {
                float loadpercent;
                BinaryWriter output;
                idGameScriptParser scriptparser;

                loadpercent = (float)i / (float)fileTableLookup.Count;
                loadpercent *= 100;

                Program.contentBuilder.CurrentBuildFile = Program.contentBuilder[fileTableLookup[i]];
                if (Program.contentBuilder.ShouldGenerateOutputFile(Program.contentBuilder.CurrentBuildFile) == false)
                {
                    continue;
                }

                // Open the file for writing
                output = Program.contentBuilder.OpenCurrentBuildFileForWriting();

                Program.Print("Building Script(" + (int)loadpercent + "%): " + Program.contentBuilder.CurrentBuildFile + "                          \n");

                // Import and process the BSP.
                scriptparser = scriptimporter.Import(Program.contentBuilder.CurrentBuildFile, Program.contentBuilder.XnaImporter);
                scriptparser = scriptprocessor.Process(scriptparser, Program.contentBuilder.XnaProcessor);

                scriptparser.WriteToFile(ref output);

                output.Close();
            }

            Program.UnlockPrintPosition();
        }
    }
}
