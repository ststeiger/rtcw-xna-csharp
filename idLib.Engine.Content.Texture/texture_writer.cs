using System;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace idLib.Engine.Content.Texture
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentTypeWriter]
    public class idTextureWriter : ContentTypeWriter<idTexture2DContent>
    {
        //
        // WriteAsset
        // Basically I needed a way to use the content system without XNA writing its file header,
        // I couldn't find a nicer way so I had to use reflection :/. Basically what we do is get finalOutput and Content streams,
        // when I write our asset data use the finalOuput stream, than when we are done set binarywriter stream to the content stream,
        // and set finalOutput stream to the content stream.
        //
        private void WriteAsset(ContentWriter output, idTexture2DContent image)
        {
            byte[] buffer;
            Type contentWriterType = output.GetType();
            FieldInfo finalOutputStreamInfo = contentWriterType.GetField("finalOutput", BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo baseOutputStream = contentWriterType.GetField("OutStream", BindingFlags.Instance | BindingFlags.NonPublic);

            if (finalOutputStreamInfo == null || baseOutputStream == null)
            {
                throw new Exception("WriteAsset: reflection failed.\n");
            }

            Stream finalOutputStream = (Stream)finalOutputStreamInfo.GetValue(output);

            // Set the current stream to the real handle to write the file out.
            baseOutputStream.SetValue(output, finalOutputStream);

            buffer = image.texture.Faces[0][0].GetPixelData();

            output.Write((short)image.texture.Faces[0][0].Width);
            output.Write((short)image.texture.Faces[0][0].Height);
            output.Write(buffer);

            // Dispose of the file handle.
            finalOutputStream.Flush();
            finalOutputStream.Dispose();

            // Now create a temp finalOutputStream so xna can dispose of it later(stupid I know).
            finalOutputStream = new MemoryStream();
            finalOutputStreamInfo.SetValue(output, finalOutputStream);
        }

        protected override bool ShouldCompressContent(TargetPlatform targetPlatform, object value)
        {
            return false;
        }

        protected override void Write(ContentWriter output, idTexture2DContent value)
        {
            WriteAsset(output, value);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "notused";
        }
    }
}
