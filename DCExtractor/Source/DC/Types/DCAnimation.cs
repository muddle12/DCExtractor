using Custom.Math;
using System.Collections.Generic;

namespace DC.Types
{
    /// <summary>
    /// Class   :   "Animation"
    /// 
    /// Purpose :   describes a series of keyframes that orient a skeleton in a interpolated sequence to give the illusion of motion. Moving pictures stuff.
    /// 
    /// These are loaded exclusively by MOT.cs
    /// </summary>
    public class Animation
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Classes.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Class   :   "ChannelHeader"
        /// 
        /// Purpose :   the header before the keyframes which denotes which bone these keyframes are targeting.
        /// </summary>
        public struct ChannelHeader
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Data Members.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            public int boneIndex;               //The bone that this set of keyframes influences.
            public int Unknown2;                //appears to be unused.
            public int channelType;             //See Channel.ChannelType enum below.
            public int headerByteSize;          //The total size of this header. Always 32 bytes.
            public int keyframeCount;           //The number of keyframes in this channel.
            public int animationByteSize;       //Unknown, 
            public int Unknown4;                //appears to be unused.
            public int Unknown5;                //appears to be unused.
        }

        /// <summary>
        /// Class   :   "KeyFrame"
        /// 
        /// Purpose :   a single keyframe of data in the animation.
        /// </summary>
        public struct KeyFrame
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Data Members.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            public int index;                   //The index of the keyframe.
            public Vector4 position;            //The position data for this keyframe.
            public Vector4 quatRotation;        //The quaternion rotation data for this keyframe.
            public Vector4 scale;               //The scale data for this keyframe.
            public bool needsInterpolation;     //In order to properly expand the keyframes, we need to know which frames need interpolation.

            //Calculated data. This is leftover from when I was trying out different orientations.
            //public Vector4 localPosition;       
            //public Vector4 localRotation;
            //public Vector4 localScale;

            //public Matrix4x4 ToMatrix4x4()
            //{
            //    Matrix4x4 tTranslate = Matrix4x4.GenerateTranslation(position.x, position.y, position.z);

            //    Vector4 tEulers = quatRotation.ToEulersFromQuaternion();
            //    Matrix4x4 tRotX = Matrix4x4.GenerateRotationX(MathHelpers.ToDegrees(tEulers.x));
            //    Matrix4x4 tRotY = Matrix4x4.GenerateRotationY(MathHelpers.ToDegrees(tEulers.y));
            //    Matrix4x4 tRotZ = Matrix4x4.GenerateRotationZ(MathHelpers.ToDegrees(tEulers.z));

            //    Matrix4x4 tScale = Matrix4x4.GenerateScale(scale.x, scale.y, scale.z);

            //    return tTranslate * tRotX * tRotY * tRotZ * tScale;
            //}

            //public void FromMatrix4x4Local(Matrix4x4 tMatrix)
            //{
            //    localPosition = tMatrix.translation;
            //    localRotation = tMatrix.rotation;
            //    localScale = tMatrix.scale;
            //}
        }

        /// <summary>
        /// Class   :   "Channel"
        /// 
        /// Purpose :   defines a single array of keyframes for a specific bone.
        /// </summary>
        public class Channel
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Enumerations.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Denotes what kind of animation data is present in the keyframes following the bone header.
            /// </summary>
            public enum ChannelType
            {
                Translation = 2,    //The keyframe data is 3-float vectors(x,y,z) with position data.
                Rotation = 0,       //The keyframe data is 4-float quaternions(w,x,y,z) with rotation data.
                Scale = 1,          //The keyframe data is 3-float vectors(x,y,z) with scale data.
                ScaleSingle = 40    //The keyframe data is 1 float with uniform scale data.
            };


            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Data Members.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            public ChannelType channelType;     //The type of data this channel will manipulate.
            public int boneIndex;               //The bone index, or the index into the bone list that this channel is targeting.
            public KeyFrame[] keyFrames;        //The keyframes.


            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Functions.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="eChannelType">The type of data held by the channel.</param>
            /// <param name="nBoneIndex">The index of the bone this channel drives.</param>
            /// <param name="nKeyFrameCount">The number of keyframes in this channel.</param>
            public Channel(ChannelType eChannelType, int nBoneIndex, int nKeyFrameCount)
            {
                channelType = eChannelType;
                boneIndex = nBoneIndex;
                keyFrames = new KeyFrame[nKeyFrameCount];

                //Set these values to defaults.
                for (int key = 0; key < nKeyFrameCount; key++)
                {
                    keyFrames[key].quatRotation = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);  //quaternion identity.
                    keyFrames[key].scale = new Vector4(1, 1, 1, 1);                     //1 scale.
                }
            }

            /// <summary>
            /// Interpolates the frame at the target using surrounding frames.
            /// </summary>
            /// <param name="tKeyFrames">The list of keyframes.</param>
            /// <param name="nTargetFrame">The target frame that will be interpolated.</param>
            static void InterpolateFrame(KeyFrame[] tKeyFrames, int nTargetFrame)
            {
                int nPreviousFrame = nTargetFrame - 1;
                int nNextFrame = nTargetFrame + 1;

                //Find the next suitable frame.
                for (int i = nTargetFrame; i < tKeyFrames.Length; i++)
                {
                    if (tKeyFrames[i].needsInterpolation == false)
                    {
                        nNextFrame = i;
                        break;
                    }
                }

                //Find a previous suitable frame.
                for (int i = nTargetFrame; i >= 0; i--)
                {
                    if (tKeyFrames[i].needsInterpolation == false)
                    {
                        nPreviousFrame = i;
                        break;
                    }
                }

                //Figure out our inputs for lerp.
                KeyFrame tNext = tKeyFrames[nNextFrame];
                KeyFrame tPrevious = tKeyFrames[nPreviousFrame];
                float fTime = (float)(nTargetFrame - nPreviousFrame) / (float)(nNextFrame - nPreviousFrame);

                //Lerp the keyframes.
                tKeyFrames[nTargetFrame].position = Vector4.LerpVector(tPrevious.position, tNext.position, fTime);
                tKeyFrames[nTargetFrame].quatRotation = Vector4.LerpQuaternion(tPrevious.quatRotation, tNext.quatRotation, fTime);
                tKeyFrames[nTargetFrame].scale = Vector4.LerpVector(tPrevious.scale, tNext.scale, fTime);

                //We've interpolated this frame, set interpolation to false.
                tKeyFrames[nTargetFrame].needsInterpolation = false;
            }

            /// <summary>
            /// Expands the channel's keyframes to the new keyframe length.
            /// </summary>
            /// <param name="nNewKeyframeCount">The new keyframe count for the channel.</param>
            public void Expand(int nNewKeyframeCount)
            {
                //Only expand if we have less keyframes than nNewKeyframeCount.
                if (keyFrames.Length > 0 && keyFrames.Length >= nNewKeyframeCount)
                    return;

                //Save the previous values before we start.
                bool bFound = false;

                //Allocate a new list of keyframes to match the new expanded size.
                KeyFrame[] tNewKeyframes = new KeyFrame[nNewKeyframeCount];
                for (int key = 0; key < keyFrames.Length; key++)
                {
                    //We'll just set the keyframe index first.
                    bFound = false;
                    tNewKeyframes[key].index = key;
                    tNewKeyframes[key].needsInterpolation = false;

                    //Now, we'll loop through and find the matching index with the new key index.
                    for (int oldkeys = 0; oldkeys < keyFrames.Length; oldkeys++)
                    {
                        //If we find this index, that means there is data to save.
                        if (keyFrames[oldkeys].index == key)
                        {
                            //Save that data.
                            tNewKeyframes[keyFrames[oldkeys].index] = keyFrames[oldkeys];
                            bFound = true;
                            break;
                        }
                    }
                    
                    //Otherwise, there is a gap in the keyframes, we'll need to flag that one for interpolation.
                    if (bFound == false)
                        tNewKeyframes[key].needsInterpolation = true;
                }

                //Go through and interpolate the frames accordingly.
                for (int key = 0; key < tNewKeyframes.Length; key++)
                {
                    if (tNewKeyframes[key].needsInterpolation)
                        InterpolateFrame(tNewKeyframes, key);
                }

                //Assign the new keyframes.
                keyFrames = tNewKeyframes;
            }

            /// <summary>
            /// Merges the two channels together, overlapping their data.
            /// </summary>
            /// <param name="tChannel">The channel to merge with.</param>
            public void Merge(Channel tChannel)
            {
                //Let's quit if we can't merge.
                if (tChannel == this ||
                    tChannel.boneIndex != boneIndex ||
                    tChannel.keyFrames.Length != keyFrames.Length)
                    return;

                //We want to loop through every keyframe and try to match it to the frame of our of those in our list.
                for (int theirKeys = 0; theirKeys < tChannel.keyFrames.Length; theirKeys++)
                {
                    for (int ourKeys = 0; ourKeys < keyFrames.Length; ourKeys++)
                    {
                        //If the keyframe index matches, we can merge.
                        if (tChannel.keyFrames[theirKeys].index == keyFrames[ourKeys].index)
                        {
                            //We only want to merge the relevant data in the channel. Don't overwrite other stuff that's not ours to overwrite.
                            switch (tChannel.channelType)
                            {
                                case ChannelType.Translation:
                                    keyFrames[ourKeys].position = tChannel.keyFrames[theirKeys].position;
                                    break;
                                case ChannelType.Rotation:
                                    keyFrames[ourKeys].quatRotation = tChannel.keyFrames[theirKeys].quatRotation;
                                    break;
                                case ChannelType.Scale:
                                    keyFrames[ourKeys].scale = tChannel.keyFrames[theirKeys].scale;
                                    break;
                                case ChannelType.ScaleSingle:
                                    keyFrames[ourKeys].scale = tChannel.keyFrames[theirKeys].scale;
                                    break;
                            }
                            break;
                        }
                    }
                }
            }

            /// <summary>
            /// Slices the channel based on the start and end frame and returns the result.
            /// </summary>
            /// <param name="nStart">The start of the slice.</param>
            /// <param name="nEnd">The end of the slice.</param>
            /// <returns>A slice of the original channel.</returns>
            public Channel Slice(int nStart, int nEnd)
            {
                Channel tSliced = new Channel(channelType, boneIndex, nEnd - nStart);
                for (int key = 0; key < tSliced.keyFrames.Length && nStart + key < keyFrames.Length; key++)
                    tSliced.keyFrames[key] = keyFrames[nStart + key];
                return tSliced;
            }

            /// <summary>
            /// Clones the channel and returns it.
            /// </summary>
            /// <returns>A copy of this channel.</returns>
            public Channel Clone()
            {
                Channel tClone = new Channel(channelType, boneIndex, ((keyFrames != null) ? keyFrames.Length : 0));
                if (keyFrames != null)
                {
                    tClone.keyFrames = new KeyFrame[keyFrames.Length];
                    for (int i = 0; i < keyFrames.Length; i++)
                        tClone.keyFrames[i] = keyFrames[i];
                }
                return tClone;
            }
        }

        /// <summary>
        /// Class   :   "AnimationSlice"
        /// 
        /// Purpose :   Animations are held in one long animation clip inside the mot file. The info.cfg defines the individual animations inside of this
        /// master clip; slices of that master animation. This class holds the information extracted from the info.cfg which defines that slice.
        /// </summary>
        public class AnimationSlice
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Enumerations.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Describes the current read state during config parsing. Used in parsing the keyframe data from the config script.
            /// </summary>
            public enum ReadState { KeyStart, Key, KeyEnd};


            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Data Members.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            public string name;         //The name of the animation clip. I'd love to auto translate the japanese to english, but that's outside the scope of this project.
            public int start;           //The start frame of the animation slice.
            public int end;             //The end frame of the animation slice.
            public float speed;         //The speed of the animation.


            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Functions.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="szName">The name of this animation slice.</param>
            /// <param name="nStartFrame">The starting frame from the master animation that this slice starts at.</param>
            /// <param name="nEndFrame">The ending frame from the master animation that this slice ends at.</param>
            /// <param name="fSpeed">The speed of this animation.</param>
            public AnimationSlice(string szName, int nStartFrame, int nEndFrame, float fSpeed)
            {
                name = szName;
                start = nStartFrame;
                end = nEndFrame;
                speed = fSpeed;
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Data Members.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Channel[] channels = null;       //Effectively, the bones and their associated keyframes.
        public float speed = 1.0f;              //The speed of this animation. This gets handed over from the clips during the slice.


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tChannels">The channels for this animation.</param>
        public Animation(Channel[] tChannels, float fSpeed = 1.0f)
        {
            channels = tChannels;
            speed = fSpeed;
        }

        /// <summary>
        /// Converts the animation's frames into local space relative to the bone list passed.
        /// </summary>
        /// <param name="bones">The list of reference bones for use in the transform conversion.</param>
        //public void ToLocalSpace(Bone[] bones)
        //{
        //    Matrix4x4 tKeyFrameMatrix;
        //    Matrix4x4 tLocalKeyframeMatrix = new Matrix4x4();
        //    for (int chan = 0; chan < channels.Length; chan++)
        //    {
        //        for (int key = 0; key < channels[chan].keyFrames.Length; key++)
        //        {
        //            tKeyFrameMatrix = channels[chan].keyFrames[key].ToMatrix4x4();

        //            tLocalKeyframeMatrix = tKeyFrameMatrix * bones[channels[chan].boneIndex].worldInverse;

        //            channels[chan].keyFrames[key].FromMatrix4x4Local(tLocalKeyframeMatrix);
        //        }
        //    }
        //}

        /// <summary>
        /// Combines the channels of the animation together, expanding and interpolating channels together.
        /// </summary>
        public void CombineChannels()
        {
            //First, we'll need to sort our channels into buckets of similar bone index.
            bool bFound = false;
            List<List<Channel>> tChannelSets = new List<List<Channel>>();
            for (int chan = 0; chan < channels.Length; chan++)
            {
                //Loop through all the sets comparing bone indices.
                bFound = false;
                for (int set = 0; set < tChannelSets.Count; set++)
                {
                    //If this channel matches the channel set's bone index.
                    if (channels[chan].boneIndex == tChannelSets[set][0].boneIndex)
                    {
                        //Add that channel to this set.
                        tChannelSets[set].Add(channels[chan]);
                        bFound = true;
                        break;
                    }
                }
                //If we didn't find a match, create a new bucket and add the channel to that.
                if (bFound == false)
                {
                    tChannelSets.Add(new List<Channel>());
                    tChannelSets[tChannelSets.Count - 1].Add(channels[chan]);
                }
            }

            //Next, we'll need to find the highest keyframe count for each bucket.
            int nHigh = 0;
            for (int set = 0; set < tChannelSets.Count; set++)
            {
                //Loop through each bucket and record the highest keyframe count.
                nHigh = 0;
                for (int chan = 0; chan < tChannelSets[set].Count; chan++)
                {
                    if (tChannelSets[set][chan].keyFrames.Length > nHigh)
                        nHigh = tChannelSets[set][chan].keyFrames.Length;
                }

                //Loop through again and expand each of bucket's channels to the new keyframe count.
                for (int chan = 0; chan < tChannelSets[set].Count; chan++)
                {
                    tChannelSets[set][chan].Expand(nHigh);
                }
            }

            //We're going to loop through and combine our similar channels.
            Channel[] tNewChannels = new Channel[tChannelSets.Count];
            for (int chan = 0; chan < tChannelSets.Count; chan++)
            {
                //Copy over the first channel.
                tNewChannels[chan] = tChannelSets[chan][0];

                //If there is more than one channel, we'll need to merge.
                if (tChannelSets[chan].Count > 1)
                {
                    //Loop through and merge each subchannel with the first one.
                    for (int subChan = 1; subChan < tChannelSets[chan].Count; subChan++)
                        tNewChannels[chan].Merge(tChannelSets[chan][subChan]);
                }
            }

            //Assign our new channels.
            channels = tNewChannels;
        }

        /// <summary>
        /// Creates a list of animations that have been split by the list of slices passed.
        /// </summary>
        /// <param name="tSlices">The array of slices.</param>
        /// <returns>An array of animations split from this animation.</returns>
        public Animation[] Slice(AnimationSlice[] tSlices)
        {
            //Return ourselves if the slices are invalid.
            if (tSlices == null || tSlices.Length == 0)
                return new Animation[] { this };

            List<Animation> tSlicedAnimations = new List<Animation>();
            List<Channel> tSlicedChannels = new List<Channel>();

            //We basically need to slice the channels out of the master into separate animations.
            for (int slice = 0; slice < tSlices.Length; slice++)
            {
                //Clear the slice list for the next slice.
                tSlicedChannels.Clear();

                //Loop through each channel and slice according to the start and end frames.
                for (int chan = 0; chan < channels.Length; chan++)
                    tSlicedChannels.Add(channels[chan].Slice(tSlices[slice].start, tSlices[slice].end));

                //Add that slice to the output list of sliced animations.
                tSlicedAnimations.Add(new Animation(tSlicedChannels.ToArray(), tSlices[slice].speed));
            }

            //Return all the slices.
            return tSlicedAnimations.ToArray();
        }

        /// <summary>
        /// Clones the animation and returns it.
        /// </summary>
        /// <returns>A copy of this animation.</returns>
        public Animation Clone()
        {
            Channel[] tChannels = null;
            if (channels != null)
            {
                tChannels = new Channel[channels.Length];
                for (int chan = 0; chan < channels.Length; chan++)
                    tChannels[chan] = channels[chan].Clone();
            }
            return new Animation(channels, speed);
        }
    }
}
