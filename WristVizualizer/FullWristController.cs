using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using libWrist;
using libCoin3D;

namespace WristVizualizer
{    
    class FullWristController : Controller
    {
        private bool _showErrors = false;
        private ColoredBone[] _colorBones;
        private Separator[] _bones;
        private Separator[] _inertias;
        private Wrist _wrist;
        private TransformMatrix[][] _transformMatrices;
        private TransformMatrix[] _inertiaMatrices;
        private int _currentPositionIndex;
        private int _lastPositionIndex;
        private int _fixedBoneIndex;
        private int _lastFixedBoneIndex;

        private Separator _root;

        //animation stuff
        private bool _animatePositionChanges;
        private ShortAnimationController _shortAnimationController;
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

        public FullWristController()
        {
            setupControl();
            setupControlEventListeners();
            _root = new Separator();
            _bones = new Separator[Wrist.NumBones];
            _colorBones = new ColoredBone[Wrist.NumBones];
            _inertias = new Separator[Wrist.NumBones];

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
            try
            {
                _wrist.setupWrist(radiusFilename);
                loadTransforms();
                populateSeriesList();
            }
            catch (ArgumentException ex)
            {
                if (_showErrors)
                {
                    string msg = "Error loading wrist kinematics.\n\n" + ex.Message;
                    //TODO: Change to abort,retry, and find way of cancelling load
                    MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                for (int i = 0; i < Wrist.NumBones; i++)
                    _wristControl.disableFixingBone(i);
            }

            for (int i = 0; i < Wrist.NumBones; i++)
            {
                string fname = _wrist.bpaths[i];
                if (File.Exists(fname))
                {
                    _bones[i] = new Separator();
                    _bones[i].makeHideable();
                    try
                    {
                        _colorBones[i] = new ColoredBone(fname);
                        _bones[i].addNode(_colorBones[i]);
                    }
                    catch (System.ArgumentException)
                    {
                        //try and load non-standard bones here.... shit. This needs to be fixed
                        //TODO: Better error handling...
                        _colorBones[i] = null;
                        _bones[i].addFile(fname);
                    }
                    _root.addChild(_bones[i]);
                }
                else
                {
                    _bones[i] = null;
                    _wristControl.disableBone(i);
                }
            }

            //create empty distance map strucutre
            _distMap = new DistanceMaps(_wrist, _transformMatrices, _colorBones);

            //try and load the inertialInformation
            loadInertiaAndACSData();
            setupPositionGraphIfPossible(8); //hardcoded default reference bone to the Capitate
        }

        private bool hasPositionInformation(int referenceBoneIndex)
        {
            //only need to check the ACS and the capitate (index 8)
            if (_inertiaMatrices[0] == null || _inertiaMatrices[0].Determinant() == 0 ||
                _inertiaMatrices[referenceBoneIndex] == null || _inertiaMatrices[referenceBoneIndex].Determinant() == 0)
                return false;
            else
                return true;
        }

        private bool hasPronationSupinationInformation()
        {
            //Check for existance of the actual AnatCoordSys_uln.dat file; else this information is probably wrong...
            if (!File.Exists(_wrist.acsFile_uln))
                return false;

            //need to check for the Radius and the Ulna, (index 0 & 1)
            if (_inertiaMatrices[0] == null || _inertiaMatrices[0].Determinant() == 0 ||
                _inertiaMatrices[1] == null || _inertiaMatrices[1].Determinant() == 0)
                return false;
            else
                return true;
        }

        public void changeWristPositionReferenceBoneIndex(int referenceBoneIndex)
        {
            //first lets remove any existing position graph
            removeExistingPositionGraph();

            //now lets setup for the new reference bone
            setupPositionGraphIfPossible(referenceBoneIndex);
        }

        private void removeExistingPositionGraph()
        {
            if (_positionGraph == null) return;
            _positionGraph.SelectedSeriesChanged -= new SelectedSeriesChangedHandler(_positionGraph_SelectedSeriesChanged);
            _layoutControl.removeControl(_positionGraph);
            _positionGraph = null;
        }

        private void setupPositionGraphIfPossible(int referenceBoneIndex)
        {
            if (!hasPositionInformation(referenceBoneIndex))
                return;

            _positionGraph = new PositionGraph(_inertiaMatrices, _transformMatrices, referenceBoneIndex);
            if (!hasPronationSupinationInformation())
                _positionGraph.HidePS();
            _positionGraph.setCurrentVisisblePosture(_currentPositionIndex); //make sure the correct position is highlighted
            _layoutControl.addControl(_positionGraph);
            _positionGraph.SelectedSeriesChanged += new SelectedSeriesChangedHandler(_positionGraph_SelectedSeriesChanged);
        }

        void _positionGraph_SelectedSeriesChanged(object sender, SelectedSeriesChangedEventArgs e)
        {
            //update the visible control, that will send out another event and make the actual position change :)
            _wristControl.selectedSeriesIndex = e.SelectedIndex;
        }

        private void loadInertiaAndACSData()
        {
            _inertiaMatrices = new TransformMatrix[Wrist.NumBones];
            if (File.Exists(_wrist.inertiaFile))
            {
                try
                {
                    TransformRT[] inert = DatParser.parseInertiaFileToRT(_wrist.inertiaFile);
                    for (int i = 0; i < Wrist.NumBones; i++) //skip the long bones
                    {
                        //No longer check for missing bone... not important here, already read the data, just assigning it
                        //if (_bones[i] == null)
                        //    continue;

                        _inertiaMatrices[i] = new TransformMatrix(inert[i]);
                    }
                }
                catch { }
            }

            if (File.Exists(_wrist.acsFile))
            {
                try
                {
                    //If its checked, then we need to add it
                    TransformRT[] acs = DatParser.parseACSFileToRT(_wrist.acsFile);

                    //only for radius, check if it exists
                    //if (_bones[0] == null)
                    //    return;
                    _inertiaMatrices[0] = new TransformMatrix(acs[0]);
                }
                catch { }
            }

            if (File.Exists(_wrist.acsFile_uln))
            {
                try
                {
                    //If its checked, then we need to add it
                    TransformRT[] acs = DatParser.parseACSFileToRT(_wrist.acsFile_uln);

                    //only for radius, check if it exists
                    //if (_bones[1] == null)
                    //    return;
                    _inertiaMatrices[1] = new TransformMatrix(acs[0]);
                }
                catch { }
            }
        }

        public void calculateDistanceMapsToolClickedHandler()
        {
            //setup the dialog window
            DistanceAndContourDialog dialog = new DistanceAndContourDialog(_distMap.ContourDistances);
            dialog.ColorMapMaxDistance = _distMap.MaxColoredDistance;
            dialog.setContourColors(_distMap.ContourColors);
            if (_hideMaps)
                dialog.CalculateColorMap = DistanceAndContourDialog.CalculationTypes.None;
            else if (_distMap.hasDistanceColorMapsForPosition(_currentPositionIndex))
                dialog.CalculateColorMap = DistanceAndContourDialog.CalculationTypes.CachedOnly;
            else 
                dialog.CalculateColorMap = DistanceAndContourDialog.CalculationTypes.Current;

            if (_hideContours)
                dialog.CalculateContours = DistanceAndContourDialog.CalculationTypes.None;
            else if (_distMap.hasContourForBonePosition(0, _currentPositionIndex)) //check the radius only
                dialog.CalculateContours = DistanceAndContourDialog.CalculationTypes.CachedOnly;
            else
                dialog.CalculateContours = DistanceAndContourDialog.CalculationTypes.Current;

            //show the dialog window
            DialogResult r = dialog.ShowDialog();
            if (r != DialogResult.OK)
                return;

            calculateDistanceMapsHelper(dialog);
        }

        private void calculateDistanceMapsHelper(DistanceAndContourDialog dialog)
        {
            //set hidden variables
            _hideMaps = dialog.HideColorMap;
            _hideContours = dialog.HideContour;

            //apply new values if we need to
            if (dialog.RequiresCalculatingColorMaps)
                _distMap.setMaxColoredDistance(dialog.ColorMapMaxDistance);
            if (dialog.RequiresCalculatingContours)
                _distMap.setContourDistances(dialog.getContourDistancesToCalculate(), dialog.getContourColorsToCalculate());

            //setup background worker... to process loading....
            bool readAllColors = dialog.CalculateAllColorMaps;
            bool readAllContours = dialog.CalculateAllContours;

            _distMap.addToColorMapQueue(_currentPositionIndex, readAllColors, dialog.CalculateCurrentColorMap);
            _distMap.addToContourQueue(_currentPositionIndex, readAllContours, dialog.CalculateCurrentContour);
            _distMap.processAllPendingQueues();
            
            //they have all been calculated, lets apply them so they are visisble.
            applyDistanceMapsIfRequired();
        }

        private void loadTransforms()
        {
            int numPos = _wrist.motionFiles.Length;
            _transformMatrices = new TransformMatrix[numPos][];

            for (int i = 0; i < numPos; i++)
            {
                _transformMatrices[i] = DatParser.parseMotionFileToTransformMatrix(_wrist.motionFiles[i]);
            }
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

        private void removeCurrentTransforms()
        {
            for (int i = 0; i < _bones.Length; i++)
            {
                //skip missing bones & remove the old
                if (_bones[i] != null && _bones[i].hasTransform())
                    _bones[i].removeTransform();
            }
        }

        private TransformMatrix[] calculateRelativeMotionFromNeutral(int positionIndex, int fixedBoneIndex)
        {
            TransformMatrix[] relMotions = new TransformMatrix[Wrist.NumBones];

            //check if neutral, if so, do something special....fuck?
            if (positionIndex == 0)
                return relMotions;  //okay, for now return null for all....

            TransformMatrix tmFixedBone = _transformMatrices[positionIndex - 1][fixedBoneIndex];
            for (int i = 0; i < _bones.Length; i++)
            {
                //skip missing bones
                if (_bones[i] == null) 
                    continue;

                //do we need to check if we are the fixed bone....?
                if (i == fixedBoneIndex)
                    continue;

                TransformMatrix tmCurrentBone = _transformMatrices[positionIndex - 1][i];
                relMotions[i] = tmFixedBone.Inverse() * tmCurrentBone;
            }
            return relMotions;
        }

        private void animateChangeInPosition()
        {
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
        }

        private void hideBonesWithNoKinematicsForPosition(int positionIndex)
        {
            if (positionIndex==0)
                return; //no bone can be missing in neutral

            for (int i=0; i<_bones.Length; i++) {
            //only check if the bone exists
            if (_bones[i]==null)
                continue;

                if (_transformMatrices[positionIndex-1][i].isIdentity())
                    _wristControl.hideBone(i); //send to the control, so the GUI gets updated, it will call back to the controller to actually hide the bone :)
            }
        }

        private void applyDistanceMapsIfRequired()
        {
            //load in the color maps, if they already exist
            if (!_hideMaps)
                _distMap.showDistanceColorMapsForPositionIfCalculatedOrClear(_currentPositionIndex);
            if (!_hideContours)
                _distMap.showContoursForPositionIfCalculatedOrClear(_currentPositionIndex);
        }

        private void setTransformsForCurrentPositionAndFixedBone()
        {
            hideBonesWithNoKinematicsForPosition(_currentPositionIndex);
            if (_animatePositionChanges)
            {
                animateChangeInPosition();
            }
            else
            {
                applyDistanceMapsIfRequired();
                
                //first remove the old transforms, if they exist
                removeCurrentTransforms();

                //save to current positions, then apply
                TransformMatrix[] transforms = calculateRelativeMotionFromNeutral(_currentPositionIndex, _fixedBoneIndex);
                for (int i = 0; i < _bones.Length; i++)
                {
                    //skip missing bones, or bones without motion
                    if (_bones[i] == null || transforms[i] == null)
                        continue;

                    _bones[i].addTransform(transforms[i].ToTransform());
                }

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
            if (_colorBones[e.BoneIndex] != null)
                _colorBones[e.BoneIndex].setHidden(e.BoneHidden);
            if (e.BoneHidden)
                _bones[e.BoneIndex].hide();
            else
                _bones[e.BoneIndex].show();
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
            if (visible)
            {
                bool continueOnError = false;
                foreach (int i in boneIndexes)
                {
                    if (_bones[i] == null)
                        continue;

                    //check that it exists:
                    if (_inertiaMatrices[i] == null)
                    {
                        MessageBox.Show("Unable to show inertial axes. Error reading file.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (_inertiaMatrices[i].Determinant() == 0)
                    {
                        if (continueOnError) continue;
                        string msg = String.Format("Invalid inertial axis for '{0}'. Determinant==0!\n\nDo you wish to hide this error message and continue anyway?", Wrist.LongBoneNames[i]);
                        DialogResult r = MessageBox.Show(msg, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (r == DialogResult.No) return;

                        continueOnError = true;
                        continue;

                    }

                    _inertias[i] = new Separator();
                    Transform t = _inertiaMatrices[i].ToTransform();
                    if (arrowLength == 0)
                        _inertias[i].addNode(new ACS());
                    else
                        _inertias[i].addNode(new ACS(45));
                    _inertias[i].addTransform(t);
                    _bones[i].addChild(_inertias[i]);
                }
            }
            else
            {
                //so we want to remove the inertia files
                foreach (int i in boneIndexes)
                {
                    if (_inertias[i] != null) _bones[i].removeChild(_inertias[i]);
                    _inertias[i] = null;
                }

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
            

            //remove switches
            for (int i = 0; i < _bones.Length; i++)
            {
                if (_animationSwitches[i] != null)
                    _bones[i].removeChild(_animationSwitches[i]);
            }
            _animationSwitches = null;
            _animationTimer.Tick -= new EventHandler(_animationTimer_Tick);
            _animationTimer.Stop();
            _animationTimer = null;
            _animationControl = null;


            //try and reset the display back to where it was....hopefully everything is reset, so we don't have too?
        }

        private void startFullAnimation(AnimationCreatorForm acf)
        {
            int[] animationOrder = acf.getAnimationOrder();
            int numFrames = acf.NumberStepsPerPositionChange;
            //TODO: all the distance map stuff, etc.
            AnimationCreator ac = new AnimationCreator();
            _animationSwitches = ac.CreateAnimationSwitches(_bones, _transformMatrices, animationOrder, numFrames);
            _animationHamSwitches = ac.CreateHAMSwitches(_bones, _transformMatrices, _inertiaMatrices, animationOrder, numFrames);

            //Okay, at this point, lets remove the current transforms...
            removeCurrentTransforms();
            //now, lets go and add the switches into place
            for (int i = 0; i < _bones.Length; i++)
            {
                if (_animationSwitches[i] != null)
                {
                    _bones[i].insertNode(_animationSwitches[i], 0);
                    _animationSwitches[i].whichChild(0);
                    _root.addNode(_animationHamSwitches[i]);
                    _animationHamSwitches[i].hideAll();
                }
            }

            //little bit of gui stuff
            if (_layoutControl.Contains(_positionGraph))
                _layoutControl.removeControl(_positionGraph);
            _wristControl.changeToAnimationMode();

            //redirect change in fixed bone....
            _wristControl.FixedBoneChanged -= new FixedBoneChangedHandler(_control_FixedBoneChanged);
            _wristControl.setFixedBone(0); //reset the fixed bone to radius for later :)
            _wristControl.selectedSeriesIndex = 0;
            _wristControl.FixedBoneChanged += new FixedBoneChangedHandler(_control_Animation_FixedBoneChanged);
            _wristControl.ShowHamChanged += new ShowHamChangedHandler(_wristControl_ShowHamChanged);

            _animationControl = new AnimationControl();
            _layoutControl.addControl(_animationControl);

            int totalNumFrames = numFrames * (animationOrder.Length - 1) + 1;
            _animationControl.setupController(totalNumFrames);
            _animationControl.FPS = 10;

            _animationControl.TrackbarScroll += new AnimationControl.TrackbarScrollHandler(_animationControl_TrackbarScroll);
            _animationControl.StopClicked += new AnimationControl.StopClickedHandler(_animationControl_StopClicked);
            _animationControl.PlayClicked += new AnimationControl.PlayClickedHandler(_animationControl_PlayClicked);
            _animationControl.FPSChanged += new AnimationControl.FPSChangedHandler(_animationControl_FPSChanged);

            _animationTimer = new Timer();
            _animationTimer.Tick += new EventHandler(_animationTimer_Tick);
            _animationTimer.Interval = (int)(1000 / (double)_animationControl.FPS);
        }

        void _wristControl_ShowHamChanged(object sender, BoneHideChangeEventArgs e)
        {
            if (e.BoneHidden)
            {
                int index = _animationControl.currentFrame;
                _animationHamSwitches[e.BoneIndex].whichChild(index);
            }
            else
            {
                _animationHamSwitches[e.BoneIndex].hideAll();
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
            for (int i = 0; i < _bones.Length; i++)
            {
                if (_animationSwitches[i] != null)
                {
                    _animationSwitches[i].whichChild(index);
                    if (_wristControl.IsHamVissible(i))
                        _animationHamSwitches[i].whichChild(index);
                }
            }
        }

        void _control_Animation_FixedBoneChanged(object sender, FixedBoneChangeEventArgs e)
        {
            Console.WriteLine("Fixed bone changed to {0}",e.BoneIndex);
            //TODO: Yeah...something!
        }

    }
}
