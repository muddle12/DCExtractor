using Custom.IO;
using DC.Types;
using System.Collections.Generic;
using System.IO;

namespace DC.IO
{
    /// <summary>
    /// Class   :   "WGT"
    /// 
    /// Purpose :   provides functions for loading Level-5's WGT format.
    /// 
    /// A rather confusing format at first, mainly because of the alignment not being 16-bytes, which I was used to because of working in MDS for a month.
    /// 
    /// The format is rather bloated, containing a lot of padded data. Most of it can be skipped, as long as you know where the relevant data is kept.
    /// 
    /// Weight data is divided into two types, bone headers, and vertex weight data.
    /// 
    /// Bone headers contain the information about the current mesh and the current bone that the following weights are going to affect. You'll see one at the beginning of the 
    /// WGT file. Bone Headers can reference zero weights, meaning multiple Bone Headers can exist back to back with no weight data in between.
    /// 
    /// Each Bone Header references a mesh in the MDS's mesh list by index, in the order that meshes (MDT) are listed in the MDS. The bone index references the bone list,
    /// which is also sequential in the order they are listed in the MDS.
    /// 
    /// Vertex Weights are listed after a Bone Header that has its weight count set to something other than zero. Vertex Weights contain the index of
    /// the vertex they affect, and the influence amount.
    /// 
    /// Fun fact, the influences are stored in floating point, but are set to values that range from 0.0f-100.0f. I'm not sure why you'd use floating point and then
    /// simply not use the fractional portion of the float. This was likely a mistake early in development that stuck for the rest of this engine's life cycle.
    /// Alternatively, to give the developers some credit, perhaps there is a PS2 reason this data is kept in this format. No judgement. I know all too well that
    /// engine development is half hindsight.
    /// 
    /// 
    /// If you are viewing weight data with a hex editor, it is best viewed at a 32 bytes per row. 
    /// </summary>
    public static class WGT
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Loads a weight file containing the vertex weights for the model.
        /// </summary>
        /// <param name="szWGTPath">The path to the weight file.</param>
        /// <returns>A list of vertex weights.</returns>
        public static VertexWeight[] Load(string szWGTPath)
        {
            BinaryReader tReader = FileStreamHelpers.OpenBinaryReader(szWGTPath);
            if (tReader == null)
                return null;

            List<VertexWeight> tWeights = new List<VertexWeight>();

            VertexWeight.VertexWeightBoneHeader tBoneHeader;
            while (tReader.BaseStream.Position < tReader.BaseStream.Length)
            {
                //Read in the header.
                tReader.ReadVertexWeightBoneHeader(out tBoneHeader);

                if (tBoneHeader.weightCount > 0)
                {
                    //Read in weights based on the weight count.
                    for (int weight = 0; weight < tBoneHeader.weightCount; weight++)
                        tWeights.Add(tReader.ReadVertexWeight(tBoneHeader.meshBoneIndex, tBoneHeader.boneIndex));
                }
            }
            tReader.Close();

            return tWeights.ToArray();
        }

        /// <summary>
        /// Reads a vertex weight bone header from a wgt file.
        /// </summary>
        /// <param name="tReader">The input stream.</param>
        /// <param name="tHeader">The header to recieve the data being loaded.</param>
        static void ReadVertexWeightBoneHeader(this BinaryReader tReader, out VertexWeight.VertexWeightBoneHeader tHeader)
        {
            tHeader.meshBoneIndex = tReader.ReadInt32();
            tHeader.boneIndex = tReader.ReadInt32();
            tHeader.Unknown2 = tReader.ReadInt32();
            tHeader.headerSize = tReader.ReadInt32();
            tHeader.weightCount = tReader.ReadInt32();
            tHeader.nextHeaderOffset = tReader.ReadInt32();
            tHeader.headerMagic1 = tReader.ReadInt32();
            tHeader.headerMagic2 = tReader.ReadInt32();
        }

        /// <summary>
        /// Reads a single vertex line from the wgt file.
        /// </summary>
        /// <param name="tReader">The input stream.</param>
        /// <param name="nMeshIndex">The mesh this weight is applied to.</param>
        /// <param name="nBoneIndex">The bone influencing this weight.</param>
        /// <returns>A vertex weight.</returns>
        static VertexWeight ReadVertexWeight(this BinaryReader tReader, int nMeshIndex, int nBoneIndex)
        {
            VertexWeight.VertexWeightBlock tWeightBlock;
            tWeightBlock.vertexIndex = tReader.ReadInt32();
            tReader.BaseStream.Position += 12;//Skip.
            //tWeightBlock.Unknown1 = tReader.ReadInt32();
            //tWeightBlock.Unknown2 = tReader.ReadInt32();
            //tWeightBlock.Unknown3 = tReader.ReadInt32();
            tWeightBlock.vertexWeight = tReader.ReadSingle();
            tReader.BaseStream.Position += 12;//Skip.
            //tWeightBlock.Unknown4 = tReader.ReadInt32();
            //tWeightBlock.Unknown5 = tReader.ReadInt32();
            //tWeightBlock.Unknown6 = tReader.ReadInt32();

            /************************************************************************/
            //  As referenced above, the weight influences are stored as values
            //  ranging between 0.0f-100.0f. They'll need to be normalized to 0.0f-1.0f
            //  range, by simply multiplying by 0.01f;
            /************************************************************************/
            const float WeightConversion = 0.01f;

            //Normalize the weight from 0-100 to 0-1 by dividing by 100.
            return new VertexWeight(nMeshIndex, nBoneIndex, tWeightBlock.vertexIndex, tWeightBlock.vertexWeight * WeightConversion);
        }
    }
}
