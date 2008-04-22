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
            //pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
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
            newMri.loadBitmapDataAllLayers();
            if (_mri != null) //save memory. TODO: use dispose instead
                _mri.deleteFrames();
            _mri = newMri; //save it, now that we know it works
            _path = filename;


            textBoxXSize.Text = _mri.width.ToString();
            textBoxYSize.Text = _mri.height.ToString();
            textBoxZSize.Text = _mri.depth.ToString();
            textBoxXVoxel.Text = _mri.voxelSizeX.ToString();
            textBoxYVoxel.Text = _mri.voxelSizeY.ToString();
            textBoxZVoxel.Text = _mri.voxelSizeZ.ToString();
            textBoxLayersSize.Text = _mri.Layers.ToString();

            numericUpDownLayer.Minimum = 0;
            numericUpDownLayer.Maximum = _mri.Layers - 1;
            numericUpDownLayer.Value = 0;

            trackBar1.Value = 0;
            trackBar1.Maximum = _mri.depth - 1;
            loadFrame(0);
        }

        private void loadFrame(int slice)
        {
            int echo = (int)numericUpDownLayer.Value;
            _frame = (Bitmap)_mri.getFrame(slice,echo).Clone();
            pictureBox1.Image = _frame;
        }

        private void numericUpDownLayer_ValueChanged(object sender, EventArgs e)
        {
            loadFrame(trackBar1.Value);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            int z = trackBar1.Value;

            //lets convert x & y to their real pixel value....
            int x = (int)((double)e.X * _mri.width / pictureBox1.Width);
            int y = _mri.height - (int)((double)e.Y * _mri.height / pictureBox1.Height) - 1; //need to flip Y coordinate
            textBoxX.Text = x.ToString();
            textBoxY.Text = y.ToString();
            textBoxZ.Text = z.ToString();
            textBoxIntensity.Text = _mri.getVoxel(x, y, z, (int)numericUpDownLayer.Value).ToString();
            textBoxIntensityScaled.Text = _mri.getVoxel_s(x, y, z, (int)numericUpDownLayer.Value).ToString();
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            textBoxX.Clear();
            textBoxY.Clear();
            textBoxZ.Clear();
            textBoxIntensity.Clear();
            textBoxIntensityScaled.Clear();
            textBoxIntensitySigned.Clear();
        }
    }
}