// texture_importer.cs (c) 2010 JV Software
//

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using DevIL;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

using Color = Microsoft.Xna.Framework.Color;
using idRectangle = System.Drawing.Rectangle;

namespace idLib.Engine.Content.Texture
{
    //
    // idTexture2DContent
    //
    public struct idTexture2DContent
    {
        public Texture2DContent texture;
    }

    //
    // http://theinstructionlimit.com/?p=195
    //
    [ContentImporter(".bmp", ".dds", ".dib", ".hdr", ".jpg", ".pfm", ".png", ".ppm", ".tga", DisplayName = "RTCW Texture Importer", DefaultProcessor = "SpriteTextureProcessor")]
    public class idTextureImporter : ContentImporter<idTexture2DContent>
    {
        //
        // BitmapToByteArray
        //
        private unsafe void BitmapToByteArray(ref Bitmap image, out byte[] buffer)
        {
            var bitmapData = image.LockBits(new idRectangle(0, 0, image.Width, image.Height),
                                             ImageLockMode.ReadOnly, image.PixelFormat);

            byte *ptr = (byte *)bitmapData.Scan0.ToPointer();
            int byteCount = bitmapData.Stride * image.Height;
            
            buffer = new byte[byteCount];

            //Marshal.Copy(bitmapData.Scan0, buffer, 0, byteCount);

            for (int i = 0; i < byteCount; i+=4)
            {
                buffer[i + 2] = *ptr++;
                buffer[i + 1] = *ptr++;
                buffer[i + 0] = *ptr++;
                buffer[i + 3] = *ptr++;
            }

            image.UnlockBits(bitmapData);
        }

        private bool ThumbnailCallback()
        {
            return true;
        }

        //
        // AddMipmapToContent
        //
        private void AddMipmapToContent(Bitmap image, ref idTexture2DContent content, byte[] image_buffer)
        {
            var bitmapContent = new PixelBitmapContent<Color>(image.Width, image.Height);

            // Set the pixel data to the content holder.
            bitmapContent.SetPixelData(image_buffer);

            // Add the color data to the next mipmap level.
            content.texture.Mipmaps.Add(bitmapContent);

            // Convert the pixel data to DXT compression.
            content.texture.ConvertBitmapType(typeof(Dxt5BitmapContent));
        }

        public override idTexture2DContent Import(string filename, ContentImporterContext context)
        {
            Image img;
            idTexture2DContent content;

            byte[] image_buffer;
            int scaled_width, scaled_height;

            // Load in the bitmap.
            Bitmap image = DevIL.DevIL.LoadBitmap(filename);

            //
            // convert to exact power of 2 sizes
            //
            for (scaled_width = 1; scaled_width < image.Width; scaled_width <<= 1)
                ;
            for (scaled_height = 1; scaled_height < image.Height; scaled_height <<= 1)
                ;

            // If we are building a windows phone build, scale the images by half there size.
            // This is a hack I know this should probably be in the content processor but its faster just to put it here.
            if (context.OutputDirectory.Contains("Windows Phone") && filename.Contains("ui") == false)
            {
                int scale = 0;
                int phoneTextureSize = 128;
                // Half the texture size for the phone.
                //scaled_width = scaled_width / 2;
                //scaled_height = scaled_height / 2;

                for (scale = 1; scale < phoneTextureSize; scale <<= 1)
                {
                    ;
                }

                while (scaled_width > scale || scaled_height > scale)
                {
                    scaled_width >>= 1;
                    scaled_height >>= 1;
                }
            }

            img = image.GetThumbnailImage(scaled_width, scaled_height, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);

            image.Dispose();
            image = new Bitmap(img);

            // Convert the bitmap to a bytearray.
            BitmapToByteArray(ref image, out image_buffer);

            // Create the new idTexture2DContent.
            content.texture = new Texture2DContent();

            // Add the buffer to the first mipmap level.
            AddMipmapToContent(image, ref content, image_buffer);

            // Dispose of the image.
            image.Dispose();

            return content;
        }
    }
}
