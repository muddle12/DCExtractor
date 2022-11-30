namespace Custom.Data
{
    /// <summary>
    /// Class   :   "Endian"
    /// 
    /// Purpose :   provides functions that modify the endianess of a data type.
    /// 
    /// For the record, there are very few instances of big endian in the format. The main culprit is the magic values at the beginning of the DC format headers.
    /// You probably don't even need this, but it's a product of an earlier time.
    /// </summary>
    public static class Endian
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Converts the value from one endianess to another.
        /// </summary>
        /// <param name="val">The value to convert.</param>
        /// <returns>The other endian equivalent of the value.</returns>
        public static uint Swap(uint val)
        {
            return (val & 0xFF000000) >> (6 * 4) |
                (val & 0x00FF0000) >> (2 * 4) |
                (val & 0x0000FF00) << (2 * 4) |
                (val & 0x000000FF) << (6 * 4);
        }

        /// <summary>
        /// Converts the value from one endianess to another.
        /// </summary>
        /// <param name="val">The value to convert.</param>
        /// <returns>The other endian equivalent of the value.</returns>
        public static int Swap(int val)
        {
            unchecked
            {
                return ((val & (int)0xFF000000) >> (6 * 4)) |
                    ((val & (int)0x00FF0000) >> (2 * 4)) |
                    ((val & (int)0x0000FF00) << (2 * 4)) |
                    ((val & (int)0x000000FF) << (6 * 4));
            }
        }
    }
}
