﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using idLib.Engine.Content.MDS;

namespace xnacontentbuilder
{
    //
    // idMDSContentManager
    //
    public class idMDSContentManager : idContentManager
    {
        public List<int> fileTableLookup = new List<int>();
        string[] textureFileTypes = new string[] { ".mds" };
        idSkeletalModelImporter mdsImporter = new idSkeletalModelImporter();
        idSkeletalModelProcessor mdsProcessor = new idSkeletalModelProcessor();
        
        public override void Init()
        {
            Program.Print("...MDS Model Content Processor.\n");
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
            Program.Print("MDS Asset Count: " + fileTableLookup.Count + "\n");
        }

        public override void BuildAssets()
        {
            Program.LockPrintPosition();
            for (int i = 0; i < fileTableLookup.Count; i++)
            {
                float loadpercent;
                BinaryWriter output;
                idModelMDS mdsModel;
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

                Program.Print("Building MDS(" + (int)loadpercent + "%): " + Program.contentBuilder.CurrentBuildFile + "                          \n");

                // Import and process the BSP.
                mdsModel = mdsImporter.Import(Program.contentBuilder.CurrentBuildFile, Program.contentBuilder.XnaImporter);
                mdsModel = mdsProcessor.Process(mdsModel, Program.contentBuilder.XnaProcessor);

                mdsModel.WriteMDSBinary(ref memfile);
                output.Write(SevenZip.Compression.LZMA.SevenZipHelper.Compress(((MemoryStream)memfile.BaseStream).ToArray()));
                memfile.Dispose();

                output.Close();
            }
            Program.UnlockPrintPosition();
        }
    }
}
