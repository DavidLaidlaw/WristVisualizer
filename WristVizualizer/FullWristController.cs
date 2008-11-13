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
        
        private Wrist _wrist;
        private FullWrist _fullWrist;
        
        private int _currentPositionIndex;
        private int _lastPositionIndex;
        private int _fixedBoneIndex;
        private int _lastFixedBoneIndex;

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
        private Switch[] _animationSwitches;
        private Switch[] _animationHamSwitches;
        private AnimationControl _animationControl;        

        //Distance Maps
        private DistanceMaps _distMap;
        private bool _hideMaps;
        private bool _hideContours;

        //GUI stuff
        private WristPanelLayoutControl _layoutControl;
        private FullWristControl _wristControl;
        private PositionGraph _positionGraph;

        public FullWristController(ExaminerViewer viewer)
        {
            _viewer = viewer;
            setupControl();
            setupControlEventListeners();
            _root = new Separator();
            _distMap = new DistanceMaps();
            

            //defaults
            _FPS = 15;
            _animateDuration = 0.5;
        }

        public new void CleanUp()
        {
            removeControlEventListeners();
        }

        public void loadFullWrist(string radiusFilename)
        {
            //TODO: ShowFullWristControlBox
            //TODO: Block importing a file
            //TODO: Block viewSource
            //TODO: Enable ShowInertia
            //TODO: Enable ShowACS

            //First Try and load the wrist data
            
            _wrist = new Wrist();

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
            for (int i = 0; i < Wrist.NumBones; i++)
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

        public void changeWristPositionReferenceBoneIndex(int referenceBoneIndex)
        {
            //now lets setup for the new reference bone
            setupPositionGraphIfPossible(referenceBoneIndex);
        }

        private void setupPositionGraphIfPossible(int referenceBoneIndex)
        {
            //first lets calculate the new data
            Bone acsBone = _fullWrist.Bones[(int)Wrist.BIndex.RAD];
            Bone refBone = _fullWrist.Bones[referenceBoneIndex];
            Bone ulnBone = _fullWrist.Bones[(int)Wrist.BIndex.ULN];

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


        public void calculateDistanceMapsToolClickedHandler()
        {
            //setup the dialog window
            DistanceAndContourDialog dialog = new DistanceAndContourDialog(_distMap.ContourDistances);
            dialog.ColorMapMaxDistance = _distMap.MaxColoredDistance;
            dialog.setContourColors(_distMap.ContourColors);
            dialog.CalculateColorMap = !_hideMaps;
            dialog.CalculateContours = !_hideContours;

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
            Queue<Queue<DistanceMaps.DistanceCalculationJob>> q = _fullWrist.CreateDistanceMapJobQueue(colorDistance, contourDistances, dialog.getContourColorsToCalculate());
            //go compute this!
            _distMap.ProcessMasterQueue(q);

            //update on screen now to current settings
            _hideContours = !dialog.CalculateContours;
            _hideMaps = !dialog.CalculateColorMap;
            _fullWrist.ShowColorMap = dialog.CalculateColorMap;
            _fullWrist.ShowContours = dialog.CalculateContours;
            _fullWrist.UpdateColorsAndContoursForCurrentPosition();
        }


        #region public properties
        public bool AnimatePositionTransitions
        {
            get { return _animatePositionChanges; }
            set { _animatePositionChanges = value; }
        }

        public int FPS
        {
            get { return _FPS; }
            set { _FPS = value; }
        }

        public double AnimationDuration
        {
            get { return _animateDuration; }
            set { _animateDuration = value; }
        }

        public DistanceMaps DistanceMaps
        {
            get { return _distMap; }
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

        public string getTitleCaption()
        {
            return _wrist.subject + _wrist.side + " - " + _wrist.subjectPath;
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
            _wristControl.setupControl(Wrist.LongBoneNames, true);
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


        private void animateChangeInPosition()
        {
            /*

            //setup animations....
            if (_shortAnimationController != null)
                _shortAnimationController.Stop(); //ugly, but should work
            _shortAnimationController = new ShortAnimationController();
            int numFrames = Math.Max((int)(_FPS * _animateDuration), 1); //want at least one frame

            HelicalTransform[] htRelMotions = new HelicalTransform[_bones.Length]; //rel motion from last to current

            TransformMatrix[] lastRelMotion = calculateRelativeMotionFromNeutral(_lastPositionIndex, _lastFixedBoneIndex); //rel motion from neutral to last
            TransformMatrix[] currentRelMotion = calculateRelativeMotionFromNeutral(_currentPositionIndex, _fixedBoneIndex); //rel motion from neutral to current

            //loop for each bone, and setup
            for (int i = 0; i < _bones.Length; i++)
            {
                //skip missing bones
                if (_bones[i] == null) continue;
                                
                //skip if no transforms at all, then the bone is not changing places...
                if (lastRelMotion[i] == null && currentRelMotion[i] == null)
                    continue;

                //if there is no transform for the current position, but there is a reverse
                if (currentRelMotion[i] == null)
                    currentRelMotion[i] = new TransformMatrix(); //set to identity....

                //if the last position was null, then just use the current position. This should happen when moving from neutral
                if (lastRelMotion[i] == null)
                {
                    htRelMotions[i] = currentRelMotion[i].ToHelical();
                }
                else
                {
                    //so we should have both transforms now... though one can be the identity.... hm....
                    TransformMatrix relLastToCurrent = currentRelMotion[i] * lastRelMotion[i].Inverse();
                    htRelMotions[i] = relLastToCurrent.ToHelical();
                }
            }

            //clear the coloring scheme, its not really calculated yet for intermediary positions
            _distMap.clearDistanceColorMapsForAllBones();
            _distMap.clearContoursForAllBones();

            _shortAnimationController.setupAnimationForLinearInterpolation(_bones, htRelMotions, lastRelMotion, numFrames);
            _shortAnimationController.LoopAnimation = false;
            _shortAnimationController.FPS = _FPS;
            _shortAnimationController.Start();
            //TODO: add color information back in at the end....how?
             
             */
        }

        private void applyDistanceMapsIfRequired()
        {
            //load in the color maps, if they already exist
            /*
            if (!_hideMaps)
                _distMap.showDistanceColorMapsForPositionIfCalculatedOrClear(_currentPositionIndex);
            if (!_hideContours)
                _distMap.showContoursForPositionIfCalculatedOrClear(_currentPositionIndex);
             */
        }

        private void setTransformsForCurrentPositionAndFixedBone()
        {
            _fullWrist.HideBonesWithNoKinematics(_currentPositionIndex);

            if (_animatePositionChanges)
            {
                animateChangeInPosition();
            }
            else
            {
                applyDistanceMapsIfRequired();
                
                //first remove the old transforms, if they exist
                _fullWrist.MoveToPositionAndFixedBone(_currentPositionIndex, _fixedBoneIndex);
            }

            //now that we are done, lets save the last positions
            _lastFixedBoneIndex = _fixedBoneIndex;
            _lastPositionIndex = _currentPositionIndex;
        }

        void _control_SelectedSeriesChanged(object sender, SelectedSeriesChangedEventArgs e)
        {
            if (_currentPositionIndex == e.SelectedIndex)
                return;

            _currentPositionIndex = e.SelectedIndex;
            if (_positionGraph != null)
                _positionGraph.setCurrentVisisblePosture(_currentPositionIndex);

            setTransformsForCurrentPositionAndFixedBone();
        }

        void _control_FixedBoneChanged(object sender, FixedBoneChangeEventArgs e)
        {
            _fixedBoneIndex = e.BoneIndex;

            setTransformsForCurrentPositionAndFixedBone();
        }

        void _control_BoneHideChanged(object sender, BoneHideChangeEventArgs e)
        {
            _fullWrist.Bones[e.BoneIndex].SetBoneVisibility(!e.BoneHidden);
        }

        public void setInertiaVisibilityCarpalBones(bool visible)
        {
            setInertiaVisibility(visible, Wrist.CarpalBoneIndexes);
        }

        public void setInertiaVisibilityMetacarpalBones(bool visible)
        {
            setInertiaVisibility(visible, Wrist.MetacarpalBoneIndexes);
        }

        private void setInertiaVisibility(bool visible, int[] boneIndexes) { setInertiaVisibility(visible, boneIndexes, 0); }
        private void setInertiaVisibility(bool visible, int[] boneIndexes, int arrowLength)
        {
            foreach (int i in boneIndexes)
            {
                _fullWrist.Bones[i].SetInertiaVisibility(visible);
            }
        }

        public void setACSVisibility(bool visible)
        {
            int[] forearm = {0, 1};
            setInertiaVisibility(visible, forearm, 45);
        }



        public DialogResult createComplexAnimationMovie()
        {
            string[] positionNames = createSeriesListWithNiceNames();
            AnimationCreatorForm acf = new AnimationCreatorForm(positionNames);
            //Pass in positions....
            DialogResult r = acf.ShowDialog();
            if (r == DialogResult.OK)
                startFullAnimation(acf);

            return r;
        }

        public void endComplexAnimationMovie()
        {
            endFullAnimation();
        }

        private void endFullAnimation()
        {
            //remove switches
            removeAnimationSwitchesFromBones();

            //return GUI
            _layoutControl.removeControl(_animationControl);
            if (_positionGraph != null)
                _layoutControl.addControl(_positionGraph);
            _wristControl.changeBackToNormalMode();

            _wristControl.FixedBoneChanged += new FixedBoneChangedHandler(_control_FixedBoneChanged);
            _wristControl.ShowHamChanged -= new ShowHamChangedHandler(_wristControl_ShowHamChanged);

            _animationControl.TrackbarScroll -= new AnimationControl.TrackbarScrollHandler(_animationControl_TrackbarScroll);
            _animationControl.StopClicked -= new AnimationControl.StopClickedHandler(_animationControl_StopClicked);
            _animationControl.PlayClicked -= new AnimationControl.PlayClickedHandler(_animationControl_PlayClicked);
            _animationControl.FPSChanged -= new AnimationControl.FPSChangedHandler(_animationControl_FPSChanged);
                        
            _animationSwitches = null;
            _animationHamSwitches = null;
            _animationTimer.Tick -= new EventHandler(_animationTimer_Tick);
            _animationTimer.Stop();
            _animationTimer = null;
            _animationControl = null;


            //try and reset the display back to where it was....hopefully everything is reset, so we don't have too?
        }

        private void addAnimationSwitchesToBones()
        {
            /*
            for (int i = 0; i < _bones.Length; i++)
            {
                if (_animationSwitches[i] != null)
                {
                    _bones[i].insertNode(_animationSwitches[i], 0);
                    _animationSwitches[i].whichChild(_animationControl.currentFrame);
                }

                if (_animationHamSwitches[i] != null)
                {
                    _animationHamSwitches[i].reference();
                    if (_wristControl.IsHamVissible(i))
                    {
                        _root.addNode(_animationHamSwitches[i]);
                        _animationHamSwitches[i].whichChild(_animationControl.currentFrame);
                    }
                }
            }
             */
        }

        private void removeAnimationSwitchesFromBones()
        {
            /*
            for (int i = 0; i < _bones.Length; i++)
            {
                //bone transforms
                if (_animationSwitches[i] != null)
                    _bones[i].removeChild(_animationSwitches[i]);

                //ham axes
                if (_wristControl.IsHamVissible(i)) //TODO: Check if displayed...?
                    _root.removeChild(_animationHamSwitches[i]);
                if (_animationHamSwitches[i] != null)
                    _animationHamSwitches[i].unref(); //unref to be deleted
            }
             */
        }

        private void startFullAnimation(AnimationCreatorForm acf)
        {
            /*
            _acf = acf;
            int[] animationOrder = acf.getAnimationOrder();
            int numFrames = acf.NumberStepsPerPositionChange;
            //TODO: all the distance map stuff, etc.
            AnimationCreator ac = new AnimationCreator();
            _animationSwitches = ac.CreateAnimationSwitches(0, _bones, _transformMatrices, animationOrder, numFrames);
            _animationHamSwitches = ac.CreateHAMSwitches(0, _bones, _transformMatrices, _inertiaMatrices, animationOrder, numFrames);

            _animationControl = new AnimationControl();
            _layoutControl.addControl(_animationControl);

            int totalNumFrames = numFrames * (animationOrder.Length - 1) + 1;
            _animationControl.setupController(totalNumFrames);
            _animationControl.FPS = 10;

            //little bit of gui stuff
            if (_layoutControl.Contains(_positionGraph))
                _layoutControl.removeControl(_positionGraph);
            _wristControl.changeToAnimationMode();

            //Okay, at this point, lets remove the current transforms...
            removeCurrentTransforms();
            //now, lets go and add the switches into place
            addAnimationSwitchesToBones();

            //redirect change in fixed bone....
            _wristControl.FixedBoneChanged -= new FixedBoneChangedHandler(_control_FixedBoneChanged);
            _wristControl.setFixedBone(0); //reset the fixed bone to radius for later :)
            _wristControl.selectedSeriesIndex = 0;
            _wristControl.FixedBoneChanged += new FixedBoneChangedHandler(_control_Animation_FixedBoneChanged);
            _wristControl.ShowHamChanged += new ShowHamChangedHandler(_wristControl_ShowHamChanged);

            _animationControl.TrackbarScroll += new AnimationControl.TrackbarScrollHandler(_animationControl_TrackbarScroll);
            _animationControl.StopClicked += new AnimationControl.StopClickedHandler(_animationControl_StopClicked);
            _animationControl.PlayClicked += new AnimationControl.PlayClickedHandler(_animationControl_PlayClicked);
            _animationControl.FPSChanged += new AnimationControl.FPSChangedHandler(_animationControl_FPSChanged);

            _animationTimer = new Timer();
            _animationTimer.Tick += new EventHandler(_animationTimer_Tick);
            _animationTimer.Interval = (int)(1000 / (double)_animationControl.FPS);
             */
        }

        void _wristControl_ShowHamChanged(object sender, BoneHideChangeEventArgs e)
        {
            if (e.BoneHidden)
            {

                //TODO: Check if the bone to be hidden or shown is the fixed bone...fudge
                int index = _animationControl.currentFrame;
                _root.addNode(_animationHamSwitches[e.BoneIndex]);
                _animationHamSwitches[e.BoneIndex].whichChild(index);
            }
            else
            {
                _root.removeChild(_animationHamSwitches[e.BoneIndex]);
            }
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
            /*
            for (int i = 0; i < _bones.Length; i++)
            {
                if (_animationSwitches[i] != null)
                {
                    _animationSwitches[i].whichChild(frameIndex);
                    if (_wristControl.IsHamVissible(i))
                        _animationHamSwitches[i].whichChild(frameIndex);
                }
            }
             */
        }

        void _control_Animation_FixedBoneChanged(object sender, FixedBoneChangeEventArgs e)
        {
            /*
            AnimationCreator ac = new AnimationCreator();
            int[] animationOrder = _acf.getAnimationOrder();
            int numFrames = _acf.NumberStepsPerPositionChange;
            removeAnimationSwitchesFromBones();
            _animationSwitches = ac.CreateAnimationSwitches(e.BoneIndex, _bones, _transformMatrices, animationOrder, numFrames);
            _animationHamSwitches = ac.CreateHAMSwitches(e.BoneIndex, _bones, _transformMatrices, _inertiaMatrices, animationOrder, numFrames);
            addAnimationSwitchesToBones();
             */
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

    }
}
