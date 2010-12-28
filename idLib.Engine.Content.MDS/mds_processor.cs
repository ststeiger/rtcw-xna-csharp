// mds_processor.cs (c) 2010 JV SOftware
//

using System;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace idLib.Engine.Content.MDS
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    ///
    /// TODO: change the ContentProcessor attribute to specify the correct
    /// display name for this processor.
    /// </summary>
    [ContentProcessor(DisplayName = "RTCW MDS Model Processor")]
    public class idSkeletalModelProcessor : ContentProcessor<idModelMDS, idModelMDS>
    {
        public override idModelMDS Process(idModelMDS input, ContentProcessorContext context)
        {
            return input;
        }
    }
}