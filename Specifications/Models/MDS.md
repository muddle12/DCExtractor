File Specification:		Level-5 Model
------------------------------------------------------------------------------------------------

Extension:			.mds/.MDS

Purpose:			contains data that describes a 3D model with a skeletal hierarchy. 

Author:				Level-5(Game Developer, www.level5.co.jp)

Author Date:			2000

Applications:			Dark Cloud 1, Dark Cloud 2

Spec Author:			muddle12

Disclaimer:				This format is speculative. Only the original author knows the exact specification.
	This information was derived through reverse engineering and experimentation. Information may be incorrect or	
	incomplete.
	
External References:	https://github.com/ps2dev/ps2gl/blob/master/include/GL/gl.h


Purpose(expanded):
------------------------------------------------------------------------------------------------

	The MDS format is a container for 3D model data with a skeletal hierarchy. It contains
	3D vertex information, 3D normal information, 2D texture coordinate information, floating point vertex color 
	information, surface material information, a variable-size index data section, and a skeletal hierarchy using 4x4 
	matrices as local transformations relative to each bone's parent bone.
	
	Each MDS contains at least one bone in its hierachy, and one or more meshes. 
	
	Most data types are encoded in 4-byte floating point, arranged in a 16-byte four float vector4(x,y,z,w). 
	These types include vertices, normals, texcoords(uvs), and colors. Even if the data type would normally
	be stored in a smaller vector, they will instead be packed into a vector4.
	
	For example: Texture Coordinates are normally two floats in size (u,v), but will be represented in the 
	MDS format as four floats (u,v, unused padding, unused padding). Vertices would normally be vector3(x,y,z), but
	are stored as vector4(x,y,z, unused padding). Colors are stored in vector4s, but it is unknown how they are
	oriented. It is likely they are either (r,g,b,a) or (a,r,g,b). No examples exist of this MDS feature being
	used.
	
	Index data, which will be referred to by terms such as polygon, triangle strip, triangle, and indices, are non-rigid
	data types, and can change in size depending on specific style and type flags. Some indices will contain
	vertex, normal, uv, and color integers. Others will only contain vertex integers. These type names are inferred by the 
	specification author. It is unknown what the original developer's naming scheme was.
	
	As such, I will define these types here: A polygon is a collection of shared triangle strips. A triangle strip is a single
	run of several triangles that are grouped together. A triangle is a collection of three indices that form a triangle
	from three vertices. An index is a collection of anywhere between one to four integers in a vector that refer to
	a specific vertex in the vertices array, an optional specific normal in the normals array, an optional specific 
	texcoord in the texture coordinates array, and an optional specific color in the colors array. An index will
	always have an integer value for vertices.
	
	And yes, my naming scheme is a bit confusing. It's the result of interative reverse engineering.
	
	Materials are the other data type, which contain a texture name and miscellaneous color data. 
	I will go into detail about the structure of materials in a later part of the specification.
	
	
	The MDS file is denoted by the magic value MDS0(0x4D445300). It begins with the MDSHeader, which contains 
	information about the skeletal hierarchy and the list of meshes. This header is 16-bytes, containing four 
	4-byte integers. The first integer is the magic value, the second is version number(which is always 1), the
	third is boneCount(the number of bones in the hierarchy), and the fourth is the boneOffset(offset from the
	beginning of the MDS file to the first bone in the hierarchy, always 16).

	After the MDSHeader is the bone list. Each bone section is 112 bytes in size. There will be boneCount number
	of bone sections following the MDSHeader.
	
	
	Each bone section is divided into two parts, the header and the local matrices. The bone header starts 
	with a 4-byte integer index that denotes the bone's position in the hierarchy. The next 4-byte integer is 
	size of the header(always 112), after this is the boneName, which is a character array section that is 
	32-bytes long, and is null-terminated. Next is a 4-byte integer associatedMDTOffset, see the section below
	for more details. Finally, the last 4-byte integer in the bone header is the parent index, 
	indicating the parent bone of this bone in the hierarchy.
	
	After the bone header is the bone's localMatrix, which is a 64-byte section with a sixteen float 4x4 matrix of 
	transformation data, describing the local transformation of the bone, relative to its parent. The matrix is
	stored in row order(translation along the bottom, scale along the diagonal, and rotation in the top left 3x3).
	
	
	After the last bone should be the first mesh, denoted by the magic MDT0(0x4D445400). There can be one or more
	meshes in an MDS. It is likely you will need to search for MDT0 within the file, as there is no mesh count or
	mesh offset variables to guide you to their location.
	
	Each MDT mesh starts with and MDTHeader. The MDTHeader is 64 bytes in size. The first 4-byte integer is the
	magic value. Then the header size(always 64). Then there is vertexCount, vertexOffset, normalCount, normalOffset,
	colorCount, colorOffset, polygonBlockSize, polygonOffset, uvCount, uvOffset, materialCount, and finally materialOffset.
	These are all 4-byte integers.
	
	Each count denotes the number of that data type contained within the mesh. Each offset denotes the offset in bytes,
	starting at the beginning of the MDTHeader, to the beginning of the relevant data. polygonBlockSize is the
	outlier, as it describes how large the indices section is. This is due to the permutative nature of the indices.
	
	Vertices are held at the vertexOffset from the beginning of the MDTHeader. Each vertex is stored as a 16-byte
	four float vector4(x, y, z, unused). There are exactly vertexCount of them in this section.
	
	Normals are held at the normalOffset from the beginning of the MDTHeader. Each normal is stored as a 16-byte
	four float vector4(x, y, z, unused). There are exactly normalCount of them in this section.
	
	Texture Coordinates/UVs are held at the uvOffset from the beginning of the MDTHeader. Each uv is stored as a 16-byte
	four float vector4(x, y, unused, unused). There are exactly uvCount of them in this section.
	
	Vertex Colors are held at the colorOffset from the beginning of the MDTHeader. Each color is stored as a 16-byte
	four float vector4. It is unknown if these are in rgba or argb format. There are exactly colorCount of them in this section.
	
	
	Polgyons are held at the polygonOffset from the beginning of the MDTHeader. Each polygon begins with a 16-byte header made
	up of four 4-byte integers. The first integer is unknown, and can have a value of 0 or 0xBB000C. The second integer 
	is unknown, but always has a value of 0x10. The third integer is the triangleStripCount, denoting how many triangle 
	strips follow this section. The final integer is unknown, and is always 0.
	
	After the polygon header is a series of triangle strips, of triangleStripCount in number. Each triangle strip begins
	with a 12-byte header, made up of two bytes, a 2-byte short, and two 4-byte integers. 
	
	The first byte is triangleStripStyle, which is how the triangles are arranged in the strip. These are literal 
	OpenGL Primitive Flags used by the Sony Playstation 2's modified OpenGL implementation. See the External References 
	section above for more information. The triangleStripStyle has three common values: GL_TRIANGLES(0x0003), 
	GL_TRIANGLE_STRIP(0x0004), and a third custom style implemented by the developer, which I will refer to 
	as DC_COLLISION_TRIANGLES(0x0013). There are a variety of other values possible for the triangleStripStyle, but they are 
	currently not understood, and are rarely encountered. Due to their cryptic nature, and how they override the 
	triangleStripType value, I thought it better to simply avoid meshes that uses these obscure styles.
	
	The next byte is triangleStripType, which describes how many integers per index are held in the following indices. There are
	four permuntations: VertexNormalUV(0x0), VertexNormalUVColor(0x1), VertexNormal(0x2), and Vertex(0x3). As is implied by
	their names, they denote how many integers per index will be present, and in what order those integers are arranged. As an
	example, VertexNormalUVColor will have four 4-byte indices, in the order of vertex, normal, uv, and color. Vertex
	will only have one 4-byte index, corresponding to a vertex in the vertices array. As a word of warning, this value can
	be overriden by the style. For example, DC_COLLISION_TRIANGLES requires Vertex type, but the type value may not be
	Vertex. You will have to force this type.
	
	After the style and type bytes is a 2-byte short, which is always 0 and it is assumed that it is padding.
	
	The remaining 8 bytes of the triangle strip header are two 4-byte integers, indexCount and materialIndex. indexCount
	refers to number of indices that will follow the triangle strip header. materialIndex refers to the index into the materials
	array, or which material these triangles will use.
	
	Finally, we have the indices section that follows the triangle strip header. There will be indexCount number of
	indices, and they will vary in size based on the triangleStripType. As an example, if the triangleStripType is
	VertexNormal(0x2), and the indexCount is 6, there will be 6 indices, as vectors of two 4-byte integers following
	the triangle strip header. In bytes, this would be 48 bytes of index data.
	
	At the end of this index array will begin another triangle strip header, or if triangleStripCount has been reached,
	another polgyon. This pattern will repeat until the end of the polgyons section has been reached. The end of the
	polygons section is the polygonOffset from the start of the MDTHeader plus the polygonBlockSize 
	(polygonEnd = MDTHeaderStartOffset + MDTHeader.polygonOffset + MDTHeader.polgyonBlockSize).
	
	You may be wondering where the triangle type fits into this. Triangles are not defined literally in the indices, they
	are inferred by the triangleStripStyle. If triangleStripStyle equals GL_TRIANGLES, then the number of triangles in the
	strip are indexCount / 3. If triangleStripStyle equals GL_TRIANGLE_STRIP, then the number of triangles in the
	strip are indexCount - 2. If triangleStripStyle equals DC_COLLISION_TRIANGLES, then the number of triangles in the
	strip are indexCount / 3. If you wish to use this mesh data in another format, you will have to unpack the 
	indices as triangles.
	
	
	Materials are held at the materialOffset from the beginning of the MDTHeader. There are exactly materialCount of 
	them in this section. Each material is 96-bytes in size. The header begins with three colors, each of which are 
	16-byte four float vector4. It is assumed that these are diffuse, specular, and ambinent colors. Following the 
	48-bytes of color data is a single 4-byte float, which is assumed to be a specular roughness or opengl glossiness. 
	Finally, the last 44-bytes is a texture name, represented as a 44-byte character array with null terminators. 
	The material portion of the MDS format is still speculative. One point of intrigue is the rather peculiar 44-byte 
	length of the name field, where name sections are normally 32-byte or 64-byte in other formats created by Level-5.
	
	Materials are the last section of the current MDT mesh. There may be more meshes following the first mesh. Once
	again, you will need to seek to the next MDT0 magic number to find more meshes.
	
	
	associatedMDTOffset: Each bone in the skeletal hierarchy has an associatedMDTOffset value. This value is special,
	as it denotes whether or not this bone is a container for a mesh object. Multiple other formats rely on the
	associatedMDTOffset to direct things like weights and binding data to a mesh. Instead of referencing a mesh directly
	using an index or offset, other formats will use the index of the bone, and then find the mesh attached to that 
	bone via the associatedMDTOffset. It is a rather roundabout way to handle mesh data referencing, but it appears that 
	whenever you see bone indices they can either be pointing to a bone, or a mesh associated with a bone depending
	on the context.
	
	If associatedMDTOffset is zero, the bone functions as a regular bone. If associatedMDTOffset is not zero, it is
	a container for a mesh. It is unclear if mesh container bones also inhabit the skeletal hierarchy, or if they
	are culled entirely.
	
File Layout:
---------------------------
```cs
int32 == 4 byte integer
short16 == 2 byte short integer
float32 == 4 byte floating point single
char == 1 byte ASCII character
long64 = 8 byte integer
eof == end of file

MDS
{
	MDSHeader(16 bytes)
		int32 header;             	//The 0x4D445300 MDS0 header marker.
        	int32 version;             	//The version number. Always 1.
        	int32 boneCount;           	//The number of bones following the initial MDS header.
        	int32 boneOffset;          	//The offset from Offset to the bone section.
	MDSBone(112 bytes)[boneCount]
		int32 index;				//The index of this bone in the hierarchy.                        
		int32 size;                 //The size of this header.     
		char[32] name;              //The name of this bone. Always 32 bytes long, with null terminators.
		int32 associatedMDTOffset;  //The mesh that this bone's bind pose is associated with.
		int32 parentIndex;          //The index of the parent bone.
		Matrix4x4 local;			//The local matrix transformation of the bone.
			float32[16] matrices;		//The matrices of this 4x4 matrix.
	MDTMesh[]						//The mesh data. Unknown number of meshes, search for the header MDT0 to find a mesh.
		int32 magic;                //Magic MDT0
        	int32 headerSize;           //The size of this header. Always 64 bytes.
        	int32 meshDataSize;         //The size of the MDT section, including all mesh data, starting at the header.
        	int32 vertexCount;          //The number of 16-byte vertices in the vertex section.
        	int32 vertexOffset;         //The offset, starting at the beginning of the MDT header, where the vertex data starts.
        	int32 normalCount;          //The number of 16-byte normals in the vertex section.
        	int32 normalOffset;         //The offset, starting at the beginning of the MDT header, where the normal data starts.
        	int32 colorCount;           //The number of 16-byte colors in the vertex section.
        	int32 colorOffset;          //The offset, starting at the beginning of the MDT header, where the color data starts.
        	int32 polygonBlockSize;     //Due to how the index data can change size and structure depending on its type, this describes how large the index section is in bytes.
        	int32 polygonsOffset;       //The offset, starting at the beginning of the MDT header, where the indx data starts.
        	int32 uvCount;              //The number of 16-byte uvs in the uv section.
        	int32 uvOffset;             //The offset, starting at the beginning of the MDT header, where the uv data starts.
        	int32 materialCount;        //The number of 96-byte materials in material section.
        	int32 materialOffset;       //The offset, starting at the beginning of the MDT header, where the material data starts.
		Vector4(16 bytes)[vertexCount] vertices;	//The list of vertices.
			float32 x;						//The x-component.
			float32 y;						//The y-component.
			float32 z;						//The x-component.
			float32 w;						//The w-component.
		Polygon[polygonCount] polygons;	//A collection of triangle strips.
			int32 unknown1;            		//Either 0 or 0xBB000C.  
			int32 unknown2;            		//Always 0x10, possibly polygonHeaderSize.
			int32 triangleStripCount;      	//The number of triangle strips.
			int32 padding;         			//Always 0
			TriangleStrip[triangleStripCount]	triangleStrips;		//A collection of triangle indices.
				byte indexStyle;        			//The type of OpenGL primitive this strip represents. Values: GL_TRIANGLES(0x0003), GL_TRIANGLE_STRIP(0x0004), or DC2_COLLISION_TRIANGLES(0x13)                                                    
				byte indexType;          			//Determines what kind of data is stored in the indices. Values: VertexNormalUV(0x0), VertexNormalUVColor(0x1), VertexNormal(0x2), Vertex(0x3)                                                          
				short16 padding;               		//Always zero, assumed padding.                   
				int32 indexCount;              		//The number of indices in this strip.                                                             
				int32 materialIndex;           		//The index into the material array.                                                                    
				Index[] indices;             		//The list of indices in this strip. WARNING: this structure can change based on indexType.
					if(indexType == VertexNormalUV)
					{
						int32 vertexIndex;			//The index of a vertex in the vertices array.
						int32 normalIndex;			//The index of a normal in the normals array.
						int32 uvIndex;				//The index of a uv in the uv array.
					}
					else if(indexType == VertexNormalUVColor)
					{
						int32 vertexIndex;			//The index of a vertex in the vertices array.
						int32 normalIndex;			//The index of a normal in the normals array.
						int32 uvIndex;				//The index of a uv in the uv array.
						int32 colorIndex;			//The index of a color in the color array.
					}
					else if(indexType == VertexNormal)
					{
						int32 vertexIndex;			//The index of a vertex in the vertices array.
						int32 normalIndex;			//The index of a normal in the normals array.
					}
					else if(indexType == Vertex)
					{
						int32 vertexIndex;			//The index of a vertex in the vertices array.
					}
		Vector4(16 bytes)[normalCount] normals;		//The list of normals.
		Vector4(16 bytes)[uvCount] uvs;			//The list of uvs.
		Vector4(16 bytes)[colorCount] colors;		//The list of colors.
		Material(96 bytes)[materialCount] materials;	//The list of materials.
			Vector4 firstColor;             	//Unknown, I presume this is a diffuse color.
            		Vector4 secondColor;            	//Unknown, possibly a specular or ambinet color.
            		Vector4 thirdColor;             	//Unknown, possibly a specular or ambient color.
            		float32 unknown1;                 	//Unknown, potentially glossiness/roughness.
            		char[44] name;                  	//The name of the texture this material uses.
	eof
}
```

Implementation(pseudocode):
---------------------------
```cs
//MDS magic.
const int32 MDSMagic = 0x4D445300;

//MDT magic.
const int32 MDTMagic = 0x4D445400;

//indexStyles.
const byte GL_TRIANGLES = (byte)0x0003;
const byte GL_TRIANGLE_STRIP = (byte)0x0004;
const byte DC2_COLLISION_TRIANGLES = (byte)0x0013;

//indexTypes.
const byte TYPE_VERTEXNORMALUV = 0x0;
const byte TYPE_VERTEXNORMALUVCOLOR = 0x1;
const byte TYPE_VERTEXNORMAL = 0x2;
const byte TYPE_VERTEX = 0x3;


//A 4x4 matrix.
struct Matrix4x4
{
	float32[16] m;
}

//A 4-component vector.
struct Vector4
{
	float32 x;
	float32 y;
	float32 z;
	float32 w;
}

//A bone in the MDS skeletal hierarchy.
struct MDSBone
{
	int32 index;                    //The index of this bone in the hierarchy.
	int32 size;                     //The size of this header.
	string name;                    //The name of the bone.
	int32 associatedMDTOffset;      //The mesh that this bone's bind pose is associated with.
	int32 parentIndex;              //The index of the parent bone.
	Matrix4x4 local;   		//The local transformation matrix.
	//Matrix4x4 bind;   		//Optional: The bind transformation matrix. See BBP specification.
}

//A material used by the MDS triangles.
struct MDSMaterial
{
	Vector4 firstColor;             //Unknown, I presume this is a diffuse color.
	Vector4 secondColor;            //Unknown, possibly a specular or ambinet color.
	Vector4 thirdColor;             //Unknown, possibly a specular or ambient color.
	float32 unknown1;               //Unknown, potentially glossiness/roughness.
	string name;                    //The name of the material, which matches the corresponding texture associated with this material. This section is 44 bytes long.
}

//An index that forms a triangle.
struct MDSIndex
{
	int32 Vertex;      		//The index of a vertex in the vertices list.
	int32 Normal;      		//The index of a normal in the normals list.
	int32 UV;          		//The index of a uv in the uvs list.
	int32 Color;       		//The index of a color in the colors list.
}

//A collection of indices that form a list of triangles.
struct MDSTriangleStrip
{
	byte indexStyle;        	//The type of OpenGL primitive this strip represents. Values: GL_TRIANGLES(0x0003), GL_TRIANGLE_STRIP(0x0004), or DC2_COLLISION_TRIANGLES(0x13)     
	byte indexType;          	//Determines what kind of data is stored in the indices. Values: VertexNormalUV(0x0), VertexNormalUVColor(0x1), VertexNormal(0x2), Vertex(0x3)      
	short16 padding;               	//Always zero, assumed padding.                   
	int32 indexCount;              	//The number of indices in this strip.                                                             
	int32 materialIndex;           	//The index into the material array.                                                                    
	MDSIndex[] indices;             //The list of indices in this strip. WARNING: this structure can change based on indexType.
}

//A collection of triangle strips.
struct MDSPolygon
{
	int32 unknown1;            		//Either 0 or 0xBB000C.  
	int32 unknown2;            		//Always 0x10, possibly polygonHeaderSize.
	int32 triangleStripCount;  		//The number of triangle strips.
	int32 padding1;         		//Always 0
	MDSTriangleStrip[] triangleStrips;	//The list of triangle strips in this polygon.
}

//A section of mesh data for the MDS.
struct MDSMesh
{
	int32 magic;                 	//Magic MDT0
	int32 headerSize;              	//The size of this header. Always 64 bytes.
	int32 meshDataSize;            	//The size of the MDT section, including all mesh data, starting at the header.
	int32 vertexCount;             	//The number of 16-byte vertices in the vertex section.
	int32 vertexOffset;            	//The offset, starting at the beginning of the MDT header, where the vertex data starts.
	int32 normalCount;             	//The number of 16-byte normals in the vertex section.
	int32 normalOffset;            	//The offset, starting at the beginning of the MDT header, where the normal data starts.
	int32 colorCount;              	//The number of 16-byte colors in the vertex section.
	int32 colorOffset;             	//The offset, starting at the beginning of the MDT header, where the color data starts.
	int32 polygonBlockSize;        	//Due to how the index data can change size and structure depending on its type, this describes how large the index section is in bytes.
	int32 polygonsOffset;          	//The offset, starting at the beginning of the MDT header, where the indx data starts.
	int32 uVCount;                 	//The number of 16-byte uvs in the uv section.
	int32 uVOffset;                	//The offset, starting at the beginning of the MDT header, where the uv data starts.
	int32 materialCount;           	//The number of 96-byte materials in material section.
	int32 materialOffset;          	//The offset, starting at the beginning of the MDT header, where the material data starts.
	Vector4[] vertices;          	//The positional vertex data.
	MDSPolygon[] polygons;		//The triangle indices for this mesh.
	Vector4[] normals;           	//The normal data.
	Vector4[] uvs;               	//The uv texture coordinates.
	Vector4[] colors;            	//The vertex color data.
	MDSMaterial[] materials;        //The material data.
}		

struct MDSModel
{
	int32 header;            	//The 0x4D445300 MDS0 header marker.
	int32 version;           	//The version number. Always 1.
	int32 boneCount;         	//The number of bones following the initial MDS header.
	int32 boneOffset;        	//The offset to the bone section. Also possibly the header size.
	MDSBone[] Bones;            	//A list of bones for the model's hierarchy.
	MDSMesh[] Meshes;           	//A list of meshes that contain the render data for the model.
	//MDSVertexWeight[] Weights;  //Optional: A list of vertex weights in a raw compressed form. They'll need to be propogated to the vertices in order to be used. See WGT specification.
    	//MDSAnimation[] Animations;  //Optional: A list of animations loaded from the motion file and split by the .cfg. See MOT specification.
}
```
```cs
//Loads a mesh. See definition below.
MDSMesh LoadMesh(reference InputStream input);

// Loads the model at the specified path.
MDSModel LoadMDS(string szMDSFilePath)
{
	//Open our MDS file.
	InputStream input = File.Open(szMDSFilePath);

	//Read in the magic value and check it against the MDSMagic.
	int nHeaderMagic = input.ReadInt32();
	if(nHeaderMagic != MDSMagic)
	{
		//Close our input stream and return, this is not an MDS.
		input.Close();
		return;
	}
	
	//Create a new model.
	MDSModel tModel;
	
	//Read in our MDS header.
	tModel.header = MDSMagic;
	tModel.version = input.ReadInt32(); //You may need to swap the endianness here.
	tModel.boneCount = input.ReadInt32();
	tModel.boneOffset = input.ReadInt32();


	//Read in the bones.
	tModel.bones = new MDSBone[tModel.boneCount];
	for (int b = 0; b < tModel.boneCount; b++)
	{
		//Read in the bone header information.
		tModel.bones[b].index = input.ReadInt32();
		tModel.bones[b].size = input.ReadInt32();
		tModel.bones[b].name = input.ReadString(limit=32);
		tModel.bones[b].associatedMDTOffset = input.ReadInt32();
		tModel.bones[b].parentIndex = input.ReadInt32();
		
		//Read in 16 floats.
		for(int i = 0; i < 16; i++)
			tModel.bones[b].local.m[i] = input.ReadFloat32();
	}


	//read in our meshes.
	List<MDSMesh> tMeshes;
	while (input.SeekBytes(MDTMagic))
	{
		//Load a mesh from the file stream.
		MDSMesh tMesh = LoadMesh(input);
		
		//Fail state, if we got back an invalid or improperly loaded mesh, it will cascade and fail the entire model. Bail out.
		if(tMesh == null)
			return null;
		
		//Add the mesh to the list.
		tMeshes.Add(tMesh);
	}
	
	//Convert the mesh list to an array.
	tModel.meshes = new MDSMesh[tMeshes.Length];
	for(int i = 0; i < tMeshes.Length; i++)
		tModel.meshes[i] = tMeshes[i];
	
	
	//Optional: load in the BBP file and apply it. See BBP specification.
	
	//Optional: load in the WGT file. See WGT specification.
	
	//Opional: load in the MOT file. See MOT specification.
	

	//Close our input stream.
	input.Close();
	
	//Return the loaded model.
	return tModel;
}

//Loads a mesh. See definition below.
MDSMesh LoadMesh(reference InputStream input)
{
	//Record the MDTOffset, we'll need it for later.
	long64 nMDTOffset = input.Position;
	
	//Create a mesh.
	MDSMesh tMesh;

	//Load in the MDT header.
	tMesh.magic = input.ReadInt32();
	tMesh.headerSize = input.ReadInt32();
	tMesh.meshDataSize = input.ReadInt32();

	tMesh.vertexCount = input.ReadInt32();
	tMesh.vertexOffset = input.ReadInt32();

	tMesh.normalCount = input.ReadInt32();
	tMesh.normalOffset = input.ReadInt32();

	tMesh.colorCount = input.ReadInt32();
	tMesh.colorOffset = input.ReadInt32();

	tMesh.polygonBlockSize = input.ReadInt32();
	tMesh.polygonsOffset = input.ReadInt32();

	tMesh.uVCount = input.ReadInt32();
	tMesh.uVOffset = input.ReadInt32();

	tMesh.materialCount = input.ReadInt32();
	tMesh.materialOffset = input.ReadInt32();

	//If we have vertices, load them at the offset.
	if (tMesh.vertexCount > 0)
	{
		input.Position = nMDTOffset + tMesh.vertexOffset;
		tMesh.vertices = new Vector4[tMesh.vertexCount];
		for (int v = 0; v < tMesh.vertexCount; v++)
		{
			tMesh.vertices[v].x = input.ReadFloat32();
			tMesh.vertices[v].y = input.ReadFloat32();
			tMesh.vertices[v].z = input.ReadFloat32();
			tMesh.vertices[v].w = input.ReadFloat32();
		}
	}

	//If we have normals, load them at the offset.
	if (tMesh.normalCount > 0)
	{
		input.Position = nMDTOffset + tMesh.normalOffset;
		tMesh.normals = new Vector4[tMesh.normalCount];
		for (int n = 0; n < tMesh.normalCount; n++)
		{
			tMesh.normals[n].x = input.ReadFloat32();
			tMesh.normals[n].y = input.ReadFloat32();
			tMesh.normals[n].z = input.ReadFloat32();
			tMesh.normals[n].w = input.ReadFloat32();
		}
	}

	//If we have uvs, load them at the offset.
	if (tMesh.uvCount > 0)
	{
		input.Position = nMDTOffset + tMesh.uvOffset;
		tMesh.uvs = new Vector4[tMesh.uvCount];
		for (int uv = 0; uv < tMesh.uvCount; uv++)
		{
			tMesh.uvs[uv].x = input.ReadFloat32();
			tMesh.uvs[uv].y = input.ReadFloat32();
			tMesh.uvs[uv].z = input.ReadFloat32();
			tMesh.uvs[uv].w = input.ReadFloat32();
		}
	}

	//If we have colors, load them at the offset.
	if (tMesh.colorCount > 0)
	{
		input.Position = nMDTOffset + tMesh.colorOffset;
		tMesh.colors = new Vector4[tMesh.colorCount];
		for (int color = 0; color < tMesh.colorCount; color++)
		{
			tMesh.colors[color].x = input.ReadFloat32();
			tMesh.colors[color].y = input.ReadFloat32();
			tMesh.colors[color].z = input.ReadFloat32();
			tMesh.colors[color].w = input.ReadFloat32();
		}
	}

	//If we have materials, we load them at the offset.
	if (tMesh.materialCount > 0)
	{
		input.BaseStream.Position = nMDTOffset + tMesh.materialOffset;
		tMesh.Materials = new MDSMaterial[tMesh.materialCount];
		for (int i = 0; i < tMesh.materialCount; i++)
		{
			tMesh.Materials[i].firstColor.x = input.ReadFloat32();
			tMesh.Materials[i].firstColor.y = input.ReadFloat32();
			tMesh.Materials[i].firstColor.z = input.ReadFloat32();
			tMesh.Materials[i].firstColor.w = input.ReadFloat32();
			
			tMesh.Materials[i].secondColor.x = input.ReadFloat32();
			tMesh.Materials[i].secondColor.y = input.ReadFloat32();
			tMesh.Materials[i].secondColor.z = input.ReadFloat32();
			tMesh.Materials[i].secondColor.w = input.ReadFloat32();
			
			tMesh.Materials[i].thirdColor.x = input.ReadFloat32();
			tMesh.Materials[i].thirdColor.y = input.ReadFloat32();
			tMesh.Materials[i].thirdColor.z = input.ReadFloat32();
			tMesh.Materials[i].thirdColor.w = input.ReadFloat32();
			
			tMesh.Materials[i].unknown1 = input.ReadFloat32();
			tMesh.Materials[i].name = input.ReadString(limit=44);
		}
	}

	//If we have faces, load them at the offset.
	if (tMesh.PolygonBlockSize > 0)
	{
		//Faces are weird, we need to know then end of the buffer since faces can have varying size. We calculate this offset.
		long64 nFaceBufferEnd = nMDTOffset + tMesh.polygonsOffset + tMesh.polygonBlockSize;

		//Start at the beginning of the face buffer.
		input.Position = nMDTOffset + tMesh.polygonsOffset;
		while (input.Position < nFaceBufferEnd)
		{
			MDSPolygon tPolygon;

			//Read the polygon header. This header contains information about the following strip of strips.
			tPolygon.unknown1 = input.ReadInt32();
			tPolygon.unknown2 = input.ReadInt32();
			tPolygon.triangleStripCount = input.ReadInt32();
			tPolygon.padding1 = input.ReadInt32();

			//For each strip.
			tPolygon.triangleStrips = new MDSTriangleStrip[tPolygon.triangleStripCount];
			for (int strip = 0; strip < tPolygon.triangleStripCount; strip++)
			{
				//Get the header for the strip.
				tPolygon.triangleStrips[strip].indexStyle = input.ReadByte();
				tPolygon.triangleStrips[strip].indexType = input.ReadByte();
				tPolygon.triangleStrips[strip].padding = input.ReadInt16();
				tPolygon.triangleStrips[strip].indexCount = input.ReadInt32();
				tPolygon.triangleStrips[strip].materialIndex = input.ReadInt32();

				//In the weird case where our incoming primitive is a collision mesh, the index data can only be of type Vertex.
				if (tPolygon.triangleStrips[strip].indexType == DC2_COLLISION_TRIANGLES)
					eType = TYPE_VERTEX;
				
				//Since the specification currently doesn't support the other edge case styles, we'll bail on this if we encounter something unknown.
				if(tPolygon.triangleStrips[strip].indexType != GL_TRIANGLES ||
					tPolygon.triangleStrips[strip].indexType != GL_TRIANGLE_STRIP ||
					tPolygon.triangleStrips[strip].indexType != DC2_COLLISION_TRIANGLES)
					return null;
				
				//At the end of the header, we loop through the indicies and grab the relevant indicies.
				tPolygon.triangleStrips[strip].indices = new MDSIndex[tPolygon.triangleStrips[strip].indexCount];
				for (int index = 0; index < tPolygon.triangleStrips[strip].indexCount; index++)
				{
					if(tPolygon.triangleStrips[strip].indexType == TYPE_VERTEXNORMALUV)
					{
						tPolygon.triangleStrips[strip].indices[index].vertex = input.ReadInt32();
						tPolygon.triangleStrips[strip].indices[index].normal = input.ReadInt32();
						tPolygon.triangleStrips[strip].indices[index].uv = input.ReadInt32();
					}
					else if(tPolygon.triangleStrips[strip].indexType == TYPE_VERTEXNORMALUVCOLOR)
					{
						tPolygon.triangleStrips[strip].indices[index].vertex = input.ReadInt32();
						tPolygon.triangleStrips[strip].indices[index].normal = input.ReadInt32();
						tPolygon.triangleStrips[strip].indices[index].uv = input.ReadInt32();
						tPolygon.triangleStrips[strip].indices[index].color = input.ReadInt32();
					}
					else if(tPolygon.triangleStrips[strip].indexType == TYPE_VERTEXNORMAL)
					{
						tPolygon.triangleStrips[strip].indices[index].vertex = input.ReadInt32();
						tPolygon.triangleStrips[strip].indices[index].normal = input.ReadInt32();
					}
					else if(tPolygon.triangleStrips[strip].indexType == TYPE_VERTEX)
					{
						tPolygon.triangleStrips[strip].indices[index].vertex = input.ReadInt32();
					}
				}
			}
			
			//Add this polygon to our list.
			tMesh.Polygons.Add(tPolygon);
		}
	}

	//Return the loaded mesh.
	return tMesh;
}
```
