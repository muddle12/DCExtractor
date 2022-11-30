namespace Custom.Data
{
    /// <summary>
    /// Class   :   "Swizzle"
    /// 
    /// Purpose :   provides functions for unswizzling bytes.
    /// Reference:  https://gist.github.com/Fireboyd78/1546f5c86ebce52ce05e7837c697dc72
    /// 
    /// The short explanation: the bytes inside a TM2 image are formatted so it's more efficient for the CPU/GPU on the PS2. 
    /// Swizzling greatly speeds up load and draw times, at the cost of making them not easily editable/readable. Different systems use different swizzles.
    /// This class unjumbles the bytes into something readable by image editors.
    /// 
    /// If this section confuses you, don't worry! It confused me too. If you don't know what any of this means, I'd recommend brushing up on it,
    /// or avoiding this class all together. I'm not going to comment these sections, as it's largely knowledge related to the PS2 byte swizzle,
    /// and it isn't particularly helpful to know the ins and outs of how this works unless you're writing your own importer.
    /// </summary>
    public static class Swizzle
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Constants.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static readonly byte[] InterlaceMatrix = {
                0x00, 0x10, 0x02, 0x12,
                0x11, 0x01, 0x13, 0x03,
            };

        static readonly int[] MatrixY = { 0, 1, -1, 0 };
        static readonly int[] TileMatrix = { 4, -4 };


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Unpacks the 4-bit byte stream into a 8-bit byte stream.
        /// </summary>
        /// <param name="buffer">The buffer to unpack.</param>
        /// <param name="width">The width of the buffer.</param>
        /// <param name="height">The height of the buffer.</param>
        /// <returns>A 8-bit byte stream.</returns>
        static byte[] Unpack4Bit(byte[] buffer, int width, int height)
        {
            int d = 0;
            int s = 0;
            byte[] pixels = new byte[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < (width >> 1); x++)
                {
                    byte p = buffer[s++];

                    pixels[d++] = (byte)(p & 0xF);
                    pixels[d++] = (byte)(p >> 4);
                }
            }
            return pixels;
        }

        /// <summary>
        /// Packs the 8-bit byte stream into a 4-bit byte stream.
        /// </summary>
        /// <param name="buffer">The buffer to pack.</param>
        /// <param name="width">The width of the buffer.</param>
        /// <param name="height">The height of the buffer.</param>
        /// <returns>A 4-bit byte stream.</returns>
        static byte[] Pack4Bit(byte[] buffer, int width, int height)
        {
            int s = 0;
            int d = 0;
            byte[] result = new byte[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < (width >> 1); x++)
                    result[d++] = (byte)((buffer[s++] & 0xF) | (buffer[s++] << 4));
            }
            return result;
        }

        /// <summary>
        /// Unswizzles a 4-bit byte stream.
        /// </summary>
        /// <param name="buffer">The buffer to unswizzle.</param>
        /// <param name="width">The width of the buffer.</param>
        /// <param name="height">The height of the buffer.</param>
        /// <returns>The unswizzled 4-bit byte stream.</returns>
        public static byte[] UnSwizzle4Bit(byte[] buffer, int width, int height)
        {
            //Unpack from 4-bit to 8-bit bytes.
            byte[] bUnpacked = Unpack4Bit(buffer, width, height);
            //Unswizzle the 8-bit bytes.
            byte[] bUnswizzled = UnSwizzle(bUnpacked, width, height);
            //Repack from 8-bit to 4-bit bytes.
            return Pack4Bit(bUnswizzled, width, height);
        }

        /// <summary>
        /// Unswizzles the byte stream passed.
        /// </summary>
        /// <param name="buffer">The buffer to unswizzle.</param>
        /// <param name="width">The width of the buffer.</param>
        /// <param name="height">The height of the buffer.</param>
        /// <returns>The unswizzled byte stream.</returns>
        public static byte[] UnSwizzle(byte[] buffer, int width, int height)
        {
            bool bIsOdd = false;
            int nYOffset, nBufferIndex = 0, nPixelIndex, nInterlaceIndex;
            int nXIndex;
            int nYIndex;
            byte[] newPixels = new byte[width * height];
            for (int y = 0; y < height; y++)
            {
                //Only need to calculate these values once per loop.
                bIsOdd = (y & 1) != 0;
                nYOffset = y * width;
                if (bIsOdd)
                    nYOffset -= width;
                nYIndex = y + MatrixY[y % 4];

                for (int x = 0; x < width; x++)
                {
                    nInterlaceIndex = ((x >> 2) & 3);
                    if (bIsOdd)
                        nInterlaceIndex += 4;

                    //Just calculating the next byte swizzle position.
                    nXIndex = x + ((y >> 2) & 1) * TileMatrix[((x >> 2) & 1)];

                    //Byte unswizzles use an interlace matrix to move the bytes into their swizzle positions(or back out of them).
                    nBufferIndex = InterlaceMatrix[nInterlaceIndex] + ((x << 2) & 0x0F) + ((x >> 4) << 5) + nYOffset;
                    nPixelIndex = nYIndex * width + nXIndex;

                    /**************************************************************************************/
                    //  So, this is a weird edge case with image loading. 
                    //  Apparently a very small handful of dark cloud 2's images are not powers of 2. 
                    //  Unswizzle fails because the image is not a power of two.
                    //  Naturally, this causes an exception when we index out of range on the next line.
                    //  I've put in this little hack to gracefully bail out when this happens. I suppose
                    //  there "might" be data loss, but based on what I've seen it's a single horizontal
                    //  line, and it's not that we're losing anything(we're at the end of the source buffer)
                    //  rather just quitting the algorithm.
                    //
                    //  If you want to test this, uncomment this line and attempt to convert "f0401.img",
                    //  The png "f0401_06.png" will IndexOutOfRange because the image is 256x255.
                    /**************************************************************************************/
                    if (nBufferIndex >= buffer.Length)
                        return newPixels;//I don't like bailing on the function, but I can't let it crash here.

                    newPixels[nPixelIndex] = buffer[nBufferIndex];
                }
            }
            return newPixels;
        }

        #region OldVersionForReference
        //public static byte[] UnSwizzleOld(byte[] buffer, int width, int height)
        //{
        //    int a, b, c, d, e, f, g, i, j;
        //    int xx;
        //    int yy;
        //    byte[] newPixels = new byte[width * height];
        //    for (int y = 0; y < height; y++)
        //    {
        //        for (int x = 0; x < width; x++)
        //        {
        //            bool oddRow = ((y & 1) != 0);

        //            a = (byte)((y / 4) & 1);
        //            b = (byte)((x / 4) & 1);
        //            c = (y % 4);

        //            d = ((x / 4) % 4);
        //            if (oddRow)
        //                d += 4;

        //            e = ((x * 4) % 16);
        //            f = ((x / 16) * 32);

        //            g = (oddRow) ? ((y - 1) * width) : (y * width);

        //            xx = x + a * TileMatrix[b];
        //            yy = y + Matrix[c];

        //            i = InterlaceMatrix[d] + e + f + g;
        //            j = yy * width + xx;

        //            newPixels[j] = buffer[i];
        //        }
        //    }
        //    return newPixels;
        //}
        #endregion
    }
}