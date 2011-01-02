using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;

using idLib;
using idLib.Game;

namespace idLib.Engine.Content.FileList
{
    [ContentProcessor(DisplayName = "RTCW Script Processor")]
    public class idScriptProcessor : ContentProcessor<idGameScriptParser, idGameScriptParser>
    {
        public override idGameScriptParser Process(idGameScriptParser input, ContentProcessorContext context)
        {
            return input;
        }
    }
}