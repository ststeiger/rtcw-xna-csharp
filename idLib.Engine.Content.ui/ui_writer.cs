// ui_writer.cs (c) 2010 JV Software
//

using System;
using idLib.Math;
using idLib.Engine.Content.ui.Private;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace idLib.Engine.Content.ui
{
    [ContentTypeWriter]
    public class idUserInterfaceWriter : ContentTypeWriter<idUserInterfaceCompiledContent>
    {
        protected override void Write(ContentWriter output, idUserInterfaceCompiledContent value)
        {
            System.IO.BinaryWriter writer = (System.IO.BinaryWriter)output;
            value.assets.WriteBinaryFile(ref writer);
            value.menudef.WriteBinaryFile(ref writer);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "idLib.Engine.Public.idUserInterfaceContentReader, idLib";
        }
    }
}
