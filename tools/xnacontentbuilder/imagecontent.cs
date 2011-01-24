using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using idLib;
using idLib.Engine.Public;
using idLib.Engine.Content.Texture;

namespace xnacontentbuilder
{
    //
    // idImageContentManager
    //
    public class idImageContentManager : idContentManager
    {
        public List<int> fileTableLookup = new List<int>();
        private idTextureImporter textureImport;
        private idTextureProcessor textureProcessor;

        string[] textureFileTypes = new string[] { ".bmp", ".dds", ".dib", ".hdr", ".jpg", ".pfm", ".png", ".ppm", ".tga" };

        public override void Init()
        {
            Program.Print("...Image Content Processor.\n");
            textureImport = new idTextureImporter();
            textureProcessor = new idTextureProcessor();
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
            Program.Print("Image Asset Count: " + fileTableLookup.Count + "\n");
        }

        public override void BuildAssets()
        {
            idImageDataTable imageTable = new idImageDataTable();

            imageTable.InitNewTable(Program.BASEDIR + "textures/texturedata.xnb");

            Program.LockPrintPosition();

            for (int i = 0; i < fileTableLookup.Count; i++)
            {
                idTexture2DContent texture2d;
                uint imagecrc = Program.contentBuilder.GenerateCrcForFile(Program.contentBuilder.CurrentBuildFile);

                float loadpercent = (float)i / (float)fileTableLookup.Count;
                loadpercent *= 100;

                Program.contentBuilder.CurrentBuildFile = Program.contentBuilder[fileTableLookup[i]];

                // Check to see if the image has been updated or not.
                if(!imageTable.ExternalFileIsNewer( Program.contentBuilder.CurrentBuildFile, imagecrc))
                {
                    continue;
                }

                Program.Print("Building Images(" + (int)loadpercent + "%): " + Program.contentBuilder.CurrentBuildFile + "                          \n");

                // Import the texture.
                texture2d = textureImport.Import(Program.contentBuilder.CurrentBuildFile, Program.contentBuilder.XnaImporter);

                // Do any processing we need to do for the image.
                texture2d = textureProcessor.Process(texture2d, Program.contentBuilder.XnaProcessor);

                imageTable.AddImageToTable(Program.contentBuilder.CurrentBuildFile, 
                                           imagecrc,
                                           texture2d.texture.Faces[0][0].Width,
                                           texture2d.texture.Faces[0][0].Height,
                                           SevenZip.Compression.LZMA.SevenZipHelper.Compress(texture2d.texture.Faces[0][0].GetPixelData()));
            }

            imageTable.WriteImageDatabase(Program.BASEDIR + "textures/texturedata.xnb");

            Program.UnlockPrintPosition();
        }
    }
}
