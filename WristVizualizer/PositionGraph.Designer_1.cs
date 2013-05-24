namespace WristVizualizer
{
    partial class PositionGraph
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
            this.pictureBoxGraph = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxFE = new System.Windows.Forms.TextBox();
            this.textBoxRU = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxPS = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGraph)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxGraph
            // 
            this.pictureBoxGraph.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxGraph.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxGraph.Name = "pictureBoxGraph";
            this.pictureBoxGraph.Size = new System.Drawing.Size(230, 247);
            this.pictureBoxGraph.TabIndex = 0;
            this.pictureBoxGraph.TabStop = false;
            this.pictureBoxGraph.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBoxGraph_MouseClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 256);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "FE:";
            // 
            // textBoxFE
            // 
            this.textBoxFE.Location = new System.Drawing.Point(33, 253);
            this.textBoxFE.Name = "textBoxFE";
            this.textBoxFE.ReadOnly = true;
            this.textBoxFE.Size = new System.Drawing.Size(44, 20);
            this.textBoxFE.TabIndex = 2;
            // 
            // textBoxRU
            // 
            this.textBoxRU.Location = new System.Drawing.Point(111, 253);
            this.textBoxRU.Name = "textBoxRU";
            this.textBoxRU.ReadOnly = true;
            this.textBoxRU.Size = new System.Drawing.Size(44, 20);
            this.textBoxRU.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(82, 256);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "RU:";
            // 
            // textBoxPS
            // 
            this.textBoxPS.Location = new System.Drawing.Point(186, 253);
            this.textBoxPS.Name = "textBoxPS";
            this.textBoxPS.ReadOnly = true;
            this.textBoxPS.Size = new System.Drawing.Size(44, 20);
            this.textBoxPS.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(157, 256);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(24, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "PS:";
            // 
            // PositionGraph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBoxPS);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxRU);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxFE);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBoxGraph);
            this.Name = "PositionGraph";
            this.Size = new System.Drawing.Size(230, 279);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGraph)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxGraph;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxFE;
        private System.Windows.Forms.TextBox textBoxRU;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxPS;
        private System.Windows.Forms.Label label3;
    }
}
