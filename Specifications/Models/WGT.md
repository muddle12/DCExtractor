*------------------------------------------------------------------------------------------------*

File Specification:		Level-5 Vertex Weights

Extension:			.wgt/.WGT

Purpose:			contains data that defines a weight map for skeletal mesh skinning.

Author:				Level-5(Game Developer, www.level5.co.jp)

Author Date:			2000

Applications:			Dark Cloud 1, Dark Cloud 2

Spec Author:			muddle

Disclaimer:				This format is speculative. Only the original author knows the exact specification.
	This information was derived through reverse engineering and experimentation. Information may be incorrect or	
	incomplete.

*------------------------------------------------------------------------------------------------*

Purpose(expanded):		This is a child format of the MDS format. If you are unfamiliar with the MDS format, I would 
	recommend reading the MDS specification first before reading the WGT specification.
	
	The WGT is a separate file that accompanies the MDS file. This file requires an associated MDS file in order to be used.
	
	
	The WGT format is a container for vertex weights. Weights are used by a skeletal mesh skinning system. Mesh skinning is the
	process of transforming vertices around a local skeletal bone, often called a joint. The "weights" are influence values that govern
	how much a vertex should move with respect to its associated bone. Multiple weights can be applied to the same
	vertex, allowing meshes to bend, stretch, and compress around the skeletal hierarchy, giving the impression that
	that the polygons are "skin" affixed to a skeletal structure. More details about mesh skinng will not be elaborated on in 
	this specification. I would advise researching this topic if you are not familiar with the terminology.
	

	The WGT Format is divided into two structures: BoneHeaders and VertexWeights. Both structures are 32 bytes in size.
	For each BoneHeader, there will be zero or more VertexWeights in a list following the header. Then another BoneHeader,
	then zero or more VertexWeights, and so on and so forth until end of file.
	
	
	The BoneHeader describes which bone the following weights will be influenced by. It contains eight 4-byte integers. 
	
	The first integer is the meshBoneIndex, which points to a specific mesh, using a specific bone's associatedMDTOffset. In simplier
	terms, meshBoneIndex is the index of a bone, which has an associatedMDTOffset, which points to a specific mesh, which the
	following weights will target. It's basically a daisy-chain to get to a mesh. Vertex weights are not global to the MDS, but 
	rather local to specific MDTMeshes in the file. A set of weights for a BoneHeader will only target one specific mesh at a time.
	
	The second integer is the boneIndex, which is the index of the bone in the MDS skeletal hierarchy that the weights will be influenced by.
	The third integer is padding. The fourth integer is the headerSize, which is always 32 bytes. The fifth integer is the
	weightCount, or the number of weights following this BoneHeader. The sixth integer is the nextHeaderOffset, which is the offset,
	starting at the beginning of this BoneHeader, to the next BoneHeader. This allows you to skip the vertex weights for a
	BoneHeader, though I'm not sure why one would do that. The final two integers are magic values, 0xACE63701 and 0xB0F0FC77.
	
	
	After the BoneHeader, if weightCount is not zero, will be an array of VertexWeights of weightCount in length. Each
	VertexWeight is 32 bytes in size. It begins with a 4-byte integer vertexIndex, which is an index into the vertices array
	of the BoneHeader's meshBoneIndex's associated target mesh. The next 12 bytes are padding. After this is a 4-byte float
	vertexWeight, which is the amount of influence this bone has on the vertex. This value ranges from 0.0f - 100.0f. It is
	assumed that 100.0f == 1.0f, or 50.0f == 0.5f. It is unknown why these values are stored in numerals rather than fractions.
	Finally, the last 12 bytes are padding. After the following VertexWeights of weightCount in number, begins another 
	BoneHeader or end of file.
	
*------------------------------------------------------------------------------------------------*

File Layout:
---------------------------
int32 == 4 byte integer
float32 == 4 byte floating point single
eof == end of file

WGT
{
	BoneHeader(32 bytes)
	    int32 meshBoneIndex;			//A reference to the bone that references the target mesh of the following weights.
        int32 boneIndex;				//The index of the bone the following weights are influenced by.
        int32 Unknown2;					//Unknown.
        int32 headerSize;				//The size of this header, always 32.
        int32 weightCount;				//The number of weights following this header.
        int32 nextHeaderOffset;			//The offset, from the beginning of the header, to the next BoneHeader.
        int32 headerMagic1;				//A magic value.
        int32 headerMagic2;				//A magic value.
		VertexWeight(32 bytes)[weightCount]		//The list of weights for this bone.
			int32 vertexIndex;         	//The index of the vertex into the weight list this weight applies to.
			int32 Unknown1;            	//The 6 unknowns in this sequence are always 0. There's a lot of empty space here for some reason. Likely the 32-byte alignment.
			int32 Unknown2;
			int32 Unknown3;
			float32 vertexWeight;      	//The weight value, stored as a value between 0f-100f. It's a strange way to store a weight (you could use 0.0f-1.0f, or you know, a byte(0-255)), but whatever.
			int32 Unknown4;
			int32 Unknown5;
			int32 Unknown6;
	BoneHeader(32 bytes)
	BoneHeader(32 bytes)
	...
	BoneHeader(32 bytes)
	eof
}

*------------------------------------------------------------------------------------------------*

Implementation(pseudocode):
---------------------------

//A weight upon a vertex.
struct WGTVertexWeight
{
	int32 vertexIndex;
	int32 Unknown1;
	int32 Unknown2;
	int32 Unknown3;
	float32 vertexWeight;
	int32 Unknown4;
	int32 Unknown5;
	int32 Unknown6;
}

//The bone header that describes which mesh and bone will affect the following weights.
struct WGTBoneHeader(32 bytes)
{
	int32 meshBoneIndex;
	int32 boneIndex;
	int32 Unknown2;
	int32 headerSize;
	int32 weightCount;
	int32 nextHeaderOffset;
	int32 headerMagic1;
	int32 headerMagic2;
	WGTVertexWeight[] weights;
}

//A simple structure for organzing the weight data. This is optional.
struct WGTSkin
{
	WGTBoneHeader[] bones;
};

//Loads a WGT and returns the weight data.
WGTSkin LoadWGT(string szWGTFilePath)
{
	//Open our WGT file.
	InputStream input = File.Open(szMOTFilePath);
	
	//Temporary variables.
	WGTBoneHeader tHeader;
	List<WGTBoneHeader> tBoneHeaderList;
	
	//Loop until we reach EOF.
	while(input.EOF() == false)
	{
		//Read in the header information.
		tHeader.meshBoneIndex = input.ReadInt32();
		tHeader.boneIndex = input.ReadInt32();
		tHeader.Unknown2 = input.ReadInt32();
		tHeader.headerSize = input.ReadInt32();
		tHeader.weightCount = input.ReadInt32();
		tHeader.nextHeaderOffset = input.ReadInt32();
		tHeader.headerMagic1 = input.ReadInt32();
		tHeader.headerMagic2 = input.ReadInt32();
		
		//If there are weights in this section, we'll need to load them.
		if(tHeader.weightCount > 0)
		{
			//Allocate a new array of weights.
			tHeader.weights = new WGTVertexWeight[tHeader.weightCount];
			
			//Loop through all of the weights and read them.
			for(int w = 0; w < tHeader.weightCount; w++)
			{			
				//Read in the weight data.
				tHeader.weights[w].vertexIndex = input.ReadInt32();
				tHeader.weights[w].Unknown1 = input.ReadInt32();
				tHeader.weights[w].Unknown2 = input.ReadInt32();
				tHeader.weights[w].Unknown3 = input.ReadInt32();
				tHeader.weights[w].vertexWeight = input.ReadFloat32() * 0.01f; //Values are stored as 0.0f-100.0f, We'll go ahead and normalize this to a value between 0.0f-1.0f by dividing by 100.0f or multiplying by 0.01f.
				tHeader.weights[w].Unknown4 = input.ReadInt32();
				tHeader.weights[w].Unknown5 = input.ReadInt32();
				tHeader.weights[w].Unknown6 = input.ReadInt32();
			}
		}
		
		//Add our header to the list.
		tBoneHeaderList.Add(tHeader);
	}
	
	//Close our input stream.
	input.Close();
	
	//Allocate a new skin.
	WGTSkin tSkin;
	
	//Allocate a new list of bone headers.
	tSkin.bones = new WGTBoneHeader[tBoneHeaderList.Length];
	
	//Convert the list into an array.
	for(int header = 0; header < tBoneHeaderList.Length; header++)
		tSkin.bones[header] = tBoneHeaderList[header];

	//Return the skin.
	return tSkin;
}