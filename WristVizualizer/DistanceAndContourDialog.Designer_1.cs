namespace WristVizualizer
{
    partial class DistanceAndContourDialog
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
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numericUpDownDistanceMapDist = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.radioButtonDistanceAll = new System.Windows.Forms.RadioButton();
            this.radioButtonDistanceHide = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.radioButtonContourAll = new System.Windows.Forms.RadioButton();
            this.radioButtonContourHide = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDistanceMapDist)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(273, 230);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(84, 28);
            this.buttonCancel.TabIndex = 9;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(183, 230);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(84, 28);
            this.buttonOK.TabIndex = 8;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.AutoSize = true;
            this.tableLayoutPanel.ColumnCount = 4;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.Controls.Add(this.numericUpDown1, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.checkBox1, 0, 0);
            this.tableLayoutPanel.Location = new System.Drawing.Point(206, 33);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 1;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(95, 26);
            this.tableLayoutPanel.TabIndex = 10;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.DecimalPlaces = 2;
            this.numericUpDown1.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDown1.Location = new System.Drawing.Point(24, 3);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(59, 20);
            this.numericUpDown1.TabIndex = 0;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(3, 3);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(15, 14);
            this.checkBox1.TabIndex = 11;
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.numericUpDownDistanceMapDist);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.radioButtonDistanceAll);
            this.groupBox1.Controls.Add(this.radioButtonDistanceHide);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(347, 76);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Distance Map";
            // 
            // numericUpDownDistanceMapDist
            // 
            this.numericUpDownDistanceMapDist.DecimalPlaces = 2;
            this.numericUpDownDistanceMapDist.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDownDistanceMapDist.Location = new System.Drawing.Point(269, 19);
            this.numericUpDownDistanceMapDist.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownDistanceMapDist.Name = "numericUpDownDistanceMapDist";
            this.numericUpDownDistanceMapDist.Size = new System.Drawing.Size(53, 20);
            this.numericUpDownDistanceMapDist.TabIndex = 3;
            this.numericUpDownDistanceMapDist.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(323, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "mm";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(194, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Max Distance:";
            // 
            // radioButtonDistanceAll
            // 
            this.radioButtonDistanceAll.AutoSize = true;
            this.radioButtonDistanceAll.Location = new System.Drawing.Point(6, 19);
            this.radioButtonDistanceAll.Name = "radioButtonDistanceAll";
            this.radioButtonDistanceAll.Size = new System.Drawing.Size(69, 17);
            this.radioButtonDistanceAll.TabIndex = 2;
            this.radioButtonDistanceAll.Text = "Calculate";
            this.radioButtonDistanceAll.UseVisualStyleBackColor = true;
            // 
            // radioButtonDistanceHide
            // 
            this.radioButtonDistanceHide.AutoSize = true;
            this.radioButtonDistanceHide.Location = new System.Drawing.Point(6, 42);
            this.radioButtonDistanceHide.Name = "radioButtonDistanceHide";
            this.radioButtonDistanceHide.Size = new System.Drawing.Size(47, 17);
            this.radioButtonDistanceHide.TabIndex = 1;
            this.radioButtonDistanceHide.Text = "Hide";
            this.radioButtonDistanceHide.UseVisualStyleBackColor = true;
            this.radioButtonDistanceHide.CheckedChanged += new System.EventHandler(this.radioButtonDistanceHide_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.AutoSize = true;
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.radioButtonContourAll);
            this.groupBox2.Controls.Add(this.tableLayoutPanel);
            this.groupBox2.Controls.Add(this.radioButtonContourHide);
            this.groupBox2.Location = new System.Drawing.Point(12, 94);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(347, 124);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Contours";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(202, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Visible Contours:";
            // 
            // radioButtonContourAll
            // 
            this.radioButtonContourAll.AutoSize = true;
            this.radioButtonContourAll.Location = new System.Drawing.Point(6, 25);
            this.radioButtonContourAll.Name = "radioButtonContourAll";
            this.radioButtonContourAll.Size = new System.Drawing.Size(69, 17);
            this.radioButtonContourAll.TabIndex = 8;
            this.radioButtonContourAll.Text = "Calculate";
            this.radioButtonContourAll.UseVisualStyleBackColor = true;
            // 
            // radioButtonContourHide
            // 
            this.radioButtonContourHide.AutoSize = true;
            this.radioButtonContourHide.Location = new System.Drawing.Point(6, 48);
            this.radioButtonContourHide.Name = "radioButtonContourHide";
            this.radioButtonContourHide.Size = new System.Drawing.Size(47, 17);
            this.radioButtonContourHide.TabIndex = 7;
            this.radioButtonContourHide.Text = "Hide";
            this.radioButtonContourHide.UseVisualStyleBackColor = true;
            this.radioButtonContourHide.CheckedChanged += new System.EventHandler(this.radioButtonContourHide_CheckedChanged);
            // 
            // DistanceAndContourDialog
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(369, 270);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "DistanceAndContourDialog";
            this.Text = "DistanceAndContourDialog";
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDistanceMapDist)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDownDistanceMapDist;
        private System.Windows.Forms.RadioButton radioButtonDistanceAll;
        private System.Windows.Forms.RadioButton radioButtonDistanceHide;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButtonContourAll;
        private System.Windows.Forms.RadioButton radioButtonContourHide;
        private System.Windows.Forms.Label label3;
    }
}