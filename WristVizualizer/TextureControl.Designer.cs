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
            this.SuspendLayout();
            // 
            // listBoxTransforms
            // 
            this.listBoxTransforms.FormattingEnabled = true;
            this.listBoxTransforms.Location = new System.Drawing.Point(30, 2);
            this.listBoxTransforms.Name = "listBoxTransforms";
            this.listBoxTransforms.Size = new System.Drawing.Size(166, 342);
            this.listBoxTransforms.TabIndex = 1;
            this.listBoxTransforms.SelectedIndexChanged += new System.EventHandler(this.listBoxTransforms_SelectedIndexChanged);
            // 
            // TextureControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listBoxTransforms);
            this.Name = "TextureControl";
            this.Size = new System.Drawing.Size(230, 359);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxTransforms;

    }
}
