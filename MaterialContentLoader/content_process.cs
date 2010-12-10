using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content.Pipeline;
using idLib;
using idLib.Engine.Content;

namespace rtcw.renderer.Content
{
    //
    // MaterialProcessor
    //
    [ContentProcessor(DisplayName = "RTCW Material Processor")]
    public class idMaterialProcessor : ContentProcessor<idMaterialSource, idMaterialLookupTable>
    {
        public override idMaterialLookupTable Process(idMaterialSource input, ContentProcessorContext context)
        {
            idMaterialLookupTable table = new idMaterialLookupTable();
            idParser parser = new idParser(input.mtrsource);

            while (parser.ReachedEndOfBuffer == false)
            {
                idMaterialCached mtrcache = new idMaterialCached();

                mtrcache.mtrname = parser.NextToken;
                if (mtrcache.mtrname == null || mtrcache.mtrname.Length <= 0)
                {
                    break;
                }

                mtrcache.buffer = parser.NextBracketSection;

                table.AddMaterialToCache(mtrcache);
            }

            return table;
        }
    }
}