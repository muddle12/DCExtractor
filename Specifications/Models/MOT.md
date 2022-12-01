File Specification:		Level-5 Motion Animation
------------------------------------------------------------------------------------------------

Extension:			.mot/.MOT

Purpose:			contains data that describes a sequence of 3D transformations of a series of bones in a skeletal hierarchy. 

Author:				Level-5(Game Developer, www.level5.co.jp)

Author Date:			2000

Applications:			Dark Cloud 1, Dark Cloud 2

Spec Author:			muddle12

Disclaimer:				This format is speculative. Only the original author knows the exact specification.
	This information was derived through reverse engineering and experimentation. Information may be incorrect or	
	incomplete.

Purpose(expanded):
------------------------------------------------------------------------------------------------

	This is a child format of the MDS format. If you are unfamiliar with the MDS format, I would 
	recommend reading the MDS specification first before reading the MOT specification.
	
	The MOT is a separate file that accompanies the MDS file. This file requires an associated MDS file in order 
	to be used.
	
	
	The MOT format is a container for animations. Animations are a series of individual
	keyframes, or transformations, which drive bones within a skeletal hierarchy. When these transformations
	are applied in sequence over time, it produces the illusion of motion. More details about
	how 3D animations work will not be elaborated on in this specification. I would advise researching this
	topic if you are not familiar with the terminology.
	
	The MOT format is divided into two types of structures: Channels and Keyframes. Both structures are
	32 bytes in size. For each Channel, there will be a list of Keyframes. After those Keyframes will be
	another Channel. This pattern continues until end of file.
	
	The Channel contains eight 4-byte integers. The first integer is the boneIndex, which is the index of a
	target bone in the MDS's skeletal hierarchy which will be driven by the following Keyframes. The second
	integer is padding. The third integer is the channelType, which can be one of five values: Translation(0x2), 
	Rotation(0x0), Scale(0x1), and ScaleUniform(0x28), and Unknown(0x32). channelType determines what kind of 
	Keyframes this Channel contains. The fourth integer is the headerByteSize, which is always 32 bytes. 
	The fifth integer is the keyframeCount, or the number of Keyframes following this Channel. 
	The sixth integer is the animationByteSize; It is unknown what the purpose of this value is. The 
	last two integers are unused padding.
	
	After the Channel is a list of Keyframes of keyframeCount in number. The first 4-byte integer is the current
	frame, which starts at 1(it is not zero-based). The next 12 bytes are padding. The last 16 bytes are 
	four 4-byte float values, which are interpreted based on the Channel's channelType. 
	
	If the channelType is Translation(0x2), three of these four values will be the x,y,z translation for the bone, 
	with the last value being unused. 
	
	If the channelType is Rotation(0x0), then the four values will be a Quaternion rotation. It is unknown what order
	these Quaternion's components are in, though it is likely either (w,x,y,z) or (x,y,z,w). My limited testing
	points to (w,x,y,z).
	
	If the channelType is Scale(0x1), then three of the four values will be the x,y,z scale for the bone, with the
	last value being unused.
	
	If the channelType is UniformScale(0x28), then the first value will be the scale for all three axes, x,y,z, with
	the last three values being unused.
	
	If the channelType is Unknown(0x32), then it's some kind of extremely rare keyframe type. It appears the first
	value is normalized, with the other three values being padding. This type only shows up in one or two files.
	It's best to just skip this section, as this data might be internal to the Dark Cloud engine.
	
	
	Due to the fact that a Channel may only contain one type of keyframe, it is possible to see multiple Channels
	referencing the same bone. For example, you might have a Channel that references bone 6, and has a channelType
	of Translation, with an array of 12 Translation Keyframes following it. After that is another Channel that
	also references bone 6, with a channelType of Rotation, with an array of 12 Rotation Keyframes following it.
	These Channels should be combined together when loading the animation data, as they are both driving different
	aspects of the bone's transformation on the same frames.
	
	It is worth noting that different channels may have different keyframe counts. You will need to remap these
	channels to larger keyframe arrays if you wish to combine them.
	
	
	Unlike conventional animation systems, where each individual animation for a character is split into several files,
	the MOT contains every animation back to back in one long master animation. For example, frame 0-30 might contain
	the walking animation, and frame 31-50 might contain the attack animation, so on and so forth.
	
	In order to extract these individual animations from the master, you will need to load another configuration file,
	either referred to as "info.cfg", or <MDSFileName>.cfg.
	
	The CFG is a script format that defines various commands and settings in relation to the MDS and MOT file. I will
	describe commands that are only relevant to the MOT format. Other commands may be detailed in the CFG specifiation or
	other specifications.
	
	Inside the CFG is several commands. The commands beginning with KEY_START, KEY, and KEY_END define the individual
	animation frames for the master. These commands will end with a semicolon(;) at the end of each line. KEY_START
	defines the beginning of the animation frame section. KEY_END defines the end of the animation frame section.
	
	KEY is a command that defines the name, starting frame, ending frame, and speed of the animation in the master.
	The first value after KEY is the name, which is usually surrounded with quotations(""). The text is usually in 
	2-byte unicode and in Japanese. The second value is the start frame, in integer. The third value is the end frame, in
	integer. The final value is the global speed of this animation, in floating point. I assume this value is a
	multiplier against 30 frames per second.
	
	Format: 	KEY (string)"name" (int)startFrame, (int)endFrame, (float)speed;
	Example:	KEY "立ち", 362, 364, 0.02;
	
	
	Once you have loaded in these KEY commands, you will then need to "slice" the master animation into sub-animations
	using the start and end frames. These are inclusive values. Slicing is the act of creating a new animation starting
	at the start frame of the master animation, copying values up to and including the end frame. For every KEY, there
	will be a new animation slice.
	
	
	While this system is not fully understood, I'm relatively confident with the information I've been able to
	gather on how MOT is laid out. However, due to issues I've encountered with the skeletal hierarchy and the BBP format, 
	I am not certain that this format is correct. Anyone else following up on this specification may have to 
	revise MOT's specification once the BBP format is better understood.

File Layout:
---------------------------
```cs
int32 == 4 byte integer
float32 == 4 byte floating point single
string == char array with null terminator
eof == end of file

MOT
{
	MOTChannel(32 bytes)
		int32 boneIndex;               //The bone that this set of keyframes influences.
		int32 Unknown1;                //appears to be unused.
		int32 channelType;             //See Channel.ChannelType enum below.
		int32 headerByteSize;          //The total size of this header. Always 32 bytes.
		int32 keyframeCount;           //The number of keyframes in this channel.
		int32 animationByteSize;       //Unknown.
		int32 Unknown2;                //appears to be unused.
		int32 Unknown3;                //appears to be unused.
		MOTKeyFrame(32 bytes)[keyframeCount] keyFrames;
			int32 frameIndex;		//The index of this keyframe.
			int32 padding1;			//Padding.
			int32 padding2;			//Padding.
			int32 padding3;			//Padding.
			float32 x;			//x-component.
			float32 y;			//y-component.
			float32 z;			//z-component.
			float32 w;			//w-component.
	MOTChannel(32 bytes)
	MOTChannel(32 bytes)
	...
	MOTChannel(32 bytes)
	eof
}
```

Implementation(pseudocode):
---------------------------
```cs
//Contains transformation data for bone animaton.
struct MOTKeyframe
{
	int32 frameIndex;		//The index of this keyframe.
	int32 padding1;			//Padding.
	int32 padding2;			//Padding.
	int32 padding3;			//Padding.
	float32 x;			//x-component.
	float32 y;			//y-component.
	float32 z;			//z-component.
	float32 w;			//w-component.
}

//Contains a list of keyframes for a single bone.
struct MOTChannel
{
	int32 boneIndex;               	//The bone that this set of keyframes influences.
	int32 Unknown1;                	//appears to be unused.
	int32 channelType;             	//See Channel.ChannelType enum below.
	int32 headerByteSize;          	//The total size of this header. Always 32 bytes.
	int32 keyframeCount;           	//The number of keyframes in this channel.
	int32 animationByteSize;       	//Unknown.
	int32 Unknown2;                	//appears to be unused.
	int32 Unknown3;                	//appears to be unused.
	MOTKeyframe[] keyFrames;	//The keyframes for this channel.
}

//Contains a list of animations for a hierarchy of bones.
struct MOTAnimation
{
	MOTChannel[] channels;
	float32 speed;
}

//Loads a MOT file and returns the master animation.
MOTAnimation LoadMOT(string szMOTFilePath)
{
	//Open our MOT file.
	InputStream input = File.Open(szMOTFilePath);
	
	//Create some temprary variables.
	MOTChannel tChannel;
	List<MOTChannel> tChannelList;
	
	//Loop until we reach EOF.
	while(input.EOF() == false)
	{
		//Since there is no magic, we assume the first entry is a channel.
		
		//Read in the channel.
		tChannel.boneIndex = input.ReadInt32();
		tChannel.Unknown1 = input.ReadInt32(); 
		tChannel.channelType = input.ReadInt32();
		tChannel.headerByteSize = input.ReadInt32();
		tChannel.keyframeCount = input.ReadInt32();
		tChannel.animationByteSize = input.ReadInt32();
		tChannel.Unknown2 = input.ReadInt32();
		tChannel.Unknown3 = input.ReadInt32();
		
		//Read each keyframe up to keyframeCount.
		tChannel.keyFrames = new MOTKeyframe[tChannel.keyframeCount];
		for(int key = 0; key < tChannel.keyframeCount; key++)
		{
			//Read each keyframe.
			tChannel.keyFrames[key].frameIndex = input.ReadInt32();
			tChannel.keyFrames[key].padding1 = input.ReadInt32();
			tChannel.keyFrames[key].padding2 = input.ReadInt32();
			tChannel.keyFrames[key].padding3 = input.ReadInt32();
			tChannel.keyFrames[key].x = input.ReadFloat32();
			tChannel.keyFrames[key].y = input.ReadFloat32();
			tChannel.keyFrames[key].z = input.ReadFloat32();
			tChannel.keyFrames[key].w = input.ReadFloat32();
		}
	}
	
	//Close our input stream.
	input.Close();
	
	//Create an animation to deposit data into.
	MOTAnimation tAnimation;
	tAnimaton.speed = 1.0f;
	
	//Copy the channels from the list into our animation.
	tAnimation.channels = new MOTChannel[tChannelList.Length];
	for(int chan = 0; chan < tChannelList.Length; chan++)
		tAnimation.channels[chan] = tChannelList[chan];
	
	//Return our master animation.
	return tAnimation;
}
```
Animation Slicing with CFG
---------------------------
```cs
//Contains the parsed slice information from the CFG.
struct CFGAnimationSlice
{
	string name;			//The name of this sub-animation.
	int32 startFrame;		//The start frame of the slice.
	int32 endFrame;			//The end frame of the slice.
	float32 speed;			//The speed of the animation.
}

//Slices the master MOTAnimation using the info.cfg at the file path. Returns an array of sliced animations.
MOTAnimation[] SliceMOT(string szCFGFilePath, const reference MOTAnimation tMaster)
{
	//Open our CFG file.
	InputStream input = File.Open(szCFGFilePath);
	
	//Create a list of slices.
	List<CFGAnimationSlice> tSliceList;
	CFGAnimationSlice tSlice;
	
	//Loop until we reach EOF.
	while(input.EOF() == false)
	{
		//input.ReadLine returns ASCII characters up to a newline /n character.
		string szLine = input.ReadLine();
		
		//string.StartsWith returns whether or not the string begins with the specified string.
		//If we hit KEY, that's an animation slice.
		if(szLine.StartsWith("KEY"))
		{
			//string.Split splits the string into an array of strings based on the split character.
			string[] szIndividualValues = szLine.Split(" ");
			
			//Ignore the [0] string because that will be "KEY".
			
			//This name will be in 2-byte unicode Japanese. It's not really useful to us, unless we intend to use this name for naming the sub-animation, or we wish to translate it to another language.
			tSlice.name = szIndividualValues[1];
			
			//int.Parse converts the string to integer if possible. Same as stoi in C.
			tSlice.startFrame = int.Parse(szIndividualValues[2]);
			
			//int.Parse converts the string to integer if possible. Same as stoi in C.
			tSlice.endFrame = int.Parse(szIndividualValues[3]);
			
			//int.Parse converts the string to float if possible. Same as stof in C.
			tSlice.speed = float.Parse(szIndividualValues[4]);
			
			//Add this new slice to the list.
			tSliceList.Add(tSlice);
		}
		//If we hit KEY_END, we're done.
		else if(szLine.StartsWith("KEY_END))
		{
			break;
		}
	}
	
	//Close our input stream.
	input.Close();
	
	//Next we'll allocate a new array of animations that will contain our sliced master animation data.
	MOTAnimation[] tSlicedAnimations = new MOTAnimation[tSliceList.Length];
	
	//Loop through and extract the slices from the masters.
	for(int slice = 0; slice < tSliceList.Length; slice++)
	{
		//Copy over the speed value.
		tSlicedAnimations[slice].speed = tSliceList[slice].speed;
		
		//Calculate the length of the slice.
		int32 nSliceLength = tSliceList[slice].endFrame - tSliceList[slice].startFrame;
		
		//Allocate new channels for out animation.
		tSlicedAnimations[slice].channels = new MOTChannel[tMaster.channels.Length];
		
		//Loop through these channels and perform a copy.
		for(int chan = 0; chan < tMaster.channels.Length; chan++)
		{
			//Copy all of the regular information.
			tSlicedAnimations[slice].channels[chan].boneIndex = tMaster.channels[chan].boneIndex;
			tSlicedAnimations[slice].channels[chan].int32 Unknown1 = tMaster.channels[chan].Unknown1;
			tSlicedAnimations[slice].channels[chan].int32 channelType = tMaster.channels[chan].channelType;
			tSlicedAnimations[slice].channels[chan].int32 headerByteSize = tMaster.channels[chan].headerByteSize;
			tSlicedAnimations[slice].channels[chan].int32 keyframeCount = tMaster.channels[chan].keyframeCount;
			tSlicedAnimations[slice].channels[chan].int32 animationByteSize = tMaster.channels[chan].animationByteSize;
			tSlicedAnimations[slice].channels[chan].int32 Unknown2 = tMaster.channels[chan].Unknown2;
			tSlicedAnimations[slice].channels[chan].int32 Unknown3 = tMaster.channels[chan].Unknown3;
			
			//Allocate only the number of keyframes we need for the slice.
			tSlicedAnimations[slice].channels[chan].keyFrames = new MOTKeyframe[nSliceLength];
			
			//Instead of doing a full copy of the keyframes, we only want to copy the section relevant to the slice.
			for(int key = tSliceList[slice].startFrame; key < tSliceList[slice].endFrame; key++)
			{
				//Copy the keyframes in.
				tSlicedAnimations[slice].channels[chan].keyFrames[key - tSliceList[slice].startFrame] = tMaster.channels[chan].keyFrames[key];
			}
		}
	}
	
	//Return our slices.
	return tSlicedAnimations;
}
```
