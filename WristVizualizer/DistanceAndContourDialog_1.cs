using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace WristVizualizer
{
    public partial class DistanceAndContourDialog : Form
    {
        private static double[] DEFAULT_DISTANCES = {1.0, 1.5, 2.0};
        private const int DEFAULT_MAX_NUM_CONTOURS = 5;

        private CheckBox[] _contourCheckBoxes;
        private NumericUpDown[] _contourNumericUpDowns;
        private Button[] _contourColorButtons;
        private double[] _defaultContourDistances;


        public DistanceAndContourDialog() : this(DEFAULT_DISTANCES, DEFAULT_MAX_NUM_CONTOURS) { }
        public DistanceAndContourDialog(double[] defaultContourDistances) : this(defaultContourDistances, DEFAULT_MAX_NUM_CONTOURS) { }
        public DistanceAndContourDialog(double[] defaultContourDistances, int maxNumberContours)
        {
            InitializeComponent();
            //check for empty defaults
            if (defaultContourDistances == null || defaultContourDistances.Length==0)
                defaultContourDistances = DEFAULT_DISTANCES;
            if (maxNumberContours < 0)
                maxNumberContours = DEFAULT_MAX_NUM_CONTOURS;


            if (maxNumberContours < defaultContourDistances.Length)
                maxNumberContours = defaultContourDistances.Length;

            _defaultContourDistances = defaultContourDistances;

            _contourCheckBoxes = new CheckBox[maxNumberContours];
            _contourNumericUpDowns = new NumericUpDown[maxNumberContours];
            _contourColorButtons = new Button[maxNumberContours];
            setupContourSelectionGUI();
        }

        #region GUI Setup
        private void setupContourSelectionGUI()
        {
            this.SuspendLayout();
            tableLayoutPanel.Controls.Clear();
            tableLayoutPanel.RowStyles.Clear();

            for (int i = 0; i < _contourCheckBoxes.Length; i++)
            {          
                setupContourRow(i);
                if (_defaultContourDistances.Length > i) //check if we have a default value...
                {
                    _contourCheckBoxes[i].Checked = true;
                    _contourNumericUpDowns[i].Enabled = true;
                    _contourNumericUpDowns[i].Value = (decimal)_defaultContourDistances[i];
                }
            }
            this.ResumeLayout();
        }

        private void setupContourRow(int row)
        {
            CheckBox box = new CheckBox();
            NumericUpDown upDown = new NumericUpDown();
            Label label = new Label();
            Button button = new Button();

            box.AutoSize = true;
            box.CheckedChanged += new EventHandler(contourCheckBox_CheckedChanged);

            upDown.Minimum = 0;
            upDown.Maximum = 5;
            upDown.DecimalPlaces = 2;
            upDown.Increment = 0.1M;
            upDown.Size = new System.Drawing.Size(59, 20);
            upDown.Enabled = false;

            button.AutoSize = true;
            button.Size = new System.Drawing.Size(22, 22);
            button.BackColor = Color.White;
            button.Click += new EventHandler(colorButton_Click);

            label.AutoSize = true;
            label.Text = "mm";
            label.Margin = new Padding(0);
            label.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label.TextAlign = ContentAlignment.TopLeft;

            tableLayoutPanel.Controls.Add(box, 0, row);
            tableLayoutPanel.Controls.Add(upDown, 1, row);
            tableLayoutPanel.Controls.Add(label, 2, row);
            tableLayoutPanel.Controls.Add(button, 3, row);
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());

            _contourCheckBoxes[row] = box;
            _contourNumericUpDowns[row] = upDown;
            _contourColorButtons[row] = button;

        }

        #endregion

        #region Properties
        public bool CalculateColorMap
        {
            get { return radioButtonDistanceAll.Checked; }
            set
            {
                radioButtonDistanceAll.Checked = value;
                radioButtonDistanceHide.Checked = !value;
            }
        }

        public bool CalculateContours
        {
            get { return radioButtonContourAll.Checked; }
            set
            {
                radioButtonContourAll.Checked = value;
                radioButtonContourHide.Checked = !value;
            }
        }

        public double ColorMapMaxDistance
        {
            get { return (double)numericUpDownDistanceMapDist.Value; }
            set
            {
                if (value <= 0) return; //skip junk values
                numericUpDownDistanceMapDist.Value = (decimal)value;
            }
        }
        #endregion

        public double[] getContourDistancesToCalculate()
        {
            ArrayList dists = new ArrayList();
            for (int i = 0; i < _contourCheckBoxes.Length; i++)
            {
                if (!_contourCheckBoxes[i].Checked) continue;
                if (_contourNumericUpDowns[i].Value == 0) continue;
                dists.Add((double)_contourNumericUpDowns[i].Value);
            }
            return (double[])dists.ToArray(typeof(double));
        }

        public Color[] getContourColorsToCalculate()
        {
            ArrayList colors = new ArrayList();
            for (int i = 0; i < _contourCheckBoxes.Length; i++)
            {
                if (!_contourCheckBoxes[i].Checked) continue;
                if (_contourNumericUpDowns[i].Value == 0) continue;
                colors.Add(_contourColorButtons[i].BackColor);
            }
            return (Color[])colors.ToArray(typeof(Color));
        }

        public void setContourColors(Color[] colors)
        {
            if (colors == null) return; //leave default
            for (int i = 0; i < _contourColorButtons.Length && i<colors.Length; i++)
                _contourColorButtons[i].BackColor = colors[i];
        }

        void contourCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            //lets find out which one we are, and change the enabled state of our little friend
            for (int i = 0; i < _contourCheckBoxes.Length; i++)
            {
                if (_contourCheckBoxes[i] == sender) //found us!
                {
                    _contourNumericUpDowns[i].Enabled = ((CheckBox)sender).Checked;
                    return;
                }
            }
        }


        void colorButton_Click(object sender, EventArgs e)
        {
            //lets find out which one we are, and change the enabled state of our little friend
            for (int i = 0; i < _contourColorButtons.Length; i++)
            {
                if (_contourColorButtons[i] == sender) //found us!
                {
                    ColorDialog cg = new ColorDialog();
                    cg.Color = ((Button)sender).BackColor;
                    DialogResult r= cg.ShowDialog();
                    if (r == DialogResult.Cancel)
                        return;

                    ((Button)sender).BackColor = cg.Color;
                }
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void radioButtonDistanceHide_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownDistanceMapDist.Enabled = radioButtonDistanceAll.Checked;
        }

        private void radioButtonContourHide_CheckedChanged(object sender, EventArgs e)
        {
            tableLayoutPanel.Enabled = radioButtonContourAll.Checked;
        }

        
    }
}