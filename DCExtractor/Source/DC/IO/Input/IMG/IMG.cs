using Custom.Data;
using Custom.IO;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace DC.IO
{
    /// <summary>
    /// Class   :   "IMG"
    /// 
    /// Purpose :   provides functions for loading Level-5 .img files. IMG files are simply packages containing 
    /// a series of TIM2 images and a simple file system.
    /// </summary>
    public static class IMG
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Constants.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        const int IM2Magic = 0x494D3200;//Dark Cloud 1 uses this. 
        const int IM3Magic = 0x494D3300;//Dark Cloud 2 uses this.


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Classes.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// A shared interface for IMG headers.
        /// </summary>
        interface IMGHeader
        {
            int imageCountValue { get; }
        }

        /// <summary>
        /// Class   :   "IM2Header"
        /// 
        /// Purpose :   the header for an IM3 image package file. This is the first header in the file denoting that it is an IMG file.
        /// </summary>
        struct IM2Header : IMGHeader
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Constants.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            public const int headerSize = 16;       //The size of this header.


            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Data Members.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            public int magic;                       //The magic numbers denoting the version of this IMG file.
            public int imageCount;                  //The number of images stored in the IMG file.
            public int unknown1;                    //Unknown, always 0
            public int unknown2;                    //Unknown, always 0


            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Properties.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Gets the value of the imageCount field.
            /// </summary>
            public int imageCountValue
            {
                get { return imageCount; }
            }
        };

        /// <summary>
        /// Class   :   "IM3Header"
        /// 
        /// Purpose :   the header for an IM3 image package file. This is the first header in the file denoting that it is an IMG file.
        /// </summary>
        struct IM3Header : IMGHeader
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Constants.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            public const int headerSize = 16;       //The size of this header.


            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Data Members.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            public int magic;                       //The magic numbers denoting the version of this IMG file.
            public int firstHeaderOffset;           //The offset to the first tim2image header in the file.
            public int imageCount;                  //The number of images stored in the IMG file.
            public int unknown1;                    //Unknown, always 0


            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Properties.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Gets the value of the imageCount field.
            /// </summary>
            public int imageCountValue
            {
                get { return imageCount; }
            }
        };

        /// <summary>
        /// A shared interface for IMGSubheaders.
        /// </summary>
        interface IMGSubHeader
        {
            string nameValue { get; }
            int imageOffsetValue { get; }
        }

        /// <summary>
        /// Class   :   "IMG2SubHeader"
        /// 
        /// Purpose :   the image header for an IM2 image package file. This is part of the IMG file system, denoting the position of each individual TIM2Image in the file.
        /// </summary>
        class IMG2SubHeader : IMGSubHeader
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Constants.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            public const int headerSize = 48;       //The size of this header.


            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Data Members.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            public string name;                     //The name of this image. Always 32 bytes long, with null terminators.
            public int imageOffset;                 //The offset into the file to find the image data.
            public int unknown1;                    //Unknown
            public int unknown2;                    //Unknown
            public int unknown3;                    //Unknown

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Properties.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Gets the name of the file for this sub header.
            /// </summary>
            public string nameValue
            {
                get { return name; }
            }

            /// <summary>
            /// Gets the value of the imageOffset field.
            /// </summary>
            public int imageOffsetValue
            {
                get { return imageOffset; }
            }
        };

        /// <summary>
        /// Class   :   "IMG3SubHeader"
        /// 
        /// Purpose :   the image header for an IM3 image package file. This is part of the IMG file system, denoting the position of each individual TIM2Image in the file.
        /// </summary>
        class IMG3SubHeader : IMGSubHeader
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Constants.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            public const int headerSize = 64;       //The size of this header.


            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Data Members.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            public string name;                     //The name of this image. Always 32 bytes long, with null terminators.

            //public int headerSize;                //The size of this ImageHeader, Always 64.
            public int imageChunkSize;              //The size of the image chunk in this tim2image.
            public int unknown5;                    //Unknown, always 1
            public int unknown6;                    //Unknown, always 0

            public int unknown7;                    //Unknown, either 0 or 1
            public int imageSize;                   //The size of the image data.
            public int unknown8;                    //Unknown, either 0 or 5
            public int unknown9;                    //Unknown, always 0

            //Calculated.
            public int imageOffset;                 //The offset into the file to find the image data.


            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Properties.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Gets the name of the file for this sub header.
            /// </summary>
            public string nameValue
            {
                get { return name; }
            }

            /// <summary>
            /// Gets the value of the imageOffset field.
            /// </summary>
            public int imageOffsetValue
            {
                get { return imageOffset; }
            }
        };


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Loads an .img file and returns the contents as TIM2Images.
        /// </summary>
        /// <param name="szIMGPath">The path to the image file.</param>
        /// <param name="tImages">The list of images that was loaded.</param>
        /// <returns>Whether or not the operation was successful.</returns>
        public static bool Load(string szIMGPath, out Types.TIM2Image[] tImages)
        {
            //Setup our progress bar.
            DCProgress.value = 0;
            DCProgress.maximum = 1;
            DCProgress.name = "Loading IMG";

            bool bLoaded = LoadIMG(szIMGPath, out tImages);

            //Report the finish.
            DCProgress.value = DCProgress.maximum;
            DCProgress.name = "Loading IMG - FINISHED";

            //Return the result.
            return bLoaded;
        }

        /// <summary>
        /// Loads a directory of .img files.
        /// </summary>
        /// <param name="szDirectory">The path of the directory to load.</param>
        /// <returns>A list of TIM2 images extracted from .img files.</returns>
        public static Types.TIM2Image[] LoadDirectory(string szDirectory)
        {
            //Setup our progress bar.
            DCProgress.value = 0;
            DCProgress.maximum = 1;
            DCProgress.name = "Loading IMG Directory";

            //Gather all of the file names of .img files in the directory.
            string[] szFiles = Directory.GetFiles(szDirectory, "*.img", SearchOption.AllDirectories);
            List<Types.TIM2Image> tImageList = new List<Types.TIM2Image>();
            Types.TIM2Image[] tImagesLoaded;

            //Set the maximum progress to the file count.
            DCProgress.maximum = szFiles.Length;

            //Loop through and attempt to load these images into a list.
            for (int i = 0; i < szFiles.Length; i++)
            {
                DCProgress.name = "Loading " + Path.GetFileName(szFiles[i]);

                //Attempt to load the image.
                if (LoadIMG(szFiles[i], out tImagesLoaded))
                    tImageList.AddRange(tImagesLoaded);

                //Update our progress.
                DCProgress.value = i;

                //If canceled, we need to bail.
                if (DCProgress.canceled)
                    return new Types.TIM2Image[0];
            }

            //Report the finish.
            DCProgress.value = DCProgress.maximum;
            DCProgress.name = "Loading IMG Directory - FINISHED";

            //Return the result.
            return tImageList.ToArray();
        }

        /// <summary>
        /// Loads an .img file and returns the contents as TIM2Images.
        /// </summary>
        /// <param name="szIMGPath">The path to the image file.</param>
        /// <param name="tImages">The list of images that was loaded.</param>
        /// <returns>Whether or not the operation was successful.</returns>
        static bool LoadIMG(string szIMGPath, out Types.TIM2Image[] tImages)
        {
            //Open a reader for the file.
            tImages = null;
            BinaryReader tReader = FileStreamHelpers.OpenBinaryReader(szIMGPath);
            if (tReader == null || tReader.BaseStream.Length == 0)
                return false;

            //We'll use some interfaces here to make loading a bit more streamlined.
            IMGHeader tHeader;
            IMGSubHeader[] tSubHeaders;

            //Read in the magic.
            int nMagic = Endian.Swap(tReader.ReadInt32());
            switch (nMagic)
            {
                //IM2 format.
                case IM2Magic:
                    {
                        //Read in the 16 byte header.
                        IM2Header tHeader2;
                        {
                            tHeader2.magic = IM2Magic;
                            tHeader2.imageCount = tReader.ReadInt32();
                            tHeader2.unknown1 = tReader.ReadInt32();
                            tHeader2.unknown2 = tReader.ReadInt32();
                        }
                        tHeader = tHeader2;

                        //Next, we're going to read in each image sub-header.
                        IMG2SubHeader[] tIMG2SubHeaders = new IMG2SubHeader[tHeader.imageCountValue];
                        {
                            //Load in all the sub headers.
                            for (int i = 0; i < tHeader.imageCountValue; i++)
                                tIMG2SubHeaders[i] = tReader.ReadIMG2SubHeader();
                        }

                        //Convert them to a generic interface so we can load more easily.
                        tSubHeaders = new IMGSubHeader[tIMG2SubHeaders.Length];
                        for (int i = 0; i < tIMG2SubHeaders.Length; i++)
                            tSubHeaders[i] = tIMG2SubHeaders[i];
                    }
                    break;
                //IM3 format.
                case IM3Magic:
                    {
                        //Read in the 16 byte header.
                        IM3Header tHeader3;
                        {
                            tHeader3.magic = IM3Magic;
                            tHeader3.firstHeaderOffset = tReader.ReadInt32();
                            tHeader3.imageCount = tReader.ReadInt32();
                            tHeader3.unknown1 = tReader.ReadInt32();
                        }
                        tHeader = tHeader3;

                        //Next, we're going to read in each image sub-header.
                        IMG3SubHeader[] tIMG3SubHeaders = new IMG3SubHeader[tHeader.imageCountValue];
                        {
                            //Image data is stored after all of the headers. We want to skip the file header, then all the individual image sub-headers.
                            int nOffset = IM3Header.headerSize + tHeader.imageCountValue * IMG3SubHeader.headerSize;
                            for (int i = 0; i < tHeader.imageCountValue; i++)
                            {
                                //Read in the image header.
                                tIMG3SubHeaders[i] = tReader.ReadIMG3SubHeader();

                                //We know the images start after the image headers, so we'll record this offset now for loading images later.
                                tIMG3SubHeaders[i].imageOffset = nOffset;

                                //Move to the next offset by adding the image size found in the image header.
                                nOffset += tIMG3SubHeaders[i].imageSize;
                            }
                        }

                        //Convert them to a generic interface so we can load more easily.
                        tSubHeaders = new IMGSubHeader[tIMG3SubHeaders.Length];
                        for (int i = 0; i < tIMG3SubHeaders.Length; i++)
                            tSubHeaders[i] = tIMG3SubHeaders[i];
                    }
                    break;
                default://Unknown header.
                    {
                        tReader.Close();
                        MessageBox.Show(Path.GetFileName(szIMGPath) + " is not a valid IMG file. The leading magic characters are invalid");
                        return false;
                    }
            }

            /**********************************************************************/
            //  This is a bit confusing, but the IMG format is just a special pack
            //  file only for image data. It's literally packing entire TIM2 images
            //  into itself, with headers and all. We have to parse through headers
            //  in the IMG file system to get to the TIM2 images just so we can load
            //  them. I structured the load like this because TIM2 images can be
            //  found outside the IMG pack.
            /**********************************************************************/

            //Finally, we can load in our individual images.
            List<Types.TIM2Image> tImageList = new List<Types.TIM2Image>();
            {
                Types.TIM2Image tImage;
                for (int image = 0; image < tSubHeaders.Length; image++)
                {
                    //Read the image from the stream.
                    tImage = tReader.ReadTIM2Image(tSubHeaders[image].imageOffsetValue);

                    //If the load was valid, we want to add it to the resulting image list.
                    if (tImage != null)
                    {
                        //Calculate a file path by using the image's name in the file path.
                        tImage.filePath = Path.Combine(Path.GetDirectoryName(szIMGPath), tSubHeaders[image].nameValue) + ".tm2";

                        //Add to our successfully loaded image list.
                        tImageList.Add(tImage);
                    }
                }
            }

            //Close our file stream.
            tReader.Close();

            //Return the results of the load.
            tImages = tImageList.ToArray();
            return true;
        }

        /// <summary>
        /// Reads an IMG2SubHeader from the file stream.
        /// </summary>
        /// <param name="tReader">The input stream.</param>
        /// <returns>The header that was read.</returns>
        static IMG2SubHeader ReadIMG2SubHeader(this BinaryReader tReader)
        {
            IMG2SubHeader tHeader = new IMG2SubHeader();

            tHeader.name = tReader.ReadString(32);
            tHeader.imageOffset = tReader.ReadInt32();
            tHeader.unknown1 = tReader.ReadInt32();
            tHeader.unknown2 = tReader.ReadInt32();
            tHeader.unknown3 = tReader.ReadInt32();

            return tHeader;
        }

        /// <summary>
        /// Reads an IMG3SubHeader from the file stream.
        /// </summary>
        /// <param name="tReader">The input stream.</param>
        /// <returns>The header that was read.</returns>
        static IMG3SubHeader ReadIMG3SubHeader(this BinaryReader tReader)
        {
            IMG3SubHeader tHeader = new IMG3SubHeader();

            //Load in the header data.
            tHeader.name = tReader.ReadString(32);
            tHeader.imageChunkSize = tReader.ReadInt32();
            //tHeader.headerSize = tReader.ReadInt32();
            tHeader.imageOffset = tReader.ReadInt32();
            tHeader.unknown5 = tReader.ReadInt32();
            tHeader.unknown6 = tReader.ReadInt32();
            tHeader.unknown7 = tReader.ReadInt32();
            tHeader.imageSize = tReader.ReadInt32();
            tHeader.unknown8 = tReader.ReadInt32();
            tHeader.unknown9 = tReader.ReadInt32();

            return tHeader;
        }
    }
}