using System;
using Microsoft.Xna.Framework.Content.Pipeline;

using idLib.Engine.Content;

namespace rtcw.renderer.Content
{
    //
    // idMaterialSource
    // 
    public class idMaterialSource
    {
        public idMaterialSource(string src, string filename)
        {
            mtrsource = src;
            mtrfilename = filename;
        }
        public string mtrsource;
        public string mtrfilename;
    }

    [ContentImporter(".shader", DisplayName = "RTCW Matrial Importer", DefaultProcessor = "idMaterialProcessor")]
    public class MaterialImporter : ContentImporter<idMaterialSource>
    {
        public override idMaterialSource Import(string filename, ContentImporterContext context)
        {
            string mtrbuffer = System.IO.File.ReadAllText(filename);

            return new idMaterialSource(mtrbuffer, filename);
        }
    }
}
