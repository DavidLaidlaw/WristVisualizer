using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WristVizualizer
{
    public partial class FullWristControl : UserControl
    {
        private Label[] _labels;
        private CheckBox[] _checkBoxesHide;
        private RadioButton[] _radioButtonsFixed;

        private string[] _boneNames;
        private bool _showSeriesList;

        public FullWristControl()
        {
            InitializeComponent();

            //TODO: TEMP STUFF - REMOVE
            string[] bnames = { "Radius", "uln", "sca", "lun", "Triquetrum", "pis", "tpd", "tpm", "cap", "ham", "mc1", "mc2", "mc3", "mc4", "mc5" };
            setupControl(bnames, true);
        }

        private void setupControl(string[] boneNames, bool showSeriesList)
        {
            _boneNames = boneNames;
            //_boneNames = new string[] { "Radius" };
            //_boneNames = new string[0];
            _showSeriesList = showSeriesList;
            int numBones = _boneNames.Length;

            this.SuspendLayout();
            _labels = new Label[numBones];
            _checkBoxesHide = new CheckBox[numBones];
            _radioButtonsFixed = new RadioButton[numBones];
            tableLayoutPanel1.RowCount = numBones + 1;
            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 19F));
            for (int i = 0; i < numBones; i++)
                setupForm_generateRow(i);
            //seriesListBox.Visible = false;
            tableLayoutPanel1.SetRowSpan(seriesListBox, numBones);

            this.ResumeLayout();
        }

        private void setupForm_generateRow(int rowIndex)
        {
            Label l = new Label();
            CheckBox cb = new CheckBox();
            RadioButton rb = new RadioButton();

            l.Text = _boneNames[rowIndex];
            l.TextAlign = ContentAlignment.MiddleLeft;
            l.AutoSize = true;
            l.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));

            cb.CheckAlign = ContentAlignment.MiddleCenter;
            cb.AutoSize = true;
            cb.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));

            rb.CheckAlign = ContentAlignment.MiddleCenter;
            rb.AutoSize = true;
            rb.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));

            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 19F));
            tableLayoutPanel1.Controls.Add(l, 0, rowIndex+1);
            tableLayoutPanel1.Controls.Add(cb, 1, rowIndex+1);
            tableLayoutPanel1.Controls.Add(rb, 2, rowIndex+1);

            //save these
            _labels[rowIndex] = l;
            _checkBoxesHide[rowIndex] = cb;
            _radioButtonsFixed[rowIndex] = rb;
        }
    }
}
