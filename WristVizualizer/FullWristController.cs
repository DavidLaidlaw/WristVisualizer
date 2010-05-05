using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using libWrist;
using libCoin3D;
using AviFile;

namespace WristVizualizer
{    
    class FullWristController : Controller
    {
        private bool _showErrors = false;
        
        private WristFilesystem _wrist;
        private FullWrist _fullWrist;
        
        private int _currentPositionIndex;
        private int _fixedBoneIndex;

        private ExaminerViewer _viewer;
        private Separator _root;

        //animation stuff
        private bool _animatePositionChanges;
        private ShortAnimationController _shortAnimationController;
        AnimationCreatorForm _acf;
        private int _FPS;
        private double _animateDuration;
        private Timer _animationTimer;

        //Full Animation Stuff
        private AnimationControl _animationControl;        

        //Distance Maps
        private BulkCalculator _calculator;

        //GUI stuff
        private WristPanelLayoutControl _layoutControl;
        private FullWristControl _wristControl;
        private PositionGraph _positionGraph;

        public FullWristController(ExaminerViewer viewer, string radiusFilename)
        {
            _viewer = viewer;
            setupControl();
            setupControlEventListeners();
            _root = new Separator();
            _calculator = new BulkCalculator();
            

            //defaults
            _animationControl = null;
            _FPS = 15;
            _animateDuration = 0.5;

            //now lets load
            loadFullWrist(radiusFilename);
        }

        public override string LastFileFilename { get { return _wrist.Radius; } }

        public override void CleanUp() 
        {
            removeControlEventListeners();
        }

        private void loadFullWrist(string radiusFilename)
        {
            //TODO: ShowFullWristControlBox
            //TODO: Block viewSource

            //First Try and load the wrist data
            
            _wrist = new WristFilesystem();

            _wrist.setupWrist(radiusFilename);
            _fullWrist = new FullWrist(_wrist);
            _fullWrist.LoadFullWrist();
            _root = _fullWrist.Root;

            populateSeriesList(); //TODO?
            
            //catch (ArgumentException ex)
            //{
            //    if (_showErrors)
            //    {
            //        string msg = "Error loading wrist kinematics.\n\n" + ex.Message;
            //        //TODO: Change to abort,retry, and find way of cancelling load
            //        libWrist.ExceptionHandling.HandledExceptionManager.ShowDialog(msg, "", "", ex);
            //    }
            //    for (int i = 0; i < Wrist.NumBones; i++)
            //        _wristControl.disableFixingBone(i);
            //}

            //disable invalid bones!
            for (int i = 0; i < WristFilesystem.NumBones; i++)
            {
                if (!_fullWrist.Bones[i].IsValidBone)
                    _wristControl.disableBone(i);
            }

            setupPositionGraphIfPossible(8); //hardcoded default reference bone to the Capitate
        }

        private bool hasPositionInformation(int referenceBoneIndex)
        {
            //only need to check the ACS and the capitate (index 8)
            if (_fullWrist.Bones[0].HasInertia && _fullWrist.Bones[referenceBoneIndex].HasInertia)
                return true;
            else
                return false;
        }

        private bool hasPronationSupinationInformation()
        {
            //Check for existance of the actual AnatCoordSys_uln.dat file; else this information is probably wrong...
            if (!File.Exists(_wrist.acsFile_uln))
                return false;

            //need to check for the Radius and the Ulna, (index 0 & 1)
            if (_fullWrist.Bones[0].HasInertia && _fullWrist.Bones[1].HasInertia)
                return true;
            else
                return false;
        }

        public override bool CanChangeReferenceBone { get { return true; } }
        public override void changeReferenceBoneByIndex(int referenceBoneIndex)
        {
            //now lets setup for the new reference bone
            setupPositionGraphIfPossible(referenceBoneIndex);
        }

        private void setupPositionGraphIfPossible(int referenceBoneIndex)
        {
            //first lets calculate the new data
            Bone acsBone = _fullWrist.Bones[(int)WristFilesystem.BIndex.RAD];
            Bone refBone = _fullWrist.Bones[referenceBoneIndex];
            Bone ulnBone = _fullWrist.Bones[(int)WristFilesystem.BIndex.ULN];

            PostureCalculator.Posture[] postureFE = PostureCalculator.CalculatePosturesFE(acsBone, refBone);
            if (postureFE == null)
            {
                //if we could not calculate it, then we need to remove an existing graph
                if (_positionGraph == null) return;
                _positionGraph.SelectedSeriesChanged -= new SelectedSeriesChangedHandler(_positionGraph_SelectedSeriesChanged);
                _layoutControl.removeControl(_positionGraph);
                _positionGraph = null;
                return;
            }

            //so we have data, lets first make sure that the graph exists
            if (_positionGraph == null)
            {
                _positionGraph = new PositionGraph();
                _layoutControl.addControl(_positionGraph);
                _positionGraph.SelectedSeriesChanged += new SelectedSeriesChangedHandler(_positionGraph_SelectedSeriesChanged);
            }

            //now lets push the new data up
            PostureCalculator.PronationSupination[] posturesPS = PostureCalculator.CalculatePosturesPS(acsBone, ulnBone);
            _positionGraph.SetPositions(postureFE, posturesPS);
            _positionGraph.setCurrentVisisblePosture(_currentPositionIndex);
        }

        void _positionGraph_SelectedSeriesChanged(object sender, SelectedSeriesChangedEventArgs e)
        {
            //update the visible control, that will send out another event and make the actual position change :)
            _wristControl.selectedSeriesIndex = e.SelectedIndex;
        }

        public override bool CanCalculateDistanceMap { get { return true; } }
        public override void calculateDistanceMapsToolClickedHandler()
        {
            //setup the dialog window
            DistanceAndContourDialog dialog = new DistanceAndContourDialog(_fullWrist.ContourDistances);
            dialog.ColorMapMaxDistance = _fullWrist.ColorMapDistance;
            dialog.setContourColors(_fullWrist.ContourColors);
            dialog.CalculateColorMap = _fullWrist.ShowColorMap;
            dialog.CalculateContours = _fullWrist.ShowContours;

            //show the dialog window
            DialogResult r = dialog.ShowDialog();
            if (r != DialogResult.OK)
                return;

            double colorDistance = -1;
            double[] contourDistances = new double[0];
            if (dialog.CalculateColorMap)
                colorDistance = dialog.ColorMapMaxDistance;
            if (dialog.CalculateContours)
                contourDistances = dialog.getContourDistancesToCalculate();

            //get the queue to calculate stuff
            Queue<Queue<BulkCalculator.DistanceCalculationJob>> q = _fullWrist.CreateDistanceMapMasterQueue(colorDistance, contourDistances, dialog.getContourColorsToCalculate());
            //go compute this!
            _calculator.ProcessMasterQueue(q);

            //update on screen now to current settings
            _fullWrist.ShowColorMap = dialog.CalculateColorMap;
            _fullWrist.ShowContours = dialog.CalculateContours;
            _fullWrist.UpdateColorsAndContoursForCurrentPosition();
        }


        #region public properties
        public override bool AnimatePositionTransitions
        {
            set { _animatePositionChanges = value; }
        }

        public bool ShowErrors
        {
            get { return _showErrors; }
            set { _showErrors = value; }
        }

        public override Control Control
        {
            get { return _layoutControl; }
        }

        public override Separator Root
        {
            get { return _root; }
        }

        public override string ApplicationTitle
        {
            get { return _wrist.subject + _wrist.side + " - " + _wrist.subjectPath; }
        }

        public string getFilenameOfFirstFile()
        {
            return Path.Combine(_wrist.subjectPath, _wrist.subject + _wrist.side);
        }
        #endregion

        #region GUI & Control Setup
        private string[] createSeriesListWithNiceNames()
        {
            string[] series = _wrist.series;

            //make niceSeries a complete list of all the current "not-nice" names
            string[] niceSeries = new string[series.Length + 1];
            niceSeries[0] = _wrist.neutralSeries;
            Array.ConstrainedCopy(series, 0, niceSeries, 1, series.Length); //copy the existing value

            //now try and replace ugly names with nice ones :)
            string configFile = _wrist.SeriesNamesFilename;
            if (File.Exists(configFile))
            {
                Dictionary<string, string> lookupTable = IniFileParser.GetIniFileStrings(configFile, "SeriesNames");
                for (int i = 0; i < niceSeries.Length; i++)
                {
                    if (lookupTable.ContainsKey(niceSeries[i]))
                        niceSeries[i] = lookupTable[niceSeries[i]];
                }
            }            
            return niceSeries;
        }

        private void populateSeriesList()
        {
            _currentPositionIndex = 0;
            _wristControl.clearSeriesList();
            string[] seriesNames = createSeriesListWithNiceNames();
            _wristControl.addToSeriesList(seriesNames);
            _wristControl.selectedSeriesIndex = 0;
        }


        private void setupControl()
        {
            _layoutControl = new WristPanelLayoutControl();
            _wristControl = new FullWristControl();
            _wristControl.setupControl(WristFilesystem.LongBoneNames, true);
            _layoutControl.addControl(_wristControl);
        }

        private void setupControlEventListeners()
        {
            _wristControl.BoneHideChanged += new BoneHideChangedHandler(_control_BoneHideChanged);
            _wristControl.FixedBoneChanged += new FixedBoneChangedHandler(_control_FixedBoneChanged);
            _wristControl.SelectedSeriesChanged += new SelectedSeriesChangedHandler(_control_SelectedSeriesChanged);
        }

        private void removeControlEventListeners()
        {
            _wristControl.BoneHideChanged -= new BoneHideChangedHandler(_control_BoneHideChanged);
            _wristControl.FixedBoneChanged -= new FixedBoneChangedHandler(_control_FixedBoneChanged);
            _wristControl.SelectedSeriesChanged -= new SelectedSeriesChangedHandler(_control_SelectedSeriesChanged);

            if (_positionGraph != null)
                _positionGraph.SelectedSeriesChanged -= new SelectedSeriesChangedHandler(_positionGraph_SelectedSeriesChanged);
        }
        #endregion

        public override bool CanAnimatePositionTransforms { get { return true; } }
        public override void setPositionTransitionAnimationRate(int FPS, double animationDuration)
        {
            _FPS = FPS;
            _animateDuration = animationDuration;
        }

        private void animateChangeInPosition(int lastPositionIndex, int currentPositionIndex, int lastFixedBoneIndex, int currentFixedBoneIndex)
        {
            //setup animations....
            if (_shortAnimationController != null)
                _shortAnimationController.Stop(); //ugly, but should work
            _shortAnimationController = new ShortAnimationController();
            int numFrames = Math.Max((int)(_FPS * _animateDuration), 1); //want at least one frame
            _shortAnimationController.FPS = _FPS;

            _fullWrist.HideColorMapAndContoursTemporarily();

            _shortAnimationController.SetupAnimationForLinearInterpolation(_fullWrist, lastPositionIndex, currentPositionIndex, lastFixedBoneIndex, currentFixedBoneIndex, numFrames);
            _shortAnimationController.Start();
        }

        private void MoveToPosition(int positionIndex, int fixedBoneIndex)
        {
            _fullWrist.HideBonesWithNoKinematics(_currentPositionIndex);

            if (_animatePositionChanges)
            {
                animateChangeInPosition(_currentPositionIndex, positionIndex, _fixedBoneIndex, fixedBoneIndex);
            }
            else
            {
                _fullWrist.MoveToPositionAndFixedBone(positionIndex, fixedBoneIndex);
            }

            //now that we are done, lets save the last positions
            _currentPositionIndex = positionIndex;
            _fixedBoneIndex = fixedBoneIndex;
        }

        void _control_SelectedSeriesChanged(object sender, SelectedSeriesChangedEventArgs e)
        {
            if (_currentPositionIndex == e.SelectedIndex)
                return;

            if (_positionGraph != null)
                _positionGraph.setCurrentVisisblePosture(e.SelectedIndex);

            MoveToPosition(e.SelectedIndex, _fixedBoneIndex);
        }

        void _control_FixedBoneChanged(object sender, FixedBoneChangeEventArgs e)
        {
            MoveToPosition(_currentPositionIndex, e.BoneIndex);
        }

        void _control_BoneHideChanged(object sender, BoneHideChangeEventArgs e)
        {
            _fullWrist.Bones[e.BoneIndex].SetBoneVisibility(!e.BoneHidden);
        }

        
        public override bool CanShowCarpalInertias { get { return true; } }
        public override void setInertiaVisibilityCarpalBones(bool visible)
        {
            setInertiaVisibility(visible, WristFilesystem.CarpalBoneIndexes);
        }

        public override bool CanShowMetacarpalInertias { get { return true; } }
        public override void setInertiaVisibilityMetacarpalBones(bool visible)
        {
            setInertiaVisibility(visible, WristFilesystem.MetacarpalBoneIndexes);
        }

        public override bool CanShowACS { get { return true; } }
        public override void setACSVisibility(bool visible)
        {
            int[] forearm = { 0, 1 };
            setInertiaVisibility(visible, forearm, 45);
        }

        private void setInertiaVisibility(bool visible, int[] boneIndexes) { setInertiaVisibility(visible, boneIndexes, 0); }
        private void setInertiaVisibility(bool visible, int[] boneIndexes, int arrowLength)
        {
            foreach (int i in boneIndexes)
            {
                _fullWrist.Bones[i].SetInertiaVisibility(visible);
            }
        }


        public override bool CanCreateComplexAnimations { get { return true; } }
        public override DialogResult createComplexAnimationMovie()
        {
            string[] positionNames = createSeriesListWithNiceNames();
            AnimationCreatorForm acf = new AnimationCreatorForm(positionNames);
            //Pass in positions....
            DialogResult r = acf.ShowDialog();
            if (r == DialogResult.OK)
                startFullAnimation(acf);

            return r;
        }

        public override void EndFullAnimation()
        {
            //return GUI
            _layoutControl.removeControl(_animationControl);
            if (_positionGraph != null)
                _layoutControl.addControl(_positionGraph);
            _wristControl.changeBackToNormalMode();

            _wristControl.FixedBoneChanged += new FixedBoneChangedHandler(_control_FixedBoneChanged);
            _wristControl.setFixedBone(_fixedBoneIndex); //set back to where we were
            _wristControl.FixedBoneChanged -= new FixedBoneChangedHandler(_control_Animation_FixedBoneChanged);
            _wristControl.ShowHamChanged -= new ShowHamChangedHandler(_wristControl_ShowHamChanged);

            _animationControl.TrackbarScroll -= new AnimationControl.TrackbarScrollHandler(_animationControl_TrackbarScroll);
            _animationControl.StopClicked -= new AnimationControl.StopClickedHandler(_animationControl_StopClicked);
            _animationControl.PlayClicked -= new AnimationControl.PlayClickedHandler(_animationControl_PlayClicked);
            _animationControl.FPSChanged -= new AnimationControl.FPSChangedHandler(_animationControl_FPSChanged);
                        
            _animationTimer.Tick -= new EventHandler(_animationTimer_Tick);
            _animationTimer.Stop();
            _animationTimer = null;
            _animationControl = null;

            //reset wrist
            _fullWrist.EndAnimation();
        }

        private void startFullAnimation(AnimationCreatorForm acf)
        {
            _acf = acf;
            int[] animationOrder = acf.getAnimationOrder();
            int numFrames = acf.NumberStepsPerPositionChange;

            _animationControl = new AnimationControl();
            _layoutControl.addControl(_animationControl);

            int totalNumFrames = numFrames * (animationOrder.Length - 1) + 1;
            _animationControl.setupController(totalNumFrames);
            _animationControl.FPS = 10;

            //little bit of gui stuff
            if (_layoutControl.Contains(_positionGraph))
                _layoutControl.removeControl(_positionGraph);
            _wristControl.changeToAnimationMode();

            int startingFixedBone = (int)WristFilesystem.BIndex.RAD; //default to fixing to the radius...
            //now, lets go and add the switches into place
            SetupWristDistancesForAnimation(startingFixedBone, animationOrder, numFrames, (double)acf.DistanceMapMaximumValue, acf.GetContourDistancesToCalculate(), acf.GetContourColorsToCalculate());
            _fullWrist.SetupWristForAnimation(startingFixedBone, animationOrder, numFrames); //default to radius fixed...
            _fullWrist.SetToAnimationFrame(0); //set us to the first frame

            //redirect change in fixed bone....
            _wristControl.FixedBoneChanged -= new FixedBoneChangedHandler(_control_FixedBoneChanged);
            _wristControl.setFixedBone(startingFixedBone); //do this while no one is listening
            _wristControl.FixedBoneChanged += new FixedBoneChangedHandler(_control_Animation_FixedBoneChanged);
            _wristControl.ShowHamChanged += new ShowHamChangedHandler(_wristControl_ShowHamChanged);

            _animationControl.TrackbarScroll += new AnimationControl.TrackbarScrollHandler(_animationControl_TrackbarScroll);
            _animationControl.StopClicked += new AnimationControl.StopClickedHandler(_animationControl_StopClicked);
            _animationControl.PlayClicked += new AnimationControl.PlayClickedHandler(_animationControl_PlayClicked);
            _animationControl.FPSChanged += new AnimationControl.FPSChangedHandler(_animationControl_FPSChanged);

            _animationTimer = new Timer();
            _animationTimer.Tick += new EventHandler(_animationTimer_Tick);
            _animationTimer.Interval = (int)(1000 / (double)_animationControl.FPS);
        }

        private void SetupWristDistancesForAnimation(int fixedBoneIndex, int[] animationOrder, int numFrames, double colorMapDistance, double[] cDistances, System.Drawing.Color[] colors)
        {
            Queue<Queue<BulkCalculator.DistanceCalculationJob>> q;
            q = _fullWrist.CreateDistanceMapMasterAnimationQueue(animationOrder, numFrames, _fullWrist.Bones[fixedBoneIndex], colorMapDistance, cDistances, colors);
            if (q == null)
                return;
            //go compute this!
            _calculator.ProcessMasterQueue(q);
        }

        void _wristControl_ShowHamChanged(object sender, BoneHideChangeEventArgs e)
        {
            _fullWrist.Bones[e.BoneIndex].SetHamVisibility(!e.BoneHidden);
        }

        void _animationTimer_Tick(object sender, EventArgs e)
        {
            _animationControl.AdvanceCurrentFrameTrackbar();
            updateAnimationFrame(); //do I need this? Or will the trackbar update event not fire
        }

        void _animationControl_FPSChanged()
        {
            _animationTimer.Interval = (int)(1000 / (double)_animationControl.FPS);
        }

        void _animationControl_PlayClicked()
        {
            _animationControl.StopButtonEnabled = true;
            _animationControl.PlayButtonEnabled = false;
            _animationTimer.Start();
        }

        void _animationControl_StopClicked()
        {
            _animationControl.StopButtonEnabled = false;
            _animationControl.PlayButtonEnabled = true;
            _animationTimer.Stop();
        }

        void _animationControl_TrackbarScroll()
        {
            updateAnimationFrame();
        }

        private void updateAnimationFrame()
        {
            int index = _animationControl.currentFrame;
            setAnimationToFrame(index);
        }

        private void setAnimationToFrame(int frameIndex)
        {
            _fullWrist.SetToAnimationFrame(frameIndex);
        }

        void _control_Animation_FixedBoneChanged(object sender, FixedBoneChangeEventArgs e)
        {
            //Recalculate animation....
            int[] animationOrder = _acf.getAnimationOrder();
            int numFrames = _acf.NumberStepsPerPositionChange;
            SetupWristDistancesForAnimation(e.BoneIndex, animationOrder, numFrames, (double)_acf.DistanceMapMaximumValue, _acf.GetContourDistancesToCalculate(), _acf.GetContourColorsToCalculate());
            _fullWrist.SetupWristForAnimation(e.BoneIndex, animationOrder, numFrames);
            updateAnimationFrame(); //make certain we are on the correct frame...
        }

        public override bool CanSaveToMovie
        {
            // we can only save if we are in animation mode, which is defined by the control existing
            get { return (_animationControl != null); }
        }
        public override void saveToMovie()
        {
            //save starting state & stop playback
            bool startPlaying = _animationTimer.Enabled;
            int startFrame = _animationControl.currentFrame;
            _animationTimer.Stop();

            //show save dialogue
            MovieExportOptions dialog = new MovieExportOptions(_wrist.subjectPath, _animationControl.FPS);
            dialog.ShowDialog();

            //okay, now lets figure out what we are doing here
            switch (dialog.results)
            {
                case MovieExportOptions.SaveType.CANCEL:
                    //nothing to do, we were canceled
                    break;
                case MovieExportOptions.SaveType.IMAGES:
                    //save images
                    string outputDir = dialog.OutputFileName;
                    //save it to a movie
                    _viewer.cacheOffscreenRenderer();
                    for (int i = 0; i < _animationControl.NumberOfFrames; i++)
                    {
                        setAnimationToFrame(i);  //change to current frame
                        string fname = Path.Combine(outputDir, String.Format("outfile{0:d3}.jpg", i));
                        _viewer.saveToJPEG(fname);
                    }
                    _viewer.clearOffscreenRenderer();
                    break;
                case MovieExportOptions.SaveType.MOVIE:
                    //save movie
                    try
                    {
                        AviManager aviManager = new AviManager(dialog.OutputFileName, false);
                        int smooth = dialog.SmoothFactor;
                        _viewer.cacheOffscreenRenderer(smooth); //TODO: Check that output is multiple of 4!!!!
                        setAnimationToFrame(0); //set to first frame, so we can grab it.
                        System.Drawing.Bitmap frame = getSmoothedFrame(smooth);
                        VideoStream vStream = aviManager.AddVideoStream(dialog.MovieCompress, (double)dialog.MovieFPS, frame);
                        for (int i = 1; i < _animationControl.NumberOfFrames; i++) //start from frame 1, frame 0 was added when we began
                        {
                            setAnimationToFrame(i);  //change to current frame
                            vStream.AddFrame(getSmoothedFrame(smooth));
                        }
                        aviManager.Close();  //close out and save
                        _viewer.clearOffscreenRenderer();
                    }
                    catch (Exception ex)
                    {
                        string msg = "Error saving to movie file.\n\n" + ex.Message;
                        libWrist.ExceptionHandling.HandledExceptionManager.ShowDialog(msg, "", "", ex);
                    }
                    break;
            }

            //wrap us up, resume frame and playing status
            updateAnimationFrame();
            if (startPlaying)
                _animationTimer.Start();
        }

        private System.Drawing.Bitmap getSmoothedFrame(int smoothFactor)
        {
            System.Drawing.Image rawImage = _viewer.getImage();
            if (smoothFactor == 1)
                return (System.Drawing.Bitmap)rawImage;

            System.Drawing.Image finalImage = new System.Drawing.Bitmap(rawImage.Size.Width / smoothFactor, rawImage.Size.Height / smoothFactor);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(finalImage))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(rawImage, 0, 0, rawImage.Size.Width / smoothFactor, rawImage.Size.Height / smoothFactor);
            }
            rawImage.Dispose();
            return (System.Drawing.Bitmap)finalImage;
        }

        public override bool CanEditBoneColors { get { return true; } }
        public override void EditBoneColorsShowDialog()
        {
            EditBoneColors edit = new EditBoneColors(_fullWrist);
            DialogResult r = edit.ShowDialog();
            if (r != DialogResult.OK)
                return;
            for (int i = 0; i < WristFilesystem.NumBones; i++)
            {
                if (_fullWrist.Bones[i].IsValidBone && edit.IsColorChanged(i))
                    _fullWrist.Bones[i].SetColor(edit.GetNewBoneColor(i));
            }
        }
    }
}
