using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using libWrist;

namespace WristVizualizer
{
    public partial class EditBoneColors : Form
    {
        private FullWrist _fullWrist;
        private Button[] _colorButtons;
        private Color[] _startingColors;

        public EditBoneColors(FullWrist fullWrist)
        {
            _fullWrist = fullWrist;
            InitializeComponent();
            _startingColors = new Color[WristFilesystem.NumBones];
            for (int i = 0; i < WristFilesystem.NumBones; i++)
                _startingColors[i] = _fullWrist.Bones[i].GetColor();
            setupGUI();
        }

        private void setupGUI()
        {
            this.SuspendLayout();
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.RowStyles.Clear();

            _colorButtons = new Button[WristFilesystem.NumBones];
            for (int i = 0; i < WristFilesystem.NumBones; i++)
            {
                string labelText = String.Format("{0}:", _fullWrist.Bones[i].LongName);
                //Color startColor = _fullWrist.Bones[i]
                setupContourRow(i, labelText, Color.White);
                if (!_fullWrist.Bones[i].IsValidBone)
                {
                    _colorButtons[i].Enabled = false;
                }
                else
                {
                    _colorButtons[i].BackColor = _startingColors[i];
                }
            }
            this.ResumeLayout();
        }

        private void setupContourRow(int row, string labelText, Color startColor)
        {
            Label label = new Label();
            Button button = new Button();

            label.AutoSize = true;
            label.Text = labelText;
            label.Margin = new Padding(0);
            label.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label.TextAlign = ContentAlignment.TopLeft;

            button.AutoSize = true;
            button.Size = new System.Drawing.Size(20, 20);
            button.BackColor = startColor;
            button.Click += new EventHandler(colorButton_Click);

            tableLayoutPanel1.Controls.Add(label, 0, row);
            tableLayoutPanel1.Controls.Add(button, 1, row);
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());

            _colorButtons[row] = button;

        }

        void colorButton_Click(object sender, EventArgs e)
        {
            //lets find out which one we are, and change the enabled state of our little friend
            for (int i = 0; i < _colorButtons.Length; i++)
            {
                if (_colorButtons[i] == sender) //found us!
                {
                    ColorDialog cg = new ColorDialog();
                    cg.Color = ((Button)sender).BackColor;
                    DialogResult r = cg.ShowDialog();
                    if (r == DialogResult.Cancel)
                        return;

                    ((Button)sender).BackColor = cg.Color;
                }
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public bool IsColorChanged(int boneIndex)
        {
            return (_startingColors[boneIndex] != _colorButtons[boneIndex].BackColor);
        }

        public System.Drawing.Color GetNewBoneColor(int boneIndex)
        {
            return _colorButtons[boneIndex].BackColor;
        }

    }
}