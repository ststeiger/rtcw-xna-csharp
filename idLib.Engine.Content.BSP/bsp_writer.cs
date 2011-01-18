using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using SevenZip;

namespace idLib.Engine.Content.BSP
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentTypeWriter]
    public class bsp_writer : ContentTypeWriter<idBsp>
    {
        protected override void Write(ContentWriter output, idBsp value)
        {
            MemoryStream compstream = new MemoryStream();
            BinaryWriter file = new BinaryWriter(compstream);

            value.WriteBSP(ref file);
            output.Write(SevenZip.Compression.LZMA.SevenZipHelper.Compress(compstream.ToArray()));
        }

        protected override bool ShouldCompressContent(TargetPlatform targetPlatform, object value)
        {
            return false;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "notused";
        }
    }
}
