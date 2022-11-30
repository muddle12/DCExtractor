using Custom.IO;
using DCExtractor;
using System.IO;
using System.Windows.Forms;

namespace DC.IO
{
    /// <summary>
    /// Class   :   "PAK"
    /// 
    /// Purpose :   the generic package file for a Level-5 PS2 game.
    /// 
    /// Credit to Kojin for figuring out the pak format.
    /// https://tcrf.net/User:Kojin/Dark_Cloud_2_Technical_Information
    /// 
    /// Originally, I only intended to figure out the MDS format. External tools would be included and invoked from DCExtractor, mainly out of convienance.
    /// I would invoke Kojin's pakextract.exe from DCExtractor, sending it the relevant pack file path that I needed extracted, and then retrieving that 
    /// data after the operation was complete. However, pakextract was only written to accept one file at a time, and would dump files
    /// in the same directory as pakextract, which meant if you wanted to extract a lot of assets at once, you'd end up with dozens if not hundreds of command 
    /// lines constantly popping up and being dismissed, and lots of assets being dumped in the root directory of pakextract.
    /// I had to auto-kill these command prompts using send inputs via .NET, and move files to the destinations from the root folder. It was a bit of a nightmare. 
    /// After a while, I realized this was a rather hazardous(if not alarming to the end user) way to go about it.
    /// 
    /// Eventually, I broke down and wrote my own, learning about the format by comparing the outputs of pakextract against the input pak files,
    /// and ultimately reverse engineereing how the tool was extracting the data. Now, all pak file types are internally supported by DCExtractor.
    /// 
    /// Another interesting tidbit about this format that I learned over time is that it's basically everywhere. The following formats use the pak format:
    /// 
    ///     PAK     =   "Level-5 Pack File"
    ///     CHR     =   "Level-5 Character"
    ///     IPK     =   "Level-5 Image-Pack"
    ///     MPK     =   "Level-5 Model-Pack"
    ///     PCP     =   "Level-5 PCP"
    ///     EFP     =   "Level-5 Effect Pack"
    ///     SND     =   "Level-5 Sound"
    ///     SKY     =   "Level-5 Skybox"
    ///
    /// IMG uses a modified form of pak, specifically tailored for tm2 images.
    /// 
    /// 
    /// The pak file format is a long chain of file headers with data attached. There is no magic for these headers, so you have to calculate which header you are
    /// at based on whatever came before it.
    /// 
    /// At the start of a pak file is the first header, which begins with a name section, which is always 64-bytes. The name string will be null terminated, 
    /// and could potentially be a file path. After that is the header size which is always 80 bytes, a file length which is how long the next file is in bytes, and
    /// the end offset in bytes where the file's bytes ends.
    /// 
    /// For best results, save the start and end offsets of the header while reading.
    /// 
    /// From the end of the header, you'll want to read all bytes using either the file length or the end offset, they both do the same thing. After that, the
    /// next file header will begin.
    /// 
    /// If you encounter a file header with 0 bytes and a length that is greater than the length of the file, you've reached the terminating header, and you are done
    /// extracting the pak file.
    /// 
    /// Be warned, paks are not prefect. They contain hard-coded file paths from the original developer's harddrives, and occasionally they will be improperly compiled.
    /// I only encountered one or two of those in Dark Cloud 2, and have made the necessary hacks to avoid them, but there might be more in Dark Cloud 1.
    /// </summary>
    public static class PAK
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Classes.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        struct FileHeader
        {
            public string name;        //The first 64 bytes are the file path. This is null terminated, and the remainder appears to be garbage.
            public int headerSize;     //The size of the header in the file. Always 80 bytes.
            public int fileLength;     //The length of the file chunk.
            public int endOffset;      //The end offset of the file chunk.
            public int type;           //Possibly some kind of flag. It's definitely some kind of type, but as to what exactly it relates to is unknown.

            //calculated.
            public long headerStart;
            public long headerEnd;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Extracts a .pak file to a subdirectory of the same name.
        /// </summary>
        /// <param name="szPath">The path of the pak file to unpack.</param>
        public static void ExtractFile(string szPath)
        {
            //Setup our progress bar.
            DCProgress.value = 0;
            DCProgress.maximum = 1;
            DCProgress.name = "Extracting PAK";

            //Extract the PAK file.
            ExtractPAK(szPath);

            //Report the finish.
            DCProgress.value = DCProgress.maximum;
            DCProgress.name = "Extracting PAK - FINISHED";
        }

        /// <summary>
        /// Extracts all .pak files found within the directory and subdirectories.
        /// </summary>
        /// <param name="szDirectory">The root directory to start the extraction.</param>
        /// <param name="szExtensions">The types of pak files to laod.</param>
        public static void ExtractDirectory(string szDirectory, string[] szExtensions)
        {
            //Setup our progress bar.
            DCProgress.value = 0;
            DCProgress.maximum = 1;
            DCProgress.name = "Extracting PAK Directory";

            //Calculate how many files we're going to process.
            int nMax = 0;
            for (int fileType = 0; fileType < szExtensions.Length; fileType++)
                nMax += Directory.GetFiles(szDirectory, "*." + szExtensions[fileType], SearchOption.AllDirectories).Length;

            //Set our new maximum.
            DCProgress.maximum = nMax;

            //Extract all pak file types.
            for (int fileType = 0; fileType < szExtensions.Length; fileType++)
            {
                DCProgress.name = "Extracting " + szExtensions[fileType] + " Directory";

                //Extract the directory.
                ExtractDirectoryInternal(szDirectory, "*." + szExtensions[fileType]);

                //If we canceled, leave early.
                if (DCProgress.canceled)
                    return;
            }

            //Report the finish.
            DCProgress.value = DCProgress.maximum;
            DCProgress.name = "Extracting PAK Directory - FINISHED";
        }

        /// <summary>
        /// Extracts all .pak files found within the directory and subdirectories.
        /// </summary>
        /// <param name="szDirectory">The root directory to start the extraction.</param>
        /// <param name="szFilter">The filter to use for finding files.</param>
        static void ExtractDirectoryInternal(string szDirectory, string szFilter)
        {
            //Gather a list of all pak files using the filter passed.
            string[] szFiles = Directory.GetFiles(szDirectory, szFilter, SearchOption.AllDirectories);

            //Extract each of these files.
            for (int i = 0; i < szFiles.Length; i++)
            {
                //Extract a PAK file.
                ExtractPAK(szFiles[i]);

                //Increment the progress value.
                DCProgress.value += 1;

                //Quit if the process has been canceled.
                if (DCProgress.canceled)
                    break;
            }
        }

        /// <summary>
        /// Extracts a PAK file.
        /// </summary>
        /// <param name="szPath">The file path of the PAK file.</param>
        static void ExtractPAK(string szPath)
        {
            //Open our file stream.
            BinaryReader tReader = FileStreamHelpers.OpenBinaryReader(szPath);
            if (tReader == null)
                return;

            //Create some paths before we start extracting.
            string szOutputDir = Path.GetDirectoryName(szPath);
            string szSubDir = Path.Combine(szOutputDir, Path.GetFileNameWithoutExtension(szPath));

            /**************************************************************************/
            //  So, a dumb hack, but... for some reason s55 (and only s55) has a 
            //  double header, causing it to be dumped to disk without an extension. 
            //  When CreateDirectory rolls around and tries to create a directory, 
            //  it explodes because s55 exists. Renaming this file to an mds fixes the 
            //  conflict. I ran PAK extraction on the entire game and this was the 
            //  only pain point. Hopefully, it's the only one. I tried to gracefully 
            //  rename the file with an extension, but File.Move explodes instead. 
            //  Best I can do is throw an error to the user.
            /**************************************************************************/
            if (File.Exists(szSubDir))
            {
                if (Settings.showFileExtensionWarnings)
                {
                    MessageBox.Show("Conflict Detected!\n\n A file at path " + szSubDir + " does not have an extension.\n " +
                        "This would normally crash the program.\n" +
                        "Please add an extension to this file and extract it manually.", "File Has No Extension: Skipping");
                }
                tReader.Close();
                return;
            }

            //Create a directory to place the extracted files.
            Directory.CreateDirectory(szSubDir);

            //Loop through the pak and read headers.
            FileHeader tHeader;
            while (tReader.BaseStream.Position < tReader.BaseStream.Length)
            {
                //Read in a file header.
                tReader.ReadHeader(out tHeader);

                //If the file header is valid, extract the file to another file.
                if (tHeader.fileLength > 0 && tHeader.fileLength < tReader.BaseStream.Length)
                    tReader.ExtractFileData(ref tHeader, szSubDir);

                //Quit if the process has been canceled.
                if (DCProgress.canceled)
                    break;
            }

            //Close our input stream.
            tReader.Close();
        }

        /// <summary>
        /// Reads a file header from the pak file.
        /// </summary>
        /// <param name="tReader">The input stream.</param>
        /// <param name="tHeader">The header to read into.</param>
        static void ReadHeader(this BinaryReader tReader, out FileHeader tHeader)
        {
            tReader.NextAlignment(16);

            tHeader.headerStart = tReader.BaseStream.Position;
            tHeader.headerEnd = tHeader.headerStart + 80;

            //We need to skip the name and read this stuff first, since if we get a bad header the string read will fail.
            tReader.BaseStream.Position = tHeader.headerStart + 64;
            tHeader.headerSize = tReader.ReadInt32();
            tHeader.fileLength = tReader.ReadInt32();
            tHeader.endOffset = tReader.ReadInt32();
            tHeader.type = tReader.ReadInt32();

            //If this file is valid, go back and read the name.
            if (tHeader.fileLength > 0 && tHeader.fileLength < tReader.BaseStream.Length)
            {
                tReader.BaseStream.Position = tHeader.headerStart;
                tHeader.name = tReader.ReadString(32);
                tHeader.name = DAT.RemoveIllegalCharacters(tHeader.name);
                tHeader.name = Path.GetFileName(tHeader.name);//So, we need to call Path.GetFileName because the devs embedded their file system into their pak files. Cool.
            }
            else
            {
                tHeader.name = string.Empty;
            }
            tReader.BaseStream.Position = tHeader.headerEnd;
        }

        /// <summary>
        /// Extracts a file from the file stream into a separate file.
        /// </summary>
        /// <param name="tReader">The input stream.</param>
        /// <param name="tHeader">The header to use to extract the data from the pak.</param>
        /// <param name="szOutputDir">The output directory for the file's contents.</param>
        static void ExtractFileData(this BinaryReader tReader, ref FileHeader tHeader, string szOutputDir)
        {
            string szOutputPath = Path.Combine(szOutputDir, tHeader.name);
            BinaryWriter tWriter = FileStreamHelpers.OpenBinaryWriter(szOutputPath);

            tReader.BaseStream.Position = tHeader.headerEnd;
            tWriter.Write(tReader.ReadBytes(tHeader.fileLength));

            tWriter.Close();
        }
    }
}