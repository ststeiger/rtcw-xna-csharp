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
    // http://theinstructionlimit.com/?p=195
    //
    [ContentImporter(".bmp", ".dds", ".dib", ".hdr", ".jpg", ".pfm", ".png", ".ppm", ".tga", DisplayName = "RTCW Texture Importer", DefaultProcessor = "SpriteTextureProcessor")]
    public class idTextureImporter : ContentImporter<Texture2DContent>
    {
        //
        // BitmapToByteArray
        //
        private unsafe void BitmapToByteArray(ref Bitmap image, out byte[] buffer)
        {
            var bitmapData = image.LockBits(new idRectangle(0, 0, image.Width, image.Height),
                                             ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

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
        private void AddMipmapToContent(int width, int height, ref Texture2DContent content, byte[] image_buffer)
        {
            var bitmapContent = new PixelBitmapContent<Color>(width, height);
            bitmapContent.SetPixelData(image_buffer);
            content.Mipmaps.Add(bitmapContent);
        }

        public override Texture2DContent Import(string filename, ContentImporterContext context)
        {
            Texture2DContent content;
            byte[] image_buffer;

            // Load in the bitmap.
            Bitmap image = DevIL.DevIL.LoadBitmap(filename);

            // If we are building a windows phone build, scale the images by half there size.
            // This is a hack I know this should probably be in the content processor but its faster just to put it here.
            if (context.OutputDirectory.Contains("Windows Phone"))
            {
                Image img = image.GetThumbnailImage(image.Width / 2, image.Height / 2, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);

                image.Dispose();
                image = new Bitmap(img);
            }

            // Convert the bitmap to a bytearray.
            BitmapToByteArray(ref image, out image_buffer);

            // Create the new texture2dcontent.
            content = new Texture2DContent();

            // Add the buffer to the first mipmap level.
            AddMipmapToContent(image.Width, image.Height, ref content, image_buffer);

            // Dispose of the image.
            image.Dispose();

            return content;
        }
    }
}
