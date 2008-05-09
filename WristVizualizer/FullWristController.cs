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
        private int _currentPositionIndex;
        private int _lastPositionIndex;
        private int _fixedBoneIndex;
        private int _lastFixedBoneIndex;

        private Separator _root;

        //animation stuff
        private bool _animatePositionChanges;
        private AnimationController _animationController;
        private int _FPS;
        private double _animateDuration;

        //Distance Maps
        private DistanceMaps _distMap;
        private bool _hideMaps;
        private bool _hideContours;

        //Background worker....
        private BackgroundWorkerStatusForm _background;

        //GUI stuff
        private FullWristControl _wristControl;

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
                    _colorBones[i] = new ColoredBone(fname);
                    _bones[i].addNode(_colorBones[i]);
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
        }


        public void calculateDistanceMapsToolClickedHandler()
        {
            //setup the dialog window
            DistanceAndContourDialog dialog = new DistanceAndContourDialog(_distMap.ContourDistances);
            dialog.ColorMapMaxDistance = _distMap.MaxColoredDistance;
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

            //setup background worker...
            //bool readAllColors = dialog.CalculateColorMap == DistanceAndContourDialog.CalculationTypes.All;
            //bool readAllContours = dialog.CalculateContours == DistanceAndContourDialog.CalculationTypes.All;
            //_background = new BackgroundWorkerStatusForm();
            //_background.processDistanceFieldCalculations(_distMap, readAllColors, readAllContours);
            //_background = null;

            //first lets check for color maps
            loadDistanceMaps(dialog.CalculateColorMap, dialog.ColorMapMaxDistance);

            //now lets execute contours...
            loadContours(dialog.CalculateContours, dialog.getContourDistancesToCalculate());
        }

        public void loadDistanceMaps(DistanceAndContourDialog.CalculationTypes whatToLoad, double maxDistance)
        {
            _hideMaps = false; //default
            switch (whatToLoad)
            {
                case DistanceAndContourDialog.CalculationTypes.None:
                    //hide all the maps
                    _hideMaps = true;
                    _distMap.clearDistanceColorMapsForAllBones();
                    break;
                case DistanceAndContourDialog.CalculationTypes.CachedOnly:
                    _distMap.showDistanceColorMapsForPositionIfCalculatedOrClear(_currentPositionIndex);
                    break;  //don't do shit
                case DistanceAndContourDialog.CalculationTypes.Current:
                    _distMap.setMaxColoredDistance(maxDistance);
                    _distMap.showDistanceColorMapsForPosition(_currentPositionIndex);
                    break;
                case DistanceAndContourDialog.CalculationTypes.All:
                    _distMap.setMaxColoredDistance(maxDistance);
                    _distMap.readInAllDistanceColorMaps(_background); //read them all in
                    _distMap.showDistanceColorMapsForPosition(_currentPositionIndex); //make sure we display the current one...
                    break;
                default:
                    throw new ArgumentException("Unknown type of calculation...");
            }
        }

        public void loadContours(DistanceAndContourDialog.CalculationTypes whatToLoad, double[] contourDistances)
        {
            _hideContours = false; //default
            switch (whatToLoad)
            {
                case DistanceAndContourDialog.CalculationTypes.None:
                    //hide contours....
                    _hideContours = true;
                    _distMap.clearContoursForAllBones();
                    break;
                case DistanceAndContourDialog.CalculationTypes.CachedOnly:
                    _distMap.showContoursForPositionIfCalculatedOrClear(_currentPositionIndex);
                    break;  //don't do shit
                case DistanceAndContourDialog.CalculationTypes.Current:
                    _distMap.setContourDistances(contourDistances);
                    _distMap.showContoursForPosition(_currentPositionIndex);
                    break;
                case DistanceAndContourDialog.CalculationTypes.All:
                    _distMap.setContourDistances(contourDistances);
                    _distMap.calculateAllContours(_background);
                    _distMap.showContoursForPosition(_currentPositionIndex);
                    break;
                default:
                    throw new ArgumentException("Unknown type of calculation...");
            }
        }

        private void loadTransforms()
        {
            int numPos = _wrist.motionFiles.Length;
            _transformMatrices = new TransformMatrix[numPos][];

            for (int i = 0; i < numPos; i++)
            {
                _transformMatrices[i] = new TransformMatrix[Wrist.NumBones];
                //TODO: create DatParser.parseMotionFileToTM()
                TransformRT[] tfm = DatParser.parseMotionFileToRT(_wrist.motionFiles[i]);
                for (int j = 0; j < Wrist.NumBones; j++)
                {
                    _transformMatrices[i][j] = new TransformMatrix(tfm[j]);
                }
            }
        }

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
            get { return _wristControl; }
        }

        public override Separator Root
        {
            get { return _root; }
        }

        private void populateSeriesList()
        {
            _currentPositionIndex = 0;
            _wristControl.clearSeriesList();
            _wristControl.addToSeriesList(_wrist.neutralSeries);
            _wristControl.addToSeriesList(_wrist.series);
            _wristControl.selectedSeriesIndex = 0;
        }

        public string getTitleCaption()
        {
            return _wrist.subject + _wrist.side + " - " + _wrist.subjectPath;
        }

        public string getFilenameOfFirstFile()
        {
            return Path.Combine(_wrist.subjectPath, _wrist.subject + _wrist.side);
        }

        private void setupControl()
        {
            _wristControl = new FullWristControl();
            _wristControl.setupControl(Wrist.LongBoneNames, true);
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
        }

        private void removeCurrentTransforms()
        {
            if (_root.hasTransform())
                _root.removeTransform();
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
            if (_animationController != null)
                _animationController.Stop(); //ugly, but should work
            _animationController = new AnimationController();
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

            _animationController.setupAnimationForLinearInterpolation(_bones, htRelMotions, lastRelMotion, numFrames);
            _animationController.LoopAnimation = false;
            _animationController.FPS = _FPS;
            _animationController.Start();
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

        private void setTransformsForCurrentPositionAndFixedBone()
        {
            hideBonesWithNoKinematicsForPosition(_currentPositionIndex);
            if (_animatePositionChanges)
            {
                animateChangeInPosition();
            }
            else
            {
                //load in the color maps, if they already exist
                if (!_hideMaps)
                    _distMap.showDistanceColorMapsForPositionIfCalculatedOrClear(_currentPositionIndex);
                if (!_hideContours)
                    _distMap.showContoursForPositionIfCalculatedOrClear(_currentPositionIndex);
                
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

            setTransformsForCurrentPositionAndFixedBone();
        }

        void _control_FixedBoneChanged(object sender, FixedBoneChangeEventArgs e)
        {
            _fixedBoneIndex = e.BoneIndex;

            setTransformsForCurrentPositionAndFixedBone();
        }

        void _control_BoneHideChanged(object sender, BoneHideChangeEventArgs e)
        {
            _colorBones[e.BoneIndex].setHidden(e.BoneHidden);
        }

        public void setInertiaVisibility(bool visible)
        {
            if (visible)
            {
                try
                {
                    //If its checked, then we need to add it
                    TransformRT[] inert = DatParser.parseInertiaFileToRT(_wrist.inertiaFile);
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
                    throw new WristVizualizerException(msg, ex);
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

        public void setACSVisibility(bool visible)
        {
            if (visible)
            {
                try
                {
                    //If its checked, then we need to add it
                    TransformRT[] inert = DatParser.parseACSFileToRT(_wrist.acsFile);

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
                    throw new WristVizualizerException(msg, ex);
                }
            }
            else
            {
                _bones[0].removeChild(_inertias[0]);
                _inertias[0] = null;
            }
        }

    }
}
