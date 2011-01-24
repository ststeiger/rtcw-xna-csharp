using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace xnacontentbuilder
{
    public abstract class idContentManager
    {
        public abstract void Init();
        public abstract bool AddAsset(string path, int offset);
        public abstract void PrintStats();
        public abstract void BuildAssets();

        public enum idContentManagerType
        {
            CONTENTMANGER_IMAGE = 0,
            CONTENTMANAGER_MDS,
          //  CONTENTMANAGER_MDC,
            CONTENTMANAGER_BSP,
            CONTENTMANAGER_GAMESCRIPT,
            CONTENTMANAGER_MATERIAL,
            CONTENTMANAGER_UI,
          //  CONTENTMANAGER_SOUND,
            MAXCONTENTMANAGERS
        }
    }

    public class idContentBuilder
    {
        private List<string> assetTableOfContents = new List<string>();
        private List<string> assetsToCopyOver = new List<string>();

        private idAssetToc assetTable = new idAssetToc();

        private int numskippedfiles = 0;
        private idContentManager[] contentManagers = new idContentManager[(int)idContentManager.idContentManagerType.MAXCONTENTMANAGERS];
        private string[] importExtWithCompiling = new string[] { ".cfg", ".camera", ".dat", ".sounds", ".aas", ".rcd", ".skin", ".mdc", ".wav", ".mp3", "wolfanim.script"  };
        private idXnaContentImportContext xnaImporter = new idXnaContentImportContext();
        private idXnaContentProcessor xnaProcessor = new idXnaContentProcessor();

        public uint GenerateCrcForFile(string filepath)
        {
            return assetTable.GenerateCrcForFile(filepath);
        }

        public string GetFileNameAndPathWithoutExtension( string _filename )
        {
            string dir;

            dir = Path.GetDirectoryName(_filename);

            if (dir.Length <= 0)
            {
                return Path.GetFileNameWithoutExtension(_filename);
            }
            return Path.GetDirectoryName(_filename) + "/" + Path.GetFileNameWithoutExtension(_filename);
        }

        public bool ShouldGenerateOutputFile(string inputfilepath)
        {
            return assetTable.ShouldGenerateFile(inputfilepath);
        }

        public string GetOutputFileName(string path)
        {
            foreach (string s in importExtWithCompiling)
            {
                if (path.Contains(s))
                {
                    return path;
                }
            }

            return GetFileNameAndPathWithoutExtension(path) + ".xnb";
        }

        public BinaryWriter OpenCurrentBuildFileForWriting()
        {
            string filepath = GetOutputFileName(Program.contentBuilder.CurrentBuildFile);
 
            // Create the directory if we need to.
            Directory.CreateDirectory(Program.BASEDIR + Path.GetDirectoryName(filepath));

            assetTable.AddAssetToToc(Program.contentBuilder.CurrentBuildFile, filepath);

            // Open the output for writing.
            return new BinaryWriter(new FileStream(Program.BASEDIR + filepath, FileMode.Create));
        }

        public idXnaContentProcessor XnaProcessor
        {
            get
            {
                return xnaProcessor;
            }
        }

        public idXnaContentImportContext XnaImporter
        {
            get
            {
                return xnaImporter;
            }
        }
        
        public string CurrentBuildFile;

        public string this[int index]
        {
            get
            {
                return assetTableOfContents[index];
            }
        }

        //
        // FindGameAssets
        //
        private void FindGameAssets(string sDir)
        {
            foreach (string d in Directory.GetDirectories(sDir, "*.*", SearchOption.AllDirectories))
            {
                Program.Print("Scanning Folder(" + assetTableOfContents.Count + ")" + ": " + sDir + "                           \n");

                foreach (string f in Directory.GetFiles(d, "*.*"))
                {
                    string path;
                    StringBuilder pathbuilder = new StringBuilder(f);
                    path = pathbuilder.Remove(0, 2).ToString();

                    bool foundExporter = false;

                    for (int c = 0; c < importExtWithCompiling.Length; c++)
                    {
                        if (path.Contains(importExtWithCompiling[c]))
                        {
                            assetTableOfContents.Add(path);
                            assetsToCopyOver.Add(path);
                            foundExporter = true;
                            break;
                        }
                    }

                    if (foundExporter == true)
                    {
                        continue;
                    }

                    for (int i = 0; i <= (int)idContentManager.idContentManagerType.MAXCONTENTMANAGERS; i++)
                    {
                        if (i == (int)idContentManager.idContentManagerType.MAXCONTENTMANAGERS)
                        {
                            numskippedfiles++;
                            break;
                        }
                        else if (contentManagers[i].AddAsset(path, assetTableOfContents.Count))
                        {
                            assetTableOfContents.Add(path);
                            break;
                        }
                    }
                }
                FindGameAssets(d);
            }

            if (sDir == ".\\")
            {
                Program.Print("Analyze folders complete.                             \n");

                if (numskippedfiles > 0)
                {
                    Program.Warning("FindGameAssets: Skipped " + numskippedfiles + " files, no valid content importer found. \n");
                }
            }
        }

        private void InitContentManagers()
        {
            Program.Print("Init Content Managers...\n");

            for (idContentManager.idContentManagerType type = idContentManager.idContentManagerType.CONTENTMANGER_IMAGE; 
                type < idContentManager.idContentManagerType.MAXCONTENTMANAGERS; type++)
            {
                switch (type)
                {
                    case idContentManager.idContentManagerType.CONTENTMANGER_IMAGE:
                        contentManagers[(int)type] = new idImageContentManager();
                        break;

                    case idContentManager.idContentManagerType.CONTENTMANAGER_UI:
                        contentManagers[(int)type] = new idUIContentManager();
                        break;

                 //   case idContentManager.idContentManagerType.CONTENTMANAGER_SOUND:
                 //       contentManagers[(int)type] = new idSoundContentManager();
                 //       break;

                    case idContentManager.idContentManagerType.CONTENTMANAGER_GAMESCRIPT:
                        contentManagers[(int)type] = new idGameScriptContentManager();
                        break;

                    case idContentManager.idContentManagerType.CONTENTMANAGER_MDS:
                        contentManagers[(int)type] = new idMDSContentManager();
                        break;

                    case idContentManager.idContentManagerType.CONTENTMANAGER_BSP:
                        contentManagers[(int)type] = new idBspContentManager();
                        break;

                 //   case idContentManager.idContentManagerType.CONTENTMANAGER_MDC:
                 //       contentManagers[(int)type] = new idMDCContentManager();
                 //       break;

                    case idContentManager.idContentManagerType.CONTENTMANAGER_MATERIAL:
                        contentManagers[(int)type] = new idMaterialContentManager();
                        break;

                    default:
                        Program.Error("InitContentManagers: Unknown content type\n");
                        break;
                }

                contentManagers[(int)type].Init();
            }
        }

        public void Init()
        {
            Program.PrintColor("--------- idContentBuilderInit -----------\n", ConsoleColor.White);

            // Set the current directory relative to how the sdk is organized.
            Directory.SetCurrentDirectory("../../rtcw/work");

            // Init the file asset toc.
            assetTable.LoadAssetToc();

            // Init the content managers
            InitContentManagers();

            // List all the files in the assets folder.
            Program.LockPrintPosition();
            FindGameAssets(".\\");
            Program.UnlockPrintPosition();
        }

        //
        // PrintAssetStats
        //
        public void PrintAssetStats()
        {
            // Print out all the statistics
            Program.PrintColor("-------- Content Statistics -------\n", ConsoleColor.White);
            for (int i = 0; i < (int)idContentManager.idContentManagerType.MAXCONTENTMANAGERS; i++)
            {
                contentManagers[i].PrintStats();
            }
            Program.Print("Direct copy Asset Count: " + assetsToCopyOver.Count + "\n");
            Program.Print("Total Assets " + assetTableOfContents.Count + " assets.\n");
        }

        

        public void AddAssetToTOC(string filename)
        {
            assetTable.AppendAssetFilePath(filename, 0, 0);
        }

        //
        // idBuildContentAssets
        //
        public void BuildContentAssets()
        {
            // Build all the assets.
            Program.PrintColor("-------- idBuildContentAssets -------\n", ConsoleColor.White);
            
            // Copy over all the assets that are a direct copy.
            Program.Print("Copying over assets that skip processing...\n");
            Program.LockPrintPosition();
            for (int i = 0; i < assetsToCopyOver.Count; i++)
            {
                float loadpercent;

                loadpercent = (float)i / (float)assetsToCopyOver.Count;
                loadpercent *= 100;

                Program.contentBuilder.CurrentBuildFile = assetsToCopyOver[i];
                if (Program.contentBuilder.ShouldGenerateOutputFile(Program.contentBuilder.CurrentBuildFile) == false)
                {
                    continue;
                }

                BinaryReader file = new BinaryReader(new FileStream(assetsToCopyOver[i], FileMode.Open));
                byte[] buffer = file.ReadBytes((int)file.BaseStream.Length);
                file.Dispose();

                BinaryWriter filew = OpenCurrentBuildFileForWriting();

                if (assetsToCopyOver[i].Contains(".aas") || assetsToCopyOver[i].Contains(".rcd"))
                {
                    Program.Print("Compressing Asset(" + (int)loadpercent + "%): " + Program.contentBuilder.CurrentBuildFile + "                          \n");
                    filew.Write(SevenZip.Compression.LZMA.SevenZipHelper.Compress(buffer));
                }
                else
                {
                    Program.Print("Copying Asset(" + (int)loadpercent + "%): " + Program.contentBuilder.CurrentBuildFile + "                          \n");
                    filew.Write(buffer);
                }
                buffer = null;
                filew.Dispose();
            }

            Program.UnlockPrintPosition();

            Program.Print("Building and Writing assets...\n");
            for (int i = 0; i < (int)idContentManager.idContentManagerType.MAXCONTENTMANAGERS; i++)
            {
                contentManagers[i].BuildAssets();
            }

            //Program.Print("Writing Content Project...\n");
            //BuildContentProject();

            Program.Print("Assets Built Successfully...\n");
        }
    }
}
