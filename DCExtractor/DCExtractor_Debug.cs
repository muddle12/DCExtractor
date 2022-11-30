using Custom.Forms;
using System;
using System.IO;
using System.Windows.Forms;

#if DEBUG
using Custom.Debug;
#endif

namespace DCExtractor
{
    /// <summary>
    /// Class   :   "DCExtractorForm_Debug"
    /// 
    /// 
    /// </summary>
    public partial class DCExtractorForm : Form
    {
        /*************************************************************************/
        //  In an effort to make the debugging process more palatable when
        //  extracting and converting these cryptic formats, I spun off a lot of
        //  debug functions to test files and data.
        /*************************************************************************/
        /// <summary>
        /// Fires when the compare file button is clicked.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void BT_compareFile_Click(object sender, EventArgs e)
        {
#if DEBUG
            //Let the user choose two files to compare.
            string szFirst, szSecond;
            if (FileDialogHelpers.ShowOpenFileDialog("All Files|*.*", "Pick the first file to compare to.", out szFirst) &&
                FileDialogHelpers.ShowOpenFileDialog("All Files|*.*", "Pick the second file to compare to.", out szSecond))
            {
                //Compare those files and display the result in a messagebox.
                Analysis.FileComparisonResult tResult = Analysis.CompareFiles(szFirst, szSecond);
                switch (tResult.result)
                {
                    case Analysis.FileComparisonResult.Result.Identical:
                        MessageBox.Show("These two files are identical", "Identical Files");
                        break;
                    case Analysis.FileComparisonResult.Result.DifferentLengths:
                        MessageBox.Show("These two files cannot be the same, as they have different lengths.", "Different File Lengths.");
                        break;
                    case Analysis.FileComparisonResult.Result.DifferentContents:
                        MessageBox.Show("These two files are not the same. They differ at offset [" + tResult.firstOffset + "," + tResult.secondOffset + "]", "Files Differ At Byte Level.");
                        break;
                    case Analysis.FileComparisonResult.Result.SameDirectory:
                        MessageBox.Show("These directories are the same.", "Same Directory");
                        break;
                    case Analysis.FileComparisonResult.Result.SameFile:
                        MessageBox.Show("These files are the same.", "Same Files");
                        break;
                }
            }
#endif
        }

        /// <summary>
        /// Fires when the compare directory button is clicked.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void BT_compareDirectory_Click(object sender, EventArgs e)
        {
#if DEBUG
            //Let the user choose two directories to compare.
            string szFirst, szSecond;
            if (FileDialogHelpers.ShowOpenFolderDialog("Pick the first directory to compare to.", out szFirst) &&
                FileDialogHelpers.ShowOpenFolderDialog("Pick the second directory to compare to.", out szSecond))
            {
                //We want to count up the number of differences between the two directories.
                int nIdenticals = 0, nDifferentLengths = 0, nDifferentContents = 0, nSameFile = 0, nSameDirectory = 0;

                Analysis.FileComparisonResult[] tResults = Analysis.CompareDirectories(szFirst, szSecond, true);
                for (int i = 0; i < tResults.Length; i++)
                {
                    switch (tResults[i].result)
                    {
                        case Analysis.FileComparisonResult.Result.Identical:
                            ++nIdenticals;
                            break;
                        case Analysis.FileComparisonResult.Result.DifferentLengths:
                            ++nDifferentLengths;
                            break;
                        case Analysis.FileComparisonResult.Result.DifferentContents:
                            ++nDifferentContents;
                            break;
                        case Analysis.FileComparisonResult.Result.SameFile:
                            ++nSameFile;
                            break;
                        case Analysis.FileComparisonResult.Result.SameDirectory:
                            ++nSameDirectory;
                            break;
                        default:
                            break;
                    }
                }

                //If there was a same directory found, they are obviously the same.
                if (nSameDirectory > 0)
                    MessageBox.Show("You have chosen the same directories. Aborting...", "Same Directory");

                //Show the results.
                MessageBox.Show("Identical Files: " + (nIdenticals + nSameFile) + "\n" + "Differing Files: " + (nDifferentLengths + nDifferentContents));
            }
#endif
        }

        /// <summary>
        /// Fires when the generate mds metadata button is clicked.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void BT_genMDSMetaData_Click(object sender, EventArgs e)
        {
#if DEBUG
            //Ask the user which file they wish to generate metadata for, and then generate it.
            string szPath;
            if (FileDialogHelpers.ShowOpenFileDialog(MDSFormat.ToString(), "Choose a .mds file to create meta data for", out szPath))
                DC.Debug.Analysis.Meta.MDS.GenerateMetaData(szPath);
#endif
        }

        /// <summary>
        /// Fires when the generate mds metadata all button is clicked.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void BT_genMDSMetaAll_Click(object sender, EventArgs e)
        {
#if DEBUG
            //Ask the user which directory they wish to generate meta data for.
            string szPath;
            if (FileDialogHelpers.ShowOpenFolderDialog("Choose a directory for generating meta data for .mds files", out szPath))
            {
                //Gather all of the files with the mds extension.
                string[] szFiles = Directory.GetFiles(szPath, "*" + MDSFormat.extensions[0], SearchOption.AllDirectories);
                for (int file = 0; file < szFiles.Length; file++)
                {
                    //Generate metadata for each mds.
                    if (szFiles[file].ToLower().EndsWith(MDSFormat.extensions[0]))
                        DC.Debug.Analysis.Meta.MDS.GenerateMetaData(szFiles[file]);
                }
            }
#endif
        }

        /// <summary>
        /// Fires when the generate wgt metadata button is clicked.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void BT_generateWGTMeta_Click(object sender, EventArgs e)
        {
#if DEBUG
            //Ask the user which weight file they wish to generate metadata for, and then generate it.
            string szPath;
            if (FileDialogHelpers.ShowOpenFileDialog(WGTFormat.ToString(), "Choose a .wgt file to create meta data for", out szPath))
                DC.Debug.Analysis.Meta.WGT.GenerateMetaData(szPath);
#endif
        }
    }
}