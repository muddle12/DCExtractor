namespace DCExtractor
{
    partial class DCExtractorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DCExtractorForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BT_extract = new System.Windows.Forms.ToolStripMenuItem();
            this.BT_ExtractDAT = new System.Windows.Forms.ToolStripMenuItem();
            this.pakextractbutton = new System.Windows.Forms.ToolStripMenuItem();
            this.BT_extractPAKFile = new System.Windows.Forms.ToolStripMenuItem();
            this.BT_extractPAKDirectory = new System.Windows.Forms.ToolStripMenuItem();
            this.BT_convert = new System.Windows.Forms.ToolStripMenuItem();
            this.mDSToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.objmenuitem = new System.Windows.Forms.ToolStripMenuItem();
            this.BT_convertMDSOBJFile = new System.Windows.Forms.ToolStripMenuItem();
            this.BT_convertMDSOBJDirectory = new System.Windows.Forms.ToolStripMenuItem();
            this.smdmenuitem = new System.Windows.Forms.ToolStripMenuItem();
            this.BT_convertMDSSMDFile = new System.Windows.Forms.ToolStripMenuItem();
            this.BT_convertMDSSMDDirectory = new System.Windows.Forms.ToolStripMenuItem();
            this.iMGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BT_convertIMGFile = new System.Windows.Forms.ToolStripMenuItem();
            this.BT_convertIMGDirectory = new System.Windows.Forms.ToolStripMenuItem();
            this.tM2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BT_convertTM2File = new System.Windows.Forms.ToolStripMenuItem();
            this.BT_convertTM2Directory = new System.Windows.Forms.ToolStripMenuItem();
            this.BT_extractAndConvert = new System.Windows.Forms.ToolStripMenuItem();
            this.BT_close = new System.Windows.Forms.ToolStripMenuItem();
            this.BT_settings = new System.Windows.Forms.ToolStripMenuItem();
            this.DEBUG_menuStripButton = new System.Windows.Forms.ToolStripMenuItem();
            this.DEBUG_fileCompareButton = new System.Windows.Forms.ToolStripMenuItem();
            this.DEBUG_fileCompareSubButton = new System.Windows.Forms.ToolStripMenuItem();
            this.DEBUG_BT_compareFile = new System.Windows.Forms.ToolStripMenuItem();
            this.DEBUG_BT_compareDirectory = new System.Windows.Forms.ToolStripMenuItem();
            this.DEBUG_mdsMetaDataButton = new System.Windows.Forms.ToolStripMenuItem();
            this.DEBUG_mdsMetaDataSubButton = new System.Windows.Forms.ToolStripMenuItem();
            this.DEBUG_BT_genMDSMeta = new System.Windows.Forms.ToolStripMenuItem();
            this.DEBUG_BT_genMDSMetaDir = new System.Windows.Forms.ToolStripMenuItem();
            this.DEBUG_wgtMetaDataButton = new System.Windows.Forms.ToolStripMenuItem();
            this.DEBUG_wgtMetaDataSubButton = new System.Windows.Forms.ToolStripMenuItem();
            this.DEBUG_BT_generateWGTMeta = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.PB_progress = new System.Windows.Forms.ProgressBar();
            this.BT_cancelWork = new System.Windows.Forms.Button();
            this.LB_progressText = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.BT_settings,
            this.DEBUG_menuStripButton});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(375, 24);
            this.menuStrip1.TabIndex = 9;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BT_extract,
            this.BT_convert,
            this.BT_extractAndConvert,
            this.BT_close});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            this.fileToolStripMenuItem.ToolTipText = "A list of operations performed by the program";
            // 
            // BT_extract
            // 
            this.BT_extract.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BT_ExtractDAT,
            this.pakextractbutton});
            this.BT_extract.Name = "BT_extract";
            this.BT_extract.Size = new System.Drawing.Size(259, 22);
            this.BT_extract.Text = "Extract";
            this.BT_extract.ToolTipText = "Functions which extract data from an archive";
            // 
            // BT_ExtractDAT
            // 
            this.BT_ExtractDAT.Name = "BT_ExtractDAT";
            this.BT_ExtractDAT.Size = new System.Drawing.Size(287, 22);
            this.BT_ExtractDAT.Text = "DAT";
            this.BT_ExtractDAT.ToolTipText = "Extract the contents of a Level-5 .dat file and its accompanying .hd2 or .hd3 fil" +
    "e";
            this.BT_ExtractDAT.Click += new System.EventHandler(this.BT_ExtractDAT_Click);
            // 
            // pakextractbutton
            // 
            this.pakextractbutton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BT_extractPAKFile,
            this.BT_extractPAKDirectory});
            this.pakextractbutton.Name = "pakextractbutton";
            this.pakextractbutton.Size = new System.Drawing.Size(287, 22);
            this.pakextractbutton.Text = "PAK, CHR, IPK, MPK, PCP, EFP, SND, SKY";
            this.pakextractbutton.ToolTipText = "Extract the contents of a Level-5 .pak file type. There are many different types " +
    "of pak file";
            // 
            // BT_extractPAKFile
            // 
            this.BT_extractPAKFile.Name = "BT_extractPAKFile";
            this.BT_extractPAKFile.Size = new System.Drawing.Size(122, 22);
            this.BT_extractPAKFile.Text = "File";
            this.BT_extractPAKFile.ToolTipText = "Extracts a pak file\'s contents to the same directory";
            this.BT_extractPAKFile.Click += new System.EventHandler(this.BT_extractPAKFile_Click);
            // 
            // BT_extractPAKDirectory
            // 
            this.BT_extractPAKDirectory.Name = "BT_extractPAKDirectory";
            this.BT_extractPAKDirectory.Size = new System.Drawing.Size(122, 22);
            this.BT_extractPAKDirectory.Text = "Directory";
            this.BT_extractPAKDirectory.ToolTipText = "Extracts all pak-related files in the directory and subdirectories to their respe" +
    "ctive directories";
            this.BT_extractPAKDirectory.Click += new System.EventHandler(this.BT_extractPAKDirectory_Click);
            // 
            // BT_convert
            // 
            this.BT_convert.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mDSToolStripMenuItem2,
            this.iMGToolStripMenuItem,
            this.tM2ToolStripMenuItem});
            this.BT_convert.Name = "BT_convert";
            this.BT_convert.Size = new System.Drawing.Size(259, 22);
            this.BT_convert.Text = "Convert";
            this.BT_convert.ToolTipText = "Functions which convert files from one format to another";
            // 
            // mDSToolStripMenuItem2
            // 
            this.mDSToolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.objmenuitem,
            this.smdmenuitem});
            this.mDSToolStripMenuItem2.Name = "mDSToolStripMenuItem2";
            this.mDSToolStripMenuItem2.Size = new System.Drawing.Size(99, 22);
            this.mDSToolStripMenuItem2.Text = "MDS";
            this.mDSToolStripMenuItem2.ToolTipText = "Functions for converting Level-5 .mds model files into other formats";
            // 
            // objmenuitem
            // 
            this.objmenuitem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BT_convertMDSOBJFile,
            this.BT_convertMDSOBJDirectory});
            this.objmenuitem.Name = "objmenuitem";
            this.objmenuitem.Size = new System.Drawing.Size(99, 22);
            this.objmenuitem.Text = "OBJ";
            this.objmenuitem.ToolTipText = "Functions for converting Level-5 .mds models to wavefront .obj models";
            // 
            // BT_convertMDSOBJFile
            // 
            this.BT_convertMDSOBJFile.Name = "BT_convertMDSOBJFile";
            this.BT_convertMDSOBJFile.Size = new System.Drawing.Size(122, 22);
            this.BT_convertMDSOBJFile.Text = "File";
            this.BT_convertMDSOBJFile.ToolTipText = "Converts the selected Level-5 .mds model to a wavefront .obj file";
            this.BT_convertMDSOBJFile.Click += new System.EventHandler(this.BT_convertMDStoOBJFile_Click);
            // 
            // BT_convertMDSOBJDirectory
            // 
            this.BT_convertMDSOBJDirectory.Name = "BT_convertMDSOBJDirectory";
            this.BT_convertMDSOBJDirectory.Size = new System.Drawing.Size(122, 22);
            this.BT_convertMDSOBJDirectory.Text = "Directory";
            this.BT_convertMDSOBJDirectory.ToolTipText = "Converts a directory of Level-5 .mds models to wavefront .obj models and deposits" +
    " them in the same directory";
            this.BT_convertMDSOBJDirectory.Click += new System.EventHandler(this.BT_convertMDStoOBJDirectory_Click);
            // 
            // smdmenuitem
            // 
            this.smdmenuitem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BT_convertMDSSMDFile,
            this.BT_convertMDSSMDDirectory});
            this.smdmenuitem.Name = "smdmenuitem";
            this.smdmenuitem.Size = new System.Drawing.Size(99, 22);
            this.smdmenuitem.Text = "SMD";
            this.smdmenuitem.ToolTipText = "Functions for converting Level-5 .mds models to studiomdl .smd models";
            // 
            // BT_convertMDSSMDFile
            // 
            this.BT_convertMDSSMDFile.Name = "BT_convertMDSSMDFile";
            this.BT_convertMDSSMDFile.Size = new System.Drawing.Size(122, 22);
            this.BT_convertMDSSMDFile.Text = "File";
            this.BT_convertMDSSMDFile.ToolTipText = "Converts the selected Level-5 .mds model to a studiomdl .smd file";
            this.BT_convertMDSSMDFile.Click += new System.EventHandler(this.BT_convertMDStoSMDFile_Click);
            // 
            // BT_convertMDSSMDDirectory
            // 
            this.BT_convertMDSSMDDirectory.Name = "BT_convertMDSSMDDirectory";
            this.BT_convertMDSSMDDirectory.Size = new System.Drawing.Size(122, 22);
            this.BT_convertMDSSMDDirectory.Text = "Directory";
            this.BT_convertMDSSMDDirectory.ToolTipText = "Converts a directory of Level-5 .mds models to studiomdl .smd models and deposits" +
    " them in the same directory";
            this.BT_convertMDSSMDDirectory.Click += new System.EventHandler(this.BT_convertMDStoSMDDirectory_Click);
            // 
            // iMGToolStripMenuItem
            // 
            this.iMGToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BT_convertIMGFile,
            this.BT_convertIMGDirectory});
            this.iMGToolStripMenuItem.Name = "iMGToolStripMenuItem";
            this.iMGToolStripMenuItem.Size = new System.Drawing.Size(99, 22);
            this.iMGToolStripMenuItem.Text = "IMG";
            this.iMGToolStripMenuItem.ToolTipText = "Functions for converting Level-5 image pack .img files into other formats";
            // 
            // BT_convertIMGFile
            // 
            this.BT_convertIMGFile.Name = "BT_convertIMGFile";
            this.BT_convertIMGFile.Size = new System.Drawing.Size(122, 22);
            this.BT_convertIMGFile.Text = "File";
            this.BT_convertIMGFile.ToolTipText = "Unpacks a Level-5 image pack .img to .png files";
            this.BT_convertIMGFile.Click += new System.EventHandler(this.BT_convertIMGFile_Click);
            // 
            // BT_convertIMGDirectory
            // 
            this.BT_convertIMGDirectory.Name = "BT_convertIMGDirectory";
            this.BT_convertIMGDirectory.Size = new System.Drawing.Size(122, 22);
            this.BT_convertIMGDirectory.Text = "Directory";
            this.BT_convertIMGDirectory.ToolTipText = "Unpacks a directory of Level-5 image packs to .png files";
            this.BT_convertIMGDirectory.Click += new System.EventHandler(this.BT_convertIMGDirectory_Click);
            // 
            // tM2ToolStripMenuItem
            // 
            this.tM2ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BT_convertTM2File,
            this.BT_convertTM2Directory});
            this.tM2ToolStripMenuItem.Name = "tM2ToolStripMenuItem";
            this.tM2ToolStripMenuItem.Size = new System.Drawing.Size(99, 22);
            this.tM2ToolStripMenuItem.Text = "TM2";
            this.tM2ToolStripMenuItem.ToolTipText = "Functions for converting PS2 .tm2 files into .png files (This function is not sup" +
    "ported and may not work)";
            // 
            // BT_convertTM2File
            // 
            this.BT_convertTM2File.Name = "BT_convertTM2File";
            this.BT_convertTM2File.Size = new System.Drawing.Size(122, 22);
            this.BT_convertTM2File.Text = "File";
            this.BT_convertTM2File.ToolTipText = "Converts a PS2 .tm2 image into a .png file";
            this.BT_convertTM2File.Click += new System.EventHandler(this.BT_convertTM2_Click);
            // 
            // BT_convertTM2Directory
            // 
            this.BT_convertTM2Directory.Name = "BT_convertTM2Directory";
            this.BT_convertTM2Directory.Size = new System.Drawing.Size(122, 22);
            this.BT_convertTM2Directory.Text = "Directory";
            this.BT_convertTM2Directory.ToolTipText = "Converts a directory of PS2 .tm2 images into a .png files";
            this.BT_convertTM2Directory.Click += new System.EventHandler(this.BT_convertTM2Directory_Click);
            // 
            // BT_extractAndConvert
            // 
            this.BT_extractAndConvert.Name = "BT_extractAndConvert";
            this.BT_extractAndConvert.Size = new System.Drawing.Size(259, 22);
            this.BT_extractAndConvert.Text = "Unpack All (Extract/Convert/Move)";
            this.BT_extractAndConvert.ToolTipText = resources.GetString("BT_extractAndConvert.ToolTipText");
            this.BT_extractAndConvert.Click += new System.EventHandler(this.BT_extractAndConvert_Click);
            // 
            // BT_close
            // 
            this.BT_close.Name = "BT_close";
            this.BT_close.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.BT_close.Size = new System.Drawing.Size(259, 22);
            this.BT_close.Text = "Close";
            this.BT_close.ToolTipText = "Close the application";
            this.BT_close.Click += new System.EventHandler(this.BT_close_Click);
            // 
            // BT_settings
            // 
            this.BT_settings.Name = "BT_settings";
            this.BT_settings.Size = new System.Drawing.Size(61, 20);
            this.BT_settings.Text = "Settings";
            this.BT_settings.ToolTipText = "View and change settings for the application";
            this.BT_settings.Click += new System.EventHandler(this.BT_settings_Click);
            // 
            // DEBUG_menuStripButton
            // 
            this.DEBUG_menuStripButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DEBUG_fileCompareButton,
            this.DEBUG_mdsMetaDataButton,
            this.DEBUG_wgtMetaDataButton});
            this.DEBUG_menuStripButton.Name = "DEBUG_menuStripButton";
            this.DEBUG_menuStripButton.Size = new System.Drawing.Size(54, 20);
            this.DEBUG_menuStripButton.Text = "Debug";
            this.DEBUG_menuStripButton.ToolTipText = "Functions related to debugging outputs from this application";
            // 
            // DEBUG_fileCompareButton
            // 
            this.DEBUG_fileCompareButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DEBUG_fileCompareSubButton});
            this.DEBUG_fileCompareButton.Name = "DEBUG_fileCompareButton";
            this.DEBUG_fileCompareButton.Size = new System.Drawing.Size(100, 22);
            this.DEBUG_fileCompareButton.Text = "File";
            this.DEBUG_fileCompareButton.ToolTipText = "File-related debug operations";
            // 
            // DEBUG_fileCompareSubButton
            // 
            this.DEBUG_fileCompareSubButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DEBUG_BT_compareFile,
            this.DEBUG_BT_compareDirectory});
            this.DEBUG_fileCompareSubButton.Name = "DEBUG_fileCompareSubButton";
            this.DEBUG_fileCompareSubButton.Size = new System.Drawing.Size(123, 22);
            this.DEBUG_fileCompareSubButton.Text = "Compare";
            this.DEBUG_fileCompareSubButton.ToolTipText = "Functions for comparing files against each other";
            // 
            // DEBUG_BT_compareFile
            // 
            this.DEBUG_BT_compareFile.Name = "DEBUG_BT_compareFile";
            this.DEBUG_BT_compareFile.Size = new System.Drawing.Size(122, 22);
            this.DEBUG_BT_compareFile.Text = "File";
            this.DEBUG_BT_compareFile.ToolTipText = "Compares two files and determines if they are not the same";
            this.DEBUG_BT_compareFile.Click += new System.EventHandler(this.BT_compareFile_Click);
            // 
            // DEBUG_BT_compareDirectory
            // 
            this.DEBUG_BT_compareDirectory.Name = "DEBUG_BT_compareDirectory";
            this.DEBUG_BT_compareDirectory.Size = new System.Drawing.Size(122, 22);
            this.DEBUG_BT_compareDirectory.Text = "Directory";
            this.DEBUG_BT_compareDirectory.ToolTipText = "Compares entire directories of files against other directories to determine if an" +
    "y files are not the same";
            this.DEBUG_BT_compareDirectory.Click += new System.EventHandler(this.BT_compareDirectory_Click);
            // 
            // DEBUG_mdsMetaDataButton
            // 
            this.DEBUG_mdsMetaDataButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DEBUG_mdsMetaDataSubButton});
            this.DEBUG_mdsMetaDataButton.Name = "DEBUG_mdsMetaDataButton";
            this.DEBUG_mdsMetaDataButton.Size = new System.Drawing.Size(100, 22);
            this.DEBUG_mdsMetaDataButton.Text = "MDS";
            this.DEBUG_mdsMetaDataButton.ToolTipText = "MDS-related debug functions";
            // 
            // DEBUG_mdsMetaDataSubButton
            // 
            this.DEBUG_mdsMetaDataSubButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DEBUG_BT_genMDSMeta,
            this.DEBUG_BT_genMDSMetaDir});
            this.DEBUG_mdsMetaDataSubButton.Name = "DEBUG_mdsMetaDataSubButton";
            this.DEBUG_mdsMetaDataSubButton.Size = new System.Drawing.Size(128, 22);
            this.DEBUG_mdsMetaDataSubButton.Text = "Meta Data";
            this.DEBUG_mdsMetaDataSubButton.ToolTipText = "Functions for generating meta data for MDS files";
            // 
            // DEBUG_BT_genMDSMeta
            // 
            this.DEBUG_BT_genMDSMeta.Name = "DEBUG_BT_genMDSMeta";
            this.DEBUG_BT_genMDSMeta.Size = new System.Drawing.Size(122, 22);
            this.DEBUG_BT_genMDSMeta.Text = "File";
            this.DEBUG_BT_genMDSMeta.ToolTipText = "Generate a meta file for debugging a .mds file";
            this.DEBUG_BT_genMDSMeta.Click += new System.EventHandler(this.BT_genMDSMetaData_Click);
            // 
            // DEBUG_BT_genMDSMetaDir
            // 
            this.DEBUG_BT_genMDSMetaDir.Name = "DEBUG_BT_genMDSMetaDir";
            this.DEBUG_BT_genMDSMetaDir.Size = new System.Drawing.Size(122, 22);
            this.DEBUG_BT_genMDSMetaDir.Text = "Directory";
            this.DEBUG_BT_genMDSMetaDir.ToolTipText = "Generates meta files for all .mds files in a directory";
            this.DEBUG_BT_genMDSMetaDir.Click += new System.EventHandler(this.BT_genMDSMetaAll_Click);
            // 
            // DEBUG_wgtMetaDataButton
            // 
            this.DEBUG_wgtMetaDataButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DEBUG_wgtMetaDataSubButton});
            this.DEBUG_wgtMetaDataButton.Name = "DEBUG_wgtMetaDataButton";
            this.DEBUG_wgtMetaDataButton.Size = new System.Drawing.Size(100, 22);
            this.DEBUG_wgtMetaDataButton.Text = "WGT";
            this.DEBUG_wgtMetaDataButton.ToolTipText = "WGT-related debug functions";
            // 
            // DEBUG_wgtMetaDataSubButton
            // 
            this.DEBUG_wgtMetaDataSubButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DEBUG_BT_generateWGTMeta});
            this.DEBUG_wgtMetaDataSubButton.Name = "DEBUG_wgtMetaDataSubButton";
            this.DEBUG_wgtMetaDataSubButton.Size = new System.Drawing.Size(128, 22);
            this.DEBUG_wgtMetaDataSubButton.Text = "Meta Deta";
            this.DEBUG_wgtMetaDataSubButton.ToolTipText = "Functions for generating meta data for WGT files";
            // 
            // DEBUG_BT_generateWGTMeta
            // 
            this.DEBUG_BT_generateWGTMeta.Name = "DEBUG_BT_generateWGTMeta";
            this.DEBUG_BT_generateWGTMeta.Size = new System.Drawing.Size(92, 22);
            this.DEBUG_BT_generateWGTMeta.Text = "File";
            this.DEBUG_BT_generateWGTMeta.ToolTipText = "Generate a meta file for debugging a .wgt file";
            this.DEBUG_BT_generateWGTMeta.Click += new System.EventHandler(this.BT_generateWGTMeta_Click);
            // 
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 15000;
            this.toolTip1.AutoPopDelay = 30000;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.ReshowDelay = 500;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(13, 34);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(351, 17);
            this.label4.TabIndex = 14;
            this.label4.Text = "Extracts common formats from PS2-era Level-5 games";
            // 
            // PB_progress
            // 
            this.PB_progress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PB_progress.Location = new System.Drawing.Point(12, 221);
            this.PB_progress.Name = "PB_progress";
            this.PB_progress.Size = new System.Drawing.Size(320, 23);
            this.PB_progress.TabIndex = 15;
            // 
            // BT_cancelWork
            // 
            this.BT_cancelWork.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BT_cancelWork.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_cancelWork.Location = new System.Drawing.Point(340, 221);
            this.BT_cancelWork.Name = "BT_cancelWork";
            this.BT_cancelWork.Size = new System.Drawing.Size(23, 23);
            this.BT_cancelWork.TabIndex = 16;
            this.BT_cancelWork.Text = "X";
            this.BT_cancelWork.UseVisualStyleBackColor = true;
            this.BT_cancelWork.Click += new System.EventHandler(this.BT_cancelWork_Click);
            // 
            // LB_progressText
            // 
            this.LB_progressText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LB_progressText.AutoSize = true;
            this.LB_progressText.BackColor = System.Drawing.Color.Transparent;
            this.LB_progressText.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_progressText.Location = new System.Drawing.Point(13, 201);
            this.LB_progressText.Name = "LB_progressText";
            this.LB_progressText.Size = new System.Drawing.Size(20, 17);
            this.LB_progressText.TabIndex = 17;
            this.LB_progressText.Text = "...";
            this.LB_progressText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DCExtractorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(375, 256);
            this.Controls.Add(this.LB_progressText);
            this.Controls.Add(this.BT_cancelWork);
            this.Controls.Add(this.PB_progress);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "DCExtractorForm";
            this.Text = "DCExtractor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

#endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem BT_extract;
        private System.Windows.Forms.ToolStripMenuItem BT_ExtractDAT;
        private System.Windows.Forms.ToolStripMenuItem BT_close;
        private System.Windows.Forms.ToolStripMenuItem BT_convert;
        private System.Windows.Forms.ToolStripMenuItem mDSToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem objmenuitem;
        private System.Windows.Forms.ToolStripMenuItem smdmenuitem;
        private System.Windows.Forms.ToolStripMenuItem pakextractbutton;
        private System.Windows.Forms.ToolStripMenuItem BT_extractPAKFile;
        private System.Windows.Forms.ToolStripMenuItem BT_extractPAKDirectory;
        private System.Windows.Forms.ToolStripMenuItem iMGToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem BT_convertIMGFile;
        private System.Windows.Forms.ToolStripMenuItem BT_convertIMGDirectory;
        private System.Windows.Forms.ToolStripMenuItem tM2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem BT_convertTM2File;
        private System.Windows.Forms.ToolStripMenuItem BT_convertTM2Directory;
        private System.Windows.Forms.ToolStripMenuItem BT_extractAndConvert;
        private System.Windows.Forms.ToolStripMenuItem BT_settings;
        private System.Windows.Forms.ToolStripMenuItem BT_convertMDSOBJFile;
        private System.Windows.Forms.ToolStripMenuItem BT_convertMDSOBJDirectory;
        private System.Windows.Forms.ToolStripMenuItem BT_convertMDSSMDFile;
        private System.Windows.Forms.ToolStripMenuItem BT_convertMDSSMDDirectory;
        private System.Windows.Forms.ToolStripMenuItem DEBUG_menuStripButton;
        private System.Windows.Forms.ToolStripMenuItem DEBUG_wgtMetaDataButton;
        private System.Windows.Forms.ToolStripMenuItem DEBUG_wgtMetaDataSubButton;
        private System.Windows.Forms.ToolStripMenuItem DEBUG_BT_generateWGTMeta;
        private System.Windows.Forms.ToolStripMenuItem DEBUG_fileCompareButton;
        private System.Windows.Forms.ToolStripMenuItem DEBUG_fileCompareSubButton;
        private System.Windows.Forms.ToolStripMenuItem DEBUG_BT_compareFile;
        private System.Windows.Forms.ToolStripMenuItem DEBUG_BT_compareDirectory;
        private System.Windows.Forms.ToolStripMenuItem DEBUG_mdsMetaDataButton;
        private System.Windows.Forms.ToolStripMenuItem DEBUG_mdsMetaDataSubButton;
        private System.Windows.Forms.ToolStripMenuItem DEBUG_BT_genMDSMeta;
        private System.Windows.Forms.ToolStripMenuItem DEBUG_BT_genMDSMetaDir;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ProgressBar PB_progress;
        private System.Windows.Forms.Button BT_cancelWork;
        private System.Windows.Forms.Label LB_progressText;
    }
}

