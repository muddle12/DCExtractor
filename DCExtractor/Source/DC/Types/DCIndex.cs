namespace DC.Types
{
    /// <summary>
    /// Structure   :   "Index"
    /// 
    /// Purpose     :   describes an index that pairs several lists of information together, and then is strung along to form triangle data.
    /// </summary>
    public struct Index
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Enumerations.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public enum IndexType { Vertex, Normal, UV, Color };


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Data Members.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public int Vertex;      //The index of a vertex in the vertices list.
        public int Normal;      //The index of a normal in the normals list.
        public int UV;          //The index of a uv in the uvs list.
        public int Color;       //The index of a color in the colors list.


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets the index based on the type passed.
        /// </summary>
        /// <param name="eType">The type of index to return.</param>
        /// <returns>The index of a specific type.</returns>
        public int Get(IndexType eType)
        {
            switch (eType)
            {
                case IndexType.Vertex:
                    return Vertex;
                case IndexType.Normal:
                    return Normal;
                case IndexType.UV:
                    return UV;
                case IndexType.Color:
                    return Color;
            }
            return -1;
        }
    }
}