*------------------------------------------------------------------------------------------------*
File Specification:		Sony Texture Image Version 2 (TIM2)

Extension:			.tm2/.TM2

Purpose:			contains formatted image data used by the Playstation 2.

Author:				Sony Group Corporation(Corporation, https://www.sony.com/en/)

Author Date:			2000

Applications:			Dark Cloud 1, Dark Cloud 2, Playstation 2 Games

Spec Author:			muddle

Disclaimer:				This format is speculative. Only the original author knows the exact specification.
	This information was derived through reverse engineering and experimentation. Information may be incorrect or	
	incomplete.
	
	There are more comprehensive specs out there(see references). It's written from the perspective of extracting 
	and converting TIM2Images to other image formats. This is not an explanation of how the image functions with 
	respect to the original hardware. It is also not a explanation of how to convert other image formats to TIM2.
	
External References:	https://github.com/marco-calautti/Rainbow/
						https://wiki.xentax.com/index.php/TM2_TIM2_Image

*------------------------------------------------------------------------------------------------*

Purpose(expanded):		The TIM2 image format is a standard image format used by the Sony Playstation 2 and PSP systems. It is a heavily
	modified bitmap format that supports alpha transparency, mipmapping, and paletting.
	
	Image data contained within a TIM2Image can support 4-bit Indexed/Paletted Color, 8-bit Indexed/Paletted Color, 16-bit ABGR1555 True Color,
	24-bit RGB True Color, and 32-bit RGBA True Color. The palette data can support 16-bit ABGR1555 True Color, 24-bit RGB True Color, and 32-bit RGBA 
	True Color. This palette data can be linear or interlaced. 
	
	Both image data and palette data are swizzled, meaning they have been reorganized into blocks instead of horizontal lines to more efficiently be 
	read by the Playstation 2's graphics system. They must be unswizzled in order to restore them to their original horizontal line layout. The
	algorithm for unswizzling will be listed below in the implementation section.
	
	The TIM2 image begins with a TIM2Header, which contains general information about the file, and is denoted by its magic value TIM2 (0x54494D32).
	The TIM2Header is 16 bytes in size. It starts with a 4-byte magic value, then a 1-byte version number, a 1-byte format number, a 2-byte imageCount, 
	and 8 bytes of unknown padding.
	
	Following the TIM2Header is the image information section. It begins with a list of various sizes and flags, which are too numerous to describe in
	this section. See the File Layout below for a verbose structure. After the information section is an array of image bytes. Finally, an array of
	palette bytes if there is a palette present in the image.
	
*------------------------------------------------------------------------------------------------*

File Layout:
---------------------------
int32 == 4 byte integer
short16 == 2 byte short integer
eof == end of file
long64 == 8 byte long integer

TIM2
{
	TIM2Header(16 bytes)
		int32 magic;                   			//The magic number that denotes the file type, TIM2.
		byte version;                			//The version of this TIM image.
		byte format;                 			//Usually 0.
		short16 pictureCount;          			//The number of pictures stored in this TIM2.
		int32 padding1;							//Padding
		int32 padding2;							//Padding
	TIM2Picture[pictureCount] pictures;		//The list of pictures held by the image.
		int32 totalSize;                       	//The total size of the image AFTER it's been unpacked.
        int32 paletteSize;                     	//The size of the palette section in bytes.
        int32 imageDataSize;                   	//The size of the image section in bytes.
        short16 headerSize;                    	//The size of this header.
        short16 colorEntries;                  	//The number of color entries in the image.
        byte imageFormat;                    	//The image format, presumably for the PS2 graphics engine.
        byte mipmapCount;                    	//The number of mipmaps in this image.
        byte CLUTFormat;                     	//Unknown
        byte bitsPerPixel;                   	//The number of bits per pixel in the image buffer. These are generally multiples of 4.
        short16 imageWidth;                    	//The width of this image.
        short16 imageHeight;                   	//The height of this image.
        long64 gsTEX0;                         	//Unknown 
        long64 gsTEX1;                         	//Unknown 
        int32 gsRegs;                          	//Unknown
        int32 gsTexClut;                       	//Unknown
        byte[headerSize - 48] userData;       	//Game-specific developer data.
        byte[imageDataSize] imageBytes;     	//The buffer of image data in a paletted/swizzled format that needs to be converted.
        byte[paletteSize] paletteBytes;   		//The buffer of palette pixels used for indexing the image bytes. It is in a paletted/swizzled format that needs to be converted.
	eof
}

*------------------------------------------------------------------------------------------------*

Implementation(pseudocode):
---------------------------

//The Magic for the TIM2.
const int32 TIM2Magic = 0x54494D32;

//The header at the start of the TIM2Image.
struct TIM2Header
{
	int32 magic;
	byte version;
	byte format;
	short16 imageCount;
	int32 padding1;
	int32 padding2;
}

//A single picture in the TIM2Image pictures array.
struct TIM2Picture
{
	int32 totalSize;
	int32 paletteSize;
	int32 imageDataSize;
	short16 headerSize;
	short16 colorEntries;
	byte imageFormat;
	byte mipmapCount;
	byte CLUTFormat;
	byte bitsPerPixel;
	short16 imageWidth;
	short16 imageHeight;
	long64 gsTEX0;
	long64 gsTEX1;
	int32 gsRegs;
	int32 gsTexClut;
	byte[] userData;
	byte[] imageBytes;
	byte[] paletteBytes;
	
	//calculated data. Not necessary, but handy to have.
	bool isLinearPalette;		//Refers to whether or not the palette data is linear or interleaved/interlaced.
	int colorSize;				//How many bits in size an individual entry in the imageBytes is.
	int userDataSize;			//The size of the user data section.
}

//The TIM2Image file.
struct TIM2Image
{
	TIM2Header header;
	TIM2Picture[] pictures;
}


//Reading a TIM2Image file.
TIM2Image LoadTIM2Image(string szTIM2ImageFilePath)
{
	//Open our DAT file.
	InputStream input = File.Open(szDATFilePath);
	
	//Allocate a header.
	TIM2Image tImage;

	//Read in the header information.
	tImage.header.magic = tReader.ReadInt32();
	
	//If the magic doesn't match the TIM2Magic, it's not a TIM2.
	if (tImage.header.magic != TIM2Magic)
	{
		//Close our input and return.
		input.Close();
		return null;
	}

	//Read in the remainder of the header.
	tImage.header.version = tReader.ReadByte();
	tImage.header.format = tReader.ReadByte();
	tImage.header.pictureCount = tReader.ReadInt16();
	tImage.header.padding1 = tReader.ReadInt32();
	tImage.header.padding2 = tReader.ReadInt32();
	
	//Allocate our pictures array.
	tImage.pictures = new TIM2Picture[tImage.header.pictureCount];
	
	//loop through the file, loading each picture after the header.
	for(int picture = 0; picture < tImage.header.pictureCount; picture++)
	{
		//Reading in bulk information.
		tImage.pictures[picture].totalSize = tReader.ReadInt32();
		tImage.pictures[picture].paletteSize = tReader.ReadInt32();
		tImage.pictures[picture].imageDataSize = tReader.ReadInt32();

		tImage.pictures[picture].headerSize = tReader.ReadInt16();
		tImage.pictures[picture].colorEntries = tReader.ReadInt16();
		tImage.pictures[picture].imageFormat = tReader.ReadByte();
		tImage.pictures[picture].mipmapCount = tReader.ReadByte();
		tImage.pictures[picture].CLUTFormat = tReader.ReadByte();

		//Read in the bitsPerPixel, which must be converted to a bit count.
		byte bDepth = tReader.ReadByte();
		switch (bDepth)
		{
			case 1://16-bit ARGB1555
				tImage.pictures[picture].bitsPerPixel = 16;
				break;
			case 2://24-bit RGB24
				tImage.pictures[picture].bitsPerPixel = 24;
				break;
			case 3://32-bit RGBA32
				tImage.pictures[picture].bitsPerPixel = 32;
				break;
			case 4://4-bit paletted.
				tImage.pictures[picture].bitsPerPixel = 4;
				break;
			case 5://8-bit paletted.
				tImage.pictures[picture].bitsPerPixel = 8;
				break;
		}
		
		//Read in the image dimensions.
		tImage.pictures[picture].imageWidth = tReader.ReadInt16();
		tImage.pictures[picture].imageHeight = tReader.ReadInt16();

		//This is some PS2 specific information.
		tImage.pictures[picture].gsTEX0 = tReader.ReadInt64();
		tImage.pictures[picture].gsTEX1 = tReader.ReadInt64();
		tImage.pictures[picture].gsRegs = tReader.ReadInt32();
		tImage.pictures[picture].gsTexClut = tReader.ReadInt32();

		//Reserved data used by the game developer.
		tImage.pictures[picture].userDataSize = tImage.pictures[picture].headerSize - 0x30;
		if (tImage.pictures[picture].userDataSize > 0)
			tImage.pictures[picture].userData = tReader.ReadBytes(tImage.pictures[picture].userDataSize);

		//Information for parsing the palette and image byte data.
		tImage.pictures[picture].isLinearPalette = (tImage.pictures[picture].CLUTFormat & 0x80) != 0;
		tImage.pictures[picture].CLUTFormat &= 0x7F;
		tImage.pictures[picture].colorSize = ((tImage.pictures[picture].bitsPerPixel > 8) ? (tImage.pictures[picture].bitsPerPixel / 8) : ((tImage.pictures[picture].CLUTFormat & 0x07) + 1));

		//Make sure our image matches the inputs.
		if (tImage.pictures[picture].imageDataSize == tImage.pictures[picture].imageWidth * tImage.pictures[picture].imageHeight)
		{
			//Read in the byte sections for image data and palette.
			tImage.pictures[picture].imageBytes = tReader.ReadBytes(tImage.pictures[picture].imageDataSize);
			tImage.pictures[picture].paletteBytes = tReader.ReadBytes(tImage.pictures[picture].paletteSize);
		}
	}
	
	//Close our input stream.
	input.Close();
	
	//Return our result.
	return tImage;
}


Palette Conversion
---------------------------

//Generic color data.
struct Color
{
	byte r;
	byte g;
	byte b;
	byte a;
}


//Unpacks the bytes at the offset from ABGR1555 2-byte to RGBA 4-byte.
Color FromABGR1555(byte[] tBytes, int nOffset)
{
	//For reference: 2 bytes: A BBBBB GGGGG RRRRR

	//Rip the bits from the 2 byte color and convert them to ABGR bytes.
	int a = ((tBytes[nOffset] & 0x80) >> 7);
	int b = ((tBytes[nOffset] & 0x7C) >> 2);
	int g = (((tBytes[nOffset] & 0x03) << 3) | ((tBytes[nOffset + 1] & 0xE0) >> 5));
	int r = tBytes[nOffset] & 0x1F;

	//Multiply those bytes to get them into 255 color space.
	const float conv = 1.0f / 31.0f;
	float bf = ((float)b * conv) * 255.0f;
	float gf = ((float)g * conv) * 255.0f;
	float rf = ((float)r * conv) * 255.0f;

	//return the converted color.
	Color tOut;
	tOut.r = (byte)rf;
	tOut.g = (byte)gf;
	tOut.b = (byte)bf;
	tOut.a = (byte)a;
	
	return tOut;
}

//Unpacks the entire buffer from ABGR1555 2-byte color data to RGBA 4-byte color data.
Color[] FromBufferABGR1555(byte[] buffer, int size)
{
	//For reference: 2 bytes: A BBBBB GGGGG RRRRR

	//Convert the array using the AGBR1555 function.
	const int stride = 2;
	Color[] tColors = new Color[size / stride];
	for (int p = 0, c = 0; p < size; p += stride, c++)
		tColors[c] = FromABGR1555(buffer, p);
	return tColors;
}

//Unpacks the entire buffer from RGB24 3-byte color data to RGBA 4-byte color data.
Color[] FromBufferRGB24Bit(byte[] buffer, int size)
{
	//For reference: 3 bytes: RRRRRRRR GGGGGGGG BBBBBBBB

	const int stride = 3;
	Color[] tColors = new Color[size / stride];
	for (int p = 0, c = 0; p < size; p += stride, c++)
	{
		tColors[c].a = 255;
		tColoes[c].r = buffer[p];
		tColors[c].g = buffer[p + 1];
		tColors[c].b = buffer[p + 2]);
	}
	return tColors;
}

//Unpacks the entire buffer from RGBA32 4-byte color data to RGBA 4-byte color data.
Color[] FromBufferRGBA32Bit(byte[] buffer, int size)
{
	//NOTE  :   DC2 stores its alpha value at a half intensity(128). You must multiply the alpha by 2 to get correct alpha values. Other games may vary.
	const int DC2_BIT_ALPHA_32_HACK = 2;

	//For reference: 4 bytes: RRRRRRRR GGGGGGGG BBBBBBBB AAAAAAAA
	const int stride = 4;
	Color[] tColors = new Color[size / stride];
	for (int p = 0, c = 0; p < size; p += stride, c++)
	{
		int nAlpha = (int)buffer[p + 3] * BIT_ALPHA_32_HACK;
		if (nAlpha > 255)
			nAlpha = 255;
		if (nAlpha < 0)
			nAlpha = 0;
		
		tColors[c].a = (byte)nAlpha;
		tColoes[c].r = buffer[p];
		tColors[c].g = buffer[p + 1];
		tColors[c].b = buffer[p + 2]);
	}
	return tColors;
}

//Convert the TIM2Picture's palette to linear 32-bit True Color.
Color[] ConvertPicturePaletteToRGBA(TIM2Picture tPicture)
{
	//If there is no palette, return an empty palette.
	if (tPicture.paletteBytes == null || tPicture.paletteBytes.Length == 0)
		return null;

	//Otherwise, we're going to have to figure out which palette we're using based on the paletteColorSize.
	Color[] tPalette;
	if (tPicture.bitsPerPixel <= 8)
	{
		//We'll infer which palette type we're using based on the size of each individual palette entry.
		switch (tPicture.colorSize)
		{
			case 2://16 BIT LE_ABGR_1555?
				{
					tPalette = FromBufferABGR1555(tPicture.paletteBytes, tPicture.paletteSize);
				}
				break;
			case 3://24 BIT RGB?
				{
					tPalette = FromBufferRGB24Bit(tPicture.paletteBytes, tPicture.paletteSize);
				}
				break;
			case 4://32 BIT RGBA?
				{
					tPalette = FromBufferRGBA32Bit(tPicture.paletteBytes, tPicture.paletteSize);
				}
				break;
			default://Invalid data.
				return null;
		}

		//If the palette is interleaved, we need to unpack that into something linear.
		if (tPicture.isLinearPalette == false)
		{
			//The TIM2 palettes always have these fixed values.
			const int nBlockCount = 2;
			const int nStripeCount = 2;
			const int nColorCount = 8;

			//Some calculations for unpacking the palette.
			int nPartCount = tPalette.Length / 32;
			int nPartStride = nColorCount * nStripeCount * nBlockCount;
			int nStripeStride = nStripeCount * nColorCount;

			//This next part was ripped wholesale from the xentax references. I couldn't tell you exactly what's going on, but it appears to work.
			//If I had to guess, TIM2 has a very particular kind of interleave using blocks instead of straight stripes.
			int i = 0;
			Color[] tUnpackedColors = new Color[tPalette.Length];
			for (int part = 0; part < nPartCount; part++)
			{
				for (int block = 0; block < nBlockCount; block++)
				{
					for (int stripe = 0; stripe < nStripeCount; stripe++)
					{
						for (int color = 0; color < nColorCount; color++)
						{
							tUnpackedColors[i++] = tPalette[(part * nPartStride) + (block * nColorCount) + (stripe * nStripeStride) + color];
						}
					}
				}
			}

			//overwrite the original palette, as we don't need it anymore.
			tPalette = tUnpackedColors;
		}
	}
	return tPalette;
}


Image Data Conversion
---------------------------

//Unpacks the 4-bit byte stream into a 8-bit byte stream.
byte[] Unpack4Bit(byte[] buffer, int width, int height)
{
	int d = 0;
	int s = 0;
	byte[] pixels = new byte[width * height];
	for (int y = 0; y < height; y++)
	{
		for (int x = 0; x < (width >> 1); x++)
		{
			byte p = buffer[s++];

			pixels[d++] = (byte)(p & 0xF);
			pixels[d++] = (byte)(p >> 4);
		}
	}
	return pixels;
}

//Packs the 8-bit byte stream into a 4-bit byte stream.
byte[] Pack4Bit(byte[] buffer, int width, int height)
{
	int s = 0;
	int d = 0;
	byte[] result = new byte[width * height];
	for (int y = 0; y < height; y++)
	{
		for (int x = 0; x < (width >> 1); x++)
			result[d++] = (byte)((buffer[s++] & 0xF) | (buffer[s++] << 4));
	}
	return result;
}

//Unswizzles a 4-bit byte stream.
byte[] UnSwizzle4Bit(byte[] buffer, int width, int height)
{
	//Unpack from 4-bit to 8-bit bytes.
	byte[] bUnpacked = Unpack4Bit(buffer, width, height);
	//Unswizzle the 8-bit bytes.
	byte[] bUnswizzled = UnSwizzle(bUnpacked, width, height);
	//Repack from 8-bit to 4-bit bytes.
	return Pack4Bit(bUnswizzled, width, height);
}

//UnSwizzle matrices.
const byte[] InterlaceMatrix = 
{
		0x00, 0x10, 0x02, 0x12,
		0x11, 0x01, 0x13, 0x03,
};

const int[] MatrixY = { 0, 1, -1, 0 };
const int[] TileMatrix = { 4, -4 };

//Unswizzles the byte stream passed.
byte[] UnSwizzle(byte[] buffer, int width, int height)
{
	bool bIsOdd = false;
	int nYOffset, nBufferIndex = 0, nPixelIndex, nInterlaceIndex;
	int nXIndex;
	int nYIndex;
	byte[] newPixels = new byte[width * height];
	for (int y = 0; y < height; y++)
	{
		//Only need to calculate these values once per loop.
		bIsOdd = (y & 1) != 0;
		nYOffset = y * width;
		if (bIsOdd)
			nYOffset -= width;
		
		//Calculate the y pixel offset.
		nYIndex = y + MatrixY[y % 4];

		for (int x = 0; x < width; x++)
		{
			//Calculate the interlace index.
			nInterlaceIndex = ((x >> 2) & 3);
			if (bIsOdd)
				nInterlaceIndex += 4;

			//Calculate the next byte swizzle x-position.
			nXIndex = x + ((y >> 2) & 1) * TileMatrix[((x >> 2) & 1)];

			//Byte unswizzles use an interlace matrix to move the bytes into their swizzle positions(or back out of them).
			nBufferIndex = InterlaceMatrix[nInterlaceIndex] + ((x << 2) & 0x0F) + ((x >> 4) << 5) + nYOffset;
			nPixelIndex = nYIndex * width + nXIndex;

			//If the buffer is not a power of two, the program will crash. We need to quit before this happens. This is not our problem.
			if (nBufferIndex >= buffer.Length)
				return newPixels;//Return whatever we converted.

			//assign the pixel.
			newPixels[nPixelIndex] = buffer[nBufferIndex];
		}
	}
	return newPixels;
}

//Convert the TIM2Picture's image data to linear 32-bit True Color.
Color[] ConvertPictureImageDataToRGBA(TIM2Picture tPicture)
{
	//If there aren't any image bytes, return an empty color array.
	if (tPicture.imageBytes == null || tPicture.imageBytes.Length == 0)
		return null;

	byte[] tImageBytesUnswizzled;
	Color[] tColors;

	//If our data is less than or equal to 8 bytes, that means that it's indexed, and uses the palette. We're going to have to unpack that.
	if (tPicture.bitsPerPixel <= 8)
	{
		//First, we'll go ahead and get the palette.
		Color[] tPalette = ConvertPicturePaletteToRGBA(tPicture);

		//Next, we'll figure out how our byte data is packed.
		switch (tPicture.bitsPerPixel)
		{
			//4 bit image bytes means each byte contains 2 indices of color data.
			case 4:
				{
					//Perform an unswizzle on the entire image to get out usable data we can index with.
					tImageBytesUnswizzled = UnSwizzle4Bit(tPicture.imageBytes, tPicture.imageWidth, tPicture.imageHeight);

					//Unpack the individual 4-bit sections into colors by indexing the palette with their values.
					int nColorCount = tPicture.imageDataSize * 2;
					tColors = new Color[nColorCount];
					for (int c = 0, b = 0; b < tPicture.imageDataSize; b++)
					{
						//Index the palette and retrieve colors.
						tColors[c++] = tPalette[(tImageBytesUnswizzled[b] & 0xF0) >> 4];
						tColors[c++] = tPalette[(tImageBytesUnswizzled[b] & 0x0F)];
					}
				}
				break;
			//8-bit image bytes means each color index is a single byte in size.
			case 8:
				{
					//Perform an unswizzle on the entire image to get out usable data we can index with.
					tImageBytesUnswizzled = UnSwizzle(tPicture.imageBytes, tPicture.imageWidth, tPicture.imageHeight);

					//Unpack the individual indicies into colors by indexing the palette with their values.
					tColors = new Color[tPicture.imageDataSize];
					for (int p = 0; p < tPicture.imageDataSize; p++)
						tColors[p] = tPalette[tImageBytesUnswizzled[p]];//Index the palette and retrieve colors.
				}
				break;
			default://Invalid data.
				return null;
		}
	}
	//True Color. This means that the data is not indexed, and is simply represented by the bytes in the buffer.
	else
	{
		//Perform an unswizzle on the entire image to get out usable data we can index with.
		tImageBytesUnswizzled = UnSwizzle(tPicture.imageBytes, tPicture.imageWidth, tPicture.imageHeight);

		//Switch on the color size to determine what kind of color information we're dealing with.
		switch (tPicture.colorSize)
		{
			case 2://16 BIT LE_ABGR_5551
				{
					tColors = FromBufferABGR1555(tImageBytesUnswizzled, tPicture.imageDataSize);
				}
				break;
			case 3://24 BIT RGB
				{
					tColors = FromBufferRGB24Bit(tImageBytesUnswizzled, tPicture.imageDataSize);
				}
				break;
			case 4://32 BIT RGBA
				{
					tColors = FromBufferRGBA32Bit(tImageBytesUnswizzled, tPicture.imageDataSize);
				}
				break;
			default://Invalid data.
				return null;
		}
	}

	//These images are stored upside down, probably for hardware reasons. We'll flip them back for the end user.
	return tColors;
}

//At this point, you should have enough true color information to convert this to other formats like bmp, targa, png, etc.