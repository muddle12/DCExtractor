using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Windows.Forms;

namespace Custom.Forms
{
    /// <summary>
    /// Class   :   "FileDialogHelpers"
    /// 
    /// Purpose :   defines a bunch of handy functions for interfacing with file dialogs and their filter strings.
    /// </summary>
    public static class FileDialogHelpers
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Classes.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Class   :   "FileDialogFilter"
        /// 
        /// Purpose :   defines a simple helper class for generating file dialog filter strings.
        /// 
        /// Filter strings are a pain in the ass to work with. This class automates the process of generating them.
        /// </summary>
        public class FileDialogFilter
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Data Members.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            string m_szName = "All";
            string[] m_szExtensions = new string[] { "*" };


            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Properties.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Gets a dialog filter that corresponds to all files.
            /// </summary>
            public static FileDialogFilter allFiles
            {
                get { return new FileDialogFilter(); }
            }

            /// <summary>
            /// Gets the name of this file type.
            /// </summary>
            public string name
            {
                get { return m_szName; }
            }

            /// <summary>
            /// Gets the extensions associated with this file type.
            /// </summary>
            public string[] extensions
            {
                get { return m_szExtensions; }
            }


            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Functions.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Constructor.
            /// </summary>
            public FileDialogFilter()
            {
    
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="szName">The name of the file type.</param>
            /// <param name="szExtension">The extension associated with this type.</param>
            public FileDialogFilter(string szName, string szExtension)
            {
                m_szName = szName;
                m_szExtensions = new string[1] { szExtension };
                Verify();
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="szName">The name of the file type.</param>
            /// <param name="szExtensions">The list of extensions associated with this type.</param>
            public FileDialogFilter(string szName, string[] szExtensions)
            {
                m_szName = szName;
                m_szExtensions = szExtensions;
                Verify();
            }

            /// <summary>
            /// Verifies the intergrity of the extensions.
            /// </summary>
            void Verify()
            {
                for (int i = 0; i < m_szExtensions.Length; i++)
                {
                    if (m_szExtensions[i].StartsWith("."))
                        m_szExtensions[i] = m_szExtensions[i].Substring(1);
                }
            }

            /// <summary>
            /// Returns the string equivalent of this object.
            /// </summary>
            /// <returns>The string equivalent of this object.</returns>
            public override string ToString()
            {
                //All Graphics Types|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff"
                string szResult = m_szName + " (";
                for (int i = 0; i < m_szExtensions.Length; i++)
                {
                    szResult += "*." + m_szExtensions[i];
                    if (i < m_szExtensions.Length - 1)
                        szResult += ",";
                }
                szResult += ")|";
                for (int i = 0; i < m_szExtensions.Length; i++)
                {
                    szResult += "*." + m_szExtensions[i];
                    if (i < m_szExtensions.Length - 1)
                        szResult += ";";
                }
                return szResult;
            }

            /// <summary>
            /// Generates a filter string from the dialog filters passed.
            /// </summary>
            /// <param name="tFilters">The list of filters.</param>
            /// <returns>The string restult of all filters.</returns>
            public static string GenerateFilterString(FileDialogFilter[] tFilters)
            {
                string szResult = string.Empty;
                for (int i = 0; i < tFilters.Length; i++)
                {
                    szResult += tFilters[i];
                    if (i < tFilters.Length - 1)
                        szResult += "|";
                }
                return szResult;
            }

            /// <summary>
            /// Combines the array of filters into one overarching filter.
            /// </summary>
            /// <param name="szNewName">The new name of this filter.</param>
            /// <param name="tFilters">The list of filters to add to this filter.</param>
            /// <returns>A new overarching filter.</returns>
            public static FileDialogFilter CombineFilters(string szNewName, FileDialogFilter[] tFilters)
            {
                FileDialogFilter tCombined = new FileDialogFilter();
                tCombined.m_szName = szNewName;

                System.Collections.Generic.List<string> tExtensions = new System.Collections.Generic.List<string>();
                for (int filter = 0; filter < tFilters.Length; filter++)
                {
                    for (int ext = 0; ext < tFilters[filter].extensions.Length; ext++)
                    {
                        if(tExtensions.Contains(tFilters[filter].extensions[ext]) == false)
                            tExtensions.Add(tFilters[filter].extensions[ext]);
                    }
                }
                tCombined.m_szExtensions = tExtensions.ToArray();

                return tCombined;
            }
        };


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Data Members.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static string m_szCurrentWorkingDirectory = string.Empty;


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Properties.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets/Sets the initial directory for open dialog operations.
        /// </summary>
        public static string currentWorkingDirectory
        {
            get { return m_szCurrentWorkingDirectory; }
            set { m_szCurrentWorkingDirectory = value; }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Opens a file dialog and collects a selection from the user and returns it.
        /// </summary>
        /// <param name="szFilter">The filter to use for selecting a file.</param>
        /// <param name="szTitle">The title of the file dialog.</param>
        /// <param name="szPath">The resulting path selected by the user.</param>
        /// <returns>Whether or not the open file dialog selected something valid.</returns>
        public static bool ShowOpenFileDialog(string szFilter, string szTitle, out string szPath)
        {
            OpenFileDialog tOpen = new OpenFileDialog();
            tOpen.Title = szTitle;
            tOpen.Filter = szFilter;
            tOpen.InitialDirectory = m_szCurrentWorkingDirectory;

            if (tOpen.ShowDialog() == DialogResult.OK)
            {
                szPath = tOpen.FileName;
                m_szCurrentWorkingDirectory = Path.GetDirectoryName(szPath);
                return true;
            }
            szPath = string.Empty;
            return false;
        }

        /// <summary>
        /// Opens a folder dialog and collects a selection from the user and returns it.
        /// </summary>
        /// <param name="szTitle">The title of the file dialog.</param>
        /// <param name="szPath">The resulting path selected by the user.</param>
        /// <returns>Whether or not the open folder dialog selected something valid.</returns>
        public static bool ShowOpenFolderDialog(string szTitle, out string szPath)
        {
            CommonOpenFileDialog tOpen = new CommonOpenFileDialog();
            tOpen.Title = szTitle;
            tOpen.IsFolderPicker = true;
            tOpen.InitialDirectory = m_szCurrentWorkingDirectory;

            if (tOpen.ShowDialog() == CommonFileDialogResult.Ok)
            {
                szPath = tOpen.FileName;
                m_szCurrentWorkingDirectory = Path.GetDirectoryName(szPath);
                return true;
            }
            szPath = string.Empty;
            return false;
        }

        /// <summary>
        /// Opens a save file dialog and collects a selection from the user and returns it.
        /// </summary>
        /// <param name="szFilter">The filter to use for selecting a file.</param>
        /// <param name="szTitle">The title of the file dialog.</param>
        /// <param name="szPath">The resulting path selected by the user.</param>
        /// <param name="szFileName">The suggested name of the file to prompt the user with when the dialog first opens.</param>
        /// <returns>Whether or not the open file dialog selected something valid.</returns>
        public static bool ShowSaveFileDialog(string szFilter, string szTitle, out string szPath, string szFileName)
        {
            SaveFileDialog tSave = new SaveFileDialog();
            tSave.Title = szTitle;
            tSave.Filter = szFilter;
            tSave.AddExtension = true;
            tSave.CheckFileExists = false;
            tSave.FileName = szFileName;

            if (tSave.ShowDialog() == DialogResult.OK)
            {
                szPath = tSave.FileName;
                m_szCurrentWorkingDirectory = Path.GetDirectoryName(szPath);
                return true;
            }
            szPath = string.Empty;
            return false;
        }

        /// <summary>
        /// A shorthand version of the ShowSaveFileDialog that's slightly less error-prone than the other version.
        /// </summary>
        /// <param name="tFormat">The format to save as.</param>
        /// <param name="szTitle">The title to display on the dialog.</param>
        /// <param name="szInputPath">The input path of the file that's being converted.</param>
        /// <param name="szOutPath">The output path to write to.</param>
        /// <returns>Whether or not a selection was made by the user.</returns>
        public static bool ShowSaveFileDialog(FileDialogFilter tFormat, string szTitle, string szInputPath, out string szOutPath)
        {
            return ShowSaveFileDialog(tFormat.ToString(), szTitle, out szOutPath, Path.ChangeExtension(Path.GetFileNameWithoutExtension(szInputPath), tFormat.extensions[0]));
        }
    }
}
