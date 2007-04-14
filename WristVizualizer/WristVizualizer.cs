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
            _currentPositionIndex = 0;
            setupControlBox();
            showControlBox();
            
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
            _bones = new Separator[15];
            for (int i = 0; i < _bnames.Length; i++)
            {
                _hideBoxes[i].Checked = false;
                _hideBoxes[i].Enabled = true;
            }
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

                if (_viewer == null)
                    _viewer = new ExaminerViewer((int)panelCoin.Handle);

                _root = new Separator();

                bool loadFull = false;

                if (open.FileNames.Length == 1)
                {
                    //check if this is a radius and what we want to do....
                    if (WristHelper.isRadius(open.FileName))
                    {
                        string msg = "It looks like you are trying to open a radius.\n\nDo you wish to load the entire wrist?";
                        if (DialogResult.Yes == MessageBox.Show(msg, "Wrist Vizualizer", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                            //load full wrist
                            loadFull = true;
                    

                    }
                }

                //if we get here, then we are loading a new file....so
                resetForm();

                if (loadFull)
                {

                    showControlBox();
                    loadFullWrist(open.FileName, _root);
                }
                else
                {
                    hideControlBox();
                    foreach (string filename in open.FileNames)
                        _root.addFile(filename);
                }

                _viewer.setSceneGraph(_root);
            }
        }

        private void populateSeriesList()
        {
            if (_wrist != null)
            {
                _currentPositionIndex = 0;
                seriesListBox.Items.Clear();
                //add neutral
                seriesListBox.Items.Add(_wrist.neturalSeries);
                seriesListBox.Items.AddRange(_wrist.series);
                seriesListBox.SelectedIndex = 0;
            }
        }

        private void loadFullWrist(string radius, Separator root)
        {
            string basepath = Path.GetDirectoryName(radius);
            string extension = Path.GetExtension(radius);
            string series = Path.GetFileNameWithoutExtension(radius).Substring(3, 3);

            //Setup motion files, etc
            _wrist = new Wrist(radius);
            _wrist.setupPaths(radius);
            _wrist.findAllSeries();
            _transforms = DatParser.makeAllTransforms(_wrist.motionFiles,_bnames.Length);
            populateSeriesList();




            for (int i = 0; i < _bnames.Length; i++)
            {
                string fname = Path.Combine(basepath,_bnames[i]+series+extension);
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

        private void pos2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_wrist != null)
            {
                libWrist.TransformMatrix[] tfm = DatParser.parseMotionFile2(_wrist.getMotionFilePath(2));
                for (int i = 0; i < _bones.Length; i++)
                {
                    libCoin3D.Transform t = new libCoin3D.Transform();
                    /*
                    t.setRotation(tfm[i].R.Array[0][0], tfm[i].R.Array[0][1], tfm[i].R.Array[0][2],
                        tfm[i].R.Array[1][0], tfm[i].R.Array[1][1], tfm[i].R.Array[1][2],
                        tfm[i].R.Array[2][0], tfm[i].R.Array[2][1], tfm[i].R.Array[2][2]);
                     
                    t.setTranslation(tfm[i].T.Array[0][0], tfm[i].T.Array[0][1], tfm[i].T.Array[0][2]);
                    _bones[i].addTransform(t);
                     */
                    t.setTransform(tfm[i].R.Array[0][0], tfm[i].R.Array[0][1], tfm[i].R.Array[0][2],
                        tfm[i].R.Array[1][0], tfm[i].R.Array[1][1], tfm[i].R.Array[1][2],
                        tfm[i].R.Array[2][0], tfm[i].R.Array[2][1], tfm[i].R.Array[2][2],
                        tfm[i].T.Array[0][0], tfm[i].T.Array[0][1], tfm[i].T.Array[0][2]);
                    _bones[i].addTransform(t);
                }    

                /*
                //do root
                libCoin3D.Transform t = new libCoin3D.Transform();

                t.setRotation(tfm[0].R.Array[0][0], tfm[0].R.Array[0][1], tfm[0].R.Array[0][2],
                    tfm[0].R.Array[1][0], tfm[0].R.Array[1][1], tfm[0].R.Array[1][2],
                    tfm[0].R.Array[2][0], tfm[0].R.Array[2][1], tfm[0].R.Array[2][2]);

                t.setTranslation(tfm[0].T.Array[0][0], tfm[0].T.Array[0][1], tfm[0].T.Array[0][2]);
                
                _bones[i].addTransform(t);
                */
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
    }
}