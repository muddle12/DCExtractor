using Custom.IO;
using DC.Types;
using System.IO;
using static DC.Types.TriangleStrip;

namespace DC.IO
{
    /// <summary>
    /// Class   :   "WavefrontOBJ"
    /// 
    /// Purpose :   defines a simple wavefront obj exporter.
    /// 
    /// Wavefront OBJ is probably the simpliest model format you can export to. It's great for debugging purposes, but lacks the ability to output rigs, weight mappings, and animation.
    /// </summary>
    public static class WavefrontOBJ
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Constants.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public const string OBJExtension = ".obj";


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Helpers.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Formats a face into a wavefront face string.
        /// </summary>
        /// <param name="nVertex">The vertex index.</param>
        /// <param name="nUV">The uv index.</param>
        /// <param name="nNormal">The normal index.</param>
        /// <returns>A formatted face string.</returns>
        static string FormatFace(int nVertex, int nUV, int nNormal)
        {
            string szFace = nVertex.ToString();
            if (nUV != -1)
                szFace += "/" + nUV.ToString();
            if (nNormal != -1)
            {
                if (nUV == -1)
                    szFace += "/";
                szFace += "/";
                szFace += nNormal;
            }
            return szFace;
        }

        /// <summary>
        /// Formats a triangle using faces.
        /// </summary>
        /// <param name="A">The first vertex.</param>
        /// <param name="B">The second vertex.</param>
        /// <param name="C">The third vertex.</param>
        /// <returns>The triangle format string.</returns>
        static string FormatTriangle(Index A, Index B, Index C)
        {
            return "f " +
                FormatFace(A.Vertex + 1, A.UV + 1, A.Normal + 1) + " " +
                FormatFace(B.Vertex + 1, B.UV + 1, B.Normal + 1) + " " +
                FormatFace(C.Vertex + 1, C.UV + 1, C.Normal + 1);
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Saves a series of wavefront objects to file from the model provided at the location of the model.
        /// </summary>
        /// <param name="tModel">The model to save to .objs.</param>
        public static void Save(Model tModel)
        {
            //Due to the way the obj format seems to work, we can't have more than one mesh, otherwise it doesn't import correctly. We'll split it first and save each mesh individually.
            Model[] tModels = tModel.Split();
            for (int model = 0; model < tModels.Length; model++)
            {
                //Write out the wavefront object to file.
                SaveMesh(DCExtractor.Data.FileHelpers.ChangeExtension(tModels[model].filePath, OBJExtension), tModels[model].Meshes[0]);
            }
        }

        /// <summary>
        /// Writes an individual mesh to a .obj file.
        /// </summary>
        /// <param name="szFilePath">The path of the .obj file.</param>
        /// <param name="tMesh">The mesh to source for the file's data.</param>
        static void SaveMesh(string szFilePath, Mesh tMesh)
        {
            //Open our file stream.
            StreamWriter tWriter = FileStreamHelpers.OpenStreamWriter(szFilePath);
            if (tWriter == null)
                return;

            //Output our vertex data.
            for (int v = 0; v < tMesh.VertexCount; v++)
            {
                tWriter.WriteLine("v " + tMesh.Vertices[v].x.ToString("0.00000") + " " +
                    tMesh.Vertices[v].y.ToString("0.00000") + " " +
                    tMesh.Vertices[v].z.ToString("0.00000"));
            }

            //Output our normal data.
            for (int n = 0; n < tMesh.NormalCount; n++)
            {
                tWriter.WriteLine("vn " + tMesh.Normals[n].x.ToString("0.00000") + " " +
                tMesh.Normals[n].y.ToString("0.00000") + " " +
                tMesh.Normals[n].z.ToString("0.00000"));
            }

            //Output our uv data.
            for (int uv = 0; uv < tMesh.UVCount; uv++)
            {
                tWriter.WriteLine("vt " + tMesh.UVs[uv].x.ToString("0.00000") + " " +
                tMesh.UVs[uv].y.ToString("0.00000"));
            }

            //This part is tricky, we need to convert the polygons in the DCMesh into regular triangles.
            for (int f = 0; f < tMesh.Polygons.Count; f++)
            {
                for (int s = 0; s < tMesh.Polygons[f].Strips.Length; s++)
                {
                    //Style relates to PS2 OpenGL primitives formats. We need to backconvert them to standard triangles.
                    switch (tMesh.Polygons[f].Strips[s].Style)
                    {
                        case PrimitiveStyle.GL_TRIANGLES:
                            {
                                tWriter.WriteTriangles(tMesh.Polygons[f].Strips[s]);
                            }
                            break;
                        case PrimitiveStyle.GL_TRIANGLE_STRIP:
                            {
                                tWriter.WriteTriangleStrip(tMesh.Polygons[f].Strips[s]);
                            }
                            break;
                        //Weirdly, I didn't find any models that had these formats, so I'm not going to implement them.
                        //case PrimitiveStyle.GL_TRIANGLE_FAN:
                        //    break;
                        //case PrimitiveStyle.GL_QUADS:
                        //    break;
                        //case PrimitiveStyle.GL_QUAD_STRIP:
                        //    break;
                        //case PrimitiveStyle.GL_POLYGON:
                        //    break;
                        default:
                            throw new System.NotImplementedException("The Primitive Style has not been implemented in WavefrontOBJ: " + System.Enum.GetName(typeof(PrimitiveStyle), tMesh.Polygons[f].Strips[s].Style));
                    }
                }
            }

            //Close our stream.
            tWriter.Close();
        }

        /// <summary>
        /// Writes the triangle strip to the stream as a triangle batch.
        /// </summary>
        /// <param name="tWriter">The output stream.</param>
        /// <param name="tStrip">The strip to write.</param>
        static void WriteTriangles(this StreamWriter tWriter, TriangleStrip tStrip)
        {
            //Output the incoming triangle strip as individual non-overlapping triangles.
            for (int i = 0; i < tStrip.Indices.Length; i += 3)
                tWriter.WriteLine(FormatTriangle(tStrip.Indices[i], tStrip.Indices[i + 1], tStrip.Indices[i + 2]));
        }

        /// <summary>
        /// Writes the triangle strip to the stream as a triangle strip.
        /// </summary>
        /// <param name="tWriter">The output stream.</param>
        /// <param name="tStrip">The strip to write.</param>
        static void WriteTriangleStrip(this StreamWriter tWriter, TriangleStrip tStrip)
        {
            //We need to unpack these triangles as an OpenGL trianglestrip, which means we have to bounce back and forth forming triangles from the overlapping edges.
            string szLine;
            for (int i = 0; i < tStrip.IndexCount - 2; i++)
            {
                //We're switching on even/odd here because the triangles are sharing edges on their overlap.
                if ((i & 1) != 0)
                    szLine = FormatTriangle(tStrip.Indices[i + 1], tStrip.Indices[i], tStrip.Indices[i + 2]);
                else
                    szLine = FormatTriangle(tStrip.Indices[i], tStrip.Indices[i + 1], tStrip.Indices[i + 2]);
                tWriter.WriteLine(szLine);
            }
        }
    }
}