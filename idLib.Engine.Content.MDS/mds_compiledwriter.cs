using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using SevenZip;

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
        CoderPropID[] propIDs = 
				{
					CoderPropID.DictionarySize,
					CoderPropID.PosStateBits,
					CoderPropID.LitContextBits,
					CoderPropID.LitPosBits,
					CoderPropID.Algorithm,
					CoderPropID.NumFastBytes,
					CoderPropID.MatchFinder,
					CoderPropID.EndMarker
				};
        
        protected override void Write(ContentWriter output, idModelMDS value)
        {
#if HUFFMAN_COMPRESS
            MemoryStream compstream = new MemoryStream();
            BinaryWriter file = new BinaryWriter(compstream);
            idHuffman huffman;

            value.WriteMDSBinary(ref file);

            huffman = new idHuffman(compstream);

            output.Write(huffman.Encode());
#elif false
            MemoryStream compstream = new MemoryStream();
            BinaryWriter file = new BinaryWriter(compstream);

            Int32 posStateBits = 2;
            Int32 litContextBits = 3; // for normal files
            // UInt32 litContextBits = 0; // for 32-bit data
            Int32 litPosBits = 0;
            // UInt32 litPosBits = 2; // for 32-bit data
            Int32 algorithm = 2;
            Int32 numFastBytes = 128;

            object[] properties = 
				{
					(Int32)(1 << 23),
					(Int32)(posStateBits),
					(Int32)(litContextBits),
					(Int32)(litPosBits),
					(Int32)(algorithm),
					(Int32)(numFastBytes),
					"bt4",
					false
				};

            value.WriteMDSBinary(ref file);

            SevenZip.Compression.LZMA.Encoder encoder = new SevenZip.Compression.LZMA.Encoder();
            encoder.SetCoderProperties(propIDs, properties);
            encoder.WriteCoderProperties(output.BaseStream);

            output.Write((int)compstream.Length);

            encoder.Code(file.BaseStream, output.BaseStream, -1, -1, null);
#else
            MemoryStream compstream = new MemoryStream();
            BinaryWriter file = new BinaryWriter(compstream);

            value.WriteMDSBinary(ref file);
           // output.Write(SevenZip.Compression.LZMA.SevenZipHelper.Compress(compstream.ToArray()));
            output.Write(compstream.ToArray());
#endif
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
