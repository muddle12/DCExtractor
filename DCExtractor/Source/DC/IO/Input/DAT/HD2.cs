using Custom.IO;
using System.Collections.Generic;
using System.IO;

namespace DC.IO
{
    /// <summary>
    /// Class   :   "HD2"
    /// 
    /// Purpose :   The file system for extracting file information from the .DAT file. This is the HD2 version. This serves the same purpose as the HD3 version, only
    /// with what I presume is an outdated version of the file system.
    /// </summary>
    public static class HD2
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Classes.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public struct HD2Entry : DAT.IDATEntry
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Constants.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            public const long IsoAlignment = 0x800;


            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Data Members.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            public int NameOffset;     //offset to the name of the file entry.
            public int Unused1;        //unused padding.
            public int Unused2;
            public int Unused3;
            public int Position;       //position into the dat.
            public int Size;           //size of the file.
            public int BlockPosition;  //the position of the data block (ISO aligned).
            public int BlockSize;      //The size of the block.

            //calculated data.
            public string Name;        //You need to use the nameoffset to jump to the name and retrieve it. It is not necessarily contiguous with the block entry.


            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Properties.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Gets the name of this entry.
            /// </summary>
            public string name { get { return Name; } }

            /// <summary>
            /// Gets the position of this entry in the dat.
            /// </summary>
            public long position
            {
                get { return Position; }
            }

            /// <summary>
            /// Gets the size of this entry in the dat.
            /// </summary>
            public int size { get { return Size; } }

            /// <summary>
            /// Gets the position of the data block.
            /// </summary>
            public int blockPosition { get { return BlockPosition; } }

            /// <summary>
            /// Gets the size of the data block.
            /// </summary>
            public int blockSize { get { return BlockSize; } }

            /// <summary>
            /// Gets whether or not this struct is empty.
            /// </summary>
            public bool isEmpty
            {
                get
                {
                    return Position == 0 &&
                        Size == 0 &&
                        BlockPosition == 0 &&
                        BlockSize == 0;
                }
            }
        };


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Loads an HD2 file and returns the list of file entries.
        /// </summary>
        /// <param name="szHD2Path">The path to the HD2.</param>
        /// <param name="tOutputEntries">A list of output entries.</param>
        public static bool LoadHD2(string szHD2Path, ref List<DAT.IDATEntry> tOutputEntries)
        {
            //clear the output.
            tOutputEntries.Clear();

            //open our hd2 file stream.
            BinaryReader tReaderHD2 = FileStreamHelpers.OpenBinaryReader(szHD2Path);
            if (tReaderHD2 == null)
                return false;

            //Read in all of the file entries.
            HD2Entry tEntry;
            while (tReaderHD2.BaseStream.Position < tReaderHD2.BaseStream.Length)
            {
                //The entries are sequential, so we just read them one at a time until we get an invalid one.
                tReaderHD2.ReadHD2Entry(out tEntry);
                if (tEntry.isEmpty)//The last entry is zeroed out.
                    break;
                tOutputEntries.Add(tEntry);
            }
            tReaderHD2.Close();

            return true;
        }

        /// <summary>
        /// Reads an HD2Entry from the file stream.
        /// </summary>
        /// <param name="tReader">The input stream.</param>
        /// <param name="tEntry">The entry to write to.</param>
        static void ReadHD2Entry(this BinaryReader tReader, out HD2Entry tEntry)
        {
            //read in the entry's data.
            tEntry.NameOffset = tReader.ReadInt32();
            tEntry.Unused1 = tReader.ReadInt32();
            tEntry.Unused2 = tReader.ReadInt32();
            tEntry.Unused3 = tReader.ReadInt32();
            tEntry.Position = tReader.ReadInt32();
            tEntry.Size = tReader.ReadInt32();
            tEntry.BlockPosition = tReader.ReadInt32();
            tEntry.BlockSize = tReader.ReadInt32();

            //Save this position, as we'll need to make a jump to get the name data.
            long nSave = tReader.BaseStream.Position;

            //Jump to the name offset.
            tReader.BaseStream.Position = (long)tEntry.NameOffset;

            //Read in the name.
            tEntry.Name = tReader.ReadStringZeroOrEnd();

            //Restore our original position before the jump.
            tReader.BaseStream.Position = nSave;
        }

        /// <summary>
        /// An alternative string load method that stops at 0 or the end of file.
        /// </summary>
        /// <returns>A string.</returns>
        static string ReadStringZeroOrEnd(this BinaryReader tReader)
        {
            //Snapshot the beginning of the read.
            long nBeginning = tReader.BaseStream.Position;
            int nStringLength = 0;
            bool bEOF = true;

            //count until we see a 0 or end of file.
            while (tReader.BaseStream.Position < tReader.BaseStream.Length)
            {
                if (tReader.ReadByte() == 0)
                {
                    bEOF = false;
                    break;
                }
                nStringLength++;
            }
            //return to the beginning of the read.
            tReader.BaseStream.Position = nBeginning;

            //Load bytes from the file. ReadChars doesn't work well with the japanese characters, we've got to go around them using ReadBytes.
            byte[] tBytes = tReader.ReadBytes(nStringLength);
            char[] tChars = new char[nStringLength];
            for (int i = 0; i < tChars.Length; i++)
                tChars[i] = (char)tBytes[i];

            //read the chars in as a string.
            string szResult = new string(tChars);

            //correct our position.
            if (bEOF)
                tReader.BaseStream.Position = tReader.BaseStream.Length;
            else
                tReader.BaseStream.Position = nBeginning + nStringLength + 1;

            //return the result.
            return szResult;
        }
    }
}
