using Custom.IO;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DC.IO
{
    /// <summary>
    /// Class   :   "DAT"
    /// 
    /// Purpose :   a storage file for all data related to PS2-era Level-5 games. DAT is the root file found on the disc that contains
    /// all of the non-code related data(Models, Textures, Sounds, Scripts).
    /// 
    /// The DAT file is paired with either an HD2 or HD3 file system for indexing the archive. You cannot unpack a DAT without at least
    /// one of them.
    /// 
    /// There's a lot of debug functions in here for checking data integerity when parsing the format. That stuff is legacy now and
    /// you can safely ignore it. It doesn't make it into the release build. I probably should have extracted it to its own class.
    /// 
    /// This code is mostly thanks to Xeeynamo's whitecloud tool. https://github.com/Xeeynamo/DarkCloud
    /// Since it was open source, and I already had a good 9 formats supported by the tool, I decided to go ahead and integrate
    /// his code and extend support for the DAT format as well.
    /// </summary>
    public static class DAT
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Classes.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Interface   :   "DATEntry"
        /// 
        /// Purpose     :   describes a generic entry into the dat file. The reason we are using an interface here is because
        /// the entry can vary depending on which version we are using. HD2 entries are different from HD3 entries. The
        /// interface stream-lines things so we only need one load function.
        /// </summary>
        public interface IDATEntry
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Functions.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Gets the name of this entry.
            /// </summary>
            string name { get; }

            /// <summary>
            /// Gets the position of this entry in the dat.
            /// </summary>
            long position { get; }

            /// <summary>
            /// Gets the size of this entry in the dat.
            /// </summary>
            int size { get; }

            /// <summary>
            /// Gets the position of the data block.
            /// </summary>
            int blockPosition { get; }

            /// <summary>
            /// Gets the size of the data block.
            /// </summary>
            int blockSize { get; }

            /// <summary>
            /// Gets whether or not this struct is empty.
            /// </summary>
            bool isEmpty { get; }
        };

#if DEBUG
        /// <summary>
        /// Class   :   "FileEntry"
        /// 
        /// Purpose :   this is entirely for debug purposes, it records duplicate file names to make sure we're loading this stuff properly.
        /// </summary>
        public class FileEntry
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Data Members.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            public string name;
            public int count;


            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Functions.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="szName">The name of the entry.</param>
            public FileEntry(string szName)
            {
                name = szName;
                count = 1;
            }
        };
#endif
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Enumerations.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Describes either an HD2 or HD3 file type.
        /// </summary>
        public enum HDType { Two, Three };
#if DEBUG
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Data Members.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static List<FileEntry> g_tEntries = new List<FileEntry>();


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Properties.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets a list of all entries loaded by the last DAT extraction.
        /// </summary>
        public static FileEntry[] entries
        {
            get { return g_tEntries.ToArray(); }
        }

        /// <summary>
        /// Gets a list of entries that overwrote each other during the extraction process.
        /// </summary>
        public static FileEntry[] conflictEntries
        {
            get
            {
                List<FileEntry> tConflictedEntries = new List<FileEntry>();
                for (int i = 0; i < g_tEntries.Count; i++)
                {
                    if (g_tEntries[i].count > 1)
                        tConflictedEntries.Add(g_tEntries[i]);
                }
                return tConflictedEntries.ToArray();
            }
        }
#endif

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Extracts data from a dat file using the hd2 file as an indexor.
        /// </summary>
        /// <param name="szDatPath">The path of the .dat file.</param>
        /// <param name="szHDPath">The path of the .hd2 file.</param>
        /// <param name="szDestDirectory">The directory to write the files to.</param>
        /// <param name="eType">The type of hd to load.</param>
        public static void ExtractHD(string szDatPath, string szHDPath, string szDestDirectory, HDType eType)
        {
            //Setup our progress bar.
            DCProgress.value = 0;
            DCProgress.maximum = 1;
            DCProgress.name = "Reading DAT File System";

            //First, let's load in the HD file based on type.
            List<DAT.IDATEntry> tEntries = new List<DAT.IDATEntry>();
            switch (eType)
            {
                case HDType.Two:
                    {
                        if (HD2.LoadHD2(szHDPath, ref tEntries) == false)
                            return;
                    }
                    break;
                case HDType.Three:
                    {
                        if (HD3.LoadHD3(szHDPath, ref tEntries) == false)
                            return;
                    }
                    break;
                default:
                    return;
            }

            //Update the progress bar to the number of entries.
            DCProgress.maximum = tEntries.Count;
            DCProgress.name = "Extracting Files From DAT";

            //open our dat file stream.
            BinaryReader tReaderDAT = FileStreamHelpers.OpenBinaryReader(szDatPath);
            if (tReaderDAT == null)
                return;

            //Output the individual files to disk.
            for (int entry = 0; entry < tEntries.Count; entry++)
            {
                tReaderDAT.ExtractFile(tEntries[entry], szDestDirectory);

                DCProgress.value = entry;//Update the progress bar.
                DCProgress.name = "Extracting " + tEntries[entry].name;

                //Quit if the process has been canceled.
                if (DCProgress.canceled)
                {
                    DCProgress.name = "Extracting " + tEntries[entry].name + " - CANCELED";
                    break;
                }
            }

            //close our file streams.
            tReaderDAT.Close();

            //Quit if the process has been canceled.
            if (DCProgress.canceled)
                return;

            //Report the finish.
            DCProgress.value = DCProgress.maximum;
            DCProgress.name = "Extracting DAT - FINISHED";
        }

        /// <summary>
        /// Removes all illegal characters from the path.
        /// </summary>
        /// <param name="szPath">The path to remove the illegal characters from.</param>
        /// <returns>A sanitized string.</returns>
        public static string RemoveIllegalCharacters(string szPath)
        {
            return Encoding.ASCII.GetString(Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding(Encoding.ASCII.EncodingName, new EncoderReplacementFallback(string.Empty), new DecoderExceptionFallback()), Encoding.UTF8.GetBytes(szPath)));
        }
#if DEBUG
        /// <summary>
        /// Returns the index of the file entry that shares the same name passed.
        /// </summary>
        /// <param name="szName">The name of the file entry to search for.</param>
        /// <returns>The index of the file index, otherwise -1.</returns>
        static int GetFileEntryIndex(string szName)
        {
            for (int i = 0; i < g_tEntries.Count; i++)
            {
                if (g_tEntries[i].name == szName)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Adds a file entry to the file entry list.
        /// </summary>
        /// <param name="szName">The name of the file entry.</param>
        static void AddEntry(string szName)
        {
            int nIndex = GetFileEntryIndex(szName);
            if (nIndex == -1)
                g_tEntries.Add(new FileEntry(szName));
            else
                g_tEntries[nIndex].count = 0;
        }
#endif

        /// <summary>
        /// Extracts a file from the file stream and deposits it in to the output file.
        /// </summary>
        /// <param name="tReader">The input stream.</param>
        /// <param name="tEntry">The entry of the file to extract.</param>
        /// <param name="szDestDirectory">The output directory for the file.</param>
        static void ExtractFile(this BinaryReader tReader, DAT.IDATEntry tEntry, string szDestDirectory)
        {
            //Create an output path from the entry's name.
            string szOutFilePath = Path.Combine(szDestDirectory, RemoveIllegalCharacters(tEntry.name));
            string szOutputDir = Path.GetDirectoryName(szOutFilePath);
#if DEBUG
            //Adds an entry to the name list.
            AddEntry(tEntry.name);
#endif
            //create the directory for the new file.
            Directory.CreateDirectory(szOutputDir);

            //open an output stream.
            BinaryWriter tWriter = FileStreamHelpers.OpenBinaryWriter(szOutFilePath);
            if (tWriter == null)
                return;

            //write the bytes to the output file.
            tReader.BaseStream.Position = tEntry.position;
            tWriter.Write(tReader.ReadBytes(tEntry.size));

            //close our stream.
            tWriter.Close();
        }
    }
}