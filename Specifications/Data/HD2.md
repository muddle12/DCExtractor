File Specification:		Level-5 HD2
------------------------------------------------------------------------------------------------

Extension:			.hd2/.HD2

Purpose:			contains a file system that describes the layout of the .DAT archive file. 
							This is version 2 of the file system.

Author:				Level-5(Game Developer, www.level5.co.jp)

Author Date:			2000

Applications:			Dark Cloud 1, Dark Cloud 2

Spec Author:			muddle12

Disclaimer:				This format is speculative. Only the original author knows the exact specification.
	This information was derived through reverse engineering and experimentation. Information may be incorrect or	
	incomplete.

------------------------------------------------------------------------------------------------
Purpose(expanded):
---------------------------
	HD2 files contain the file system for indexing and retrieving files from the DAT archive.
	The file system contains a sequential list of file headers, which describe individual files and their locations and
	sizes within the DAT. The DAT file cannot be extracted without a valid file system. The HD2 is generally found in
	the root directory of the game's iso, next to the DAT archive.
	
	HD2 describes version 2 of the file system. This is not to be confused with HD3, which is version 3. Both versions
	should contain the same file system data, but are organized differently.
	
	All file headers have a fixed size of 32 bytes. They are arranged sequentially, starting at the beginning of the file 
	system. The file system ends upon encountering a file header with a position, size, blockposition, and blocksize that 
	equal zero. 
	
	Following the invalid file header is a list of all of the names of each file in the file system. Each file name is an ascii 
	character string with a null terminator. These strings can vary in size. They will have file extensions and potentially 
	subdirectories.
	
	Each file header contains a nameoffset that indicates where in the file names section of the file system one can find
	the name of this file. It is an offset from the beginning of the HD2. You should seek from the beginning of the file
	to the nameoffset and read the name until you reach a null terminator.
	
	You can use the file header's position value to seek to the file data in the DAT and extract bytes equal
	to the file header's size. The rest of the header's data(blockposition, blocksize) is unused.

File Layout:
---------------------------
```cs
int32 == 4 byte integer
string == char array with null terminator
eof == end of file

HD2
{
	FileHeader(32 bytes)
		int32 nameoffset
		int32 unused
		int32 unused
		int32 unused
		int32 position
		int32 size
		int32 blockposition
		int32 blocksize
	FileHeader(32 bytes)
	FileHeader(32 bytes)
	FileHeader(32 bytes)
	...
	FileHeader(32 bytes) //Invalid header: position, size, blockposition, and blocksize == 0
	string filename1	//starting the file system name section.
	string filename2
	string filename3
	...
	eof
}
```

Implementation(pseudocode):
---------------------------
```cs
//A file header within the HD2 file system.
struct HD2FileEntry
{
	int32 nameOffset;		//the offset to the name of the file entry.
	int32 unused1;			//unused padding.
	int32 unused2;			//unused padding.
	int32 unused3;			//unused padding.
	int32 position;			//the offset into the DAT file where the file begins.
	int32 size;				//size of the file.
	int32 blockPosition;	//Unused, the position of the data block (ISO aligned).
	int32 blockSize;		//Unused, The size of the block.
	string name;			//The name of this file.
}

//The file entry loading function.
HD2FileEntry[] ReadHD2(string szHD2FilePath)
{
	//Open our HD2 file.
	InputStream input = File.Open(szHD2FilePath);

	//temporary variable.
	HD2FileEntry tEntry;
	
	//Create a list to store our entries.
	List<HD2FileEntry> tEntries;

	//Loop until we reach EOF. We'll break on an invalid header.
	while(input.EOF() == false)
	{
		//Read 32 bytes of header information.
		tEntry.nameOffset = input.ReadInt32();
		tEntry.unused1 = input.ReadInt32();
		tEntry.unused2 = input.ReadInt32();
		tEntry.unused3 = input.ReadInt32();
		tEntry.position = input.ReadInt32();
		tEntry.size = input.ReadInt32();
		tEntry.blockPosition = input.ReadInt32();
		tEntry.blockSize = input.ReadInt32();
		
		//If the entry is invalid, we've reached the end of the file headers.
		if(tEntry.position == 0 && tEntry.size == 0 && tEntry.blockPosition == 0 && tEntry.blockSize == 0)
			break;
			
		//Otherwise, the entry is valid and should be added to the list of loaded headers.
		tEntries.Add(tEntry);
	}
	
	//Next, we will find and read all of the names for each of the headers.
	for(int header = 0; header < tEntries.Length; header++)
	{
		//Seek to the name offset in the HD2.
		input.Position = tEntries[header].nameOffset;
		
		//Read a null-terminated string from the stream.
		tEntries[header].name = input.ReadString();
		
		//Optional, you could trim the file paths out of the name.
		//tEntries[header].name = GetFileName(tEntries[header].name);
	}
	
	//Close the input stream.
	input.Close();
	
	//Return the list as an array or whatever array type you prefer.
	return tEntries.ToArray();
}
```
