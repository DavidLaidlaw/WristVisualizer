namespace WristVizualizer
{
    partial class AnimationCreatorForm
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
            this.listBoxAllPositions = new System.Windows.Forms.ListBox();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.listBoxAnimationSequence = new System.Windows.Forms.ListBox();
            this.buttonMoveUp = new System.Windows.Forms.Button();
            this.buttonMoveDown = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownSteps = new System.Windows.Forms.NumericUpDown();
            this.checkBoxDistanceMap = new System.Windows.Forms.CheckBox();
            this.numericUpDownDistanceMapDist = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSteps)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDistanceMapDist)).BeginInit();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // listBoxAllPositions
            // 
            this.listBoxAllPositions.FormattingEnabled = true;
            this.listBoxAllPositions.Location = new System.Drawing.Point(6, 19);
            this.listBoxAllPositions.Name = "listBoxAllPositions";
            this.listBoxAllPositions.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxAllPositions.Size = new System.Drawing.Size(143, 199);
            this.listBoxAllPositions.TabIndex = 0;
            this.listBoxAllPositions.SelectedIndexChanged += new System.EventHandler(this.listBoxAllPositions_SelectedIndexChanged);
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(174, 80);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(60, 25);
            this.buttonAdd.TabIndex = 1;
            this.buttonAdd.Text = "-->";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonRemove
            // 
            this.buttonRemove.Location = new System.Drawing.Point(174, 111);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(60, 25);
            this.buttonRemove.TabIndex = 2;
            this.buttonRemove.Text = "<--";
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // listBoxAnimationSequence
            // 
            this.listBoxAnimationSequence.FormattingEnabled = true;
            this.listBoxAnimationSequence.Location = new System.Drawing.Point(260, 19);
            this.listBoxAnimationSequence.Name = "listBoxAnimationSequence";
            this.listBoxAnimationSequence.Size = new System.Drawing.Size(143, 199);
            this.listBoxAnimationSequence.TabIndex = 3;
            this.listBoxAnimationSequence.SelectedIndexChanged += new System.EventHandler(this.listBoxAnimationSequence_SelectedIndexChanged);
            // 
            // buttonMoveUp
            // 
            this.buttonMoveUp.Location = new System.Drawing.Point(409, 80);
            this.buttonMoveUp.Name = "buttonMoveUp";
            this.buttonMoveUp.Size = new System.Drawing.Size(60, 25);
            this.buttonMoveUp.TabIndex = 4;
            this.buttonMoveUp.Text = "Up";
            this.buttonMoveUp.UseVisualStyleBackColor = true;
            this.buttonMoveUp.Click += new System.EventHandler(this.buttonMoveUp_Click);
            // 
            // buttonMoveDown
            // 
            this.buttonMoveDown.Location = new System.Drawing.Point(409, 111);
            this.buttonMoveDown.Name = "buttonMoveDown";
            this.buttonMoveDown.Size = new System.Drawing.Size(60, 25);
            this.buttonMoveDown.TabIndex = 5;
            this.buttonMoveDown.Text = "Down";
            this.buttonMoveDown.UseVisualStyleBackColor = true;
            this.buttonMoveDown.Click += new System.EventHandler(this.buttonMoveDown_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(421, 450);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(84, 28);
            this.buttonCancel.TabIndex = 11;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(331, 450);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(84, 28);
            this.buttonOK.TabIndex = 10;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listBoxAllPositions);
            this.groupBox1.Controls.Add(this.buttonAdd);
            this.groupBox1.Controls.Add(this.buttonRemove);
            this.groupBox1.Controls.Add(this.buttonMoveDown);
            this.groupBox1.Controls.Add(this.listBoxAnimationSequence);
            this.groupBox1.Controls.Add(this.buttonMoveUp);
            this.groupBox1.Location = new System.Drawing.Point(10, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(484, 234);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Positions and Order";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.tableLayoutPanel);
            this.groupBox2.Controls.Add(this.numericUpDownDistanceMapDist);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.checkBoxDistanceMap);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.numericUpDownSteps);
            this.groupBox2.Location = new System.Drawing.Point(10, 258);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(485, 186);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Animation Options";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(169, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Number of Frames/Animation Step";
            // 
            // numericUpDownSteps
            // 
            this.numericUpDownSteps.Location = new System.Drawing.Point(181, 16);
            this.numericUpDownSteps.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownSteps.Name = "numericUpDownSteps";
            this.numericUpDownSteps.Size = new System.Drawing.Size(53, 20);
            this.numericUpDownSteps.TabIndex = 0;
            this.numericUpDownSteps.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // checkBoxDistanceMap
            // 
            this.checkBoxDistanceMap.AutoSize = true;
            this.checkBoxDistanceMap.Location = new System.Drawing.Point(9, 43);
            this.checkBoxDistanceMap.Name = "checkBoxDistanceMap";
            this.checkBoxDistanceMap.Size = new System.Drawing.Size(97, 17);
            this.checkBoxDistanceMap.TabIndex = 3;
            this.checkBoxDistanceMap.Text = "Distance Maps";
            this.checkBoxDistanceMap.UseVisualStyleBackColor = true;
            this.checkBoxDistanceMap.CheckedChanged += new System.EventHandler(this.checkBoxDistanceMap_CheckedChanged);
            // 
            // numericUpDownDistanceMapDist
            // 
            this.numericUpDownDistanceMapDist.DecimalPlaces = 2;
            this.numericUpDownDistanceMapDist.Enabled = false;
            this.numericUpDownDistanceMapDist.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDownDistanceMapDist.Location = new System.Drawing.Point(189, 42);
            this.numericUpDownDistanceMapDist.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownDistanceMapDist.Name = "numericUpDownDistanceMapDist";
            this.numericUpDownDistanceMapDist.Size = new System.Drawing.Size(53, 20);
            this.numericUpDownDistanceMapDist.TabIndex = 5;
            this.numericUpDownDistanceMapDist.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(114, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Max Distance:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(307, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Visible Contours:";
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
            this.tableLayoutPanel.Location = new System.Drawing.Point(311, 35);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 1;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(95, 26);
            this.tableLayoutPanel.TabIndex = 12;
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
            // AnimationCreatorForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(517, 490);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AnimationCreatorForm";
            this.Text = "Wrist Vizualizer";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSteps)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDistanceMapDist)).EndInit();
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxAllPositions;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.ListBox listBoxAnimationSequence;
        private System.Windows.Forms.Button buttonMoveUp;
        private System.Windows.Forms.Button buttonMoveDown;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDownSteps;
        private System.Windows.Forms.CheckBox checkBoxDistanceMap;
        private System.Windows.Forms.NumericUpDown numericUpDownDistanceMapDist;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}