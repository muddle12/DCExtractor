using Custom.Math;
using System.Collections.Generic;

namespace DC.Types
{
    /// <summary>
    /// Class   :   "Mesh"
    /// 
    /// Purpose :   describes a section of polygons that correspond to a part of a model.
    /// </summary>
    public class Mesh
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Classes.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Class   :   "Material"
        /// 
        /// Purpose :   describes the rendering surface of a polygon.
        /// </summary>
        public struct Material
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Constants.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /********************************************************/
            //  I imagine there is more data in the materials after
            //  the name, but it always appears to be zero. For now,
            //  I'm going to assume they were putting file system
            //  paths in the name, hence why it needs 44 bytes rather
            //  than the 32 they normally allocate for other names in
            //  other parts of the format. I could be wrong, though.
            /********************************************************/
            public const int NameLength = 44;       


            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Data Members.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            public Vector4 firstColor;              //Unknown, I presume this is a diffuse color.
            public Vector4 secondColor;             //Unknown, possibly a specular or ambinet color.
            public Vector4 thirdColor;              //Unknown, possibly a specular or ambient color.
            public float unknown1;                  //Unknown, potentially glossiness/roughness.
            public string name;                     //The name of the material, which matches the corresponding texture associated with this material. This section is 44 bytes long.


            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Functions.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Clones the material.
            /// </summary>
            /// <returns>A copy of the material.</returns>
            public Material Clone()
            {
                Material tCopy = new Material();
                tCopy.firstColor = firstColor;
                tCopy.secondColor = secondColor;
                tCopy.thirdColor = thirdColor;
                tCopy.unknown1 = unknown1;
                tCopy.name = name;
                return tCopy;
            }
        };


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Data Members.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //public int Magic;                 //Magic MDT0

        public int HeaderSize;              //The size of this header. Always 64 bytes.
        public int MeshDataSize;            //The size of the MDT section, including all mesh data, starting at the header.

        public int VertexCount;             //The number of 16-byte vertices in the vertex section.
        public int VertexOffset;            //The offset, starting at the beginning of the MDT header, where the vertex data starts.

        public int NormalCount;             //The number of 16-byte normals in the vertex section.
        public int NormalOffset;            //The offset, starting at the beginning of the MDT header, where the normal data starts.

        public int ColorCount;              //The number of 16-byte colors in the vertex section.
        public int ColorOffset;             //The offset, starting at the beginning of the MDT header, where the color data starts.

        public int PolygonBlockSize;        //Due to how the index data can change size and structure depending on its type, this describes how large the index section is in bytes.
        public int PolygonsOffset;          //The offset, starting at the beginning of the MDT header, where the indx data starts.

        public int UVCount;                 //The number of 16-byte uvs in the uv section.
        public int UVOffset;                //The offset, starting at the beginning of the MDT header, where the uv data starts.

        public int MaterialCount;           //The number of 96-byte materials in material section.
        public int MaterialOffset;          //The offset, starting at the beginning of the MDT header, where the material data starts.

        public Vector4[] Vertices;          //The positional vertex data.
        public Vector4[] Normals;           //The normal data.
        public Vector4[] UVs;               //The uv texture coordinates.
        public Vector4[] Colors;            //The vertex color data.
        public Material[] Materials;        //The material data.

        public List<Polygon> Polygons = new List<Polygon>();    //The list of polygons generated from the index section.

        //Calculated data. 
        //This is a peculiar one. We need to grab the offset of this mdt in the file in order to associate the weights and bind poses.
        public long MDTOffset;
        public int MeshBoneIndex;


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Shifts all vertices by the transform.
        /// </summary>
        /// <param name="transform">The transform to apply to the vertices.</param>
        public void TransformVertices(Matrix4x4 transform)
        {
            //This is technically a hack. We don't want to destroy any useful geometry just because the devs wanted it to be hidden. We're reversing after all.
            if (transform.isZero)
                return;

            //Transform the vertices by the transform passed, moving it to a new position in 3d space.
            for (int vert = 0; vert < Vertices.Length; vert++)
                Vertices[vert] = Matrix4x4.VectorMultiply(Vertices[vert], transform);
        }

        /// <summary>
        /// Clones the mesh.
        /// </summary>
        /// <returns>A copy of the mesh.</returns>
        public Mesh Clone()
        {
            Mesh tCopy = new Mesh();

            tCopy.HeaderSize = HeaderSize;
            tCopy.MeshDataSize = MeshDataSize;
            tCopy.VertexCount = VertexCount;
            tCopy.VertexOffset = VertexOffset;
            tCopy.NormalCount = NormalCount;
            tCopy.NormalOffset = NormalOffset;
            tCopy.ColorCount = ColorCount;
            tCopy.ColorOffset = ColorOffset;
            tCopy.PolygonBlockSize = PolygonBlockSize;
            tCopy.PolygonsOffset = PolygonsOffset;
            tCopy.UVCount = UVCount;
            tCopy.UVOffset = UVOffset;
            tCopy.MaterialCount = MaterialCount;
            tCopy.MaterialOffset = MaterialOffset;
            tCopy.MDTOffset = MDTOffset;
            tCopy.MeshBoneIndex = MeshBoneIndex;

            tCopy.Vertices = Copy<Vector4>(Vertices);
            tCopy.Normals = Copy<Vector4>(Normals);
            tCopy.UVs = Copy<Vector4>(UVs);
            tCopy.Colors = Copy<Vector4>(Colors);
            tCopy.Materials = new Material[Materials.Length];
            for (int mat = 0; mat < Materials.Length; mat++)
                tCopy.Materials[mat] = Materials[mat];
            for (int poly = 0; poly < Polygons.Count; poly++)
                tCopy.Polygons.Add(Polygons[poly].Clone());

            return tCopy;
        }

        /// <summary>
        /// Preforms an array copy.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="array">The array to copy.</param>
        /// <returns>A copy of the array passed.</returns>
        static T[] Copy<T>(T[] array)
        {
            if (array != null)
            {
                T[] copy = new T[array.Length];
                for (int i = 0; i < array.Length; i++)
                    copy[i] = array[i];
                return copy;
            }
            return null;
        }
    }
}