namespace WristVizualizer
{
    partial class AnimationControl
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
            this.trackBarCurrentFrame = new System.Windows.Forms.TrackBar();
            this.label19 = new System.Windows.Forms.Label();
            this.numericUpDownFPS = new System.Windows.Forms.NumericUpDown();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonPlay = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarCurrentFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFPS)).BeginInit();
            this.SuspendLayout();
            // 
            // trackBarCurrentFrame
            // 
            this.trackBarCurrentFrame.Location = new System.Drawing.Point(3, 3);
            this.trackBarCurrentFrame.Maximum = 50;
            this.trackBarCurrentFrame.Name = "trackBarCurrentFrame";
            this.trackBarCurrentFrame.Size = new System.Drawing.Size(205, 45);
            this.trackBarCurrentFrame.TabIndex = 17;
            this.trackBarCurrentFrame.Scroll += new System.EventHandler(this.trackBarCurrentFrame_Scroll);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(173, 59);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(27, 13);
            this.label19.TabIndex = 16;
            this.label19.Text = "FPS";
            // 
            // numericUpDownFPS
            // 
            this.numericUpDownFPS.Location = new System.Drawing.Point(125, 56);
            this.numericUpDownFPS.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDownFPS.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numericUpDownFPS.Name = "numericUpDownFPS";
            this.numericUpDownFPS.Size = new System.Drawing.Size(41, 20);
            this.numericUpDownFPS.TabIndex = 15;
            this.numericUpDownFPS.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownFPS.ValueChanged += new System.EventHandler(this.numericUpDownFPS_ValueChanged);
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(64, 53);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(52, 23);
            this.buttonStop.TabIndex = 14;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonPlay
            // 
            this.buttonPlay.Location = new System.Drawing.Point(6, 53);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(52, 23);
            this.buttonPlay.TabIndex = 13;
            this.buttonPlay.Text = "Play";
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // AnimationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.trackBarCurrentFrame);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.numericUpDownFPS);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonPlay);
            this.Name = "AnimationControl";
            this.Size = new System.Drawing.Size(209, 87);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarCurrentFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFPS)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar trackBarCurrentFrame;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.NumericUpDown numericUpDownFPS;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonPlay;

    }
}
