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
            Coin3DBase.Init();
//#if DEBUG
//            if (System.Diagnostics.Debugger.IsAttached)
//            {
//                //fileArgs = new string[] { @"-s:F:\LocalCopies\Functional\Cleaned Subjects For Vizual\asdf\E02862", "--side:R", "--testbone:sca", "--refbone:rad", "--poslist:02R", "-c:1.0,1.5", "-f:rad", @"--saveArea:F:\LocalCopies\Functional\Cleaned Subjects For Vizual\asdf\E02862" };
//                //fileArgs = new string[] { @"-s:P:\Data\Functional_Wrist\E01424", "--side:R", "--testbone:lun", "--refbone:rad", "--poslist:15R,02R,03R,04R,05R,06R,07R,08R,09R,10R", "-c:0.000000,0.010000,0.020000,0.030000,0.040000,0.050000,0.060000,0.070000,0.080000,0.090000,0.100000,0.110000,0.120000,0.130000,0.140000,0.150000,0.160000,0.170000,0.180000,0.190000,0.200000,0.210000,0.220000,0.230000,0.240000,0.250000,0.260000,0.270000,0.280000,0.290000,0.300000,0.310000,0.320000,0.330000,0.340000,0.350000,0.360000,0.370000,0.380000,0.390000,0.400000,0.410000,0.420000,0.430000,0.440000,0.450000,0.460000,0.470000,0.480000,0.490000,0.500000,0.510000,0.520000,0.530000,0.540000,0.550000,0.560000,0.570000,0.580000,0.590000,0.600000,0.610000,0.620000,0.630000,0.640000,0.650000,0.660000,0.670000,0.680000,0.690000,0.700000,0.710000,0.720000,0.730000,0.740000,0.750000,0.760000,0.770000,0.780000,0.790000,0.800000,0.810000,0.820000,0.830000,0.840000,0.850000,0.860000,0.870000,0.880000,0.890000,0.900000,0.910000,0.920000,0.930000,0.940000,0.950000,0.960000,0.970000,0.980000,0.990000,1.000000,1.010000,1.020000,1.030000,1.040000,1.050000,1.060000,1.070000,1.080000,1.090000,1.100000,1.110000,1.120000,1.130000,1.140000,1.150000,1.160000,1.170000,1.180000,1.190000,1.200000,1.210000,1.220000,1.230000,1.240000,1.250000,1.260000,1.270000,1.280000,1.290000,1.300000,1.310000,1.320000,1.330000,1.340000,1.350000,1.360000,1.370000,1.380000,1.390000,1.400000,1.410000,1.420000,1.430000,1.440000,1.450000,1.460000,1.470000,1.480000,1.490000,1.500000,1.510000,1.520000,1.530000,1.540000,1.550000,1.560000,1.570000,1.580000,1.590000,1.600000,1.610000,1.620000,1.630000,1.640000,1.650000,1.660000,1.670000,1.680000,1.690000,1.700000,1.710000,1.720000,1.730000,1.740000,1.750000,1.760000,1.770000,1.780000,1.790000,1.800000,1.810000,1.820000,1.830000,1.840000,1.850000,1.860000,1.870000,1.880000,1.890000,1.900000,1.910000,1.920000,1.930000,1.940000,1.950000,1.960000,1.970000,1.980000,1.990000,2.000000,2.010000,2.020000,2.030000,2.040000,2.050000,2.060000,2.070000,2.080000,2.090000,2.100000,2.110000,2.120000,2.130000,2.140000,2.150000,2.160000,2.170000,2.180000,2.190000,2.200000,2.210000,2.220000,2.230000,2.240000,2.250000,2.260000,2.270000,2.280000,2.290000,2.300000,2.310000,2.320000,2.330000,2.340000,2.350000,2.360000,2.370000,2.380000,2.390000,2.400000,2.410000,2.420000,2.430000,2.440000,2.450000,2.460000,2.470000,2.480000,2.490000,2.500000,2.510000,2.520000,2.530000,2.540000,2.550000,2.560000,2.570000,2.580000,2.590000,2.600000,2.610000,2.620000,2.630000,2.640000,2.650000,2.660000,2.670000,2.680000,2.690000,2.700000,2.710000,2.720000,2.730000,2.740000,2.750000,2.760000,2.770000,2.780000,2.790000,2.800000,2.810000,2.820000,2.830000,2.840000,2.850000,2.860000,2.870000,2.880000,2.890000,2.900000,2.910000,2.920000,2.930000,2.940000,2.950000,2.960000,2.970000,2.980000,2.990000,3.000000", @"--saveArea:P:\Data\Functional_Wrist\Results\ContourArea\" };
//                //fileArgs = new string[] { @"--side:r", "--saveArea: " };
//                fileArgs = new string[] { @"-s:F:\LocalCopies\Functional\Cleaned Subjects For Vizual\asdf\E02862", "--side:R", "--testbone:sca", "--refbone:rad", "--poslist:02R", "-c:0.000000,0.010000,0.020000,0.030000,0.040000,0.050000,0.060000,0.070000,0.080000,0.090000,0.100000,0.110000,0.120000,0.130000,0.140000,0.150000,0.160000,0.170000,0.180000,0.190000,0.200000,0.210000,0.220000,0.230000,0.240000,0.250000,0.260000,0.270000,0.280000,0.290000,0.300000,0.310000,0.320000,0.330000,0.340000,0.350000,0.360000,0.370000,0.380000,0.390000,0.400000,0.410000,0.420000,0.430000,0.440000,0.450000,0.460000,0.470000,0.480000,0.490000,0.500000,0.510000,0.520000,0.530000,0.540000,0.550000,0.560000,0.570000,0.580000,0.590000,0.600000,0.610000,0.620000,0.630000,0.640000,0.650000,0.660000,0.670000,0.680000,0.690000,0.700000,0.710000,0.720000,0.730000,0.740000,0.750000,0.760000,0.770000,0.780000,0.790000,0.800000,0.810000,0.820000,0.830000,0.840000,0.850000,0.860000,0.870000,0.880000,0.890000,0.900000,0.910000,0.920000,0.930000,0.940000,0.950000,0.960000,0.970000,0.980000,0.990000,1.000000,1.010000,1.020000,1.030000,1.040000,1.050000,1.060000,1.070000,1.080000,1.090000,1.100000,1.110000,1.120000,1.130000,1.140000,1.150000,1.160000,1.170000,1.180000,1.190000,1.200000,1.210000,1.220000,1.230000,1.240000,1.250000,1.260000,1.270000,1.280000,1.290000,1.300000,1.310000,1.320000,1.330000,1.340000,1.350000,1.360000,1.370000,1.380000,1.390000,1.400000,1.410000,1.420000,1.430000,1.440000,1.450000,1.460000,1.470000,1.480000,1.490000,1.500000,1.510000,1.520000,1.530000,1.540000,1.550000,1.560000,1.570000,1.580000,1.590000,1.600000,1.610000,1.620000,1.630000,1.640000,1.650000,1.660000,1.670000,1.680000,1.690000,1.700000,1.710000,1.720000,1.730000,1.740000,1.750000,1.760000,1.770000,1.780000,1.790000,1.800000,1.810000,1.820000,1.830000,1.840000,1.850000,1.860000,1.870000,1.880000,1.890000,1.900000,1.910000,1.920000,1.930000,1.940000,1.950000,1.960000,1.970000,1.980000,1.990000,2.000000,2.010000,2.020000,2.030000,2.040000,2.050000,2.060000,2.070000,2.080000,2.090000,2.100000,2.110000,2.120000,2.130000,2.140000,2.150000,2.160000,2.170000,2.180000,2.190000,2.200000,2.210000,2.220000,2.230000,2.240000,2.250000,2.260000,2.270000,2.280000,2.290000,2.300000,2.310000,2.320000,2.330000,2.340000,2.350000,2.360000,2.370000,2.380000,2.390000,2.400000,2.410000,2.420000,2.430000,2.440000,2.450000,2.460000,2.470000,2.480000,2.490000,2.500000,2.510000,2.520000,2.530000,2.540000,2.550000,2.560000,2.570000,2.580000,2.590000,2.600000,2.610000,2.620000,2.630000,2.640000,2.650000,2.660000,2.670000,2.680000,2.690000,2.700000,2.710000,2.720000,2.730000,2.740000,2.750000,2.760000,2.770000,2.780000,2.790000,2.800000,2.810000,2.820000,2.830000,2.840000,2.850000,2.860000,2.870000,2.880000,2.890000,2.900000,2.910000,2.920000,2.930000,2.940000,2.950000,2.960000,2.970000,2.980000,2.990000,3.000000", "--saveArea" };
//                fileArgs = new string[] { @"-s:F:\LocalCopies\Functional\Cleaned Subjects For Vizual\asdf\E02862", "--side:R", "--testbone:sca", "--refbone:rad", "--poslist:02R", "--contourArea:77.1601", "--tolerance:0.0001", "--saveArea" };
//            }
//#endif
            CommandLineOptions options = new CommandLineOptions();
            options.ProcessArgs(fileArgs);

            if (options.isBatchMode())
            {
                BatchMode batch = new BatchMode(options);
                System.Environment.Exit(0);
            }

            InitializeComponent();
            setupScaleFactorMenuItemTags();

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
                    showMetacarpalInertiasToolStripMenuItem.Enabled = true;
                    referenceBoneForWristPositionToolStripMenuItem.Enabled = true;
                    createAnimationToolStripMenuItem.Enabled = true;
                    boneColorsToolStripMenuItem.Enabled = true;
                    break;
            }
        }

        #region setupMethods
        private void setupExaminerWindow()
        {
            _viewer = new ExaminerViewer((int)panelCoin.Handle);
            _viewer.setHighlightType(getSelectionHighlightRenderType());
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
            cameraPositionOrientationToolStripMenuItem.Enabled = true;
            copyToClipboardToolStripMenuItem.Enabled = true;
            saveImageOptionsToolStripMenuItem.Enabled = true;
            decoratorToolStripMenuItem.Enabled = true;
            saveFrameToolStripMenuItem.Enabled = true;
            showInertiasToolStripMenuItem.Enabled = false;
            showInertiasToolStripMenuItem.Checked = false;
            showMetacarpalInertiasToolStripMenuItem.Enabled = false;
            showMetacarpalInertiasToolStripMenuItem.Checked = false;
            showACSToolStripMenuItem.Enabled = false;
            showACSToolStripMenuItem.Checked = false;
            showAxesToolStripMenuItem.Enabled = true;
            pointIntersectionToolStripMenuItem.Enabled = true;
            pointIntersectionToolStripMenuItem.Checked = false;
            colorTransparencyToolStripMenuItem.Enabled = false;
            createAnimationToolStripMenuItem.Enabled = false;
            createAnimationToolStripMenuItem.Checked = false;
            transparencyToolStripMenuItem.Enabled = true;
            selectionStyleToolStripMenuItem.Enabled = true;
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
            referenceBoneForWristPositionToolStripMenuItem.Enabled = false;
            capitateToolStripMenuItem.Checked = true;
            mC3ToolStripMenuItem.Checked = false;
            boneColorsToolStripMenuItem.Enabled = false;
            
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
            open.Filter = "Compatable Files (*.iv;*.wrl)|*.iv;*.wrl|Inventor Files (*.iv)|*.iv|VRML Files (*.wrl)|*.wrl|Stack Files (*.stack)|*.stack|All Files (*.*)|*.*";
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
                    addFileToRoot(filename);

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
            //save to recently opened files
            RegistrySettings.saveMostRecentFile(filenames[0]);
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
                addFileToRoot(filename);
        }

        /// <summary>
        /// Intelligently try and add the given file to the root node
        /// </summary>
        /// <param name="filename"></param>
        private void addFileToRoot(string filename)
        {
            string ext = Path.GetExtension(filename).ToLower();
            try
            {
                switch (ext)
                {
                    case ".iv":
                    case ".vrml":
                    case ".wrl":
                        _root.addFile(filename);
                        break;
                    case ".stack":
                    case ".dat":
                        _root.addChild(readStackfile(filename));
                        break;
                    default:
                        MessageBox.Show("Error: Unknown file extension for: " + filename, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                }
            }
            catch (Exception ex)
            {
                if (!hideErrorMessagesToolStripMenuItem.Checked)
                {
                    string msg = String.Format("Error loading file: {0}\n\n{1}", filename, ex.ToString());
                    libWrist.ExceptionHandling.HandledExceptionManager.ShowDialog(msg, "", "", ex);
                }
            }
        }

        private Separator readStackfile(string filename)
        {
            double[][] pts = DatParser.parseDatFile(filename);
            return Texture.createPointsFileObject(pts);
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
                    string msg = "Error opening wrist. " + ex.Message;
                    libWrist.ExceptionHandling.HandledExceptionManager.ShowDialog(msg, "", "", ex);
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
            _fullWristController = new FullWristController(_viewer);
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
            //_posViewController.saveToMovie();
            _currentController.saveToMovie();
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
                libWrist.ExceptionHandling.HandledExceptionManager.ShowDialog(msg, "", "", ex);
            }
        }
        #endregion
        #endregion

        #region Drag and Drop
        private void WristVizualizer_DragDrop(object sender, DragEventArgs e)
        {
            string[] filenames = e.Data.GetData("FileDrop") as string[];
            try
            {
                if (sender == panelCoin && importToolStripMenuItem.Enabled && _viewer != null && _root != null)
                    importFile(filenames);
                else
                    openFile(filenames);
            }
            catch (Exception ex)
            {
                string msg = "Error loading file(s)";
                libWrist.ExceptionHandling.HandledExceptionManager.ShowDialog(msg, "", "", ex);
            }
        }

        private void WristVizualizer_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("FileDrop"))
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            if (sender == panelCoin && importToolStripMenuItem.Enabled)
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.Move;           
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
            setStatusStripVisibility(true);
        }

        private void hideStatusStrip()
        {
            setStatusStripVisibility(false);
        }

        private void setStatusStripVisibility(bool visible)
        {
            if (statusStrip1.Visible == visible)
                return; //nothing to change

            statusStrip1.Visible = visible;
            int heightNeeded = (visible) ? 22 : 0; //it takes 22pixels when visible, 0 when not :)
            //in the default mode, the MainPanel starts 359px high, and the main form starts 442px high
            // so the normal offset is 442-359 = 83
            const int normalOffset = 83;
            mainLayoutPanel.Height = this.Height - normalOffset - heightNeeded;
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
            if (r != DialogResult.Yes)
                return;

            //lets load the file. New option is to save the camera setting first, and then re-apply them...
            Camera originalCam = _viewer.Camera; //save starting camera
            int backColor = _viewer.getBackgroundColor();
            openFile(new string[] { _firstFileName }, false); //load new scene
            _viewer.Camera.copySettingsFromCamera(originalCam); //copy back settings I hope...
            _viewer.setBackgroundColor(backColor);
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
            _fullWristController.setInertiaVisibilityCarpalBones(showInertiasToolStripMenuItem.Checked);
        }

        private void showMetacarpalInertiasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _fullWristController.setInertiaVisibilityMetacarpalBones(showMetacarpalInertiasToolStripMenuItem.Checked);
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

        private void startHeadtrackingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WiiTracking _tracking = new WiiTracking(this, _viewer);
            bool result = _tracking.TryConnect();
            if (!result)
            {
                MessageBox.Show("Error connecting to Wiimote");
                return;
            }
        }

        private ExaminerViewer.HighlighRenderTypes getSelectionHighlightRenderType()
        {
            if (boundingBoxToolStripMenuItem.Checked) return ExaminerViewer.HighlighRenderTypes.BOX_HIGHLIGHT_RENDER;
            if (lineToolStripMenuItem.Checked) return ExaminerViewer.HighlighRenderTypes.LINE_HIGHLIGHT_RENDER;
            throw new WristVizualizerException("Unknown Selection HighlightRender type. None appear to be selected...");
        }

        private void boundingBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (boundingBoxToolStripMenuItem.Checked) return;

            boundingBoxToolStripMenuItem.Checked = true;
            lineToolStripMenuItem.Checked = false;

            if (_viewer != null)
                _viewer.setHighlightType(ExaminerViewer.HighlighRenderTypes.BOX_HIGHLIGHT_RENDER);
        }

        private void lineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lineToolStripMenuItem.Checked) return;

            lineToolStripMenuItem.Checked = true;
            boundingBoxToolStripMenuItem.Checked = false;

            if (_viewer != null)
                _viewer.setHighlightType(ExaminerViewer.HighlighRenderTypes.LINE_HIGHLIGHT_RENDER);
        }

        private int getBoneIndexForWristPositionReferenceBone()
        {
            foreach (ToolStripMenuItem menuItem in referenceBoneForWristPositionToolStripMenuItem.DropDownItems)
            {
                if (menuItem.Checked)
                    return (int)menuItem.Tag;
            }
            throw new WristVizualizerException("Unable to determine the ReferenceBone for determining Wrist Position");
        }

        private void referenceBoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //update the check box.
            ToolStripMenuItem clickedItem = (ToolStripMenuItem)sender;
            if (clickedItem.Checked) return; //if we are already set, get out. Nothing to do.

            //update the display
            foreach (ToolStripMenuItem menuItem in referenceBoneForWristPositionToolStripMenuItem.DropDownItems)
            {
                menuItem.Checked = (clickedItem == menuItem);
            }

            //change the reference bone.
            _fullWristController.changeWristPositionReferenceBoneIndex(getBoneIndexForWristPositionReferenceBone());
        }

        private void cameraPositionOrientationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_viewer == null) return;
            Camera cam = _viewer.Camera;
            CameraEditor camEditor = new CameraEditor(cam);
            camEditor.CheckAndShowDialog();
        }

        private void recentFilesToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            //load saved files
            string[] files = RegistrySettings.getAllRecentFiles();
            recentFilesToolStripMenuItem.DropDownItems.Clear();
            if (files.Length == 0)
            {
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = "No Recent Files";
                item.Enabled = false;
                recentFilesToolStripMenuItem.DropDownItems.Add(item);
            }

            for (int i=0; i<files.Length; i++)
            {
                ToolStripMenuItem item = new ToolStripMenuItem();
                string shortFname = RegistrySettings.ShortenPathname(Path.GetFullPath(files[i]), 48);
                item.Text = String.Format("{0} {1}",i+1, shortFname);
                item.Tag = files[i];
                item.Click += new EventHandler(recentFileToolStripMenuItem_Click);
                recentFilesToolStripMenuItem.DropDownItems.Add(item);
            }
            
        }

        private void recentFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            string fname = (string)item.Tag;
            if (!File.Exists(fname))
            {
                //TODO: Error handling for invalid file...?
                string msg = String.Format("Error: Unable to find file: {0}", fname);
                MessageBox.Show(msg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            //now lets open the file
            openFile(new string[] { fname });
        }

        private void trimCameraMaterialsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TrimIVFileForm trim = new TrimIVFileForm();
            trim.Show();
        }

        private void copyToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            KeyValuePair<int, int> factors = getSelectedScaleAndSmoothFactors();
            int finalScale = factors.Key;
            int smooth = factors.Value;

            int scale = finalScale * smooth;
            Image rawImage;
            //lets get the image
            if (scale == 1)
                rawImage = _viewer.getImage();
            else
            {
                _viewer.cacheOffscreenRenderer(scale);
                rawImage = _viewer.getImage();
                _viewer.clearOffscreenRenderer();
            }
            
            //now lets check if we don't need to downsample
            if (smooth == 1)
            {
                Clipboard.SetImage(rawImage);
                rawImage.Dispose();
                this.Cursor = Cursors.Default;
                return;
            }
             
            //else, we need to downsample
            Image finalImage = new Bitmap(rawImage.Size.Width / smooth, rawImage.Size.Height / smooth, rawImage.PixelFormat);
            using (Graphics g = Graphics.FromImage(finalImage))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(rawImage, 0, 0, rawImage.Size.Width / smooth, rawImage.Size.Height / smooth);
            }
            Clipboard.SetImage(finalImage);
            rawImage.Dispose();
            finalImage.Dispose();
            this.Cursor = Cursors.Default;
        }

        private void createAnimationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //check that we are in FullWristMode
            if (_mode != Modes.FULL_WRIST || _currentController == null ||
                !_currentController.GetType().Equals(typeof(FullWristController)))
                return;

            FullWristController control = (FullWristController)_currentController;

            if (createAnimationToolStripMenuItem.Checked)
            {
                //get out of this crap
                control.EndFullAnimation();
                createAnimationToolStripMenuItem.Checked = false;
                saveMovieToolStripMenuItem.Enabled = false;
            }
            else
            {
                DialogResult r = control.createComplexAnimationMovie();
                if (r == DialogResult.OK)
                {
                    createAnimationToolStripMenuItem.Checked = true;
                    saveMovieToolStripMenuItem.Enabled = true;
                }
            }
        }

        private void xScaleSmoothingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem choiceItem in saveImageOptionsToolStripMenuItem.DropDownItems)
            {
                choiceItem.Checked = (choiceItem == sender);
            }
        }

        private KeyValuePair<int, int> getSelectedScaleAndSmoothFactors()
        {
            foreach (ToolStripMenuItem choiceItem in saveImageOptionsToolStripMenuItem.DropDownItems)
            {
                if (choiceItem.Checked)
                    return (KeyValuePair<int, int>)choiceItem.Tag;
            }
            return new KeyValuePair<int, int>(1, 0);
        }

        private void setupScaleFactorMenuItemTags()
        {
            this.xScaleNoSmoothingToolStripMenuItem.Tag = new System.Collections.Generic.KeyValuePair<int, int>(1, 1);
            this.xScale2xSmoothingToolStripMenuItem.Tag = new System.Collections.Generic.KeyValuePair<int, int>(1, 2);
            this.xScale3xSmoothingToolStripMenuItem.Tag = new System.Collections.Generic.KeyValuePair<int, int>(1, 3);
            this.xScaleNoSmoothingToolStripMenuItem1.Tag = new System.Collections.Generic.KeyValuePair<int, int>(2, 1);
            this.xScaleNoSmoothingToolStripMenuItem2.Tag = new System.Collections.Generic.KeyValuePair<int, int>(3, 1);
            this.xScaleNoSmoothingToolStripMenuItem3.Tag = new System.Collections.Generic.KeyValuePair<int, int>(4, 1);
        }

        private void boneColorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FullWristController control = (FullWristController)_currentController;
            control.EditBoneColorsShowDialog();
        }
    }
}