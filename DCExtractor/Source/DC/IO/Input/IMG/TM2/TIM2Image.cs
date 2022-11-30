namespace DC.Types
{
    /// <summary>
    /// Class   :   "TIM2Image"
    /// 
    /// Purpose :   defines a TIM2 PS2 image texture. TIM2 is the standard texture format used by the PS2. It's usually represented with the .tm2 extension.
    /// 
    /// Image compression and storage is generally a complicated and sometimes confusing subject. I'll do my best to document what I've learned about the
    /// load process based on what I've uncovered. A lot of this information was read on repos and wikis related to the subject.
    /// 
    /// https://openkh.dev/common/tm2.html
    /// https://wiki.xentax.com/index.php/TM2_TIM2_Image
    /// </summary>
    public class TIM2Image
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Constants.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public const int TIM2Magic = 0x54494D32;


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Enumerations.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// I ended up not using this. It describes the different format types.
        /// </summary>
        enum PaletteType : byte { Bit32 = 0, Bit24 = 1, Bit16 = 2, Bit16s = 10 }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Classes.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public class TIM2Header
        {
            public int magic;                   //The magic number that denotes the file type, TIM2.
            public byte version;                //The version of this TIM image.
            public byte format;                 //Usually 0.
            public short pictureCount;          //The number of pictures stored in this TIM2.
            //public int unused1;
            //public int unused2;
        };


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Data Members.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public TIM2Header header;                   //The file header information.
        public TIM2Picture[] pictures;              //The list of pictures in this image.
        public string filePath;                     //A convention of DCExtractor. The file path this image was loaded from, calculated by the load function. Used for conversions.
    }
}
