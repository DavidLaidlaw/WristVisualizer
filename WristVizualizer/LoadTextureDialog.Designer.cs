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
            this.label12 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.loadVolumeRender = new System.Windows.Forms.CheckBox();
            this.checkBoxEnableStepping = new System.Windows.Forms.CheckBox();
            this.labelErrorImageFile = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.textBoxImageFile = new System.Windows.Forms.TextBox();
            this.labelErrorSeries = new System.Windows.Forms.Label();
            this.labelErrorStackFileDir = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.textBoxStackFileDirectory = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.numericUpDownMaxZ = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.numericUpDownMinZ = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.numericUpDownMaxY = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.numericUpDownMinY = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDownMaxX = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownMinX = new System.Windows.Forms.NumericUpDown();
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
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMinZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMinY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMinX)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(964, 643);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(147, 45);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonOK.Location = new System.Drawing.Point(809, 643);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(147, 45);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // textBoxSubjectDirectory
            // 
            this.textBoxSubjectDirectory.Location = new System.Drawing.Point(166, 42);
            this.textBoxSubjectDirectory.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxSubjectDirectory.Name = "textBoxSubjectDirectory";
            this.textBoxSubjectDirectory.Size = new System.Drawing.Size(320, 26);
            this.textBoxSubjectDirectory.TabIndex = 0;
            this.textBoxSubjectDirectory.TextChanged += new System.EventHandler(this.textBoxSubjectDirectory_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.buttonCancel);
            this.groupBox1.Controls.Add(this.loadVolumeRender);
            this.groupBox1.Controls.Add(this.buttonOK);
            this.groupBox1.Controls.Add(this.checkBoxEnableStepping);
            this.groupBox1.Controls.Add(this.labelErrorImageFile);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.textBoxImageFile);
            this.groupBox1.Controls.Add(this.labelErrorSeries);
            this.groupBox1.Controls.Add(this.labelErrorStackFileDir);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.textBoxStackFileDirectory);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.numericUpDownMaxZ);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.numericUpDownMinZ);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.numericUpDownMaxY);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.numericUpDownMinY);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.numericUpDownMaxX);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.numericUpDownMinX);
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
            this.groupBox1.Location = new System.Drawing.Point(18, 18);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Size = new System.Drawing.Size(1119, 804);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Texture";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(702, 609);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(143, 20);
            this.label12.TabIndex = 47;
            this.label12.Text = "Gamma Correction";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.textBox1.Location = new System.Drawing.Point(628, 606);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(63, 26);
            this.textBox1.TabIndex = 46;
            this.textBox1.Text = "1.0";
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // loadVolumeRender
            // 
            this.loadVolumeRender.AutoSize = true;
            this.loadVolumeRender.Checked = true;
            this.loadVolumeRender.CheckState = System.Windows.Forms.CheckState.Checked;
            this.loadVolumeRender.Location = new System.Drawing.Point(168, 640);
            this.loadVolumeRender.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.loadVolumeRender.Name = "loadVolumeRender";
            this.loadVolumeRender.Size = new System.Drawing.Size(186, 24);
            this.loadVolumeRender.TabIndex = 45;
            this.loadVolumeRender.Text = "Load Volume Render";
            this.loadVolumeRender.UseVisualStyleBackColor = true;
            this.loadVolumeRender.CheckedChanged += new System.EventHandler(this.loadVolumeRender_CheckedChanged);
            // 
            // checkBoxEnableStepping
            // 
            this.checkBoxEnableStepping.AutoSize = true;
            this.checkBoxEnableStepping.Location = new System.Drawing.Point(166, 608);
            this.checkBoxEnableStepping.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkBoxEnableStepping.Name = "checkBoxEnableStepping";
            this.checkBoxEnableStepping.Size = new System.Drawing.Size(306, 24);
            this.checkBoxEnableStepping.TabIndex = 44;
            this.checkBoxEnableStepping.Text = "Enable Stepping Through Registration";
            this.checkBoxEnableStepping.UseVisualStyleBackColor = true;
            this.checkBoxEnableStepping.CheckedChanged += new System.EventHandler(this.checkBoxEnableStepping_CheckedChanged);
            // 
            // labelErrorImageFile
            // 
            this.labelErrorImageFile.AutoSize = true;
            this.labelErrorImageFile.ForeColor = System.Drawing.Color.Red;
            this.labelErrorImageFile.Location = new System.Drawing.Point(606, 417);
            this.labelErrorImageFile.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelErrorImageFile.Name = "labelErrorImageFile";
            this.labelErrorImageFile.Size = new System.Drawing.Size(118, 20);
            this.labelErrorImageFile.TabIndex = 43;
            this.labelErrorImageFile.Text = "Error ImageFile";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(9, 417);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(87, 20);
            this.label14.TabIndex = 42;
            this.label14.Text = "Image File:";
            // 
            // textBoxImageFile
            // 
            this.textBoxImageFile.Location = new System.Drawing.Point(166, 412);
            this.textBoxImageFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxImageFile.Name = "textBoxImageFile";
            this.textBoxImageFile.ReadOnly = true;
            this.textBoxImageFile.Size = new System.Drawing.Size(428, 26);
            this.textBoxImageFile.TabIndex = 4;
            this.textBoxImageFile.TextChanged += new System.EventHandler(this.textBoxImageFile_TextChanged);
            // 
            // labelErrorSeries
            // 
            this.labelErrorSeries.AutoSize = true;
            this.labelErrorSeries.ForeColor = System.Drawing.Color.Red;
            this.labelErrorSeries.Location = new System.Drawing.Point(330, 162);
            this.labelErrorSeries.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelErrorSeries.Name = "labelErrorSeries";
            this.labelErrorSeries.Size = new System.Drawing.Size(93, 20);
            this.labelErrorSeries.TabIndex = 40;
            this.labelErrorSeries.Text = "Error Series";
            // 
            // labelErrorStackFileDir
            // 
            this.labelErrorStackFileDir.AutoSize = true;
            this.labelErrorStackFileDir.ForeColor = System.Drawing.Color.Red;
            this.labelErrorStackFileDir.Location = new System.Drawing.Point(606, 457);
            this.labelErrorStackFileDir.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelErrorStackFileDir.Name = "labelErrorStackFileDir";
            this.labelErrorStackFileDir.Size = new System.Drawing.Size(138, 20);
            this.labelErrorStackFileDir.TabIndex = 39;
            this.labelErrorStackFileDir.Text = "Error StackFile Dir";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(9, 457);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(150, 20);
            this.label13.TabIndex = 38;
            this.label13.Text = "Stack File Directory:";
            // 
            // textBoxStackFileDirectory
            // 
            this.textBoxStackFileDirectory.Location = new System.Drawing.Point(166, 452);
            this.textBoxStackFileDirectory.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxStackFileDirectory.Name = "textBoxStackFileDirectory";
            this.textBoxStackFileDirectory.ReadOnly = true;
            this.textBoxStackFileDirectory.Size = new System.Drawing.Size(428, 26);
            this.textBoxStackFileDirectory.TabIndex = 5;
            this.textBoxStackFileDirectory.TextChanged += new System.EventHandler(this.textBoxStackFileDirectory_TextChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(840, 572);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(52, 20);
            this.label11.TabIndex = 36;
            this.label11.Text = "MaxZ:";
            // 
            // numericUpDownMaxZ
            // 
            this.numericUpDownMaxZ.Location = new System.Drawing.Point(902, 568);
            this.numericUpDownMaxZ.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericUpDownMaxZ.Maximum = new decimal(new int[] {
            511,
            0,
            0,
            0});
            this.numericUpDownMaxZ.Name = "numericUpDownMaxZ";
            this.numericUpDownMaxZ.Size = new System.Drawing.Size(63, 26);
            this.numericUpDownMaxZ.TabIndex = 15;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(706, 572);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(48, 20);
            this.label10.TabIndex = 34;
            this.label10.Text = "MinZ:";
            // 
            // numericUpDownMinZ
            // 
            this.numericUpDownMinZ.Location = new System.Drawing.Point(765, 568);
            this.numericUpDownMinZ.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericUpDownMinZ.Maximum = new decimal(new int[] {
            511,
            0,
            0,
            0});
            this.numericUpDownMinZ.Name = "numericUpDownMinZ";
            this.numericUpDownMinZ.Size = new System.Drawing.Size(63, 26);
            this.numericUpDownMinZ.TabIndex = 14;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(568, 572);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 20);
            this.label9.TabIndex = 32;
            this.label9.Text = "MaxY:";
            // 
            // numericUpDownMaxY
            // 
            this.numericUpDownMaxY.Location = new System.Drawing.Point(628, 568);
            this.numericUpDownMaxY.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericUpDownMaxY.Maximum = new decimal(new int[] {
            511,
            0,
            0,
            0});
            this.numericUpDownMaxY.Name = "numericUpDownMaxY";
            this.numericUpDownMaxY.Size = new System.Drawing.Size(63, 26);
            this.numericUpDownMaxY.TabIndex = 13;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(435, 572);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(49, 20);
            this.label8.TabIndex = 30;
            this.label8.Text = "MinY:";
            // 
            // numericUpDownMinY
            // 
            this.numericUpDownMinY.Location = new System.Drawing.Point(492, 568);
            this.numericUpDownMinY.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericUpDownMinY.Maximum = new decimal(new int[] {
            511,
            0,
            0,
            0});
            this.numericUpDownMinY.Name = "numericUpDownMinY";
            this.numericUpDownMinY.Size = new System.Drawing.Size(63, 26);
            this.numericUpDownMinY.TabIndex = 12;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(297, 572);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 20);
            this.label7.TabIndex = 28;
            this.label7.Text = "MaxX:";
            // 
            // numericUpDownMaxX
            // 
            this.numericUpDownMaxX.Location = new System.Drawing.Point(356, 568);
            this.numericUpDownMaxX.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericUpDownMaxX.Maximum = new decimal(new int[] {
            511,
            0,
            0,
            0});
            this.numericUpDownMaxX.Name = "numericUpDownMaxX";
            this.numericUpDownMaxX.Size = new System.Drawing.Size(63, 26);
            this.numericUpDownMaxX.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(164, 572);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 20);
            this.label5.TabIndex = 26;
            this.label5.Text = "MinX:";
            // 
            // numericUpDownMinX
            // 
            this.numericUpDownMinX.Location = new System.Drawing.Point(219, 568);
            this.numericUpDownMinX.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericUpDownMinX.Maximum = new decimal(new int[] {
            511,
            0,
            0,
            0});
            this.numericUpDownMinX.Name = "numericUpDownMinX";
            this.numericUpDownMinX.Size = new System.Drawing.Size(63, 26);
            this.numericUpDownMinX.TabIndex = 10;
            // 
            // labelErrorKinematicFile
            // 
            this.labelErrorKinematicFile.AutoSize = true;
            this.labelErrorKinematicFile.ForeColor = System.Drawing.Color.Red;
            this.labelErrorKinematicFile.Location = new System.Drawing.Point(606, 532);
            this.labelErrorKinematicFile.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelErrorKinematicFile.Name = "labelErrorKinematicFile";
            this.labelErrorKinematicFile.Size = new System.Drawing.Size(117, 20);
            this.labelErrorKinematicFile.TabIndex = 23;
            this.labelErrorKinematicFile.Text = "Error Kinematic";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 532);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(111, 20);
            this.label6.TabIndex = 22;
            this.label6.Text = "Kinematic File:";
            // 
            // textBoxKinematicFilename
            // 
            this.textBoxKinematicFilename.Location = new System.Drawing.Point(166, 528);
            this.textBoxKinematicFilename.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxKinematicFilename.Name = "textBoxKinematicFilename";
            this.textBoxKinematicFilename.ReadOnly = true;
            this.textBoxKinematicFilename.Size = new System.Drawing.Size(428, 26);
            this.textBoxKinematicFilename.TabIndex = 9;
            this.textBoxKinematicFilename.TextChanged += new System.EventHandler(this.textBoxKinematicFilename_TextChanged);
            // 
            // radioButtonKinematicMotion
            // 
            this.radioButtonKinematicMotion.AutoSize = true;
            this.radioButtonKinematicMotion.Location = new System.Drawing.Point(435, 492);
            this.radioButtonKinematicMotion.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.radioButtonKinematicMotion.Name = "radioButtonKinematicMotion";
            this.radioButtonKinematicMotion.Size = new System.Drawing.Size(111, 24);
            this.radioButtonKinematicMotion.TabIndex = 8;
            this.radioButtonKinematicMotion.Text = "Motion File";
            this.radioButtonKinematicMotion.UseVisualStyleBackColor = true;
            this.radioButtonKinematicMotion.CheckedChanged += new System.EventHandler(this.radioButtonKinematics_CheckedChanged);
            // 
            // radioButtonKinematicRT
            // 
            this.radioButtonKinematicRT.AutoSize = true;
            this.radioButtonKinematicRT.Location = new System.Drawing.Point(296, 492);
            this.radioButtonKinematicRT.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.radioButtonKinematicRT.Name = "radioButtonKinematicRT";
            this.radioButtonKinematicRT.Size = new System.Drawing.Size(125, 24);
            this.radioButtonKinematicRT.TabIndex = 7;
            this.radioButtonKinematicRT.Text = "Collected RT";
            this.radioButtonKinematicRT.UseVisualStyleBackColor = true;
            this.radioButtonKinematicRT.CheckedChanged += new System.EventHandler(this.radioButtonKinematics_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 495);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(149, 20);
            this.label4.TabIndex = 18;
            this.label4.Text = "Kinematic File Type:";
            // 
            // radioButtonKinematicAutoRegistr
            // 
            this.radioButtonKinematicAutoRegistr.AutoSize = true;
            this.radioButtonKinematicAutoRegistr.Checked = true;
            this.radioButtonKinematicAutoRegistr.Location = new System.Drawing.Point(166, 492);
            this.radioButtonKinematicAutoRegistr.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.radioButtonKinematicAutoRegistr.Name = "radioButtonKinematicAutoRegistr";
            this.radioButtonKinematicAutoRegistr.Size = new System.Drawing.Size(119, 24);
            this.radioButtonKinematicAutoRegistr.TabIndex = 6;
            this.radioButtonKinematicAutoRegistr.TabStop = true;
            this.radioButtonKinematicAutoRegistr.Text = "auto_registr";
            this.radioButtonKinematicAutoRegistr.UseVisualStyleBackColor = true;
            this.radioButtonKinematicAutoRegistr.CheckedChanged += new System.EventHandler(this.radioButtonKinematics_CheckedChanged);
            // 
            // labelErrorCropValues
            // 
            this.labelErrorCropValues.AutoSize = true;
            this.labelErrorCropValues.ForeColor = System.Drawing.Color.Red;
            this.labelErrorCropValues.Location = new System.Drawing.Point(606, 122);
            this.labelErrorCropValues.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelErrorCropValues.Name = "labelErrorCropValues";
            this.labelErrorCropValues.Size = new System.Drawing.Size(131, 20);
            this.labelErrorCropValues.TabIndex = 16;
            this.labelErrorCropValues.Text = "Error CropValues";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 162);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 20);
            this.label3.TabIndex = 15;
            this.label3.Text = "Series List:";
            // 
            // listBoxSeries
            // 
            this.listBoxSeries.FormattingEnabled = true;
            this.listBoxSeries.ItemHeight = 20;
            this.listBoxSeries.Location = new System.Drawing.Point(166, 157);
            this.listBoxSeries.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.listBoxSeries.Name = "listBoxSeries";
            this.listBoxSeries.Size = new System.Drawing.Size(152, 244);
            this.listBoxSeries.TabIndex = 3;
            this.listBoxSeries.SelectedIndexChanged += new System.EventHandler(this.listBoxSeries_SelectedIndexChanged);
            this.listBoxSeries.DoubleClick += new System.EventHandler(this.listBoxSeries_DoubleClick);
            // 
            // labelErrorSubject
            // 
            this.labelErrorSubject.AutoSize = true;
            this.labelErrorSubject.ForeColor = System.Drawing.Color.Red;
            this.labelErrorSubject.Location = new System.Drawing.Point(657, 46);
            this.labelErrorSubject.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelErrorSubject.Name = "labelErrorSubject";
            this.labelErrorSubject.Size = new System.Drawing.Size(102, 20);
            this.labelErrorSubject.TabIndex = 12;
            this.labelErrorSubject.Text = "Error Subject";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 122);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(129, 20);
            this.label2.TabIndex = 11;
            this.label2.Text = "Crop Values File:";
            // 
            // textBoxCropValuesFilename
            // 
            this.textBoxCropValuesFilename.Location = new System.Drawing.Point(166, 117);
            this.textBoxCropValuesFilename.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxCropValuesFilename.Name = "textBoxCropValuesFilename";
            this.textBoxCropValuesFilename.ReadOnly = true;
            this.textBoxCropValuesFilename.Size = new System.Drawing.Size(428, 26);
            this.textBoxCropValuesFilename.TabIndex = 2;
            this.textBoxCropValuesFilename.TextChanged += new System.EventHandler(this.textBoxCropValuesFilename_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 85);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 20);
            this.label1.TabIndex = 8;
            this.label1.Text = "Mode:";
            // 
            // buttonBrowseSubject
            // 
            this.buttonBrowseSubject.Location = new System.Drawing.Point(506, 40);
            this.buttonBrowseSubject.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonBrowseSubject.Name = "buttonBrowseSubject";
            this.buttonBrowseSubject.Size = new System.Drawing.Size(142, 32);
            this.buttonBrowseSubject.TabIndex = 1;
            this.buttonBrowseSubject.Text = "Browse";
            this.buttonBrowseSubject.UseVisualStyleBackColor = true;
            this.buttonBrowseSubject.Click += new System.EventHandler(this.buttonBrowseImage_Click);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(9, 46);
            this.label19.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(134, 20);
            this.label19.TabIndex = 5;
            this.label19.Text = "Subject Directory:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.radioButtonManual);
            this.panel1.Controls.Add(this.radioButtonAutomatic);
            this.panel1.Location = new System.Drawing.Point(152, 65);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(405, 48);
            this.panel1.TabIndex = 24;
            // 
            // radioButtonManual
            // 
            this.radioButtonManual.AutoSize = true;
            this.radioButtonManual.Location = new System.Drawing.Point(134, 15);
            this.radioButtonManual.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.radioButtonManual.Name = "radioButtonManual";
            this.radioButtonManual.Size = new System.Drawing.Size(86, 24);
            this.radioButtonManual.TabIndex = 1;
            this.radioButtonManual.Text = "Manual";
            this.radioButtonManual.UseVisualStyleBackColor = true;
            // 
            // radioButtonAutomatic
            // 
            this.radioButtonAutomatic.AutoSize = true;
            this.radioButtonAutomatic.Checked = true;
            this.radioButtonAutomatic.Location = new System.Drawing.Point(16, 15);
            this.radioButtonAutomatic.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.radioButtonAutomatic.Name = "radioButtonAutomatic";
            this.radioButtonAutomatic.Size = new System.Drawing.Size(106, 24);
            this.radioButtonAutomatic.TabIndex = 0;
            this.radioButtonAutomatic.TabStop = true;
            this.radioButtonAutomatic.Text = "Automatic";
            this.radioButtonAutomatic.UseVisualStyleBackColor = true;
            this.radioButtonAutomatic.CheckedChanged += new System.EventHandler(this.radioButtonMode_CheckedChanged);
            // 
            // LoadTextureDialog
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(1180, 1050);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoadTextureDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Load Texture";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMinZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMinY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMinX)).EndInit();
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
        private System.Windows.Forms.NumericUpDown numericUpDownMinX;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown numericUpDownMaxZ;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown numericUpDownMinZ;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown numericUpDownMaxY;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown numericUpDownMinY;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numericUpDownMaxX;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label labelErrorStackFileDir;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBoxStackFileDirectory;
        private System.Windows.Forms.Label labelErrorSeries;
        private System.Windows.Forms.Label labelErrorImageFile;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBoxImageFile;
        private System.Windows.Forms.CheckBox checkBoxEnableStepping;
        private System.Windows.Forms.CheckBox loadVolumeRender;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label12;
    }
}