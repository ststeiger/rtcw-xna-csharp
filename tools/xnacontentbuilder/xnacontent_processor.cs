using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

using TOutput = System.String;
using TInput = System.String;

namespace xnacontentbuilder
{
    //
    // idXnaContentProcessor
    //
    public class idXnaContentProcessor : ContentProcessorContext
    {
        idXnaContentImportLogger logger = new idXnaContentImportLogger();
        OpaqueDataDictionary parameters = new OpaqueDataDictionary();

        public override TargetPlatform TargetPlatform { 
            get 
            { 
                return TargetPlatform.Windows; 
            } 
        }

        public override GraphicsProfile TargetProfile { 
            get { 
                return GraphicsProfile.Reach; 
            } 
        }

        public override string BuildConfiguration { 
            get { 
                return string.Empty; 
            } 
        }

        public override string IntermediateDirectory { 
            get 
            { 
                return string.Empty; 
            } 
        }

        public override string OutputDirectory 
        { 
            get 
            {
                return Program.BASEDIR; 
            } 
        }

        public override string OutputFilename 
        { 
            get 
            {
                string dir;

                dir = Path.GetDirectoryName(Program.contentBuilder.CurrentBuildFile);

                if (dir.Length <= 0)
                {
                    return Program.BASEDIR + Path.GetFileNameWithoutExtension(Program.contentBuilder.CurrentBuildFile);
                }
                return Program.BASEDIR + Path.GetDirectoryName(Program.contentBuilder.CurrentBuildFile) + "/" + Path.GetFileNameWithoutExtension(Program.contentBuilder.CurrentBuildFile);
            } 
        }

        public override OpaqueDataDictionary Parameters 
        { 
            get 
            { 
                return parameters; 
            } 
        }
        

        public override ContentBuildLogger Logger 
        { 
            get 
            { 
                return logger; 
            } 
        }

        public override void AddDependency(string filename) { 
        
        }
        public override void AddOutputFile(string filename) {
            Program.contentBuilder.AddAssetToTOC(filename);
        }

        public override TOutput Convert<TInput, TOutput>(TInput input, string processorName, OpaqueDataDictionary processorParameters) { 
            throw new NotImplementedException(); 
        }

        public override TOutput BuildAndLoadAsset<TInput, TOutput>(ExternalReference<TInput> sourceAsset, string processorName, OpaqueDataDictionary processorParameters, string importerName) 
        { 
            throw new NotImplementedException(); 
        }

        public override ExternalReference<TOutput> BuildAsset<TInput, TOutput>(ExternalReference<TInput> sourceAsset, string processorName, OpaqueDataDictionary processorParameters, string importerName, string assetName) 
        { 
            throw new NotImplementedException(); 
        }
    }
}
