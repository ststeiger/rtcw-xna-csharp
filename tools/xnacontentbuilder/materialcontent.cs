using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using idLib.Engine.Content.Materials;
using idLib.Engine.Content;

namespace xnacontentbuilder
{
    //
    // idMaterialContentManager
    //
    public class idMaterialContentManager : idContentManager
    {
        public List<int> fileTableLookup = new List<int>();
        string[] textureFileTypes = new string[] { ".shader" };
        idMaterialImporter materialImporter = new idMaterialImporter();
        idMaterialProcessor materialProcessor = new idMaterialProcessor();

        public override void Init()
        {
            Program.Print("...Material Content Processor.\n");
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
            Program.Print("Material Asset Count: " + fileTableLookup.Count + "\n");
        }

        public override void BuildAssets()
        {
            Program.LockPrintPosition();
            for (int i = 0; i < fileTableLookup.Count; i++)
            {
                float loadpercent;
                BinaryWriter output;
                idMaterialSource mtrSource;
                idMaterialLookupTable mtrTable;
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

                Program.Print("Building Material(" + (int)loadpercent + "%): " + Program.contentBuilder.CurrentBuildFile + "                          \n");

                // Import and process the BSP.
                mtrSource = materialImporter.Import( Program.contentBuilder.CurrentBuildFile, Program.contentBuilder.XnaImporter );
                mtrTable = materialProcessor.Process( mtrSource, Program.contentBuilder.XnaProcessor );
                
                output.Write(mtrTable.Count);

                for (int c = 0; c < mtrTable.Count; c++)
                {
                    output.Write(mtrTable.GetMaterial(c).hashValue);
                    output.Write(mtrTable.GetMaterial(c).buffer);
                }

                output.Close();
            }

            Program.UnlockPrintPosition();
        }
    }
}
