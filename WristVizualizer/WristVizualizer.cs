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
        private RadioButton[] _fixRadios;
        private Wrist _wrist;
        private Transform[][] _transforms;
        private int _currentPositionIndex;
        private int _fixedBoneIndex;
        private PointSelection _pointSelection;
        private MaterialEditor _materialEditor;
        private string _firstFileName;
        private const string PROGRAM_TITLE = "Wrist Vizualizer";

        public WristVizualizer(string[] fileArgs)
        {
            InitializeComponent();

            _base = new Coin3DBase();
            _viewer = null;
            _root = null;
            _wrist = null;
            _bones = new Separator[15];
            _inertias = new Separator[15];
            _currentPositionIndex = 0;
            _fixedBoneIndex = 0;
            setupControlBox();
            //showControlBox();            

            VersionManager manager = new VersionManager();
            manager.checkForUpdatesAsynch(); //check for updates in the backround

            //if we were passed something
            if (fileArgs != null && fileArgs.Length >= 1)
            {
                openFile(fileArgs);
            }
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

        #region setupMethods

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

            _fixRadios = new RadioButton[15];
            _fixRadios[0] = radioButtonFixedRad;
            _fixRadios[1] = radioButtonFixedUln;
            _fixRadios[2] = radioButtonFixedSca;
            _fixRadios[3] = radioButtonFixedLun;
            _fixRadios[4] = radioButtonFixedTrq;
            _fixRadios[5] = radioButtonFixedPis;
            _fixRadios[6] = radioButtonFixedTpd;
            _fixRadios[7] = radioButtonFixedTpm;
            _fixRadios[8] = radioButtonFixedCap;
            _fixRadios[9] = radioButtonFixedHam;
            _fixRadios[10] = radioButtonFixedMC1;
            _fixRadios[11] = radioButtonFixedMC2;
            _fixRadios[12] = radioButtonFixedMC3;
            _fixRadios[13] = radioButtonFixedMC4;
            _fixRadios[14] = radioButtonFixedMC5;
        }

        private void setupExaminerWindow()
        {
            _viewer = new ExaminerViewer((int)panelCoin.Handle);
            _viewer.OnObjectSelected += new ObjectSelectedHandler(_viewer_OnObjectSelected);
            _viewer.OnObjectDeselected += new ObjectDeselectedHandler(_viewer_OnObjectDeselected);
        }
        #endregion

        private void resetForm()
        {
            this.Text = PROGRAM_TITLE;
            importToolStripMenuItem.Enabled = true;
            backgroundColorToolStripMenuItem.Enabled = true;
            decoratorToolStripMenuItem.Enabled = true;
            saveFrameToolStripMenuItem.Enabled = true;
            showInertiasToolStripMenuItem.Enabled = false;
            showInertiasToolStripMenuItem.Checked = false;
            showACSToolStripMenuItem.Enabled = false;
            showACSToolStripMenuItem.Checked = false;
            showAxesToolStripMenuItem.Enabled = true;
            pointIntersectionToolStripMenuItem.Enabled = true;
            pointIntersectionToolStripMenuItem.Checked = false;
            colorTransparencyToolStripMenuItem.Enabled = false;
            transparencyToolStripMenuItem.Enabled = true;
            viewSourceToolStripMenuItem.Enabled = true;
            saveAsToolStripMenuItem.Enabled = true;
            seriesListBox.Items.Clear();
            if (_pointSelection != null)
            {
                _pointSelection.stopSelecting();
                _pointSelection = null;
            }
            hideStatusStrip();
            toolStripStatusLabel1.Text = "";
            _bones = new Separator[15];
            for (int i = 0; i < _bnames.Length; i++)
            {
                _bones[i] = null;
                _hideBoxes[i].Checked = false;
                _hideBoxes[i].Enabled = true;
                _fixRadios[i].Enabled = true;
            }
            radioButtonFixedRad.Checked = true;
        }





        #region File Open

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
#if DEBUG
            if (System.IO.Directory.Exists(@"L:\Data\CADAVER_WRISTS\Pinned\l\E03274\S15L\IV.files"))
                open.InitialDirectory = @"L:\Data\CADAVER_WRISTS\Pinned\l\E03274\S15L\IV.files";
#endif
            open.Filter = "Compatable Files (*.iv;*.wrl)|*.iv;*.wrl|Inventor Files (*.iv)|*.iv|VRML Files (*.wrl)|*.wrl|All Files (*.*)|*.*";
            open.Multiselect = true;
            if (DialogResult.OK != open.ShowDialog())
                return;

            openFile(open.FileNames);
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
            if (DialogResult.OK != open.ShowDialog())
                return;

            foreach (string filename in open.FileNames)
                _root.addFile(filename);
        }


        /// <summary>
        /// This function will take a file(s) and load them into the scene,
        /// removing whatever was there before (so this is open, not import). 
        /// It will try determine if this is a full wrist or not.
        /// </summary>
        /// <param name="filenames">List of files to open. If only a single file, and
        /// a radius, it will ask and then try to open the full wrist</param>
        private void openFile(string[] filenames)
        {
            if (filenames.Length == 0) return;

            bool loadFull = false;
            //check if this is a radius and what we want to do....
            if (Wrist.isRadius(filenames))
            {
                string msg = "It looks like you are trying to open a radius.\n\nDo you wish to load the entire wrist?";
                if (DialogResult.Yes == MessageBox.Show(msg, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    //load full wrist
                    loadFull = true;
            }
            openFile(filenames, loadFull);
        }

        /// <summary>
        /// This function will take a file(s) and load them into the scene,
        /// removing whatever was there before (so this is open, not import). 
        /// It will try and load a full wrist if specified, or load the files as
        /// individual objects
        /// </summary>
        /// <param name="filenames">List of files to open</param>
        /// <param name="loadFull">Whether to treat the files as a full wrist. If so
        /// only the first string in filenames is looked at, and it is assumed to be
        /// the radius.</param>
        private void openFile(string[] filenames, bool loadFull)
        {
            if (_viewer == null)
                setupExaminerWindow();


            //if we get here, then we are loading a new file....so
            resetForm();
            _root = new Separator(); //setup new node (might need to change that...

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

                //save first filename for recording sake
                _firstFileName = filenames[0];

                //set title
                if (filenames.Length == 1)
                    this.Text = PROGRAM_TITLE + " - " + filenames[0];
            }

            _viewer.setSceneGraph(_root);
        }


        private void openFullWristToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
#if DEBUG
            if (System.IO.Directory.Exists(@"L:\Data\NIH_Phase_I\Normal_Male\E17172"))
#endif
                folder.SelectedPath = @"L:\Data\NIH_Phase_I\Normal_Male\E17172";
            folder.ShowNewFolderButton=false;
            folder.Description = "Select the subject folder to open";
            if (DialogResult.OK == folder.ShowDialog())
            {
                try
                {
                    string left = Wrist.findLeftRadius(folder.SelectedPath);
                    string right = Wrist.findRightRadius(folder.SelectedPath);
                    if (left.Length == 0 && right.Length == 0)
                        throw new ArgumentException("No wrist found.");

                    string[] files = new string[1];
                    if (left.Length == 0) // so only a right
                        files[0] = right;
                    else if (right.Length == 0) //so only a left
                        files[0] = left;
                    else
                    {
                        // have both a left and a right, need to ask the user what to open.
                        SideSelector s = new SideSelector();
                        s.ShowDialog();
                        if (s.results == SideSelector.SideResult.LEFT)
                            files[0] = left;
                        else if (s.results == SideSelector.SideResult.RIGHT)
                            files[0] = right;
                        else
                            return;
                    }
                    openFile(files, true);
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show("Error opening wrist. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
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
            showACSToolStripMenuItem.Enabled = true;

            //Setup motion files, etc

            _wrist = new Wrist();
            try
            {
                _wrist.setupWrist(radius);
                _transforms = DatParser.makeAllTransforms(_wrist.motionFiles, _bnames.Length);
                populateSeriesList();
            }
            catch (ArgumentException ex)
            {
                if (!hideErrorMessagesToolStripMenuItem.Checked)
                {
                    string msg = "Error loading wrist kinematics.\n\n" + ex.Message;
                    //TODO: Change to abort,retry, and find way of cancelling load
                    MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                for (int i = 0; i < _bnames.Length; i++)
                    _fixRadios[i].Enabled = false;
            }
            

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
                    _bones[i] = null;
                    _hideBoxes[i].Enabled = false;
                    _fixRadios[i].Enabled = false;
                }
            }

            //set title bar now
            this.Text = PROGRAM_TITLE + " - " + _wrist.subject + _wrist.side + " - " + _wrist.subjectPath;
            _firstFileName = Path.Combine(_wrist.subjectPath, _wrist.subject + _wrist.side);
        }



        #endregion

        private void loadSampleWristToolStripMenuItem_Click(object sender, EventArgs e)
        {
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

        #region Special Code for manipulating FullWrists
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

        private void linkLabelShowAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            for (int i = 0; i < _hideBoxes.Length; i++)
                _hideBoxes[i].Checked = false;
        }

        private void linkLabelHideAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            for (int i = 0; i < _hideBoxes.Length; i++)
                _hideBoxes[i].Checked = true;
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
                {
                    if (_bones[i] == null) continue; //skip missing bone

                    if (_bones[i].hasTransform())
                        _bones[i].removeTransform();
                }
            }
            else
            {
                //int seriesIndex = _wrist.getSeriesIndexFromName((string)seriesListBox.SelectedItem);
                _currentPositionIndex = seriesListBox.SelectedIndex;
                if (_root.hasTransform())
                    _root.removeTransform();
                Transform t = new Transform();
                //TODO: Fix so this doesn't have to re-parse the motion file from disk each time...
                DatParser.addRTtoTransform(DatParser.parseMotionFile2(_wrist.getMotionFilePath(_currentPositionIndex - 1))[_fixedBoneIndex], t);
                t.invert();
                //_root.addTransform(_transforms[_currentPositionIndex-1][0]); //minus 1 to skip neutral
                _root.addTransform(t);
                for (int i = 0; i < _bones.Length; i++)
                {
                    //skip missing bones
                    if (_bones[i] == null) continue;

                    //remove the old
                    if (_bones[i].hasTransform())
                        _bones[i].removeTransform();

                    _bones[i].addTransform(_transforms[_currentPositionIndex-1][i]);
                    if (_transforms[_currentPositionIndex - 1][i].isIdentity())
                        _hideBoxes[i].Checked = true;
                }
            }
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

        private void showACSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showACSToolStripMenuItem.Checked)
            {
                try
                {
                    //If its checked, then we need to add it
                    TransformMatrix[] inert = DatParser.parseACSFile2(_wrist.acsFile);

                    //only for radius, check if it exists
                    if (_bones[0] == null)
                        return;

                    _inertias[0] = new Separator();
                    Transform t = new Transform();
                    DatParser.addRTtoTransform(inert[0], t);
                    _inertias[0].addNode(new ACS(45)); //longer axes for the radius/ACS
                    _inertias[0].addTransform(t);
                    _bones[0].addChild(_inertias[0]);
                }
                catch (ArgumentException ex)
                {
                    string msg = "Error loading ACS file.\n\n" + ex.Message;
                    MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    showACSToolStripMenuItem.Checked = false;
                }
            }
            else
            {
                //so we want to remove the inertia files
                _bones[0].removeChild(_inertias[0]);
                _inertias[0] = null;
            }
        }



        private void radioButtonFixed_CheckedChanged(object sender, EventArgs e)
        {
            /* This method will get called twice on a change, once when checked, 
             * and once when unchecked. To prevent problems, we will ignore 
             * events for the button that was unchecked, and deal only with
             * events for the check event
             */
            RadioButton b = (RadioButton)sender;
            if (!b.Checked) return; //only want to deal with the checked button

            //figure out what box this is
            for (int i = 0; i < _bones.Length; i++)
            {
                if (_fixRadios[i] == sender && _bones[i] != null)
                {
                    _fixedBoneIndex = i;

                    //do nothing for neutral
                    if (_currentPositionIndex == 0) return;

                    //so now change the top level
                    //do the neutral thing....
                    if (_root.hasTransform())
                        _root.removeTransform();
                    
                    Transform t = new Transform();
                    DatParser.addRTtoTransform(DatParser.parseMotionFile2(_wrist.getMotionFilePath(_currentPositionIndex - 1))[i], t);
                    t.invert();
                    //_root.addTransform(_transforms[_currentPositionIndex-1][0]); //minus 1 to skip neutral
                    _root.addTransform(t);

                }
            }
        }
        #endregion

        /// <summary>
        /// Get the Root Seperator node
        /// </summary>
        public Separator Root
        {
            get { return _root; }
        }

        #region Callbacks for Menu Items
        void decoratorToolStripMenuItem_CheckedChanged(object sender, System.EventArgs e)
        {
            if (_viewer != null)
                _viewer.setDecorator(decoratorToolStripMenuItem.Checked);
        }

        private void showAxesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _viewer.setFeedbackVisibility(showAxesToolStripMenuItem.Checked);
        }

        private void backgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog cg = new ColorDialog();
            int oldc = _viewer.getBackgroundColor();
            oldc = (oldc >> 8);
            cg.Color = Color.FromArgb(oldc);

            cg.FullOpen = true;
            if (cg.ShowDialog() == DialogResult.Cancel)
                return;

            int col = cg.Color.ToArgb();
            //bit correction needed to move from 0xAARRGGBB -> 0xRRGGBBAA
            col = (col << 8) | 0x000000FF;
            _viewer.setBackgroundColor(col);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpAbout ha = new HelpAbout();
            ha.ShowDialog();
        }

        private void checkForupdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VersionManager manager = new VersionManager();
            manager.checkForUpdates();
        }

        private void viewSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_viewer == null || _firstFileName == null || _firstFileName.Length == 0)
                return;

            //launch in wordpad
            const string wordpadLocation = @"C:\Program Files\Windows NT\Accessories\wordpad.exe";
            System.Diagnostics.Process.Start(wordpadLocation,String.Format("\"{0}\"",_firstFileName));

        }

        #region Saving/Output
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

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_viewer == null) return;
            SaveFileDialog save = new SaveFileDialog();
            if (Directory.Exists(Path.GetDirectoryName(_firstFileName)))
                save.InitialDirectory = Path.GetDirectoryName(_firstFileName);
            save.Filter = "Inventor Files (*.iv)|*.iv|All Files (*.*)|*.*";
            save.OverwritePrompt = true;
            save.AddExtension = true;
            save.DefaultExt = ".iv";
            if (DialogResult.OK != save.ShowDialog())
                return;

            try
            {
                //save file
                string filename = save.FileName;
                _viewer.saveSceneGraph(filename);
            }
            catch (ApplicationException ex)
            {
                string msg = "Error: " + ex;
                MessageBox.Show(msg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion
        #endregion

        #region Drag and Drop
        private void WristVizualizer_DragDrop(object sender, DragEventArgs e)
        {
            new System.Security.Permissions.FileIOPermission(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            string[] filenames = e.Data.GetData("FileDrop") as string[];
            openFile(filenames);
            /*
            foreach (string fileListItem in (e.Data.GetData("FileDrop") as string[]))
            {
                Console.WriteLine(fileListItem);
            }
            */
            System.Security.CodeAccessPermission.RevertAssert();
        }

        private void WristVizualizer_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("FileDrop"))
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }

        private void panelCoin_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("FileDrop"))
            {
                if (importToolStripMenuItem.Enabled)
                    e.Effect = DragDropEffects.Copy;
                else
                    e.Effect = DragDropEffects.Move;
            }
            else
                e.Effect = DragDropEffects.None;
        }

        private void panelCoin_DragDrop(object sender, DragEventArgs e)
        {
            new System.Security.Permissions.FileIOPermission(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            string[] filenames = e.Data.GetData("FileDrop") as string[];
            if (importToolStripMenuItem.Enabled && _viewer != null & _root != null)
            {
                foreach (string file in filenames)
                {
                    _root.addFile(file);
                }
            }
            else
                openFile(filenames);
            System.Security.CodeAccessPermission.RevertAssert();
        }
        #endregion

        #region Point Selection
        private void pointIntersectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_viewer == null)  //can't do anything
                return;
            
            if (pointIntersectionToolStripMenuItem.Checked == true) //it was already running
            {
                pointIntersectionToolStripMenuItem.Checked = false;
                _pointSelection.stopSelecting();
                _pointSelection = null;
                hideStatusStrip();
            }
            else
            {
                PointSelection ps = new PointSelection(_viewer, this, _firstFileName);
                ps.ShowDialog();
                if (!ps.SelectionEnabled) return; //if we were canceled, etc.

                pointIntersectionToolStripMenuItem.Checked = true;
                _pointSelection = ps;
                if (ps.DisplayInStatusBar)
                    showStatusStrip();
            }
        }

        private void showStatusStrip()
        {
            if (statusStrip1.Visible)
                return;

            statusStrip1.Visible = true;
            panelCoin.Height -= 22; //22 is the height of the statusStrip
        }

        private void hideStatusStrip()
        {
            if (!statusStrip1.Visible)
                return;

            statusStrip1.Visible = false;
            panelCoin.Height += 22;
        }

        public void setStatusStripText(string text)
        {
            toolStripStatusLabel1.Text = text;
        }
        #endregion

        #region Transparency Adjustments
        private void transparencyToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (_viewer == null) return;
            ExaminerViewer.TransparencyTypes t = _viewer.getTransparencyType();
            screenDoorToolStripMenuItem.Checked = (t == ExaminerViewer.TransparencyTypes.SCREEN_DOOR);
            addToolStripMenuItem.Checked = (t == ExaminerViewer.TransparencyTypes.ADD);
            delayedAddToolStripMenuItem.Checked = (t == ExaminerViewer.TransparencyTypes.DELAYED_ADD);
            sortedObjectAddToolStripMenuItem.Checked = (t == ExaminerViewer.TransparencyTypes.SORTED_OBJECT_ADD);
            blendToolStripMenuItem.Checked = (t == ExaminerViewer.TransparencyTypes.BLEND);
            delayedBlendToolStripMenuItem.Checked = (t == ExaminerViewer.TransparencyTypes.DELAYED_BLEND);
            sortedObjectBlendToolStripMenuItem.Checked = (t == ExaminerViewer.TransparencyTypes.SORTED_OBJECT_BLEND);
            sortedObjectSortedTriangleAddToolStripMenuItem.Checked = (t == ExaminerViewer.TransparencyTypes.SORTED_OBJECT_SORTED_TRIANGLE_ADD);
            sortedObjectSortedTriangleBlendToolStripMenuItem.Checked = (t == ExaminerViewer.TransparencyTypes.SORTED_OBJECT_SORTED_TRIANGLE_BLEND);
        }

        private void allTransparencyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_viewer == null) return;
            ExaminerViewer.TransparencyTypes t = ExaminerViewer.TransparencyTypes.SCREEN_DOOR;
            if (screenDoorToolStripMenuItem == sender) t = ExaminerViewer.TransparencyTypes.SCREEN_DOOR;
            if (addToolStripMenuItem == sender) t = ExaminerViewer.TransparencyTypes.ADD;
            if (delayedAddToolStripMenuItem == sender) t = ExaminerViewer.TransparencyTypes.DELAYED_ADD;
            if (sortedObjectAddToolStripMenuItem == sender) t = ExaminerViewer.TransparencyTypes.SORTED_OBJECT_ADD;
            if (blendToolStripMenuItem == sender) t = ExaminerViewer.TransparencyTypes.BLEND;
            if (delayedBlendToolStripMenuItem == sender) t = ExaminerViewer.TransparencyTypes.DELAYED_BLEND;
            if (sortedObjectBlendToolStripMenuItem == sender) t = ExaminerViewer.TransparencyTypes.SORTED_OBJECT_BLEND;
            if (sortedObjectSortedTriangleAddToolStripMenuItem == sender) t = ExaminerViewer.TransparencyTypes.SORTED_OBJECT_SORTED_TRIANGLE_ADD;
            if (sortedObjectSortedTriangleBlendToolStripMenuItem == sender) t = ExaminerViewer.TransparencyTypes.SORTED_OBJECT_SORTED_TRIANGLE_BLEND;
            _viewer.setTransparencyType(t);
        }
        #endregion

        #region Material Editing
        void _viewer_OnObjectDeselected()
        {
            colorTransparencyToolStripMenuItem.Enabled = false;

            //if the material is being edited, then we need to shut it down.
            if (_materialEditor != null && _materialEditor.Visible)
            {
                _materialEditor.materialDeselected();
            }
        }

        void _viewer_OnObjectSelected()
        {
            colorTransparencyToolStripMenuItem.Enabled = true;
        }

        private void colorTransparencyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_materialEditor != null && _materialEditor.Visible)
            {
                //then we should simply show it
                _materialEditor.Activate();
                return;
            }
            //setup new editor
            _materialEditor = new MaterialEditor(_viewer);
            _materialEditor.Show();
        }
        #endregion


    }
}