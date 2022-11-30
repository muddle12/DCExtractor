using System.Collections.Generic;
using System.IO;

namespace Custom.IO
{
    /// <summary>
    /// Class   :   "PathHelpers"
    /// 
    /// Purpose :   provides helper functions for creating directory or file paths.
    /// </summary>
    public static class PathHelpers
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Splits the path passed into individual directory names.
        /// </summary>
        /// <param name="szPath">The path to split.</param>
        /// <returns>A list of directories.</returns>
        static string[] SplitPath(string szPath)
        {
            //Split the path into individual directories.
            string[] szDirectories = szPath.Split(new char[] { ':', '\\', '/' });

            List<string> szValidDirs = new List<string>();
            for (int i = 0; i < szDirectories.Length; i++)
            {
                //Assuming we have a valid directory, add it to the list.
                if (szDirectories[i] != string.Empty)
                    szValidDirs.Add(szDirectories[i]);
            }
            
            //Return the list as an array of directory strings.
            return szValidDirs.ToArray();
        }

        /// <summary>
        /// Creates a new directory path using the list of directory names.
        /// </summary>
        /// <param name="szDirectories">The list of directories to form into a complete path.</param>
        /// <param name="nStart">The starting offset into the directory list to begin building the path. Specifying a number other than 0 creates a relative subpath.</param>
        /// <returns>A directory path or subpath.</returns>
        static string CreatePathFromDirectories(string[] szDirectories, int nStart)
        {
            //If the start offset is greater than the length, that means we're at the lowest directory. Return the lowest directory.
            if (nStart >= szDirectories.Length)
                return string.Empty;

            //Otherwise, we'll need to build the new path from the starting offset in the directory list.
            string szRet = szDirectories[nStart];
            for (int i = nStart + 1; i < szDirectories.Length; i++)
                szRet += "\\" + szDirectories[i];
            return szRet;
        }

        /// <summary>
        /// Creates a subpath based on the parent and absolute paths passed.
        /// </summary>
        /// <param name="szParentPath">The parent path, usually aboslute, that contains the child path.</param>
        /// <param name="szChildPath">The child path, or a path to something contained by the parent path.</param>
        /// <param name="bIncludeParent">Whether or not to include the parent directory in the subpath, or omit it.</param>
        /// <returns>A subpath of child relative to parent.</returns>
        public static string SubPathOf(string szParentPath, string szChildPath, bool bIncludeParent)
        {
            //If they do not share the same drive letter, they cannot be relative to each other.
            if (szParentPath[0] != szChildPath[0])
                return szChildPath;

            //If the parent path has a file, convert it to a directory.
            if (szParentPath.Contains("."))
                szParentPath = Path.GetDirectoryName(szParentPath);

            //If the child path has a file, convert it to a directory.
            if (szChildPath.Contains("."))
                szChildPath = Path.GetDirectoryName(szChildPath);

            //Split the paths of both so we can compare them.
            string[] szParentPaths = SplitPath(szParentPath);
            string[] szChildPaths = SplitPath(szChildPath);

            //We want to compare against the smaller path.
            int nLength = System.Math.Min(szParentPaths.Length, szChildPaths.Length);

            //Loop through and find the break point, if any.
            int nOffset;
            for (nOffset = 0; nOffset < nLength; nOffset++)
            {
                if (szParentPaths[nOffset] != szChildPaths[nOffset])
                    break;
            }
            //If we're including the parent and our offset is non-zero, move back once.
            if (bIncludeParent && nOffset != 0)
                nOffset -= 1;

            //Create the subpath.
            return CreatePathFromDirectories(szChildPaths, nOffset);
        }
    }
}