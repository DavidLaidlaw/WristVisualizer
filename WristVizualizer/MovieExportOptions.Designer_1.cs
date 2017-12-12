namespace WristVizualizer
{
    partial class MovieExportOptions
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label19 = new System.Windows.Forms.Label();
            this.numericUpDownFPS = new System.Windows.Forms.NumericUpDown();
            this.checkBoxCompression = new System.Windows.Forms.CheckBox();
            this.radioMovie = new System.Windows.Forms.RadioButton();
            this.radioImages = new System.Windows.Forms.RadioButton();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonNoSmoothing = new System.Windows.Forms.RadioButton();
            this.radioButton2xSmoothing = new System.Windows.Forms.RadioButton();
            this.radioButton3xSmoothing = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFPS)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label19);
            this.groupBox1.Controls.Add(this.numericUpDownFPS);
            this.groupBox1.Controls.Add(this.checkBoxCompression);
            this.groupBox1.Controls.Add(this.radioMovie);
            this.groupBox1.Controls.Add(this.radioImages);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(302, 129);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Saving Methods";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(80, 93);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(73, 13);
            this.label19.TabIndex = 4;
            this.label19.Text = "FPS for movie";
            // 
            // numericUpDownFPS
            // 
            this.numericUpDownFPS.Location = new System.Drawing.Point(33, 91);
            this.numericUpDownFPS.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDownFPS.Name = "numericUpDownFPS";
            this.numericUpDownFPS.Size = new System.Drawing.Size(41, 20);
            this.numericUpDownFPS.TabIndex = 3;
            // 
            // checkBoxCompression
            // 
            this.checkBoxCompression.AutoSize = true;
            this.checkBoxCompression.Location = new System.Drawing.Point(33, 68);
            this.checkBoxCompression.Name = "checkBoxCompression";
            this.checkBoxCompression.Size = new System.Drawing.Size(263, 17);
            this.checkBoxCompression.TabIndex = 2;
            this.checkBoxCompression.Text = "Use Compression (you will have to select a codec)";
            this.checkBoxCompression.UseVisualStyleBackColor = true;
            // 
            // radioMovie
            // 
            this.radioMovie.AutoSize = true;
            this.radioMovie.Location = new System.Drawing.Point(17, 45);
            this.radioMovie.Name = "radioMovie";
            this.radioMovie.Size = new System.Drawing.Size(100, 17);
            this.radioMovie.TabIndex = 1;
            this.radioMovie.Text = "Export as movie";
            this.radioMovie.UseVisualStyleBackColor = true;
            this.radioMovie.CheckedChanged += new System.EventHandler(this.radioMovie_CheckedChanged);
            // 
            // radioImages
            // 
            this.radioImages.AutoSize = true;
            this.radioImages.Checked = true;
            this.radioImages.Location = new System.Drawing.Point(17, 22);
            this.radioImages.Name = "radioImages";
            this.radioImages.Size = new System.Drawing.Size(208, 17);
            this.radioImages.TabIndex = 0;
            this.radioImages.TabStop = true;
            this.radioImages.Text = "Export frames as a collection of images";
            this.radioImages.UseVisualStyleBackColor = true;
            this.radioImages.CheckedChanged += new System.EventHandler(this.radioMovie_CheckedChanged);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(126, 278);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(98, 29);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(230, 278);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(98, 29);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButton3xSmoothing);
            this.groupBox2.Controls.Add(this.radioButton2xSmoothing);
            this.groupBox2.Controls.Add(this.radioButtonNoSmoothing);
            this.groupBox2.Location = new System.Drawing.Point(12, 147);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(302, 98);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Smoothing Options";
            // 
            // radioButtonNoSmoothing
            // 
            this.radioButtonNoSmoothing.AutoSize = true;
            this.radioButtonNoSmoothing.Location = new System.Drawing.Point(17, 19);
            this.radioButtonNoSmoothing.Name = "radioButtonNoSmoothing";
            this.radioButtonNoSmoothing.Size = new System.Drawing.Size(92, 17);
            this.radioButtonNoSmoothing.TabIndex = 0;
            this.radioButtonNoSmoothing.Text = "No Smoothing";
            this.radioButtonNoSmoothing.UseVisualStyleBackColor = true;
            // 
            // radioButton2xSmoothing
            // 
            this.radioButton2xSmoothing.AutoSize = true;
            this.radioButton2xSmoothing.Checked = true;
            this.radioButton2xSmoothing.Location = new System.Drawing.Point(17, 42);
            this.radioButton2xSmoothing.Name = "radioButton2xSmoothing";
            this.radioButton2xSmoothing.Size = new System.Drawing.Size(89, 17);
            this.radioButton2xSmoothing.TabIndex = 1;
            this.radioButton2xSmoothing.TabStop = true;
            this.radioButton2xSmoothing.Text = "2x Smoothing";
            this.radioButton2xSmoothing.UseVisualStyleBackColor = true;
            // 
            // radioButton3xSmoothing
            // 
            this.radioButton3xSmoothing.AutoSize = true;
            this.radioButton3xSmoothing.Location = new System.Drawing.Point(17, 65);
            this.radioButton3xSmoothing.Name = "radioButton3xSmoothing";
            this.radioButton3xSmoothing.Size = new System.Drawing.Size(89, 17);
            this.radioButton3xSmoothing.TabIndex = 2;
            this.radioButton3xSmoothing.Text = "3x Smoothing";
            this.radioButton3xSmoothing.UseVisualStyleBackColor = true;
            // 
            // MovieExportOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(340, 319);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MovieExportOptions";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Side";
            this.TopMost = true;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFPS)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioMovie;
        private System.Windows.Forms.RadioButton radioImages;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.CheckBox checkBoxCompression;
        private System.Windows.Forms.NumericUpDown numericUpDownFPS;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButton3xSmoothing;
        private System.Windows.Forms.RadioButton radioButton2xSmoothing;
        private System.Windows.Forms.RadioButton radioButtonNoSmoothing;
    }
}