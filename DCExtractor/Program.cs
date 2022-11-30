using System;
using System.Windows.Forms;

namespace DCExtractor
{
    /// <summary>
    /// Class   :   "Program"
    /// 
    /// Purpose :   the root class that holds the main entry point of the application.
    /// </summary>
    static class Program
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new DCExtractorForm());
        }
    }
}
