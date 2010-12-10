using System;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using idLib.Engine.Content;

namespace rtcw.framework.Content
{
    [ContentTypeWriter]
    public class jvDirectoryListWriter : ContentTypeWriter<idFileList>
    {
        protected override void Write(ContentWriter output, idFileList value)
        {
            output.Write(value.Count);
            for (int i = 0; i < value.Count; i++)
            {
                output.Write((string)value[i]);
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            // TODO: change this to the name of your ContentTypeReader
            // class which will be used to load this data.
            return "rtcw.Framework.idFileListReader, rtcw";
        }
    }
}
