using DC.IO;
using DC.Types;
using DCExtractor.Data;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using static Custom.Forms.FileDialogHelpers;

namespace DCExtractor
{
    /// <summary>
    /// Class   :   "DCExtractorForm_Operations"
    /// 
    /// Purpose :   adding a progress bar to DCExtractor made things a lot more complicated than I intended. In order to
    /// cut down on the complexity in adding new operations to the form, I spun off this partial class to give you
    /// an easier bird's eye view of all the functions the extractor can perform. You can add new ones here, but you'll
    /// need to add buttons to the form(and event handlers) to create new functionality to DCExtractor. Follow the TODOs
    /// for more information on how this is done.
    /// </summary>
    public partial class DCExtractorForm : Form
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Data Members.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Our directory of workers. 
        DCBackgroundWorker worker_ExtractDAT;
        DCBackgroundWorker worker_ExtractPAK;
        DCBackgroundWorker worker_ExtractPAKDirectory;
        DCBackgroundWorker worker_ConvertMDStoOBJ;
        DCBackgroundWorker worker_ConvertMDStoOBJDirectory;
        DCBackgroundWorker worker_ConvertMDStoSMDFile;
        DCBackgroundWorker worker_ConvertMDStoSMDDirectory;
        DCBackgroundWorker worker_ConvertIMGFile;
        DCBackgroundWorker worker_ConvertIMGDirectory;
        DCBackgroundWorker worker_ConvertTM2;
        DCBackgroundWorker worker_ConvertTM2Directory;
        DCBackgroundWorker worker_ExtractAndConvert;

        //TODO  :   If you make any new operations, add a new worker to this list.


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Initializes the workers for the form.
        /// </summary>
        void InitializeWorkers()
        {
            //Create our workers.
            //TODO  :   Add new workers here for any new operations you might create.
            worker_ExtractDAT = new DCBackgroundWorker(ExtractDAT);
            worker_ExtractPAK = new DCBackgroundWorker(ExtractPAK);
            worker_ExtractPAKDirectory = new DCBackgroundWorker(ExtractPAKDirectory);
            worker_ConvertMDStoOBJ = new DCBackgroundWorker(ConvertMDStoOBJ);
            worker_ConvertMDStoOBJDirectory = new DCBackgroundWorker(ConvertMDStoOBJDirectory);
            worker_ConvertMDStoSMDFile = new DCBackgroundWorker(ConvertMDStoSMDFile);
            worker_ConvertMDStoSMDDirectory = new DCBackgroundWorker(ConvertMDStoSMDDirectory);
            worker_ConvertIMGFile = new DCBackgroundWorker(ConvertIMGFile);
            worker_ConvertIMGDirectory = new DCBackgroundWorker(ConvertIMGDirectory);
            worker_ConvertTM2 = new DCBackgroundWorker(ConvertTM2);
            worker_ConvertTM2Directory = new DCBackgroundWorker(ConvertTM2Directory);
            worker_ExtractAndConvert = new DCBackgroundWorker(ExtractAndConvert);

            //Create our list of workers.
            //TODO  :   Add new workers to this list so they will be processed by the application.
            workers = new DCBackgroundWorker[]
                {
                    worker_ExtractDAT,
                    worker_ExtractPAK,
                    worker_ExtractPAKDirectory,
                    worker_ConvertMDStoOBJ,
                    worker_ConvertMDStoOBJDirectory,
                    worker_ConvertMDStoSMDFile,
                    worker_ConvertMDStoSMDDirectory,
                    worker_ConvertIMGFile,
                    worker_ConvertIMGDirectory,
                    worker_ConvertTM2,
                    worker_ConvertTM2Directory,
                    worker_ExtractAndConvert
                };

            //Finish up the initialization of workers.
            FinishInitializingWorkers();
        }

        #region Extraction
        /// <summary>
        /// The async extraction function for DAT files.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        void ExtractDAT(object sender, DoWorkEventArgs e)
        {
            //Convert our parameters to arguments.
            string[] szParameters = e.Argument as string[];
            string szDATPath = szParameters[0];
            string szHDPath = szParameters[1];
            string szOutPath = szParameters[2];

            //Depending on which type of file system the user chose, open the DAT and extract it based on that.
            if (szHDPath.ToLower().EndsWith(HD2Format.extensions[0]))
                DAT.ExtractHD(szDATPath, szHDPath, szOutPath, DAT.HDType.Two);
            else if (szHDPath.ToLower().EndsWith(HD3Format.extensions[0]))
                DAT.ExtractHD(szDATPath, szHDPath, szOutPath, DAT.HDType.Three);
#if DEBUG
            //This is some debug code from when we were still working out whether or not we were doing the DAT extraction properly. This tests whether or not we had file conflicts during the extraction process.
            DAT.FileEntry[] tFileEntries = DAT.conflictEntries;
            if (tFileEntries.Length > 0)
                MessageBox.Show(tFileEntries.Length.ToString() + " files were overwritten during the extraction process!", "Warning! Filename conflict detected!");
#endif
        }

        /// <summary>
        /// The async extraction function for single PAK files.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        void ExtractPAK(object sender, DoWorkEventArgs e)
        {
            //Convert our parameters to arguments.
            string[] szParameters = e.Argument as string[];
            string szPath = szParameters[0];

            //Extract our PAK file.
            PAK.ExtractFile(szPath);
        }

        /// <summary>
        /// The async extraction function for a directory of PAK files.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        void ExtractPAKDirectory(object sender, DoWorkEventArgs e)
        {
            //Convert our parameters to arguments.
            string[] szParameters = e.Argument as string[];
            string szPath = szParameters[0];

            //Gather all of the PAK extensions.
            string[] szExtensions = new string[PAK_TypeList.Length];
            for (int fileType = 0; fileType < PAK_TypeList.Length; fileType++)
                szExtensions[fileType] = PAK_TypeList[fileType].extensions[0];

            //Extract the directory.
            PAK.ExtractDirectory(szPath, szExtensions);
        }
        #endregion

        #region Conversion
        /// <summary>
        /// The async conversion function for a single MDS file to OBJ.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        void ConvertMDStoOBJ(object sender, DoWorkEventArgs e)
        {
            //Convert our parameters to arguments.
            string[] szParameters = e.Argument as string[];
            string szInputPath = szParameters[0];

            //Open the mds and then convert it to wavefront obj.
            Model tModel = null;
            if (MDS.Load(szInputPath, out tModel, false))
                WavefrontOBJ.Save(tModel);
        }

        /// <summary>
        /// The async conversion function for directory of MDS files to OBJs.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        void ConvertMDStoOBJDirectory(object sender, DoWorkEventArgs e)
        {
            //Convert our parameters to arguments.
            string[] szParameters = e.Argument as string[];
            string szInputDir = szParameters[0];

            //Open the mds models and then convert them to wavefront objs.
            Model[] tModels = MDS.LoadDirectory(szInputDir);
            for (int i = 0; i < tModels.Length; i++)
                WavefrontOBJ.Save(tModels[i]);
        }

        /// <summary>
        /// The async conversion function for a single MDS file to SMD.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        void ConvertMDStoSMDFile(object sender, DoWorkEventArgs e)
        {
            //Convert our parameters to arguments.
            string[] szParameters = e.Argument as string[];
            string szPath = szParameters[0];

            //Open the mds and then convert it to studiomdl smd.
            Model tModel = null;
            if (MDS.Load(szPath, out tModel, false))
                PerformSaveSMD(tModel, Settings.modelConversion);
        }

        /// <summary>
        /// The async conversion function for directory of MDS files to SMDs.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        void ConvertMDStoSMDDirectory(object sender, DoWorkEventArgs e)
        {
            //Convert our parameters to arguments.
            string[] szParameters = e.Argument as string[];
            string szInputDir = szParameters[0];

            //Open the mds models and then convert them to studiomdl smds.
            Model[] tModels = MDS.LoadDirectory(szInputDir);
            for (int model = 0; model < tModels.Length; model++)
                PerformSaveSMD(tModels[model], Settings.modelConversion);
        }

        /// <summary>
        /// The async conversion function for a single IMG file to PNG.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        void ConvertIMGFile(object sender, DoWorkEventArgs e)
        {
            //Convert our parameters to arguments.
            string[] szParameters = e.Argument as string[];
            string szInputPath = szParameters[0];

            //Open the img and then convert the contained tm2 images into pngs.
            TIM2Image[] tImages = null;
            if (IMG.Load(szInputPath, out tImages))
            {
                for (int i = 0; i < tImages.Length; i++)
                    ImageHelpers.SavePNG(tImages[i]);
            }
        }

        /// <summary>
        /// Saves a list of png images to image files in the same directory that they were loaded.
        /// </summary>
        /// <param name="tImages">The list of images to save.</param>
        static void SavePNGs(TIM2Image[] tImages)
        {
            //Don't do this if there was a cancel.
            if (DCProgress.canceled)
                return;

            //Reset for processing.
            DCProgress.value = 0;
            DCProgress.maximum = tImages.Length;
            DCProgress.name = "Saving PNGs";

            //Loop through and save all images as pngs.
            for (int i = 0; i < tImages.Length; i++)
            {
                DCProgress.name = "Saving PNG " + Path.ChangeExtension(Path.GetFileName(tImages[i].filePath), ".png");

                //Save our png.
                ImageHelpers.SavePNG(tImages[i]);

                //Increment the progress.
                DCProgress.value = i;

                //Bail out if we've canceled.
                if (DCProgress.canceled)
                {
                    DCProgress.name = "Saving PNGs - CANCELED";
                    return;
                }
            }

            //Finish.
            DCProgress.value = DCProgress.maximum;
            DCProgress.name = "Saving PNGs - FINISHED";
        }

        /// <summary>
        /// The async conversion function for directory of IMG files to PNGs.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        void ConvertIMGDirectory(object sender, DoWorkEventArgs e)
        {
            //Convert our parameters to arguments.
            string[] szParameters = e.Argument as string[];
            string szPath = szParameters[0];

            //Open the imgs and then convert them to pngs.
            TIM2Image[] tImages = IMG.LoadDirectory(szPath);
            SavePNGs(tImages);
        }

        /// <summary>
        /// The async conversion function for a single TM2 file to PNG.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        void ConvertTM2(object sender, DoWorkEventArgs e)
        {
            //Convert our parameters to arguments.
            string[] szParameters = e.Argument as string[];
            string szPath = szParameters[0];

            //Open the tm2 and convert it to png.
            TIM2Image tImage = TM2.Load(szPath);
            if (tImage != null)
                ImageHelpers.SavePNG(tImage);
        }

        /// <summary>
        /// The async conversion function for directory of TM2 files to PNGs.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        void ConvertTM2Directory(object sender, DoWorkEventArgs e)
        {
            //Convert our parameters to arguments.
            string[] szParameters = e.Argument as string[];
            string szPath = szParameters[0];

            //Open the tm2 images and convert them to pngs.
            TIM2Image[] tImages = TM2.LoadDirectory(szPath);
            SavePNGs(tImages);
        }

        #endregion

        #region ExtractAndConvert
        /// <summary>
        /// The async extraction and conversion function which unpacks an entire DAT into converted files.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        void ExtractAndConvert(object sender, DoWorkEventArgs e)
        {
            DCProgress.name = "Unpacking All";
            DCProgress.value = 0;

            /***********************************************************************************/
            //  I'm not particularly proud of the way I went about the extraction process in the
            //  previous functions, namely dumping the files in the same directory as the source
            //  object. My reasoning was that I wanted to cut down on the bulk of folder dialogs
            //  and avoid the messy nature of using File.Move and File.Copy unecessarily without
            //  user permission to do so. Rather than barrage the user with prompts, I opted
            //  to create a singular button instead.
            //
            //  This function does a clean extract, convert, and move all in one. It's intended
            //  to be called once, as a sort of full extract of the entire archive to a project
            //  folder.
            //
            //  There's a lot going on in this function. It is non-trivial.
            /***********************************************************************************/

            //Convert our parameters to arguments.
            string[] szParameters = e.Argument as string[];
            string szExtractPath = szParameters[0];
            string szDestinationPath = szParameters[1];

            //We'll extract all the files 
            {
                //First, we'll extract all the data from the DAT.
                if (DCProgress.canceled == false)
                    DAT.ExtractHD(Path.Combine(szExtractPath, "DATA.DAT"), Path.Combine(szExtractPath, "DATA.HD2"), szExtractPath, DAT.HDType.Two);

                //Next, we'll extract all the pak related files to get the intermediate assets.
                if (DCProgress.canceled == false)
                {
                    //Gather all of the PAK extensions.
                    string[] szExtensions = new string[PAK_TypeList.Length];
                    for (int fileType = 0; fileType < PAK_TypeList.Length; fileType++)
                        szExtensions[fileType] = PAK_TypeList[fileType].extensions[0];

                    PAK.ExtractDirectory(szExtractPath, szExtensions);
                }

                //Then the model data.
                if (DCProgress.canceled == false)
                {
                    Model[] tModels = MDS.LoadDirectory(szExtractPath, false);
                    for (int model = 0; model < tModels.Length; model++)
                        PerformSaveSMD(tModels[model], Settings.modelConversion);
                }

                if (DCProgress.canceled == false)
                {
                    //Any miscellaneous TIM2 images.
                    TIM2Image[] tImages = TM2.LoadDirectory(szExtractPath);
                    SavePNGs(tImages);
                }

                if (DCProgress.canceled == false)
                {
                    //Any images.
                    TIM2Image[] tImages = IMG.LoadDirectory(szExtractPath);
                    SavePNGs(tImages);
                }
            }

            //We'll move these files to the destination.
            if (DCProgress.canceled == false)
                FileHelpers.MoveFiles(szExtractPath, szDestinationPath, new FileDialogFilter[] { SMDFormat, PNGFormat, CFGFormat }, true, new FileDialogFilter[] { SMDFormat, PNGFormat });

            if (DCProgress.canceled)
            {
                DCProgress.name = "Unpack All - CANCELED";
                return;
            }

            //Prompt the user.
            DCProgress.name = "Unpack All - FINISHED";
            DCProgress.value = DCProgress.maximum;
        }
        #endregion

        //TODO  :   add custom operations to the form.
    }
}
