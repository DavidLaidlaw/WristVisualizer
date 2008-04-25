namespace WristVizualizer
{
    partial class PosViewControl
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
            this.checkBoxPosViewLabels = new System.Windows.Forms.CheckBox();
            this.checkBoxPosViewOverrideMaterial = new System.Windows.Forms.CheckBox();
            this.checkBoxPosViewShowAxes = new System.Windows.Forms.CheckBox();
            this.trackBarPosViewCurrentFrame = new System.Windows.Forms.TrackBar();
            this.label19 = new System.Windows.Forms.Label();
            this.numericUpDownPosViewFPS = new System.Windows.Forms.NumericUpDown();
            this.buttonPosViewStop = new System.Windows.Forms.Button();
            this.buttonPosViewPlay = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarPosViewCurrentFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPosViewFPS)).BeginInit();
            this.SuspendLayout();
            // 
            // checkBoxPosViewLabels
            // 
            this.checkBoxPosViewLabels.AutoSize = true;
            this.checkBoxPosViewLabels.Location = new System.Drawing.Point(91, 168);
            this.checkBoxPosViewLabels.Name = "checkBoxPosViewLabels";
            this.checkBoxPosViewLabels.Size = new System.Drawing.Size(127, 17);
            this.checkBoxPosViewLabels.TabIndex = 15;
            this.checkBoxPosViewLabels.Text = "Show Position Labels";
            this.checkBoxPosViewLabels.UseVisualStyleBackColor = true;
            this.checkBoxPosViewLabels.CheckedChanged += new System.EventHandler(this.checkBoxPosViewLabels_CheckedChanged);
            // 
            // checkBoxPosViewOverrideMaterial
            // 
            this.checkBoxPosViewOverrideMaterial.AutoSize = true;
            this.checkBoxPosViewOverrideMaterial.Location = new System.Drawing.Point(91, 145);
            this.checkBoxPosViewOverrideMaterial.Name = "checkBoxPosViewOverrideMaterial";
            this.checkBoxPosViewOverrideMaterial.Size = new System.Drawing.Size(132, 17);
            this.checkBoxPosViewOverrideMaterial.TabIndex = 14;
            this.checkBoxPosViewOverrideMaterial.Text = "Override bone material";
            this.checkBoxPosViewOverrideMaterial.UseVisualStyleBackColor = true;
            this.checkBoxPosViewOverrideMaterial.CheckedChanged += new System.EventHandler(this.checkBoxPosViewOverrideMaterial_CheckedChanged);
            // 
            // checkBoxPosViewShowAxes
            // 
            this.checkBoxPosViewShowAxes.AutoSize = true;
            this.checkBoxPosViewShowAxes.Location = new System.Drawing.Point(91, 122);
            this.checkBoxPosViewShowAxes.Name = "checkBoxPosViewShowAxes";
            this.checkBoxPosViewShowAxes.Size = new System.Drawing.Size(106, 17);
            this.checkBoxPosViewShowAxes.TabIndex = 13;
            this.checkBoxPosViewShowAxes.Text = "Show HAM Axes";
            this.checkBoxPosViewShowAxes.UseVisualStyleBackColor = true;
            this.checkBoxPosViewShowAxes.CheckedChanged += new System.EventHandler(this.checkBoxPosViewShowAxes_CheckedChanged);
            // 
            // trackBarPosViewCurrentFrame
            // 
            this.trackBarPosViewCurrentFrame.Location = new System.Drawing.Point(7, 34);
            this.trackBarPosViewCurrentFrame.Maximum = 50;
            this.trackBarPosViewCurrentFrame.Name = "trackBarPosViewCurrentFrame";
            this.trackBarPosViewCurrentFrame.Size = new System.Drawing.Size(205, 45);
            this.trackBarPosViewCurrentFrame.TabIndex = 12;
            this.trackBarPosViewCurrentFrame.Scroll += new System.EventHandler(this.trackBarPosViewCurrentFrame_Scroll);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(177, 90);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(27, 13);
            this.label19.TabIndex = 11;
            this.label19.Text = "FPS";
            // 
            // numericUpDownPosViewFPS
            // 
            this.numericUpDownPosViewFPS.Location = new System.Drawing.Point(129, 87);
            this.numericUpDownPosViewFPS.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDownPosViewFPS.Name = "numericUpDownPosViewFPS";
            this.numericUpDownPosViewFPS.Size = new System.Drawing.Size(41, 20);
            this.numericUpDownPosViewFPS.TabIndex = 10;
            this.numericUpDownPosViewFPS.ValueChanged += new System.EventHandler(this.numericUpDownPosViewFPS_ValueChanged);
            // 
            // buttonPosViewStop
            // 
            this.buttonPosViewStop.Location = new System.Drawing.Point(68, 84);
            this.buttonPosViewStop.Name = "buttonPosViewStop";
            this.buttonPosViewStop.Size = new System.Drawing.Size(52, 23);
            this.buttonPosViewStop.TabIndex = 9;
            this.buttonPosViewStop.Text = "Stop";
            this.buttonPosViewStop.UseVisualStyleBackColor = true;
            this.buttonPosViewStop.Click += new System.EventHandler(this.buttonPosViewStop_Click);
            // 
            // buttonPosViewPlay
            // 
            this.buttonPosViewPlay.Location = new System.Drawing.Point(10, 84);
            this.buttonPosViewPlay.Name = "buttonPosViewPlay";
            this.buttonPosViewPlay.Size = new System.Drawing.Size(52, 23);
            this.buttonPosViewPlay.TabIndex = 8;
            this.buttonPosViewPlay.Text = "Play";
            this.buttonPosViewPlay.UseVisualStyleBackColor = true;
            this.buttonPosViewPlay.Click += new System.EventHandler(this.buttonPosViewPlay_Click);
            // 
            // PosViewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkBoxPosViewLabels);
            this.Controls.Add(this.checkBoxPosViewOverrideMaterial);
            this.Controls.Add(this.checkBoxPosViewShowAxes);
            this.Controls.Add(this.trackBarPosViewCurrentFrame);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.numericUpDownPosViewFPS);
            this.Controls.Add(this.buttonPosViewStop);
            this.Controls.Add(this.buttonPosViewPlay);
            this.Name = "PosViewControl";
            this.Size = new System.Drawing.Size(230, 359);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarPosViewCurrentFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPosViewFPS)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxPosViewLabels;
        private System.Windows.Forms.CheckBox checkBoxPosViewOverrideMaterial;
        private System.Windows.Forms.CheckBox checkBoxPosViewShowAxes;
        private System.Windows.Forms.TrackBar trackBarPosViewCurrentFrame;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.NumericUpDown numericUpDownPosViewFPS;
        private System.Windows.Forms.Button buttonPosViewStop;
        private System.Windows.Forms.Button buttonPosViewPlay;
    }
}
