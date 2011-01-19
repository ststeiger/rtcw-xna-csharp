using System;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace idLib.Engine.Content.Texture
{
    [ContentProcessor(DisplayName = "RTCW Texture Processor")]
    public class idTextureProcessor : ContentProcessor<idTexture2DContent, idTexture2DContent>
    {
        public override idTexture2DContent Process(idTexture2DContent input, ContentProcessorContext context)
        {
            return input;
        }
    }
}