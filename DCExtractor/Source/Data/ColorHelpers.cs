using System.Drawing;

namespace Custom.Data
{
    /// <summary>
    /// Class   :   "ColorHelpers"
    /// 
    /// Purpose :   provides additional color functions.
    /// This class contains functions for unpacking formats into System.Drawing.Color.
    /// </summary>
    public static class ColorHelpers
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Converts a two byte ABGR1555 format color into a ARBG color.
        /// </summary>
        /// <param name="tBytes">The byte array.</param>
        /// <param name="nOffset">The offset in the array where the color starts.</param>
        /// <returns>An ARGB color.</returns>
        public static Color FromABGR1555(byte[] tBytes, int nOffset)
        {
            //For reference: 2 bytes: A BBBBB GGGGG RRRRR

            //Rip the bits from the 2 byte color and convert them to ABGR bytes.
            int a = ((tBytes[nOffset] & 0x80) >> 7);
            int b = ((tBytes[nOffset] & 0x7C) >> 2);
            int g = (((tBytes[nOffset] & 0x03) << 3) | ((tBytes[nOffset + 1] & 0xE0) >> 5));
            int r = tBytes[nOffset] & 0x1F;

            //Multiply those bytes to get them into 255 color space.
            const float conv = 1.0f / 31.0f;
            float bf = ((float)b * conv) * 255.0f;
            float gf = ((float)g * conv) * 255.0f;
            float rf = ((float)r * conv) * 255.0f;

            //return the converted color.
            return Color.FromArgb(a * 255, (int)rf, (int)gf, (int)bf);
        }

        /// <summary>
        /// Converts the buffer of ABGR1555 colors to a Color array.
        /// </summary>
        /// <param name="buffer">The buffer to convert.</param>
        /// <param name="size">The size of the buffer.</param>
        /// <returns>An array of ARGB colors.</returns>
        public static Color[] FromBufferABGR1555(byte[] buffer, int size)
        {
            //For reference: 2 bytes: A BBBBB GGGGG RRRRR

            //Convert the array using the AGBR1555 function.
            const int stride = 2;
            Color[] tColors = new Color[size / stride];
            for (int p = 0, c = 0; p < size; p += stride, c++)
                tColors[c] = ColorHelpers.FromABGR1555(buffer, p);
            return tColors;
        }

        /// <summary>
        /// Converts the buffer of RGB24Bit colors to a Color array.
        /// </summary>
        /// <param name="buffer">The buffer to convert.</param>
        /// <param name="size">The size of the buffer.</param>
        /// <returns>An array of ARGB colors.</returns>
        public static Color[] FromBufferRGB24Bit(byte[] buffer, int size)
        {
            //For reference: 3 bytes: RRRRRRRR GGGGGGGG BBBBBBBB

            const int stride = 3;
            Color[] tColors = new Color[size / stride];
            for (int p = 0, c = 0; p < size; p += stride, c++)
                tColors[c] = Color.FromArgb(255, buffer[p], buffer[p + 1], buffer[p + 2]);
            return tColors;
        }

        /// <summary>
        /// Converts the buffer of RGBA32Bit colors to a Color array.
        /// </summary>
        /// <param name="buffer">The buffer to convert.</param>
        /// <param name="size">The size of the buffer.</param>
        /// <returns>An array of ARGB colors.</returns>
        public static Color[] FromBufferRGBA32Bit(byte[] buffer, int size)
        {
            //HACK  :   for whatever reason, the alpha values coming in from virtually all the textures in DC2 are at exactly half alpha intensity(128). 
            //  There might be some proprietary or ps2 related reason for this, but I can't find it anywhere on the web. Oh well, hopefully this doesn't break something.
            const int BIT_ALPHA_32_HACK = 2;

            //For reference: 4 bytes: RRRRRRRR GGGGGGGG BBBBBBBB AAAAAAAA
            const int stride = 4;
            Color[] tColors = new Color[size / stride];
            for (int p = 0, c = 0; p < size; p += stride, c++)
            {
                int nAlpha = (int)buffer[p + 3] * BIT_ALPHA_32_HACK;
                if (nAlpha > 255)
                    nAlpha = 255;
                if (nAlpha < 0)
                    nAlpha = 0;
                tColors[c] = Color.FromArgb(nAlpha, buffer[p], buffer[p + 1], buffer[p + 2]);
            }
            return tColors;
        }
    }
}