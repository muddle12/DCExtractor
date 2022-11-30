namespace DCExtractor.Forms
{
    partial class SettingsForm
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
            this.BT_save = new System.Windows.Forms.Button();
            this.BT_cancel = new System.Windows.Forms.Button();
            this.SC_options = new System.Windows.Forms.SplitContainer();
            this.SF_showFileExtensionWarnings = new DCExtractor.Controls.SettingField();
            this.SF_ShowMDSWarnings = new DCExtractor.Controls.SettingField();
            this.SF_exportAnimation = new DCExtractor.Controls.SettingField();
            this.SF_modelConversion = new DCExtractor.Controls.SettingField();
            ((System.ComponentModel.ISupportInitialize)(this.SC_options)).BeginInit();
            this.SC_options.Panel1.SuspendLayout();
            this.SC_options.Panel2.SuspendLayout();
            this.SC_options.SuspendLayout();
            this.SuspendLayout();
            // 
            // BT_save
            // 
            this.BT_save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BT_save.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.BT_save.Location = new System.Drawing.Point(12, 8);
            this.BT_save.Name = "BT_save";
            this.BT_save.Size = new System.Drawing.Size(100, 30);
            this.BT_save.TabIndex = 1;
            this.BT_save.Text = "Save";
            this.BT_save.UseVisualStyleBackColor = true;
            // 
            // BT_cancel
            // 
            this.BT_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BT_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BT_cancel.Location = new System.Drawing.Point(449, 8);
            this.BT_cancel.Name = "BT_cancel";
            this.BT_cancel.Size = new System.Drawing.Size(100, 30);
            this.BT_cancel.TabIndex = 3;
            this.BT_cancel.Text = "Cancel";
            this.BT_cancel.UseVisualStyleBackColor = true;
            // 
            // SC_options
            // 
            this.SC_options.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SC_options.IsSplitterFixed = true;
            this.SC_options.Location = new System.Drawing.Point(0, 0);
            this.SC_options.Name = "SC_options";
            this.SC_options.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SC_options.Panel1
            // 
            this.SC_options.Panel1.Controls.Add(this.SF_showFileExtensionWarnings);
            this.SC_options.Panel1.Controls.Add(this.SF_ShowMDSWarnings);
            this.SC_options.Panel1.Controls.Add(this.SF_exportAnimation);
            this.SC_options.Panel1.Controls.Add(this.SF_modelConversion);
            // 
            // SC_options.Panel2
            // 
            this.SC_options.Panel2.Controls.Add(this.BT_save);
            this.SC_options.Panel2.Controls.Add(this.BT_cancel);
            this.SC_options.Size = new System.Drawing.Size(561, 305);
            this.SC_options.SplitterDistance = 251;
            this.SC_options.TabIndex = 2;
            // 
            // SF_showFileExtensionWarnings
            // 
            this.SF_showFileExtensionWarnings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SF_showFileExtensionWarnings.FormalName = "Show File Extension Warnings";
            this.SF_showFileExtensionWarnings.Location = new System.Drawing.Point(12, 181);
            this.SF_showFileExtensionWarnings.Name = "SF_showFileExtensionWarnings";
            this.SF_showFileExtensionWarnings.OptionName1 = "No";
            this.SF_showFileExtensionWarnings.OptionName2 = "Yes";
            this.SF_showFileExtensionWarnings.SettingName = "ShowFileExtensionWarnings";
            this.SF_showFileExtensionWarnings.Size = new System.Drawing.Size(537, 51);
            this.SF_showFileExtensionWarnings.TabIndex = 3;
            this.SF_showFileExtensionWarnings.Value = 0;
            // 
            // SF_ShowMDSWarnings
            // 
            this.SF_ShowMDSWarnings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SF_ShowMDSWarnings.FormalName = "Show MDS Warnings";
            this.SF_ShowMDSWarnings.Location = new System.Drawing.Point(12, 126);
            this.SF_ShowMDSWarnings.Name = "SF_ShowMDSWarnings";
            this.SF_ShowMDSWarnings.OptionName1 = "No";
            this.SF_ShowMDSWarnings.OptionName2 = "Yes";
            this.SF_ShowMDSWarnings.SettingName = "ShowMDSWarnings";
            this.SF_ShowMDSWarnings.Size = new System.Drawing.Size(537, 51);
            this.SF_ShowMDSWarnings.TabIndex = 2;
            this.SF_ShowMDSWarnings.Value = 0;
            // 
            // SF_exportAnimation
            // 
            this.SF_exportAnimation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SF_exportAnimation.FormalName = "Export Animations";
            this.SF_exportAnimation.Location = new System.Drawing.Point(12, 69);
            this.SF_exportAnimation.Name = "SF_exportAnimation";
            this.SF_exportAnimation.OptionName1 = "No";
            this.SF_exportAnimation.OptionName2 = "Yes";
            this.SF_exportAnimation.SettingName = "ExportAnimations";
            this.SF_exportAnimation.Size = new System.Drawing.Size(537, 51);
            this.SF_exportAnimation.TabIndex = 1;
            this.SF_exportAnimation.Value = 0;
            // 
            // SF_modelConversion
            // 
            this.SF_modelConversion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SF_modelConversion.FormalName = "Model Conversion";
            this.SF_modelConversion.Location = new System.Drawing.Point(12, 12);
            this.SF_modelConversion.Name = "SF_modelConversion";
            this.SF_modelConversion.OptionName1 = "Export One Model (With All Meshes)";
            this.SF_modelConversion.OptionName2 = "Export Many Models(With One Mesh)";
            this.SF_modelConversion.SettingName = "ModelConversion";
            this.SF_modelConversion.Size = new System.Drawing.Size(537, 51);
            this.SF_modelConversion.TabIndex = 0;
            this.SF_modelConversion.Value = 0;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(561, 305);
            this.Controls.Add(this.SC_options);
            this.Name = "SettingsForm";
            this.Text = "DCExtractor Settings";
            this.SC_options.Panel1.ResumeLayout(false);
            this.SC_options.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SC_options)).EndInit();
            this.SC_options.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button BT_save;
        private System.Windows.Forms.Button BT_cancel;
        private System.Windows.Forms.SplitContainer SC_options;
        private Controls.SettingField SF_showFileExtensionWarnings;
        private Controls.SettingField SF_ShowMDSWarnings;
        private Controls.SettingField SF_exportAnimation;
        private Controls.SettingField SF_modelConversion;
    }
}