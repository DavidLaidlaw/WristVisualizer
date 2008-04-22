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
            this.textBoxXLow = new System.Windows.Forms.TextBox();
            this.textBoxXHigh = new System.Windows.Forms.TextBox();
            this.textBoxYHigh = new System.Windows.Forms.TextBox();
            this.textBoxZHigh = new System.Windows.Forms.TextBox();
            this.textBoxYLow = new System.Windows.Forms.TextBox();
            this.textBoxZLow = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
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
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.textBoxSlice);
            this.panel1.Controls.Add(this.labelMin);
            this.panel1.Controls.Add(this.labelMax);
            this.panel1.Controls.Add(this.trackBar1);
            this.panel1.Enabled = false;
            this.panel1.Location = new System.Drawing.Point(22, 61);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(833, 605);
            this.panel1.TabIndex = 3;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.textBoxXLow);
            this.groupBox2.Controls.Add(this.textBoxXHigh);
            this.groupBox2.Controls.Add(this.textBoxYHigh);
            this.groupBox2.Controls.Add(this.textBoxZHigh);
            this.groupBox2.Controls.Add(this.textBoxYLow);
            this.groupBox2.Controls.Add(this.textBoxZLow);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.textBoxZVoxel);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.textBoxYVoxel);
            this.groupBox2.Controls.Add(this.textBoxXVoxel);
            this.groupBox2.Location = new System.Drawing.Point(561, 38);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(254, 171);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Crop Data";
            // 
            // textBoxXLow
            // 
            this.textBoxXLow.Location = new System.Drawing.Point(41, 40);
            this.textBoxXLow.Name = "textBoxXLow";
            this.textBoxXLow.Size = new System.Drawing.Size(60, 20);
            this.textBoxXLow.TabIndex = 0;
            // 
            // textBoxXHigh
            // 
            this.textBoxXHigh.Location = new System.Drawing.Point(104, 40);
            this.textBoxXHigh.Name = "textBoxXHigh";
            this.textBoxXHigh.Size = new System.Drawing.Size(60, 20);
            this.textBoxXHigh.TabIndex = 1;
            // 
            // textBoxYHigh
            // 
            this.textBoxYHigh.Location = new System.Drawing.Point(104, 63);
            this.textBoxYHigh.Name = "textBoxYHigh";
            this.textBoxYHigh.Size = new System.Drawing.Size(60, 20);
            this.textBoxYHigh.TabIndex = 3;
            // 
            // textBoxZHigh
            // 
            this.textBoxZHigh.Location = new System.Drawing.Point(104, 86);
            this.textBoxZHigh.Name = "textBoxZHigh";
            this.textBoxZHigh.Size = new System.Drawing.Size(60, 20);
            this.textBoxZHigh.TabIndex = 5;
            // 
            // textBoxYLow
            // 
            this.textBoxYLow.Location = new System.Drawing.Point(41, 63);
            this.textBoxYLow.Name = "textBoxYLow";
            this.textBoxYLow.Size = new System.Drawing.Size(60, 20);
            this.textBoxYLow.TabIndex = 2;
            // 
            // textBoxZLow
            // 
            this.textBoxZLow.Location = new System.Drawing.Point(41, 86);
            this.textBoxZLow.Name = "textBoxZLow";
            this.textBoxZLow.Size = new System.Drawing.Size(60, 20);
            this.textBoxZLow.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(59, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Low";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(116, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "High";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "X";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(167, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 13);
            this.label6.TabIndex = 22;
            this.label6.Text = "Voxel Size";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(26, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Z";
            // 
            // textBoxZVoxel
            // 
            this.textBoxZVoxel.Location = new System.Drawing.Point(167, 86);
            this.textBoxZVoxel.Name = "textBoxZVoxel";
            this.textBoxZVoxel.ReadOnly = true;
            this.textBoxZVoxel.Size = new System.Drawing.Size(60, 20);
            this.textBoxZVoxel.TabIndex = 21;
            this.textBoxZVoxel.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(26, 67);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(14, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "Y";
            // 
            // textBoxYVoxel
            // 
            this.textBoxYVoxel.Location = new System.Drawing.Point(167, 63);
            this.textBoxYVoxel.Name = "textBoxYVoxel";
            this.textBoxYVoxel.ReadOnly = true;
            this.textBoxYVoxel.Size = new System.Drawing.Size(60, 20);
            this.textBoxYVoxel.TabIndex = 20;
            this.textBoxYVoxel.TabStop = false;
            // 
            // textBoxXVoxel
            // 
            this.textBoxXVoxel.Location = new System.Drawing.Point(167, 40);
            this.textBoxXVoxel.Name = "textBoxXVoxel";
            this.textBoxXVoxel.ReadOnly = true;
            this.textBoxXVoxel.Size = new System.Drawing.Size(60, 20);
            this.textBoxXVoxel.TabIndex = 19;
            this.textBoxXVoxel.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBox1.Location = new System.Drawing.Point(17, 13);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(515, 515);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // textBoxSlice
            // 
            this.textBoxSlice.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.textBoxSlice.Location = new System.Drawing.Point(277, 576);
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
            this.labelMin.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.labelMin.AutoSize = true;
            this.labelMin.Location = new System.Drawing.Point(78, 549);
            this.labelMin.Name = "labelMin";
            this.labelMin.Size = new System.Drawing.Size(13, 13);
            this.labelMin.TabIndex = 2;
            this.labelMin.Text = "0";
            // 
            // labelMax
            // 
            this.labelMax.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.labelMax.AutoSize = true;
            this.labelMax.Location = new System.Drawing.Point(527, 549);
            this.labelMax.Name = "labelMax";
            this.labelMax.Size = new System.Drawing.Size(13, 13);
            this.labelMax.TabIndex = 1;
            this.labelMax.Text = "0";
            // 
            // trackBar1
            // 
            this.trackBar1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.trackBar1.Location = new System.Drawing.Point(97, 534);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(419, 45);
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
            // MRIViewerMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1038, 734);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBoxXLow;
        private System.Windows.Forms.TextBox textBoxXHigh;
        private System.Windows.Forms.TextBox textBoxYHigh;
        private System.Windows.Forms.TextBox textBoxZHigh;
        private System.Windows.Forms.TextBox textBoxYLow;
        private System.Windows.Forms.TextBox textBoxZLow;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
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
    }
}