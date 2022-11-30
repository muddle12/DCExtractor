using Custom.Data;
using Custom.IO;
using Custom.Math;
using DC.Types;
using DCExtractor;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using static DC.Types.TriangleStrip;

namespace DC.IO
{
    /// <summary>
    /// Class   :   "MDS"
    /// 
    /// Purpose :   provides functions for loading Level-5's MDS format.
    /// 
    /// The impetus what started this whole tool. Reverse engineering the mds file. It was tough going, and took roughly a month to parse through
    /// all of the data bit by bit. I had very little to go on, mainly scraping what I could from various posts from ages ago and one script for quickBMS.
    /// 
    /// The index data was the main pain point. Most of my time was spent figuring out what the index data meant, as it appeared to shapeshift depending
    /// on style and type. Eventually, I realized that it was referencing OpenGL, and once I pulled up the PS2 graphics specs, it all became clear.
    /// 
    /// The rest just fell into place.
    /// 
    /// 
    /// Structure-wise, the MDS has a header at the beginning which contains basic info about the model. After the header is a list of bones. 
    /// Then comes the MDTs, which are the meshes. There can be more than one MDT in a file.
    /// 
    /// MDTs contain all of the rendering information, with a header at the beginning.
    /// After the header is the vertex data, then the index data, normals, colors, uvs, and finally materials.
    /// I don't think I've seen color data used anywhere in Dark Cloud 2, so it's usually blank.
    /// You can seek to the various data types by using their respective offset values found in the header. The seek position begins at the beginning of the MDT header.
    /// 
    /// The entire spec is probably too complex to post here, so I'll offload it to the spec file.
    /// 
    /// For the record, if you see a boolean for "Scan", it is used to load in only useful bits of the model for debugging purposes. This was used back when I was still
    /// working out how the indices functioned. I would scan the model for surface level data and skip all the meat, then output that data to metadata text files so I could
    /// compare values. You can just keep scan = false if you want to do normal loads, and ignore/remove it entirely if you don't want it anymore. It's still useful for
    /// getting skeleton data for comparison.
    /// 
    /// 
    /// If you are viewing mds data with a hex editor, it is best viewed at a 16 bytes per row. 
    /// </summary>
    public static class MDS
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Loads the MDS from the file path.
        /// </summary>
        /// <param name="szMDSPath">The mds to load at the path.</param>
        /// <param name="tModel">The output model to place the data into.</param>
        /// <param name="bScanOnly">Whether or not to perform a surface level scan instead of a full load.</param>
        /// <returns>Whether or not the operation was successful.</returns>
        public static bool Load(string szMDSPath, out Model tModel, bool bScanOnly)
        {
            //Setup our progress bar.
            DCProgress.value = 0;
            DCProgress.maximum = 1;
            DCProgress.name = "Loading MDS";

            //Load our model.
            bool bLoaded = LoadModel(szMDSPath, out tModel, bScanOnly);

            //Report the finish.
            DCProgress.value = DCProgress.maximum;
            DCProgress.name = "Converting MDS - FINISHED";

            //Return the result.
            return bLoaded;
        }

        /// <summary>
        /// Loads an array of models from the directory specified.
        /// </summary>
        /// <param name="szFolderPath">The directory to load the models from.</param>
        /// <param name="bScanOnly">Whether or not to perform a surface level scan instead of a full load.</param>
        /// <returns>An array of loaded models.</returns>
        public static Model[] LoadDirectory(string szFolderPath, bool bScanOnly = false)
        {
            //Setup our progress bar.
            DCProgress.value = 0;
            DCProgress.name = "Loading MDS Directory";

            //Get a list of files from the target directory that are of type .mds.
            string[] szFiles = Directory.GetFiles(szFolderPath, "*.mds", SearchOption.AllDirectories);
            List<Model> tModels = new List<Model>();
            Model tResult;

            //Set our maximum to the file count.
            DCProgress.maximum = szFiles.Length;

            //Loop through and load those models.
            for (int i = 0; i < szFiles.Length; i++)
            {
                DCProgress.name = "Loading " + Path.GetFileName(szFiles[i]);

                //Make sure this file ends in mds. We've gotten false positives before. Add the result if the load was successful.
                if (szFiles[i].EndsWith(".mds") && LoadModel(szFiles[i], out tResult, bScanOnly))
                    tModels.Add(tResult);

                //Update our progress.
                DCProgress.value += 1;

                //If the operation was canceled, bail out.
                if (DCProgress.canceled)
                    return new Model[0];
            }

            //Post the finish.
            DCProgress.value = DCProgress.maximum;
            DCProgress.name = "Loading MDS Directory - FINISHED";

            return tModels.ToArray();
        }

        /// <summary>
        /// Loads the model at the specified path.
        /// </summary>
        /// <param name="szMDSPath">The path of the model to load.</param>
        /// <param name="tModel">The output model to place the data into.</param>
        /// <param name="bScanOnly">Whether or not to perform a surface level scan instead of a full load.</param>
        /// <returns>Whether or not the operation was a success.</returns>
        static bool LoadModel(string szMDSPath, out Model tModel, bool bScanOnly)
        {
            //Open our file stream.
            tModel = null;
            BinaryReader tReader = FileStreamHelpers.OpenBinaryReader(szMDSPath);
            if (tReader == null || tReader.BaseStream.Length == 0)
                return false;

            //Read in the header.
            tModel = new Model();
            tModel.filePath = szMDSPath;
            tReader.BaseStream.Position += 4;
            tModel.Header = Model.ModelIdentifier;
            tModel.Version = Endian.Swap(tReader.ReadInt32());
            tModel.BoneCount = tReader.ReadInt32();
            tModel.BoneOffset = tReader.ReadInt32();

            {
                //Read in the bones.
                tModel.Bones = new Bone[tModel.BoneCount];
                for (int b = 0; b < tModel.BoneCount; b++)
                {
                    tModel.Bones[b] = new Bone();
                    tModel.Bones[b].index = tReader.ReadInt32();
                    tModel.Bones[b].size = tReader.ReadInt32();
                    tModel.Bones[b].name = tReader.ReadBytesAsString(Bone.BoneNameLength);
                    tModel.Bones[b].associatedMDTOffset = tReader.ReadInt32();
                    tModel.Bones[b].parentIndex = tReader.ReadInt32();
                    tModel.Bones[b].local = tReader.ReadMatrix();
                }

                //Assign all of the parent bones.
                for (int i = 0; i < tModel.BoneCount; i++)
                {
                    if (tModel.Bones[i].parentIndex != -1)
                        tModel.Bones[i].parent = tModel.Bones[tModel.Bones[i].parentIndex];
                }

                //Calculate the bone hierarchy.
                for (int i = 0; i < tModel.BoneCount; i++)
                    tModel.Bones[i].CalculateHierarchy();

                //Calculate the world matrices. This part is purely optional, as I was using it for debugging.
                //for (int i = 0; i < tModel.BoneCount; i++)
                //    tModel.Bones[i].CalculateWorldMatrix();
            }

            //read in our meshes.
            List<Mesh> tMeshes = new List<Mesh>();
            while (tReader.SeekHeader(Model.MeshIdentifier))
            {
                Mesh tMesh = LoadMesh(tReader, tReader.BaseStream.Position, bScanOnly, szMDSPath);
                if (tMesh == null)
                    return false;
                tMeshes.Add(tMesh);
            }
            tModel.Meshes = tMeshes.ToArray();

            //load in the bone bind pose file if it exists.
            {
                string szBBPPath = Path.ChangeExtension(szMDSPath, ".bbp");
                if (File.Exists(szBBPPath))
                {
                    Matrix4x4[] tBindPoses = BBP.Load(szBBPPath);
                    for (int bone = 0; bone < tModel.Bones.Length && bone < tBindPoses.Length; bone++)
                    {
                        tModel.Bones[bone].bind = tBindPoses[bone];
                        tModel.Bones[bone].hasBindPose = true;
                    }
                }
            }

            //load in our weight map if it exists.
            string szWGTPath = Path.ChangeExtension(szMDSPath, ".wgt");
            if (File.Exists(szWGTPath))
                tModel.Weights = WGT.Load(szWGTPath);

            //associate the bone/mesh indices so we can properly weight the meshes.
            if (bScanOnly == false)
            {
                for (int bone = 0; bone < tModel.Bones.Length; bone++)
                {
                    if (tModel.Bones[bone].associatedMDTOffset == 0)
                        continue;
                    for (int mesh = 0; mesh < tModel.Meshes.Length; mesh++)
                    {
                        if (tModel.Bones[bone].associatedMDTOffset == tModel.Meshes[mesh].MDTOffset)
                        {
                            tModel.Meshes[mesh].MeshBoneIndex = bone;

                            /****************************************************/
                            //  Here's a fun little problem that eluded me for a
                            //  while. The bind bones seemed to be easy to nab
                            //  from the file, but their actual purpose was rather
                            //  mystifying. They didn't apply to the hierarchy or
                            //  the weights, they just were... there.
                            //
                            //  Eventually, I realized that these are actually
                            //  offsets for meshes to put them into their
                            //  correct bind positions. Perhaps there was an
                            //  engine or modeling package reason why certain
                            //  meshes sit at the origin, but I'm glad to have
                            //  finally figured this out.
                            //
                            //  Long story short, these are offsets to transform
                            //  meshes to their correct bind positions from
                            //  their origin starting position.
                            /****************************************************/
                            tModel.Meshes[mesh].TransformVertices(tModel.Bones[bone].bind);
                            break;
                        }
                    }
                }

                //Load in any animations.
                string szMOTPath = Path.ChangeExtension(szMDSPath, ".mot");
                if (File.Exists(szMOTPath))
                    tModel.Animations = MOT.Load(szMOTPath);
            }

            tReader.Close();
            return true;
        }

        /// <summary>
        /// Loads in a mesh file at the specified offset.
        /// </summary>
        /// <param name="tReader">The reader loading in the mesh.</param>
        /// <param name="nMeshOffset">The offset into the file where the mesh header is located.</param>
        /// <param name="bScanOnly">Whether or not to only scan instead of loading all data.</param>
        /// <param name="szFileName">Used to throw errors to messagebox when an exception happens.</param>
        /// <returns>A new mesh.</returns>
        static Mesh LoadMesh(this BinaryReader tReader, long nMeshOffset, bool bScanOnly, string szFileName)
        {
            Mesh tMesh = new Mesh();

            //Grab the offset.
            tMesh.MDTOffset = tReader.BaseStream.Position;

            //Skip the magic.
            tReader.BaseStream.Position += 4;

            //Load in the mdt header.
            tMesh.HeaderSize = tReader.ReadInt32();
            tMesh.MeshDataSize = tReader.ReadInt32();

            tMesh.VertexCount = tReader.ReadInt32();
            tMesh.VertexOffset = tReader.ReadInt32();

            tMesh.NormalCount = tReader.ReadInt32();
            tMesh.NormalOffset = tReader.ReadInt32();

            tMesh.ColorCount = tReader.ReadInt32();
            tMesh.ColorOffset = tReader.ReadInt32();

            tMesh.PolygonBlockSize = tReader.ReadInt32();
            tMesh.PolygonsOffset = tReader.ReadInt32();

            tMesh.UVCount = tReader.ReadInt32();
            tMesh.UVOffset = tReader.ReadInt32();

            tMesh.MaterialCount = tReader.ReadInt32();
            tMesh.MaterialOffset = tReader.ReadInt32();

            //An fyi, the scan option is mainly for debugging purposes. It can safely be ignored if you aren't doing stuff with the meta file generation.
#if DEBUG
            if (bScanOnly == false)
            {
#endif
                //If we have vertices, load them at the offset.
                if (tMesh.VertexCount > 0)
                {
                    tReader.BaseStream.Position = nMeshOffset + tMesh.VertexOffset;
                    tMesh.Vertices = new Vector4[tMesh.VertexCount];
                    for (int v = 0; v < tMesh.VertexCount; v++)
                        tMesh.Vertices[v] = tReader.ReadVector4();
                }

                //If we have normals, load them at the offset.
                if (tMesh.NormalCount > 0)
                {
                    tReader.BaseStream.Position = nMeshOffset + tMesh.NormalOffset;
                    tMesh.Normals = new Vector4[tMesh.NormalCount];
                    for (int n = 0; n < tMesh.NormalCount; n++)
                        tMesh.Normals[n] = tReader.ReadVector4();
                }

                //If we have uvs, load them at the offset.
                if (tMesh.UVCount > 0)
                {
                    tReader.BaseStream.Position = nMeshOffset + tMesh.UVOffset;
                    tMesh.UVs = new Vector4[tMesh.UVCount];
                    for (int uv = 0; uv < tMesh.UVCount; uv++)
                        tMesh.UVs[uv] = tReader.ReadVector4();
                }

                //If we have colors, load them at the offset.
                if (tMesh.ColorCount > 0)
                {
                    tReader.BaseStream.Position = nMeshOffset + tMesh.ColorOffset;
                    tMesh.Colors = new Vector4[tMesh.ColorCount];
                    for (int color = 0; color < tMesh.ColorCount; color++)
                        tMesh.Colors[color] = tReader.ReadVector4();
                }

                //If we have materials, we load them at the offset.
                if (tMesh.MaterialCount > 0)
                {
                    tReader.BaseStream.Position = nMeshOffset + tMesh.MaterialOffset;
                    tMesh.Materials = new Mesh.Material[tMesh.MaterialCount];
                    for (int i = 0; i < tMesh.MaterialCount; i++)
                    {
                        tMesh.Materials[i].firstColor = tReader.ReadVector4();
                        tMesh.Materials[i].secondColor = tReader.ReadVector4();
                        tMesh.Materials[i].thirdColor = tReader.ReadVector4();
                        tMesh.Materials[i].unknown1 = tReader.ReadSingle();
                        tMesh.Materials[i].name = tReader.ReadString(Mesh.Material.NameLength);
                    }
                }
#if DEBUG
            }
#endif
            //If we have faces, load them at the offset.
            if (tMesh.PolygonBlockSize > 0)// && bScanOnly == false)
            {
                //Faces are weird, we need to know then end of the buffer since faces can have varying size. We calculate this offset.
                long nFaceBufferEnd = nMeshOffset + tMesh.PolygonsOffset + tMesh.PolygonBlockSize;

                //Start at the beginning of the face buffer.
                tReader.BaseStream.Position = nMeshOffset + tMesh.PolygonsOffset;
                while (tReader.BaseStream.Position < nFaceBufferEnd)
                {
                    Polygon tPolygon = new Polygon();

                    //Read the polygon header. This header contains information about the following strip of strips.
                    tPolygon.unk1 = tReader.ReadInt32();
                    tPolygon.Type = tReader.ReadInt32();
                    tPolygon.StripCount = tReader.ReadInt32();
                    tPolygon.Unused2 = tReader.ReadInt32();

                    //For each strip.
                    tPolygon.Strips = new TriangleStrip[tPolygon.StripCount];
                    for (int strip = 0; strip < tPolygon.StripCount; strip++)
                    {
                        //Get the header for the strip.
                        tPolygon.Strips[strip].Style = (PrimitiveStyle)tReader.ReadByte();
                        tPolygon.Strips[strip].Type = (TriangleStrip.PrimitiveType)tReader.ReadByte();
                        tPolygon.Strips[strip].Unused1 = tReader.ReadInt16();
                        tPolygon.Strips[strip].IndexCount = tReader.ReadInt32();
                        tPolygon.Strips[strip].MaterialIndex = tReader.ReadInt32();

                        //In the weird case where our incoming primitive is a collision mesh, the index data can only be of type Vertex.
                        TriangleStrip.PrimitiveType eType = tPolygon.Strips[strip].Type;
                        if (tPolygon.Strips[strip].Style == PrimitiveStyle.DC2_COLLISION_TRIANGLES)
                            eType = TriangleStrip.PrimitiveType.Vertex;

                        //HACK  :   We'll switch here to make sure we're going getting a style we don't support. Most of these styles appear to be custom, and
                        //  since they are so rarely encountered, it's probably best to just not support them.
                        switch (tPolygon.Strips[strip].Style)
                        {
                            case PrimitiveStyle.GL_TRIANGLES:
                            case PrimitiveStyle.GL_TRIANGLE_STRIP:
                            case PrimitiveStyle.DC2_COLLISION_TRIANGLES:
                                break;
                            default:
                                {
                                    string szEnumType = System.Enum.GetName(typeof(PrimitiveStyle), tPolygon.Strips[strip].Style);
                                    if (string.IsNullOrEmpty(szEnumType))
                                        szEnumType = ((int)tPolygon.Strips[strip].Style).ToString();
                                    if (Settings.showMDSWarnings)
                                    {
                                        MessageBox.Show("Invalid index data found in MDS: " + Path.GetFileName(szFileName) + "\n" +
                                             szEnumType + " style is not supported.\n Skipping File.",
                                            "MDS Parsing Failed!");
                                    }
                                }
                                return null;
                        }

#if DEBUG
                        if (bScanOnly == false)
                        {
#endif
                            //At the end of the header, we loop through the indicies and grab the relevant indicies.
                            tPolygon.Strips[strip].Indices = new Index[tPolygon.Strips[strip].IndexCount];
                            for (int index = 0; index < tPolygon.Strips[strip].IndexCount; index++)
                                tPolygon.Strips[strip].Indices[index] = tReader.ReadIndex(eType);
#if DEBUG
                        }
                        else
                        {
                            tPolygon.Strips[strip].Indices = new Index[1];
                            tPolygon.Strips[strip].Indices[0] = tReader.ReadIndex(tPolygon.Strips[strip].Type);
                            //Skip the rest of the indices.
                            tReader.BaseStream.Position += (tPolygon.Strips[strip].IndexCount - 1) * sizeof(int) * 3;
                        }
#endif
                    }
                    tMesh.Polygons.Add(tPolygon);
                    if (bScanOnly)
                        break;
                }
            }

            return tMesh;
        }

        /// <summary>
        /// Reads an individual index from the face strip.
        /// </summary>
        /// <param name="tReader">The input stream.</param>
        /// <returns>An index.</returns>
        static Index ReadIndex(this BinaryReader tReader, TriangleStrip.PrimitiveType eType)
        {
            //Since index data can vary in its contents, we'll initialize everything with -1.
            Index tIndexData;
            tIndexData.Vertex = -1;
            tIndexData.Normal = -1;
            tIndexData.UV = -1;
            tIndexData.Color = -1;

            //Switch on the type and retrieve the necessary index data in this index.
            switch (eType)
            {
                case TriangleStrip.PrimitiveType.VertexNormalUV:
                    {
                        tIndexData.Vertex = tReader.ReadInt32();
                        tIndexData.Normal = tReader.ReadInt32();
                        tIndexData.UV = tReader.ReadInt32();
                    }
                    break;
                case TriangleStrip.PrimitiveType.VertexNormalUVColor:
                    {
                        tIndexData.Vertex = tReader.ReadInt32();
                        tIndexData.Normal = tReader.ReadInt32();
                        tIndexData.UV = tReader.ReadInt32();
                        tIndexData.Color = tReader.ReadInt32();
                    }
                    break;
                case TriangleStrip.PrimitiveType.VertexNormal:
                    {
                        tIndexData.Vertex = tReader.ReadInt32();
                        tIndexData.Normal = tReader.ReadInt32();
                    }
                    break;
                case TriangleStrip.PrimitiveType.Vertex:
                    {
                        tIndexData.Vertex = tReader.ReadInt32();
                    }
                    break;
                default:
                    break;
            }

            //Return the result.
            return tIndexData;
        }

        /// <summary>
        /// Reads a vector4 and returns it.
        /// </summary>
        /// <param name="tReader">The input stream.</param>
        /// <returns>A 16 byte Vector4.</returns>
        public static Vector4 ReadVector4(this BinaryReader tReader)
        {
            //Remember, everything is 16 byte aligned, so even data that's only Vector2/Vector3 must be read as a Vector4.
            return new Vector4(tReader.ReadSingle(), tReader.ReadSingle(), tReader.ReadSingle(), tReader.ReadSingle());
        }
    }
}