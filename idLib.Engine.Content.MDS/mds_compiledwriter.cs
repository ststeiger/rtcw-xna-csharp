using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using idLib.Engine.Content;

namespace idLib.Engine.Content.MDS
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentTypeWriter]
    public class idSkeletalModelWriter : ContentTypeWriter<idModelMDS>
    {
        protected override void Write(ContentWriter output, idModelMDS value)
        {
            BinaryWriter file = (BinaryWriter)output;
            value.WriteMDSBinary(ref file);
            output = (ContentWriter)file;
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
