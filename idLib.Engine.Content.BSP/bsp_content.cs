using System;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

using idLib;
using idLib.Math;


namespace idLib.Engine.Content.BSP
{
    // Yup this class looks like crap :/.
    public class idBsp
    {
        private idMapHeader header;
        private idLightmap[] lightmaps;
        private string mapname;

        //
        // idBsp
        //
        public idBsp(string mappath)
        {
            mapname = Path.GetFileNameWithoutExtension(mappath);
        }

        //
        // LoadBSP
        //
        public void LoadBSP(ref BinaryReader reader)
        {
            header  = new idMapHeader();

            header.ident = reader.ReadInt32();
            header.version = reader.ReadInt32();

            if (header.ident != BSP_IDENT)
            {
                throw new Exception("BSP ident invalid.\n");
            }

            if (header.version != BSP_VERSION)
            {
                throw new Exception("BSP Version invalid.\n");
            }

            // Load in all the lumps
            header.lumps = new lump_t[HEADER_LUMPS];
            for (int i = 0; i < HEADER_LUMPS; i++)
            {
                header.lumps[i].fileofs = reader.ReadInt32();
                header.lumps[i].filelen = reader.ReadInt32();
            }

            // Load in all the lumps into byte arrays.
            for (int i = 0; i < HEADER_LUMPS; i++)
            {
                reader.BaseStream.Position = header.lumps[i].fileofs;

                if (i != LUMP_LIGHTMAPS)
                {
                    header.lumps[i].buffer = reader.ReadBytes(header.lumps[i].filelen);
                }
                else
                {
                    LoadLightmaps(ref reader, header.lumps[i].filelen);
                }
            }
        }

        //
        // WriteBSPLumpHeader
        //
        private void WriteBSPLumpHeader(ref BinaryWriter writer)
        {
            
            for (int i = 0; i < HEADER_LUMPS; i++)
            {
                // Lump entities are stored externally.
                if (i == LUMP_ENTITIES)
                {
                    header.lumps[i].fileofs = -1;
                    header.lumps[i].filelen = -1;
                }
                else if (i == LUMP_LIGHTMAPS)
                {
                    header.lumps[i].filelen = 0;
                    header.lumps[i].filelen = lightmaps.Length;
                }

                writer.Write(header.lumps[i].fileofs);
                writer.Write(header.lumps[i].filelen);
            }
        }

        //
        // WriteBSP
        //
        public void WriteBSP(ref BinaryWriter writer)
        {
            int lumppos;

            // Write out the ident and version.
            writer.Write(BSPCOMPILED_IDENT);
            writer.Write(BSP_VERSION);

            // We are going to update the lump pos/ofs later on.
            lumppos = (int)writer.BaseStream.Position;

            WriteBSPLumpHeader(ref writer);

            // Write out all the lump data and record what offset its at.
            for (int i = 0; i < HEADER_LUMPS; i++)
            {
                header.lumps[i].fileofs = (int)writer.BaseStream.Position;

                if (i == LUMP_ENTITIES)
                {
                    continue;
                }
                else if (i == LUMP_LIGHTMAPS)
                {
                    for (int c = 0; c < lightmaps.Length; c++)
                    {
                        writer.Write(lightmaps[c].dxtimage.Length);
                        writer.Write(lightmaps[c].dxtimage);
                    }
                }
                else
                {
                    writer.Write(header.lumps[i].buffer);
                }
            }

            // Rewrite the lump headers with the updated offsets.
            writer.BaseStream.Position = lumppos;
            WriteBSPLumpHeader(ref writer);
        }

        //
        // BuildWorld
        //
        public void BuildWorld(ContentProcessorContext context)
        {
            // DXT Compress the lightmaps.
            GenerateLightmaps(context);

            // Write the entities to a seperate file.
            GenerateEntities(context);
        }

        //
        // BspStringToEntityDict
        //
        private void BspStringToEntityDict(ref idParser parser, ref idDict dict)
        {
            while (true)
            {
                string token = parser.NextToken;

                if (token == null || token.Length <= 0)
                {
                    throw new Exception("BspStringToEntityDict: Unexpected EOF inside of entity string\n");
                }

                if (token == "}")
                {
                    break;
                }

                string val = parser.NextToken;
                dict.AddKey(token, val);
            }
        }

        //
        // GenerateEntities
        //
        private void GenerateEntities(ContentProcessorContext context)
        {
            idParser parser;
            int numEntities = 0;
            string entity_filepath = context.OutputDirectory + "maps\\" + mapname + ".entities";
            context.AddOutputFile(entity_filepath);

            BinaryWriter writer = new BinaryWriter(new FileStream(entity_filepath, FileMode.Create));

            //writer.Write(header.lumps[LUMP_ENTITIES].buffer);
            writer.Write(numEntities);

            parser = new idParser(Encoding.UTF8.GetString(header.lumps[LUMP_ENTITIES].buffer, 0, header.lumps[LUMP_ENTITIES].buffer.Length - 1).Trim('\0'));

            while (parser.ReachedEndOfBuffer == false)
            {
                string token = parser.NextToken;

                if (token == null || token.Length <= 0)
                    break;

                if (token == "{")
                {
                    idDict dict = new idDict();
                    BspStringToEntityDict(ref parser, ref dict);
                    dict.WriteToStream(ref writer);
                    numEntities++;
                }
                else
                {
                    throw new Exception("SpawnEntitiesFromBsp: Unknown or unepxected token " + token + "\n");
                }
            }

            writer.BaseStream.Position = 0;
            writer.Write(numEntities);

            parser.Dispose();
            writer.Dispose();
        }
        //
        // GenerateLightmaps
        //
        private void GenerateLightmaps(ContentProcessorContext context)
        {
            // Convert the lightmap to dxt compression.
            for (int i = 0; i < lightmaps.Length; i++)
            {
                var bitmapContent = new PixelBitmapContent<Color>(LIGHTMAP_SIZE, LIGHTMAP_SIZE);
                lightmaps[i].tex2D = new Texture2DContent();

                bitmapContent.SetPixelData(lightmaps[i].image);

                lightmaps[i].tex2D.Mipmaps.Add(bitmapContent);
                lightmaps[i].tex2D.ConvertBitmapType(typeof(Dxt5BitmapContent));

                lightmaps[i].dxtimage = lightmaps[i].tex2D.Faces[0][0].GetPixelData();
                if (lightmaps[i].dxtimage == null)
                {
                    throw new Exception("Failed to DXt3 compress lightmap.\n");
                }
            }
        }

        /*
        ===============
        ColorShiftLightingBytes

        ===============
        */
        public static void ColorShiftLightingBytes(byte[] inrgb, int inpos, int colorpos, ref byte[] color)
        {
            int shift, r, g, b;

            // shift the color data based on overbright range
            shift = 2; // - tr.overbrightBits;

            // shift the data based on overbright range
            r = inrgb[inpos + 0] << shift;
            g = inrgb[inpos + 1] << shift;
            b = inrgb[inpos + 2] << shift;

            // normalize by color instead of saturating to white
            if ((r | g | b) > 255)
            {
                int max;

                max = r > g ? r : g;
                max = max > b ? max : b;
                r = r * 255 / max;
                g = g * 255 / max;
                b = b * 255 / max;
            }

            color[colorpos + 0] = (byte)r;
            color[colorpos + 1] = (byte)g;
            color[colorpos + 2] = (byte)b;
            color[colorpos + 3] = 255;
        }

        //
        // LoadLightmap
        //
        public const int LIGHTMAP_SIZE = 128;
        public const int LIGHTMAP_LUMP_SIZE = LIGHTMAP_SIZE * LIGHTMAP_SIZE * 3;
        private void LoadLightmaps( ref BinaryReader reader, int lumplen)
        {
            int count = 0;

            count = lumplen / LIGHTMAP_LUMP_SIZE;

            lightmaps = new idLightmap[count];

            if (count == 1)
            {
                //FIXME: HACK: maps with only one lightmap turn up fullbright for some reason.
                //this avoids this, but isn't the correct solution.
                count++;
            }

            for (int i = 0; i < count; i++)
            {
                // expand the 24 bit on-disk to 32 bit
                byte[] buf_p = reader.ReadBytes(LIGHTMAP_LUMP_SIZE);

                lightmaps[i].image = new byte[LIGHTMAP_SIZE * LIGHTMAP_SIZE * 4];

                for (int c = 0, d = 0; c < LIGHTMAP_SIZE * LIGHTMAP_SIZE * 3; c += 3, d+=4)
                {
                    ColorShiftLightingBytes(buf_p, c, d, ref lightmaps[i].image);
                }
            }
        }

        public const int BSP_IDENT = (('P' << 24) + ('S' << 16) + ('B' << 8) + 'I');
        public const int BSPCOMPILED_IDENT = (('P' << 24) + ('S' << 16) + ('B' << 8) + 'J');
        public const int BSP_VERSION = 47;  // updated (9/12/2001) to sync everything up pre-beta

        public const int LUMP_ENTITIES = 0;
        public const int LUMP_SHADERS = 1;
        public const int LUMP_PLANES = 2;
        public const int LUMP_NODES = 3;
        public const int LUMP_LEAFS = 4;
        public const int LUMP_LEAFSURFACES = 5;
        public const int LUMP_LEAFBRUSHES = 6;
        public const int LUMP_MODELS = 7;
        public const int LUMP_BRUSHES = 8;
        public const int LUMP_BRUSHSIDES = 9;
        public const int LUMP_DRAWVERTS = 10;
        public const int LUMP_DRAWINDEXES = 11;
        public const int LUMP_FOGS = 12;
        public const int LUMP_SURFACES = 13;
        public const int LUMP_LIGHTMAPS = 14;
        public const int LUMP_LIGHTGRID = 15;
        public const int LUMP_VISIBILITY = 16;
        public const int HEADER_LUMPS = 17;

        //
        // lump_t
        //
        public struct lump_t
        {
            public int fileofs, filelen;
            public byte[] buffer; // the entire lump stored in a byte[] array.
        };

        //
        // idLightmap
        //
        struct idLightmap
        {
            public byte[] image;
            public byte[] dxtimage;
            public Texture2DContent tex2D;
        }

        //
        // idMapHeader
        //
        public struct idMapHeader
        {
            public int ident;
            public int version;

            public lump_t[] lumps;
        }
    }
}
