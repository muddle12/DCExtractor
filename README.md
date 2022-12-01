# DCExtractor
DCExtractor is a tool designed to unpack and convert many of the files and formats used by several of the Level-5 PS2-era video games, specifically targeting Dark Cloud 1 and Dark Cloud 2. It is the culmination of all of the previous efforts over the course of decades to reverse engineer these games, all handily rolled into one package.

Features
----------------
DCExtractor is an all in one suite of extraction and conversion tools that are capable of converting 90% of the file types found within the Dark Cloud 1/2 .DAT archives, including the .DAT archive. It can read many of the formats for Dark Cloud 1/2, and can convert certain types of data to other friendlier formats for external use. Model data can be converted to Wavefront OBJ or Studiomdl SMD. Image formats can be converted to PNG.

DCExtractor is not a repacking tool, and cannot return these formats back to their original archives/formats.

#### DCExtractor can read the following formats:

    Root Data Archives: 
        .DAT (Data)
        .HD2 (File System)
        .HD3 (File System)
        
    Model Formats: 
        .MDS (Model Data)
        .WGT (Vertex Weight Data)
        .MOT (Motion/Animation Data)
        .BBP (Bind Pose Skeletal Data)
        
    Image Formats: 
        .IMG (IM2/IM3 Image Archive)
        .TM2 (Sony Playstation 2 Texture)
        
    Pack Formats:
        .PAK (Generic Data Archive Package)
        .CHR (Character Data Archive Package)
        .EFP (Effect Data Archive Package)
        .IPK (Image Data Archive Package)
        .MPK (Model Data Archive Package)
        .PCP (Level Data Archive Package)
        .SKY (Skybox Data Archive Package)
        .SND (Sound Data Archive Package)
        
#### DCExtractor can convert the following formats:

    .MDS Model Data -> Wavefront OBJ
        Supports:
            Vertices
            Normals
            Triangles/Polygons
            Texture Coordinates (UVs)
            Bind Pose Mesh Corrections (Mesh data will be shifted to its proper resting postion instead of the origin)
            
    .MDS Model Data -> Studiomdl SMD
        Supports:
            Vertices
            Normals
            Triangles/Polygons
            Texture Coordinates (UVs)
            Skeletal Hierarchy (Bones with proper hierarchy)
            Vertex Weights/Skinning (Mesh deforms with bones)
            Bind Pose Mesh Corrections (Mesh data will be shifted to its proper resting postion instead of the origin)
            Materials (Color data and texture names)
            
    .IMG Image Archive -> Portable Network Graphics PNG
    
    .TM2 Texture -> Portable Network Graphics PNG
    
#### DCExtractor does not support ( or partially supports ):

    .MDS Rare special-case models (Specifically skyboxes, certain effects models, and ui/menu elements)
    
    .MDS Animation data (.MOT animations are fully parsed and converted, but do not output to the correct trasnformations, can be exported but won't work)
    
    .MDS Scale data on skeletal hierarchy/animations (Neither Wavefront OBJ or Studio MDL SMD supports scale in their formats)
    
    Plain text configuration files (.BAK, .CLO, .EFF, .EM, .LST, etc)
    
    .CFG config files (Partially supported for animations)
    
    Sound conversion (DCExtractor can extract sound archives to PS2 sound formats, but you'll need a different tool to read those)
    
    Any and all other proprietary data created by the developer for Dark Cloud 1/2 engine specific applications.

Usage
----------------
DCExtractor features several options which extract and convert data. All operations support either single file or entire directory extraction/conversion. All
operations will extract/convert the source file to the same directory as the file, with the exception of .DAT. You may cancel any operation at any time by
using the Cancel (X) button in the bottom right hand corner of tool, on the right side of the progress bar.

###### File -> Extract -> DAT
    Extracts a Dark Cloud 1/2 .DAT file using either an .HD2 or .HD3 file system. Files will be extracted to the 
    chosen destination folder. This option will generate sub-directories in order to match the original 
    .DAT's file system structure.

###### File -> Extract -> PAK,CHR,IPK...
    Extracts a Level-5 PAK file, or a PAK-type file. All supported types are listed above, and on the button. 
    Files will be extracted from the archive to the same folder as the archive, in a sub-directory.

###### File -> Convert -> MDS -> OBJ -> File/Directory
    Reads a .MDS model file and converts it to Wavefront OBJ .OBJ, saving it to the same directory as the .MDS. 
    The File option converts a single file. The Directory option will convert all MDS files in the directory, 
    including sub-directories. This operation may produce more than one .OBJ file, for each mesh within the .MDS.

###### File -> Convert -> MDS -> SMD -> File/Directory
    Reads a .MDS model file and converts it to Studiomdl SMD .SMD, saving it to the same directory as the .MDS.
    The File option converts a single file. The Directory option will convert all MDS files in the directory, 
    including sub-directories. This operation will produce one .SMD file, with all meshes contained within it.
    
    You can enable multiple mesh output via the Settings, which will generate multiple SMDs for each mesh within 
    the .MDS. 
    
    You can enable animation output via the settings, which will generate a list of animations using 
    the .MOT file and the .CFG file, and output it to a subdirectory titled "anims".

###### File -> Convert -> IMG -> File/Directory
    Reads a .IMG archive file and converts all textures within it to .PNG, saving it to the same directory as 
    the .IMG. The File option converts a single file. The Directory option will convert all IMG files in the 
    directory, including sub-directories. This operation may produce more than one .PNG, as each IMG can 
    contain several textures.

###### File -> Convert -> TM2 -> File/Directory
    Reads a .TM2 file and converts all textures within it to .PNG, saving it to the same directory as the 
    .TM2. The File option converts a single file. The Directory option will convert all TM2 files in the 
    directory, including sub-directories. This operation may produce more than one .PNG, as each TM2 
    can contain several frames.

###### File -> Unpack All (Extract/Convert/Move)
    A quality of life option which performs all of the above options in one, generating a completely 
    unpacked directory, and moving all converted formats to a separate folder for ease of use. This is a 
    non-trivial operation, and may take some time to complete. It will do the following:

    1. Extract the .DAT archive to the same directory as the .DAT.
    2. Extract all .PAK archives of all types in all directories and sub-directories (PAK, CHR, EFP, IPK, MPK, PCP, SKY, SND) to the same directory as the archive was located.
    3. Convert all .MDS files in all directories and sub-directories to .SMD. (Will popup messages if there were any issues)
    4. Convert all .IMG files in all directories and sub-directories to .PNG. (Will popup messages if there were any issues)
    5. Convert all .TM2 files in all directories and sub-directories to .PNG. (Will popup messages if there were any issues)
    6. Move all files of specific converted types(SMD, PNG, and CFG) to the destination directory specified by you.
    7. Optional: Delete all .SMD and .PNG from the source directory where the previous operations took place, reducing excess files.
  
###### Settings
    This will open a list of settings that allow you to tweak some of the behaviours of DCExtractor. These settings are:

    1. Model Conversion: (Only for SMD. OBJ will always use Many Models Mode due to a format limitation)
        Export One Model (With All Meshes): DCExtractor will always favor creating a single .SMD per .MDS, with all meshes in one file.
        Export Many Models (With One Mesh): DCExtractor will always favor creating several .SMD files per .MDS, splitting the individual meshes out of a larger model.
        
    2. Export Animations: (Only for SMD, OBJ cannot support animation)
        No: Do not export animations from the .MOT file when converting .MDS to .SMD.
        Yes: Do export eanimations from the .MOT file when converting .MDS to .SMD.
    
    3. Show MDS Warnings: (Show warnings when an invalid or unsupported MDS was loaded. Useful for debugging)
        No: Do not warn about invalid or unsupported MDS files when they are loaded.
        Yes: Do warn about invalid or unsupported MDS files when they are loaded.
        
    4. Show File Extension Warnings: (Show warnings when a file is extracted from a DAT or PAK that has an invalid file extension)
        No: Do not warn about files with invalid file extensions are extracted.
        Yes: Do warn about files with invalid file extensions are extracted.
        
###### Debug
    This option is available to developers in debug mode. It contains useful functions for comparing files and generating metadata for debugging file formats. It is disabled in release builds, and can be safely ignored by the end user.
    
Disclaimer:
----------------
This tool is provided as is, with no warranty. It was created with the intent of sharing with the community all that I have learned while reverse-engineering these formats. This tool and its source code is provided freely, with the intent that others may use and build upon it.

I will make no promises or bargains. I do not intend to support this codebase or repository. I will not be adding additions to the tool. I will not be improving the tool or continuing work on this project. I may address critical issues with the tool's functionality(show stopping bugs), but I will leave improvements to future developers that follow after me.

Development:
----------------
If you are a developer who wishes to continue work on this project, feel free to fork this repository. I will be locking the repository, mainly to preserve the integrity of the codebase. I will not be altering the base implementation. If I decide to return at some point, I'll fork a separate repository and continue from there (or maybe contribute to others if they exist). The only reasonable changes to the original codebase will be to address critical defects.

Perhaps you can figure out the remaining mysteries in the formats where I was stone-walled (animation, weird model index formats, configuration files). I have documented all of the file formats in the Specification section, with example file layouts and implementation. It is basically all of the knowledge I gathered during the reverse engineering process in one place.


Oh, and one last thing. Keep it free, and keep it open source.


License
----------------
Liscensed under GNU GENERAL PUBLIC LICENSE v3
