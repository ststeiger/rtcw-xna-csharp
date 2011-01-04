using System;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using idLib.Engine.Content;

namespace idLib.Engine.Content.Materials
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentTypeWriter]
    public class idMaterialContentWriter : ContentTypeWriter<idMaterialLookupTable>
    {
        protected override void Write(ContentWriter output, idMaterialLookupTable value)
        {
            output.Write(value.Count);
            
            for (int i = 0; i < value.Count; i++)
            {
                output.Write(value.GetMaterial(i).hashValue);
                output.Write(value.GetMaterial(i).buffer);
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            // TODO: change this to the name of your ContentTypeReader
            // class which will be used to load this data.
            return "rtcw.Renderer.idMaterialTableReader, rtcw";
        }
    }
}
