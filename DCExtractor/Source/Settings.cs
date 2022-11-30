using Custom.IO;
using DCExtractor.Forms;
using System.IO;
using System.Windows.Forms;

namespace DCExtractor
{
    /// <summary>
    /// Class   :   "Settings"
    /// 
    /// Purpose :   a global namespace for holding settings data used by the DCExtractor application.
    /// 
    /// This is a bit overkill, but I've created a class just for storing settings values.
    /// </summary>
    public static class Settings
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Enumerations.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Enumeration :   "ModelConversionSetting"
        /// 
        /// Purpose     :   governs how the meshes of an mds model are packaged when they are exported to another format(smd/obj).
        /// 
        /// OneModel    :   all meshes will be kept inside a single model and exported as a single mesh to a single model file.
        /// ManyModels  :   each mesh will be unpacked from the model and exported as separate models.
        /// </summary>
        public enum ModelConversionSetting : int { OneModel, ManyModels };


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Classes.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Class   :   "Setting"
        /// 
        /// Purpose :   defines a setting saved and loaded by the application.
        /// </summary>
        class Setting
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Data Members.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            string m_szName;
            string m_szCommand;
            int m_nDefaultValue = 0;
            int m_nValue;


            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Properties.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Gets the name of the setting.
            /// </summary>
            public string name
            {
                get { return m_szName; }
            }

            /// <summary>
            /// Gets the command as seen in the settings file.
            /// </summary>
            public string command
            {
                get { return m_szCommand; }
            }

            /// <summary>
            /// Gets/Sets the value of the setting.
            /// </summary>
            public int value
            {
                get { return m_nValue; }
                set { m_nValue = value; }
            }


            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Functions.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="szName">The name of the setting.</param>
            /// <param name="szCommand">The command as written in the settings file.</param>
            /// <param name="nDefaultValue">The default value of the setting.</param>
            public Setting(string szName, string szCommand, int nDefaultValue)
            {
                m_szName = szName;
                m_szCommand = szCommand;
                m_nDefaultValue = nDefaultValue;
                m_nValue = nDefaultValue;
            }
        }

        
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Data Members.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static Setting[] m_tSettings = new Setting[0];


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Properties.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets the model conversion setting. This setting governs how meshes are package when a model is exported to another format. See the enumeration for more details.
        /// </summary>
        public static ModelConversionSetting modelConversion
        {
            get { return (ModelConversionSetting)GetSetting("ModelConversion"); }
        }

        /// <summary>
        /// Gets whether or not to export animations when converting mds files.
        /// </summary>
        public static bool exportAnimations
        {
            get { return GetSetting("ExportAnimations") == 1; }
        }

        /// <summary>
        /// Gets whether or not the show warnings about invalid MDS data. This setting supresses the MessageBoxes that pop up when a mds fails to parse.
        /// </summary>
        public static bool showMDSWarnings
        {
            get { return GetSetting("ShowMDSWarnings") == 1; }
        }

        /// <summary>
        /// Gets whether or not the show file extension warnings about invalid file extensions on extracted files. This setting supresses the MessageBoxes that pop up when a file extension is invalid.
        /// </summary>
        public static bool showFileExtensionWarnings
        {
            get { return GetSetting("ShowFileExtensionWarnings") == 1; }
        }

        /// <summary>
        /// Gets the path to the settings file.
        /// </summary>
        static string settingsPath
        {
            get { return Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "settings.txt"); }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Initializes the settings system.
        /// </summary>
        public static void Initialize()
        {
            //Add settings here if you want them to show up. You'll also need to update the SettingsForm with a new field.
            m_tSettings = new Setting[]
                {
                    new Setting("ModelConversion", "ModelConversion:", (int)ModelConversionSetting.OneModel),
                    new Setting("ExportAnimations", "ExportAnimations:", 0),
                    new Setting("ShowMDSWarnings", "ShowMDSWarnings:", 0),
                    new Setting("ShowFileExtensionWarnings", "ShowFileExtensionWarnings:", 1)
                };
        }

        /// <summary>
        /// Shows a settings dialogue and modifies the current settings if the result was saved.
        /// </summary>
        public static void ShowSettingsDialog()
        {
            //Setup the settings form.
            SettingsForm tSettingForm = new SettingsForm();

            //Push the settings to the form.
            for (int i = 0; i < m_tSettings.Length; i++)
                tSettingForm.SetSettingValue(m_tSettings[i].name, m_tSettings[i].value);

            //Show the form and wait for user changes.
            if (tSettingForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //Retrieve the results of the user's selection.
                for (int i = 0; i < m_tSettings.Length; i++)
                {
                    int nValue = tSettingForm.GetSettingValue(m_tSettings[i].name);
                    if (nValue != -1)
                        m_tSettings[i].value = nValue;
                }
            }
        }

        /// <summary>
        /// Returns the value of the setting specified.
        /// </summary>
        /// <param name="szSettingName">The name of the setting to find.</param>
        /// <returns>The value of the specified setting.</returns>
        public static int GetSetting(string szSettingName)
        {
            for (int i = 0; i < m_tSettings.Length; i++)
            {
                if (m_tSettings[i].name == szSettingName)
                    return m_tSettings[i].value;
            }
            throw new System.ArgumentException("Invalid setting specified for GetSetting: " + szSettingName + " not found!");
        }

        /// <summary>
        /// Saves the settings to a text file next to the executable.
        /// </summary>
        public static void SaveSettings()
        {
            if(m_tSettings == null)

            //If the settings file does not exist, create it.
            if (File.Exists(settingsPath) == false)
                File.CreateText(settingsPath);

            //Open our output stream.
            StreamWriter tWriter = FileStreamHelpers.OpenStreamWriter(settingsPath);
            if (tWriter == null)
                return;

            //Write out our settings.
            for (int i = 0; i < m_tSettings.Length; i++)
                tWriter.Write(m_tSettings[i].command + m_tSettings[i].value.ToString() + "\n");

            //Close the output stream.
            tWriter.Close();
        }

        /// <summary>
        /// Loads the settings from a text file next to the executable.
        /// </summary>
        public static void LoadSettings()
        {
            //If the settings file doesn't exist, then quit.
            if (File.Exists(settingsPath) == false)
                return;

            //Read in the file.
            string[] szLines = File.ReadAllLines(settingsPath);

            //Loop through the file looking for settings.
            int nValue = 0;
            for (int line = 0; line < szLines.Length; line++)
            {
                //Load in our settings.
                for (int i = 0; i < m_tSettings.Length; i++)
                {
                    if (LoadIntegerSetting(m_tSettings[i].command, ref nValue, szLines[line]))
                    {
                        m_tSettings[i].value = nValue;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Loads a setting as an integer value.
        /// </summary>
        /// <param name="szSettingName">The name of the setting.</param>
        /// <param name="nValue">The value to deposit the setting in.</param>
        /// <param name="szLine">The line to parse.</param>
        /// <returns>Whether or not the line could be parsed.</returns>
        static bool LoadIntegerSetting(string szSettingName, ref int nValue, string szLine)
        {
            if (szLine.StartsWith(szSettingName))
            {
                //Try to parse the line to get out the setting.
                try
                {
                    nValue = int.Parse(szLine.Substring(szSettingName.Length));
                }
                catch (System.Exception)
                {
                    return false;
                }
                return true;
            }
            return false;
        }
    } 
}