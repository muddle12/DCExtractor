namespace DCExtractor.Forms
{
    partial class FileConflictDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.BT_yes = new System.Windows.Forms.Button();
            this.BT_no = new System.Windows.Forms.Button();
            this.BT_yesToAll = new System.Windows.Forms.Button();
            this.BT_noToAll = new System.Windows.Forms.Button();
            this.LB_destinationFile = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.LB_sourceFile = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(248, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "File already exists at destination.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 139);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(185, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "Would you like to overwrite?";
            // 
            // BT_yes
            // 
            this.BT_yes.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.BT_yes.Location = new System.Drawing.Point(12, 171);
            this.BT_yes.Name = "BT_yes";
            this.BT_yes.Size = new System.Drawing.Size(110, 30);
            this.BT_yes.TabIndex = 2;
            this.BT_yes.Text = "Yes";
            this.BT_yes.UseVisualStyleBackColor = true;
            this.BT_yes.Click += new System.EventHandler(this.BT_yes_Click);
            // 
            // BT_no
            // 
            this.BT_no.DialogResult = System.Windows.Forms.DialogResult.No;
            this.BT_no.Location = new System.Drawing.Point(128, 171);
            this.BT_no.Name = "BT_no";
            this.BT_no.Size = new System.Drawing.Size(110, 30);
            this.BT_no.TabIndex = 3;
            this.BT_no.Text = "No";
            this.BT_no.UseVisualStyleBackColor = true;
            this.BT_no.Click += new System.EventHandler(this.BT_no_Click);
            // 
            // BT_yesToAll
            // 
            this.BT_yesToAll.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.BT_yesToAll.Location = new System.Drawing.Point(244, 171);
            this.BT_yesToAll.Name = "BT_yesToAll";
            this.BT_yesToAll.Size = new System.Drawing.Size(110, 30);
            this.BT_yesToAll.TabIndex = 4;
            this.BT_yesToAll.Text = "Yes To All";
            this.BT_yesToAll.UseVisualStyleBackColor = true;
            this.BT_yesToAll.Click += new System.EventHandler(this.BT_yesToAll_Click);
            // 
            // BT_noToAll
            // 
            this.BT_noToAll.DialogResult = System.Windows.Forms.DialogResult.No;
            this.BT_noToAll.Location = new System.Drawing.Point(359, 171);
            this.BT_noToAll.Name = "BT_noToAll";
            this.BT_noToAll.Size = new System.Drawing.Size(110, 30);
            this.BT_noToAll.TabIndex = 5;
            this.BT_noToAll.Text = "No To All";
            this.BT_noToAll.UseVisualStyleBackColor = true;
            this.BT_noToAll.Click += new System.EventHandler(this.BT_noToAll_Click);
            // 
            // LB_destinationFile
            // 
            this.LB_destinationFile.AutoSize = true;
            this.LB_destinationFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_destinationFile.Location = new System.Drawing.Point(95, 51);
            this.LB_destinationFile.Name = "LB_destinationFile";
            this.LB_destinationFile.Size = new System.Drawing.Size(0, 17);
            this.LB_destinationFile.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(6, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 17);
            this.label4.TabIndex = 8;
            this.label4.Text = "Source:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(6, 51);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(83, 17);
            this.label10.TabIndex = 9;
            this.label10.Text = "Destination:";
            // 
            // LB_sourceFile
            // 
            this.LB_sourceFile.AutoSize = true;
            this.LB_sourceFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_sourceFile.Location = new System.Drawing.Point(95, 16);
            this.LB_sourceFile.Name = "LB_sourceFile";
            this.LB_sourceFile.Size = new System.Drawing.Size(0, 17);
            this.LB_sourceFile.TabIndex = 10;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.LB_sourceFile);
            this.groupBox1.Controls.Add(this.LB_destinationFile);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(15, 43);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(714, 82);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            // 
            // FileConflictDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(741, 213);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.BT_noToAll);
            this.Controls.Add(this.BT_yesToAll);
            this.Controls.Add(this.BT_no);
            this.Controls.Add(this.BT_yes);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MinimumSize = new System.Drawing.Size(757, 252);
            this.Name = "FileConflictDialog";
            this.Text = "File Conflict Detected";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button BT_yes;
        private System.Windows.Forms.Button BT_no;
        private System.Windows.Forms.Button BT_yesToAll;
        private System.Windows.Forms.Button BT_noToAll;
        private System.Windows.Forms.Label LB_destinationFile;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label LB_sourceFile;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}