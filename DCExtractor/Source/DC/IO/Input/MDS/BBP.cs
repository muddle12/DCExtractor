using Custom.IO;
using Custom.Math;
using System.Collections.Generic;
using System.IO;

namespace DC.IO
{
    /// <summary>
    /// Class   :   "BBP"
    /// 
    /// Purpose :   provides functions for loading Level-5's BBP format.
    /// 
    /// It's just a list of 4x4 16 float matrices in row order. There should always be as many matrices in the bbp as there are bones in the mds.
    /// 
    /// 
    /// I have no idea what the purpose of this format is. It appears to be 4x4 matrix data that matches the number of bones found in the MDS file.
    /// My best guess is that it stands for "Bone Bind Position", which would imply that it's related to the bind pose for animating meshes.
    /// 
    /// What's mysterious about this format is that most of the matrices don't appear to be used. The only case I could find was for very specific matrices who
    /// have their associatedMDTOffset value checked on the associated bone. In this particular instance, the matrix data is an offset by which to transform the
    /// vertices of their matching mesh, moving it from its origin position to a binding position elsewhere on the model. This corrects stuff like hands/face/hats
    /// being at the origin of the model.
    /// 
    /// In my experience, when performing skeletal skinning, you don't need the bind pose matrices to be stored separate, as you can derive them at runtime by
    /// multiplying the parent and child bones world/local matrices together to get your binds.
    /// 
    /// The BBP rotations are fishy. I don't know exactly what purpose the BBP serves, and after loading it with
    /// a dozen different transformations and multiplication orders, I'm at a loss for what the bind data means.
    /// 
    /// This format right here might hold the secret as to why the tool's animation data won't export correctly, but I just can't figure out what the issue is. I'll leave it
    /// to someone else.
    /// 
    /// 
    /// If you are viewing bbp data with a hex editor, it is best viewed at a 16 bytes per row. 
    /// </summary>
    public static class BBP
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Loads a bone bind pose file.
        /// </summary>
        /// <param name="szBBPPath">The path to the bone bind pose file.</param>
        /// <returns>A list of bone matrices that signify the starting positions of the bones for the model's bind pose.</returns>
        public static Matrix4x4[] Load(string szBBPPath)
        {
            //Open a file stream to read in our data.
            BinaryReader tReader = FileStreamHelpers.OpenBinaryReader(szBBPPath);

            //The file format is extremely simple, it's just matrices made up of 16 floats stored in row-order.
            List<Matrix4x4> tMatrices = new List<Matrix4x4>();
            while (tReader.BaseStream.Position < tReader.BaseStream.Length)
                tMatrices.Add(tReader.ReadMatrix());

            //Close the stream.
            tReader.Close();

            //Return the result.
            return tMatrices.ToArray();
        }
    }
}
