namespace WristVizualizer
{
    partial class CameraEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CameraEditor));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.textBoxPosX = new System.Windows.Forms.TextBox();
            this.textBoxPosY = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxPosZ = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxOrientZ = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxOrientY = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxOrientX = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxOrientRadians = new System.Windows.Forms.TextBox();
            this.textBoxFocalDistance = new System.Windows.Forms.TextBox();
            this.buttonSaveClipboard = new System.Windows.Forms.Button();
            this.buttonLoadClipboard = new System.Windows.Forms.Button();
            this.textBoxNearDistance = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textBoxFarDistance = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.textBoxHeight = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Position:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 133);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Focal Distance:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(89, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(12, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "x";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 56);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Orientation";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(31, 81);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Axis:";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(261, 279);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(84, 28);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(171, 279);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(84, 28);
            this.buttonOK.TabIndex = 6;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // textBoxPosX
            // 
            this.textBoxPosX.Location = new System.Drawing.Point(107, 27);
            this.textBoxPosX.Name = "textBoxPosX";
            this.textBoxPosX.Size = new System.Drawing.Size(40, 20);
            this.textBoxPosX.TabIndex = 8;
            // 
            // textBoxPosY
            // 
            this.textBoxPosY.Location = new System.Drawing.Point(178, 27);
            this.textBoxPosY.Name = "textBoxPosY";
            this.textBoxPosY.Size = new System.Drawing.Size(40, 20);
            this.textBoxPosY.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(160, 30);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(12, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "y";
            // 
            // textBoxPosZ
            // 
            this.textBoxPosZ.Location = new System.Drawing.Point(249, 27);
            this.textBoxPosZ.Name = "textBoxPosZ";
            this.textBoxPosZ.Size = new System.Drawing.Size(40, 20);
            this.textBoxPosZ.TabIndex = 12;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(231, 30);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(12, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "z";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(31, 107);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(50, 13);
            this.label8.TabIndex = 13;
            this.label8.Text = "Rotation:";
            // 
            // textBoxOrientZ
            // 
            this.textBoxOrientZ.Location = new System.Drawing.Point(249, 78);
            this.textBoxOrientZ.Name = "textBoxOrientZ";
            this.textBoxOrientZ.Size = new System.Drawing.Size(40, 20);
            this.textBoxOrientZ.TabIndex = 19;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(231, 81);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(12, 13);
            this.label9.TabIndex = 18;
            this.label9.Text = "z";
            // 
            // textBoxOrientY
            // 
            this.textBoxOrientY.Location = new System.Drawing.Point(178, 78);
            this.textBoxOrientY.Name = "textBoxOrientY";
            this.textBoxOrientY.Size = new System.Drawing.Size(40, 20);
            this.textBoxOrientY.TabIndex = 17;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(160, 81);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(12, 13);
            this.label10.TabIndex = 16;
            this.label10.Text = "y";
            // 
            // textBoxOrientX
            // 
            this.textBoxOrientX.Location = new System.Drawing.Point(107, 78);
            this.textBoxOrientX.Name = "textBoxOrientX";
            this.textBoxOrientX.Size = new System.Drawing.Size(40, 20);
            this.textBoxOrientX.TabIndex = 15;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(89, 81);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(12, 13);
            this.label11.TabIndex = 14;
            this.label11.Text = "x";
            // 
            // textBoxOrientRadians
            // 
            this.textBoxOrientRadians.Location = new System.Drawing.Point(107, 104);
            this.textBoxOrientRadians.Name = "textBoxOrientRadians";
            this.textBoxOrientRadians.Size = new System.Drawing.Size(40, 20);
            this.textBoxOrientRadians.TabIndex = 20;
            // 
            // textBoxFocalDistance
            // 
            this.textBoxFocalDistance.Location = new System.Drawing.Point(107, 130);
            this.textBoxFocalDistance.Name = "textBoxFocalDistance";
            this.textBoxFocalDistance.Size = new System.Drawing.Size(40, 20);
            this.textBoxFocalDistance.TabIndex = 21;
            // 
            // buttonSaveClipboard
            // 
            this.buttonSaveClipboard.Location = new System.Drawing.Point(261, 178);
            this.buttonSaveClipboard.Name = "buttonSaveClipboard";
            this.buttonSaveClipboard.Size = new System.Drawing.Size(84, 38);
            this.buttonSaveClipboard.TabIndex = 22;
            this.buttonSaveClipboard.Text = "Save To Clipboard";
            this.buttonSaveClipboard.UseVisualStyleBackColor = true;
            this.buttonSaveClipboard.Click += new System.EventHandler(this.buttonSaveClipboard_Click);
            // 
            // buttonLoadClipboard
            // 
            this.buttonLoadClipboard.Location = new System.Drawing.Point(261, 222);
            this.buttonLoadClipboard.Name = "buttonLoadClipboard";
            this.buttonLoadClipboard.Size = new System.Drawing.Size(84, 38);
            this.buttonLoadClipboard.TabIndex = 23;
            this.buttonLoadClipboard.Text = "Load From Clipboard";
            this.buttonLoadClipboard.UseVisualStyleBackColor = true;
            this.buttonLoadClipboard.Click += new System.EventHandler(this.buttonLoadClipboard_Click);
            // 
            // textBoxNearDistance
            // 
            this.textBoxNearDistance.Location = new System.Drawing.Point(107, 156);
            this.textBoxNearDistance.Name = "textBoxNearDistance";
            this.textBoxNearDistance.Size = new System.Drawing.Size(40, 20);
            this.textBoxNearDistance.TabIndex = 25;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 159);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(78, 13);
            this.label12.TabIndex = 24;
            this.label12.Text = "Near Distance:";
            // 
            // textBoxFarDistance
            // 
            this.textBoxFarDistance.Location = new System.Drawing.Point(107, 182);
            this.textBoxFarDistance.Name = "textBoxFarDistance";
            this.textBoxFarDistance.Size = new System.Drawing.Size(40, 20);
            this.textBoxFarDistance.TabIndex = 27;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(12, 185);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(70, 13);
            this.label13.TabIndex = 26;
            this.label13.Text = "Far Distance:";
            // 
            // textBoxHeight
            // 
            this.textBoxHeight.Location = new System.Drawing.Point(107, 208);
            this.textBoxHeight.Name = "textBoxHeight";
            this.textBoxHeight.Size = new System.Drawing.Size(40, 20);
            this.textBoxHeight.TabIndex = 29;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(12, 211);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(41, 13);
            this.label14.TabIndex = 28;
            this.label14.Text = "Height:";
            // 
            // CameraEditor
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(357, 319);
            this.Controls.Add(this.textBoxHeight);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.textBoxFarDistance);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.textBoxNearDistance);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.buttonLoadClipboard);
            this.Controls.Add(this.buttonSaveClipboard);
            this.Controls.Add(this.textBoxFocalDistance);
            this.Controls.Add(this.textBoxOrientRadians);
            this.Controls.Add(this.textBoxOrientZ);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.textBoxOrientY);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.textBoxOrientX);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBoxPosZ);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBoxPosY);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBoxPosX);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CameraEditor";
            this.Text = "Camera Editor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.TextBox textBoxPosX;
        private System.Windows.Forms.TextBox textBoxPosY;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxPosZ;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxOrientZ;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxOrientY;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxOrientX;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBoxOrientRadians;
        private System.Windows.Forms.TextBox textBoxFocalDistance;
        private System.Windows.Forms.Button buttonSaveClipboard;
        private System.Windows.Forms.Button buttonLoadClipboard;
        private System.Windows.Forms.TextBox textBoxNearDistance;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBoxFarDistance;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBoxHeight;
        private System.Windows.Forms.Label label14;
    }
}