using System.Windows.Forms;

namespace DCExtractor.Controls
{
    /// <summary>
    /// Class   :   "SettingsField"
    /// 
    /// Purpose :   represents a control that contains a binary option that controls an application setting's value.
    /// </summary>
    public partial class SettingField : UserControl
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Data Members.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        string m_szSettingName = "Setting";


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Properties.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets the name of this field.
        /// </summary>
        public string SettingName
        {
            get { return m_szSettingName; }
            set { m_szSettingName = value; }
        }

        /// <summary>
        /// Gets/Sets the formal name of the field.
        /// </summary>
        public string FormalName
        {
            get { return groupBox1.Text; }
            set { groupBox1.Text = value; }
        }

        /// <summary>
        /// Gets/Sets the name of the first option.
        /// </summary>
        public string OptionName1
        {
            get { return BT_state1.Text; }
            set { BT_state1.Text = value; }
        }

        /// <summary>
        /// Gets/Sets the name of the first option.
        /// </summary>
        public string OptionName2
        {
            get { return BT_state2.Text; }
            set { BT_state2.Text = value; }
        }

        /// <summary>
        /// Gets/Sets the value of the settings field.
        /// </summary>
        public int Value
        {
            get { return ((BT_state2.Checked) ? 1 : 0); }
            set
            {
                if (value == 0)
                {
                    BT_state1.Checked = true;
                    BT_state2.Checked = false;
                }
                else
                {
                    BT_state1.Checked = false;
                    BT_state2.Checked = true;
                }
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        public SettingField()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Fires when the state1 checkbox changes value.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void BT_state1_CheckedChanged(object sender, System.EventArgs e)
        {
            if (BT_state1.Checked)
                BT_state2.Checked = false;
        }

        /// <summary>
        /// Fires when the state1 checkbox changes value.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void BT_state2_CheckedChanged(object sender, System.EventArgs e)
        {
            if (BT_state2.Checked)
                BT_state1.Checked = false;
        }
    }
}
