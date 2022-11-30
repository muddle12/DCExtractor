*------------------------------------------------------------------------------------------------*
File Specification:		Level-5 Image3(IM3) Archive

Extension:			.img/.IMG

Purpose:			contains packaged non-compressed images used by Level-5's Playstation 2-era games. 
						This is version 3.

Author:				Level-5(Game Developer, www.level5.co.jp)

Author Date:			2000

Applications:			Dark Cloud 2

Spec Author:			muddle

Disclaimer:				This format is speculative. Only the original author knows the exact specification.
	This information was derived through reverse engineering and experimentation. Information may be incorrect or	
	incomplete.

*------------------------------------------------------------------------------------------------*

Purpose(expanded):		The Image3 archive is an off-shoot of the PAK/package format, specifically tailored to hold 
	TIM2Image files. TIM2Images are the native image format used by the Playstation 2. TIM2Images are not exclusive
	to the Image3 archive, and do not need to be stored in an Image3 archive in order to be utilized. They can be
	stand-alone and loaded individually by the game engine.
	
	The Image3 archive begins with an Image3Header, denoted by the magic value IM30 (0x494D3300). The header is 16 bytes
	in size and contains four 4-byte integer values. A magic value, a firstHeaderOffset denoting the offset to the first 
	subheader in the archive, an imageCount, and finally 4 bytes of padding.
	
	Following the Image3Header is an array of Image3SubHeaders equal to the imageCount, which describe the size of 
	each image stored in the archive. They are 64 bytes in size, beginning with a 32-byte name and eight 4-byte 
	integer values. These values are the headerSize(always 64), imageChunkSize, unknown(always 1), padding(always 0), 
	unknown2(0 or 1), imageSize, unknown3(0 or 5), and padding(always 0).
	
	These subheaders are sequential, matching the order of the TIM2Images being stored. The first subheader corresponds to
	the first TIM2Image, the second subheader corresponds to the second TIM2Image, etc.
	
	After the last subheader is the first of one or more TIM2Images up to imageCount. These are stored in their entirity, header and all.
	The TIM2Image is described more in the TIM2Image specification (TM2.txt).
	
	You can extract these TIM2Images from the Image3 archive directly into a new file by simply copying everything starting
	at the beginning of the TIM2ImageHeader to imageSize.
	
	Technically, you don't even need an external tool to do this. Using a simple text editor or hex editor, you can extract
	the TIM2s from the archive manually directly into a new file and they should work.
	
*------------------------------------------------------------------------------------------------*

File Layout:
---------------------------
int32 == 4 byte integer
long64 = 8 byte integer
char == 1 byte ASCII character
string == char array with null terminator
eof == end of file

IM3
{
	Image3Header(16 bytes)
        int32 magic;                       	//The magic numbers denoting the version of this IMG file.
        int32 firstHeaderOffset;           	//The offset to the first tim2image header in the file.
        int32 imageCount;                 	//The number of images stored in the IMG file.
        int32 padding;                    	//always 0.
	Image3SubHeader(64 bytes)[imageCount]
		char[32] name;                     	//The name of this image. Always 32 bytes long, with null terminators.
		int32 headerSize;                	//The size of this ImageHeader, Always 64.
		int32 imageChunkSize;              	//The size of the image chunk in this tim2image.
		int32 unknown5;                    	//Unknown, always 1
		int32 padding1;                    	//always 0
		int32 unknown7;                    	//Unknown, either 0 or 1
		int32 imageSize;                   	//The size of the image data.
		int32 unknown8;                    	//Unknown, either 0 or 5
		int32 padding2;                    	//always 0
	TIM2Image(Image3SubHeader.imageSize)[imageCount]
		(see TM2.txt spec for more info)
	eof
}

*------------------------------------------------------------------------------------------------*

Implementation(pseudocode):
---------------------------

//IM3 magic.
const int32 IM3Magic = 0x494D3300;

//The main header for the IM3 archive.
struct IM3Header
{
	int32 magic;            		//The magic numbers denoting the version of this IMG file.
	int32 firstHeaderOffset;		//The offset to the first tim2image header in the file.
	int32 imageCount;       		//The number of images stored in the IMG file.
	int32 padding;         			//padding, always 0
};

//Sub headers which describe the TIM2Images packed in the file.
struct IMG3SubHeader
{
	string name;                    //The name of this image. Always 32 bytes long, with null terminators.
	int32 headerSize;        		//The size of this ImageHeader, Always 64.
	int32 imageChunkSize;      		//The size of the image chunk in this tim2image.
	int32 unknown5;            		//Unknown, always 1
	int32 padding1;            		//padding, always 0
	int32 unknown7;            		//Unknown, either 0 or 1
	int32 imageSize;           		//The size of the image data.
	int32 unknown8;            		//Unknown, either 0 or 5
	int32 padding2;            		//padding, always 0
}

//Reading an IMG file using and extracting the TIM2Images from it.
void ExtractIMG3(string szIMGFilePath)
{
	//Open our IMG file.
	InputStream input = File.Open(szIMGFilePath);
	
	//First, we'll read the main header.
	IM3Header tMainHeader;
	
	//Read in the 16 byte header as four 4-byte integers.
	tMainHeader.magic = input.ReadInt32();
	tMainHeader.firstHeaderOffset = input.ReadInt32();
	tMainHeader.imageCount = input.ReadInt32();
	tMainHeader.padding = input.ReadInt32();
	
	//If the magic value is not IM30, this is not a valid archive.
	if(tMainHeader.magic != IM3Magic)
		return;
	
	//Allocate an array of subheaders.
	IMG3SubHeader[tMainHeader.imageCount] tSubHeaders; 
	
	//loop through the sub headers and read them in.
	for(int subheader = 0; subheader < tMainHeader.imageCount; subheader++)
	{
		//Save the starting position of this header.
		long64 headerStart = input.Position;
		
		//Read a null-terminated string from the stream. Do not exceed 32 bytes.
		tSubHeaders[subheader].name = input.ReadString(limit=32);
		
		//Skip past the name section, starting at the header start offset.
		input.Position = headerStart + 32;
		
		//Load in 32 bytes of 4-byte integer data.
		tSubHeaders[subheader].headerSize = input.ReadInt32();
		tSubHeaders[subheader].imageChunkSize = input.ReadInt32();
		tSubHeaders[subheader].unknown5 = input.ReadInt32();
		tSubHeaders[subheader].padding1 = input.ReadInt32();
		tSubHeaders[subheader].unknown7 = input.ReadInt32();
		tSubHeaders[subheader].imageSize = input.ReadInt32();
		tSubHeaders[subheader].unknown8 = input.ReadInt32();
		tSubHeaders[subheader].padding2 = input.ReadInt32();
	}
	
	//Loop through the TIM2Images and extract them to new files.
	for(int tim2image = 0; tim2image < tMainHeader.imageCount; tim2image++)
	{
		//Generate an output path based on your preferences.
		string szOutputPath = "Some\Output\Path\" + tEntries[file].name; 

		//Create an output file.
		OutputStream output = File.Create(szOutputPath);
		
		//Read the file's bytes from the input stream.
		byte[] tFileBytes = input.ReadBytes(tSubHeaders[tim2image].imageSize);
		
		//Write the file's bytes to the output stream.
		output.WriteBytes(tFileBytes);
		
		//Close the output stream.
		output.Close();
	}
	
	//Close the input stream.
	input.Close();
}