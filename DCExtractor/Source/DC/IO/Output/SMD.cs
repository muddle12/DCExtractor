using Custom.IO;
using Custom.Math;
using DC.Types;
using DCExtractor.Data;
using System.IO;

namespace DC.IO
{
    /// <summary>
    /// Class   :   "SMD"
    /// 
    /// Purpose :   implements saving from MDS to studiomdl SMD.
    /// https://developer.valvesoftware.com/wiki/Studiomdl_Data
    /// 
    /// This format is not heavily documented, and is largely abandoned by its developer, with its compilers and internal workings being closed source and proprietary.
    /// It also doesn't support scaling, which means the scale data found in the mds files gets destroyed. If you want to recover that data, you'll need another format.
    /// 
    /// Still, it's a well-supported format by the community, and it's easy to read, write, and debug. You can export skeletons, weightmaps, and animation.
    /// 
    /// The main reason I ended up settling with smd was because it was one of the few widely used formats that supported animation AND wasn't a
    /// blackbox like FBX or incredibly obtuse like collada/alembic. I did attempt to use Assimp at one point, but it kept crashing and had no support for the issue I was
    /// encountering, so I had to turn to purely text formats to get the job done.
    /// 
    /// SMD is a purely text format that's divided into chunks denoted by a beginning keyword and terminated by an "end" keyword. Data is stored in floating point, with
    /// rotations being stored as Tait-Bryan angles(or so I'm told) in radians.
    /// </summary>
    public static class SMD
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Data Members.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public const string SMDExtension = ".smd";


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Saves the model to the specified path in the smd format.
        /// </summary>
        /// <param name="tModel">The model to output to smd.</param>
        /// <returns>Whether or not the operation was a success.</returns>
        public static bool Save(Model tModel)
        {
            //save out the model to an smd file.
            return SaveModel(DCExtractor.Data.FileHelpers.ChangeExtension(tModel.filePath, SMDExtension), tModel);
        }

        /// <summary>
        /// Saves out the animations from the model file.
        /// </summary>
        /// <param name="tModel">The model to source animations from.</param>
        /// <returns>Whether or not the operation was a success.</returns>
        public static bool SaveAnimations(Model tModel)
        {
            //Make sure we have animations to save.
            if (tModel.Animations != null && tModel.Animations.Length > 0)
            {
                //Generate a base name for the animations.
                string szBaseName = Path.GetFileNameWithoutExtension(tModel.filePath);

                //Generate a new animation directory for the animations to save to.
                string szAnimationDirectory = Path.Combine(Path.GetDirectoryName(tModel.filePath), "anims");

                //Save out the animations.
                SaveAnimations(szBaseName, szAnimationDirectory, tModel);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Saves out a model to an smd file.
        /// </summary>
        /// <param name="szFilePath">The file path of the smd.</param>
        /// <param name="tModel">The model being saved.</param>
        /// <returns>Whether or not the operation was a success.</returns>
        static bool SaveModel(string szFilePath, Model tModel)
        {
            //Change the extension if it's invalid.
            if (szFilePath.ToLower().EndsWith(SMDExtension) == false)
                szFilePath = FileHelpers.ChangeExtension(szFilePath, SMDExtension);

            //Open our file stream.
            StreamWriter tWriter = FileStreamHelpers.OpenStreamWriter(szFilePath);
            if (tWriter == null)
                return false;

            //Write out our data one chunk at a time.
            tWriter.WriteHeader();
            tWriter.WriteBones(tModel);
            tWriter.WriteSkeleton(tModel);
            tWriter.WriteTriangleSection(tModel);

            //Close our file stream and return.
            tWriter.Close();
            return true;
        }

        /// <summary>
        /// Writes the smd header.
        /// </summary>
        /// <param name="tWriter">The output stream.</param>
        static void WriteHeader(this StreamWriter tWriter)
        {
            //These are always version 1.
            tWriter.WriteLine("version 1");
        }

        /// <summary>
        /// Writes the list of bones.
        /// </summary>
        /// <param name="tWriter">The output stream.</param>
        /// <param name="tModel">The model to source from.</param>
        static void WriteBones(this StreamWriter tWriter, Model tModel)
        {
            //Write the nodes keyword.
            tWriter.WriteLine("nodes");
            {
                for (int bone = 0; bone < tModel.BoneCount; bone++)
                {
                    //Write out each bone, starting with an index, the name of the bone, and the index of the parent bone.
                    tWriter.WriteLine(tModel.Bones[bone].index.ToString() + " \"" + tModel.Bones[bone].name + "\" " + tModel.Bones[bone].parentIndex);
                }
            }
            //Write the end keyword.
            tWriter.WriteLine("end");
        }

        /// <summary>
        /// Writes the skeleton and animation of the model.
        /// </summary>
        /// <param name="tWriter">The output stream.</param>
        /// <param name="tModel">The model to source from.</param>
        static void WriteSkeleton(this StreamWriter tWriter, Model tModel)
        {
            //Write the skeleton keyword.
            tWriter.WriteLine("skeleton");
            {
                //Technically we're writing out an animation with one frame. Write the time of the first frame, which is zero.
                tWriter.WriteLine("time 0");
                for (int bone = 0; bone < tModel.BoneCount; bone++)
                {
                    //Write each bone with its translation and rotation as sets of three floating point values each.
                    tWriter.WriteBone(tModel.Bones[bone].index, tModel.Bones[bone].local.translation, tModel.Bones[bone].local.rotation);
                }
            }
            //Write the end keyword.
            tWriter.WriteLine("end");
        }

        /// <summary>
        /// Writes the triangles for the model.
        /// </summary>
        /// <param name="tWriter">The output stream.</param>
        /// <param name="tModel">The model to source from.</param>
        static void WriteTriangleSection(this StreamWriter tWriter, Model tModel)
        {
            //Write the triangles keyword.
            tWriter.WriteLine("triangles");
            {
                for (int mesh = 0; mesh < tModel.Meshes.Length; mesh++)
                {
                    //Write each polygon out per mesh. We're basically combining all meshes together here.
                    for (int poly = 0; poly < tModel.Meshes[mesh].Polygons.Count; poly++)
                        tWriter.WritePolygon(tModel, tModel.Meshes[mesh], tModel.Meshes[mesh].Polygons[poly]);
                }
            }
            //Write the end keyword.
            tWriter.WriteLine("end");
        }

        /// <summary>
        /// Writes the polygons for the model.
        /// </summary>
        /// <param name="tWriter">The output stream.</param>
        /// <param name="tModel">The model to source from.</param>
        /// <param name="tMesh">The mesh to source from.</param>
        /// <param name="tPoly">The current poly to output.</param>
        static void WritePolygon(this StreamWriter tWriter, Model tModel, Mesh tMesh, Polygon tPoly)
        {
            //Write each triangle strip out one at a time.
            for (int strip = 0; strip < tPoly.Strips.Length; strip++)
                tWriter.WriteTriangleStrip(tModel, tMesh, tPoly.Strips[strip]);
        }

        /// <summary>
        /// Writes a triangle strip to the output stream.
        /// </summary>
        /// <param name="tWriter">The output stream.</param>
        /// <param name="tMesh">The mesh to source from.</param>
        /// <param name="tStrip">The current strip to output.</param>
        static void WriteTriangleStrip(this StreamWriter tWriter, Model tModel, Mesh tMesh, TriangleStrip tStrip)
        {
            //This part is tricky. We need to translate these triangles from OpenGL primitives.
            int nEnd = 0;
            int nIter = 1;
            switch (tStrip.Style)
            {
                //Traditional triangle buffer.
                case Types.TriangleStrip.PrimitiveStyle.GL_TRIANGLES:
                case TriangleStrip.PrimitiveStyle.DC2_COLLISION_TRIANGLES:
                    {
                        nEnd = tStrip.Indices.Length;
                        nIter = 3;
                    }
                    break;
                //Triangle strip format, it needs to be reformatted to fit a traditional triangle buffer.
                case Types.TriangleStrip.PrimitiveStyle.GL_TRIANGLE_STRIP:
                    {
                        nEnd = tStrip.Indices.Length - 2;
                        nIter = 1;
                    }
                    break;
                default:
                    return;
            }

            //In an effort to stream-line things, I select the indices of each triangle based on the triangle type.
            int a = 0, b = 0, c = 0;
            for (int i = 0; i < nEnd; i += nIter)
            {
                //Switch on the opengl primitive style.
                switch (tStrip.Style)
                {
                    //Traditional triangle buffer.
                    case Types.TriangleStrip.PrimitiveStyle.GL_TRIANGLES:
                    case TriangleStrip.PrimitiveStyle.DC2_COLLISION_TRIANGLES:
                        {
                            //standard triangle arrangement.
                            a = i;
                            b = i + 1;
                            c = i + 2;
                        }
                        break;
                    //Triangle strip format, it needs to be reformatted to fit a traditional triangle buffer.
                    case Types.TriangleStrip.PrimitiveStyle.GL_TRIANGLE_STRIP:
                        {
                            //flipflop triangle-strip arrangement.
                            if ((i & 1) != 0)
                            {
                                a = i + 1;
                                b = i;
                                c = i + 2;
                            }
                            else
                            {
                                a = i;
                                b = i + 1;
                                c = i + 2;
                            }
                        }
                        break;
                        //I wasn't able to find any other primitives other than the above to. If one does surface, it's like in Dark Cloud 1(where I did less testing).
                    default:
                        continue;//If you got here, something went wrong with the original mds parse, or the format is unsupported.
                }

                //create the face from the indices.
                tWriter.WriteLine(tMesh.Materials[tStrip.MaterialIndex].name);  //Write the material for this triangle first.
                tWriter.WriteVertex(a, tModel, tMesh, tStrip);
                tWriter.WriteVertex(b, tModel, tMesh, tStrip);
                tWriter.WriteVertex(c, tModel, tMesh, tStrip);
            }
        }

        /// <summary>
        /// Writes a vertex to the stream.
        /// </summary>
        /// <param name="tWriter">The output stream.</param>
        /// <param name="index">The index of the vertex</param>
        /// <param name="tMesh">The output mesh.</param>
        /// <param name="tStrip">The current strip.</param>
        static void WriteVertex(this StreamWriter tWriter, int index, Model tModel, Mesh tMesh, TriangleStrip tStrip)
        {
            //Not all primitives pulled from the mds have all of their data. Some are only vertices. We'll allocate defaults up front.
            Vector4 vert = tMesh.Vertices[tStrip.Indices[index].Vertex];
            Vector4 norm = new Vector4(0.0f, 0.0f, 1.0f);
            Vector4 uv = new Vector4();

            /****************************************************************************/
            //  This sucks, but some models have meshes with invalid indices that are
            //  outside the range of the indices. This is probably because we haven't
            //  properly parsed the Style indices for all styles. Until that gets fixed
            //  We'll need to make sure we don't index outside the normal and uv arrays.
            /****************************************************************************/
            //If this triangle strip has normals, we'll grab them.
            if (tStrip.Indices[index].Normal != -1 && tStrip.Indices[index].Normal < tMesh.Normals.Length)//HACK    :   cap the index.
                norm = tMesh.Normals[tStrip.Indices[index].Normal];
            //If this triangle strip has uvs, we'll grab them.
            if (tStrip.Indices[index].UV != -1 && tStrip.Indices[index].UV < tMesh.UVs.Length)//HACK    :   cap the index.
                uv = tMesh.UVs[tStrip.Indices[index].UV];

            //This might be a hack, but the first value is usually the bone weight, which gets overwritten later along the line by another set of bone weights. I'll write 0 as a default.
            tWriter.Write("0 ");
            {
                //Write out our triangle data.
                tWriter.Write(vert.x + " " + vert.y + " " + vert.z + " ");
                tWriter.Write(norm.x + " " + norm.y + " " + norm.z + " ");
                tWriter.Write(uv.x + " " + uv.y);

                //This part... I'm not entirely sure it's the best way to go about it, but it works, so I'm not going to question it.
                //If the triangle has weights, we need to output them after the triangle data.
                if (tModel.hasWeights)
                {
                    //Grab our weights based on which mesh and vertex we're dealing with.
                    VertexWeight[] tWeights = tModel.GetWeights(tMesh.MeshBoneIndex, tStrip.Indices[index].Vertex);

                    //If we have weights, we'll daisy-chain them on the end.
                    if (tWeights.Length > 0)
                    {
                        tWriter.Write(" " + tWeights.Length);
                        for (int weight = 0; weight < tWeights.Length; weight++)
                        {
                            tWriter.Write(" " + tWeights[weight].boneIndex + " " + tWeights[weight].weight);
                        }
                    }
                    //Otherwise, we'll write some defaults.
                    else
                    {
                        tWriter.Write(" 1 " + tMesh.MeshBoneIndex + " 1");
                    }
                }
            }
            //Add a newline to finish the line.
            tWriter.Write(System.Environment.NewLine);
        }

        /// <summary>
        /// Saves out the animations in the model to separate animation files.
        /// </summary>
        /// <param name="szBaseName">The base name of the original model file.</param>
        /// <param name="szAnimationDirectory">The directory to save the animations to.</param>
        /// <param name="tModel">The model that contains the animations.</param>
        /// <returns>Whether or not the operation was a success.</returns>
        static void SaveAnimations(string szBaseName, string szAnimationDirectory, Model tModel)
        {
            //create the directory if it does not exist.
            if (Directory.Exists(szAnimationDirectory) == false)
                Directory.CreateDirectory(szAnimationDirectory);

            //loop through and save out each animation individually.
            string szBaseAnimationFilePath = Path.Combine(szAnimationDirectory, szBaseName);
            for (int anim = 0; anim < tModel.Animations.Length; anim++)
                SaveAnimation(szBaseAnimationFilePath + "_animation" + anim + ".smd", tModel, tModel.Animations[anim]);
        }

        /// <summary>
        /// Saves out an animation to a file.
        /// </summary>
        /// <param name="szFilePath">The path to the file to create.</param>
        /// <param name="tModel">The model that is associated with the animation.</param>
        /// <param name="tAnimation">The animation to be saved.</param>
        static void SaveAnimation(string szFilePath, Model tModel, Animation tAnimation)
        {
            //Open a file stream.
            StreamWriter tWriter = FileStreamHelpers.OpenStreamWriter(szFilePath);
            if (tWriter == null)
                return;

            //Write out the usual smd stuff, which is detailed above.
            tWriter.WriteHeader();
            tWriter.WriteBones(tModel);
            tWriter.WriteLine("skeleton");
            {
                //Instead of writing triangle data, we're just going to keep writing skeletal hierarchies with new positions and rotations.
                Vector4 tPosition = new Vector4();
                Vector4 tRotation = new Vector4();
                bool bOverride = false;

                //Due to the way smd formats its data, we have to do a full print on the first frame of all bones regardless of whether or not they have bindings. After that we can trim the selection.
                tWriter.WriteLine("time 0");
                for (int i = 0; i < tModel.Bones.Length; i++)
                {
                    //A channel is basically a single animation for a single bone.
                    bOverride = false;
                    for (int chan = 0; chan < tAnimation.channels.Length; chan++)
                    {
                        //If this bone has keyframes on this channel.
                        if (tAnimation.channels[chan].keyFrames.Length > 0)
                        {
                            //Loop through and find that bone in the hierarchy.
                            if (tAnimation.channels[chan].boneIndex == tModel.Bones[i].index)
                            {
                                //Record those values.
                                tPosition = tAnimation.channels[chan].keyFrames[0].position;
                                tRotation = ConvertRotationToSMD(tAnimation.channels[chan].keyFrames[0].quatRotation);
                                bOverride = true;
                            }
                        }
                    }
                    //If we didn't find any keyframes, set defaults.
                    if (bOverride == false)
                    {
                        tPosition = new Vector4();
                        tRotation = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
                    }
                    //Write out our animation data.
                    tWriter.WriteBone(tModel.Bones[i].index, tPosition, tRotation);
                }
                
                //Now that the initial bones have been printed out, we just need to print whatever keyframes we have remaining.
                int nKeyFrameCount = tAnimation.channels[0].keyFrames.Length;
                for (int key = 1; key < nKeyFrameCount; key++)
                {
                    //Write the current keyframe.
                    tWriter.WriteLine("time " + key);
                    for (int chan = 0; chan < tAnimation.channels.Length; chan++)
                    {
                        /************************************************************/
                        //  Dark Cloud 1 has some variable keyframe stuff going on.
                        //  Since animations don't work anyway, I'm going to break
                        //  out of here for now. If you come along and are attempting
                        //  to figure out how the animation system works, you'll
                        //  need to solve this problem.
                        /************************************************************/
                        if (key >= tAnimation.channels[chan].keyFrames.Length)
                            break;//HACK
                        tWriter.WriteBone(tAnimation.channels[chan].boneIndex, tAnimation.channels[chan].keyFrames[key].position, ConvertRotationToSMD(tAnimation.channels[chan].keyFrames[key].quatRotation));
                    }
                }
            }

            //Write the end keyword.
            tWriter.WriteLine("end");

            //Close the file stream.
            tWriter.Close();
        }

        /// <summary>
        /// Converts the incoming quaternion to an euler format used by smd.
        /// </summary>
        /// <param name="tQuaternion">The quaternion to convert.</param>
        /// <returns>A converted quaternion to euler.</returns>
        static Vector4 ConvertRotationToSMD(Vector4 tQuaternion)
        {
            /******************************************************************/
            /// I've rigorously doubled checked my math on this one, 
            /// and I'm confident it is correct, but the results keep coming out 
            /// wrong. I think the issue is in the rig.
            /******************************************************************/
            return Matrix4x4.FromQuaternion(tQuaternion).rotation;
        }

        /// <summary>
        /// Writes a bone to the output stream.
        /// </summary>
        /// <param name="tWriter">The output stream.</param>
        /// <param name="nIndex">The index of the bone.</param>
        /// <param name="trans">The translation.</param>
        /// <param name="rot">The rotation.</param>
        static void WriteBone(this StreamWriter tWriter, int nIndex, Vector4 trans, Vector4 rot)
        {
            //Most of the values we are getting are close to zero, so let's just go ahead and clamp them.
            trans.x = MathHelpers.ClampToZero(trans.x);
            trans.y = MathHelpers.ClampToZero(trans.y);
            trans.z = MathHelpers.ClampToZero(trans.z);

            //Most of the values we are getting are close to zero, so let's just go ahead and clamp them.
            rot.x = MathHelpers.ClampToZero(rot.x);
            rot.y = MathHelpers.ClampToZero(rot.y);
            rot.z = MathHelpers.ClampToZero(rot.z);

            //This is mainly due to glitches while debugging the animations. We shouldn't HAVE to check for NaN, but just to be safe we'll go ahead and do it anyway.
            if (float.IsNaN(rot.x))
                rot.x = 0.0f;
            if (float.IsNaN(rot.y))
                rot.y = 0.0f;
            if (float.IsNaN(rot.z))
                rot.z = 0.0f;

            //Write out our data.
            tWriter.WriteLine(nIndex + "  " +
                trans.x + " " + trans.y + " " + trans.z + "  " +
                rot.x + " " + rot.y + " " + rot.z);
        }
    }
}