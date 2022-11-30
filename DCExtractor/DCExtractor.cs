using Custom.Forms;
using DC.IO;
using DC.Types;
using System;
using System.IO;
using System.Windows.Forms;
using static Custom.Forms.FileDialogHelpers;

namespace DCExtractor
{
    //Known (Possible) Issues:
    //------------------------------------------------------------
    //  -MDS does not use the MOTION data in the info.cfg, it just assumes the file names. This might not be a big issue, but could be on some edge case somewhere in one of the meshes.
    //  -The PS2 uses a modified form of OpenGL, which is a right-handed coordinate system. We're not doing any kind of conversion when we load in the MDS, we just spit it back out in the output formats. Platforms/tools that import this data may be left-handed.
    //  -The motion data has been loaded and tested against various test cases, and it appears to be correct. The problem is the binding poses on the MDS bone hierarchy. I can't seem to figure out how those bind poses work. As such, animation data doesn't actually work with the rigs outputted.
    //  -There's a hack in ColorHelpers.FromBufferRGBA32Bit that has to do with the alpha values of only the 32bit paletted images being half intensity(128). I have no clue why this is, but it's widely prevelant. Hopefully it won't break any images.
    //  -Tooltips have some hardcoded crap in them with AutomaticDelay which makes them leave very quickly. Not great, but I don't really want to fool with it(aka write my own).

    /// <summary>
    /// Class   :   "DCExtractorForm"
    /// 
    /// Purpose :   a form that displays various extraction operations for Level-5 file formats.
    /// 
    /// Most of the heavy lifting has been off-loaded to static classes in order to keep this class relatively simple and clean. 
    /// It only handles button clicks and file dialogs. I've also moved shared functionality to other partial classes in the
    /// same directory to reduce the amount of bloat in this file. If you're looking to add new functionality to DCExtractor,
    /// head over to DCExtractor_Operations first.
    /// </summary>
    public partial class DCExtractorForm : Form
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Constants.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //data.
        readonly FileDialogFilter DATFormat = new FileDialogFilter("Level-5 Data", ".dat");
        readonly FileDialogFilter HD2Format = new FileDialogFilter("Level-5 HD2 Data", ".hd2");
        readonly FileDialogFilter HD3Format = new FileDialogFilter("Level-5 HD3 Data", ".hd3");
        string HD_AllTypesFilter;

        //packs.
        readonly FileDialogFilter PAKFormat = new FileDialogFilter("Level-5 Pack File", ".pak");
        readonly FileDialogFilter CHRFormat = new FileDialogFilter("Level-5 Character", ".chr");
        readonly FileDialogFilter IPKFormat = new FileDialogFilter("Level-5 Image-Pack", ".ipk");
        readonly FileDialogFilter MPKFormat = new FileDialogFilter("Level-5 Model-Pack", ".mpk");
        readonly FileDialogFilter PCPFormat = new FileDialogFilter("Level-5 PCP", ".pcp");
        readonly FileDialogFilter EFPFormat = new FileDialogFilter("Level-5 Effect Pack", ".efp");
        readonly FileDialogFilter SNDFormat = new FileDialogFilter("Level-5 Sound", ".snd");
        readonly FileDialogFilter SKYFormat = new FileDialogFilter("Level-5 Skybox", ".sky");
        FileDialogFilter[] PAK_TypeList;
        string PAK_AllTypesFilter;

        //assets.
        readonly FileDialogFilter MDSFormat = new FileDialogFilter("Level-5 Model", ".mds");
        readonly FileDialogFilter TM2Format = new FileDialogFilter("PS2 TIM2 Image", ".tm2");
        readonly FileDialogFilter IMGFormat = new FileDialogFilter("Level-5 Image", ".img");
        readonly FileDialogFilter WGTFormat = new FileDialogFilter("Level-5 Weight Map", ".wgt");

        //exports.
        readonly FileDialogFilter OBJFormat = new FileDialogFilter("Wavefront Object", ".obj");
        readonly FileDialogFilter SMDFormat = new FileDialogFilter("Studiomdl Data", ".smd");
        readonly FileDialogFilter PNGFormat = new FileDialogFilter("PNG Image", ".png");

        //plain text.
        readonly FileDialogFilter CFGFormat = new FileDialogFilter("Configuration", ".cfg");


        //MDS Child Formats.
        //  Animation/Motion - .mot
        //  Bind Bone Pose? - .bbp
        //  Weights - .wgt

        //Sound Formats
        //  Sony PS2 Sound Files.   -   .hd/.bd

        //Plain-Text Formats(for reference)
        //  Effect Script - .em
        //  Particle Effect - .eff
        //  Cloth - .clo
        //  Bak - .bak
        //  List - .lst

        //Unknown formats
        //  stb (some kind of script format, proprietary)
        //  mes (some kind of menu or message script)
        //  Icon - .ico (I didn't bother parsing this)
        //  vol (Sound file, I presume volume)
        //  sq  (Sound file, I presume sound quality?)

#if DEBUG
        Debug.Forms.DebugForm m_tDebugForm;
#endif
        bool m_bIsClosing = false;


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        public DCExtractorForm()
        {
            //Initialize our form.
            InitializeComponent();

            //Set the default button state for the form.
            ToggleButtons(true);

            //Initialize our workers.
            InitializeWorkers();

            //Set the current working directory based on the executable path.
            FileDialogHelpers.currentWorkingDirectory = Path.GetDirectoryName(Application.ExecutablePath);

            //Build our filter list for opening/saving files.
            BuildFilters();

            //Initialize our settings.
            Settings.Initialize();

            //Load the application settings.
            Settings.LoadSettings();
#if DEBUG
#else
            //Disable the debug button in release mode.
            DEBUG_menuStripButton.Enabled = false;
#endif
        }

        /// <summary>
        /// Builds the filter strings for the application.
        /// </summary>
        void BuildFilters()
        {
            //Assemble all the DAT file types into one filter.
            HD_AllTypesFilter = FileDialogFilter.GenerateFilterString(new FileDialogFilter[]
            {
                FileDialogFilter.CombineFilters("HD2/HD3 Files", new FileDialogFilter[] { HD2Format, HD3Format }),
                HD2Format,
                HD3Format,
                FileDialogFilter.allFiles
            });

            //Create an array of filters based on PAK file types.
            PAK_TypeList = new FileDialogFilter[]
            {
                PAKFormat,
                CHRFormat,
                IPKFormat,
                MPKFormat,
                PCPFormat,
                EFPFormat,
                SNDFormat,
                SKYFormat
            };

            //Assemble all the PAK file types into one filter.
            FileDialogFilter[] tPAKFilterList = new FileDialogFilter[]
            {
                FileDialogFilter.CombineFilters("PAK-Type Files", PAK_TypeList),
                PAKFormat,
                CHRFormat,
                IPKFormat,
                MPKFormat,
                PCPFormat,
                EFPFormat,
                SNDFormat,
                SKYFormat,
                FileDialogFilter.allFiles
            };
            //Generate the file string for this list.
            PAK_AllTypesFilter = FileDialogFilter.GenerateFilterString(tPAKFilterList);
        }

        /// <summary>
        /// Saves the model to the studiomdl model (.smd) format.
        /// </summary>
        /// <param name="tModel">The input model.</param>
        /// <param name="eConversion">How the model should be saved.</param>
        static void PerformSaveSMD(Model tModel, Settings.ModelConversionSetting eConversion)
        {
            //If we're set to one model, we want to save out the model data in one file.
            if (eConversion == Settings.ModelConversionSetting.OneModel)
            {
                SMD.Save(tModel);
                if (Settings.exportAnimations)
                    SMD.SaveAnimations(tModel);
            }
            //Otherwise, we want to split the model into individual files based on the number of meshes.
            else
            {
                Model[] tModels = tModel.Split();
                for (int model = 0; model < tModels.Length; model++)
                    SMD.Save(tModels[model]);
                if (Settings.exportAnimations)
                    SMD.SaveAnimations(tModel);
            }
        }

        /// <summary>
        /// Toggles the enabled state of certain buttons on the form.
        /// </summary>
        /// <param name="bState">The state to set the buttons to.</param>
        void ToggleButtons(bool bState)
        {
            BT_settings.Enabled = bState;
            BT_extract.Enabled = bState;
            BT_convert.Enabled = bState;
            BT_extractAndConvert.Enabled = bState;
            BT_cancelWork.Enabled = !bState;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Event Handlers.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Called when the window is closed.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnClosed(EventArgs e)
        {
            m_bIsClosing = true;
            Settings.SaveSettings();
            base.OnClosed(e);
        }

        /// <summary>
        /// Called when the form is shown.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
#if DEBUG
            m_tDebugForm = new Debug.Forms.DebugForm();
            m_tDebugForm.Show();
#endif
        }

        /// <summary>
        /// Fires when the close button is clicked.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void BT_close_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Fires when the settings button is clicked.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void BT_settings_Click(object sender, EventArgs e)
        {
            Settings.ShowSettingsDialog();
        }

        #region Extraction
        /// <summary>
        /// Fires when the extract DAT button is clicked.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void BT_ExtractDAT_Click(object sender, EventArgs e)
        {
            //Block this function if we're already working.
            if (isWorking)
                return;

            //A lot going on in this if, but basically we're opening three different windows to map out the user's selection. Figure out what to extract, which file system to extract it with, and where to place the extracted files.
            string szDATPath = string.Empty, szHDPath = string.Empty, szOutPath = string.Empty;
            if (FileDialogHelpers.ShowOpenFileDialog(DATFormat.ToString(), "Choose a valid .DAT to extract", out szDATPath) &&
                FileDialogHelpers.ShowOpenFileDialog(HD_AllTypesFilter, "Choose a valid .HD2 or .HD3", out szHDPath) &&
                FileDialogHelpers.ShowOpenFolderDialog("Choose an output directory to write the .DAT to", out szOutPath))
            {
                //Call ExtractDAT via its worker and pass it three values: The path to the DAT, the path to an HD2 or HD3, and the output directory.
                StartWork(worker_ExtractDAT, new string[] { szDATPath, szHDPath, szOutPath });
            }
        }

        /// <summary>
        /// Fires when the extract PAK file button is clicked.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void BT_extractPAKFile_Click(object sender, EventArgs e)
        {
            //Block this function if we're already working.
            if (isWorking)
                return;

            string szPath;
            if (FileDialogHelpers.ShowOpenFileDialog(PAK_AllTypesFilter, "Choose a PAK-type file to extract", out szPath))
                StartWork(worker_ExtractPAK, new string[] { szPath });
        }

        /// <summary>
        /// Fires when the extract PAK directory button is clicked.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void BT_extractPAKDirectory_Click(object sender, EventArgs e)
        {
            //Block this function if we're already working.
            if (isWorking)
                return;

            string szPath;
            if (FileDialogHelpers.ShowOpenFolderDialog("Choose a directory to extract PAK-type files", out szPath))
                StartWork(worker_ExtractPAKDirectory, new string[] { szPath });
        }
        #endregion

        #region Conversion
        /// <summary>
        /// Fires when the convert mds to obj file button is clicked.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void BT_convertMDStoOBJFile_Click(object sender, EventArgs e)
        {
            //Block this function if we're already working.
            if (isWorking)
                return;

            //Prompt the user which file they want to convert.
            string szInputPath;
            if (FileDialogHelpers.ShowOpenFileDialog(MDSFormat.ToString(), "Choose a .mds file to convert to a .obj.", out szInputPath))
                StartWork(worker_ConvertMDStoOBJ, new string[] { szInputPath });
        }

        /// <summary>
        /// Fires when the convert mds to obj directory button is clicked.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void BT_convertMDStoOBJDirectory_Click(object sender, EventArgs e)
        {
            //Block this function if we're already working.
            if (isWorking)
                return;

            //Prompt the user which directory they want to convert.
            string szInputDir;
            if (FileDialogHelpers.ShowOpenFolderDialog("Choose an input directory to convert mds files.", out szInputDir))
                StartWork(worker_ConvertMDStoOBJDirectory, new string[] { szInputDir });
        }

        /// <summary>
        /// Fires when the convert mds to smd file button is clicked.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void BT_convertMDStoSMDFile_Click(object sender, EventArgs e)
        {
            //Block this function if we're already working.
            if (isWorking)
                return;

            //Prompt the user which file they want to convert.
            string szPath;
            if (FileDialogHelpers.ShowOpenFileDialog(MDSFormat.ToString(), "Choose a .mds file to convert to a .smd.", out szPath))
                StartWork(worker_ConvertMDStoSMDFile, new string[] { szPath });
        }

        /// <summary>
        /// Fires when the convert mds to smd directory button is clicked.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void BT_convertMDStoSMDDirectory_Click(object sender, EventArgs e)
        {
            //Block this function if we're already working.
            if (isWorking)
                return;

            //Prompt the user which directory they want to convert.
            string szInputDir;
            if (FileDialogHelpers.ShowOpenFolderDialog("Choose an input directory to convert mds files.", out szInputDir))
                StartWork(worker_ConvertMDStoSMDDirectory, new string[] { szInputDir });
        }

        /// <summary>
        /// Fires when the convert img file button is clicked.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void BT_convertIMGFile_Click(object sender, EventArgs e)
        {
            //Block this function if we're already working.
            if (isWorking)
                return;

            //Prompt the user which file they want to convert.
            string szInputPath;
            if (FileDialogHelpers.ShowOpenFileDialog(IMGFormat.ToString(), "Choose a directory to convert tm2 files.", out szInputPath))
                StartWork(worker_ConvertIMGFile, new string[] { szInputPath });
        }

        /// <summary>
        /// Fires when the convert img directory button is clicked.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void BT_convertIMGDirectory_Click(object sender, EventArgs e)
        {
            //Block this function if we're already working.
            if (isWorking)
                return;

            //Prompt the user which directory they want to convert.
            string szPath;
            if (FileDialogHelpers.ShowOpenFolderDialog("Choose a directory to convert img files.", out szPath))
                StartWork(worker_ConvertIMGDirectory, new string[] { szPath });
        }

        /// <summary>
        /// Fires when the convert tm2 file button is clicked.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void BT_convertTM2_Click(object sender, EventArgs e)
        {
            //Block this function if we're already working.
            if (isWorking)
                return;

            //Prompt the user which file they want to convert.
            string szPath = string.Empty;
            if (FileDialogHelpers.ShowOpenFileDialog(TM2Format.ToString(), "Choose a .tm2 file to convert.", out szPath))
                StartWork(worker_ConvertTM2, new string[] { szPath });
        }

        /// <summary>
        /// Fires when the convert tm2 directory button is clicked.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void BT_convertTM2Directory_Click(object sender, EventArgs e)
        {
            //Block this function if we're already working.
            if (isWorking)
                return;

            //Prompt the user which directory they want to convert.
            string szPath;
            if (FileDialogHelpers.ShowOpenFolderDialog("Choose a directory to convert tm2 files.", out szPath))
                StartWork(worker_ConvertTM2Directory, new string[] { szPath });
        }
        #endregion

        #region ExtractAndConvert
        /// <summary>
        /// Fires when the extract and convert button is clicked.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void BT_extractAndConvert_Click(object sender, EventArgs e)
        {
            //Block this function if we're already working.
            if (isWorking)
                return;

            //Prompt the user which directory they want to extract and convert.
            string szExtractPath;
            string szDestinationPath;
            if (FileDialogHelpers.ShowOpenFolderDialog("Choose a directory to extract ALL files.", out szExtractPath) &&
                FileDialogHelpers.ShowOpenFolderDialog("Choose a directory to deposit all extracted files to.", out szDestinationPath))
                StartWork(worker_ExtractAndConvert, new string[] { szExtractPath, szDestinationPath });
        }
        #endregion

        //TODO  :   add custom operations to the form.

    }
}