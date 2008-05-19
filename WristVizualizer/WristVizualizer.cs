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
        
        private Modes _mode = Modes.NONE;

        private Controller _currentController;

        private PosViewController _posViewController;
        private FullWristController _fullWristController;

        private PointSelection _pointSelection;
        private MaterialEditor _materialEditor;
        private string _firstFileName;
        private int _numberFilesLoaded;

        private FileSystemWatcher _fileSysWatcher;
        private string _fileNameOfChangedFile;

        private enum Modes
        {
            NONE,
            SCENEVIEWER,
            FULL_WRIST,
            POSVIEW,
            TEXTURE,
            DISTV
        }

        public WristVizualizer(string[] fileArgs)
        {
            InitializeComponent();

            _base = new Coin3DBase();
            _viewer = null;
            _root = null;
            
            VersionManager manager = new VersionManager();
            manager.checkForUpdatesAsynch(); //check for updates in the backround

            try
            {
                //if we were passed something
                if (fileArgs != null && fileArgs.Length == 1 &&
                    Path.GetExtension(fileArgs[0]).ToLower().Equals("pos"))
                {
                    //check for being passed a single pos view file
                    loadPosView(fileArgs[0]);
                }
                else if (fileArgs != null && fileArgs.Length >= 1)
                {
                    openFile(fileArgs);
                }
            }
            catch (Exception ex)
            {
                string msg = "Error loading file:\n" + ex.Message;
                MessageBox.Show(msg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        #region Control Visibility
        private void addControlBox(Control Control)
        {
            //do I need to check for an existing control first... hopefully not
            int startHeight = mainLayoutPanel.Height;
            mainLayoutPanel.Controls.Add(Control, 1, 0);

            if (Control.PreferredSize.Height > startHeight)
            {
                //we should expand the size of the main form....
                this.Height += Control.PreferredSize.Height - startHeight;
            }
        }

        private void removeControlBox(Control Control)
        {
            if (mainLayoutPanel.Contains(Control))
                mainLayoutPanel.Controls.Remove(Control);
        }
        #endregion

        [Obsolete("use setFormForMode(Modes mode) which will both save the mode, and then apply the settings")]
        private void setFormForCurrentMode()
        {
            setFormForMode(_mode);
        }
        private void setFormForMode(Modes mode)
        {
            _mode = mode; //update current mode

            if (_currentController != null && _currentController.Control != null)
                addControlBox(_currentController.Control);

            switch (mode)
            {
                case Modes.POSVIEW:
                    importToolStripMenuItem.Enabled = false;
                    saveMovieToolStripMenuItem.Visible = true;
                    saveMovieToolStripMenuItem.Enabled = true;
                    break;
                case Modes.FULL_WRIST:
                    animatePositionTransitionsToolStripMenuItem.Enabled = true;
                    animationRateToolStripMenuItem.Enabled = true;
                    calculateDistanceMapToolStripMenuItem.Enabled = true;
                    break;
            }
        }

        #region setupMethods
        private void setupExaminerWindow()
        {
            _viewer = new ExaminerViewer((int)panelCoin.Handle);
            _viewer.OnObjectSelected += new ObjectSelectedHandler(_viewer_OnObjectSelected);
            _viewer.OnObjectDeselected += new ObjectDeselectedHandler(_viewer_OnObjectDeselected);
        }
        #endregion

        private void resetExaminerViewer()
        {
            _viewer.setBackgroundColor(0f, 0f, 0f);
        }

        private void resetForm()
        {
            if (_viewer == null)
                setupExaminerWindow();

            this.Text = Application.ProductName;
            if (_currentController != null)
            {
                //first remove the control, if it exists
                if (_currentController.Control != null)
                {
                    removeControlBox(_currentController.Control);
                }
                _currentController.CleanUp();
                _currentController = null;
            }

            _posViewController = null;
            _fullWristController = null;

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
            showScenegraphToolStripMenuItem.Enabled = true;
            saveMovieToolStripMenuItem.Enabled = false;
            animatePositionTransitionsToolStripMenuItem.Checked = false; //default value?
            animatePositionTransitionsToolStripMenuItem.Enabled = false;
            animationRateToolStripMenuItem.Enabled = false;
            rate_05sec_15FpsToolStripMenuItem.Checked = true;
            rate_1sec_15FpsToolStripMenuItem1.Checked = false;
            rate_2sec_15FpsToolStripMenuItem2.Checked = false;
            calculateDistanceMapToolStripMenuItem.Enabled = false;
            
            if (_pointSelection != null)
            {
                _pointSelection.stopSelecting();
                _pointSelection = null;
            }

            hideStatusStrip();
            toolStripStatusLabel1.Text = "";

            resetExaminerViewer();
        }

        #region File Open

        /// <summary>
        /// Presents the user with an OpenFileDialog and gets the list of files to open
        /// </summary>
        /// <returns>List of files to open, or null if the user cancels</returns>
        private string[] getFilesToOpen()
        {
            OpenFileDialog open = new OpenFileDialog();
#if DEBUG
            if (System.IO.Directory.Exists(@"L:\Data\CADAVER_WRISTS\Pinned\l\E03274\S15L\IV.files"))
                open.InitialDirectory = @"L:\Data\CADAVER_WRISTS\Pinned\l\E03274\S15L\IV.files";
#endif
            open.Filter = "Compatable Files (*.iv;*.wrl)|*.iv;*.wrl|Inventor Files (*.iv)|*.iv|VRML Files (*.wrl)|*.wrl|All Files (*.*)|*.*";
            open.Multiselect = true;
            if (DialogResult.OK != open.ShowDialog())
                return null;

            return open.FileNames;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] filenames = getFilesToOpen();
            if (filenames == null) return;

            openFile(filenames);
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] filenames = getFilesToOpen();
            if (filenames == null) return;

            importFile(filenames);
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
            if (_viewer == null) //should only happen for the first file opened
                setupExaminerWindow();


            //if we get here, then we are loading a new file....so
            resetForm();
            _root = new Separator(); //setup new node (might need to change that...

            if (loadFull)
            {
                loadFullWrist(filenames[0], _root);
            }
            else
            {
                _mode = Modes.SCENEVIEWER;
                foreach (string filename in filenames)
                    _root.addFile(filename);

                //save first filename for recording sake
                if (filenames.Length >= 1)
                    _firstFileName = filenames[0];
                _numberFilesLoaded = filenames.Length;

                //setup watching
                if (filenames.Length == 1)
                    startWatchingFile(filenames[0]);

                //set title
                if (filenames.Length == 1)
                    this.Text = Application.ProductName + " - " + filenames[0];
            }

            _viewer.setSceneGraph(_root);
        }

        /// <summary>
        /// Adds the given files to the scene
        /// </summary>
        /// <param name="filenames">list of files to add</param>
        private void importFile(string[] filenames)
        {
            if (filenames == null) return;
            _numberFilesLoaded += filenames.Length;
            foreach (string filename in filenames)
                _root.addFile(filename);
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
            //block importing a file
            importToolStripMenuItem.Enabled = false;
            viewSourceToolStripMenuItem.Enabled = false;
            showInertiasToolStripMenuItem.Enabled = true;
            showACSToolStripMenuItem.Enabled = true;

            //Setup motion files, etc
            _fullWristController = new FullWristController();
            _fullWristController.loadFullWrist(radius);
            _currentController = _fullWristController;
            _root = _fullWristController.Root;
            _viewer.setSceneGraph(_root);

            setFormForMode(Modes.FULL_WRIST);

            //set title bar now
            this.Text = Application.ProductName + " - " + _fullWristController.getTitleCaption();
            _firstFileName = _fullWristController.getFilenameOfFirstFile();
        }
        #endregion


        #region PosView

        private void loadPosView(string posViewFilename)
        {
            resetForm();
            _posViewController = new PosViewController(posViewFilename, _viewer);
            _currentController = _posViewController;
            _root = _posViewController.Root; //save local copy also
            setFormForMode(Modes.POSVIEW);
            //_viewer.setDrawStyle(); //attempt to fix rendering problem....fuck
        }

        #endregion


        private void loadSampleWristToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string baseFolder = Path.GetDirectoryName(Application.ExecutablePath);
            string location = Path.Combine(Path.Combine(Path.Combine("Example", "66582"), "LeftIV"), "66582_rad_L.iv");
            string radFile = Path.Combine(baseFolder, location);
#if DEBUG
            if (!File.Exists(radFile))
            {
                //second try here for the setup path
                // start in Vizualizer\WristVizualizer\bin\Deubg, so move up to Vizualizer
                string vizPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(baseFolder)));
                radFile = Path.Combine(vizPath, @"SetupWristVizualizer\Sample Wrist\66582\LeftIV\66582_rad_L.iv");
            }
#endif
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

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFile(new string[0], false);
            importToolStripMenuItem.Enabled = false; //disable importing
            _firstFileName = "";
            this.Text = Application.ProductName;
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

        private void showScenegraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_viewer == null || _root == null)
                return;
            ScenegraphTreeViewer viewer = new ScenegraphTreeViewer(_viewer, _root);
            viewer.Show();
        }

        private void openPosViewFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "PosView Files (*.pos)|*.pos|All Files (*.*)|*.*";
            open.Multiselect = false;
            if (DialogResult.OK != open.ShowDialog())
                return;

            string fname = open.FileName;
            loadPosView(fname);
        }

        private void saveMovieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _posViewController.saveToMovie();
        }

        private void calculateInertiasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Separator s = _viewer.getSecondSeparatorForSelection();
            if (s == null)
                return;

            TessellatedSurface ts = s.findTeselatedSurface();
            if (ts == null)
                return;

            InertialProperties ip = new InertialProperties(ts.Points, ts.Connections);
            TransformRT tfrmMatrix = new TransformRT();
            tfrmMatrix.R = ip.EigenVectors;
            tfrmMatrix.T = new DotNetMatrix.GeneralMatrix(ip.Centroid, 1);

            //create separator for inertial axes
            Separator axesSeparator = new Separator();
            Transform tfrm = new Transform();
            DatParser.addRTtoTransform(tfrmMatrix, tfrm);
            axesSeparator.addNode(tfrm);
            axesSeparator.addNode(new ACS());

            s.insertNode(axesSeparator, 0);
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
            try
            {
                openFile(filenames);
            }
            catch (Exception ex)
            {
                string msg = "Error loading file(s):\n" + ex.Message;
                MessageBox.Show(msg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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
            try
            {
                if (importToolStripMenuItem.Enabled && _viewer != null && _root != null)
                {
                    importFile(filenames); 
                }
                else
                    openFile(filenames);
            }
            catch (Exception ex)
            {
                string msg = "Error loading file(s):\n" + ex.Message;
                MessageBox.Show(msg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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
            calculateInertiasToolStripMenuItem.Enabled = false;

            //if the material is being edited, then we need to shut it down.
            if (_materialEditor != null && _materialEditor.Visible)
            {
                _materialEditor.materialDeselected();
            }
        }

        void _viewer_OnObjectSelected()
        {
            colorTransparencyToolStripMenuItem.Enabled = true;
            calculateInertiasToolStripMenuItem.Enabled = true;
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

        #region FileWatching
        private void startWatchingFile(string filename)
        {
            if (_fileSysWatcher == null)
            {
                _fileSysWatcher = new FileSystemWatcher();
                _fileSysWatcher.IncludeSubdirectories = false;
                _fileSysWatcher.Changed += new FileSystemEventHandler(_fileSysWatcher_Changed);
            }
            //setup watching
            _fileSysWatcher.Path = Path.GetDirectoryName(filename);
            _fileSysWatcher.Filter = Path.GetFileName(filename);
            _fileSysWatcher.EnableRaisingEvents = true;
        }

        private void stopWatchingAllFiles()
        {
            if (_fileSysWatcher == null) return;
            _fileSysWatcher.Changed -= new FileSystemEventHandler(_fileSysWatcher_Changed);
            _fileSysWatcher.EnableRaisingEvents = false;
            _fileSysWatcher.Dispose(); //kill it!
            _fileSysWatcher = null;
        }

        void _fileSysWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            //check if we are somehow watching a different file, or if there is more then one
            //file loaded that we should be watching
            if (!Path.Equals(_firstFileName.ToLower(), e.FullPath.ToLower()) || _numberFilesLoaded != 1)
            {
                _fileSysWatcher.EnableRaisingEvents = false; //stop watching and exit
                return;
            }
            _fileNameOfChangedFile = e.FullPath;            
        }

        private void WristVizualizer_Activated(object sender, EventArgs e)
        {
            if (_fileNameOfChangedFile == null) return;
            //now something exists, save local copy, and clear global
            string fname = _fileNameOfChangedFile;
            _fileNameOfChangedFile = null;

            //check that we are dealing with the same file and again, that there is only one file loaded
            if (!Path.Equals(_firstFileName.ToLower(), fname.ToLower()) || _numberFilesLoaded != 1)
                return;  //if not, get out

            //at this point, we should tell the user that a new file was loaded, and let them deal with it
            string msg = String.Format("{0} has been updated outside of {1}.\n\nDo you wish to reload the file?", fname, Application.ProductName);
            DialogResult r = MessageBox.Show(msg, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (r == DialogResult.Yes)
                openFile(new string[] { _firstFileName }, false);
        }

        #endregion

        private void openTextureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadTextureDialog texture = new LoadTextureDialog();
            DialogResult r = texture.ShowDialog();
            if (r == DialogResult.Cancel)
                return;
            try
            {
                this.Cursor = Cursors.WaitCursor;
                resetForm();

                TextureController controller = texture.setup(_viewer);
                _currentController = controller;
                _root = controller.Root;  //save root;
                setFormForMode(Modes.TEXTURE);

                _viewer.disableSelection();
                _viewer.setBackgroundColor(0.8f, 0.8f, 0.8f);
                this.Text = Application.ProductName + " - " + texture.DisplayTitle;
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void launchMRIViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MRIViewer.MRIViewerMainForm viewer = new MRIViewer.MRIViewerMainForm();
            viewer.Show();
        }

        private void showInertiasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _fullWristController.setInertiaVisibility(showInertiasToolStripMenuItem.Checked);
        }

        private void showACSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _fullWristController.setACSVisibility(showACSToolStripMenuItem.Checked);
        }

        private void loadDistvToolStripMenuItem_Click(object sender, EventArgs e)
        {
            resetForm();
            DistvController distv = null;
            string basePath = @"P:\WORKING_OI_CODE\distv\data";
#if DEBUG
            if (!Directory.Exists(basePath))
            {
                string vizPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Application.ExecutablePath))));
                basePath = Path.Combine(vizPath, @"SetupWristVizualizer\Sample DistV Data\"); //doesn't yet exist :)
            }
#endif
            try
            {
                distv = new DistvController(basePath);
            }
            catch
            {
                string msg = "Unable to load the Distv data.\n\n";
                msg += "You must be connected to the private network to access these files, sorry.";
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            _currentController = distv;
            _root = distv.Root; //save local copy also
            _viewer.setSceneGraph(_root);
            setFormForMode(Modes.DISTV);
        }

        private void animatePositionTransitionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //check that we are in FullWristMode
            if (_mode != Modes.FULL_WRIST || _currentController == null ||
                !_currentController.GetType().Equals(typeof(FullWristController)))
                return;

            FullWristController control = (FullWristController)_currentController;
            control.AnimatePositionTransitions = animatePositionTransitionsToolStripMenuItem.Checked;
        }

        private void animationRateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //check that we are in FullWristMode
            if (_mode != Modes.FULL_WRIST || _currentController == null ||
                !_currentController.GetType().Equals(typeof(FullWristController)))
                return;

            FullWristController control = (FullWristController)_currentController;

            //let go through each one, and proccess
            if (sender == rate_05sec_15FpsToolStripMenuItem)
            {
                control.FPS = 15;
                control.AnimationDuration = 0.5;
            }
            else
            {
                rate_05sec_15FpsToolStripMenuItem.Checked = false;
            }

            if (sender == rate_1sec_15FpsToolStripMenuItem1)
            {
                control.FPS = 15;
                control.AnimationDuration = 1.0;
            }
            else
            {
                rate_1sec_15FpsToolStripMenuItem1.Checked = false;
            }

            if (sender == rate_2sec_15FpsToolStripMenuItem2)
            {
                control.FPS = 15;
                control.AnimationDuration = 2.0;
            }
            else
            {
                rate_2sec_15FpsToolStripMenuItem2.Checked = false;
            }
        }

        private void calculateDistanceMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //check that we are in FullWristMode
            if (_mode != Modes.FULL_WRIST || _currentController == null ||
                !_currentController.GetType().Equals(typeof(FullWristController)))
                return;

            FullWristController control = (FullWristController)_currentController;
            control.calculateDistanceMapsToolClickedHandler();
        }
    }
}