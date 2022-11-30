namespace DC.Types
{
    /// <summary>
    /// Structure   :   "TriangleStrip"
    /// 
    /// Purpose     :   contains a list of triangles that are organized in a certain pattern depending on the style.
    /// </summary>
    public struct TriangleStrip
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Enumerations.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Style is OpenGL primitive flags:
        /// The ps2 used a modified opengl. This is the header for it.
        /// https://github.com/ps2dev/ps2gl/blob/master/include/GL/gl.h
        /// </summary>
        public enum PrimitiveStyle : byte
        {
            GL_POINTS = 0x0000,
            GL_LINES = 0x0001,
            GL_LINE_STRIP = 0x0002,
            GL_TRIANGLES = 0x0003,          //The buffer contains a continguous list of individual non-overlapping triangle data.
            GL_TRIANGLE_STRIP = 0x0004,     //The buffer contains a continguous list of triangles that are formatted with overlapping index data to form a triangle strip.
            GL_TRIANGLE_FAN = 0x0005,
            GL_QUADS = (0x0008 | GL_TRIANGLE_STRIP),
            GL_QUAD_STRIP = (0x0018 | GL_TRIANGLE_STRIP),
            GL_POLYGON = (0x0008 | GL_TRIANGLE_FAN),
            GL_LINE_LOOP = GL_LINE_STRIP,
            DC2_COLLISION_TRIANGLES = 19        //I have no idea, I'm just inferring this based on what I'm seeing in the binary view. It definitely looks like regular triangles when we export.
        }

        /// <summary>
        /// Describes what kinds of index data is stored in the indices. Each index is 4 bytes unsigned.
        /// 
        /// Example : VertexNormalUV is a single vertex in a triangle that contains 
        /// 12 bytes of index data(1 4-byte vertex position index, 1 4-byte normal index, and 1 4-byte uv index).
        /// 
        /// Vertex = the index of the vertex in the vertices list.
        /// Normal = the index of the normal in the normals list.
        /// UV = the index of the uv in the uvs list.
        /// Color = the index of the color in the colors list.
        /// </summary>
        public enum PrimitiveType : byte
        {
            VertexNormalUV = 0,
            VertexNormalUVColor = 1,
            VertexNormal = 2,
            Vertex = 3                  //This guy was especially tricky to deduce, but ultimately I pegged this as collision data.
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Data Members.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PrimitiveStyle Style;        //The type of OpenGL primitive this strip represents. See the enum above.                                                        
        public PrimitiveType Type;          //Determines what kind of data is stored in the indices. See the enum above.                                                       
        public short Unused1;               //This is always zero, period. There are no permutations.                     
        public int IndexCount;              //The number of indices in this strip.                                                             
        public int MaterialIndex;           //Denotes which material is applied to this triangle strip.                                                                        

        public Index[] Indices;             //The list of indices in this strip.


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Clones the triangle strip.
        /// </summary>
        /// <returns>A copy of the strip.</returns>
        public TriangleStrip Clone()
        {
            TriangleStrip tStrip = new TriangleStrip();

            tStrip.Style = Style;
            tStrip.Type = Type;
            tStrip.Unused1 = Unused1;
            tStrip.IndexCount = IndexCount;
            tStrip.MaterialIndex = MaterialIndex;

            tStrip.Indices = new Index[Indices.Length];
            for (int index = 0; index < Indices.Length; index++)
                tStrip.Indices[index] = Indices[index];

            return tStrip;
        }
    }
}
