using System.Collections.Generic;
using System.IO;

namespace DC.Types
{
    /// <summary>
    /// Class   :   "Model"
    /// 
    /// Purpose :   defines an entire model used by the Level-5 MDS format. For all intents and purposes, this IS an MDS.
    /// </summary>
    public class Model
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Constants.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public const uint ModelIdentifier = 0x4D445300;     //MDS
        public const uint MeshIdentifier = 0x4D445400;      //MDT


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Data Members.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint Header;             //The 0x4D445300 MDS0 header marker.
        public int Version;             //The version number, or so I'm told. I've not seen any models that use something other than 1.
        public int BoneCount;           //The number of bones following the initial MDS header.
        public int BoneOffset;          //The offset from Offset to the bone section.
        public Bone[] Bones;            //A list of bones for the model's hierarchy.
        public Mesh[] Meshes;           //A list of meshes that contain the render data for the model.
        public VertexWeight[] Weights;  //A list of vertex weights in a raw compressed form. They'll need to be propogated to the vertices in order to be used.
        public Animation[] Animations;  //A list of animations loaded from the motion file and split by the .cfg.


        //Calculated.
        public long Offset;             //The offset of the MDS in the file. This is calculated by the seek, it is not actually found in the original specification.
        public string Name;             //The name of the model, which is found 80 bytes before the MDS section. Again, this is unused since standalone mds files do not have the 80 byte pre-header.
        public string filePath = string.Empty;  //Calculated by the load function. It's just the file path of the mds we loaded.


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Properties.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets whether or not the model has weights.
        /// </summary>
        public bool hasWeights
        {
            get { return Weights != null && Weights.Length > 0; }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets the vertex weight based on the vertex index.
        /// </summary>
        /// <param name="nMeshIndex">The mesh the weights are targeting.</param>
        /// <param name="nVertexIndex">The absolute vertex index into the vertex array for the entire model.</param>
        /// <returns>The vertex weight, otherwise null.</returns>
        public VertexWeight[] GetWeights(int nMeshIndex, int nVertexIndex)
        {
            /**************************************************************/
            //  Weights in MDS are stored in this lopsided way where the
            //  weight targets a specific mesh and vertex within said mesh.
            //  It was likely a later addition to the format after static
            //  meshes had been created, which is why WGT is a separate
            //  file. Anyway, we need to match the correct mesh AND the
            //  correct vertex index before we can return a weight.
            /**************************************************************/
            List<VertexWeight> tWeights = new List<VertexWeight>();
            if (Weights != null)
            {
                for (int i = 0; i < Weights.Length; i++)
                {
                    if (Weights[i].meshIndex == nMeshIndex && 
                        Weights[i].vertexIndex == nVertexIndex)
                        tWeights.Add(Weights[i]);
                }
            }
            return tWeights.ToArray();
        }

        /// <summary>
        /// Splits the model's meshes into several models.
        /// </summary>
        /// <returns></returns>
        public Model[] Split()
        {
            /****************************************************************/
            //  This method exists because originally I was using .obj as
            //  my export format for testing, and .obj can't support multiple
            //  meshes.
            //
            //  This function is basically a glorified clone function that
            //  chops all of the meshes out of the calling model and returns
            //  them as individual models.
            /****************************************************************/
            Model[] tModels = new Model[Meshes.Length];
            for (int mesh = 0; mesh < Meshes.Length; mesh++)
            {
                tModels[mesh] = new Model();
                tModels[mesh].Offset = Offset;
                tModels[mesh].Name = Name;
                tModels[mesh].Header = Header;
                tModels[mesh].Version = Version;
                tModels[mesh].BoneCount = BoneCount;
                tModels[mesh].BoneOffset = BoneOffset;

                tModels[mesh].Bones = new Bone[Bones.Length];
                for (int bone = 0; bone < Bones.Length; bone++)
                    tModels[mesh].Bones[bone] = Bones[bone].Clone();

                tModels[mesh].Meshes = new Mesh[1] { Meshes[mesh].Clone() };

                if (Weights != null)
                {
                    tModels[mesh].Weights = new VertexWeight[Weights.Length];
                    for (int weight = 0; weight < Weights.Length; weight++)
                        tModels[mesh].Weights[weight] = Weights[weight];
                }

                if (Animations != null)
                {
                    tModels[mesh].Animations = new Animation[Animations.Length];
                    for (int anim = 0; anim < Animations.Length; anim++)
                        tModels[mesh].Animations[anim] = Animations[anim].Clone();
                }

                string szFileName = Path.GetFileNameWithoutExtension(filePath) + "_mesh" + mesh + ".mds";
                tModels[mesh].filePath = Path.Combine(Path.GetDirectoryName(filePath), szFileName);
            }
            return tModels;
        }
    }
}