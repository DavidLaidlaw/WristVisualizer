using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using libCoin3D;

namespace WristVizualizer
{
    public partial class PointSelection : Form
    {
        private ExaminerViewer _viewer;
        private WristVizualizer _visualizer;
        private bool _selectionEnabled = false;

        private int _currentPoint = 1;
        private string _saveFileFormat = "";
        private string _pointFormat;

        public PointSelection(ExaminerViewer viewer, WristVizualizer visualizer)
        {
            _viewer = viewer;
            _visualizer = visualizer;

            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            _selectionEnabled = true;
            _viewer.setRaypick();  //active raypicking event handler
            _viewer.OnRaypick += new RaypickEventHandler(_viewer_OnRaypick);

            _pointFormat = precisionOutput((int)numericUpDownPrecision.Value);

            this.Close();
        }

        

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void stopSelecting()
        {
            _viewer.resetRaypick();
            _viewer.OnRaypick -= new RaypickEventHandler(_viewer_OnRaypick);
        }

        private string precisionOutput(int precision)
        {
            if (precision == 0) return "({0:0}, {1:0}, {2:0})";
            StringBuilder b = new StringBuilder(":0.");
            for (int i = 0; i < precision; i++)
                b.Append("0");

            string f = b.ToString();
            return "({0" + f + "}, {1" + f + "}, {2" + f + "})";
        }

        void _viewer_OnRaypick(float x, float y, float z)
        {
            string msg;
            if (checkBoxSaveToFile.Checked)
            {
                string fname = String.Format("{0}_point{1}.stack", _saveFileFormat, _currentPoint);
                //TODO: save out to file....

                msg = String.Format("Point {3} saved: {4} " + _pointFormat, x, y, z, _currentPoint, fname);

                _currentPoint++; //update counter
            }
            else
            {
                msg = String.Format("Point selected: " + _pointFormat, x, y, z);
            }
            _visualizer.setStatusStripText(msg);
        }

        private void checkBoxSaveToFile_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxOverwrite.Enabled = checkBoxSaveToFile.Checked;
        }

        public bool SelectionEnabled
        {
            get { return _selectionEnabled; }
        }

        public bool DisplayInStatusBar
        {
            get { return checkBoxShowStatus.Checked; }
        }

        private void checkBoxShowStatus_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownPrecision.Enabled = checkBoxShowStatus.Checked;
        }
    }
}