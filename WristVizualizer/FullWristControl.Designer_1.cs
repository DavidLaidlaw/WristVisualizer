namespace WristVizualizer
{
    partial class FullWristControl
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.linkLabelHideAll = new System.Windows.Forms.LinkLabel();
            this.seriesListBox = new System.Windows.Forms.ListBox();
            this.linkLabelShowAll = new System.Windows.Forms.LinkLabel();
            this.label18 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.labelSeries = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.linkLabelHideAll, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.seriesListBox, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.linkLabelShowAll, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label18, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label16, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelSeries, 3, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(157, 138);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // linkLabelHideAll
            // 
            this.linkLabelHideAll.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabelHideAll.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.linkLabelHideAll, 2);
            this.linkLabelHideAll.Location = new System.Drawing.Point(0, 60);
            this.linkLabelHideAll.Margin = new System.Windows.Forms.Padding(0);
            this.linkLabelHideAll.Name = "linkLabelHideAll";
            this.linkLabelHideAll.Size = new System.Drawing.Size(61, 78);
            this.linkLabelHideAll.TabIndex = 38;
            this.linkLabelHideAll.TabStop = true;
            this.linkLabelHideAll.Text = "hide all";
            this.linkLabelHideAll.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelHideAll_LinkClicked);
            // 
            // seriesListBox
            // 
            this.seriesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.seriesListBox.FormattingEnabled = true;
            this.seriesListBox.Location = new System.Drawing.Point(61, 20);
            this.seriesListBox.Margin = new System.Windows.Forms.Padding(0);
            this.seriesListBox.Name = "seriesListBox";
            this.seriesListBox.Size = new System.Drawing.Size(96, 17);
            this.seriesListBox.TabIndex = 34;
            this.seriesListBox.SelectedIndexChanged += new System.EventHandler(this.seriesListBox_SelectedIndexChanged);
            // 
            // linkLabelShowAll
            // 
            this.linkLabelShowAll.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabelShowAll.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.linkLabelShowAll, 2);
            this.linkLabelShowAll.Location = new System.Drawing.Point(0, 40);
            this.linkLabelShowAll.Margin = new System.Windows.Forms.Padding(0);
            this.linkLabelShowAll.Name = "linkLabelShowAll";
            this.linkLabelShowAll.Size = new System.Drawing.Size(61, 20);
            this.linkLabelShowAll.TabIndex = 37;
            this.linkLabelShowAll.TabStop = true;
            this.linkLabelShowAll.Text = "show all";
            this.linkLabelShowAll.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelShowAll_LinkClicked);
            // 
            // label18
            // 
            this.label18.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label18.Location = new System.Drawing.Point(29, 0);
            this.label18.Margin = new System.Windows.Forms.Padding(0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(32, 20);
            this.label18.TabIndex = 54;
            this.label18.Text = "Fixed";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label16
            // 
            this.label16.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label16.Location = new System.Drawing.Point(0, 0);
            this.label16.Margin = new System.Windows.Forms.Padding(0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(29, 20);
            this.label16.TabIndex = 53;
            this.label16.Text = "Hide";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelSeries
            // 
            this.labelSeries.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSeries.AutoSize = true;
            this.labelSeries.Location = new System.Drawing.Point(61, 0);
            this.labelSeries.Margin = new System.Windows.Forms.Padding(0);
            this.labelSeries.Name = "labelSeries";
            this.labelSeries.Size = new System.Drawing.Size(96, 20);
            this.labelSeries.TabIndex = 55;
            this.labelSeries.Text = "Series";
            this.labelSeries.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FullWristControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FullWristControl";
            this.Size = new System.Drawing.Size(230, 150);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ListBox seriesListBox;
        private System.Windows.Forms.Label labelSeries;
        private System.Windows.Forms.LinkLabel linkLabelHideAll;
        private System.Windows.Forms.LinkLabel linkLabelShowAll;
    }
}
