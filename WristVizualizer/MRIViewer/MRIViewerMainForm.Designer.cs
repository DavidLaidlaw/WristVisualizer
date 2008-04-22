namespace WristVizualizer.MRIViewer
{
    partial class MRIViewerMainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MRIViewerMainForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxXSize = new System.Windows.Forms.TextBox();
            this.textBoxYSize = new System.Windows.Forms.TextBox();
            this.textBoxZSize = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxZVoxel = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxYVoxel = new System.Windows.Forms.TextBox();
            this.textBoxXVoxel = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.textBoxSlice = new System.Windows.Forms.TextBox();
            this.labelMin = new System.Windows.Forms.Label();
            this.labelMax = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxMRIPath = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxX = new System.Windows.Forms.TextBox();
            this.textBoxY = new System.Windows.Forms.TextBox();
            this.textBoxZ = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxLayersSize = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textBoxIntensity = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.numericUpDownLayer = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.textBoxIntensityScaled = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.textBoxIntensitySigned = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLayer)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.textBoxSlice);
            this.panel1.Controls.Add(this.labelMin);
            this.panel1.Controls.Add(this.labelMax);
            this.panel1.Controls.Add(this.trackBar1);
            this.panel1.Enabled = false;
            this.panel1.Location = new System.Drawing.Point(3, 42);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(744, 605);
            this.panel1.TabIndex = 3;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.textBoxLayersSize);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.textBoxXSize);
            this.groupBox2.Controls.Add(this.textBoxYSize);
            this.groupBox2.Controls.Add(this.textBoxZSize);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.textBoxZVoxel);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.textBoxYVoxel);
            this.groupBox2.Controls.Add(this.textBoxXVoxel);
            this.groupBox2.Location = new System.Drawing.Point(544, 17);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(190, 151);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Volume Info";
            // 
            // textBoxXSize
            // 
            this.textBoxXSize.Location = new System.Drawing.Point(46, 40);
            this.textBoxXSize.Name = "textBoxXSize";
            this.textBoxXSize.ReadOnly = true;
            this.textBoxXSize.Size = new System.Drawing.Size(60, 20);
            this.textBoxXSize.TabIndex = 0;
            // 
            // textBoxYSize
            // 
            this.textBoxYSize.Location = new System.Drawing.Point(46, 63);
            this.textBoxYSize.Name = "textBoxYSize";
            this.textBoxYSize.ReadOnly = true;
            this.textBoxYSize.Size = new System.Drawing.Size(60, 20);
            this.textBoxYSize.TabIndex = 2;
            // 
            // textBoxZSize
            // 
            this.textBoxZSize.Location = new System.Drawing.Point(46, 86);
            this.textBoxZSize.Name = "textBoxZSize";
            this.textBoxZSize.ReadOnly = true;
            this.textBoxZSize.Size = new System.Drawing.Size(60, 20);
            this.textBoxZSize.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(45, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Dimensions";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(31, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "X";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(111, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 13);
            this.label6.TabIndex = 22;
            this.label6.Text = "Voxel Size";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(31, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Z";
            // 
            // textBoxZVoxel
            // 
            this.textBoxZVoxel.Location = new System.Drawing.Point(111, 86);
            this.textBoxZVoxel.Name = "textBoxZVoxel";
            this.textBoxZVoxel.ReadOnly = true;
            this.textBoxZVoxel.Size = new System.Drawing.Size(60, 20);
            this.textBoxZVoxel.TabIndex = 21;
            this.textBoxZVoxel.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(31, 67);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(14, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "Y";
            // 
            // textBoxYVoxel
            // 
            this.textBoxYVoxel.Location = new System.Drawing.Point(111, 63);
            this.textBoxYVoxel.Name = "textBoxYVoxel";
            this.textBoxYVoxel.ReadOnly = true;
            this.textBoxYVoxel.Size = new System.Drawing.Size(60, 20);
            this.textBoxYVoxel.TabIndex = 20;
            this.textBoxYVoxel.TabStop = false;
            // 
            // textBoxXVoxel
            // 
            this.textBoxXVoxel.Location = new System.Drawing.Point(111, 40);
            this.textBoxXVoxel.Name = "textBoxXVoxel";
            this.textBoxXVoxel.ReadOnly = true;
            this.textBoxXVoxel.Size = new System.Drawing.Size(60, 20);
            this.textBoxXVoxel.TabIndex = 19;
            this.textBoxXVoxel.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Location = new System.Drawing.Point(17, 13);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(515, 515);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseLeave += new System.EventHandler(this.pictureBox1_MouseLeave);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            // 
            // textBoxSlice
            // 
            this.textBoxSlice.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.textBoxSlice.Location = new System.Drawing.Point(350, 573);
            this.textBoxSlice.Name = "textBoxSlice";
            this.textBoxSlice.ReadOnly = true;
            this.textBoxSlice.Size = new System.Drawing.Size(50, 20);
            this.textBoxSlice.TabIndex = 3;
            this.textBoxSlice.TabStop = false;
            this.textBoxSlice.Text = "0";
            this.textBoxSlice.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // labelMin
            // 
            this.labelMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelMin.AutoSize = true;
            this.labelMin.Location = new System.Drawing.Point(14, 549);
            this.labelMin.Name = "labelMin";
            this.labelMin.Size = new System.Drawing.Size(13, 13);
            this.labelMin.TabIndex = 2;
            this.labelMin.Text = "0";
            // 
            // labelMax
            // 
            this.labelMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMax.AutoSize = true;
            this.labelMax.Location = new System.Drawing.Point(712, 549);
            this.labelMax.Name = "labelMax";
            this.labelMax.Size = new System.Drawing.Size(13, 13);
            this.labelMax.TabIndex = 1;
            this.labelMax.Text = "0";
            // 
            // trackBar1
            // 
            this.trackBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBar1.Location = new System.Drawing.Point(33, 534);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(673, 45);
            this.trackBar1.TabIndex = 3;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // buttonLoad
            // 
            this.buttonLoad.Location = new System.Drawing.Point(616, 8);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(78, 26);
            this.buttonLoad.TabIndex = 7;
            this.buttonLoad.Text = "Load";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Location = new System.Drawing.Point(532, 8);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(78, 26);
            this.buttonBrowse.TabIndex = 6;
            this.buttonBrowse.Text = "Browse";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(14, 15);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(55, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "MRI Path:";
            // 
            // textBoxMRIPath
            // 
            this.textBoxMRIPath.AcceptsReturn = true;
            this.textBoxMRIPath.Location = new System.Drawing.Point(74, 12);
            this.textBoxMRIPath.Name = "textBoxMRIPath";
            this.textBoxMRIPath.Size = new System.Drawing.Size(452, 20);
            this.textBoxMRIPath.TabIndex = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.textBoxIntensitySigned);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.textBoxIntensityScaled);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.numericUpDownLayer);
            this.groupBox1.Controls.Add(this.textBoxIntensity);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.textBoxX);
            this.groupBox1.Controls.Add(this.textBoxY);
            this.groupBox1.Controls.Add(this.textBoxZ);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Location = new System.Drawing.Point(544, 174);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(190, 243);
            this.groupBox1.TabIndex = 23;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Itensity";
            // 
            // textBoxX
            // 
            this.textBoxX.Location = new System.Drawing.Point(87, 45);
            this.textBoxX.Name = "textBoxX";
            this.textBoxX.ReadOnly = true;
            this.textBoxX.Size = new System.Drawing.Size(60, 20);
            this.textBoxX.TabIndex = 0;
            // 
            // textBoxY
            // 
            this.textBoxY.Location = new System.Drawing.Point(87, 68);
            this.textBoxY.Name = "textBoxY";
            this.textBoxY.ReadOnly = true;
            this.textBoxY.Size = new System.Drawing.Size(60, 20);
            this.textBoxY.TabIndex = 2;
            // 
            // textBoxZ
            // 
            this.textBoxZ.Location = new System.Drawing.Point(87, 91);
            this.textBoxZ.Name = "textBoxZ";
            this.textBoxZ.ReadOnly = true;
            this.textBoxZ.Size = new System.Drawing.Size(60, 20);
            this.textBoxZ.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(86, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Position";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(72, 49);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(14, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "X";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(72, 95);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(14, 13);
            this.label10.TabIndex = 17;
            this.label10.Text = "Z";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(72, 72);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(14, 13);
            this.label11.TabIndex = 18;
            this.label11.Text = "Y";
            // 
            // textBoxLayersSize
            // 
            this.textBoxLayersSize.Location = new System.Drawing.Point(46, 109);
            this.textBoxLayersSize.Name = "textBoxLayersSize";
            this.textBoxLayersSize.ReadOnly = true;
            this.textBoxLayersSize.Size = new System.Drawing.Size(60, 20);
            this.textBoxLayersSize.TabIndex = 23;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(8, 113);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(38, 13);
            this.label12.TabIndex = 24;
            this.label12.Text = "Layers";
            // 
            // textBoxIntensity
            // 
            this.textBoxIntensity.Location = new System.Drawing.Point(87, 114);
            this.textBoxIntensity.Name = "textBoxIntensity";
            this.textBoxIntensity.ReadOnly = true;
            this.textBoxIntensity.Size = new System.Drawing.Size(60, 20);
            this.textBoxIntensity.TabIndex = 19;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(17, 118);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(71, 13);
            this.label9.TabIndex = 20;
            this.label9.Text = "Raw Intensity";
            // 
            // numericUpDownLayer
            // 
            this.numericUpDownLayer.Location = new System.Drawing.Point(100, 194);
            this.numericUpDownLayer.Name = "numericUpDownLayer";
            this.numericUpDownLayer.Size = new System.Drawing.Size(47, 20);
            this.numericUpDownLayer.TabIndex = 21;
            this.numericUpDownLayer.ValueChanged += new System.EventHandler(this.numericUpDownLayer_ValueChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(53, 197);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(33, 13);
            this.label13.TabIndex = 22;
            this.label13.Text = "Layer";
            // 
            // textBoxIntensityScaled
            // 
            this.textBoxIntensityScaled.Location = new System.Drawing.Point(87, 138);
            this.textBoxIntensityScaled.Name = "textBoxIntensityScaled";
            this.textBoxIntensityScaled.ReadOnly = true;
            this.textBoxIntensityScaled.Size = new System.Drawing.Size(60, 20);
            this.textBoxIntensityScaled.TabIndex = 23;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 142);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(82, 13);
            this.label14.TabIndex = 24;
            this.label14.Text = "Intensity Scaled";
            // 
            // textBoxIntensitySigned
            // 
            this.textBoxIntensitySigned.Location = new System.Drawing.Point(86, 162);
            this.textBoxIntensitySigned.Name = "textBoxIntensitySigned";
            this.textBoxIntensitySigned.ReadOnly = true;
            this.textBoxIntensitySigned.Size = new System.Drawing.Size(60, 20);
            this.textBoxIntensitySigned.TabIndex = 25;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(6, 166);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(82, 13);
            this.label15.TabIndex = 26;
            this.label15.Text = "Signed Intensity";
            // 
            // MRIViewerMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(749, 648);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBoxMRIPath);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MRIViewerMainForm";
            this.Text = "MRIViewer";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLayer)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBoxXSize;
        private System.Windows.Forms.TextBox textBoxYSize;
        private System.Windows.Forms.TextBox textBoxZSize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxZVoxel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxYVoxel;
        private System.Windows.Forms.TextBox textBoxXVoxel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox textBoxSlice;
        private System.Windows.Forms.Label labelMin;
        private System.Windows.Forms.Label labelMax;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxMRIPath;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxX;
        private System.Windows.Forms.TextBox textBoxY;
        private System.Windows.Forms.TextBox textBoxZ;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBoxLayersSize;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown numericUpDownLayer;
        private System.Windows.Forms.TextBox textBoxIntensity;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBoxIntensitySigned;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox textBoxIntensityScaled;
        private System.Windows.Forms.Label label14;
    }
}