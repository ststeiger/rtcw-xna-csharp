// ui_processor.cs (c) 2010 JV Software
//

using System;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace idLib.Engine.Content.ui
{
    [ContentProcessor(DisplayName = "RTCW UserInterface Processor")]
    public class idUserInterfaceProcessor : ContentProcessor<idUserInterfaceFile, idUserInterfaceCompiledContent>
    {
        public override idUserInterfaceCompiledContent Process(idUserInterfaceFile input, ContentProcessorContext context)
        {
            return new idUserInterfaceCompiledContent(ref input);
        }
    }
}