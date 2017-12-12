namespace libWrist.MRISubRegion
{
    partial class MRISubRegionMainForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxXLow = new System.Windows.Forms.TextBox();
            this.textBoxXHigh = new System.Windows.Forms.TextBox();
            this.textBoxYHigh = new System.Windows.Forms.TextBox();
            this.textBoxZHigh = new System.Windows.Forms.TextBox();
            this.buttonRadialStyloid = new System.Windows.Forms.Button();
            this.textBoxYLow = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxZLow = new System.Windows.Forms.TextBox();
            this.textBoxRadialStyloid = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxZVoxel = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxYVoxel = new System.Windows.Forms.TextBox();
            this.textBoxXVoxel = new System.Windows.Forms.TextBox();
            this.groupBoxSubject = new System.Windows.Forms.GroupBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.radioButtonRight = new System.Windows.Forms.RadioButton();
            this.radioButtonLeft = new System.Windows.Forms.RadioButton();
            this.textBoxSeries = new System.Windows.Forms.TextBox();
            this.textBoxSubject = new System.Windows.Forms.TextBox();
            this.buttonSaveAdvance = new System.Windows.Forms.Button();
            this.buttonHigh = new System.Windows.Forms.Button();
            this.buttonLow = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.textBoxSlice = new System.Windows.Forms.TextBox();
            this.labelMin = new System.Windows.Forms.Label();
            this.labelMax = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.textBoxMRIPath = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBoxSubject.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(875, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.loadToolStripMenuItem.Text = "&Load...";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(117, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBoxSubject);
            this.panel1.Controls.Add(this.buttonHigh);
            this.panel1.Controls.Add(this.buttonLow);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.textBoxSlice);
            this.panel1.Controls.Add(this.labelMin);
            this.panel1.Controls.Add(this.labelMax);
            this.panel1.Controls.Add(this.trackBar1);
            this.panel1.Enabled = false;
            this.panel1.Location = new System.Drawing.Point(30, 74);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(833, 605);
            this.panel1.TabIndex = 2;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.textBoxXLow);
            this.groupBox2.Controls.Add(this.textBoxXHigh);
            this.groupBox2.Controls.Add(this.textBoxYHigh);
            this.groupBox2.Controls.Add(this.textBoxZHigh);
            this.groupBox2.Controls.Add(this.buttonRadialStyloid);
            this.groupBox2.Controls.Add(this.textBoxYLow);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.textBoxZLow);
            this.groupBox2.Controls.Add(this.textBoxRadialStyloid);
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
            // buttonRadialStyloid
            // 
            this.buttonRadialStyloid.Location = new System.Drawing.Point(168, 126);
            this.buttonRadialStyloid.Name = "buttonRadialStyloid";
            this.buttonRadialStyloid.Size = new System.Drawing.Size(59, 23);
            this.buttonRadialStyloid.TabIndex = 7;
            this.buttonRadialStyloid.Text = "Set";
            this.buttonRadialStyloid.UseVisualStyleBackColor = true;
            this.buttonRadialStyloid.Click += new System.EventHandler(this.buttonRadialStyloid_Click);
            // 
            // textBoxYLow
            // 
            this.textBoxYLow.Location = new System.Drawing.Point(41, 63);
            this.textBoxYLow.Name = "textBoxYLow";
            this.textBoxYLow.Size = new System.Drawing.Size(60, 20);
            this.textBoxYLow.TabIndex = 2;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(27, 131);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(71, 13);
            this.label7.TabIndex = 26;
            this.label7.Text = "Radial Styloid";
            // 
            // textBoxZLow
            // 
            this.textBoxZLow.Location = new System.Drawing.Point(41, 86);
            this.textBoxZLow.Name = "textBoxZLow";
            this.textBoxZLow.Size = new System.Drawing.Size(60, 20);
            this.textBoxZLow.TabIndex = 4;
            // 
            // textBoxRadialStyloid
            // 
            this.textBoxRadialStyloid.Location = new System.Drawing.Point(104, 128);
            this.textBoxRadialStyloid.Name = "textBoxRadialStyloid";
            this.textBoxRadialStyloid.Size = new System.Drawing.Size(60, 20);
            this.textBoxRadialStyloid.TabIndex = 6;
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
            // groupBoxSubject
            // 
            this.groupBoxSubject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxSubject.Controls.Add(this.buttonSave);
            this.groupBoxSubject.Controls.Add(this.label9);
            this.groupBoxSubject.Controls.Add(this.label10);
            this.groupBoxSubject.Controls.Add(this.radioButtonRight);
            this.groupBoxSubject.Controls.Add(this.radioButtonLeft);
            this.groupBoxSubject.Controls.Add(this.textBoxSeries);
            this.groupBoxSubject.Controls.Add(this.textBoxSubject);
            this.groupBoxSubject.Controls.Add(this.buttonSaveAdvance);
            this.groupBoxSubject.Location = new System.Drawing.Point(561, 226);
            this.groupBoxSubject.Name = "groupBoxSubject";
            this.groupBoxSubject.Size = new System.Drawing.Size(254, 163);
            this.groupBoxSubject.TabIndex = 1;
            this.groupBoxSubject.TabStop = false;
            this.groupBoxSubject.Text = "Subject Info";
            this.groupBoxSubject.Visible = false;
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(83, 117);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(78, 35);
            this.buttonSave.TabIndex = 2;
            this.buttonSave.Text = "Save To crop_values";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(13, 95);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(39, 13);
            this.label9.TabIndex = 22;
            this.label9.Text = "Series:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(13, 72);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(46, 13);
            this.label10.TabIndex = 21;
            this.label10.Text = "Subject:";
            // 
            // radioButtonRight
            // 
            this.radioButtonRight.AutoSize = true;
            this.radioButtonRight.Checked = true;
            this.radioButtonRight.Location = new System.Drawing.Point(104, 31);
            this.radioButtonRight.Name = "radioButtonRight";
            this.radioButtonRight.Size = new System.Drawing.Size(79, 17);
            this.radioButtonRight.TabIndex = 1;
            this.radioButtonRight.TabStop = true;
            this.radioButtonRight.Text = "Right Hand";
            this.radioButtonRight.UseVisualStyleBackColor = true;
            // 
            // radioButtonLeft
            // 
            this.radioButtonLeft.AutoSize = true;
            this.radioButtonLeft.Location = new System.Drawing.Point(18, 31);
            this.radioButtonLeft.Name = "radioButtonLeft";
            this.radioButtonLeft.Size = new System.Drawing.Size(72, 17);
            this.radioButtonLeft.TabIndex = 0;
            this.radioButtonLeft.Text = "Left Hand";
            this.radioButtonLeft.UseVisualStyleBackColor = true;
            // 
            // textBoxSeries
            // 
            this.textBoxSeries.Location = new System.Drawing.Point(62, 91);
            this.textBoxSeries.Name = "textBoxSeries";
            this.textBoxSeries.ReadOnly = true;
            this.textBoxSeries.Size = new System.Drawing.Size(60, 20);
            this.textBoxSeries.TabIndex = 20;
            this.textBoxSeries.TabStop = false;
            // 
            // textBoxSubject
            // 
            this.textBoxSubject.Location = new System.Drawing.Point(62, 68);
            this.textBoxSubject.Name = "textBoxSubject";
            this.textBoxSubject.ReadOnly = true;
            this.textBoxSubject.Size = new System.Drawing.Size(60, 20);
            this.textBoxSubject.TabIndex = 19;
            this.textBoxSubject.TabStop = false;
            // 
            // buttonSaveAdvance
            // 
            this.buttonSaveAdvance.Location = new System.Drawing.Point(167, 118);
            this.buttonSaveAdvance.Name = "buttonSaveAdvance";
            this.buttonSaveAdvance.Size = new System.Drawing.Size(78, 35);
            this.buttonSaveAdvance.TabIndex = 23;
            this.buttonSaveAdvance.Text = "Save To cv && Advance";
            this.buttonSaveAdvance.UseVisualStyleBackColor = true;
            this.buttonSaveAdvance.Click += new System.EventHandler(this.buttonSaveAdvance_Click);
            // 
            // buttonHigh
            // 
            this.buttonHigh.Location = new System.Drawing.Point(546, 539);
            this.buttonHigh.Name = "buttonHigh";
            this.buttonHigh.Size = new System.Drawing.Size(59, 23);
            this.buttonHigh.TabIndex = 4;
            this.buttonHigh.Text = "Set High";
            this.buttonHigh.UseVisualStyleBackColor = true;
            this.buttonHigh.Click += new System.EventHandler(this.buttonHigh_Click);
            // 
            // buttonLow
            // 
            this.buttonLow.Location = new System.Drawing.Point(17, 539);
            this.buttonLow.Name = "buttonLow";
            this.buttonLow.Size = new System.Drawing.Size(59, 23);
            this.buttonLow.TabIndex = 2;
            this.buttonLow.Text = "Set Low";
            this.buttonLow.UseVisualStyleBackColor = true;
            this.buttonLow.Click += new System.EventHandler(this.buttonLow_Click);
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
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
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
            this.labelMax.TextAlign = System.Drawing.ContentAlignment.TopRight;
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
            // textBoxMRIPath
            // 
            this.textBoxMRIPath.AcceptsReturn = true;
            this.textBoxMRIPath.Location = new System.Drawing.Point(79, 38);
            this.textBoxMRIPath.Name = "textBoxMRIPath";
            this.textBoxMRIPath.Size = new System.Drawing.Size(452, 20);
            this.textBoxMRIPath.TabIndex = 0;
            this.textBoxMRIPath.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxMRIPath_KeyDown);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(19, 41);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(55, 13);
            this.label8.TabIndex = 4;
            this.label8.Text = "MRI Path:";
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Location = new System.Drawing.Point(537, 34);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(78, 26);
            this.buttonBrowse.TabIndex = 1;
            this.buttonBrowse.Text = "Browse";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // buttonLoad
            // 
            this.buttonLoad.Location = new System.Drawing.Point(621, 34);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(78, 26);
            this.buttonLoad.TabIndex = 2;
            this.buttonLoad.Text = "Load";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // MRISubRegionMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(875, 703);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBoxMRIPath);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MRISubRegionMainForm";
            this.Text = "MRI SubRegion";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBoxSubject.ResumeLayout(false);
            this.groupBoxSubject.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBoxSlice;
        private System.Windows.Forms.Label labelMin;
        private System.Windows.Forms.Label labelMax;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxZLow;
        private System.Windows.Forms.TextBox textBoxYLow;
        private System.Windows.Forms.TextBox textBoxXLow;
        private System.Windows.Forms.TextBox textBoxZHigh;
        private System.Windows.Forms.TextBox textBoxYHigh;
        private System.Windows.Forms.TextBox textBoxXHigh;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxZVoxel;
        private System.Windows.Forms.TextBox textBoxYVoxel;
        private System.Windows.Forms.TextBox textBoxXVoxel;
        private System.Windows.Forms.Button buttonHigh;
        private System.Windows.Forms.Button buttonLow;
        private System.Windows.Forms.Button buttonRadialStyloid;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxRadialStyloid;
        private System.Windows.Forms.RadioButton radioButtonRight;
        private System.Windows.Forms.RadioButton radioButtonLeft;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.TextBox textBoxMRIPath;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.GroupBox groupBoxSubject;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxSeries;
        private System.Windows.Forms.TextBox textBoxSubject;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonSaveAdvance;
    }
}

