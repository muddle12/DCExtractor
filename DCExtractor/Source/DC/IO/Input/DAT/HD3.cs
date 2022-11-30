using Custom.IO;
using System.Collections.Generic;
using System.IO;

namespace DC.IO
{
    /// <summary>
    /// Class   :   "HD3"
    /// 
    /// Purpose :   The file system for extracting file information from the .DAT file. This is the HD3 version. This serves the same purpose as the HD2 version, only
    /// with what I presume is an updated version of the file system. You'll notice this version uses less space than HD2.
    /// </summary>
    public static class HD3
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Classes.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public struct HD3Entry : DAT.IDATEntry
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Constants.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            public const long IsoAlignment = 0x800;


            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Data Members.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            public int NameOffset;      //offset to the name of the file entry.
            public int Size;            //size of the file.
            public int BlockPosition;   //the position of the data block (ISO aligned).
            public int BlockSize;       //The size of the block.

            //calculated data.
            public string Name;         //You need to use the nameoffset to jump to the name and retrieve it. It is not necessarily contiguous with the block entry.


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
            public long position { get { return BlockPosition * IsoAlignment; } }

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
                    return Size == 0 &&
                        BlockPosition == 0 &&
                        BlockSize == 0;
                }
            }
        };


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Loads an HD3 file and returns the list of file entries.
        /// </summary>
        /// <param name="szHD3Path">The path to the HD3.</param>
        /// <param name="tOutputEntries">A list of output entries.</param>
        public static bool LoadHD3(string szHD3Path, ref List<DAT.IDATEntry> tOutputEntries)
        {
            //clear the output.
            tOutputEntries.Clear();

            //open our hd3 file stream.
            BinaryReader tReaderHD3 = FileStreamHelpers.OpenBinaryReader(szHD3Path);
            if (tReaderHD3 == null)
                return false;

            //Read in all of the file entries.
            HD3Entry tEntry;
            while (tReaderHD3.BaseStream.Position < tReaderHD3.BaseStream.Length)
            {
                //The entries are sequential, so we just read them one at a time until we get an invalid one.
                tReaderHD3.ReadHD3Entry(out tEntry);
                if (tEntry.isEmpty)//The last entry is zeroed out.
                    break;
                tOutputEntries.Add(tEntry);
            }
            tReaderHD3.Close();

            return true;
        }

        /// <summary>
        /// Reads an HD3Entry from the file stream.
        /// </summary>
        /// <param name="tReader">The input stream.</param>
        /// <param name="tEntry">The entry to write to.</param>
        static void ReadHD3Entry(this BinaryReader tReader, out HD3Entry tEntry)
        {
            //read in the entry's data.
            tEntry.NameOffset = tReader.ReadInt32();
            tEntry.Size = tReader.ReadInt32();
            tEntry.BlockPosition = tReader.ReadInt32();
            tEntry.BlockSize = tReader.ReadInt32();

            //Save this position, as we'll need to make a jump to get the name data.
            long nSave = tReader.BaseStream.Position;

            //Jump to the name offset.
            tReader.BaseStream.Position = (long)tEntry.NameOffset;

            //Read in the name.
            tEntry.Name = tReader.ReadString();

            //Restore our original position before the jump.
            tReader.BaseStream.Position = nSave;
        }
    }
}
