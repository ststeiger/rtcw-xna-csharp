using System;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using idLib.Game;
using idLib.Engine.Content;

namespace idLib.Engine.Content.FileList
{
    [ContentTypeWriter]
    public class idGameScriptWriter : ContentTypeWriter<idGameScriptParser>
    {
        protected override void Write(ContentWriter output, idGameScriptParser value)
        {
            System.IO.BinaryWriter outp = (System.IO.BinaryWriter)output;
            value.WriteToFile(ref outp);
            output = (ContentWriter)outp;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "idLib.Game.idScriptReader, idLib";
        }
    }
}
