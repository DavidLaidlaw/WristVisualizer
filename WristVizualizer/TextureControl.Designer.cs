namespace WristVizualizer
{
    partial class TextureControl
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
            this.listBoxTransforms = new System.Windows.Forms.ListBox();
            this.numericUpDownCenterX = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownCenterY = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownCenterZ = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownRotZ = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownRotY = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownRotX = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownTransZ = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownTransY = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownTransX = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonCopyToClipboard = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.numberOfSlices = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.OpacityBox = new System.Windows.Forms.NumericUpDown();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.showManipulator = new System.Windows.Forms.CheckBox();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.trackBar2 = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCenterX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCenterY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCenterZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRotZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRotY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRotX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTransZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTransY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTransX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numberOfSlices)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.OpacityBox)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
            this.SuspendLayout();
            // 
            // listBoxTransforms
            // 
            this.listBoxTransforms.FormattingEnabled = true;
            this.listBoxTransforms.ItemHeight = 20;
            this.listBoxTransforms.Location = new System.Drawing.Point(142, 5);
            this.listBoxTransforms.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.listBoxTransforms.Name = "listBoxTransforms";
            this.listBoxTransforms.Size = new System.Drawing.Size(247, 284);
            this.listBoxTransforms.TabIndex = 1;
            this.listBoxTransforms.SelectedIndexChanged += new System.EventHandler(this.listBoxTransforms_SelectedIndexChanged);
            // 
            // numericUpDownCenterX
            // 
            this.numericUpDownCenterX.DecimalPlaces = 2;
            this.numericUpDownCenterX.Enabled = false;
            this.numericUpDownCenterX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDownCenterX.Location = new System.Drawing.Point(101, 365);
            this.numericUpDownCenterX.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericUpDownCenterX.Maximum = new decimal(new int[] {
            350,
            0,
            0,
            0});
            this.numericUpDownCenterX.Minimum = new decimal(new int[] {
            350,
            0,
            0,
            -2147483648});
            this.numericUpDownCenterX.Name = "numericUpDownCenterX";
            this.numericUpDownCenterX.ReadOnly = true;
            this.numericUpDownCenterX.Size = new System.Drawing.Size(93, 26);
            this.numericUpDownCenterX.TabIndex = 2;
            this.numericUpDownCenterX.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // numericUpDownCenterY
            // 
            this.numericUpDownCenterY.DecimalPlaces = 2;
            this.numericUpDownCenterY.Enabled = false;
            this.numericUpDownCenterY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDownCenterY.Location = new System.Drawing.Point(223, 365);
            this.numericUpDownCenterY.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericUpDownCenterY.Maximum = new decimal(new int[] {
            350,
            0,
            0,
            0});
            this.numericUpDownCenterY.Minimum = new decimal(new int[] {
            350,
            0,
            0,
            -2147483648});
            this.numericUpDownCenterY.Name = "numericUpDownCenterY";
            this.numericUpDownCenterY.ReadOnly = true;
            this.numericUpDownCenterY.Size = new System.Drawing.Size(93, 26);
            this.numericUpDownCenterY.TabIndex = 3;
            this.numericUpDownCenterY.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // numericUpDownCenterZ
            // 
            this.numericUpDownCenterZ.DecimalPlaces = 2;
            this.numericUpDownCenterZ.Enabled = false;
            this.numericUpDownCenterZ.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.numericUpDownCenterZ.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDownCenterZ.Location = new System.Drawing.Point(345, 365);
            this.numericUpDownCenterZ.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericUpDownCenterZ.Maximum = new decimal(new int[] {
            350,
            0,
            0,
            0});
            this.numericUpDownCenterZ.Minimum = new decimal(new int[] {
            350,
            0,
            0,
            -2147483648});
            this.numericUpDownCenterZ.Name = "numericUpDownCenterZ";
            this.numericUpDownCenterZ.ReadOnly = true;
            this.numericUpDownCenterZ.Size = new System.Drawing.Size(93, 26);
            this.numericUpDownCenterZ.TabIndex = 4;
            this.numericUpDownCenterZ.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // numericUpDownRotZ
            // 
            this.numericUpDownRotZ.DecimalPlaces = 3;
            this.numericUpDownRotZ.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numericUpDownRotZ.Location = new System.Drawing.Point(345, 404);
            this.numericUpDownRotZ.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericUpDownRotZ.Maximum = new decimal(new int[] {
            31416,
            0,
            0,
            262144});
            this.numericUpDownRotZ.Minimum = new decimal(new int[] {
            31416,
            0,
            0,
            -2147221504});
            this.numericUpDownRotZ.Name = "numericUpDownRotZ";
            this.numericUpDownRotZ.Size = new System.Drawing.Size(93, 26);
            this.numericUpDownRotZ.TabIndex = 7;
            this.numericUpDownRotZ.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // numericUpDownRotY
            // 
            this.numericUpDownRotY.DecimalPlaces = 3;
            this.numericUpDownRotY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numericUpDownRotY.Location = new System.Drawing.Point(223, 404);
            this.numericUpDownRotY.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericUpDownRotY.Maximum = new decimal(new int[] {
            31416,
            0,
            0,
            262144});
            this.numericUpDownRotY.Minimum = new decimal(new int[] {
            31416,
            0,
            0,
            -2147221504});
            this.numericUpDownRotY.Name = "numericUpDownRotY";
            this.numericUpDownRotY.Size = new System.Drawing.Size(93, 26);
            this.numericUpDownRotY.TabIndex = 6;
            this.numericUpDownRotY.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // numericUpDownRotX
            // 
            this.numericUpDownRotX.DecimalPlaces = 3;
            this.numericUpDownRotX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numericUpDownRotX.Location = new System.Drawing.Point(101, 404);
            this.numericUpDownRotX.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericUpDownRotX.Maximum = new decimal(new int[] {
            31416,
            0,
            0,
            262144});
            this.numericUpDownRotX.Minimum = new decimal(new int[] {
            31416,
            0,
            0,
            -2147221504});
            this.numericUpDownRotX.Name = "numericUpDownRotX";
            this.numericUpDownRotX.Size = new System.Drawing.Size(93, 26);
            this.numericUpDownRotX.TabIndex = 5;
            this.numericUpDownRotX.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // numericUpDownTransZ
            // 
            this.numericUpDownTransZ.DecimalPlaces = 2;
            this.numericUpDownTransZ.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDownTransZ.Location = new System.Drawing.Point(345, 462);
            this.numericUpDownTransZ.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericUpDownTransZ.Maximum = new decimal(new int[] {
            350,
            0,
            0,
            0});
            this.numericUpDownTransZ.Minimum = new decimal(new int[] {
            350,
            0,
            0,
            -2147483648});
            this.numericUpDownTransZ.Name = "numericUpDownTransZ";
            this.numericUpDownTransZ.Size = new System.Drawing.Size(93, 26);
            this.numericUpDownTransZ.TabIndex = 10;
            this.numericUpDownTransZ.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // numericUpDownTransY
            // 
            this.numericUpDownTransY.DecimalPlaces = 2;
            this.numericUpDownTransY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDownTransY.Location = new System.Drawing.Point(223, 462);
            this.numericUpDownTransY.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericUpDownTransY.Maximum = new decimal(new int[] {
            350,
            0,
            0,
            0});
            this.numericUpDownTransY.Minimum = new decimal(new int[] {
            350,
            0,
            0,
            -2147483648});
            this.numericUpDownTransY.Name = "numericUpDownTransY";
            this.numericUpDownTransY.Size = new System.Drawing.Size(93, 26);
            this.numericUpDownTransY.TabIndex = 9;
            this.numericUpDownTransY.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // numericUpDownTransX
            // 
            this.numericUpDownTransX.DecimalPlaces = 2;
            this.numericUpDownTransX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDownTransX.Location = new System.Drawing.Point(101, 462);
            this.numericUpDownTransX.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericUpDownTransX.Maximum = new decimal(new int[] {
            350,
            0,
            0,
            0});
            this.numericUpDownTransX.Minimum = new decimal(new int[] {
            350,
            0,
            0,
            -2147483648});
            this.numericUpDownTransX.Name = "numericUpDownTransX";
            this.numericUpDownTransX.Size = new System.Drawing.Size(93, 26);
            this.numericUpDownTransX.TabIndex = 8;
            this.numericUpDownTransX.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(185, 440);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 20);
            this.label1.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(101, 337);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(200, 20);
            this.label2.TabIndex = 12;
            this.label2.Text = "Rotation Center && Radians";
            // 
            // buttonCopyToClipboard
            // 
            this.buttonCopyToClipboard.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCopyToClipboard.Location = new System.Drawing.Point(345, 334);
            this.buttonCopyToClipboard.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonCopyToClipboard.Name = "buttonCopyToClipboard";
            this.buttonCopyToClipboard.Size = new System.Drawing.Size(80, 28);
            this.buttonCopyToClipboard.TabIndex = 13;
            this.buttonCopyToClipboard.Text = "Copy";
            this.buttonCopyToClipboard.UseVisualStyleBackColor = true;
            this.buttonCopyToClipboard.Click += new System.EventHandler(this.buttonCopyToClipboard_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(3, 8);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(89, 24);
            this.checkBox1.TabIndex = 14;
            this.checkBox1.Text = "Volume";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(100, 437);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 20);
            this.label4.TabIndex = 18;
            this.label4.Text = "Translation";
            // 
            // numberOfSlices
            // 
            this.numberOfSlices.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numberOfSlices.Location = new System.Drawing.Point(94, 5);
            this.numberOfSlices.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numberOfSlices.Maximum = new decimal(new int[] {
            850,
            0,
            0,
            0});
            this.numberOfSlices.Name = "numberOfSlices";
            this.numberOfSlices.Size = new System.Drawing.Size(58, 26);
            this.numberOfSlices.TabIndex = 15;
            this.numberOfSlices.Value = new decimal(new int[] {
            210,
            0,
            0,
            0});
            this.numberOfSlices.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(158, 11);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 20);
            this.label3.TabIndex = 16;
            this.label3.Text = "Slices";
            // 
            // OpacityBox
            // 
            this.OpacityBox.DecimalPlaces = 2;
            this.OpacityBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.OpacityBox.Location = new System.Drawing.Point(218, 3);
            this.OpacityBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.OpacityBox.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.OpacityBox.Name = "OpacityBox";
            this.OpacityBox.Size = new System.Drawing.Size(69, 26);
            this.OpacityBox.TabIndex = 19;
            this.OpacityBox.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.OpacityBox.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged_1);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.checkBox1);
            this.panel1.Controls.Add(this.OpacityBox);
            this.panel1.Controls.Add(this.numberOfSlices);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Location = new System.Drawing.Point(97, 500);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(352, 40);
            this.panel1.TabIndex = 20;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(286, 9);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 20);
            this.label5.TabIndex = 20;
            this.label5.Text = "Opacity";
            // 
            // showManipulator
            // 
            this.showManipulator.AutoSize = true;
            this.showManipulator.Checked = true;
            this.showManipulator.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showManipulator.Location = new System.Drawing.Point(106, 302);
            this.showManipulator.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.showManipulator.Name = "showManipulator";
            this.showManipulator.Size = new System.Drawing.Size(159, 24);
            this.showManipulator.TabIndex = 21;
            this.showManipulator.Text = "show manipulator";
            this.showManipulator.UseVisualStyleBackColor = true;
            this.showManipulator.CheckedChanged += new System.EventHandler(this.showManipulator_CheckedChanged);
            // 
            // trackBar1
            // 
            this.trackBar1.CausesValidation = false;
            this.trackBar1.LargeChange = 1;
            this.trackBar1.Location = new System.Drawing.Point(87, 556);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(406, 69);
            this.trackBar1.TabIndex = 22;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar1.Scroll += new System.EventHandler(this.TrackBar1_Scroll);
            // 
            // trackBar2
            // 
            this.trackBar2.CausesValidation = false;
            this.trackBar2.Location = new System.Drawing.Point(12, 5);
            this.trackBar2.Name = "trackBar2";
            this.trackBar2.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar2.Size = new System.Drawing.Size(69, 588);
            this.trackBar2.TabIndex = 23;
            this.trackBar2.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar2.Scroll += new System.EventHandler(this.TrackBar2_Scroll);
            // 
            // TextureControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.trackBar2);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.showManipulator);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.buttonCopyToClipboard);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDownTransZ);
            this.Controls.Add(this.numericUpDownTransY);
            this.Controls.Add(this.numericUpDownTransX);
            this.Controls.Add(this.numericUpDownRotZ);
            this.Controls.Add(this.numericUpDownRotY);
            this.Controls.Add(this.numericUpDownRotX);
            this.Controls.Add(this.numericUpDownCenterZ);
            this.Controls.Add(this.numericUpDownCenterY);
            this.Controls.Add(this.numericUpDownCenterX);
            this.Controls.Add(this.listBoxTransforms);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "TextureControl";
            this.Size = new System.Drawing.Size(502, 627);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCenterX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCenterY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCenterZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRotZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRotY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRotX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTransZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTransY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTransX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numberOfSlices)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.OpacityBox)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxTransforms;
        private System.Windows.Forms.NumericUpDown numericUpDownCenterX;
        private System.Windows.Forms.NumericUpDown numericUpDownCenterY;
        private System.Windows.Forms.NumericUpDown numericUpDownCenterZ;
        private System.Windows.Forms.NumericUpDown numericUpDownRotZ;
        private System.Windows.Forms.NumericUpDown numericUpDownRotY;
        private System.Windows.Forms.NumericUpDown numericUpDownRotX;
        private System.Windows.Forms.NumericUpDown numericUpDownTransZ;
        private System.Windows.Forms.NumericUpDown numericUpDownTransY;
        private System.Windows.Forms.NumericUpDown numericUpDownTransX;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonCopyToClipboard;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numberOfSlices;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown OpacityBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox showManipulator;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.TrackBar trackBar2;
    }
}
