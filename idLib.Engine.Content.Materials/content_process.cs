using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content.Pipeline;
using idLib;
using idLib.Engine.Content;

namespace idLib.Engine.Content.Materials
{
    //
    // MaterialProcessor
    //
    [ContentProcessor(DisplayName = "RTCW Material Processor")]
    public class idMaterialProcessor : ContentProcessor<idMaterialSource, idMaterialLookupTable>
    {
        //
        // HashValue
        //
        private int GenerateHashValue(string name)
        {
            int hashValue = 0;
            int strnum = 1;
            name = name.ToLower();
            for (int i = 0; i < name.Length; i++)
            {
                if (name[i] == '.')
                {
                    break;
                }

                if (name[i] == '\\' || name[i] == '/')
                {
                    continue;
                }

                hashValue += name[i] * strnum;
                strnum++;
            }

            return hashValue;
        }
        public override idMaterialLookupTable Process(idMaterialSource input, ContentProcessorContext context)
        {
            idMaterialLookupTable table = new idMaterialLookupTable();
            idParser parser = new idParser(input.mtrsource);

            while (parser.ReachedEndOfBuffer == false)
            {
                idMaterialCached mtrcache = new idMaterialCached();
                string token;

                token = parser.NextToken;
                if (token == null || token.Length <= 0)
                {
                    break;
                }


                mtrcache.hashValue = GenerateHashValue(token);
                mtrcache.buffer = parser.NextBracketSection;

                table.AddMaterialToCache(mtrcache);
            }

            return table;
        }
    }
}