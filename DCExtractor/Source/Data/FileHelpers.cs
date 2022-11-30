using DCExtractor.Forms;
using System;
using System.IO;
using System.Windows.Forms;
using static Custom.Forms.FileDialogHelpers;

namespace DCExtractor.Data
{
    /// <summary>
    /// Class   :   "FileHelpers"
    /// 
    /// Purpose :   contains various functions that help manage files and formats.
    /// </summary>
    public static class FileHelpers
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// An modified version of the Path.ChangeExtension method to include checking for invalid cases and correcting them.
        /// </summary>
        /// <param name="szFileName">The file name to modify.</param>
        /// <param name="szNewExtension">The new extension to add.</param>
        /// <returns>The file path with a new extension.</returns>
        public static string ChangeExtension(string szFileName, string szNewExtension)
        {
            //If this extension is invalid, we need to correct it.
            if (szNewExtension.Contains(".") == false)
                szNewExtension = "." + szNewExtension;

            //If the original file doesn't have an extension, we run the risk of overwriting it. Let's add on an extension.
            if (Path.HasExtension(szFileName) == false)
                szFileName += szNewExtension;

            //If it did have an extension, there is a chance it's not the one we want. Otherwise, we'll overwrite the original file.
            return Path.ChangeExtension(szFileName, szNewExtension);
        }

        /// <summary>
        /// Moves all files from the source directory to the destination directory while preserving the folder hierarchy.
        /// </summary>
        /// <param name="szSourceDir">The source directory to move files from.</param>
        /// <param name="szDestinationDir">The destination directory to move files to.</param>
        /// <param name="tFilters">The types of files to move.</param>
        /// <param name="bDeleteSource">Whether or not to delete source files once the move is done.</param>
        /// <param name="tDeleteFilters">The types of source files to delete.</param>
        public static void MoveFiles(string szSourceDir, string szDestinationDir, FileDialogFilter[] tFilters, bool bDeleteSource, FileDialogFilter[] tDeleteFilters)
        {
            //Early out.
            if (Directory.Exists(szSourceDir) == false ||
                tFilters == null ||
                tFilters.Length == 0)
                return;

            //gather all of our file names.
            System.Collections.Generic.List<string> tFilesToMove = new System.Collections.Generic.List<string>();
            for (int filter = 0; filter < tFilters.Length; filter++)
                tFilesToMove.AddRange(Directory.GetFiles(szSourceDir, "*" + tFilters[filter].extensions[0], SearchOption.AllDirectories));

            //Leftovers from an earlier build.
            //tFilesToMove.AddRange(Directory.GetFiles(szSourceDir, "*.smd", SearchOption.AllDirectories));
            //tFilesToMove.AddRange(Directory.GetFiles(szSourceDir, "*.png", SearchOption.AllDirectories));
            //tFilesToMove.AddRange(Directory.GetFiles(szSourceDir, "*.cfg", SearchOption.AllDirectories));

            //move the files while creating new subdirectories under the destination directory.
            string szSubPath;
            string szNewPath;
            FileConflictDialog.FileConflictDialogResult eAction = FileConflictDialog.FileConflictDialogResult.None;
            FileConflictDialog tDialog = new FileConflictDialog();
            for (int i = 0; i < tFilesToMove.Count; i++)
            {
                //Reset the dialog action to the default if the user hasn't selected yestoall or notoall.
                if (eAction != FileConflictDialog.FileConflictDialogResult.YesToAll &&
                    eAction != FileConflictDialog.FileConflictDialogResult.NoToAll)
                    eAction = FileConflictDialog.FileConflictDialogResult.None;

                //Build our new path.
                szSubPath = Custom.IO.PathHelpers.SubPathOf(szSourceDir, Path.GetDirectoryName(tFilesToMove[i]), false);
                szNewPath = Path.Combine(szDestinationDir, szSubPath) + "\\" + Path.GetFileName(tFilesToMove[i]);

                //If a file conflict is detected, ask the user what to do.
                if (File.Exists(szNewPath))
                {
                    //If they have not selected yestoall or notoall, let's prompt them.
                    if (eAction != FileConflictDialog.FileConflictDialogResult.YesToAll &&
                        eAction != FileConflictDialog.FileConflictDialogResult.NoToAll)
                    {
                        tDialog.sourceFile = tFilesToMove[i];
                        tDialog.destinationFile = szNewPath;
                        eAction = tDialog.ShowConflictDialog();
                    }

                    //in the no or no to all case, just skip this file.
                    if (eAction == FileConflictDialog.FileConflictDialogResult.No ||
                        eAction == FileConflictDialog.FileConflictDialogResult.NoToAll)
                        continue;
                }

                //create the directory if it doesn't exist.
                Directory.CreateDirectory(Path.GetDirectoryName(szNewPath));

                try
                {
                    //copy the file.
                    File.Copy(tFilesToMove[i], szNewPath, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "File Copy Error");
                }
            }

            //if we are deleting files, and our user has accepted the responsibility of such an action, go ahead with the delete.
            if (bDeleteSource &&
                MessageBox.Show("Would you like to delete the extracted files(.smd, .png) in the source directory to save space? (Original pak/mds/img files will be preserved, Destination smd/png files will be preserved)", "Should I Delete?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                DeleteFiles(szSourceDir, tDeleteFilters);
        }

        /// <summary>
        /// Deletes all files under all subdirectories based on the filters.
        /// </summary>
        /// <param name="szDirectory">The directory to delete files in.</param>
        /// <param name="tDeleteFilters">The filters to delete files with.</param>
        //TODO  :   I'd like to expose this function and just let the user call this from the Form, but I just don't feel comfortable unleashing a mass delete function without rigorous testing. Testing I'm not willing to do now.
        static void DeleteFiles(string szDirectory, FileDialogFilter[] tDeleteFilters)
        {
            //We're going to gather all of the files we need to delete.
            System.Collections.Generic.List<string> tFilesToDelete = new System.Collections.Generic.List<string>();
            for (int filter = 0; filter < tDeleteFilters.Length; filter++)
                tFilesToDelete.AddRange(Directory.GetFiles(szDirectory, "*" + tDeleteFilters[filter].extensions[0], SearchOption.AllDirectories));

            //Leftovers from an earlier build.
            //tFilesToDelete.AddRange(Directory.GetFiles(szSourceDir, "*.smd", SearchOption.AllDirectories));
            //tFilesToDelete.AddRange(Directory.GetFiles(szSourceDir, "*.png", SearchOption.AllDirectories));

            //Delete the files.
            for (int i = 0; i < tFilesToDelete.Count; i++)
                File.Delete(tFilesToDelete[i]);
        }
    }
}
