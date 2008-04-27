using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WristVizualizer
{
    public delegate void SelectedSeriesChangedHandler(object sender, SelectedSeriesChangedEventArgs e);
    public delegate void FixedBoneChangedHandler(object sender, FixedBoneChangeEventArgs e);
    public delegate void BoneHideChangedHandler(object sender, BoneHideChangeEventArgs e);


    public partial class FullWristControl : UserControl
    {
        public event SelectedSeriesChangedHandler SelectedSeriesChanged;
        public event FixedBoneChangedHandler FixedBoneChanged;
        public event BoneHideChangedHandler BoneHideChanged;

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

        #region Public Interfaces
        public void clearSeriesList()
        {
            seriesListBox.Items.Clear();
        }

        public void addToSeriesList(object item)
        {
            seriesListBox.Items.Add(item);
        }
        public void addToSeriesList(object[] items)
        {
            seriesListBox.Items.AddRange(items);
        }

        public void disableBone(int boneIndex)
        {
            _checkBoxesHide[boneIndex].Enabled = false;
            _radioButtonsFixed[boneIndex].Enabled = false;
        }

        public void disableFixingBone(int boneIndex)
        {
            _radioButtonsFixed[boneIndex].Enabled = false;
        }

        public void setBoneHiddenStatus(int boneIndex, bool hidden)
        {
            _checkBoxesHide[boneIndex].Checked = hidden;
        }

        public void hideBone(int boneIndex)
        {
            setBoneHiddenStatus(boneIndex, true);
        }

        public int selectedSeriesIndex
        {
            get { return seriesListBox.SelectedIndex; }
            set { seriesListBox.SelectedIndex = value; }
        }

        #endregion

        #region GUI Control Setup
        public void setupControl(string[] boneNames, bool showSeriesList)
        {
            if (boneNames.Length == 0)
                throw new ArgumentException("Can't create Control for 0 bones");
            _boneNames = boneNames;
            _showSeriesList = showSeriesList;
            setupControlLayout();

            //set the first bone to be fixed
            _radioButtonsFixed[0].Checked = true;
        }

        private void setupControlLayout()
        {
            int numBones = _boneNames.Length;

            this.SuspendLayout();
            //remove the hide/show all
            tableLayoutPanel1.Controls.Remove(linkLabelHideAll);
            tableLayoutPanel1.Controls.Remove(linkLabelShowAll);

            _labels = new Label[numBones];
            _checkBoxesHide = new CheckBox[numBones];
            _radioButtonsFixed = new RadioButton[numBones];
            tableLayoutPanel1.RowCount = numBones + 3;
            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 19F));
            for (int i = 0; i < numBones; i++)
                setupForm_generateRow(i);

            if (!_showSeriesList)
            {
                seriesListBox.Visible = false;
                labelSeries.Visible = false;
            }
            tableLayoutPanel1.SetRowSpan(seriesListBox, numBones);

            //add in the hideall/showall links
            tableLayoutPanel1.Controls.Add(linkLabelShowAll, 1, numBones + 1);
            tableLayoutPanel1.Controls.Add(linkLabelHideAll, 1, numBones + 2);

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

            //add event callbacks
            cb.CheckedChanged += new EventHandler(checkBox_CheckedChanged);
            rb.CheckedChanged += new EventHandler(radioButton_CheckedChanged);

            //save these
            _labels[rowIndex] = l;
            _checkBoxesHide[rowIndex] = cb;
            _radioButtonsFixed[rowIndex] = rb;
        }
        #endregion

        #region Event Listeners
        void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (FixedBoneChanged == null) return;
            /* This method will get called twice on a change, once when checked, 
             * and once when unchecked. To prevent problems, we will ignore 
             * events for the button that was unchecked, and deal only with
             * events for the check event
             */
            if (!((RadioButton)sender).Checked) return;

            for (int i = 0; i < _radioButtonsFixed.Length; i++)
            {
                if (sender == _radioButtonsFixed[i])
                {
                    FixedBoneChanged(sender, new FixedBoneChangeEventArgs(i));
                }
            }
        }

        void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (BoneHideChanged == null) return;
            for (int i = 0; i < _checkBoxesHide.Length; i++)
            {
                if (sender == _checkBoxesHide[i])
                {
                    bool hidden = ((CheckBox)sender).Checked;
                    BoneHideChanged(sender, new BoneHideChangeEventArgs(i, hidden));
                }
            }
        }

        private void seriesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedSeriesChanged == null) return;
            int index = seriesListBox.SelectedIndex;
            SelectedSeriesChanged(sender, new SelectedSeriesChangedEventArgs(index));
        }

        private void linkLabelShowAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            for (int i = 0; i < _checkBoxesHide.Length; i++)
                _checkBoxesHide[i].Checked = false;
        }

        private void linkLabelHideAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            for (int i = 0; i < _checkBoxesHide.Length; i++)
                _checkBoxesHide[i].Checked = true;
        }
        #endregion
    }
}