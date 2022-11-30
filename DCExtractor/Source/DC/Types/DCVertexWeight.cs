namespace DC.Types
{
    /// <summary>
    /// Class   :   "VertexWeight"
    /// 
    /// Purpose :   describes an influence by a bone on a specific vertex by a weight value.
    /// </summary>
    public class VertexWeight
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Structures.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public struct VertexWeightBoneHeader
        {
            public int meshBoneIndex;       //This one took a while to figure out. Each bone can either be a bone, or a mesh container. This index references a bone that holds a mesh, and by extension which mesh these weights are targeting.
            public int boneIndex;           //The bone that this header is referencing.
            public int Unknown2;            //Appears to be an offset from the beginning of the header to the headerSize.
            public int headerSize;          //The total size of the header. This is always 32 bytes.
            public int weightCount;         //The number of weights following this header.
            public int nextHeaderOffset;    //The offset from the start of the header to the beginning of the next bone header.
            public int headerMagic1;        //These two magic numbers are always the same, and seem to denote the end of the header.
            public int headerMagic2;
        };

        public struct VertexWeightBlock
        {
            public int vertexIndex;         //The index of the vertex into the weight list this weight applies to.
            public int Unknown1;            //The 6 unknowns in this sequence are always 0. There's a lot of empty space here for some reason. Likely the 32-byte alignment.
            public int Unknown2;
            public int Unknown3;
            public float vertexWeight;      //The weight value, stored as a value between 0f-100f. It's a strange way to store a weight (you could use 0.0f-1.0f, or you know, a byte(0-255)), but whatever.
            public int Unknown4;
            public int Unknown5;
            public int Unknown6;
        };


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Data Members.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Calculated data, not actually representative of the original WGT format.
        public int meshIndex;               //The index of the mesh in the mesh list that this weight affects.
        public int boneIndex;               //The index of the bone this weight is applying the influence of.
        public int vertexIndex;             //The index of the vertex into the vertices list this weight is applying to.
        public float weight;                //The amount of influence to apply from the bone.


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="nMeshIndex">The mesh that this weight is applied to.</param>
        /// <param name="nBoneIndex">The bone that influences this weight.</param>
        /// <param name="nVertexIndex">The vertex that is influenced by this bone.</param>
        /// <param name="fWeight">The weight this bone influences this vertex.</param>
        public VertexWeight(int nMeshIndex, int nBoneIndex, int nVertexIndex, float fWeight)
        {
            meshIndex = nMeshIndex;
            boneIndex = nBoneIndex;
            vertexIndex = nVertexIndex;
            weight = fWeight;
        }
    };
}
