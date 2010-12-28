using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;

using idLib.Engine.Content;

namespace idLib.Engine.Content.MDS
{
    [ContentImporter(".mds", DisplayName = "RTCW MDS Model Importer", DefaultProcessor = "RTCW MDS Model Processor")]
    public class idSkeletalModelImporter : ContentImporter<idModelMDS>
    {
        public override idModelMDS Import(string filename, ContentImporterContext context)
        {
            FileStream fstream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            if (fstream == null)
            {
                throw new Exception("Failed to open file.\n");
            }

            return new idModelMDS(new BinaryReader(fstream));
        }
    }
}
