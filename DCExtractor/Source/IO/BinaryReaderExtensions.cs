using Custom.Math;
using System.IO;

namespace Custom.IO
{
    /// <summary>
    /// Class   :   "BinaryReaderExtensions"
    /// 
    /// Purpose :   an extensions class for the System.IO.BinaryReader class.
    /// </summary>
    public static class BinaryReaderExtensions
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Seeks to the specified byte combination in the file. It's used for finding headers.
        /// </summary>
        /// <param name="tReader">The reader.</param>
        /// <param name="tBytes">The byte header to search for.</param>
        /// <returns>Whether or not the header could be found.</returns>
        [System.Obsolete]
        public static bool Seek(this BinaryReader tReader, byte[] tBytes)
        {
            //While we're not at the end of the file.
            while (tReader.BaseStream.Position != tReader.BaseStream.Length)
            {
                //Read a byte.
                byte tByte = tReader.ReadByte();

                //If the byte we're searching for matches the one we just read, it's time to see if we have a match.
                if (tByte == tBytes[0])
                {
                    //Read through the next few bytes.
                    bool bFound = true;
                    for (int i = 1; i < tBytes.Length; i++)
                    {
                        //Reading a byte to compare.
                        tByte = tReader.ReadByte();

                        //Comparing it against bytes in the buffer.
                        if (tByte != tBytes[i])
                        {
                            //If we didn't find a match, that means the remaining bits can't be the ones we want, abort.
                            tReader.BaseStream.Position = tReader.BaseStream.Position - 1;
                            bFound = false;
                            break;
                        }
                    }

                    //If we found the correct byte stream.
                    if (bFound)
                    {
                        //Return to just before we started reading the bytes and return true.
                        tReader.BaseStream.Position -= tBytes.Length;
                        return true;
                    }
                }
            }

            //Otherwise, we couldn't find this byte signature in the file.
            return false;
        }

        /// <summary>
        /// Seeks to the specified byte combination in the file. It's used for finding headers.
        /// </summary>
        /// <param name="tReader">The reader.</param>
        /// <param name="nHeader">The header to search for.</param>
        /// <returns>Whether or not the header could be found.</returns>
        public static bool SeekHeader(this BinaryReader tReader, uint nHeader)
        {
            //Creating a magic number byte buffer using bit shifting.
            byte[] tBytes = new byte[4]
            {
                (byte)((nHeader & 0xFF000000) >> (6 * 4)),
                (byte)((nHeader & 0x00FF0000) >> (4 * 4)),
                (byte)((nHeader & 0x0000FF00) >> (2 * 4)),
                (byte)((nHeader & 0x000000FF) >> (0 * 4))
            };

            //Loop until we reach the end of the file.
            while (tReader.BaseStream.Position != tReader.BaseStream.Length)
            {
                //Read a byte.
                byte tByte = tReader.ReadByte();

                //Compare the byte against the first by in the header magic.
                if (tByte == tBytes[0])
                {
                    //Search the next few bytes to see if we have a match.
                    bool bFound = true;
                    for (int i = 1; i < tBytes.Length; i++)
                    {
                        //Read another byte for comparison.
                        tByte = tReader.ReadByte();

                        //If the byte does not match the byte in the stream.
                        if (tByte != tBytes[i])
                        {
                            //Abort the search.
                            tReader.BaseStream.Position = tReader.BaseStream.Position - 1;
                            bFound = false;
                            break;
                        }
                    }

                    //If we found the magic signature in the stream.
                    if (bFound)
                    {
                        //Move back before the magic, and then return true.
                        tReader.BaseStream.Position -= sizeof(uint);
                        return true;
                    }
                }
            }

            //Otherwise, we couldn't find the header.
            return false;
        }

        /// <summary>
        /// Finds the specified byte combination in the file. It's used for finding headers.
        /// </summary>
        /// <param name="tReader">The reader.</param>
        /// <param name="tBytes">The byte header to search for.</param>
        /// <returns>The offset of the header. Otherwise 0.</returns>
        public static long Find(this BinaryReader tReader, byte[] tBytes)
        {
            //Save our previous position, we'll be returning to it.
            long nPrevPos = tReader.BaseStream.Position;

            //Loop through until the end of the file.
            while (tReader.BaseStream.Position != tReader.BaseStream.Length)
            {
                //Check to see if this byte matches our byte combination.
                if (tReader.ReadByte() == tBytes[0])
                {
                    //Loop through the next few bytes to see if they match.
                    bool bFound = true;
                    for (int i = 1; i < tBytes.Length; i++)
                    {
                        //If the byte doesn't match one in the array.
                        if (tReader.ReadByte() != tBytes[i])
                        {
                            //Abort, this can't be it.
                            tReader.BaseStream.Position = tReader.BaseStream.Position - 1;
                            bFound = false;
                            break;
                        }
                    }
                    
                    //If we did find a match.
                    if (bFound)
                    {
                        //Record the position.
                        long nPos = tReader.BaseStream.Position - tBytes.Length;

                        //Return to our previous position.
                        tReader.BaseStream.Position = nPrevPos;

                        //Return the result.
                        return nPos;
                    }
                }
            }
            
            //Otherwise, just return to the previous position and return 0.
            tReader.BaseStream.Position = nPrevPos;
            return 0;
        }

        /// <summary>
        /// Finds the specified byte combination in the file. It's used for finding headers. This version returns a boolean for success/failure.
        /// </summary>
        /// <param name="tReader">The reader.</param>
        /// <param name="tReader">The offset into the file.</param>
        /// <param name="tBytes">The byte header to search for.</param>
        /// <returns>Whether or not the byte string could be found.</returns>
        public static bool FindEx(this BinaryReader tReader, out int offset, byte[] tBytes)
        {
            //Save our previous position, we'll be returning to it.
            long nPrevPos = tReader.BaseStream.Position;

            //Loop through until the end of the file.
            while (tReader.BaseStream.Position != tReader.BaseStream.Length)
            {
                //Check to see if this byte matches our byte combination.
                if (tReader.ReadByte() == tBytes[0])
                {
                    //Loop through the next few bytes to see if they match.
                    bool bFound = true;
                    for (int i = 1; i < tBytes.Length; i++)
                    {
                        //If the byte doesn't match one in the array.
                        if (tReader.ReadByte() != tBytes[i])
                        {
                            //Abort, this can't be it.
                            tReader.BaseStream.Position = tReader.BaseStream.Position - 1;
                            bFound = false;
                            break;
                        }
                    }

                    //If we did find a match.
                    if (bFound)
                    {
                        //Record the position.
                        long nPos = tReader.BaseStream.Position - tBytes.Length;

                        //Return to our previous position.
                        tReader.BaseStream.Position = nPrevPos;

                        //Return the result.
                        offset = (int)nPos;
                        return true;
                    }
                }
            }

            //Otherwise, just return to the previous position and return 0.
            tReader.BaseStream.Position = nPrevPos;
            offset = 0;
            return false;
        }

        /// <summary>
        /// Reads an array of bytes and returns them as a string.
        /// </summary>
        /// <param name="tReader">The reader.</param>
        /// <param name="nByteCount">The number of bytes to read.</param>
        /// <returns>A string equivalent of the byte string.</returns>
        public static string ReadBytesAsString(this BinaryReader tReader, int nByteCount)
        {
            //Read in the byte array.
            byte[] tArray = tReader.ReadBytes(nByteCount);

            //Copy these bytes into a char array.
            char[] tChars = new char[nByteCount];
            for (int c = 0; c < tChars.Length; c++)
                tChars[c] = (char)tArray[c];

            //Create a new string.
            string szRet = new string(tChars);

            //Return the trimmed string.
            return szRet.Trim(new char[] { '\0' });
        }

        /// <summary>
        /// Reads an array of bytes up to the limit amount or the first null terminator.
        /// </summary>
        /// <param name="tReader">The reader.</param>
        /// <param name="nLimit">The maximum byte limit to read.</param>
        /// <returns>A null terminated string.</returns>
        public static string ReadString(this BinaryReader tReader, int nLimit)
        {
            //Read in the bytes.
            byte[] tArray = tReader.ReadBytes(nLimit);

            //Loop through and copy characters into the string until we reach a null terminator.
            string szRet = string.Empty;
            for (int c = 0; c < tArray.Length; c++)
            {
                if (tArray[c] != 0)
                    szRet += (char)tArray[c];
                else
                    break;
            }
            
            //Return the result.
            return szRet;
        }

        /// <summary>
        /// Reads a 4xs4 float matrix.
        /// </summary>
        /// <param name="tReader">The reader.</param>
        /// <returns>A matrix of 4x4 floats.</returns>
        public static Matrix4x4 ReadMatrix(this BinaryReader tReader)
        {
            //Read in the 16 floats into the matrix.
            Matrix4x4 tRet = new Matrix4x4();
            for (int m = 0; m < tRet.m.Length; m++)
                tRet.m[m] = tReader.ReadSingle();
            return tRet;
        }

        /// <summary>
        /// Moves the reader to the next alignment. 
        /// </summary>
        /// <param name="tReader">The reader.</param>
        /// <param name="nAlignmentInBytes">The byte alignment.</param>
        public static void NextAlignment(this BinaryReader tReader, long nAlignmentInBytes)
        {
            /************************************************************/
            //  File alignment is one of those weird old school things
            //  that I'm not even sure is really relevant anymore.
            //
            //  To put it simply, data can be aligned in blocks of a 
            //  particular interval. For example, an alignment of 16 means
            //  that data will be present at every 16th byte in the file,
            //  and must end at the next 16th. If you choose to align your
            //  data at 16ths, then that means you have to fill the extra
            //  unused bytes with empty values.
            //
            //  For example: I'm packing my data with Vector3 values to
            //  represent vertices, but I'm aligning my data at 16 bytes.
            //  This means that the first 12 bytes will be 3 floating point
            //  values, and then that last 4 will be empty bytes.
            //
            //  This function allows you to jump to the next alignment,
            //  skipping over any irrelevant data regardless of where
            //  you are sitting. Are you at the 13th byte? You can just
            //  jump to the next 16th using this function.
            //
            //  The main reason you would align data is as an optimization.
            //  Sometimes the data you need to push to RAM/CPU/GPU expects
            //  a certain format, and if you can just read straight from
            //  the file directly into one of those systems, you don't
            //  have to do a bunch of messy conversions or unpacking to
            //  get your data ready. You just read and go.
            //
            //  Processing power is so fast now this is a non-issue. The
            //  only reason this function exists was to enforce some
            //  consistancy earlier in the process of programming this 
            //  tool. I've actually removed a lot of alignment calls, so
            //  you could almost deprecate this function.
            /************************************************************/

            //We're going to want to take our current posistion and calculate the next byte offset that is a multiple of the alignment.
            long nPosition = tReader.BaseStream.Position;

            //We'll use modulus here to get the offset to move the file stream position.
            long nOffset = nPosition % nAlignmentInBytes;
            if (nOffset != 0)
                tReader.BaseStream.Position = nPosition + (nAlignmentInBytes - nOffset);
        }
    }
}