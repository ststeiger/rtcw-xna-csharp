using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace idLib.Engine.Content.BSP
{
    
    [ContentImporter(".bsp", DisplayName = "RTCW BSP Importer", DefaultProcessor = "RTCW BSP Processor")]
    public class bsp_importer : ContentImporter<idBsp>
    {
        public override idBsp Import(string filename, ContentImporterContext context)
        {
            BinaryReader reader;
            idBsp content;

            reader = new BinaryReader(new FileStream(filename, FileMode.Open, FileAccess.Read));

            content = new idBsp(filename);
            content.LoadBSP(ref reader);

            reader.Dispose();

            return content;
        }
    }
}
