using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
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

        public PointSelection(ExaminerViewer viewer, WristVizualizer visualizer, string firstFileName)
        {
            _viewer = viewer;
            _visualizer = visualizer;
            _saveFileFormat = Path.Combine(Path.GetDirectoryName(firstFileName), Path.GetFileNameWithoutExtension(firstFileName));

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
            _visualizer.setStatusStripText("");
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
                if (!checkBoxOverwrite.Checked && File.Exists(fname))
                {
                    //error
                    msg = String.Format("Error: File already exists! ({0})", fname);
                    _visualizer.setStatusStripText(msg);
                    return;
                }
                try
                {
                    saveToFile(fname, x, y, z);
                    msg = String.Format("Point {3} saved: {4} " + _pointFormat, x, y, z, _currentPoint, fname);

                    _currentPoint++; //update counter
                }
                catch (Exception ex)
                {
                    msg = "Error saving to file: " + ex.Message;
                    _visualizer.setStatusStripText(msg);
                    return;
                }                
            }
            else
            {
                msg = String.Format("Point selected: " + _pointFormat, x, y, z);
            }
            _visualizer.setStatusStripText(msg);

            //now add marker if needed
            if (checkBoxShowMarker.Checked)
            {
                Separator sep = new Separator();
                Transform t = new Transform();
                t.setTranslation(x, y, z);
                sep.addTransform(t);
                Material m = new Material();
                m.setColor(1f, 0f, 0f);
                sep.addNode(m);
                Sphere s = new Sphere((float)numericUpDownRadius.Value);
                sep.addNode(s);
                _visualizer.Root.addChild(sep);
            }
        }

        private void saveToFile(string fname, float x, float y, float z)
        {
            using (StreamWriter writer = new StreamWriter(fname, false))
            {
                writer.WriteLine(String.Format("{0}\t{1}\t{2}", x, y, z));
            }
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

        private void checkBoxShowMarker_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownRadius.Enabled = checkBoxShowMarker.Checked;
        }
    }
}