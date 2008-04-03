namespace WristVizualizer
{
    partial class LoadTextureDialog
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
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.textBoxSubjectDirectory = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelErrorKinematicFile = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxKinematicFilename = new System.Windows.Forms.TextBox();
            this.radioButtonKinematicMotion = new System.Windows.Forms.RadioButton();
            this.radioButtonKinematicRT = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.radioButtonKinematicAutoRegistr = new System.Windows.Forms.RadioButton();
            this.labelErrorCropValues = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.listBoxSeries = new System.Windows.Forms.ListBox();
            this.labelErrorSubject = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxCropValuesFilename = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonBrowseSubject = new System.Windows.Forms.Button();
            this.label19 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.radioButtonManual = new System.Windows.Forms.RadioButton();
            this.radioButtonAutomatic = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(677, 406);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(98, 29);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(573, 406);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(98, 29);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // textBoxSubjectDirectory
            // 
            this.textBoxSubjectDirectory.Location = new System.Drawing.Point(111, 27);
            this.textBoxSubjectDirectory.Name = "textBoxSubjectDirectory";
            this.textBoxSubjectDirectory.Size = new System.Drawing.Size(215, 20);
            this.textBoxSubjectDirectory.TabIndex = 5;
            this.textBoxSubjectDirectory.TextChanged += new System.EventHandler(this.textBoxSubjectDirectory_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelErrorKinematicFile);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.textBoxKinematicFilename);
            this.groupBox1.Controls.Add(this.radioButtonKinematicMotion);
            this.groupBox1.Controls.Add(this.radioButtonKinematicRT);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.radioButtonKinematicAutoRegistr);
            this.groupBox1.Controls.Add(this.labelErrorCropValues);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.listBoxSeries);
            this.groupBox1.Controls.Add(this.labelErrorSubject);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBoxCropValuesFilename);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.buttonBrowseSubject);
            this.groupBox1.Controls.Add(this.label19);
            this.groupBox1.Controls.Add(this.textBoxSubjectDirectory);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(746, 370);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Texture";
            // 
            // labelErrorKinematicFile
            // 
            this.labelErrorKinematicFile.AutoSize = true;
            this.labelErrorKinematicFile.ForeColor = System.Drawing.Color.Red;
            this.labelErrorKinematicFile.Location = new System.Drawing.Point(332, 294);
            this.labelErrorKinematicFile.Name = "labelErrorKinematicFile";
            this.labelErrorKinematicFile.Size = new System.Drawing.Size(78, 13);
            this.labelErrorKinematicFile.TabIndex = 23;
            this.labelErrorKinematicFile.Text = "Error Kinematic";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 294);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(75, 13);
            this.label6.TabIndex = 22;
            this.label6.Text = "Kinematic File:";
            // 
            // textBoxKinematicFilename
            // 
            this.textBoxKinematicFilename.Location = new System.Drawing.Point(111, 291);
            this.textBoxKinematicFilename.Name = "textBoxKinematicFilename";
            this.textBoxKinematicFilename.ReadOnly = true;
            this.textBoxKinematicFilename.Size = new System.Drawing.Size(215, 20);
            this.textBoxKinematicFilename.TabIndex = 21;
            this.textBoxKinematicFilename.TextChanged += new System.EventHandler(this.textBoxKinematicFilename_TextChanged);
            // 
            // radioButtonKinematicMotion
            // 
            this.radioButtonKinematicMotion.AutoSize = true;
            this.radioButtonKinematicMotion.Location = new System.Drawing.Point(290, 268);
            this.radioButtonKinematicMotion.Name = "radioButtonKinematicMotion";
            this.radioButtonKinematicMotion.Size = new System.Drawing.Size(76, 17);
            this.radioButtonKinematicMotion.TabIndex = 20;
            this.radioButtonKinematicMotion.Text = "Motion File";
            this.radioButtonKinematicMotion.UseVisualStyleBackColor = true;
            this.radioButtonKinematicMotion.CheckedChanged += new System.EventHandler(this.radioButtonKinematics_CheckedChanged);
            // 
            // radioButtonKinematicRT
            // 
            this.radioButtonKinematicRT.AutoSize = true;
            this.radioButtonKinematicRT.Location = new System.Drawing.Point(197, 268);
            this.radioButtonKinematicRT.Name = "radioButtonKinematicRT";
            this.radioButtonKinematicRT.Size = new System.Drawing.Size(87, 17);
            this.radioButtonKinematicRT.TabIndex = 19;
            this.radioButtonKinematicRT.Text = "Collected RT";
            this.radioButtonKinematicRT.UseVisualStyleBackColor = true;
            this.radioButtonKinematicRT.CheckedChanged += new System.EventHandler(this.radioButtonKinematics_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 270);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(102, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "Kinematic File Type:";
            // 
            // radioButtonKinematicAutoRegistr
            // 
            this.radioButtonKinematicAutoRegistr.AutoSize = true;
            this.radioButtonKinematicAutoRegistr.Checked = true;
            this.radioButtonKinematicAutoRegistr.Location = new System.Drawing.Point(111, 268);
            this.radioButtonKinematicAutoRegistr.Name = "radioButtonKinematicAutoRegistr";
            this.radioButtonKinematicAutoRegistr.Size = new System.Drawing.Size(80, 17);
            this.radioButtonKinematicAutoRegistr.TabIndex = 17;
            this.radioButtonKinematicAutoRegistr.TabStop = true;
            this.radioButtonKinematicAutoRegistr.Text = "auto_registr";
            this.radioButtonKinematicAutoRegistr.UseVisualStyleBackColor = true;
            this.radioButtonKinematicAutoRegistr.CheckedChanged += new System.EventHandler(this.radioButtonKinematics_CheckedChanged);
            // 
            // labelErrorCropValues
            // 
            this.labelErrorCropValues.AutoSize = true;
            this.labelErrorCropValues.ForeColor = System.Drawing.Color.Red;
            this.labelErrorCropValues.Location = new System.Drawing.Point(332, 79);
            this.labelErrorCropValues.Name = "labelErrorCropValues";
            this.labelErrorCropValues.Size = new System.Drawing.Size(86, 13);
            this.labelErrorCropValues.TabIndex = 16;
            this.labelErrorCropValues.Text = "Error CropValues";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Series List:";
            // 
            // listBoxSeries
            // 
            this.listBoxSeries.FormattingEnabled = true;
            this.listBoxSeries.Location = new System.Drawing.Point(111, 102);
            this.listBoxSeries.Name = "listBoxSeries";
            this.listBoxSeries.Size = new System.Drawing.Size(103, 160);
            this.listBoxSeries.TabIndex = 13;
            this.listBoxSeries.SelectedIndexChanged += new System.EventHandler(this.listBoxSeries_SelectedIndexChanged);
            // 
            // labelErrorSubject
            // 
            this.labelErrorSubject.AutoSize = true;
            this.labelErrorSubject.ForeColor = System.Drawing.Color.Red;
            this.labelErrorSubject.Location = new System.Drawing.Point(438, 30);
            this.labelErrorSubject.Name = "labelErrorSubject";
            this.labelErrorSubject.Size = new System.Drawing.Size(68, 13);
            this.labelErrorSubject.TabIndex = 12;
            this.labelErrorSubject.Text = "Error Subject";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Crop Values File:";
            // 
            // textBoxCropValuesFilename
            // 
            this.textBoxCropValuesFilename.Location = new System.Drawing.Point(111, 76);
            this.textBoxCropValuesFilename.Name = "textBoxCropValuesFilename";
            this.textBoxCropValuesFilename.ReadOnly = true;
            this.textBoxCropValuesFilename.Size = new System.Drawing.Size(215, 20);
            this.textBoxCropValuesFilename.TabIndex = 10;
            this.textBoxCropValuesFilename.TextChanged += new System.EventHandler(this.textBoxCropValuesFilename_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Mode:";
            // 
            // buttonBrowseSubject
            // 
            this.buttonBrowseSubject.Location = new System.Drawing.Point(337, 26);
            this.buttonBrowseSubject.Name = "buttonBrowseSubject";
            this.buttonBrowseSubject.Size = new System.Drawing.Size(95, 21);
            this.buttonBrowseSubject.TabIndex = 6;
            this.buttonBrowseSubject.Text = "Browse";
            this.buttonBrowseSubject.UseVisualStyleBackColor = true;
            this.buttonBrowseSubject.Click += new System.EventHandler(this.buttonBrowseImage_Click);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(6, 30);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(91, 13);
            this.label19.TabIndex = 5;
            this.label19.Text = "Subject Directory:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.radioButtonManual);
            this.panel1.Controls.Add(this.radioButtonAutomatic);
            this.panel1.Location = new System.Drawing.Point(101, 41);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(270, 31);
            this.panel1.TabIndex = 24;
            // 
            // radioButtonManual
            // 
            this.radioButtonManual.AutoSize = true;
            this.radioButtonManual.Location = new System.Drawing.Point(89, 10);
            this.radioButtonManual.Name = "radioButtonManual";
            this.radioButtonManual.Size = new System.Drawing.Size(60, 17);
            this.radioButtonManual.TabIndex = 9;
            this.radioButtonManual.Text = "Manual";
            this.radioButtonManual.UseVisualStyleBackColor = true;
            // 
            // radioButtonAutomatic
            // 
            this.radioButtonAutomatic.AutoSize = true;
            this.radioButtonAutomatic.Checked = true;
            this.radioButtonAutomatic.Location = new System.Drawing.Point(11, 10);
            this.radioButtonAutomatic.Name = "radioButtonAutomatic";
            this.radioButtonAutomatic.Size = new System.Drawing.Size(72, 17);
            this.radioButtonAutomatic.TabIndex = 7;
            this.radioButtonAutomatic.TabStop = true;
            this.radioButtonAutomatic.Text = "Automatic";
            this.radioButtonAutomatic.UseVisualStyleBackColor = true;
            this.radioButtonAutomatic.CheckedChanged += new System.EventHandler(this.radioButtonMode_CheckedChanged);
            // 
            // LoadTextureDialog
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(787, 447);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoadTextureDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Load Texture";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.TextBox textBoxSubjectDirectory;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonBrowseSubject;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioButtonAutomatic;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxCropValuesFilename;
        private System.Windows.Forms.RadioButton radioButtonManual;
        private System.Windows.Forms.Label labelErrorSubject;
        private System.Windows.Forms.ListBox listBoxSeries;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelErrorCropValues;
        private System.Windows.Forms.RadioButton radioButtonKinematicRT;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton radioButtonKinematicAutoRegistr;
        private System.Windows.Forms.RadioButton radioButtonKinematicMotion;
        private System.Windows.Forms.Label labelErrorKinematicFile;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxKinematicFilename;
        private System.Windows.Forms.Panel panel1;
    }
}