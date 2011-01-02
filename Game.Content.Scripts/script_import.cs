// content_import.cs (c) 2010 JV Software
//

using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;

using idLib;
using idLib.Game;

namespace Game.Content.Scripts
{
    [ContentImporter(".script", DisplayName = "RTCW Script Importer", DefaultProcessor = "RTCW Script Processor")]
    public class idGameScriptImporter : ContentImporter<idGameScriptParser>
    {
        public override idGameScriptParser Import(string filename, ContentImporterContext context)
        {
            string buffer = File.ReadAllText(filename, System.Text.Encoding.ASCII);
            idParser parser = new idParser(buffer);

            idGameScriptParser gameScript = new idGameScriptParser(filename, ref parser);
            parser.Dispose();

            return gameScript;
        }
    }
}
