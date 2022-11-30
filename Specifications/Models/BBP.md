File Specification:		Level-5 Bind Pose Position
------------------------------------------------------------------------------------------------

Extension:			.bbp/.BBP

Purpose:			contains 4x4 matrices that describe the bind pose positions of the bones within the MDS's skeletal hierarchy

Author:				Level-5(Game Developer, www.level5.co.jp)

Author Date:			2000

Applications:			Dark Cloud 1, Dark Cloud 2

Spec Author:			muddle12

Disclaimer:			This format is speculative. Only the original author knows the exact specification.
	This information was derived through reverse engineering and experimentation. Information may be incorrect or	
	incomplete.

Purpose(expanded):
------------------------------------------------------------------------------------------------

	This is a child format of the MDS format. If you are unfamiliar with the MDS format, I would 
	recommend reading the MDS specification first before reading the BBP specification.

	The BBP, or Bind Pose Position, is a separate file that accompanies the MDS file. This file requires
	an associated MDS file in order to be used.
	
	
	The BBP file contains a list of 4x4 matrices. From the MDS file's header information, there will be 
	boneCount number of matrices in this file. Each bone in the BBP corresponds to a bone in the 
	MDS's skeletal hierarchy, For example, the 10th bone in the BBP will correspond 10th bone in the MDS skeleton.
	
	Each bone in the MDS bone list have an associatedMDTOffset field, which determines whether or not the bone is
	a regular bone, or a container bone. If associatedMDTOffset == 0, it is a regular bone. If associatedMDTOffset != 0,
	it is a container bone which references a specific mesh in the MDS. The associatedMDTOffset is the offset from
	the start of the MDS file to the associated MDT mesh.
	
	Information about associatedMDTOffset is still incomplete, but based on my findings, it appears to indicate that
	the MDT mesh should be transformed by the bind pose matrix if that bone is a container bone(associatedMDTOffset != 0).
	Another example: Given a matrix in the bind pose position file, matching a bone in the MDS bone list, if that MDS bone has an 
	associatedMDTOffset that is non-zero, the vertices of the target mesh should be transformed by that bind pose matrix.
	It is unknown if the normals should be given the same treatment.
	
	The reasoning for this has to do with the default orientation of the meshes in the MDS. Some meshes, such as hats, 
	faces, hands, and other objects are not modeled at their resting positions, but instead sit at the origin. The
	associatedMDTOffset moves these objects to their resting positions, assembling the mesh based on the respective
	bind pose matrix.
	
	If an MDS does not have an accompanying BBP file, then it does not require any corrective mesh transformations.
	
	
	This file's purpose is ultimately unknown. While the breakthrough in understanding the associatedMDTOffset was an
	inspiring victory for reverse engineering the format, it still raises the question as to what the other bind 
	matrices are used for. After all, only 5% of the bones in the MDS's skeletal hierarchy have associatedMDTOffset 
	set. This means that most of the BBP's bind matrices have no purpose, as they have no mesh associated with them. 
	It seems strange to waste this information.
	
	On that note, the MDS skeletal system and the animation system are still only partially understood. I suspect
	the answer lies in the BBP file and its unused data, but I have been unable to make any more headway in this
	area. Anyone else following up on this specification will likely have to look to the BBP for future answers.

File Layout:
---------------------------
```cs
float32 == 4 byte floating point single
eof == end of file

BBP
{
	Matrix4x4(64 bytes)
		float32[16] m;
	Matrix4x4(64 bytes)
	Matrix4x4(64 bytes)
	...
	Matrix4x4(64 bytes)
	eof
}
```

Implementation(pseudocode):
---------------------------
```cs
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

//Loads a BBP file.
Matrix4x4[] LoadBBP(string szBBPFilePath)
{
	//Open our DAT file.
	InputStream input = File.Open(szBBPFilePath);
	
	//Allocate a expandable list/vector of matrices. We do not know how many matrices there will be.
	List<Matrix4x4> tMatrices;//This is the same as vector<Matrix4x4>
	
	//Loop until we reach EOF.
	while(input.EOF() == false)
	{
		//temporary matrix.
		Matrix4x4 tMatrix;
		
		//Read in 16 floats.
		for(int i = 0; i < 16; i++)
			tMatrix.m[i] = input.ReadFloat32();
			
		//Add the matrix to our matrix list.
		tMatrices.Add(tMatrix);
	}
	
	//Close the input stream.
	input.Close();
	
	//Convert our list to an array. This is optional.
	Matrix4x4[] tOutput = new Matrix4x4[tMatrices.Length];
	for(int i = 0; i < tMatrices.Length; i++)
		tOutput[i] = tMatrices[i];
		
	//Return our matrices.
	return tOutput;
}

//Example code for transforming the vertices of an associated mesh using the respective bind matrix.
void TransformMeshVerticesByBindMatrix(reference Vector4[] vertices, Matrix4x4 bindMatrix)
{
	//Temporary variable.
	Vector4 transformedVert;

	//Transform the vertices by the transform passed, moving it to a new position in 3d space.
    for (int vert = 0; vert < vertices.Length; vert++)
	{
		//Perform a vector4 by matrix4x4 multiplication.	 
		transformedVert.x = vertices[vert].x * bindMatrix.m[0] + vertices[vert].y * bindMatrix.m[4] + vertices[vert].z * bindMatrix.m[8] + vertices[vert].w * bindMatrix.m[12];
		
		transformedVert.y = vertices[vert].x * bindMatrix.m[1] + vertices[vert].y * bindMatrix.m[5] + vertices[vert].z * bindMatrix.m[9] + vertices[vert].w * bindMatrix.m[13];
		
		transformedVert.z = vertices[vert].x * bindMatrix.m[2] + vertices[vert].y * bindMatrix.m[6] + vertices[vert].z * bindMatrix.m[10] + vertices[vert].w * bindMatrix.m[14];
		
		transformedVert.w = vertices[vert].x * bindMatrix.m[3] + vertices[vert].y * bindMatrix.m[7] + vertices[vert].z * bindMatrix.m[11] + vertices[vert].w * bindMatrix.m[15];

		//Save this vertex back to the reference array.
		vertices[vert] = transformedVert;
	}
}
```
