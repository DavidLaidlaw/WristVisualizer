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

        private CheckBox[] _contourCheckBoxes;
        private NumericUpDown[] _contourNumericUpDowns;
        private double[] _defaultContourDistances;

        public enum CalculationTypes
        {
            Current,
            All,
            None
        }

        public DistanceAndContourDialog() : this(DEFAULT_DISTANCES, 5) { }
        public DistanceAndContourDialog(double[] defaultContourDistances, int maxNumberContours)
        {
            InitializeComponent();
            if (maxNumberContours < defaultContourDistances.Length)
                maxNumberContours = defaultContourDistances.Length;

            _defaultContourDistances = defaultContourDistances;

            _contourCheckBoxes = new CheckBox[maxNumberContours];
            _contourNumericUpDowns = new NumericUpDown[maxNumberContours];
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

            box.AutoSize = true;
            box.CheckedChanged += new EventHandler(contourCheckBox_CheckedChanged);

            upDown.Minimum = 0;
            upDown.Maximum = 5;
            upDown.DecimalPlaces = 2;
            upDown.Increment = 0.1M;
            upDown.Size = new System.Drawing.Size(59, 20);
            upDown.Enabled = false;

            label.AutoSize = true;
            label.Text = "mm";
            label.Margin = new Padding(0);
            label.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label.TextAlign = ContentAlignment.TopLeft;

            tableLayoutPanel.Controls.Add(box, 0, row);
            tableLayoutPanel.Controls.Add(upDown, 1, row);
            tableLayoutPanel.Controls.Add(label, 2, row);
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());

            _contourCheckBoxes[row] = box;
            _contourNumericUpDowns[row] = upDown;

        }
        #endregion

        #region Properties
        public CalculationTypes CalculateColorMap
        {
            get
            {
                if (radioButtonAll.Checked) return CalculationTypes.All;
                if (radioButtonCurrent.Checked) return CalculationTypes.Current;
                if (radioButtonNone.Checked) return CalculationTypes.None;
                throw new WristVizualizerException("No type of color map specified!");
            }
        }

        public CalculationTypes CalculateContours
        {
            get
            {
                if (radioButtonContourAll.Checked) return CalculationTypes.All;
                if (radioButtonContourCurrent.Checked) return CalculationTypes.Current;
                if (radioButtonContourNone.Checked) return CalculationTypes.None;
                throw new WristVizualizerException("No type of contour specified!");
            }
        }

        public double ColorMapMaxDistance
        {
            get { return (double)numericUpDownDistanceMapDist.Value; }
            set { numericUpDownDistanceMapDist.Value = (decimal)value; }
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

        
    }
}