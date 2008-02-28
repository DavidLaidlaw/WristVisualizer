namespace WristVizualizer
{
    partial class ScenegraphTreeViewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScenegraphTreeViewer));
            this.treeViewScene = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // treeViewScene
            // 
            this.treeViewScene.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewScene.Location = new System.Drawing.Point(0, 0);
            this.treeViewScene.Name = "treeViewScene";
            this.treeViewScene.Size = new System.Drawing.Size(354, 415);
            this.treeViewScene.TabIndex = 0;
            // 
            // ScenegraphTreeViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(354, 415);
            this.Controls.Add(this.treeViewScene);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ScenegraphTreeViewer";
            this.Text = "ScenegraphTreeViewer";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeViewScene;
    }
}