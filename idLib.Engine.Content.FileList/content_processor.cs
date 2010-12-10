using System;
using Microsoft.Xna.Framework.Content.Pipeline;

using idLib.Engine.Content;

namespace idLib.Engine.Content.FileList
{
    [ContentProcessor(DisplayName = "File List Processor")]
    public class jvDirectoryProcessor : ContentProcessor<idFileList, idFileList>
    {
        public override idFileList Process(idFileList input, ContentProcessorContext context)
        {
            return input;
        }
    }
}