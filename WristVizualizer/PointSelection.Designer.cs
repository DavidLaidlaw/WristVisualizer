namespace WristVizualizer
{
    partial class PointSelection
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
            this.checkBoxShowStatus = new System.Windows.Forms.CheckBox();
            this.checkBoxSaveToFile = new System.Windows.Forms.CheckBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.checkBoxOverwrite = new System.Windows.Forms.CheckBox();
            this.numericUpDownPrecision = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPrecision)).BeginInit();
            this.SuspendLayout();
            // 
            // checkBoxShowStatus
            // 
            this.checkBoxShowStatus.AutoSize = true;
            this.checkBoxShowStatus.Checked = true;
            this.checkBoxShowStatus.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowStatus.Location = new System.Drawing.Point(12, 19);
            this.checkBoxShowStatus.Name = "checkBoxShowStatus";
            this.checkBoxShowStatus.Size = new System.Drawing.Size(117, 17);
            this.checkBoxShowStatus.TabIndex = 0;
            this.checkBoxShowStatus.Text = "Show In Status Bar";
            this.checkBoxShowStatus.UseVisualStyleBackColor = true;
            this.checkBoxShowStatus.CheckedChanged += new System.EventHandler(this.checkBoxShowStatus_CheckedChanged);
            // 
            // checkBoxSaveToFile
            // 
            this.checkBoxSaveToFile.AutoSize = true;
            this.checkBoxSaveToFile.Checked = true;
            this.checkBoxSaveToFile.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSaveToFile.Location = new System.Drawing.Point(12, 42);
            this.checkBoxSaveToFile.Name = "checkBoxSaveToFile";
            this.checkBoxSaveToFile.Size = new System.Drawing.Size(86, 17);
            this.checkBoxSaveToFile.TabIndex = 1;
            this.checkBoxSaveToFile.Text = "Save To File";
            this.checkBoxSaveToFile.UseVisualStyleBackColor = true;
            this.checkBoxSaveToFile.CheckedChanged += new System.EventHandler(this.checkBoxSaveToFile_CheckedChanged);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(89, 92);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(84, 28);
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(179, 92);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(84, 28);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // checkBoxOverwrite
            // 
            this.checkBoxOverwrite.AutoSize = true;
            this.checkBoxOverwrite.Checked = true;
            this.checkBoxOverwrite.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxOverwrite.Location = new System.Drawing.Point(34, 65);
            this.checkBoxOverwrite.Name = "checkBoxOverwrite";
            this.checkBoxOverwrite.Size = new System.Drawing.Size(124, 17);
            this.checkBoxOverwrite.TabIndex = 4;
            this.checkBoxOverwrite.Text = "Overwrite if file exists";
            this.checkBoxOverwrite.UseVisualStyleBackColor = true;
            // 
            // numericUpDownPrecision
            // 
            this.numericUpDownPrecision.Location = new System.Drawing.Point(253, 16);
            this.numericUpDownPrecision.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.numericUpDownPrecision.Name = "numericUpDownPrecision";
            this.numericUpDownPrecision.Size = new System.Drawing.Size(36, 20);
            this.numericUpDownPrecision.TabIndex = 5;
            this.numericUpDownPrecision.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(160, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Display Precision";
            // 
            // PointSelection
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(301, 132);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDownPrecision);
            this.Controls.Add(this.checkBoxOverwrite);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.checkBoxSaveToFile);
            this.Controls.Add(this.checkBoxShowStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "PointSelection";
            this.Text = "Point Selection";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPrecision)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxShowStatus;
        private System.Windows.Forms.CheckBox checkBoxSaveToFile;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.CheckBox checkBoxOverwrite;
        private System.Windows.Forms.NumericUpDown numericUpDownPrecision;
        private System.Windows.Forms.Label label1;
    }
}