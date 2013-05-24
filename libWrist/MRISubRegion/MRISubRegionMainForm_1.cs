using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;


namespace libWrist.MRISubRegion
{
    public partial class MRISubRegionMainForm : Form
    {
        CT _mri;
        Bitmap _frame;
        Graphics _graph;
        Pen _pen;
        Point _tl, _br;
        int _lowZ, _highZ, _radialZ;
        int _width, _height;
        string _path;
        bool _saved = false;

        public MRISubRegionMainForm()
        {
            InitializeComponent();
#if DEBUG
            textBoxMRIPath.Text = @"F:\LocalCopies\E01424_02";
            //textBoxMRIPath.Text = @"C:\Ortho\E01424\CTScans\E01424_07";
#endif
            Bitmap bmp = new Bitmap(512, 512);
            Graphics g = Graphics.FromImage(bmp);
            g.FillRectangle(Brushes.Black, 0, 0, 512, 512);
            pictureBox1.Image = bmp;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void setNumSlices(int num)
        {
            labelMax.Text = ((int)(num - 1)).ToString();
            trackBar1.Maximum = num - 1;
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
            CT newMri = CT.SmartLoad(filename);
            newMri.loadBitmapData();
            if (_mri != null) //save memory. TODO: use dispose instead
                _mri.deleteFrames();
            _mri = newMri; //save it, now that we know it works
            _saved = false;
            _path = filename;
            _tl = new Point(0, 0);
            _br = new Point(_mri.width - 1, _mri.height - 1);
            _width = _mri.width;
            _height = _mri.height;
            textBoxZLow.Text = "0";
            textBoxZHigh.Text = ((int)(_mri.depth - 1)).ToString();
            updateTextDisplay();
            textBoxXVoxel.Text = _mri.voxelSizeX.ToString();
            textBoxYVoxel.Text = _mri.voxelSizeY.ToString();
            textBoxZVoxel.Text = _mri.voxelSizeZ.ToString();
            textBoxRadialStyloid.Text = "";

            setNumSlices(_mri.depth);
            trackBar1.Value = 0;
            loadFrame(0);
        }

        private void loadFrame(int slice)
        {
#if DEBUG
            if (_mri.getFrame(slice) == null) return;
#endif
            _frame = (Bitmap)_mri.getFrame(slice).Clone();
            pictureBox1.Image = _frame;
            _graph = Graphics.FromImage(_frame);
            _pen = new Pen(Color.Green, 1);
            _graph.DrawRectangle(_pen, _tl.X, _tl.Y, _br.X-_tl.X ,_br.Y-_tl.Y);
        }

        private void updateTextDisplay()
        {
            textBoxXLow.Text = _tl.X.ToString();
            textBoxXHigh.Text = _br.X.ToString();
            textBoxYLow.Text = ((int)(511 - _br.Y)).ToString();
            textBoxYHigh.Text = ((int)(511 - _tl.Y)).ToString();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //Console.WriteLine("Mouse in Coord {0}x{1}",e.X,e.Y);
                //find "magic" numbers, or thresholds for moving
                int magicX_left = _tl.X + (_br.X - _tl.X) / 3;
                int magicX_right = _tl.X + (_br.X - _tl.X) * 2 / 3;
                int magicY_top = _tl.Y + (_br.Y - _tl.Y) / 3;
                int magicY_bottom = _tl.Y + (_br.Y - _tl.Y) * 2 / 3;

                bool moveLeftX = e.X <= magicX_left;
                bool moveRightX = e.X >= magicX_right;
                bool moveTopY = e.Y <= magicY_top;
                bool moveBottomY = e.Y >= magicY_bottom;

                if (moveLeftX)
                    _tl.X = Math.Min(_br.X-1, Math.Max(0, e.X));

                if (moveRightX)
                    _br.X = Math.Min(_width-1, Math.Max(_tl.X+1, e.X));

                if (moveTopY)
                    _tl.Y = Math.Min(_br.Y-1, Math.Max(0, e.Y));

                if (moveBottomY)
                    _br.Y = Math.Min(_height-1, Math.Max(_tl.Y+1, e.Y));

                updateTextDisplay();
                loadFrame(trackBar1.Value);
            }
        }

        private void buttonLow_Click(object sender, EventArgs e)
        {
            _lowZ = trackBar1.Value;
            textBoxZLow.Text = _lowZ.ToString();
        }

        private void buttonHigh_Click(object sender, EventArgs e)
        {
            _highZ = trackBar1.Value;
            textBoxZHigh.Text = _highZ.ToString();
        }

        private void buttonRadialStyloid_Click(object sender, EventArgs e)
        {
            _radialZ = trackBar1.Value;
            textBoxRadialStyloid.Text = _radialZ.ToString();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {

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
            if (!_saved && groupBoxSubject.Visible)
            {
                string msg = "You have not yet saved the crop values for this subject.\nDo you wish to load a new scan anyway?";
                DialogResult r = MessageBox.Show(msg, "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (r != DialogResult.Yes) return;
            }
            textBoxMRIPath.Text = textBoxMRIPath.Text.Trim();
            try
            {
                this.Cursor = Cursors.WaitCursor;
                detectStructure(textBoxMRIPath.Text);
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

        private void textBoxMRIPath_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                //the enter key was pressed, call load
                buttonLoad_Click(sender, null);
            }
        }

        private void detectStructure(string mriPath)
        {
            Regex reg = new Regex(@"(E\d{5})\\CTScans\\(E\d{5})_(\d{2})[\\\s]*$",RegexOptions.IgnoreCase);
            Match m = reg.Match(mriPath);
            if (!m.Success) return;
            if (m.Groups.Count != 4) return;
            
            if (!m.Groups[1].Value.ToLower().Equals(m.Groups[2].Value.ToLower()))
                return; //strange that the subjects don't align?

            textBoxSubject.Text = m.Groups[1].Value.ToUpper();
            textBoxSeries.Text = m.Groups[3].Value + (radioButtonLeft.Checked ? "L" : "R");
            groupBoxSubject.Visible = true;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            DirectoryInfo mri = new DirectoryInfo(_path);
            DirectoryInfo subject = mri.Parent.Parent;
            string filename = Path.Combine(subject.FullName, "crop_values.txt");
            StreamWriter writer = new StreamWriter(filename, true);
            object[] args = { textBoxSubject.Text, textBoxSeries.Text.Substring(0,2), textBoxSeries.Text.Substring(2,1),
                textBoxXLow.Text, textBoxXHigh.Text,
                textBoxYLow.Text, textBoxYHigh.Text,
                textBoxZLow.Text, textBoxZHigh.Text,
                textBoxXVoxel.Text, textBoxZVoxel.Text, textBoxRadialStyloid.Text};
            writer.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}", args);
            writer.Close();
            _saved = true;
        }

        private void buttonSaveAdvance_Click(object sender, EventArgs e)
        {
            //first actually save it
            buttonSave_Click(sender, e);
            if (!_saved)  //stupid double check that we didn't have an error saving
                return;
            DirectoryInfo mri = new DirectoryInfo(_path);
            DirectoryInfo ctScan = mri.Parent;
            string series = mri.Name;
            if (series.Length != 9) return;
            int seriesNum;
            if (!Int32.TryParse(series.Substring(7, 2), out seriesNum))
                return;
            seriesNum++;
            string newFullPath = Path.Combine(ctScan.FullName, String.Format("{0}{1:00}", series.Substring(0, 7), seriesNum));

            textBoxMRIPath.Text = newFullPath;
            buttonLoad_Click(sender, e);
        }

    }
}