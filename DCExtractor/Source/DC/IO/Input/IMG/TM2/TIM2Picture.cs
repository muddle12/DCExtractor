using Custom.Data;
using System.Drawing;

namespace DC.Types
{
    /// <summary>
    /// Class   :   "TIM2Picture"
    /// 
    /// Purpose :   an picture held by a TIM2Image.
    /// 
    /// https://openkh.dev/common/tm2.html
    /// https://wiki.xentax.com/index.php/TM2_TIM2_Image
    /// </summary>
    public class TIM2Picture
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Data Members.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public int totalSize;                       //The total size of the image AFTER it's been unpacked.
        public int paletteSize;                     //The size of the palette section in bytes.
        public int imageDataSize;                   //The size of the image section in bytes.
        public short headerSize;                    //The size of this header.
        public short colorEntries;                  //The number of color entries in the image.
        public byte imageFormat;                    //The image format, presumably for the PS2 graphics engine.
        public byte mipmapCount;                    //The number of mipmaps in this image.
        public byte CLUTFormat;                     //Irrelevant data.
        public byte bitsPerPixel;                   //The number of bits per pixel in the image buffer. These are generally multiples of 4.
        public short imageWidth;                    //The width of this image.
        public short imageHeight;                   //The height of this image.
        public long gsTEX0;                         //Irrelevant data for us. If I had to guess, it's a render buffer target address on the PS2 graphics engine. 
        public long gsTEX1;                         //Irrelevant data for us. If I had to guess, it's a render buffer target address on the PS2 graphics engine. 
        public int gsRegs;                          //More irrelevant data.
        public int gsTexClut;                       //More irrelevant data.
        public byte[] userData = new byte[0];       //More irrelevant data.
        public byte[] imageBytes = new byte[0];     //The buffer of image data in an uncompressed/compressed/paletted/swizzled format that needs to be converted.
        public byte[] paletteBytes = new byte[0];   //The buffer of palette pixels used for indexing the image bytes.

        //calculated data.
        public int userDataSize;                    //The size of the user data section.
        public bool isLinearPalette = false;        //Refers to whether or not the palette data is linear or interleaved/interlaced.
        public int colorSize = 0;                   //How many bits in size an individual entry in the imageBytes is.
        public int paletteStride = 1;               //The stride of the palette. Unused at this point.


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets the palette information as a System.Drawing.Color array. This operation is non-trivial, please cache the result.
        /// </summary>
        /// <returns>An array of Colors.</returns>
        public Color[] GetPalette()
        {
            //If there is no palette, return an empty palette.
            if (paletteBytes == null || paletteBytes.Length == 0)
                return new Color[0];

            //Otherwise, we're going to have to figure out which palette we're using based on the paletteColorSize.
            Color[] tPalette = new Color[0];
            if (bitsPerPixel <= 8)
            {
                //We'll infer which palette type we're using based on the size of each individual palette entry.
                switch (colorSize)
                {
                    case 2://16 BIT LE_ABGR_1555?
                        {
                            tPalette = ColorHelpers.FromBufferABGR1555(paletteBytes, paletteSize);
                        }
                        break;
                    case 3://24 BIT RGB?
                        {
                            tPalette = ColorHelpers.FromBufferRGB24Bit(paletteBytes, paletteSize);
                        }
                        break;
                    case 4://32 BIT RGBA?
                        {
                            tPalette = ColorHelpers.FromBufferRGBA32Bit(paletteBytes, paletteSize);
                        }
                        break;
                }

                //If the palette is interleaved, we need to unpack that into something linear.
                if (isLinearPalette == false)
                {
                    //The TIM2 palettes always have these fixed values.
                    const int nBlockCount = 2;
                    const int nStripeCount = 2;
                    const int nColorCount = 8;

                    //Some calculations for unpacking the palette.
                    int nPartCount = tPalette.Length / 32;
                    int nPartStride = nColorCount * nStripeCount * nBlockCount;
                    int nStripeStride = nStripeCount * nColorCount;

                    //This next part was ripped wholesale form the above references. I couldn't tell you exactly what's going on, but it appears to work.
                    //If I had to guess, TIM2 has a very particular kind of interleave using blocks instead of straight stripes.
                    int i = 0;
                    Color[] tUnpackedColors = new Color[tPalette.Length];
                    for (int part = 0; part < nPartCount; part++)
                    {
                        for (int block = 0; block < nBlockCount; block++)
                        {
                            for (int stripe = 0; stripe < nStripeCount; stripe++)
                            {
                                for (int color = 0; color < nColorCount; color++)
                                {
                                    tUnpackedColors[i++] = tPalette[(part * nPartStride) + (block * nColorCount) + (stripe * nStripeStride) + color];
                                }
                            }
                        }
                    }

                    //overwrite the original palette, as we don't need it anymore.
                    tPalette = tUnpackedColors;
                }
            }
            return tPalette;
        }

        /// <summary>
        /// Flips the entire image on the vertical axis.
        /// </summary>
        /// <param name="tInput">The array of colors to flip.</param>
        /// <returns>A new flipped color array.</returns>
        static Color[] FlipVertical(Color[] tInput, int nWidth, int nHeight)
        {
            //We're going to loop through the image and reverse the horizontal stripes on both ends.
            Color[] tOutput = new Color[tInput.Length];
            for (int y = 0; y < nHeight; y++)
            {
                for (int x = 0; x < nWidth; x++)
                {
                    //Take the input pixel from one size(top/bottom) and place it on the opposite side in the output.
                    tOutput[(nHeight - y - 1) * nWidth + x] = tInput[y * nWidth + x];
                }
            }
            return tOutput;
        }

        /// <summary>
        /// Returns the color information as an array of System.Drawing.Colors. This operation is non-trivial, please cache the result.
        /// </summary>
        /// <returns>An array of colors.</returns>
        public Color[] GetPixels()
        {
            //If there aren't any image bytes, return an empty color array.
            if (imageBytes == null || imageBytes.Length == 0)
                return new Color[0];

            byte[] tImageBytesSw = null;
            Color[] tColors = null;

            /**************************************************************************/
            //  Regardless of how the image bytes or palette is formatted, we're going
            //  to have to perform an unswizzle on the byte data to get back usable
            //  values. You can read more about this in the unswizzle class, but in
            //  simple terms the data is jumbled up to make it easier for the PS2 to use
            //  Tracking down how this unswizzle works was a pain in the ass, but it
            //  appears to work for all cases now, which is great.
            /**************************************************************************/
            //If our data is less than or equal to 8 bytes, that means it's been indexed, and uses the palette. We're going to have to unpack that.
            if (bitsPerPixel <= 8)
            {
                //First, we'll go ahead and get the palette.
                Color[] tPalette = GetPalette();

                //Next, we'll figure out how our byte data is packed.
                switch (bitsPerPixel)
                {
                    //4 bit image bytes means each byte contains 2 indices of color data.
                    case 4:
                        {
                            //Perform an unswizzle on the entire image to get out usable data we can index with.
                            tImageBytesSw = Custom.Data.Swizzle.UnSwizzle4Bit(imageBytes, imageWidth, imageHeight);

                            //Unpack the individual 4-bit sections into colors by indexing the palette with their values.
                            int nColorCount = imageDataSize * 2;
                            tColors = new Color[nColorCount];
                            for (int c = 0, b = 0; b < imageDataSize; b++)
                            {
                                tColors[c++] = tPalette[(tImageBytesSw[b] & 0xF0) >> 4];
                                tColors[c++] = tPalette[(tImageBytesSw[b] & 0x0F)];
                            }
                        }
                        break;
                    //8-bit image bytes means each color index is a single byte in size.
                    case 8:
                        {
                            //Perform an unswizzle on the entire image to get out usable data we can index with.
                            tImageBytesSw = Custom.Data.Swizzle.UnSwizzle(imageBytes, imageWidth, imageHeight);

                            //Unpack the individual indicies into colors by indexing the palette with their values.
                            tColors = new Color[imageDataSize];
                            for (int p = 0; p < imageDataSize; p++)
                                tColors[p] = tPalette[tImageBytesSw[p]];
                        }
                        break;
                }
            }
            //True Color. This means that the data is not indexed, and is simply represented by the bytes in the buffer.
            else
            {
                //Perform an unswizzle on the entire image to get out usable data we can index with.
                tImageBytesSw = Custom.Data.Swizzle.UnSwizzle(imageBytes, imageWidth, imageHeight);

                //Switch on the color size to determine what kind of color information we're dealing with.
                switch (colorSize)
                {
                    case 2://16 BIT LE_ABGR_5551
                        {
                            tColors = ColorHelpers.FromBufferABGR1555(tImageBytesSw, imageDataSize);
                        }
                        break;
                    case 3://24 BIT RGB
                        {
                            tColors = ColorHelpers.FromBufferRGB24Bit(tImageBytesSw, imageDataSize);
                        }
                        break;
                    case 4://32 BIT RGBA
                        {
                            tColors = ColorHelpers.FromBufferRGBA32Bit(tImageBytesSw, imageDataSize);
                        }
                        break;
                }
            }

            //These images are stored upside down, probably for hardware reasons. We'll flip them back for the end user.
            return FlipVertical(tColors, imageWidth, imageHeight);
        }
    }
}
