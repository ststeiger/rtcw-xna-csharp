using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace xnacontentbuilder
{
    //
    // idAssetContentLump
    //
    struct idAssetContentLump
    {
        public string filename;
        public uint inputcrc;
        public uint outputcrc;

        public void InitFromFile(ref BinaryReader reader)
        {
            filename = reader.ReadString();
            inputcrc = reader.ReadUInt32();
            outputcrc = reader.ReadUInt32();
        }

        public void WriteToFile(ref BinaryWriter writer)
        {
            writer.Write(filename);
            writer.Write(inputcrc);
            writer.Write(outputcrc);
        }
    };

    //
    // idAssetToc
    //
    public class idAssetToc
    {
        private List<idAssetContentLump> assetContentLump = new List<idAssetContentLump>();
        private const string TOCFILENAME = "assettoc.xnb";
        private const int tocheader = (('C' << 24) + ('O' << 16) + ('T' << 8) + 'J');

        string contentProjectHeader = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                                        "<Project DefaultTargets=\"Build\" ToolsVersion=\"4.0\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">\n" +
                                          "<PropertyGroup>\n" +
                                            "<ProjectGuid>{02ADCFCE-27D6-48FB-B5FE-3B6016BB139C}</ProjectGuid>\n" +
                                            "<ProjectTypeGuids>{96E2B04D-8817-42c6-938A-82C39BA4D311};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}</ProjectTypeGuids>\n" +
                                            "<Configuration Condition=\" '$(Configuration)' == '' \">Debug</Configuration>\n" +
                                            "<Platform Condition=\" '$(Platform)' == '' \">x86</Platform>\n" +
                                            "<OutputType>Library</OutputType>\n" +
                                            "<AppDesignerFolder>Properties</AppDesignerFolder>\n" +
                                            "<TargetFrameworkVersion>v4.0</TargetFrameworkVersion>\n" +
                                            "<XnaFrameworkVersion>v4.0</XnaFrameworkVersion>\n" +
                                            "<OutputPath>bin\\$(Platform)\\$(Configuration)</OutputPath>\n" +
                                            "<ContentRootDirectory>main</ContentRootDirectory>\n" +
                                          "</PropertyGroup>\n" +
                                          "<PropertyGroup Condition=\"'$(Configuration)|$(Platform)' == 'Debug|x86'\">\n" +
                                            "<PlatformTarget>x86</PlatformTarget>\n" +
                                          "</PropertyGroup>\n" +
                                          "<PropertyGroup Condition=\"'$(Configuration)|$(Platform)' == 'Release|x86'\">\n" +
                                            "<PlatformTarget>x86</PlatformTarget>\n" +
                                          "</PropertyGroup>\n" +
                                          "<PropertyGroup>\n" +
                                            "<RootNamespace>rtcwContent</RootNamespace>\n" +
                                          "</PropertyGroup>\n" +
                                          "<ItemGroup>\n";

        string contentProjectFooter = "</ItemGroup>\n" +
                                      "<Import Project=\"$(MSBuildExtensionsPath)\\Microsoft\\XNA Game Studio\\$(XnaFrameworkVersion)\\Microsoft.Xna.GameStudio.ContentPipeline.targets\" />\n" +
                                      "<!--  To modify your build process, add your task inside one of the targets below and uncomment it. \n" +
                                           "Other similar extension points exist, see Microsoft.Common.targets.\n" +
                                      "<Target Name=\"BeforeBuild\">\n" +
                                      "</Target>\n" +
                                      "<Target Name=\"AfterBuild\">\n" +
                                      "</Target>\n" +
                                      "-->\n" +
                                    "</Project>\n";

        //
        // WriteTOCHeader
        //
        private void WriteTOCHeader()
        {
            BinaryWriter writer = new BinaryWriter(new FileStream(Program.BASEDIR + TOCFILENAME, FileMode.OpenOrCreate));

            writer.Write(tocheader);
            writer.Write(assetContentLump.Count);

            writer.Dispose();
        }

        //
        // AppendFileLump
        //
        private void AppendFileLump(idAssetContentLump lump)
        {
            BinaryWriter writer = new BinaryWriter(new FileStream(Program.BASEDIR + TOCFILENAME, FileMode.Open));
            writer.BaseStream.Position = writer.BaseStream.Length;

            lump.WriteToFile(ref writer);
            assetContentLump.Add(lump);

            writer.BaseStream.Position = 0;
            writer.Write(tocheader);
            writer.Write(assetContentLump.Count);

            writer.Dispose();
        }

        //
        // LoadAssetToc
        //
        public void LoadAssetToc()
        {
            BinaryReader tocreader;
            Directory.CreateDirectory(Program.BASEDIR);

            if (!File.Exists(Program.BASEDIR + TOCFILENAME))
            {
                Program.Warning("LoadAssetToc: Failed to load asset table of contents, generating all assets. \n");
                WriteTOCHeader();
                return;
            }

            tocreader = new BinaryReader(new FileStream(Program.BASEDIR + TOCFILENAME, FileMode.Open));
            if (tocreader.ReadInt32() != tocheader)
            {
                Program.Error("File TOC incorrect header. \n");
            }

            // Read back all the files that are in the TOC.
            int numFilesInToc = tocreader.ReadInt32();
            for (int i = 0; i < numFilesInToc; i++)
            {
                idAssetContentLump assetlump = new idAssetContentLump();

                assetlump.InitFromFile(ref tocreader);
                assetContentLump.Add(assetlump);
            }
        }

        //
        // AppendAssetFilePath
        //
        public void AppendAssetFilePath(string outputpath, uint inputcrc, uint outputcrc)
        {
            idAssetContentLump lump = new idAssetContentLump();

            lump.filename = outputpath;
            lump.inputcrc = inputcrc;
            lump.outputcrc = outputcrc;
            AppendFileLump(lump);
        }

        //
        // GenerateCrcForFile
        //
        public uint GenerateCrcForFile(string filepath)
        {
            Crc32 crc32 = new Crc32();
            uint crc;
            using (FileStream f = File.Open(filepath, FileMode.Open)) 
                crc32.ComputeHash(f);
            crc = crc32.CrcValue;

            crc32.Dispose();

            return crc;
        }

        //
        // ShouldGenerateFile
        //
        public bool ShouldGenerateFile(string filepath)
        {
            string outputfile = Program.contentBuilder.GetOutputFileName(Program.contentBuilder.CurrentBuildFile);
            if (!File.Exists(Program.BASEDIR + outputfile))
            {
                return true;
            }
            for (int i = 0; i < assetContentLump.Count; i++)
            {
                if (assetContentLump[i].filename == outputfile)
                {
                    if (GenerateCrcForFile(filepath) == assetContentLump[i].inputcrc)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        //
        // BuildContentProject
        //
#if false
        public void BuildContentProject()
        {
            TextWriter writer = new StreamWriter(Program.BASEDIR + "rtcwContent.contentproj");

            writer.Write(contentProjectHeader);
            for (int i = 0; i < assetContentLump.Count; i++)
            {
                if (assetContentLump[i].filename.Contains(".wav"))
                {
                    writer.Write("<Compile Include=\"" + assetsFinalTOC[i] + "\">\n");
                    writer.Write("\t<Name>split3</Name>\n");
                    writer.Write("\t<Importer>WavImporter</Importer>\n");
                    writer.Write("\t<Processor>SoundEffectProcessor</Processor>\n");
                    writer.Write("\t<ProcessorParameters_Quality>Medium</ProcessorParameters_Quality>\n");
                    writer.Write("</Compile>\n");
                }
                else if (assetsFinalTOC[i].Contains(".mp3"))
                {
                    writer.Write("<Compile Include=\"" + assetsFinalTOC[i] + "\">\n");
                    writer.Write("\t<Name>split3</Name>\n");
                    writer.Write("\t<Importer>Mp3Importer</Importer>\n");
                    writer.Write("\t<Processor>SongProcessor</Processor>\n");
                    writer.Write("\t<ProcessorParameters_Quality>Medium</ProcessorParameters_Quality>\n");
                    writer.Write("</Compile>\n");
                }
                else
                {
                    writer.Write("<None Include=\"" + assetsFinalTOC[i] + "\">\n");
                    writer.Write("\t<Name>" + Path.GetFileNameWithoutExtension(assetsFinalTOC[i]) + "</Name>\n");
                    writer.Write("\t<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>\n");
                    writer.Write("</None>\n");
                }
            }

            writer.Write(contentProjectFooter);

            writer.Dispose();
        }
#endif
        //
        // AddAssetToToc
        //
        public void AddAssetToToc(string inputpath, string outputpath)
        {
            uint inputcrc = GenerateCrcForFile(inputpath);

            AppendAssetFilePath(outputpath, inputcrc, 0);
        }
    }
}
