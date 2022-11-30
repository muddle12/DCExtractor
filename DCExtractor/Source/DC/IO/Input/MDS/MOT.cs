using Custom.IO;
using Custom.Math;
using DC.Types;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace DC.IO
{
    /// <summary>
    /// Class   :   "MOT"
    /// 
    /// Purpose :   provides functions for loading Level-5's MOT format.
    /// 
    /// The MOT contains ALL of the individual animations for a model, in one long master animation.
    /// 
    /// Animation data is stored in two different 32-byte blocks: The channel header, and the keyframe data.
    /// 
    /// Channel Headers specify which bone is being animated, what kind of keyframes are to follow(translation, rotation, scale, and uniform scale), and how
    /// many keyframes there are in this channel.
    /// 
    /// It is possible to see a Channel Header for the same bone multiple times. This is due to the way keyframes only store one type of transformation at a time.
    /// For example: Bone 4 might have one channel that only has translation data, and another channel for rotation. When parsing the animation data, you can
    /// safely combine these channels together, as they should have the same number of keyframes.
    /// 
    /// At least, for Dark Cloud 2. Dark Cloud 1 likes to mix and match. I haven't looked into why, as animations don't work anyway due to the rigging issue.
    /// 
    /// After a channel, if the keyframe count is non-zero, will be keyframes. Keyframes come in four types, specified by the header.
    /// Translation is 3 floats vector (x,y,z) with one empty float.
    /// Rotation is a 4 float quaternion (w, x, y, z). I'm not certain the first component is w, or if the last component is w. My bets are on w,x,y,z.
    /// Scale is a 3 float vector (x,y,z) with one empty float.
    /// Uninform Scale is one float, which covers all axes uniformly(x,y,z).
    /// 
    /// Another quirk of the MOT format is the info.cfg that comes with each model. Inside the info.cfg is a bit of script that defines the individual animations and
    /// where they start and end in the master animation. These are denoted by KEY_START, KEY, and KEY_END. Those individual slices/clips must first be read in 
    /// from the info.cfg, and then the master animation must be chopped up according to the start and end frames of the individual clips. 
    /// Those animation slices can then be output as individual animations.
    /// 
    /// For reference (info.cfg):
    /// KEY_START   =   the beginning of the slice data.
    /// KEY         =   denotes an individual animation. Parameters("name", startFrame, endFrame, speed)
    /// KEY_END     =   the end of the slice data.
    /// 
    /// Notepad++ is able to read the original japanese from the info.cfg, which you can plug into Google translate and it will return the appropriate name in the language of your choosing.
    /// 
    /// 
    /// So, if you're here, I'm willing to bet you're looking into how to fix the animation export issue that plagued me for almost a month.
    /// 
    /// I've exhausted all of my knowledge and resources on the subject, I'm at a loss on what might be the issue. I've exhaustively tested both the rig and
    /// the animation data using external tools and even hotloading the assets into game engines like Source and Unity. I suspect I'm missing a crucial piece
    /// of information somewhere nested inside this format, but I can't seem to smoke it out.
    /// 
    /// From what I can tell though, the animation data is correct. I've rigorously tested the math functions in several external applications and online
    /// calculators, and also against outputs from programs like Unity and Blender. I'm fairly confident it's not the math that's failing.
    /// 
    /// Also, based on the outputs form external tools, as well as redoing the loading code for animations several times, I'm also confident that the
    /// animation data is correct as well.
    /// 
    /// Which leaves the skeleton. Check the BBP.cs for more info.
    /// 
    /// 
    /// If you are viewing motion data with a hex editor, it is best viewed at a 32 bytes per row. 
    /// </summary>
    public static class MOT
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Loads a motion file containing the animations for this model.
        /// </summary>
        /// <param name="szMOTPath">The path to the motion file.</param>
        /// <returns>A list of animations.</returns>
        public static Animation[] Load(string szMOTPath)
        {
            //load in our mot file.
            BinaryReader tReader = FileStreamHelpers.OpenBinaryReader(szMOTPath);
            if (tReader == null)
                return null;

            //establish some variables to hold the entire run.
            Animation.ChannelHeader tChannelHeader;
            Animation.Channel tCurChannel;
            List<Animation.Channel> tChannels = new List<Animation.Channel>();

            //loop through the file.
            while (tReader.BaseStream.Position < tReader.BaseStream.Length)
            {
                //read the header first and create a channel for it.
                tReader.ReadAnimationChannelHeader(out tChannelHeader);

                //Create a new channel.
                tCurChannel = new Animation.Channel((Animation.Channel.ChannelType)tChannelHeader.channelType, tChannelHeader.boneIndex, tChannelHeader.keyframeCount);

                //loop through and scoop up all the keyframes.
                for (int frame = 0; frame < tChannelHeader.keyframeCount; frame++)
                    tReader.ReadAnimationKeyframe(ref tCurChannel.keyFrames[frame], (Animation.Channel.ChannelType)tChannelHeader.channelType);

                //Add the channel to the list.
                tChannels.Add(tCurChannel);
            }

            //We're done with the motion file.
            tReader.Close();

            //Create our master animation.
            Animation tMaster = new Animation(tChannels.ToArray());

            /*******************************************************************/
            //  Right, so, channels can have different keyframe counts. You can't
            //  rely on two different channels targeting the same bone to have
            //  the same keyframe counts.
            /*******************************************************************/
            //Combine the duplicate channels together.
            tMaster.CombineChannels();

            //This is where things get tricky, the entire run of keyframes encompasses all of the animations for the model. 
            //The only way to know where each individual animation begins or ends is to load the info.cfg.
            string szCFGPath = Path.Combine(Path.GetDirectoryName(szMOTPath), "info.cfg");
            if (File.Exists(szCFGPath))
            {
                Animation.AnimationSlice[] tSlices = LoadSlicesFromCFG(szCFGPath);
                if (tSlices != null && tSlices.Length > 0)
                    return tMaster.Slice(tSlices);
            }

            //If there is no cfg, just return the master.
            return new Animation[] { tMaster };
        }

        /// <summary>
        /// Reads a header from an animation file stream.
        /// </summary>
        /// <param name="tReader">The input stream.</param>
        /// <param name="tHeader">The header to read.</param>
        static void ReadAnimationChannelHeader(this BinaryReader tReader, out Animation.ChannelHeader tHeader)
        {
            tHeader.boneIndex = tReader.ReadInt32();
            tHeader.Unknown2 = tReader.ReadInt32();
            tHeader.channelType = tReader.ReadInt32();
            tHeader.headerByteSize = tReader.ReadInt32();
            tHeader.keyframeCount = tReader.ReadInt32();
            tHeader.animationByteSize = tReader.ReadInt32();
            tHeader.Unknown4 = tReader.ReadInt32();
            tHeader.Unknown5 = tReader.ReadInt32();
        }

        /// <summary>
        /// Reads an animation frame from an animation file stream.
        /// </summary>
        /// <param name="tReader">The input stream.</param>
        /// <param name="tKeyFrame">The output keyframe to fill with data.</param>
        static void ReadAnimationKeyframe(this BinaryReader tReader, ref Animation.KeyFrame tKeyFrame, Animation.Channel.ChannelType eType)
        {
            tKeyFrame.index = tReader.ReadInt32() - 1;
            tReader.BaseStream.Position += 12;//Empty bytes.

            switch (eType)
            {
                case Animation.Channel.ChannelType.Translation:
                    //tReader.BaseStream.Position += 16;
                    tKeyFrame.position = tReader.ReadVector4();
                    break;
                case Animation.Channel.ChannelType.Rotation:
                    //tReader.BaseStream.Position += 16;
                    tKeyFrame.quatRotation = tReader.ReadQuaternionWXYZ();
                    break;
                case Animation.Channel.ChannelType.Scale:
                    //tReader.BaseStream.Position += 16;
                    tKeyFrame.scale = tReader.ReadVector4();
                    break;
                case Animation.Channel.ChannelType.ScaleSingle:
                    {
                        //tReader.BaseStream.Position += 16;
                        float fScale = tReader.ReadSingle();
                        tKeyFrame.scale = new Vector4(fScale, fScale, fScale, 1.0f);
                        tReader.BaseStream.Position += 12;
                    }
                    break;
                    //Unknown?
                default:
                    {
                        tReader.BaseStream.Position += 16;
                    }
                    break;
            }
        }

        /// <summary>
        /// Loads the animation slices from the info.cfg file.
        /// </summary>
        /// <param name="szCFGPath">The path to the info.cfg</param>
        /// <returns>A list of animation slices.</returns>
        static Animation.AnimationSlice[] LoadSlicesFromCFG(string szCFGPath)
        {
            //open the .cfg.
            StreamReader tCFGReader = FileStreamHelpers.OpenStreamReader(szCFGPath);
            if (tCFGReader == null)
                return new Animation.AnimationSlice[0];

            List<Animation.AnimationSlice> tSlices = new List<Animation.AnimationSlice>();
            string szLine;
            bool bQuit = false;
            Animation.AnimationSlice.ReadState eState = Animation.AnimationSlice.ReadState.KeyStart;

            //read through the file and parse the tags to find the key data.
            while (tCFGReader.EndOfStream == false && bQuit == false)
            {
                szLine = tCFGReader.ReadLine();
                switch (eState)
                {
                    //KEY_START;
                    case Animation.AnimationSlice.ReadState.KeyStart:
                        {
                            if (szLine.StartsWith("KEY_START;"))
                                eState = Animation.AnimationSlice.ReadState.Key;
                        }
                        break;
                    //KEY name, startKeyframe, endKeyframe, speed;
                    case Animation.AnimationSlice.ReadState.Key:
                        {
                            if (szLine.StartsWith("KEY_END;"))
                            {
                                eState = Animation.AnimationSlice.ReadState.KeyEnd;
                            }
                            else if (szLine.Contains("KEY"))
                            {
                                //Mostly just parsing plain text and filling out the appropriate variables.
                                szLine = szLine.Substring(szLine.IndexOf('Y') + 1);

                                string[] szData = szLine.Split(new char[] { ',', ';' });
                                for (int i = 0; i < szData.Length; i++)
                                    szData[i] = szData[i].Trim();

                                int nStartFrame = -1;
                                int nEndFrame = -1;
                                float fSpeed = 0.0f;

                                try
                                {
                                    nStartFrame = int.Parse(szData[1]);
                                    nEndFrame = int.Parse(szData[2]);
                                    fSpeed = float.Parse(szData[3]);
                                }
                                catch (System.Exception)
                                {
                                    MessageBox.Show("Could not parse info.cfg, some incorrect data was detected at " + szData[0] + " in:\n" + szCFGPath, "Error Parsing info.cfg");
                                    return new Animation.AnimationSlice[0];
                                }

                                tSlices.Add(new Animation.AnimationSlice(szData[0], nStartFrame, nEndFrame, fSpeed));
                            }
                        }
                        break;
                    //KEY_END;
                    case Animation.AnimationSlice.ReadState.KeyEnd:
                        bQuit = true;
                        break;
                }
            }
            tCFGReader.Close();

            //return the slices we parsed.
            return tSlices.ToArray();
        }

        /// <summary>
        /// Reads a vector4 quaternion in WXYZ format.
        /// </summary>
        /// <param name="tReader">The input stream.</param>
        /// <returns>A 16 byte Quaternion.</returns>
        static Vector4 ReadQuaternionWXYZ(this BinaryReader tReader)
        {
            float fW = tReader.ReadSingle();
            float fX = tReader.ReadSingle();
            float fY = tReader.ReadSingle();
            float fZ = tReader.ReadSingle();

            return new Vector4(fX, fY, fZ, fW);
        }
    }
}