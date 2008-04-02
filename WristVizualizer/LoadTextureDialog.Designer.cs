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
            this.buttonBrowseSubject = new System.Windows.Forms.Button();
            this.label19 = new System.Windows.Forms.Label();
            this.radioButtonAutomatic = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.radioButtonManual = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxCropValuesFilename = new System.Windows.Forms.TextBox();
            this.labelErrorSubject = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
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
            this.textBoxSubjectDirectory.Location = new System.Drawing.Point(99, 27);
            this.textBoxSubjectDirectory.Name = "textBoxSubjectDirectory";
            this.textBoxSubjectDirectory.Size = new System.Drawing.Size(215, 20);
            this.textBoxSubjectDirectory.TabIndex = 5;
            this.textBoxSubjectDirectory.TextChanged += new System.EventHandler(this.textBoxSubjectDirectory_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelErrorSubject);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBoxCropValuesFilename);
            this.groupBox1.Controls.Add(this.radioButtonManual);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.radioButtonAutomatic);
            this.groupBox1.Controls.Add(this.buttonBrowseSubject);
            this.groupBox1.Controls.Add(this.label19);
            this.groupBox1.Controls.Add(this.textBoxSubjectDirectory);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(746, 370);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Texture";
            // 
            // buttonBrowseSubject
            // 
            this.buttonBrowseSubject.Location = new System.Drawing.Point(325, 26);
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
            // radioButtonAutomatic
            // 
            this.radioButtonAutomatic.AutoSize = true;
            this.radioButtonAutomatic.Checked = true;
            this.radioButtonAutomatic.Location = new System.Drawing.Point(99, 53);
            this.radioButtonAutomatic.Name = "radioButtonAutomatic";
            this.radioButtonAutomatic.Size = new System.Drawing.Size(72, 17);
            this.radioButtonAutomatic.TabIndex = 7;
            this.radioButtonAutomatic.TabStop = true;
            this.radioButtonAutomatic.Text = "Automatic";
            this.radioButtonAutomatic.UseVisualStyleBackColor = true;
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
            // radioButtonManual
            // 
            this.radioButtonManual.AutoSize = true;
            this.radioButtonManual.Location = new System.Drawing.Point(177, 53);
            this.radioButtonManual.Name = "radioButtonManual";
            this.radioButtonManual.Size = new System.Drawing.Size(60, 17);
            this.radioButtonManual.TabIndex = 9;
            this.radioButtonManual.Text = "Manual";
            this.radioButtonManual.UseVisualStyleBackColor = true;
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
            this.textBoxCropValuesFilename.Location = new System.Drawing.Point(99, 76);
            this.textBoxCropValuesFilename.Name = "textBoxCropValuesFilename";
            this.textBoxCropValuesFilename.ReadOnly = true;
            this.textBoxCropValuesFilename.Size = new System.Drawing.Size(215, 20);
            this.textBoxCropValuesFilename.TabIndex = 10;
            // 
            // labelErrorSubject
            // 
            this.labelErrorSubject.AutoSize = true;
            this.labelErrorSubject.ForeColor = System.Drawing.Color.Red;
            this.labelErrorSubject.Location = new System.Drawing.Point(426, 30);
            this.labelErrorSubject.Name = "labelErrorSubject";
            this.labelErrorSubject.Size = new System.Drawing.Size(0, 13);
            this.labelErrorSubject.TabIndex = 12;
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
    }
}