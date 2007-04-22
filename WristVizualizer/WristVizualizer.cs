using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using libCoin3D;
using libWrist;

namespace WristVizualizer
{
    public partial class WristVizualizer : Form
    {
        private Coin3DBase _base;
        private ExaminerViewer _viewer;
        private Separator _root;
        private Separator[] _bones;
        private Separator[] _inertias;
        private string[] _bnames = { "rad", "uln", "sca", "lun", "trq", "pis", "tpd", "tpm", "cap", "ham", "mc1", "mc2", "mc3", "mc4", "mc5" };
        private CheckBox[] _hideBoxes;
        private Wrist _wrist;
        private Transform[][] _transforms;
        private int _currentPositionIndex;

        public WristVizualizer()
        {
            InitializeComponent();
            _base = new Coin3DBase();
            _viewer = null;
            _root = null;
            _wrist = null;
            _bones = new Separator[15];
            _inertias = new Separator[15];
            _currentPositionIndex = 0;
            setupControlBox();
            //showControlBox();            
        }

        private void hideControlBox()
        {
            if (panelControl.Visible)
            {
                panelCoin.Width = panelCoin.Width + panelControl.Width + 15;
                panelControl.Visible = false;
            }
        }

        private void showControlBox()
        {
            if (!panelControl.Visible)
            {
                panelCoin.Width = panelCoin.Width - panelControl.Width - 15;
                panelControl.Visible = true;
            }
        }

        private void setupControlBox()
        {
            _hideBoxes = new CheckBox[15];
            _hideBoxes[0] = checkBoxRad;
            _hideBoxes[1] = checkBoxUln;
            _hideBoxes[2] = checkBoxSca;
            _hideBoxes[3] = checkBoxLun;
            _hideBoxes[4] = checkBoxTrq;
            _hideBoxes[5] = checkBoxPis;
            _hideBoxes[6] = checkBoxTpd;
            _hideBoxes[7] = checkBoxTpm;
            _hideBoxes[8] = checkBoxCap;
            _hideBoxes[9] = checkBoxHam;
            _hideBoxes[10] = checkBoxMC1;
            _hideBoxes[11] = checkBoxMC2;
            _hideBoxes[12] = checkBoxMC3;
            _hideBoxes[13] = checkBoxMC4;
            _hideBoxes[14] = checkBoxMC5;
        }

        private void resetForm()
        {
            importToolStripMenuItem.Enabled = true;
            backgroundColorToolStripMenuItem.Enabled = true;
            decoratorToolStripMenuItem.Enabled = true;
            saveFrameToolStripMenuItem.Enabled = true;
            showInertiasToolStripMenuItem.Enabled = false;
            showInertiasToolStripMenuItem.Checked = false;
            _bones = new Separator[15];
            for (int i = 0; i < _bnames.Length; i++)
            {
                _hideBoxes[i].Checked = false;
                _hideBoxes[i].Enabled = true;
            }
        }

        private void openFile(string[] filenames, bool loadFull)
        {
            if (_viewer == null)
                _viewer = new ExaminerViewer((int)panelCoin.Handle);

            _root = new Separator();

            //if we get here, then we are loading a new file....so
            resetForm();

            if (loadFull)
            {

                showControlBox();
                loadFullWrist(filenames[0], _root);
            }
            else
            {
                hideControlBox();
                foreach (string filename in filenames)
                    _root.addFile(filename);
            }

            _viewer.setSceneGraph(_root);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
#if DEBUG
            if (System.IO.Directory.Exists(@"L:\Data\CADAVER_WRISTS\Pinned\l\E03274\S15L\IV.files"))
                open.InitialDirectory = @"L:\Data\CADAVER_WRISTS\Pinned\l\E03274\S15L\IV.files";
#endif
            open.Filter = "Compatable Files (*.iv;*.wrl)|*.iv;*.wrl|Inventor Files (*.iv)|*.iv|VRML Files (*.wrl)|*.wrl|All Files (*.*)|*.*";
            open.Multiselect = true;
            if (DialogResult.OK == open.ShowDialog())
            {
                if (open.FileNames.Length == 0) return;

                bool loadFull = false;

                if (open.FileNames.Length == 1)
                {
                    //check if this is a radius and what we want to do....
                    if (Wrist.isRadius(open.FileName))
                    {
                        string msg = "It looks like you are trying to open a radius.\n\nDo you wish to load the entire wrist?";
                        if (DialogResult.Yes == MessageBox.Show(msg, "Wrist Vizualizer", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                            //load full wrist
                            loadFull = true;
                    

                    }
                }

                openFile(open.FileNames, loadFull);
            }
        }

        private void populateSeriesList()
        {
            if (_wrist != null)
            {
                _currentPositionIndex = 0;
                seriesListBox.Items.Clear();
                //add neutral
                seriesListBox.Items.Add(_wrist.neutralSeries);
                seriesListBox.Items.AddRange(_wrist.series);
                seriesListBox.SelectedIndex = 0;
            }
        }

        private void loadFullWrist(string radius, Separator root)
        {
            string basepath = Path.GetDirectoryName(radius);
            string extension = Path.GetExtension(radius);
            string series = Path.GetFileNameWithoutExtension(radius).Substring(3, 3);

            //block importing a file
            importToolStripMenuItem.Enabled = false;
            showInertiasToolStripMenuItem.Enabled = true;

            //Setup motion files, etc
            _wrist = new Wrist(radius);
            //_wrist.setupPaths(radius);
            //_wrist.findAllSeries();
            _transforms = DatParser.makeAllTransforms(_wrist.motionFiles,_bnames.Length);
            populateSeriesList();

            for (int i = 0; i < _bnames.Length; i++)
            {
                string fname = _wrist.bpaths[i];
                if (File.Exists(fname))
                {
                    _bones[i] = new Separator();
                    _bones[i].addFile(fname, true);
                    root.addChild(_bones[i]);
                }
                else
                {
                    _hideBoxes[i].Enabled = false;
                }
            }
        }

        void checkBox_CheckedChanged(object sender, System.EventArgs e)
        {
            //figure out what check box this is...
            for (int i = 0; i < _bnames.Length; i++)
            {
                if (sender == _hideBoxes[i] && _bones[i]!=null)
                {
                    //now hide that bone
                    if (_hideBoxes[i].Checked)
                        _bones[i].hide();
                    else
                        _bones[i].show();
                }

            }
        }

        void decoratorToolStripMenuItem_CheckedChanged(object sender, System.EventArgs e)
        {
            Console.WriteLine("State is: " + decoratorToolStripMenuItem.Checked.ToString());
            if (_viewer != null)
                _viewer.setDecorator(decoratorToolStripMenuItem.Checked);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        void WristVizualizer_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            /*
            DialogResult r = MessageBox.Show("Are you sure you want to exit?", "Wrist Vizualizer", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
            if (r != DialogResult.Yes)
                e.Cancel = true;
             */
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
#if DEBUG
            if (System.IO.Directory.Exists(@"L:\Data\CADAVER_WRISTS\Pinned\l\E03274\S15L\IV.files"))
                open.InitialDirectory = @"L:\Data\CADAVER_WRISTS\Pinned\l\E03274\S15L\IV.files";
#endif
            open.Filter = "Compatable Files (*.iv;*.wrl)|*.iv;*.wrl|Inventor Files (*.iv)|*.iv|VRML Files (*.wrl)|*.wrl|All Files (*.*)|*.*";
            open.Multiselect = true;
            if (DialogResult.OK == open.ShowDialog())
            {
                if (open.FileNames.Length == 0) return;

                if (_root == null)
                    _root = new Separator();

                if (_viewer == null)
                {
                    _viewer = new ExaminerViewer((int)panelCoin.Handle);
                    _viewer.setSceneGraph(_root);
                }

                if (open.FileNames.Length == 1)
                {
                    _root.addFile(open.FileName);
                }
                else
                {
                    foreach (string filename in open.FileNames)
                        _root.addFile(filename);
                }

                
            }
        }

        private void seriesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_currentPositionIndex == seriesListBox.SelectedIndex)
                return;

            //check if neutral
            if (seriesListBox.SelectedIndex == 0)
            {
                _currentPositionIndex = 0;
                //do the neutral thing....
                if (_root.hasTransform())
                    _root.removeTransform();
                for (int i = 0; i < _bones.Length; i++)
                    if (_bones[i].hasTransform())
                        _bones[i].removeTransform();
            }
            else
            {
                //int seriesIndex = _wrist.getSeriesIndexFromName((string)seriesListBox.SelectedItem);
                _currentPositionIndex = seriesListBox.SelectedIndex;
                if (_root.hasTransform())
                    _root.removeTransform();
                Transform t = new Transform();
                DatParser.addRTtoTransform(DatParser.parseMotionFile2(_wrist.getMotionFilePath(_currentPositionIndex-1))[0], t);
                t.invert();
                //_root.addTransform(_transforms[_currentPositionIndex-1][0]); //minus 1 to skip neutral
                _root.addTransform(t);
                for (int i = 0; i < _bones.Length; i++)
                {
                    //remove the old
                    if (_bones[i].hasTransform())
                        _bones[i].removeTransform();

                    _bones[i].addTransform(_transforms[_currentPositionIndex-1][i]);
                    if (_transforms[_currentPositionIndex - 1][i].isIdentity())
                        _hideBoxes[i].Checked = true;
                }
            }
        }

        private void linkLabelShowAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            for (int i = 0; i < _hideBoxes.Length; i++ )
                _hideBoxes[i].Checked = false;
        }

        private void linkLabelHideAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            for (int i = 0; i < _hideBoxes.Length; i++ )
                _hideBoxes[i].Checked = true;
        }

        private void saveFrameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.DefaultExt = "jpeg";
            save.AddExtension = true;
            save.Filter = "JPEG Files (*.jpeg;*.jpg)|*.jpeg;*.jpg|PNG Files (*.png)|*.png|All Files (*.*)|*.*";
            save.CheckPathExists = true;
            save.ValidateNames = true;

            if (save.ShowDialog() == DialogResult.Cancel)
                return;

            string fname = save.FileName;
            bool res;
            switch (Path.GetExtension(fname).ToLower())
            {
                case ".png":
                    res = _viewer.saveToPNG(fname);
                    break;
                /* - Unknown error with these filetypes. might need to include simage - http://doc.coin3d.org/Coin/classSoOffscreenRenderer.html#a18
                case ".tiff":
                case ".tif":
                    res = _viewer.saveToTIFF(fname);
                    break;
                case ".gif":
                    res = _viewer.saveToGIF(fname);
                    break;
                case ".bmp":
                    res = _viewer.saveToBMP(fname);
                    break;
                 */
                case ".jpg":
                case ".jpeg":
                default:
                    res = _viewer.saveToJPEG(fname);
                    break;
            }
            if (!res)
                MessageBox.Show("Unknown error saving file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //_viewer.saveToJPEG(@"C:\test.jpg");
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpAbout ha = new HelpAbout();
            ha.ShowDialog();
        }

        private void loadSampleWristToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //string t = Application.ExecutablePath;
            string baseFolder = Path.GetDirectoryName(Application.ExecutablePath);
            string location = Path.Combine(Path.Combine(Path.Combine("Example", "66582"), "LeftIV"), "66582_rad_L.iv");
            string radFile = Path.Combine(baseFolder, location);
            if (!File.Exists(radFile))
            {
                string msg = "Unable to find the sample wrist:\n" + radFile + "\n\n";
                msg += "Please try reinstalling this application to install the sample wrist";
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            string[] files = new string[1];
            files[0] = radFile;
            openFile(files, true);
        }

        private void showInertiasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showInertiasToolStripMenuItem.Checked)
            {
                try
                {
                    //If its checked, then we need to add it
                    TransformMatrix[] inert = DatParser.parseInertiaFile2(_wrist.inertiaFile);
                    for (int i = 2; i < 10; i++) //skip the long bones
                    {
                        if (_bones[i] == null)
                            continue;

                        _inertias[i] = new Separator();
                        Transform t = new Transform();
                        DatParser.addRTtoTransform(inert[i], t);
                        _inertias[i].addNode(new ACS());
                        _inertias[i].addTransform(t);
                        _bones[i].addChild(_inertias[i]);
                    }
                }
                catch (ArgumentException ex)
                {
                    string msg = "Error loading inertia file.\n\n" + ex.Message;
                    MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    showInertiasToolStripMenuItem.Checked = false;
                }
            }
            else
            {
                //so we want to remove the inertia files
                for (int i = 2; i < 10; i++)
                {
                    _bones[i].removeChild(_inertias[i]);
                    _inertias[i] = null;
                }

            }
           
        }

        private void backgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog cg = new ColorDialog();
            int r = _viewer.getBackgroundColorR();
            int g = _viewer.getBackgroundColorG();
            int b = _viewer.getBackgroundColorB();
            cg.Color = Color.FromArgb(r, g, b);
            cg.FullOpen = true;
            if (cg.ShowDialog() == DialogResult.Cancel)
                return;

            _viewer.setBackgroundColor(cg.Color.R, cg.Color.G, cg.Color.B);
        }
    }
}