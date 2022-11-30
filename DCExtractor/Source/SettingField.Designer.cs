namespace DCExtractor.Controls
{
    partial class SettingField
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.BT_state1 = new System.Windows.Forms.RadioButton();
            this.BT_state2 = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.splitContainer1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(340, 51);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Setting";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(3, 19);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.BT_state1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.BT_state2);
            this.splitContainer1.Size = new System.Drawing.Size(334, 29);
            this.splitContainer1.SplitterDistance = 167;
            this.splitContainer1.TabIndex = 1;
            // 
            // BT_state1
            // 
            this.BT_state1.Appearance = System.Windows.Forms.Appearance.Button;
            this.BT_state1.AutoSize = true;
            this.BT_state1.Checked = true;
            this.BT_state1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BT_state1.Location = new System.Drawing.Point(0, 0);
            this.BT_state1.Name = "BT_state1";
            this.BT_state1.Size = new System.Drawing.Size(167, 29);
            this.BT_state1.TabIndex = 2;
            this.BT_state1.TabStop = true;
            this.BT_state1.Text = "Option 1";
            this.BT_state1.UseVisualStyleBackColor = true;
            this.BT_state1.CheckedChanged += new System.EventHandler(this.BT_state1_CheckedChanged);
            // 
            // BT_state2
            // 
            this.BT_state2.Appearance = System.Windows.Forms.Appearance.Button;
            this.BT_state2.AutoSize = true;
            this.BT_state2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BT_state2.Location = new System.Drawing.Point(0, 0);
            this.BT_state2.Name = "BT_state2";
            this.BT_state2.Size = new System.Drawing.Size(163, 29);
            this.BT_state2.TabIndex = 3;
            this.BT_state2.Text = "Option 2";
            this.BT_state2.UseVisualStyleBackColor = true;
            this.BT_state2.Click += new System.EventHandler(this.BT_state2_CheckedChanged);
            // 
            // SettingField
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "SettingField";
            this.Size = new System.Drawing.Size(340, 51);
            this.groupBox1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.RadioButton BT_state1;
        private System.Windows.Forms.RadioButton BT_state2;
    }
}
