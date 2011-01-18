using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace idLib.Engine.Content.BSP
{
    [ContentProcessor(DisplayName = "RTCW BSP Processor")]
    public class idBSPProcessor : ContentProcessor<idBsp, idBsp>
    {
        public override idBsp Process(idBsp input, ContentProcessorContext context)
        {
            input.BuildWorld(context);
            return input;
        }
    }
}