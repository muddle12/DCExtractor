using Custom.Math;
using System.Collections.Generic;

namespace DC.Types
{
    /// <summary>
    /// Class   :   "Bone"
    /// 
    /// Purpose :   represents a bone in a Level-5 mds model.
    /// </summary>
    public class Bone
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Constants.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public const int BoneNameLength = 32;       //The name field for each bone is a fixed size. There may be garbage in this section, so remember to read up until you hit a null terminator.


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Data Members.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public int index;                           //The index of this bone in the hierarchy.
        public int size;                            //The size of this header.
        public string name;                         //The name of this bone. The name section is always 32 characters long, even if the name is not that length. The header can be filled with garbage in the name field after the null terminator.
        public int associatedMDTOffset;             //This is super weird, and is probably a hack on the devs part, but the bones can either be regular bones in the hierarchy, or they can be associated with a specific mesh. They only associate using the offset.
        public int parentIndex;                     //The index of the parent bone.
        public Matrix4x4 local = new Matrix4x4();   //I'm assuming this matrix is the local matrix of the bone, in some kind of default bind pose.

        //Calculated data.
        public Bone parent;
        public List<Bone> children = new List<Bone>();

        //Loaded from the bbp file.
        public Matrix4x4 bind = new Matrix4x4();

        //Debug data. I was using this to figure out what was wrong with my hierarchy. No dice, I didn't make much progress here.
        public Matrix4x4 world = new Matrix4x4();           //The world position of each bone. For exporting purposes, this information is irrelevant.
        public Matrix4x4 worldInverse = new Matrix4x4();    //The inverse world position of each bone. Can be used to transform animations from world space.
        //public Matrix4x4 final = new Matrix4x4();         //Really only useful if we were doing skinned animation in our own game engine. This information is irrelevant.
        public bool hasBindPose = false;                    //Whether or not the bind pose loaded.
        public bool hasCalculatedWorld = false;             //Whether or not we calculated the world matrix.



        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns the string equivalent of the object.
        /// </summary>
        /// <returns>The string equivalent of the object.</returns>
        public override string ToString()
        {
            return name + ":" + index;
        }

        /// <summary>
        /// Calculates the hierarchy for this bone.
        /// </summary>
        public void CalculateHierarchy()
        {
            if (parent != null && parent.children.Contains(this) == false)
                parent.children.Add(this);
        }

        /// <summary>
        /// Calculates the world matrix for this bone.
        /// </summary>
        /// <returns>The matrix that was calculated.</returns>
        [System.Obsolete]
        public Matrix4x4 CalculateWorldMatrix()
        {
            if (hasCalculatedWorld == false)
            {
                //If we have a parent bone, we need to calculate our world matrix by flattening the hierarchy recursively.
                if (parent != null)
                    world = local * parent.CalculateWorldMatrix();
                //Otherwise our local is our world matrix.
                else
                    world = local.Clone();
                //No clue if this still works. We're inversing due to a limitation of smd, but I don't know if we should preform this step at this point.
                worldInverse = world.Inversed();
                hasCalculatedWorld = true;
            }
            return world;
        }

        /// <summary>
        /// Clones the bone's data.
        /// </summary>
        /// <returns>A clone of this bone.</returns>
        public Bone Clone()
        {
            Bone tCopy = new Bone();
            tCopy.index = index;
            tCopy.size = size;
            tCopy.name = name;
            tCopy.associatedMDTOffset = associatedMDTOffset;            
            tCopy.parentIndex = parentIndex;
            //tCopy.world = new Matrix4x4(world);
            tCopy.bind = new Matrix4x4(bind);
            tCopy.local = new Matrix4x4(local);
            //tCopy.final = new Matrix4x4(final);

            tCopy.hasBindPose = hasBindPose;
            tCopy.hasCalculatedWorld = hasCalculatedWorld;

            return tCopy;
        }
    }
}