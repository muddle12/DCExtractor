using Custom.Data;
using Custom.IO;
using DC.Types;
using System.IO;
using static DC.Types.TIM2Image;

namespace DC.IO
{
    /// <summary>
    /// Class   :   "TM2"
    /// 
    /// Purpose :   provides functions for loading and converting TM2 image files. TIM2 is a standard image format used
    /// by the PS2. It's relatively better documented than the other formats found in the Level-5 data, but there is
    /// still a bit of guesswork.
    /// 
    /// Reference:
    /// https://openkh.dev/common/tm2.html
    /// https://wiki.xentax.com/index.php/TM2_TIM2_Image
    /// </summary>
    public static class TM2
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Constants.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        const int TIM2Magic = 0x54494D32;


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Loads a TIM2 from file.
        /// </summary>
        /// <param name="szFilePath">The path to load the image from.</param>
        /// <returns>The TIM2 image file.</returns>
        public static TIM2Image Load(string szFilePath)
        {
            //Setup our progress bar.
            DCProgress.value = 0;
            DCProgress.maximum = 1;
            DCProgress.name = "Loading TM2";

            //Load the image.
            TIM2Image tImage = LoadTIM2(szFilePath);

            //Report the finish.
            DCProgress.value = DCProgress.maximum;
            DCProgress.name = "Loading TM2 - FINISHED";

            //Return the result.
            return tImage;
        }

        /// <summary>
        /// Reads a TIM2Image from the input stream.
        /// </summary>
        /// <param name="tReader">The input stream.</param>
        /// <param name="startOffset">The starting offset to begin reading from.</param>
        /// <returns></returns>
        public static TIM2Image ReadTIM2Image(this BinaryReader tReader, long startOffset)
        {
            //Since it's possible the TIM2 isn't at the beginning of the file, we need to move to the offset where the TIM2 is kept before reading.
            tReader.BaseStream.Position = startOffset;

            //Check the header to see if it is a TIM2 file.
            TIM2Header tHeader = tReader.ReadTIM2Header();
            if (tHeader.magic != TIM2Magic)
            {
                //DO NOT CLOSE THE INPUT STREAM, it's not ours to close.
                //tReader.Close();
                return null;
            }

            //Create a new TIM2Image to store our data.
            TIM2Image tImage = new TIM2Image();
            tImage.header = tHeader;
            tImage.pictures = new TIM2Picture[tImage.header.pictureCount];

            //Loop through each of the pictures and load them in.
            for (int picture = 0; picture < tImage.header.pictureCount; picture++)
                tImage.pictures[picture] = tReader.ReadTIM2Picture();

            //DO NOT CLOSE THE INPUT STREAM, it's not ours to close.
            //tReader.Close();

            //We're done, return the result.
            return tImage;
        }

        /// <summary>
        /// Loads a directory of TIM2 files.
        /// </summary>
        /// <param name="szFolderPath">The path of the directory to load.</param>
        /// <returns>A list of TIM2Images.</returns>
        public static TIM2Image[] LoadDirectory(string szFolderPath)
        {
            //Setup our progress bar.
            DCProgress.value = 0;
            DCProgress.maximum = 1;
            DCProgress.name = "Loading TM2";

            //Get a list of tm2 files to load.
            string[] szFiles = Directory.GetFiles(szFolderPath, "*.tm2", SearchOption.AllDirectories);

            //Set the progress max to the file count.
            DCProgress.maximum = szFiles.Length;

            //Loop through and load them all.
            TIM2Image[] tImages = new TIM2Image[szFiles.Length];
            for (int i = 0; i < szFiles.Length; i++)
            {
                DCProgress.name = "Loading " + Path.GetFileName(szFiles[i]);

                //Attempt to load the image.
                tImages[i] = LoadTIM2(szFiles[i]);

                //Update our progress.
                DCProgress.value = i;

                //If canceled, we need to bail.
                if (DCProgress.canceled)
                    return new Types.TIM2Image[0];
            }

            //Report the finish.
            DCProgress.value = DCProgress.maximum;
            DCProgress.name = "Loading TM2 Directory - FINISHED";

            //Return the result.
            return tImages;
        }

        /// <summary>
        /// Loads a TIM2 from file.
        /// </summary>
        /// <param name="szFilePath">The path to load the image from.</param>
        /// <returns>The TIM2 image file.</returns>
        static TIM2Image LoadTIM2(string szFilePath)
        {
            //Open a file stream.
            BinaryReader tReader = FileStreamHelpers.OpenBinaryReader(szFilePath);
            if (tReader == null || tReader.BaseStream.Length == 0)
                return null;

            //Read the TIM2Image.
            TIM2Image tImage = tReader.ReadTIM2Image(0);

            //If the image was valid.
            if (tImage != null)
            {
                //Update our filepath for saving to png later.
                tImage.filePath = szFilePath;
            }

            //Close the input stream.
            tReader.Close();

            //Return the result.
            return tImage;
        }

        /// <summary>
        /// Reads a tim2 header.
        /// </summary>
        /// <param name="tReader">The input stream.</param>
        /// <param name="nStartingOffset">The starting offset of the header.</param>
        /// <returns>The header that was loaded.</returns>
        static TIM2Header ReadTIM2Header(this BinaryReader tReader)
        {
            TIM2Header tHeader = new TIM2Header();

            //Read in the header information.
            tHeader.magic = Endian.Swap(tReader.ReadInt32());
            tHeader.version = tReader.ReadByte();
            tHeader.format = tReader.ReadByte();
            tHeader.pictureCount = tReader.ReadInt16();
            tReader.BaseStream.Position += 8;//Skip 8 bytes? These appear to be unused.

            return tHeader;
        }

        /// <summary>
        /// Reads in tim2 picture data.
        /// </summary>
        /// <param name="tReader">The input stream.</param>
        /// <returns>A tim2 picture.</returns>
        static TIM2Picture ReadTIM2Picture(this BinaryReader tReader)
        {
            TIM2Picture tTIM = new TIM2Picture();

            //Read in the chunk information.
            tTIM.totalSize = tReader.ReadInt32();
            tTIM.paletteSize = tReader.ReadInt32();
            tTIM.imageDataSize = tReader.ReadInt32();

            tTIM.headerSize = tReader.ReadInt16();
            tTIM.colorEntries = tReader.ReadInt16();
            tTIM.imageFormat = tReader.ReadByte();
            tTIM.mipmapCount = tReader.ReadByte();
            tTIM.CLUTFormat = tReader.ReadByte();

            //Read the byte flag and set the bits per pixel.
            byte bDepth = tReader.ReadByte();
            switch (bDepth)
            {
                case 1:
                    tTIM.bitsPerPixel = 16;
                    break;
                case 2:
                    tTIM.bitsPerPixel = 24;
                    break;
                case 3:
                    tTIM.bitsPerPixel = 32;
                    break;
                case 4:
                    tTIM.bitsPerPixel = 4;
                    break;
                case 5:
                    tTIM.bitsPerPixel = 8;
                    break;
            }

            //Read in the image dimensions.
            tTIM.imageWidth = tReader.ReadInt16();
            tTIM.imageHeight = tReader.ReadInt16();

            //This is some PS2 specific information we won't really need.
            tTIM.gsTEX0 = tReader.ReadInt64();
            tTIM.gsTEX1 = tReader.ReadInt64();
            tTIM.gsRegs = tReader.ReadInt32();
            tTIM.gsTexClut = tReader.ReadInt32();

            //Reserved data used by the game developer.
            tTIM.userDataSize = tTIM.headerSize - 0x30;
            if (tTIM.userDataSize > 0)
                tTIM.userData = tReader.ReadBytes(tTIM.userDataSize);

            //Information for parsing the palette and image byte data.
            tTIM.isLinearPalette = (tTIM.CLUTFormat & 0x80) != 0;
            tTIM.CLUTFormat &= 0x7F;
            tTIM.colorSize = ((tTIM.bitsPerPixel > 8) ? (tTIM.bitsPerPixel / 8) : ((tTIM.CLUTFormat & 0x07) + 1));

            //Make sure our image matches the inputs.
            if (tTIM.imageDataSize == tTIM.imageWidth * tTIM.imageHeight)
            {
                //Read in the byte sections for image data and palette.
                tTIM.imageBytes = tReader.ReadBytes(tTIM.imageDataSize);
                tTIM.paletteBytes = tReader.ReadBytes(tTIM.paletteSize);
            }

            return tTIM;
        }
    }
}