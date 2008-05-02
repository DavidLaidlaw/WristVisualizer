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

            //load last saved image
            textBoxMRIPath.Text = RegistrySettings.getSettingString("TextureLastMRIImage");

            Bitmap bmp = new Bitmap(512, 512);
            Graphics g = Graphics.FromImage(bmp);
            g.FillRectangle(Brushes.Black, 0, 0, 512, 512);
            pictureBox1.Image = bmp;
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
            loadFrame();
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
            RegistrySettings.saveSetting("TextureLastMRIImage", filename);

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
            loadFrame();
        }

        private void loadFrame()
        {
            int slice = trackBar1.Value;
            int echo = (int)numericUpDownLayer.Value;
            _frame = (Bitmap)_mri.getFrame(slice,echo).Clone();

            //check for scaling....
            if (radioButtonNoZoom.Checked && numericUpDownZoomFactor.Value > 1)
            {
                int scale = (int)numericUpDownZoomFactor.Value;
                Bitmap scaledImage = new Bitmap(_frame.Width * scale, _frame.Height * scale);
                Graphics gr = Graphics.FromImage(scaledImage);
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                gr.DrawImage(_frame, 0, 0, scaledImage.Width, scaledImage.Height);
                //Do I need to dispose of the graphics object...? not certain
                _frame = scaledImage; 
            }
            pictureBox1.Image = _frame;
        }

        private void numericUpDownLayer_ValueChanged(object sender, EventArgs e)
        {
            loadFrame();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            int z = trackBar1.Value;
            int x = 0, y = 0;
            //lets convert x & y to their real pixel value....
            if (radioButtonNoZoom.Checked)
            {
                int scale = (int)numericUpDownZoomFactor.Value;
                /* The interpolation algorithm chopps off the top half of the top row, and the 
                 * left half of the left column; so we need to correct for that in order to
                 * identify the correct pixel
                 */
                int offset = scale / 2;
                x = (e.X+offset)/scale;
                y = _mri.height - ((e.Y+offset)/scale) - 1; //flip y coord
            }
            else if (radioButtonZoomStrech.Checked)
            {
                int offsetX = (int)Math.Ceiling(pictureBox1.Width/_mri.width / 2.0);
                int offsetY = (int)Math.Ceiling(pictureBox1.Height / _mri.height / 2.0);
                x = (int)((double)(e.X+offsetX) * _mri.width / pictureBox1.Width);
                y = _mri.height - (int)((double)(e.Y+offsetY) * _mri.height / pictureBox1.Height) - 1; //need to flip Y coordinate
            }
            else if (radioButtonZoomZoom.Checked)
            {
                //TODO: don't know how to figure out the margin for the....so nothing to do
                throw new NotImplementedException("Can't yet have a scaled zoom for images");
            }

            //check if we are outside the picture
            if (x >= _mri.width || x < 0 || y >= _mri.height || y < 0)
            {
                pictureBox1_MouseLeave(this, null);
                return;
            }
            
            textBoxX.Text = x.ToString();
            textBoxY.Text = y.ToString();
            textBoxZ.Text = z.ToString();
            textBoxIntensity.Text = _mri.getVoxel(x, y, z, (int)numericUpDownLayer.Value).ToString();
            textBoxIntensitySigned.Text = ((short)_mri.getVoxel(x,y,z, (int)numericUpDownLayer.Value)).ToString();
            textBoxIntensityScaled.Text = _mri.getVoxel_s(x, y, z, (int)numericUpDownLayer.Value).ToString();
            textBoxAutoScale.Text = _mri.getVoxel_as(x, y, z, (int)numericUpDownLayer.Value).ToString();
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            textBoxX.Clear();
            textBoxY.Clear();
            textBoxZ.Clear();
            textBoxIntensity.Clear();
            textBoxIntensityScaled.Clear();
            textBoxIntensitySigned.Clear();
            textBoxAutoScale.Clear();
        }

        private void radioButtonZoom_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownZoomFactor.Enabled = false;
            if (radioButtonNoZoom.Checked)
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.Normal;
                numericUpDownZoomFactor.Enabled = true;
            }
            else if (radioButtonZoomStrech.Checked)
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            else if (radioButtonZoomZoom.Checked)
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            loadFrame(); //refresh on screen
        }

        private void numericUpDownZoomFactor_ValueChanged(object sender, EventArgs e)
        {
            loadFrame();
        }
    }
}