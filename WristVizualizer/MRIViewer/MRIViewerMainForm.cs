using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using libWrist;

namespace WristVizualizer.MRIViewer
{
    public partial class MRIViewerMainForm : Form
    {
        CTmri _mri;
        Bitmap _frame;
        string _path;

        public MRIViewerMainForm()
        {
            InitializeComponent();

            Bitmap bmp = new Bitmap(512, 512);
            Graphics g = Graphics.FromImage(bmp);
            g.FillRectangle(Brushes.Black, 0, 0, 512, 512);
            pictureBox1.Image = bmp;
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            textBoxMRIPath.Text = textBoxMRIPath.Text.Trim();
            FolderBrowserDialog browser = new FolderBrowserDialog();
            if (textBoxMRIPath.Text.Length > 0)
            {
                browser.SelectedPath = textBoxMRIPath.Text;
            }
            DialogResult r = browser.ShowDialog();
            if (r == DialogResult.Cancel) return;
            textBoxMRIPath.Text = browser.SelectedPath;
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            textBoxMRIPath.Text = textBoxMRIPath.Text.Trim();
            try
            {
                this.Cursor = Cursors.WaitCursor;
                //detectStructure(textBoxMRIPath.Text);
                loadMRI(textBoxMRIPath.Text);
                panel1.Enabled = true;
            }
            catch (Exception ex)
            {
                string msg = "Error loading mri.\n" + ex.Message;
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            textBoxSlice.Text = trackBar1.Value.ToString();
            loadFrame(trackBar1.Value);
        }

        /// <summary>
        /// Will go load the mri for the given directory
        /// </summary>
        /// <param name="filename"></param>
        /// <exception cref="System.ArgumentException"></exception>
        private void loadMRI(string filename)
        {
            /* Try and load first (and to temp space). If we fail, it will throw an exception 
             * And the rest of the code will not get run
             */
            CTmri newMri = new CTmri(filename);
            if (_mri != null) //save memory. TODO: use dispose instead
                _mri.deleteFrames();
            _mri = newMri; //save it, now that we know it works
            _path = filename;
            textBoxZLow.Text = "0";
            textBoxZHigh.Text = ((int)(_mri.depth - 1)).ToString();
            //updateTextDisplay();
            textBoxXVoxel.Text = _mri.voxelSizeX.ToString();
            textBoxYVoxel.Text = _mri.voxelSizeY.ToString();
            textBoxZVoxel.Text = _mri.voxelSizeZ.ToString();

            trackBar1.Value = 0;
            loadFrame(0);
        }

        private void loadFrame(int slice)
        {
            _frame = (Bitmap)_mri.getFrame(slice).Clone();
            pictureBox1.Image = _frame;
        }
    }
}