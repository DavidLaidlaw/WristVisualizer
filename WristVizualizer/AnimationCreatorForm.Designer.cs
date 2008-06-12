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
            this.SuspendLayout();
            // 
            // listBoxAllPositions
            // 
            this.listBoxAllPositions.FormattingEnabled = true;
            this.listBoxAllPositions.Location = new System.Drawing.Point(31, 30);
            this.listBoxAllPositions.Name = "listBoxAllPositions";
            this.listBoxAllPositions.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxAllPositions.Size = new System.Drawing.Size(143, 199);
            this.listBoxAllPositions.TabIndex = 0;
            this.listBoxAllPositions.SelectedIndexChanged += new System.EventHandler(this.listBoxAllPositions_SelectedIndexChanged);
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(202, 54);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(60, 25);
            this.buttonAdd.TabIndex = 1;
            this.buttonAdd.Text = "-->";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonRemove
            // 
            this.buttonRemove.Location = new System.Drawing.Point(202, 85);
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
            this.listBoxAnimationSequence.Location = new System.Drawing.Point(285, 30);
            this.listBoxAnimationSequence.Name = "listBoxAnimationSequence";
            this.listBoxAnimationSequence.Size = new System.Drawing.Size(143, 199);
            this.listBoxAnimationSequence.TabIndex = 3;
            this.listBoxAnimationSequence.SelectedIndexChanged += new System.EventHandler(this.listBoxAnimationSequence_SelectedIndexChanged);
            // 
            // buttonMoveUp
            // 
            this.buttonMoveUp.Location = new System.Drawing.Point(434, 72);
            this.buttonMoveUp.Name = "buttonMoveUp";
            this.buttonMoveUp.Size = new System.Drawing.Size(60, 25);
            this.buttonMoveUp.TabIndex = 4;
            this.buttonMoveUp.Text = "Up";
            this.buttonMoveUp.UseVisualStyleBackColor = true;
            this.buttonMoveUp.Click += new System.EventHandler(this.buttonMoveUp_Click);
            // 
            // buttonMoveDown
            // 
            this.buttonMoveDown.Location = new System.Drawing.Point(434, 103);
            this.buttonMoveDown.Name = "buttonMoveDown";
            this.buttonMoveDown.Size = new System.Drawing.Size(60, 25);
            this.buttonMoveDown.TabIndex = 5;
            this.buttonMoveDown.Text = "Down";
            this.buttonMoveDown.UseVisualStyleBackColor = true;
            this.buttonMoveDown.Click += new System.EventHandler(this.buttonMoveDown_Click);
            // 
            // AnimationCreatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(517, 283);
            this.Controls.Add(this.buttonMoveDown);
            this.Controls.Add(this.buttonMoveUp);
            this.Controls.Add(this.listBoxAnimationSequence);
            this.Controls.Add(this.buttonRemove);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.listBoxAllPositions);
            this.Name = "AnimationCreatorForm";
            this.Text = "AnimationCreatorForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxAllPositions;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.ListBox listBoxAnimationSequence;
        private System.Windows.Forms.Button buttonMoveUp;
        private System.Windows.Forms.Button buttonMoveDown;
    }
}