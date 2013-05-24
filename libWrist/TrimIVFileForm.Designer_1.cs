namespace libWrist
{
    partial class TrimIVFileForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrimIVFileForm));
            this.checkBoxMaterial = new System.Windows.Forms.CheckBox();
            this.checkBoxCamera = new System.Windows.Forms.CheckBox();
            this.textBoxFilename = new System.Windows.Forms.TextBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonTrim = new System.Windows.Forms.Button();
            this.panelDropFiles = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.radioButtonTrim = new System.Windows.Forms.RadioButton();
            this.radioButtonConvert = new System.Windows.Forms.RadioButton();
            this.checkBoxMimics10 = new System.Windows.Forms.CheckBox();
            this.panelDropFiles.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxMaterial
            // 
            this.checkBoxMaterial.AutoSize = true;
            this.checkBoxMaterial.Checked = true;
            this.checkBoxMaterial.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxMaterial.Location = new System.Drawing.Point(41, 66);
            this.checkBoxMaterial.Name = "checkBoxMaterial";
            this.checkBoxMaterial.Size = new System.Drawing.Size(86, 17);
            this.checkBoxMaterial.TabIndex = 0;
            this.checkBoxMaterial.Text = "Trim Material";
            this.checkBoxMaterial.UseVisualStyleBackColor = true;
            // 
            // checkBoxCamera
            // 
            this.checkBoxCamera.AutoSize = true;
            this.checkBoxCamera.Checked = true;
            this.checkBoxCamera.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCamera.Location = new System.Drawing.Point(41, 89);
            this.checkBoxCamera.Name = "checkBoxCamera";
            this.checkBoxCamera.Size = new System.Drawing.Size(85, 17);
            this.checkBoxCamera.TabIndex = 1;
            this.checkBoxCamera.Text = "Trim Camera";
            this.checkBoxCamera.UseVisualStyleBackColor = true;
            // 
            // textBoxFilename
            // 
            this.textBoxFilename.Location = new System.Drawing.Point(54, 17);
            this.textBoxFilename.Name = "textBoxFilename";
            this.textBoxFilename.Size = new System.Drawing.Size(224, 20);
            this.textBoxFilename.TabIndex = 2;
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Location = new System.Drawing.Point(284, 12);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(78, 28);
            this.buttonBrowse.TabIndex = 3;
            this.buttonBrowse.Text = "Browse";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "IV File";
            // 
            // buttonTrim
            // 
            this.buttonTrim.Location = new System.Drawing.Point(284, 126);
            this.buttonTrim.Name = "buttonTrim";
            this.buttonTrim.Size = new System.Drawing.Size(78, 28);
            this.buttonTrim.TabIndex = 5;
            this.buttonTrim.Text = "Trim";
            this.buttonTrim.UseVisualStyleBackColor = true;
            this.buttonTrim.Click += new System.EventHandler(this.buttonTrim_Click);
            // 
            // panelDropFiles
            // 
            this.panelDropFiles.AllowDrop = true;
            this.panelDropFiles.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelDropFiles.Controls.Add(this.label2);
            this.panelDropFiles.Location = new System.Drawing.Point(15, 160);
            this.panelDropFiles.Name = "panelDropFiles";
            this.panelDropFiles.Size = new System.Drawing.Size(347, 110);
            this.panelDropFiles.TabIndex = 6;
            this.panelDropFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.panelDropFiles_DragDrop);
            this.panelDropFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.panelDropFiles_DragEnter);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.label2.Location = new System.Drawing.Point(88, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(173, 74);
            this.label2.TabIndex = 0;
            this.label2.Text = "Drag Files\r\nHere";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(15, 276);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(347, 21);
            this.progressBar1.TabIndex = 7;
            this.progressBar1.Visible = false;
            // 
            // radioButtonTrim
            // 
            this.radioButtonTrim.AutoSize = true;
            this.radioButtonTrim.Checked = true;
            this.radioButtonTrim.Location = new System.Drawing.Point(15, 43);
            this.radioButtonTrim.Name = "radioButtonTrim";
            this.radioButtonTrim.Size = new System.Drawing.Size(133, 17);
            this.radioButtonTrim.TabIndex = 8;
            this.radioButtonTrim.TabStop = true;
            this.radioButtonTrim.Text = "Trim Material && Camera";
            this.radioButtonTrim.UseVisualStyleBackColor = true;
            this.radioButtonTrim.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonConvert
            // 
            this.radioButtonConvert.AutoSize = true;
            this.radioButtonConvert.Location = new System.Drawing.Point(15, 112);
            this.radioButtonConvert.Name = "radioButtonConvert";
            this.radioButtonConvert.Size = new System.Drawing.Size(124, 17);
            this.radioButtonConvert.TabIndex = 9;
            this.radioButtonConvert.Text = "Convert VRML To IV";
            this.radioButtonConvert.UseVisualStyleBackColor = true;
            this.radioButtonConvert.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // checkBoxMimics10
            // 
            this.checkBoxMimics10.AutoSize = true;
            this.checkBoxMimics10.Checked = true;
            this.checkBoxMimics10.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxMimics10.Enabled = false;
            this.checkBoxMimics10.Location = new System.Drawing.Point(41, 135);
            this.checkBoxMimics10.Name = "checkBoxMimics10";
            this.checkBoxMimics10.Size = new System.Drawing.Size(199, 17);
            this.checkBoxMimics10.TabIndex = 10;
            this.checkBoxMimics10.Text = "Detect Mimics (>=v10) meters output";
            this.checkBoxMimics10.UseVisualStyleBackColor = true;
            // 
            // TrimIVFileForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 310);
            this.Controls.Add(this.checkBoxMimics10);
            this.Controls.Add(this.radioButtonConvert);
            this.Controls.Add(this.radioButtonTrim);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.panelDropFiles);
            this.Controls.Add(this.buttonTrim);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.textBoxFilename);
            this.Controls.Add(this.checkBoxCamera);
            this.Controls.Add(this.checkBoxMaterial);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TrimIVFileForm";
            this.Text = "TrimIVFileForm";
            this.panelDropFiles.ResumeLayout(false);
            this.panelDropFiles.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxMaterial;
        private System.Windows.Forms.CheckBox checkBoxCamera;
        private System.Windows.Forms.TextBox textBoxFilename;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonTrim;
        private System.Windows.Forms.Panel panelDropFiles;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.RadioButton radioButtonTrim;
        private System.Windows.Forms.RadioButton radioButtonConvert;
        private System.Windows.Forms.CheckBox checkBoxMimics10;
    }
}