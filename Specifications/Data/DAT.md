*------------------------------------------------------------------------------------------------*

File Specification:		Level-5 Data

Extension:			.dat/.DAT

Purpose:			contains packaged non-compressed assets used by Level-5's Playstation 2-era games.

Author:				Level-5(Game Developer, www.level5.co.jp)

Author Date:			2000

Applications:			Dark Cloud 1, Dark Cloud 2

Spec Author:			muddle

Disclaimer:				This format is speculative. Only the original author knows the exact specification.
	This information was derived through reverse engineering and experimentation. Information may be incorrect or	
	incomplete.

*------------------------------------------------------------------------------------------------*

Purpose(expanded):		DAT files contain all of the assets used by both Dark Cloud and Dark Cloud 2.
	These assets cover a wide range of types, such as 3d models, sound effects, textures, menus,
	scripts, map information, etc. This information is packed into a singular non-compressed DAT
	archive. This archive is generally found in the root directory of the game's iso.

	The DAT file only contains assets. The file system for this archive is contained within an
	accompanying HD2 or HD3 file, which is found in the same directory as the DAT. Without this
	file system, the DAT cannot be extracted. See the file specs for HD2 or HD3 for more details.

	Each file is packed next to each other with no headers or markers to identify where files
	begin or end. These markers and file sizes are kept in the HD2 and HD3.

	Once the file position and size has been loaded from the HD2/HD3, you can read the corresponding
	file from the DAT.
	
	You only need one file system, either HD2 or HD3. I do not know why both file systems are
	included with the DAT. They both contain identical data, in slightly different formats.

*------------------------------------------------------------------------------------------------*

File Layout:
---------------------------
int32 == 4 byte integer
string == char array with null terminator
eof == end of file

DAT
{
	byte[fileLength] data;
	eof
}

*------------------------------------------------------------------------------------------------*

Implementation(pseudocode):
---------------------------
//The following code details how to extract files from the DAT using the HD2 file system.
	

//See HD2 spec for information about loading this struct.
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


//The file entry loading function. See HD2's file spec.
HD2FileEntry[] ReadHD2(string szHD2FilePath);


//Reading a DAT file using an HD2 file system and writing new output files.
void ExtractDAT(string szDATFilePath, string szHD2FilePath)
{
	//Open our DAT file.
	InputStream input = File.Open(szDATFilePath);

	//Read the file system from the HD2. See HD2's loading code
	FileEntryHD2[] tEntries = ReadHD2(szHD2FilePath);

	//For every entry in the file system.
	for(int file = 0; file < len(tEntries); file++)
	{
		//Generate an output path based on your preferences.
		string szOutputPath = "Some\Output\Path\" + tEntries[file].name; 

		//Create an output file.
		OutputStream output = File.Create(szOutputPath);

		//Set the read position in the input stream to the file entry's position.
		input.Position = tEntries[file].position;

		//Read the file's bytes from the input stream.
		byte[] tFileBytes = input.ReadBytes(tEntries[file].size);

		//Write the file's bytes to the output stream.
		output.WriteBytes(tFileBytes);

		//Close the output stream.
		output.Close();
	}
	
	//Close our input stream.
	input.Close();
}


---------------------------
//The following code details how to extract files from the DAT using the HD3 file system.

//The ISO Alignment of the HD3 file.
const long IsoAlignment = 0x800;


//See HD3 spec for information about loading this struct.
struct HD3FileEntry
{
    int32 nameOffset;		//the offset to the name of the file entry.
    int32 size;				//the size of the file.
    int32 blockPosition;	//the position of the data block (ISO aligned). Unlike HD2, this is actually used.
    int32 blockSize;		//Unused, The size of the block.
	string name;			//The name of the file.
}


//The file entry loading function. See HD3's file spec.
HD3FileEntry[] ReadHD3(string szHD3FilePath);


//Reading a DAT file using an HD3 file system and writing new output files.
void ExtractDAT(string szDATFilePath, string szHD3FilePath)
{
	//Open our DAT file.
	InputStream input = File.Open(szDATFilePath);

	//Read the file system from the HD3. See HD3's loading code
	FileEntryHD3[] tEntries = ReadHD3(szHD3FilePath);

	//For every entry in the file system.
	for(int file = 0; file < tEntries.Length; file++)
	{
		//Generate an output path based on your preferences.
		string szOutputPath = "Some\Output\Path\" + tEntries[file].name; 

		//Create an output file.
		OutputStream output = File.Create(szOutputPath);

		//Set the read position in the input stream to the file entry's position.
		input.Position = tEntries[file].blockPosition * IsoAlignment;

		//Read the file's bytes from the input stream.
		byte[] tFileBytes = input.ReadBytes(tEntries[file].size);

		//Write the file's bytes to the output stream.
		output.WriteBytes(tFileBytes);

		//Close the output stream.
		output.Close();
	}
	
	//Close the input stream.
	input.Close();
}