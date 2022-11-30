using DCExtractor.Controls;
using System.Windows.Forms;

namespace DCExtractor.Forms
{
    /// <summary>
    /// Class   :   "SettingsForm"
    /// 
    /// Purpose :   provides an interface for the user to modify the application's settings.
    /// </summary>
    public partial class SettingsForm : Form
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Properties.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        SettingField[] t_mSettingsFields;


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        public SettingsForm()
        {
            InitializeComponent();

            t_mSettingsFields = new SettingField[]
                {
                    SF_modelConversion,
                    SF_exportAnimation,
                    SF_ShowMDSWarnings,
                    SF_showFileExtensionWarnings
                };
        }

        /// <summary>
        /// Gets the value of the setting based on the setting name passed.
        /// </summary>
        /// <param name="szSettingName">The name of the setting to retrieve the value of.</param>
        /// <returns>The value of the setting, otherwise -1.</returns>
        public int GetSettingValue(string szSettingName)
        {
            for (int i = 0; i < t_mSettingsFields.Length; i++)
            {
                if (t_mSettingsFields[i].SettingName == szSettingName)
                    return t_mSettingsFields[i].Value;
            }
            return -1;
        }

        /// <summary>
        /// Sets the value of the setting based on the setting name passed.
        /// </summary>
        /// <param name="szSettingName">The name of the setting to set.</param>
        /// <param name="nValue">The value to set the setting to.</param>
        public void SetSettingValue(string szSettingName, int nValue)
        {
            for (int i = 0; i < t_mSettingsFields.Length; i++)
            {
                if (t_mSettingsFields[i].SettingName == szSettingName)
                {
                    t_mSettingsFields[i].Value = nValue;
                    break;
                }
            }
        }
    }
}