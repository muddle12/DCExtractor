*------------------------------------------------------------------------------------------------*
File Specification:		Level-5 Package

Extension(s):			.pak/.PAK	(default)
						.chr/.CHR	(Character Data)
						.ipk/.IPK	(Image Data)
						.mpk/.MPK	(Model Data)
						.pcp/.PCP	(Unknown, Map Data?)
						.efp/.EFP	(Effect Data)
						.snd/.SND	(Sound Effect/Music Data)
						.sky/.SKY	(Skybox Model Data)

Purpose:			a generic non-compressed archive of similar data.

Author:				Level-5(Game Developer, www.level5.co.jp)

Author Date:			2000

Applications:			Dark Cloud 1, Dark Cloud 2

Spec Author:			muddle

Disclaimer:				This format is speculative. Only the original author knows the exact specification.
	This information was derived through reverse engineering and experimentation. Information may be incorrect or	
	incomplete.

*------------------------------------------------------------------------------------------------*

Purpose(expanded):		PAK archives contain non-compressed files packed together in series. Usually, PAK archives group together
	files that relate to each other. For example, models, animations, and textures could be packaged together, usually per game object or
	character. 
	
	Unlike the DAT/HD2/HD3 file system, each file has a header before the file data, describing the file that follows. Each PAK archive
	begins with a file header, followed by its associated data, followed by another header, and its data, and so on. This continues 
	until an invalid header is reached, or end of file. Invalid Headers have all of their variables set to zero.
	
	All file headers have a fixed size 80 bytes. The first 64 bytes of the header is reserved for the file's name, sometimes represented
	as an absolute path based on the original developer's file system. This name is null-terminated, and can contain garbage characters
	after the null terminator.
	
	The rest of the header is three 4-byte integers describing the fileLength in bytes, endOffset in bytes into the file, and the 
	"type" of file. The type parameter is currently unknown, but does not appear to be necessary for extracting the file's from the
	PAK archive.

	The file header's file name may contain absolute file paths, invalid drive letters, illegal OS characters, and no file extensions. 
	The name needs to be sanitized before it can be used. Otherwise, depending on which OS you are using, this could cause errors or
	exceptions when parsing PAK archives.

*------------------------------------------------------------------------------------------------*

File Layout:
---------------------------
int32 == 4 byte integer
char == 1 byte ASCII character
long64 = 8 byte integer
eof == end of file

PAK
{
	FileHeader(80 bytes)
		char[64] name;      //This section is always 64 bytes, but is also a null-terminated string.
		int32 headerSize;
		int32 fileLength;
		int32 endOffset;
		int32 type;
	FileData(...)
		byte[FileHeader.fileLength];
	FileHeader(80 bytes)
	FileData(...)
	FileHeader(80 bytes)
	FileData(...)
	...
	FileHeader(80 bytes) //Invalid header: headerSize == 0, fileLength == 0, endOffset == 0, type == 0
	eof
}

*------------------------------------------------------------------------------------------------*

Implementation(pseudocode):
---------------------------

//A file header for a PAK file.
struct PAKFileHeader
{
	string name;        //The first 64 bytes are the file path. This is null terminated, and the remainder appears to be garbage.
	int headerSize;     //The size of the header in the file. Always 80 bytes.
	int fileLength;     //The length of the file chunk.
	int endOffset;      //The end offset of the file chunk.
	int type;           //Unused, Some kind of type flag. Unknown.
}

//Reading a PAK file and writing the outputs to new files.
void ExtractPAK(string szPAKFilePath)
{
	//Open our PAK file.
	InputStream input = File.Open(szPAKFilePath);
	
	//temporary variable.
	PAKFileHeader tHeader;
	
	//Loop until we reach EOF. We'll break on an invalid header.
	while(input.EOF() == false)
	{
		//Record the header's position. in the file.
		long64 headerStart = input.Position;
		
		//Read a null-terminated string from the stream. Do not exceed 64 bytes.
		tHeader.name = input.ReadString(limit=64);
		
		//Optional, you could trim the file paths out of the name and sanitize any invalid characters.
		//tHeader.name = GetValidFileName(tHeader.name);
		
		//Move to the next section of the header, starting at the header's start position + 64 bytes of character data.
		input.Position = headerStart + 64;
	
		//Read in the header data.
		tHeader.headerSize = input.ReadInt32();
		tHeader.fileLength = input.ReadInt32();
		tHeader.endOffset = input.ReadInt32();
		tHeader.type = input.ReadInt32();

		//If the header is invalid, we've reached the end of the file headers.
		if(tHeader.headerSize == 0 && tHeader.fileLength == 0 && tHeader.endOffset == 0 && tHeader.type == 0)
			break;
			
		//Generate an output path based on your preferences.
		string szOutputPath = "Some\Output\Path\" + tHeader.name; 

		//Create an output file.
		OutputStream output = File.Create(szOutputPath);
		
		//Read the file's bytes from the input stream.
		byte[] tFileBytes = input.ReadBytes(tHeader.fileLength);
		
		//Write the file's bytes to the output stream.
		output.WriteBytes(tFileBytes);

		//Close the output stream.
		output.Close();
	}
	
	//Close the input stream.
	input.Close();
}