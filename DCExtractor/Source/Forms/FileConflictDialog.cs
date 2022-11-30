using System.Windows.Forms;

namespace DCExtractor.Forms
{
    /// <summary>
    /// Class   :   "FileConflictDialog"
    /// 
    /// Purpose :   a homemade file conflict dialog for overwriting files.
    /// </summary>
    public partial class FileConflictDialog : Form
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Enumerations.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public enum FileConflictDialogResult { None, Yes, No, YesToAll, NoToAll };


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Data Members.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        FileConflictDialogResult m_eDialogResult = FileConflictDialogResult.None;


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Properties.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets/Sets the source file to display.
        /// </summary>
        public string sourceFile
        {
            get { return LB_sourceFile.Text; }
            set { LB_sourceFile.Text = value; }
        }

        /// <summary>
        /// Gets/Sets the destination file to display.
        /// </summary>
        public string destinationFile
        {
            get { return LB_destinationFile.Text; }
            set{ LB_destinationFile.Text = value; }
        }

        /// <summary>
        /// Gets the result of the dialog.
        /// </summary>
        public FileConflictDialogResult dialogResult
        {
            get { return m_eDialogResult; }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        public FileConflictDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Shows the conflict dialog.
        /// </summary>
        /// <returns>The result of the dialog.</returns>
        public FileConflictDialogResult ShowConflictDialog()
        {
            m_eDialogResult = FileConflictDialogResult.None;
            ShowDialog();
            return m_eDialogResult;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Event Handlers.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Fires when the yes button is clicked.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void BT_yes_Click(object sender, System.EventArgs e)
        {
            m_eDialogResult = FileConflictDialogResult.Yes;
            Close();
        }

        /// <summary>
        /// Fires when the no button is clicked.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void BT_no_Click(object sender, System.EventArgs e)
        {
            m_eDialogResult = FileConflictDialogResult.No;
            Close();
        }

        /// <summary>
        /// Fires when the yes to all button is clicked.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void BT_yesToAll_Click(object sender, System.EventArgs e)
        {
            m_eDialogResult = FileConflictDialogResult.YesToAll;
            Close();
        }

        /// <summary>
        /// Fires when the no to all button is clicked.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void BT_noToAll_Click(object sender, System.EventArgs e)
        {
            m_eDialogResult = FileConflictDialogResult.NoToAll;
            Close();
        }
    }
}